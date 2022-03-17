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
using System;
using System.Collections.Generic;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK
{
    internal class RequestSender
    {
        // TODO: Move to Leanplum 
        internal ApiConfig apiConfig = new ApiConfig();

        private static object padLock = new object();

        private RequestSender()
        {
        }

        internal void SaveRequest(Request request)
        {
            lock (padLock)
            {
                // TODO: Refactor this logic
                int start = LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.START_KEY, 0);
                int count = LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.COUNT_KEY, 0);
                count++;
                if (count > Constants.MAX_STORED_API_CALLS)
                {
                    count = Constants.MAX_STORED_API_CALLS;
                    LeanplumNative.CompatibilityLayer.DeleteSavedSetting(String.Format(Constants.Defaults.ITEM_KEY, start));
                    start++;
                    LeanplumNative.CompatibilityLayer.StoreSavedInt(Constants.Defaults.START_KEY, start);
                }
                string itemKey = String.Format(Constants.Defaults.ITEM_KEY, start + count - 1);
                // TODO: check if UUID should be attached
                var args = CreateArgsDictionary(request);
                LeanplumNative.CompatibilityLayer.StoreSavedString(itemKey, Json.Serialize(args));
                LeanplumNative.CompatibilityLayer.StoreSavedInt(Constants.Defaults.COUNT_KEY, count);
            }
        }

        public void Send(Request request)
        {
            if (Constants.isNoop)
                return;

            SaveRequest(request);

            if (Leanplum.IsDeveloperModeEnabled || RequestType.IMMEDIATE.Equals(request.Type))
            {
                try
                {
                    if (Validate(request))
                        SendRequests();
                }
                catch (Exception ex)
                {
                    // TODO: Check the logging
                    LeanplumNative.CompatibilityLayer.LogError(ex.Message);
                }
            }
        }

        internal void SendRequests()
        {

        }

        private bool Validate(Request request)
        {
            if (string.IsNullOrEmpty(apiConfig.AppId))
            {
                LeanplumNative.CompatibilityLayer.LogError("Cannot send request. appId is not set.");
                return false;
            }

            if (string.IsNullOrEmpty(apiConfig.AccessKey))
            {
                LeanplumNative.CompatibilityLayer.LogError("Cannot send request. accessKey is not set.");
                return false;
            }

            if (!LeanplumNative.CompatibilityLayer.IsConnected())
            {
                LeanplumNative.CompatibilityLayer.LogError("Device is offline, will try sending requests again later.");
                request.OnError(new Exception("Leanplum: device is offline"));
                return false;
            }

            return true;
        }

        internal virtual IDictionary<string, object> CreateArgsDictionary(Request request)
        {
            IDictionary<string, object> args = new Dictionary<string, object>
            {
                [Constants.Params.ACTION] = request.ApiMethod,
                [Constants.Params.DEVICE_ID] = apiConfig.DeviceId,
                [Constants.Params.USER_ID] = apiConfig.UserId,
                [Constants.Params.SDK_VERSION] = Constants.SDK_VERSION,
                [Constants.Params.DEV_MODE] = Constants.isDevelopmentModeEnabled.ToString(),
                [Constants.Params.TIME] = Util.GetUnixTimestamp().ToString(),
                [Constants.Params.REQUEST_ID] = request.Id
            };
            if (!string.IsNullOrEmpty(apiConfig.Token))
            {
                args[Constants.Params.TOKEN] = apiConfig.Token;
            }

            args.Merge(request.Parameters);
            return args;
        }
    }
}