#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using LeanplumSDK.MiniJSON;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Leanplum.Private
{
    /// <summary>
    /// Generates googleservices.xml file from google-services.json
    /// The json file must be inside Assets folder.
    /// This circumvents the need of the 'com.google.gms.google-services' gradle plugin
    /// and the json file to be in the project.
    /// This preprocessor is not needed if Google Unity package is used.
    /// The googleservices.xml resource file is set into LeanplumGoogle.androidlib
    /// so Unity can include it in the build project.
    /// </summary>
    class LeanplumAndroidBuildPreProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder
        {
            get { return 0; }
        }

        /// <summary>
        /// Google Services JSON filename google-services.json
        /// </summary>
        private static readonly string GOOGLE_SERVICES_JSON = "google-services.json";

        /// <summary>
        /// Google Services XML filename google-services.xml
        /// </summary>
        private static readonly string GOOGLE_SERVICES_XML = "googleservices.xml";

        /// <summary>
        /// AndroidManifest.xml filename.
        /// </summary>
        private static readonly string ANDROID_MANIFEST_FILE = "AndroidManifest.xml";

        /// <summary>
        /// AndroidManifest.xml template for Android resources.
        /// </summary>
        private static readonly string ANDROID_MANIFEST_RESOURCE_TEMPLATE =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
            "<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\"\n" +
            $"          package=\"{PlayerSettings.applicationIdentifier}.lib\"\n" +
            "          android:versionCode=\"1\"\n" +
            "          android:versionName=\"1.0\">\n" +
            "</manifest>";

        /// <summary>
        /// project.properties filename.
        /// </summary>
        private static readonly string PROJECT_PROPERTIES_FILE = "project.properties";

        /// <summary>
        /// project.properties template for Android resources.
        /// </summary>
        private static readonly string PROJECT_PROPERTIES_FILE_CONTENT =
            "target=android-9\n" +
            "android.library=true";

        /// <summary>
        /// Android Library name for Android resources.
        /// </summary>
        private static readonly string ANDROID_LIB = "LeanplumGoogle.androidlib";

        public void OnPreprocessBuild(BuildReport report)
        {
            OnPreprocessBuild(report.summary.platform, report.summary.outputPath);
        }

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
#if UNITY_ANDROID

            string playServicesJson = Path.Combine(Directory.GetCurrentDirectory(), "Assets", GOOGLE_SERVICES_JSON);

            if (File.Exists(playServicesJson))
            {
                // Resources needs to be included through android libs or aar
                string pluginsAndroid = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Plugins", "Android");
                string libPath = Path.Combine(pluginsAndroid, ANDROID_LIB);
                CreateAndroidLib(libPath);

                string json = ReadJson(playServicesJson);

                var dict = Json.Deserialize(json) as Dictionary<string, object>;
                var projectInfo = dict["project_info"] as Dictionary<string, object>;
                var projectId = projectInfo["project_id"] as string;

                var client = dict["client"] as List<object>;
                var element = client[0] as Dictionary<string, object>;
                var oauthClient = element["oauth_client"] as List<object>;
                var elementOathClient = oauthClient[0] as Dictionary<string, object>;
                var clientId = elementOathClient["client_id"] as string;

                var clientInfo = element["client_info"] as Dictionary<string, object>;
                var appId = clientInfo["mobilesdk_app_id"];

                var apiKey = element["api_key"] as List<object>;
                var apiKeyValues = apiKey[0] as Dictionary<string, object>;
                var key = apiKeyValues["current_key"] as string;

                var xml = CreateGoogleServicesXml(projectId, key, clientId, appId);
                string destPath = Path.Combine(libPath, "res", "values", GOOGLE_SERVICES_XML);
                WriteFile(xml, destPath);
            }

#endif
        }

        /// <summary>
        /// Creates Android lib to be used for the googleservices.xml
        /// Resources needs to be included through android libs or aar
        /// Creates project structure, AndroidManifest, and project.properties files
        /// </summary>
        /// <param name="libPath">Android library path</param>
        private void CreateAndroidLib(string libPath)
        {
            CreateLibFolders(libPath);
            WriteFile(ANDROID_MANIFEST_RESOURCE_TEMPLATE, Path.Combine(libPath, ANDROID_MANIFEST_FILE));
            WriteFile(PROJECT_PROPERTIES_FILE_CONTENT, Path.Combine(libPath, PROJECT_PROPERTIES_FILE));
        }

        private void CreateLibFolders(string lib)
        {
            CreateFolderIfNeed(lib);
            CreateFolderIfNeed(Path.Combine(lib, "res"));
            CreateFolderIfNeed(Path.Combine(lib, "res", "values"));
        }

        private void CreateFolderIfNeed(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        string ReadJson(string path)
        {
            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            reader.Close();
            return json;
        }

        void WriteFile(string content, string path)
        {
            StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine(content);
            writer.Close();
        }

        private string CreateGoogleServicesXml(string projectId, string apiKey, string clientId, object appId)
        {
            return "<?xml version='1.0' encoding='utf-8'?>\n<resources tools:keep=\"@string/project_id," +
                "@string/default_web_client_id,@string/google_app_id,@string/google_api_key\"" +
                " xmlns:tools=\"http://schemas.android.com/tools\">\n " +
                "<string name=\"default_web_client_id\" translatable=\"false\">" + clientId + "</string>\n" +
                "<string name=\"google_app_id\" translatable=\"false\">" + appId + "</string>\n" +
                "<string name=\"google_api_key\" translatable=\"false\">" + apiKey + "</string>\n" +
                "<string name=\"project_id\" translatable=\"false\">" + projectId + "</string>\n" +
                "</resources>";
        }
    }
}
#endif