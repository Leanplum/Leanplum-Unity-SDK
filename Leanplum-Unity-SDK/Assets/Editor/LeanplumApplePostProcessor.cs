#if UNITY_EDITOR
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
        const string DEVICE_SLICE = "ios-arm64_armv7";
        const string SIMULATOR_SLICE = "ios-arm64_i386_x86_64-simulator";
        const string DEFAULT_LOCATION_IN_PROJECT = "Frameworks/Plugins/iOS";

        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
                return;

            iOSSdkVersion target = PlayerSettings.iOS.sdkVersion;
            SetupXcFramework(path, target == iOSSdkVersion.DeviceSDK);
        }

        internal static void SetupXcFramework(string path, bool useDevice = true)
        {
            string xcFrameworkDevicePath = Path.Combine(XCFRAMEWORK_NAME, DEVICE_SLICE, FRAMEWORK_NAME);
            string xcFrameworkSimulatorPath = Path.Combine(XCFRAMEWORK_NAME, SIMULATOR_SLICE, FRAMEWORK_NAME);

            string pbxProjectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromFile(pbxProjectPath);

            string targetGuid = project.GetUnityMainTargetGuid();

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
    }
}
#endif