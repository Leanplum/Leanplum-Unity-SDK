#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using CleverTapSDK.Common;
using CleverTapSDK.Utilities;
using UnityEngine;

namespace CleverTapSDK.Native
{
    internal class UnityNativePlatformCustomTemplates : CleverTapPlatformCustomTemplates
    {
        private static readonly string CLEVERTAP_ASSETS_FOLDER = "CleverTap";
        private static readonly string CLEVERTAP_CUSTOM_TEMPLATES_FOLDER = "CustomTemplates";

        private UnityNativeEventManager unityNativeEventManager;

        internal override CleverTapTemplateContext CreateContext(string name)
        {
            CleverTapLogger.LogError("CleverTap Error: CreateContext is not supported for this platform.");
            return null;
        }

        /// <summary>
        /// Sets the dependencies.
        /// Call this method as soon as the SDK initializes.
        /// </summary>
        /// <param name="unityNativeEventManager">The event manager instance.</param>
        internal void Load(UnityNativeEventManager unityNativeEventManager)
        {
            if (unityNativeEventManager == null)
            {
                CleverTapLogger.LogError("Cannot load with null unityNativeEventManager.");
                return;
            }
            this.unityNativeEventManager = unityNativeEventManager;
        }

        internal override void SyncCustomTemplates()
        {
#if UNITY_EDITOR
            SyncCustomTemplates(false);
#else
            CleverTapLogger.LogError("CleverTap Error: SyncCustomTemplates is not supported for this platform.");
#endif
        }

        internal override void SyncCustomTemplates(bool isProduction)
        {
#if !UNITY_EDITOR
            CleverTapLogger.LogError("CleverTap Error: SyncCustomTemplates(isProduction) is not supported for this platform.");
#else
            if (unityNativeEventManager == null)
            {
                CleverTapLogger.LogError("CleverTap Error: Cannot sync custom templates." +
                    " The platform is not loaded.");
                return;
            }

            string sourceFolderPath = Path.Combine(Application.dataPath, CLEVERTAP_ASSETS_FOLDER, CLEVERTAP_CUSTOM_TEMPLATES_FOLDER);
            DirectoryInfo dir = new DirectoryInfo(sourceFolderPath);
            if (!dir.Exists)
            {
                CleverTapLogger.LogError($"CleverTap Error: Cannot sync custom templates. " +
                    $"Directory {sourceFolderPath} does not exist.");
                return;
            }

            FileInfo[] files = dir.GetFiles();
            HashSet<CustomTemplate> templates = new HashSet<CustomTemplate>();
            foreach (FileInfo file in files)
            {
                if (!file.Extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
                    continue;

                string json = string.Empty;
                try
                {
                    json = File.ReadAllText(file.FullName);
                }
                catch (Exception ex)
                {
                    CleverTapLogger.LogError($"CleverTap: Failed to read template file {file.Name}: {ex.Message}");
                    continue;
                }

                JsonTemplateProducer producer = new JsonTemplateProducer(json);
                templates.UnionWith(producer.DefineTemplates());
            }

            var payload = CustomTemplateUtils.GetSyncTemplatesPayload(templates);
            unityNativeEventManager.SyncCustomTemplates(payload);
#endif
        }
    }
}
#endif
