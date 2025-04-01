using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Leanplum.Private
{
    [InitializeOnLoad]
    public static class PackageImporter
    {
        private const string CLEVERTAP_UNITY_VERSION = "5.2.0";
        private static readonly string CLEVERTAP_UNITY_PACKAGE_NAME = "CleverTapSDK";
        private static readonly string CLEVERTAP_UNITY_PACKAGE = $"{CLEVERTAP_UNITY_PACKAGE_NAME}.unitypackage";
        private static readonly string CLEVERTAP_UNITY_PACKAGE_URL =
            $"https://github.com/CleverTap/clevertap-unity-sdk/raw/{CLEVERTAP_UNITY_VERSION}/{CLEVERTAP_UNITY_PACKAGE}";
        private static readonly string ASSETS_CLEVERTAP_UNITY = "Assets/CleverTap/";

        private const string EDM4U_VERSION = "1.2.185";
        private static readonly string EDM4U_UNITY_PACKAGE_NAME = $"external-dependency-manager-{EDM4U_VERSION}";
        private static readonly string EDM4U_UNITY_PACKAGE = $"{EDM4U_UNITY_PACKAGE_NAME}.unitypackage";
        private static readonly string EDM4U_UNITY_PACKAGE_URL =
            $"https://github.com/googlesamples/unity-jar-resolver/raw/v{EDM4U_VERSION}/external-dependency-manager-{EDM4U_VERSION}.unitypackage";
        private static readonly string ASSETS_EDM4U = "Assets/ExternalDependencyManager/";

        private static readonly HttpClient httpClient;

        static PackageImporter()
        {
            httpClient = new HttpClient();

            AssetDatabase.importPackageStarted += OnImportPackageStarted;
            AssetDatabase.importPackageCancelled += OnImportPackageCancelled;
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;
        }

        [MenuItem(MenuConstants.LEANPLUM_TOOLS_MENU + "Import CleverTap Package " + CLEVERTAP_UNITY_VERSION)]
        public static async void ImportCleverTapPackage()
        {
            Debug.Log($"Import CleverTap Package {CLEVERTAP_UNITY_VERSION} Started");
            // Path to save the downloaded package
            string packagePath = Path.Combine(Environment.CurrentDirectory, CLEVERTAP_UNITY_PACKAGE);
            // Download the package
            Task<bool> downloadPackageTask = DownloadFileAsync(CLEVERTAP_UNITY_PACKAGE_URL, packagePath);
            bool successful = await downloadPackageTask;
            if (!successful)
                return;

            // Delete CleverTap package folder.
            // This ensures files deleted in the new package version are not left in project.
            if (Directory.Exists(ASSETS_CLEVERTAP_UNITY))
            {
                Directory.Delete(ASSETS_CLEVERTAP_UNITY, true);
            }

            // Import the package
            AssetDatabase.ImportPackage(packagePath, false);
        }

        [MenuItem(MenuConstants.LEANPLUM_TOOLS_MENU + "Import EDM4U " + EDM4U_VERSION)]
        public static async void ImportEDM4U()
        {
            Debug.Log($"Import EDM4U Package {EDM4U_VERSION} Started");
            // Path to save the downloaded package
            string packagePath = Path.Combine(Environment.CurrentDirectory, EDM4U_UNITY_PACKAGE);
            // Download the package
            Task<bool> downloadPackageTask = DownloadFileAsync(EDM4U_UNITY_PACKAGE_URL, packagePath);
            bool successful = await downloadPackageTask;
            if (!successful)
                return;

            // Delete CleverTap package folder.
            // This ensures files deleted in the new package version are not left in project.
            if (Directory.Exists(ASSETS_EDM4U))
            {
                Directory.Delete(ASSETS_EDM4U, true);
            }

            // Import the package
            AssetDatabase.ImportPackage(packagePath, false);
        }

        private static async Task<bool> DownloadFileAsync(string url, string savePath)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    byte[] fileData = await response.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(savePath, fileData);
                    Debug.Log("File downloaded and saved to: " + savePath);
                    return true;
                }
                else
                {
                    Debug.LogError("File download failed. Status Code: " + response.StatusCode);
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        private static void OnImportPackageStarted(string packageName)
        {
            Debug.Log($"Import Package Started: {packageName}");
        }

        private static void OnImportPackageCancelled(string packageName)
        {
            Debug.LogError($"Import Package Cancelled: {packageName}");
            CleanUp(packageName);
        }

        private static void OnImportPackageCompleted(string packageName)
        {
            Debug.Log($"Import Package Completed: {packageName}");
            CleanUp(packageName);
        }

        private static void OnImportPackageFailed(string packageName, string errorMessage)
        {
            Debug.LogError($"Import Package Failed: {packageName}. Error: {errorMessage}");
            CleanUp(packageName);
        }

        private static void CleanUp(string packageName)
        {
            // Delete the downloaded package file
            string packagePath = string.Empty;
            if (packageName == CLEVERTAP_UNITY_PACKAGE_NAME)
            {
                packagePath = Path.Combine(Environment.CurrentDirectory, CLEVERTAP_UNITY_PACKAGE);
            }
            else if (packageName == EDM4U_UNITY_PACKAGE_NAME)
            {
                packagePath = Path.Combine(Environment.CurrentDirectory, EDM4U_UNITY_PACKAGE);
            }

            if (!string.IsNullOrEmpty(packagePath) && File.Exists(packagePath))
            {
                File.Delete(packagePath);
            }
        }
    }
}