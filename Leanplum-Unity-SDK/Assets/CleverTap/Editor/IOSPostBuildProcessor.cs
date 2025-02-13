#if UNITY_IOS && UNITY_EDITOR
using System.Collections.Generic;
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

            // Update project based on CleverTap Settings
            CleverTapSettings settings = AssetDatabase.LoadAssetAtPath<CleverTapSettings>(CleverTapSettings.settingsPath);
            UpdateProjectWithSettings(path, proj, projectTarget, settings);

            // Copy Assets to iOS project
            CopyAssetsToIOSResources(path, proj);

            File.WriteAllText(projPath, proj.WriteToString());
        }

        private static void UpdateProjectWithSettings(string path, PBXProject proj, string projectTarget, CleverTapSettings settings)
        {
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
            if (!string.IsNullOrWhiteSpace(settings.CleverTapAccountId))
            {
                rootDict.SetString("CleverTapAccountID", settings.CleverTapAccountId);
            }
            else
            {
                Debug.Log($"CleverTapAccountID has not been set.\n" +
                    $"SDK initialization will fail without this. " +
                    $"Please set it from {CleverTapSettingsWindow.ITEM_NAME} or " +
                    $"manually in the project Info.plist.");
            }

            if (!string.IsNullOrWhiteSpace(settings.CleverTapAccountToken))
            {
                rootDict.SetString("CleverTapToken", settings.CleverTapAccountToken);
            }
            else
            {
                Debug.Log($"CleverTapToken has not been set.\n" +
                    $"SDK initialization will fail without this. " +
                    $"Please set it from {CleverTapSettingsWindow.ITEM_NAME} or " +
                    $"manually in the project Info.plist.");
            }

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
        private static void CopyAssetsToIOSResources(string path, PBXProject proj)
        {
            string sourceFolderPath = Path.Combine(Application.dataPath, EditorUtils.CLEVERTAP_ASSETS_FOLDER);
            if (!Directory.Exists(sourceFolderPath))
            {
                return;
            }

            // Copy CleverTap folder
            string destinationFolderPath = Path.Combine(path, EditorUtils.CLEVERTAP_APP_ASSETS_FOLDER);
            HashSet<string> folderNamesToCopy = new HashSet<string>() { EditorUtils.CLEVERTAP_CUSTOM_TEMPLATES_FOLDER };
            try
            {
                EditorUtils.DirectoryCopy(sourceFolderPath, destinationFolderPath, true, true, folderNamesToCopy);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to Copy assets to iOS project. " +
                    $"This affects Custom Templates and App Functions. Exception: {ex}");
                return;
            }

            string mainTargetGuid = proj.GetUnityMainTargetGuid();
            // Add CleverTap folder reference and target membership
            string folderGuid = proj.AddFolderReference(destinationFolderPath, EditorUtils.CLEVERTAP_APP_ASSETS_FOLDER);
            proj.AddFileToBuild(mainTargetGuid, folderGuid);
        }
	}
}
#endif