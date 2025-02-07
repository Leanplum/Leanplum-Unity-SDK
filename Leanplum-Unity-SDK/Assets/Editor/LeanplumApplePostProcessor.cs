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

        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
                return;

            DisableBitcode(path);
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