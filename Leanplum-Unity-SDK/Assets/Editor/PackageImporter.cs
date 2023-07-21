using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Leanplum.Private
{
    public static class PackageImporter
    {
        private const string CLEVERTAP_UNITY_VERSION = "2.4.0";

        private static readonly string CLEVERTAP_UNITY_PACKAGE = "CleverTapUnityPlugin.unitypackage";
        private static readonly string CLEVERTAP_UNITY_PACKAGE_URL =
            $"https://github.com/CleverTap/clevertap-unity-sdk/raw/{CLEVERTAP_UNITY_VERSION}/{CLEVERTAP_UNITY_PACKAGE}";

        private static readonly string ASSETS_CLEVERTAP_UNITY = "Assets/CleverTapUnity/";

        private static readonly string[] FRAMEWORKS_PATHS_TO_REMOVE = new string[]
        {
            Path.Combine(Environment.CurrentDirectory, $"{ASSETS_CLEVERTAP_UNITY}/iOS/SDWebImage.framework"),
            Path.Combine(Environment.CurrentDirectory, $"{ASSETS_CLEVERTAP_UNITY}/iOS/CleverTapSDK.framework")
        };

        private static string CleverTapAndroidSDKArchivePath
        {
            get
            {
                string androidAssets = Path.Combine(Environment.CurrentDirectory, $"{ASSETS_CLEVERTAP_UNITY}/Android/");
                string file = Directory.GetFiles(androidAssets, "clevertap-android-sdk-*.aar").FirstOrDefault();
                if (!string.IsNullOrEmpty(file))
                {
                    return Path.Combine(Environment.CurrentDirectory, $"{ASSETS_CLEVERTAP_UNITY}/Android/", file);
                }

                return file;
            }
        }

        private static readonly HttpClient httpClient;

        static PackageImporter()
        {
            httpClient = new HttpClient();
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

            // Import the package
            Task<string> importPackageTask = ImportPackageAsync(packagePath);
            string result = await importPackageTask;
            if (string.IsNullOrEmpty(result))
                return;

            // Delete files from the CleverTap package that the Leanplum SDK will provide by itself
            // Delete the downloaded package file
            RemoveUnnecessaryFiles(packagePath);
            Debug.Log($"Import CleverTap Package {CLEVERTAP_UNITY_VERSION} Completed");
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

        private static void RemoveUnnecessaryFiles(string packagePath)
        {
            foreach (string path in FRAMEWORKS_PATHS_TO_REMOVE)
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }

            string archivePath = CleverTapAndroidSDKArchivePath;
            if (!string.IsNullOrEmpty(archivePath) && File.Exists(archivePath))
            {
                File.Delete(archivePath);
            }

            if (File.Exists(packagePath))
            {
                File.Delete(packagePath);
            }
        }

        public static async Task<string> ImportPackageAsync(string packagePath)
        {
            TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
            try
            {
                AssetDatabase.importPackageStarted += OnImportPackageStarted;
                AssetDatabase.importPackageCancelled += OnImportPackageCancelled;
                AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
                AssetDatabase.importPackageFailed += OnImportPackageFailed;
                AssetDatabase.ImportPackage(packagePath, false);

                return await taskCompletionSource.Task;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return null;
            }
            finally
            {
                AssetDatabase.importPackageCancelled -= OnImportPackageCancelled;
                AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
                AssetDatabase.importPackageFailed -= OnImportPackageFailed;
            }

            void OnImportPackageStarted(string packageName)
            {
                Debug.Log($"Import Package Started: {packageName}");
            }

            void OnImportPackageCancelled(string packageName)
            {
                taskCompletionSource.SetCanceled();
            }

            void OnImportPackageCompleted(string packageName)
            {
                Debug.Log($"Import Package Completed: {packageName}");
                taskCompletionSource.SetResult(packageName);
            }

            void OnImportPackageFailed(string packageName, string errorMessage)
            {
                taskCompletionSource.SetException(new Exception(errorMessage));
            }
        }
    }
}