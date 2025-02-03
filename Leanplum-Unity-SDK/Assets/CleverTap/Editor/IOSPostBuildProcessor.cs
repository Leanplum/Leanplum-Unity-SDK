#if UNITY_IOS && UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace CleverTapSDK.Private
{
    public static class IOSPostBuildProcessor
	{
        [PostProcessBuild(99)]
		public static void OnPostProcessBuild(BuildTarget target, string path)
		{
			if (target == BuildTarget.iOS)
			{
				IOSPostProcess(path);
			}
		}

        private static void IOSPostProcess(string path)
        {
            string projPath = PBXProject.GetPBXProjectPath(path);
            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));

#if UNITY_2019_3_OR_NEWER
            var projectTarget = proj.GetUnityFrameworkTargetGuid();
#else
			      var projectTarget = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif

            // Add linker flags
            proj.AddBuildProperty(projectTarget, "OTHER_LDFLAGS", "-ObjC");

            CleverTapSettings settings = AssetDatabase.LoadAssetAtPath<CleverTapSettings>(CleverTapSettings.settingsPath);
            if (settings != null)
            {
                UpdatePlistWithSettings(path, settings);

                UpdatePreprocessorMacros(proj, projectTarget, settings);
            }
            else
            {
                Debug.Log($"CleverTapSettings have not been set.\n" +
                    $"Please update them from {CleverTapSettingsWindow.ITEM_NAME} or " +
                    $"set them manually in the project Info.plist.");
            }

            AddCleverTapFolder(path, proj);

            File.WriteAllText(projPath, proj.WriteToString());
        }

        /// <summary>
        /// Updates the "GCC_PREPROCESSOR_DEFINITIONS" build property.
        /// Adds or removes preprocessor macros of the project Build Settings.
        /// </summary>
        /// <param name="proj">The project to update.</param>
        /// <param name="projectTarget">The project target guid.</param>
        /// <param name="settings">The CleverTapSettings to use.</param>
        private static void UpdatePreprocessorMacros(PBXProject proj, string projectTarget, CleverTapSettings settings)
        {
            // The UpdateBuildProperty set the property value if no values are present. This overrides the $(inherited) value.
            // Add the $(inherited) value as a workaround.
            string preprocessorMacros = "GCC_PREPROCESSOR_DEFINITIONS";
            if (!settings.CleverTapIOSUseAutoIntegrate)
            {
                proj.UpdateBuildProperty(projectTarget, preprocessorMacros, new string[] { "$(inherited)", "NO_AUTOINTEGRATE" }, null);
            }
            else
            {
                proj.UpdateBuildProperty(projectTarget, preprocessorMacros, null, new string[] { "NO_AUTOINTEGRATE" });
            }

            if (!settings.CleverTapIOSUseUNUserNotificationCenter)
            {
                proj.UpdateBuildProperty(projectTarget, preprocessorMacros, new string[] { "$(inherited)", "NO_UNUSERNOTIFICATIONCENTER" }, null);
            }
            else
            {
                proj.UpdateBuildProperty(projectTarget, preprocessorMacros, null, new string[] { "NO_UNUSERNOTIFICATIONCENTER" });
            }
        }

        /// <summary>
        /// Writes the CleverTapSettings (account id, token etc.) to the Info.plist file.
        /// </summary>
        /// <param name="path">The project path.</param>
        /// <param name="settings">The settings to use.</param>
        private static void UpdatePlistWithSettings(string path, CleverTapSettings settings)
        {
            if (settings == null)
                return;

            string plistPath = Path.Combine(path, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            PlistElementDict rootDict = plist.root;
            rootDict.SetString("CleverTapAccountID", settings.CleverTapAccountId);
            rootDict.SetString("CleverTapToken", settings.CleverTapAccountToken);
            if (!string.IsNullOrWhiteSpace(settings.CleverTapAccountRegion))
            {
                rootDict.SetString("CleverTapRegion", settings.CleverTapAccountRegion);
            }
            if (!string.IsNullOrWhiteSpace(settings.CleverTapProxyDomain))
            {
                rootDict.SetString("CleverTapProxyDomain", settings.CleverTapProxyDomain);
            }
            if (!string.IsNullOrWhiteSpace(settings.CleverTapSpikyProxyDomain))
            {
                rootDict.SetString("CleverTapSpikyProxyDomain", settings.CleverTapSpikyProxyDomain);
            }

            rootDict.SetBoolean("CleverTapDisableIDFV", settings.CleverTapDisableIDFV);

            rootDict.SetBoolean("CleverTapPresentNotificationForeground", settings.CleverTapIOSPresentNotificationOnForeground);

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }

		/// <summary>
		/// Copies the CleverTap folder (Assets/CleverTap) to the iOS exported project.
		/// The copied folder has "Build Rules" - "Apply Once to Folder".
		/// The folder is added to the "Copy Bundle Resources" Build Phase.
		/// The folder is with "Location" - "Relative to Group" and appears blue in Xcode.
		/// The folder has "Target Membership" the main target - "Unity-iPhone".
		/// </summary>
		/// <remarks>
		/// The folder must be copied with a different name than the original one in the Assets (CleverTap -> CleverTapSDK),
		/// otherwise its contents do not appear correctly in Xcode but will still be copied into the bundle.
		/// </remarks>
		/// <param name="path">The project path.</param>
		/// <param name="proj">The Xcode project.</param>
        private static void AddCleverTapFolder(string path, PBXProject proj)
        {
			// Copy CleverTap folder
            string sourceFolderPath = Path.Combine(Application.dataPath, EditorUtils.CLEVERTAP_ASSETS_FOLDER);
            string destinationFolderPath = Path.Combine(path, EditorUtils.CLEVERTAP_APP_ASSETS_FOLDER);
            EditorUtils.DirectoryCopy(sourceFolderPath, destinationFolderPath,
                true, true, new System.Collections.Generic.HashSet<string>() { EditorUtils.CLEVERTAP_CUSTOM_TEMPLATES_FOLDER });

            string mainTargetGuid = proj.GetUnityMainTargetGuid();
            // Add CleverTap folder reference and target membership
            string folderGuid = proj.AddFolderReference(destinationFolderPath, EditorUtils.CLEVERTAP_APP_ASSETS_FOLDER);
            proj.AddFileToBuild(mainTargetGuid, folderGuid);
        }
	}
}
#endif