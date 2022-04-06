//
// Copyright 2022, Leanplum, Inc.
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
using System.Collections.Generic;

namespace LeanplumSDK
{
    public class ApiConfig
    {
        internal ApiConfig()
        {
        }

        private static readonly string API_HOST_NAME = "api.leanplum.com";
        private static readonly string SOCKET_HOST = "dev.leanplum.com";
        private static readonly int SOCKET_PORT = 443;
        private static readonly bool API_SSL = true;
        private static readonly string API_PATH = "api";

        private static readonly string TOKEN_KEY = "__leanplum_token";
        private static readonly string API_HOST_KEY = "__leanplum_api_host";
        private static readonly string API_PATH_KEY = "__leanplum_api_path";
        private static readonly string SOCKET_HOST_KEY = "__leanplum_socket_host";

        public string AppId { get; private set; }
        public string AccessKey { get; private set; }

        public string DeviceId { get; set; }
        public string UserId { get; set; }

        private string token;
        public string Token
        {
            get
            {
                if (string.IsNullOrEmpty(token))
                {
                    token = LeanplumNative.CompatibilityLayer.GetSavedString(TOKEN_KEY);
                }
                return token;
            }
            set
            {
                token = value;
                LeanplumNative.CompatibilityLayer.StoreSavedString(TOKEN_KEY, value);
            }
        }

        public string ApiHost
        {
            get
            {
                string host = LeanplumNative.CompatibilityLayer.GetSavedString(API_HOST_KEY);
                if (string.IsNullOrEmpty(host))
                {
                    host = API_HOST_NAME;
                }
                return host;
            }
            private set
            {
                LeanplumNative.CompatibilityLayer.StoreSavedString(API_HOST_KEY, value);
            }
        }

        public string ApiPath
        {
            get
            {
                string path = LeanplumNative.CompatibilityLayer.GetSavedString(API_PATH_KEY);
                if (string.IsNullOrEmpty(path))
                {
                    path = API_PATH;
                }
                return path;
            }
            private set
            {
                LeanplumNative.CompatibilityLayer.StoreSavedString(API_PATH_KEY, value);
            }
        }

        public string SocketHost
        {
            get
            {
                string socketHost = LeanplumNative.CompatibilityLayer.GetSavedString(SOCKET_HOST_KEY);
                if (string.IsNullOrEmpty(socketHost))
                {
                    socketHost = SOCKET_HOST;
                }
                return socketHost;
            }
            private set
            {
                LeanplumNative.CompatibilityLayer.StoreSavedString(SOCKET_HOST_KEY, value);
            }
        }

        private bool apiSsl = API_SSL;
        public bool ApiSSL
        {
            get
            {
                return apiSsl;
            }
            private set
            {
                apiSsl = value;
            }
        }

        private int socketPort = SOCKET_PORT;
        public int SocketPort
        {
            get
            {
                return socketPort;
            }
            private set
            {
                socketPort = value;
            }
        }

        public string Client
        {
            get
            {
                string platformName = LeanplumNative.CompatibilityLayer.GetPlatformName().ToLower();
                return $"{Constants.CLIENT_PREFIX}-{platformName}";
            }
        }

        internal void SetApiConfig(string apiHost, string apiPath, bool apiSSL)
        {
            ApiHost = apiHost;
            ApiPath = apiPath;
            ApiSSL = apiSSL;
        }

        internal void SetSocketConfig(string socketHost, int socketPort)
        {
            SocketHost = socketHost;
            SocketPort = socketPort;
        }

        internal void SetAppId(string appId, string accessKey)
        {
            AppId = appId;
            AccessKey = accessKey;
        }

        public virtual bool AttachApiKeys(IDictionary<string, string> dict)
        {
            if (string.IsNullOrEmpty(AppId) || string.IsNullOrEmpty(AccessKey))
            {
                LeanplumNative.CompatibilityLayer.LogError("API keys are not set. Please use " +
                    "Leanplum.SetAppIdForDevelopmentMode or " +
                    "Leanplum.SetAppIdForProductionMode.");
                return false;
            }
            dict[Constants.Params.APP_ID] = AppId;
            dict[Constants.Params.CLIENT_KEY] = AccessKey;
            dict[Constants.Params.CLIENT] = Client;
            return true;
        }
    }
}