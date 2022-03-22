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
        public ApiConfig()
        {
        }

        internal const int MAX_REQUESTS_PER_API_CALL = 10000;
        //internal const int MAX_STORED_API_CALLS = 10000;


        public string AppId { get; set; }
        public string AccessKey { get; set; }

        public string Token { get; set; }

        public string DeviceId { get; set; }
        public string UserId { get; set; }

        public string apiHost = "api.leanplum.com";
        public string apiPath = "api";
        public bool apiSSL = true;
        public string socketHost = "dev.leanplum.com";
        public int socketPort = 443;

        public string Client
        {
            get
            {
                string platformName = LeanplumNative.CompatibilityLayer.GetPlatformName().ToLower();
                return $"{Constants.CLIENT_PREFIX}-{platformName}";
            }
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