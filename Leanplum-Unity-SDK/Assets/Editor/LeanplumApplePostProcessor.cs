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
        private static readonly string[] FRAMEWORKS = new string[] { "Leanplum", "CleverTapSDK", "SDWebImage" };
        const string DEVICE_SLICE = "ios-arm64";
        const string SIMULATOR_SLICE = "ios-arm64_x86_64-simulator";
        const string DEVICE_SLICE_ARMV7 = "ios-arm64_armv7";
        const string SIMULATOR_SLICE_I386 = "ios-arm64_i386_x86_64-simulator";
        const string DEFAULT_LOCATION_IN_PROJECT = "Frameworks/Plugins/iOS";

        const string ENABLE_BITCODE = "ENABLE_BITCODE";
        const string NO = "NO";

        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
                return;

            iOSSdkVersion target = PlayerSettings.iOS.sdkVersion;
            foreach (string framework in FRAMEWORKS)
            {
                SetupXcFramework(framework, path, target == iOSSdkVersion.DeviceSDK);
            }

            DisableBitcode(path);
        }

        internal static void SetupXcFramework(string name, string path, bool useDevice = true)
        {
            string xcframeworkName = $"{name}.xcframework";
            string frameworkName = $"{name}.framework";
            string xcFrameworkDevicePath = Path.Combine(xcframeworkName, DEVICE_SLICE, frameworkName);
            string xcFrameworkSimulatorPath = Path.Combine(xcframeworkName, SIMULATOR_SLICE, frameworkName);

            PBXProject project = GetPBXProject(path, out string pbxProjectPath);

            string frameworkPath = Path.Combine(DEFAULT_LOCATION_IN_PROJECT, xcFrameworkDevicePath);
            string fileGuid = project.FindFileGuidByProjectPath(frameworkPath);

            string frameworkPathSim = Path.Combine(DEFAULT_LOCATION_IN_PROJECT, xcFrameworkSimulatorPath);
            string fileGuidSim = project.FindFileGuidByProjectPath(frameworkPathSim);

            if (useDevice)
            {
                if (string.IsNullOrEmpty(fileGuid))
                {
                    Debug.Log($"Device {frameworkName} not found in Xcode project. Path: {frameworkPath}");
                    return;
                }

                string xcFrameworkSimulatorDirPath = Path.Combine(path, DEFAULT_LOCATION_IN_PROJECT, xcframeworkName, SIMULATOR_SLICE);
                if (!Directory.Exists(xcFrameworkSimulatorDirPath))
                    xcFrameworkSimulatorDirPath = Path.Combine(path, DEFAULT_LOCATION_IN_PROJECT, xcframeworkName, SIMULATOR_SLICE_I386);

                Setup(project, frameworkName, fileGuidSim, xcFrameworkSimulatorDirPath, fileGuid);
            }
            else
            {
                if (string.IsNullOrEmpty(fileGuidSim))
                {
                    Debug.Log($"Simulator {frameworkName} not found in Xcode project. Path: {frameworkPathSim}");
                    return;
                }

                string xcFrameworkDeviceDirPath = Path.Combine(path, DEFAULT_LOCATION_IN_PROJECT, xcframeworkName, DEVICE_SLICE);
                if (!Directory.Exists(xcFrameworkDeviceDirPath))
                    xcFrameworkDeviceDirPath = Path.Combine(path, DEFAULT_LOCATION_IN_PROJECT, xcframeworkName, DEVICE_SLICE_ARMV7);

                Setup(project, frameworkName, fileGuid, xcFrameworkDeviceDirPath, fileGuidSim);
            }

            project.WriteToFile(pbxProjectPath);
        }

        private static void Setup(PBXProject project, string frameworkName, string fileGuidToRemove, string pathToRemove, string fileGuid)
        {
            string targetGuid = project.GetUnityMainTargetGuid();

            // Remove simulator/device framework
            if (!string.IsNullOrEmpty(fileGuidToRemove))
            {
                project.RemoveFrameworkFromProject(fileGuidToRemove, frameworkName);
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

        /// <summary>
        /// Building with bitcode is deprecated in Xcode 14
        /// </summary>
        /// <param name="path"></param>
        private static void DisableBitcode(string path)
        {
            PBXProject pbxProject = GetPBXProject(path, out string pbxProjectPath);
            // Unity Framework
            var frameworkTarget = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(frameworkTarget, ENABLE_BITCODE, NO);
            // Unity iPhone
            var mainTarget = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(mainTarget, ENABLE_BITCODE, NO);

            pbxProject.WriteToFile(pbxProjectPath);
        }
    }
}
#endif