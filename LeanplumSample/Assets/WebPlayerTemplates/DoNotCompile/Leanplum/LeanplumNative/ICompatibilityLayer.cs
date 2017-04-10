// Copyright 2013, Leanplum, Inc.

using System;
using System.IO;

namespace LeanplumSDK
{
    internal interface ICompatibilityLayer
    {
        void Init();

        bool LeanplumDeveloperMode { get; set; }
        string VersionName { get; set; }

        void LogDebug(string message);
        void Log(string message);
        void LogError(string message);
        void LogError(Exception error);
        void LogError(string message, Exception error);
        void LogWarning(string message);

        string GetSavedString(string key, string defaultValue = null);
        int GetSavedInt(string key, int defaultValue = 0);
        void StoreSavedString(string key, string val);
        void StoreSavedInt(string key, int val);

        void DeleteSavedSetting(string key);
        void FlushSavedSettings();

        WebRequest CreateWebRequest(string url, int timeout);

        string GetDeviceId();
        string GetDeviceName();
        string GetDeviceModel();
        string GetSystemName();
        string GetSystemVersion();
        string GetPlatformName();

        bool IsSimulator();
        bool IsConnected();

        string URLEncode(string str);
    }
}
