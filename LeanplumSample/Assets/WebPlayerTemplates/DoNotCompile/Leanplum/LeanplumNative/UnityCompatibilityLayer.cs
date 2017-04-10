// Copyright 2013, Leanplum, Inc.

using LeanplumSDK.MiniJSON;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Reflection;

using PlayerPrefs = LeanplumSDK.Prefs.PlayerPrefs;

namespace LeanplumSDK
{
    internal class UnityCompatibilityLayer : ICompatibilityLayer
    {
                public void Init()
    {
      if (LeanplumUnityHelper.Instance == null)
      {
        LogWarning("Unable to listen for app lifecycle callbacks");
      }
    }

        /// <summary>
        ///     Gets or sets a value indicating whether the Leanplum code itself
        ///     is to be developed and tested.
        /// </summary>
        /// <value><c>true</c> if we're a Leanplum developer; otherwise, <c>false</c>.</value>
        public bool LeanplumDeveloperMode { get; set; }

        /// <summary>
        ///     Gets or sets the version of the application.
        /// </summary>
        /// <value>The version of the application.</value>
        public string VersionName { get; set; }

        #region Logging
        public void LogDebug(string message)
        {
            if (LeanplumDeveloperMode)
            {
                Debug.Log(message);
            }
        }

        public void Log(string message)
        {
            Debug.Log("Leanplum: " + message);
        }

        public void LogWarning(string message)
        {
      if (LeanplumNative.isStopped)
            {
                return;
            }
            Debug.LogWarning("Leanplum Warning: " + message);
        }

        public void LogError(string message)
        {
      if (LeanplumNative.isStopped)
            {
                return;
            }
            Debug.LogError("Leanplum Error: " + message);
        }

        public void LogError(Exception error)
        {
            LogError(error.ToString());
            LogError(error.StackTrace);
        }

        public void LogError(string message, Exception error)
        {
            LogError(message);
            LogError(error);
        }
        #endregion Logging

        #region Persistent settings
        /// <summary>
        ///     Provides a way to get a setting that persists across sessions.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value if the setting has no value,
        ///     or is the wrong type.</param>
        /// <returns>System.Object. Either returns the correctly typecast object, or defaultValue.
        /// </returns>
        public string GetSavedString(string key, string defaultValue = null)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public int GetSavedInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public void StoreSavedString(string key, string val)
        {
            PlayerPrefs.SetString(key, val);
        }

        public void StoreSavedInt(string key, int val)
        {
            PlayerPrefs.SetInt(key, val);
        }

        public void DeleteSavedSetting(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void FlushSavedSettings()
        {
            PlayerPrefs.Flush();
        }
        #endregion Persistent settings

        #region Web
        public WebRequest CreateWebRequest(string url, int timeout)
        {
            return new UnityWebRequest(url, timeout);
        }

        public string URLEncode(string str)
        {
            return WWW.EscapeURL(str, Encoding.UTF8);
        }
        #endregion Web

        #region System information
        public string GetDeviceName()
        {
            if (GetPlatformName() == "iOS" || GetPlatformName() == "Android")
            {
                return GetDeviceModel();
            }
            return SystemInfo.deviceName;
        }

        public string GetDeviceId()
        {
            // Using reflection, to avoid automatic addition of android.permission.READ_PHONE_STATE
            Type systemInfo = typeof (SystemInfo);
            PropertyInfo property = systemInfo.GetProperty("deviceUniqueIdentifier");
            return (string) property.GetValue(null, null);
        }

        public string GetDeviceModel()
        {
            return Util.Capitalize(SystemInfo.deviceModel);
        }

        public string GetSystemName()
        {
            if (IsSimulator())
            {
                return "Unity Editor";
            }

            RuntimePlatform platform = Application.platform;
            if (platform == RuntimePlatform.Android)
            {
                return "Android OS";
            }
            else if (platform == RuntimePlatform.IPhonePlayer)
            {
                return "iPhone OS";
            }
            else if (platform == RuntimePlatform.OSXPlayer)
            {
                return "Unity Standalone Mac OS";
            }
            else if (platform == RuntimePlatform.WindowsPlayer)
            {
                return "Unity Standalone Windows";
            }
            else if (platform == RuntimePlatform.LinuxPlayer)
            {
                return "Unity Standalone Linux";
            }
            return "Unknown";
        }

        public string GetPlatformName()
        {
            RuntimePlatform platform = Application.platform;
            if (platform == RuntimePlatform.IPhonePlayer)
            {
                return "iOS";
            }
            else if (platform == RuntimePlatform.Android)
            {
                return "Android";
            }
            else
            {
                return "Standalone";
            }
        }

        public string GetSystemVersion()
        {
            string[] version = SystemInfo.operatingSystem.Split(' ');
            if (GetPlatformName() == "Android")
            {
                return version[version.Length > 2 ? 2 : version.Length - 1];
            }
            return version[version.Length - 1];
        }

        public bool IsSimulator()
        {
            return Application.isEditor;
        }

        public bool IsConnected()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
        #endregion System information
    }
}
