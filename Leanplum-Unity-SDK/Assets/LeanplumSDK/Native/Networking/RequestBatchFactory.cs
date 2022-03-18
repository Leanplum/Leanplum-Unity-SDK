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
using LeanplumSDK;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK
{
    public class RequestBatchFactory
    {
        public RequestBatchFactory()
        {
        }

        public void CreateNextBatch()
        {

        }

        internal static IList<IDictionary<string, string>> GetUnsentRequests()
        {
            IList<IDictionary<string, string>> requestData = new List<IDictionary<string, string>>();
            lock (padLock)
            {
                int start = LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.START_KEY, 0);
                int count = LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.COUNT_KEY, 0);
                if (count == 0)
                {
                    return requestData;
                }
                LeanplumNative.CompatibilityLayer.DeleteSavedSetting(Constants.Defaults.START_KEY);
                LeanplumNative.CompatibilityLayer.DeleteSavedSetting(Constants.Defaults.COUNT_KEY);
                for (int i = start; i < start + count; i++)
                {
                    string itemKey = string.Format(Constants.Defaults.ITEM_KEY, i);
                    string itemValue = LeanplumNative.CompatibilityLayer.GetSavedString(itemKey, null);
                    if (itemValue != null)
                    {
                        IDictionary<string, object> requestArgs =
                            Json.Deserialize(itemValue) as IDictionary<string, object>;
                        if (requestArgs != null)
                        {
                            IDictionary<string, string> requestArgsAsStrings = new Dictionary<string, string>();
                            foreach (KeyValuePair<string, object> entry in requestArgs)
                            {
                                if (entry.Value != null)
                                {
                                    if (entry.Value is IList<object> value)
                                    {
                                        // avoid double json encoding for empty array
                                        if (value.Count == 0)
                                        {
                                            requestArgsAsStrings[entry.Key] = "[]";
                                        }
                                        else
                                        {
                                            requestArgsAsStrings[entry.Key] = entry.Value.ToString();
                                        }
                                    }
                                    else
                                    {
                                        requestArgsAsStrings[entry.Key] = entry.Value.ToString();
                                    }
                                }
                            }
                            requestData.Add(requestArgsAsStrings);
                        }
                    }
                    LeanplumNative.CompatibilityLayer.DeleteSavedSetting(itemKey);
                }
                LeanplumNative.CompatibilityLayer.FlushSavedSettings();
            }
            return requestData;
        }
    }
}