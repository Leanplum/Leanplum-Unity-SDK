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
using System.Collections.Concurrent;
using System.Collections.Generic;
using LeanplumSDK.MiniJSON;
using UnityEngine;

namespace LeanplumSDK
{
    // RequestDataManager
    // UnityDataManager or CompatibilityLayer since it uses it for persistance
    public class EventDataManager
    {
        private readonly ConcurrentDictionary<string, RequestHandler> requestHandlers =
            new ConcurrentDictionary<string, RequestHandler>();

        private static readonly object padLock = new object();

        public EventDataManager()
        {
        }

        public void AddEvent(string eventData)
        {
            lock (padLock)
            {
                // TODO: shorthand LeanplumNative.CompatibilityLayer
                // TODO: index is incremental indefinitely unless all requests are sent
                // TODO: possible race condition if max count is reached and
                // request is overridden before unsent requests are deleted - delete count overlaps added/overridden request
                int startIndex = LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.START_KEY, 0);
                int count = LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.COUNT_KEY, 0);
                Debug.Log($"[Indexes] Count: {count} | startIndex: {startIndex}");
                count++;
                if (count > Constants.MAX_STORED_API_CALLS)
                {
                    // Normalize count
                    count = Constants.MAX_STORED_API_CALLS;
                    // Delete the first request from queue
                    string itemKeyToDelete = string.Format(Constants.Defaults.ITEM_KEY, startIndex);
                    LeanplumNative.CompatibilityLayer.DeleteSavedSetting(itemKeyToDelete);
                    Debug.Log($"[Delete] Count: {count} | Key: {itemKeyToDelete}");
                    // Shift the start index to the next request
                    startIndex++;
                    LeanplumNative.CompatibilityLayer.StoreSavedInt(Constants.Defaults.START_KEY, startIndex);
                }
                string itemKey = string.Format(Constants.Defaults.ITEM_KEY, startIndex + count - 1);
                LeanplumNative.CompatibilityLayer.StoreSavedString(itemKey, eventData);
                Debug.Log($"[Add] Count: {count} | Key: {itemKey}");
                LeanplumNative.CompatibilityLayer.StoreSavedInt(Constants.Defaults.COUNT_KEY, count);
            }
        }

        public void AddCallbacks(Request request)
        {
            var eventHandler = request.GetHandler();
            if (eventHandler != null)
            {
                requestHandlers[request.Id] = eventHandler;
            }
        }

        public IList<IDictionary<string, string>> GetEvents(int count)
        {
            IList<IDictionary<string, string>> requestData = new List<IDictionary<string, string>>();
            lock (padLock)
            {
                int start = LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.START_KEY, 0);
                int totalCount = LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.COUNT_KEY, 0);
                if (count == 0 || totalCount == 0)
                {
                    return requestData;
                }

                count = count > totalCount ? totalCount : count;
                for (int i = start; i < start + count; i++)
                {
                    string itemKey = string.Format(Constants.Defaults.ITEM_KEY, i);
                    string itemValue = LeanplumNative.CompatibilityLayer.GetSavedString(itemKey, null);

                    if (Json.Deserialize(itemValue) is IDictionary<string, object> requestArgs)
                    {
                        IDictionary<string, string> requestArgsAsStrings = new Dictionary<string, string>();
                        foreach (KeyValuePair<string, object> entry in requestArgs)
                        {
                            if (entry.Value != null)
                            {
                                if (entry.Value is IList<object> value)
                                {
                                    // Avoid double json encoding for empty array
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
            }
            return requestData;
        }

        public int GetEventsCount()
        {
            return LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.COUNT_KEY, 0);
        }

        public void DeleteEvents(int count)
        {
            lock (padLock)
            {
                int start = LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.START_KEY, 0);
                int totalCount = LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.COUNT_KEY, 0);
                if (count == 0 || totalCount == 0)
                {
                    return;
                }

                count = count > totalCount ? totalCount : count;
                for (int i = start; i < start + count; i++)
                {
                    string itemKey = string.Format(Constants.Defaults.ITEM_KEY, i);
                    LeanplumNative.CompatibilityLayer.DeleteSavedSetting(itemKey);
                    Debug.Log($"[Delete] Count: {count} | Key: {itemKey}");
                }

                if (count == totalCount)
                {
                    // Reset indices
                    LeanplumNative.CompatibilityLayer.DeleteSavedSetting(Constants.Defaults.START_KEY);
                    LeanplumNative.CompatibilityLayer.DeleteSavedSetting(Constants.Defaults.COUNT_KEY);
                }
                else
                {
                    LeanplumNative.CompatibilityLayer.StoreSavedInt(Constants.Defaults.COUNT_KEY, totalCount - count);
                    // Shift to first request index
                    LeanplumNative.CompatibilityLayer.StoreSavedInt(Constants.Defaults.START_KEY, start + count);
                }

                Debug.Log($"[Indexes] Count: {LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.COUNT_KEY, 0)}" +
                    $" | startIndex: {LeanplumNative.CompatibilityLayer.GetSavedInt(Constants.Defaults.START_KEY, 0)}");

                LeanplumNative.CompatibilityLayer.FlushSavedSettings();
            }
        }

        internal void InvokeAllCallbacksWithError(LeanplumException leanplumException)
        {
            if (requestHandlers.Count == 0)
            {
                return;
            }

            List<string> keys = new List<string>();

            foreach (var pair in requestHandlers)
            {
                string reqId = pair.Key;
                var handler = pair.Value;

                handler?.OnError(leanplumException);
                keys.Add(reqId);
            }

            //requestHandlers.TryRemove()
        }

        internal void InvokeCallbacks(object responseJson)
        {
            if (responseJson == null || requestHandlers.Count == 0)
            {
                return;
            }

            List<string> keys = new List<string>();
            foreach (var pair in requestHandlers)
            {
                string reqId = pair.Key;
                
                var handler = pair.Value;

                var response = Util.GetResponseForId(responseJson, reqId);
                if (response != null)
                {
                    if (RequestSender.IsResponseSuccess(response))
                    {
                        handler.OnResponse(response);
                    }
                    else
                    {
                        string err = RequestSender.GetResponseError(response);

                        handler?.OnError(new LeanplumException(err));
                    }
                }
              
                keys.Add(reqId);
            }
            //requestHandlers.TryRemove()
        }
    }
}