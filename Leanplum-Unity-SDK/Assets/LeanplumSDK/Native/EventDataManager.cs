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
using System.Collections.Concurrent;
using System.Collections.Generic;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK
{
    /// <summary>
    /// Manages the event data of Leanplum Requests and Callbacks
    /// Uses ICompatibilityLayer for data persistance
    /// </summary>
    public class EventDataManager
    {
        private readonly ConcurrentDictionary<string, RequestHandler> requestHandlers;

        private static readonly object padLock = new object();

        internal virtual ICompatibilityLayer StorageLayer
        {
            get
            {
                return LeanplumNative.CompatibilityLayer;
            }
        }

        public EventDataManager()
        {
            requestHandlers =
               new ConcurrentDictionary<string, RequestHandler>();

        }

        public void AddEvent(string eventData)
        {
            lock (padLock)
            {
                // TODO: Rewrite as a Queue so incremental indexes are not needed
                // Index is incremental indefinitely unless all requests are sent
                // Possible race condition if max count is reached and request is overridden before
                // unsent requests are deleted - delete count overlaps added/overridden request
                int startIndex = StorageLayer.GetSavedInt(Constants.Defaults.START_KEY, 0);
                int count = StorageLayer.GetSavedInt(Constants.Defaults.COUNT_KEY, 0);
                count++;
                if (count > Constants.MAX_STORED_API_CALLS)
                {
                    // Normalize count
                    count = Constants.MAX_STORED_API_CALLS;
                    // Delete the first request from queue
                    string itemKeyToDelete = string.Format(Constants.Defaults.ITEM_KEY, startIndex);
                    StorageLayer.DeleteSavedSetting(itemKeyToDelete);
                    // Shift the start index to the next request
                    startIndex++;
                    StorageLayer.StoreSavedInt(Constants.Defaults.START_KEY, startIndex);
                }
                string itemKey = string.Format(Constants.Defaults.ITEM_KEY, startIndex + count - 1);
                StorageLayer.StoreSavedString(itemKey, eventData);
                StorageLayer.StoreSavedInt(Constants.Defaults.COUNT_KEY, count);
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
                int start = StorageLayer.GetSavedInt(Constants.Defaults.START_KEY, 0);
                int totalCount = StorageLayer.GetSavedInt(Constants.Defaults.COUNT_KEY, 0);
                if (count == 0 || totalCount == 0)
                {
                    return requestData;
                }

                count = count > totalCount ? totalCount : count;
                for (int i = start; i < start + count; i++)
                {
                    string itemKey = string.Format(Constants.Defaults.ITEM_KEY, i);
                    string itemValue = StorageLayer.GetSavedString(itemKey, null);

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
            return StorageLayer.GetSavedInt(Constants.Defaults.COUNT_KEY, 0);
        }

        public void DeleteEvents(int count)
        {
            lock (padLock)
            {
                int start = StorageLayer.GetSavedInt(Constants.Defaults.START_KEY, 0);
                int totalCount = StorageLayer.GetSavedInt(Constants.Defaults.COUNT_KEY, 0);
                if (count == 0 || totalCount == 0)
                {
                    return;
                }

                count = count > totalCount ? totalCount : count;
                for (int i = start; i < start + count; i++)
                {
                    string itemKey = string.Format(Constants.Defaults.ITEM_KEY, i);
                    StorageLayer.DeleteSavedSetting(itemKey);
                }

                if (count == totalCount)
                {
                    // Reset indices
                    StorageLayer.DeleteSavedSetting(Constants.Defaults.START_KEY);
                    StorageLayer.DeleteSavedSetting(Constants.Defaults.COUNT_KEY);
                }
                else
                {
                    StorageLayer.StoreSavedInt(Constants.Defaults.COUNT_KEY, totalCount - count);
                    // Shift to first request index
                    StorageLayer.StoreSavedInt(Constants.Defaults.START_KEY, start + count);
                }

                StorageLayer.FlushSavedSettings();
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
            RemoveHandlers(keys);
        }

        internal void InvokeCallbacks(object responseJson)
        {
            if (responseJson == null ||
                requestHandlers == null ||
                requestHandlers.Count == 0)
            {
                return;
            }

            List<string> keys = new List<string>();
            foreach (var pair in requestHandlers)
            {
                string reqId = pair.Key;

                var handler = pair.Value;

                var response = RequestUtil.GetResponseForId(responseJson, reqId);
                if (response != null)
                {
                    if (RequestUtil.IsResponseSuccess(response))
                    {
                        handler.OnResponse(response);
                    }
                    else
                    {
                        string err = RequestUtil.GetResponseError(response);
                        handler?.OnError(new LeanplumException(err));
                    }
                    keys.Add(reqId);
                }
            }
            RemoveHandlers(keys);
        }

        private void RemoveHandlers(List<string> keys)
        {
            foreach (string key in keys)
            {
                if (!requestHandlers.TryRemove(key, out _))
                {
                    LeanplumNative.CompatibilityLayer.LogDebug($"Failed to remove request handler with key: {key}");
                }
            }
        }
    }
}