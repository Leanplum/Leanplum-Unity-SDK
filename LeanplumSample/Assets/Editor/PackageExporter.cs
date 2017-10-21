using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Leanplum.Private
{
    public static class PackageExporter
    {
        private static readonly string[] pathsToExport =
        {
            "Assets/LeanplumSample",
            "Assets/Plugins",
            "Assets/LeanplumSDK",
            "Assets/link"
        };

        [MenuItem("Tools/Leanplum/Export Package")]
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

