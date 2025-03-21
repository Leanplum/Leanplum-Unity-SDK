using System;
using UnityEditor;

namespace Leanplum.Private
{
    public static class PackageExporter
    {
        private static readonly string[] pathsToExport =
        {
            "Assets/Sample",
            "Assets/Plugins",
            "Assets/LeanplumSDK",
            "Assets/CleverTap",
            "Assets/ExternalDependencyManager",
            "Assets/Editor/LeanplumAndroidBuildPreProcessor.cs",
            "Assets/Editor/LeanplumAndroidGradleBuildProcessor.cs",
            "Assets/Editor/LeanplumApplePostProcessor.cs",
        };

        [MenuItem(MenuConstants.LEANPLUM_TOOLS_MENU + "Export Package")]
        public static void ExportPackage()
        {
            string packageName = Environment.GetEnvironmentVariable("OUT_PKG");
            if (string.IsNullOrEmpty(packageName))
            {
                packageName = "DEV-SNAPSHOT.unitypackage";
            }

            AssetDatabase.ExportPackage(pathsToExport,
                packageName,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
        }
    }
}