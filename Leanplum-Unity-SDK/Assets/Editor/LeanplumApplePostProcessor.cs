#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

namespace Leanplum.Private
{
    class LeanplumApplePostProcessor
    {
        const string FRAMEWORK_NAME = "Leanplum.framework";
        const string XCFRAMEWORK_NAME = "Leanplum.xcframework";
        const string DEVICE_SLICE = "ios-arm64";
        const string SIMULATOR_SLICE = "ios-arm64_x86_64-simulator";
        const string DEVICE_SLICE_ARMV7 = "ios-arm64_armv7";
        const string SIMULATOR_SLICE_I386 = "ios-arm64_i386_x86_64-simulator";
        const string DEFAULT_LOCATION_IN_PROJECT = "Frameworks/Plugins/iOS";

        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
                return;

            iOSSdkVersion target = PlayerSettings.iOS.sdkVersion;
            SetupXcFramework(path, target == iOSSdkVersion.DeviceSDK);
            DisableBitcode(path);
            AddCodeSignScriptBuildPhase(path);
        }

        internal static void SetupXcFramework(string path, bool useDevice = true)
        {
            string xcFrameworkDevicePath = Path.Combine(XCFRAMEWORK_NAME, DEVICE_SLICE, FRAMEWORK_NAME);
            string xcFrameworkSimulatorPath = Path.Combine(XCFRAMEWORK_NAME, SIMULATOR_SLICE, FRAMEWORK_NAME);

            PBXProject project = GetPBXProject(path, out string pbxProjectPath);

            string frameworkPath = Path.Combine(DEFAULT_LOCATION_IN_PROJECT, xcFrameworkDevicePath);
            string fileGuid = project.FindFileGuidByProjectPath(frameworkPath);

            string frameworkPathSim = Path.Combine(DEFAULT_LOCATION_IN_PROJECT, xcFrameworkSimulatorPath);
            string fileGuidSim = project.FindFileGuidByProjectPath(frameworkPathSim);

            if (useDevice)
            {
                if (string.IsNullOrEmpty(fileGuid))
                {
                    Debug.Log($"Device {FRAMEWORK_NAME} not found in Xcode project. Path: {frameworkPath}");
                    return;
                }

                string xcFrameworkSimulatorDirPath = Path.Combine(path, DEFAULT_LOCATION_IN_PROJECT, XCFRAMEWORK_NAME, SIMULATOR_SLICE);
                if (!Directory.Exists(xcFrameworkSimulatorDirPath))
                    xcFrameworkSimulatorDirPath = Path.Combine(path, DEFAULT_LOCATION_IN_PROJECT, XCFRAMEWORK_NAME, SIMULATOR_SLICE_I386);

                Setup(project, fileGuidSim, xcFrameworkSimulatorDirPath, fileGuid);
            }
            else
            {
                if (string.IsNullOrEmpty(fileGuidSim))
                {
                    Debug.Log($"Simulator {FRAMEWORK_NAME} not found in Xcode project. Path: {frameworkPathSim}");
                    return;
                }

                string xcFrameworkDeviceDirPath = Path.Combine(path, DEFAULT_LOCATION_IN_PROJECT, XCFRAMEWORK_NAME, DEVICE_SLICE);
                if (!Directory.Exists(xcFrameworkDeviceDirPath))
                    xcFrameworkDeviceDirPath = Path.Combine(path, DEFAULT_LOCATION_IN_PROJECT, XCFRAMEWORK_NAME, DEVICE_SLICE_ARMV7);

                Setup(project, fileGuid, xcFrameworkDeviceDirPath, fileGuidSim);
            }

            project.WriteToFile(pbxProjectPath);
        }

        private static void Setup(PBXProject project, string fileGuidToRemove, string pathToRemove, string fileGuid)
        {
            string targetGuid = project.GetUnityMainTargetGuid();

            // Remove simulator/device framework
            if (!string.IsNullOrEmpty(fileGuidToRemove))
            {
                project.RemoveFrameworkFromProject(fileGuidToRemove, FRAMEWORK_NAME);
                project.RemoveFile(fileGuidToRemove);
            }

            // Delete simulator/device framework
            if (Directory.Exists(pathToRemove))
            {
                Directory.Delete(pathToRemove, true);
            }

            // Add device/simulator framework
            PBXProjectExtensions.AddFileToEmbedFrameworks(project, targetGuid, fileGuid);
        }

        private static PBXProject GetPBXProject(string path, out string pbxProjectPath)
        {
            pbxProjectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromFile(pbxProjectPath);
            return project;
        }

        private static void DisableBitcode(string path)
        {
            // Unity Framework
            PBXProject pbxProject = GetPBXProject(path, out string pbxProjectPath);
            var target = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            pbxProject.WriteToFile(pbxProjectPath);
        }

        private static string AddCodeSignScriptBuildPhase(string path)
        {
            PBXProject pbxProject = GetPBXProject(path, out string pbxProjectPath);
            // Unity iPhone
            string targetGuid = pbxProject.GetUnityMainTargetGuid();
            string scriptGuid = pbxProject.AddShellScriptBuildPhase(targetGuid, "Code Sign", "/bin/sh", script);
            pbxProject.WriteToFile(pbxProjectPath);
            return scriptGuid;
        }

        static readonly string script = @"
        # Populate a variable with the current code signing identity if it's available in the environment.
        SIGNING_IDENTITY=""${EXPANDED_CODE_SIGN_IDENTITY:-$CODE_SIGN_IDENTITY}""
        
        # Populate a variable with the current code signing flags and options in the environment.
        OTHER_CODE_SIGN_FLAGS=${OTHER_CODE_SIGN_FLAGS:-}
        
        TARGET_FRAMEWORKS_PATH=""${TARGET_BUILD_DIR}/${FRAMEWORKS_FOLDER_PATH}/""
        # Re-sign the packaged frameworks using the application's details.
        if [ -n ""${SIGNING_IDENTITY}"" ]; then
            if [[ -d $TARGET_FRAMEWORKS_PATH ]]
            then
                find ""$TARGET_FRAMEWORKS_PATH"" \
                -name ""*.framework"" \
                -type d \
                -exec codesign ${OTHER_CODE_SIGN_FLAGS} \
                    --force \
                    --sign ""${SIGNING_IDENTITY}"" \
                    --options runtime \
                    --deep \
                    {} \;
        fi
        fi
        ";
    }
}
#endif