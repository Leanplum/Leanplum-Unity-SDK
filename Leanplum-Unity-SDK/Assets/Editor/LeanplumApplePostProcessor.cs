#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Leanplum.Private
{
    class LeanplumApplePostProcessor
    {
        const string ENABLE_BITCODE = "ENABLE_BITCODE";
        const string NO = "NO";
        const string PREPROCESSOR_MACROS = "GCC_PREPROCESSOR_DEFINITIONS";

        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
                return;

            DisableBitcode(path);
            DisableCleverTapUnityAppController(path);
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

        /// <summary>
        /// Set the CT_NO_APP_CONTROLLER_SUBCLASS preprocessor macro.
        /// This disables CleverTap's UnityAppController subclass.
        /// The CleverTap initialization is handled by the Leanplum SDK.
        /// Use the LeanplumUnityAppController.
        /// </summary>
        /// <param name="path"></param>
        private static void DisableCleverTapUnityAppController(string path)
        {
            PBXProject pbxProject = GetPBXProject(path, out string pbxProjectPath);
            var projectTarget = pbxProject.GetUnityFrameworkTargetGuid();

            // The UpdateBuildProperty set the property value if no values are present. This overrides the $(inherited) value.
            // Add the $(inherited) value as a workaround.
            pbxProject.UpdateBuildProperty(projectTarget, PREPROCESSOR_MACROS, new string[] { "$(inherited)", "CT_NO_APP_CONTROLLER_SUBCLASS" }, null);

            pbxProject.WriteToFile(pbxProjectPath);
        }
    }
}
#endif