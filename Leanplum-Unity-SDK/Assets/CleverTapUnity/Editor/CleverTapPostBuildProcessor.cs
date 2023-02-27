using System;
using System.IO;
using UnityEditor;


#if UNITY_IOS && UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class CleverTapPostBuildProcessor
{

	private static string CLEVERTAP_ACCOUNT_ID = "";
	private static string CLEVERTAP_ACCOUNT_TOKEN = "";
	private static string CLEVERTAP_ACCOUNT_REGION = "";
	private static bool CLEVERTAP_ENABLE_PERSONALIZATION = true;

	private static string rn = "\n";

	private static string CODE_LIB_IMPORT = 
	"#import \"CleverTapUnityManager.h\"" + rn;

	private static string CODE_USER_NOTIFICATIONS_IMPORT =
	"#import <UserNotifications/UserNotifications.h>" + rn;

	private static string PATH_CONTROLLER = "/Classes/UnityAppController.mm";

	private static string SIGNATURE_URL = 
		"- (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation";
	private static string CODE_URL = rn + 
        "    [[CleverTapUnityManager sharedInstance] handleOpenURL:url sourceApplication:sourceApplication];" + rn ;

	private static string SIGNATURE_PUSH_TOKEN = 
		"- (void)application:(UIApplication*)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken";
	private static string CODE_PUSH_TOKEN = rn + 
		"    [[CleverTapUnityManager sharedInstance] setPushToken:deviceToken];" + rn ;

	private static string SIGNATURE_DID_FINISH_LAUNCH = 
		"- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions";

	private static string SIGNATURE_NOTIF_LOCAL = 
		"- (void)application:(UIApplication*)application didReceiveLocalNotification:(UILocalNotification*)notification";
	private static string SIGNATURE_NOTIF_REMOTE = 
		"- (void)application:(UIApplication*)application didReceiveRemoteNotification:(NSDictionary*)userInfo";
	private static string SIGNATURE_NOTIF_REMOTE_BG = 
		"- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))handler";
	private static string CODE_NOTIF_LOCAL = rn + 
		"    [[CleverTapUnityManager sharedInstance] registerApplication:application didReceiveRemoteNotification:notification.userInfo];" + rn ;
	private static string CODE_NOTIF = rn + 
		"    [[CleverTapUnityManager sharedInstance] registerApplication:application didReceiveRemoteNotification:userInfo];" + rn ;
	private static string CODE_ENABLE_PERSONALIZATION = rn + 
		"    [CleverTapUnityManager enablePersonalization];" + rn ;
	private static string CODE_ADD_USER_NOTIFICATION_FRAMEWORK = rn +
		"    [UNUserNotificationCenter currentNotificationCenter].delegate = (id <UNUserNotificationCenterDelegate>)self;" + rn ;

	private enum Position { Begin, End };

	[PostProcessBuild(99)] 
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		if (target == BuildTarget.iOS) {
			string projPath = PBXProject.GetPBXProjectPath(path);
			PBXProject proj = new PBXProject();
			proj.ReadFromString(File.ReadAllText(projPath));

			#if UNITY_2019_3_OR_NEWER
			      var projectTarget = proj.GetUnityFrameworkTargetGuid();
			#else
			      var projectTarget = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
			#endif

			// Add dependencies
			proj.AddFrameworkToProject(projectTarget, "SystemConfiguration.framework", false);
			proj.AddFrameworkToProject(projectTarget, "CoreTelephony.framework", false);
			proj.AddFrameworkToProject(projectTarget, "CoreLocation.framework", false);
			proj.AddFrameworkToProject(projectTarget, "Security.framework", false);
            proj.AddFrameworkToProject(projectTarget, "UserNotifications.framework", false);

            // Add linker flags
            proj.AddBuildProperty(projectTarget, "OTHER_LDFLAGS", "-ObjC");

			File.WriteAllText(projPath, proj.WriteToString());

			// Update plist
			string plistPath = path + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			// Get root
			PlistElementDict rootDict = plist.root;

			// write CleverTapAccountID and CleverTapToken
			rootDict.SetString("CleverTapAccountID", CLEVERTAP_ACCOUNT_ID);
			rootDict.SetString("CleverTapToken", CLEVERTAP_ACCOUNT_TOKEN);
			rootDict.SetString("CleverTapRegion", CLEVERTAP_ACCOUNT_REGION);
			
			// Write Disable IDFV Flag
			rootDict.SetBoolean("CleverTapDisableIDFV", EditorPrefs.GetBool("CLEVERTAP_DISABLE_IDFV"));

			// Write to file
			File.WriteAllText(plistPath, plist.WriteToString());

			// Update AppController
			InsertCodeIntoControllerClass(path);
		}
	}

	private static void InsertCodeIntoControllerClass(string projectPath) {
		string filepath = projectPath + PATH_CONTROLLER;
		string[] methodSignatures = {SIGNATURE_PUSH_TOKEN, SIGNATURE_URL, SIGNATURE_NOTIF_LOCAL, SIGNATURE_NOTIF_REMOTE, SIGNATURE_NOTIF_REMOTE_BG};
		string[] valuesToAppend = {CODE_PUSH_TOKEN, CODE_URL, CODE_NOTIF_LOCAL, CODE_NOTIF, CODE_NOTIF};
		Position[] positionsInMethod = new Position[]{Position.End, Position.Begin, Position.End, Position.End, Position.Begin};
		InsertCodeIntoClass (filepath, methodSignatures, valuesToAppend, positionsInMethod);

		string[] methodSignaturesRegPush = { SIGNATURE_DID_FINISH_LAUNCH };
		if (CLEVERTAP_ENABLE_PERSONALIZATION) {
            string[] valuesToAppendRegPush = {CODE_ENABLE_PERSONALIZATION};
			Position[] positionsInMethodRegPush = new Position[]{Position.Begin};
			InsertCodeIntoClass (filepath, methodSignaturesRegPush, valuesToAppendRegPush, positionsInMethodRegPush);
		}

		string[] valuesToAppendUserNotifications = {CODE_ADD_USER_NOTIFICATION_FRAMEWORK};
		Position[] positionsInMethodRegUserNotifications = new Position[] { Position.Begin };
		InsertCodeIntoClass(filepath, methodSignaturesRegPush, valuesToAppendUserNotifications, positionsInMethodRegUserNotifications);

	}

	private static void InsertCodeIntoClass(string filepath, string[] methodSignatures, string[] valuesToAppend, Position[]positionsInMethod) {
		if (!File.Exists (filepath)) {
			return;
		}

		string fileContent = File.ReadAllText (filepath);
		List<int> ignoredIndices = new List<int> ();

		for (int i = 0; i < valuesToAppend.Length; i++) {
			string val = valuesToAppend [i];

			if (fileContent.Contains (val)) {
				ignoredIndices.Add (i);
			}
		}

		string[] fileLines = File.ReadAllLines(filepath);
		List<string> newContents = new List<string>();
		bool found = false;   
		int foundIndex = -1;

		newContents.Add (CODE_LIB_IMPORT);
		newContents.Add (CODE_USER_NOTIFICATIONS_IMPORT);

		foreach (string line in fileLines) {
			if (line.Trim().Contains(CODE_USER_NOTIFICATIONS_IMPORT.Trim())){
				continue;
			}

			if (line.Trim().Contains(CODE_LIB_IMPORT.Trim()))
			{
				continue;
			}

			newContents.Add(line + rn);
			for(int j = 0;j<methodSignatures.Length; j++) {
				if ((line.Trim().Equals(methodSignatures[j])) && !ignoredIndices.Contains(j)){
					foundIndex = j;
					found = true;
				}
			}

			if(found) {
				if((positionsInMethod[foundIndex] == Position.Begin) && line.Trim().Equals("{")){
					newContents.Add(valuesToAppend[foundIndex] + rn);
					found = false;
				} else if((positionsInMethod[foundIndex] == Position.End) && line.Trim().Equals("}")) {
					newContents = newContents.GetRange(0, newContents.Count - 1);
					newContents.Add(valuesToAppend[foundIndex] + rn + "}" + rn);
					found = false;
				}
			}
		}
		string output = string.Join("", newContents.ToArray());
		File.WriteAllText(filepath, output);
	}
}
#endif
