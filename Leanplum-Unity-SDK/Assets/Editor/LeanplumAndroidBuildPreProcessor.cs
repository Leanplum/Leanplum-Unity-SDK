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
    /// The json file must be inside Assets folder
    /// This circumvents the need of the 'com.google.gms.google-services' gradle plugin
    /// and the json file to be in the project
    /// </summary>
    [Obsolete("Use google-services gradle plugin instead")]
    class LeanplumAndroidBuildPreProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder
        {
            get { return 0; }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            OnPreprocessBuild(report.summary.platform, report.summary.outputPath);
        }

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            #if UNITY_ANDROID

            string assets = Directory.GetCurrentDirectory() + "/Assets/";

            string playServicesJson = assets + "google-services.json";
            string destPath = assets + "/Plugins/Android/res/values/googleservices.xml";
            if (File.Exists(playServicesJson))
            {
                CreateFolderIfNeed(assets + "/Plugins/Android/res/");
                CreateFolderIfNeed(assets + "/Plugins/Android/res/values");

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

                var xml = CreateXml(projectId, key, clientId, appId);
                WriteFile(xml, destPath);
            }

            #endif
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

        private string CreateXml(string projectId, string appKey, string clientId, object appId)
        {
            return "<?xml version='1.0' encoding='utf-8'?>\n<resources tools:keep=\"@string/project_id,@string/default_web_client_id,@string/google_app_id,@string/google_app_key\" xmlns:tools=\"http://schemas.android.com/tools\">\n "
                + "<string name=\"default_web_client_id\" translatable=\"false\">" + clientId + "</string>\n"
                + "<string name=\"google_app_id\" translatable=\"false\">" + appId + "</string>\n"
                + "<string name=\"google_app_key\" translatable=\"false\">" + appKey + "</string>\n"
                + "<string name=\"project_id\" translatable=\"false\">" + projectId + "</string>\n"
                + "</resources>";
        }
    }
}
#endif