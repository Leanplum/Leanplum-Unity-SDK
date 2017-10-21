//
// Copyright 2013, Leanplum, Inc.
//
//  Licensed to the Apache Software Foundation (ASF) under one
//  or more contributor license agreements.  See the NOTICE file
//  distributed with this work for additional information
//  regarding copyright ownership.  The ASF licenses this file
//  to you under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing,
//  software distributed under the License is distributed on an
//  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//  KIND, either express or implied.  See the License for the
//  specific language governing permissions and limitations
//  under the License.
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
