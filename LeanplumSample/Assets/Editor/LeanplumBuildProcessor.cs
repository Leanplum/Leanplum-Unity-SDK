#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using LeanplumSDK.MiniJSON;
using UnityEditor;
using UnityEditor.Build;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace Leanplum.Private
{
    #if UNITY_2018_1_OR_NEWER
    class LeanplumBuildProcessor : IPreprocessBuildWithReport
    #else
    class LeanplumBuildProcessor : IPreprocessBuild
    #endif
    {

        public int callbackOrder
        {
            get { return 0; }
        }

        #if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
        {
            OnPreprocessBuild(report.summary.platform, report.summary.outputPath);
        }
        #endif

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
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
                var projectNumber = projectInfo["project_number"] as string;

                var client = dict["client"] as List<object>;
                var element = client[0] as Dictionary<string, object>;
                var oauthClient = element["oauth_client"] as List<object>;
                var elementOathClient = oauthClient[0] as Dictionary<string, object>;
                var clientId = elementOathClient["client_id"] as string;

                var clientInfo = element["client_info"] as Dictionary<string, object>;
                var appId = clientInfo["mobilesdk_app_id"];

                var xml = CreateXml(projectNumber, clientId, appId);
                WriteFile(xml, destPath);
            }
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

        private string CreateXml(string projectNumber, string clientId, object appId)
        {
            return "<?xml version='1.0' encoding='utf-8'?>\n<resources tools:keep=\"@string/gcm_defaultSenderId,@string/project_id,@string/default_web_client_id,@string/google_app_id\" xmlns:tools=\"http://schemas.android.com/tools\">\n "
                + "<string name=\"gcm_defaultSenderId\" translatable=\"false\">" + projectNumber + "</string>\n"
                + "<string name=\"default_web_client_id\" translatable=\"false\">" + clientId + "</string>\n"
                + "<string name=\"google_app_id\" translatable=\"false\">" + appId + "</string>\n"
                + "</resources>";
        }
    }
}
#endif