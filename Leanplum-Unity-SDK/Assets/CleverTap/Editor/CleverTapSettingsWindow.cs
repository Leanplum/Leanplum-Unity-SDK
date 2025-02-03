using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CleverTapSDK.Private
{
    public class CleverTapSettingsWindow : EditorWindow
    {
        internal const string ITEM_NAME = "Assets/CleverTap Settings";
        private static readonly string windowName = "CleverTap Settings";

        private CleverTapSettings settings;

        // Add menu item to open the window
        [MenuItem(ITEM_NAME)]
        public static void ShowWindow()
        {
            GetWindow<CleverTapSettingsWindow>(windowName);
        }

        private void OnEnable()
        {
            settings = LoadCleverTapSettings();
        }

        private void OnGUI()
        {
            if (settings == null)
            {
                GUILayout.Label("Error loading settings", EditorStyles.boldLabel);
                return;
            }

            float originalValue = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 180;

            GUILayout.Label(windowName, EditorStyles.boldLabel);

            settings.CleverTapAccountId = EditorGUILayout.TextField(new GUIContent("CleverTapAccountId", "Clevertap Project ID"),
                settings.CleverTapAccountId);
            settings.CleverTapAccountToken = EditorGUILayout.TextField(new GUIContent("CleverTapAccountToken", "Clevertap Project Token"),
                settings.CleverTapAccountToken);
            settings.CleverTapAccountRegion = EditorGUILayout.TextField(new GUIContent("CleverTapAccountRegion", "CleverTap Region Code"),
                settings.CleverTapAccountRegion);
            settings.CleverTapProxyDomain = EditorGUILayout.TextField(new GUIContent("CleverTapProxyDomain", "Custom Proxy Domain"),
                settings.CleverTapProxyDomain);
            settings.CleverTapSpikyProxyDomain = EditorGUILayout.TextField(new GUIContent("CleverTapSpikyProxyDomain", "Spiky Proxy Domain"),
                settings.CleverTapSpikyProxyDomain);

            GUILayout.Label("iOS specific settings", EditorStyles.boldLabel);
            settings.CleverTapDisableIDFV = EditorGUILayout.Toggle(new GUIContent("CleverTapDisableIDFV", "Disable IDFV use on iOS"),
                settings.CleverTapDisableIDFV);
            settings.CleverTapIOSUseAutoIntegrate = EditorGUILayout.Toggle(new GUIContent("UseAutoIntegrate",
                "Use [CleverTap autoIntegrate] and swizzling on iOS"),
                settings.CleverTapIOSUseAutoIntegrate);
            settings.CleverTapIOSUseUNUserNotificationCenter = EditorGUILayout.Toggle(new GUIContent("UseUNUserNotificationCenter",
                "Boolean whether to set UNUserNotificationCenter delegate on iOS. When disabled, you must implement the delegate yourself and call CleverTap methods."),
                settings.CleverTapIOSUseUNUserNotificationCenter);
            settings.CleverTapIOSPresentNotificationOnForeground = EditorGUILayout.Toggle(new GUIContent("PresentNotificationForeground",
                "Boolean whether to present remote notifications while app is on foreground on iOS."),
                settings.CleverTapIOSPresentNotificationOnForeground);

            GUILayout.Label("Other settings", EditorStyles.boldLabel);
            settings.CleverTapSettingsSaveToJSON = EditorGUILayout.Toggle(new GUIContent("Save to streaming assets",
"When enabled, settings will be saved as JSON in StreamingAssets folder for runtime access"), settings.CleverTapSettingsSaveToJSON);

            EditorGUIUtility.labelWidth = originalValue;

            if (GUILayout.Button("Save Settings"))
            {
                SaveCleverTapSettings();
            }
        }

        private CleverTapSettings LoadCleverTapSettings()
        {
            try
            {
                // Load settings from .asset file
                CleverTapSettings settings = AssetDatabase.LoadAssetAtPath<CleverTapSettings>(CleverTapSettings.settingsPath);

                if (settings == null)
                {
                    Debug.Log("Asset not found. Creating asset.");
                    // Create a new instance if it doesn't exist
                    settings = CreateInstance<CleverTapSettings>();
                    AssetDatabase.CreateAsset(settings, CleverTapSettings.settingsPath);
                    AssetDatabase.SaveAssets();
                    // Refresh the database to make sure the new asset is recognized
                    AssetDatabase.Refresh();
                }
                else
                {
                    if (settings.CleverTapSettingsSaveToJSON && !File.Exists(CleverTapSettings.jsonPath))
                    {
                        SaveSettingsToJson();
                    }
                }

                return settings;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        private void SaveCleverTapSettings()
        {
            // Save settings to .asset file
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);
            Debug.Log($"CleverTapSettings saved to {CleverTapSettings.settingsPath}");

            // Save or Delete settings JSON file
            if (settings.CleverTapSettingsSaveToJSON)
            {
                SaveSettingsToJson();
            }
            else
            {
                DeleteSettingsJson();
            }
        }

        private void SaveSettingsToJson()
        {
            try
            {
                string json = JsonUtility.ToJson(settings, true);
                if (!Directory.Exists(Application.streamingAssetsPath))
                {
                    Directory.CreateDirectory(Application.streamingAssetsPath);
                }
                File.WriteAllText(CleverTapSettings.jsonPath, json);
                Debug.Log($"CleverTap settings saved to {CleverTapSettings.jsonPath}");
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save settings to JSON: {ex.Message}");
                EditorUtility.DisplayDialog("Error",
                "Failed to save settings to JSON. Check the console for details.", "OK");
            }
        }

        private void DeleteSettingsJson()
        {
            if (File.Exists(CleverTapSettings.jsonPath))
            {
                try
                {
                    File.Delete(CleverTapSettings.jsonPath);
                    Debug.Log($"CleverTap settings deleted from: {CleverTapSettings.jsonPath}");
                    AssetDatabase.Refresh();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to delete settings JSON: {ex.Message}");
                    EditorUtility.DisplayDialog("Error",
                    "Failed to delete settings JSON. Check the console for details.", "OK");
                }
            }
        }
    }
}