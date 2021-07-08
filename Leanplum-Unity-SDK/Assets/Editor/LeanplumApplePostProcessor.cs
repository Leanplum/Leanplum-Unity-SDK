#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Leanplum.Private
{
    class LeanplumApplePostProcessor
    {
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
                return;

            const string defaultLocationInProj = "Frameworks/Plugins/iOS";
            const string fatFrameworkName = "Leanplum.framework";
            const string xcFrameworkPath = "Leanplum.xcframework/ios-arm64_armv7/Leanplum.framework";
            string frameworkPath = Path.Combine(defaultLocationInProj, fatFrameworkName);

            string pbxProjectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromFile(pbxProjectPath);

            string targetGuid = project.GetUnityMainTargetGuid();
            string fileGuid = project.FindFileGuidByProjectPath(frameworkPath);

            if (string.IsNullOrEmpty(fileGuid))
            {
                frameworkPath = Path.Combine(defaultLocationInProj, xcFrameworkPath);
                fileGuid = project.FindFileGuidByProjectPath(frameworkPath);
            }

            UnityEditor.iOS.Xcode.Extensions.PBXProjectExtensions.AddFileToEmbedFrameworks(project, targetGuid, fileGuid);

            project.WriteToFile(pbxProjectPath);
        }
    }
}
#endif