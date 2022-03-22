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
        // TODO: Refactor
        // Move to Leanplum 
        internal ApiConfig apiConfig = new ApiConfig();

        private static object padLock = new object();

        static string uuidKey = "__leanplum_uuid";

        internal EventDataManager eventDataManager = new EventDataManager();
        internal RequestBatchFactory RequestBatchFactory = new RequestBatchFactory();

        private static readonly int MAX_EVENTS_PER_API_CALL = 10000;

        private RequestSender()
        {
        }

        private string UUID
        {
            get
            {
                string uuid = LeanplumNative.CompatibilityLayer.GetSavedString(uuidKey);
                if (string.IsNullOrEmpty(uuid))
                {
                    uuid = new Guid().ToString().ToLower();
                    UUID = uuid;
                }

                return uuid;
            }
            set
            {
                LeanplumNative.CompatibilityLayer.StoreSavedString(uuidKey, value);
            }
        }

        internal void SaveRequest(Request request)
        {
            var args = CreateArgsDictionary(request);
            AttachUuid(args);

            eventDataManager.AddEvent(Json.Serialize(args));
            eventDataManager.AddCallbacks(request);
        }

        internal void AttachUuid(IDictionary<string, object> args)
        {
            int eventsCount = eventDataManager.GetEventsCount();
            if (eventsCount % ApiConfig.MAX_REQUESTS_PER_API_CALL == 0)
            {
                UUID = new Guid().ToString().ToLower();
            }
            args[Constants.Params.UUID] = UUID;
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
            RequestBatch batch = RequestBatchFactory.CreateNextBatch();

            if (batch.IsEmpty)
            {
                return;
            }

            IDictionary<string, string> multiRequestArgs = new Dictionary<string, string>();

            if (!apiConfig.AttachApiKeys(multiRequestArgs))
            {
                return;
            }

            multiRequestArgs[Constants.Params.DATA] = batch.JsonEncoded;
            multiRequestArgs[Constants.Params.SDK_VERSION] = Constants.SDK_VERSION;
            multiRequestArgs[Constants.Params.ACTION] = Constants.Methods.MULTI;
            multiRequestArgs[Constants.Params.TIME] = Util.GetUnixTimestamp().ToString();

            LeanplumNative.CompatibilityLayer.LogDebug("sending: " + Json.Serialize(multiRequestArgs));

            Util.CreateWebRequest(apiConfig.apiHost,
                apiConfig.apiPath,
                multiRequestArgs,
                RequestBuilder.POST,
                apiConfig.apiSSL,
                Constants.NETWORK_TIMEOUT_SECONDS).GetResponseAsync(delegate (WebResponse response)
                {
                    LeanplumNative.CompatibilityLayer.LogDebug("Received response with status code: "
                        + response.GetStatusCode() + " and error: " + response.GetError());

                    if (!string.IsNullOrEmpty(response.GetError()))
                    {
                        string errorMessage = response.GetError();
                        long statusCode = response.GetStatusCode();
                        bool connectionError = !string.IsNullOrEmpty(errorMessage)
                        && errorMessage.Contains("Could not resolve host");

                        if (statusCode == 408 || statusCode == 429
                        || statusCode == 502 || statusCode == 503 || statusCode == 504)
                        {
                            errorMessage = "Server is busy; will retry later";
                            LeanplumNative.CompatibilityLayer.LogWarning(errorMessage);
                        }
                        else if (connectionError)
                        {
                            LeanplumNative.CompatibilityLayer.LogWarning("Could not connect to Leanplum. Will retry later.");
                        }
                        else
                        {
                            object json = response.GetResponseBodyAsJson();
                            if (json != null && json.GetType() == typeof(IDictionary<string, object>))
                            {
                                IDictionary<string, object> responseDictionary = Util.GetLastResponse(response.GetResponseBodyAsJson()) as IDictionary<string, object>;
                                if (responseDictionary != null)
                                {
                                    string error = GetResponseError(responseDictionary);
                                    if (error != null)
                                    {
                                        if (error.StartsWith("App not found"))
                                        {
                                            error = "No app matching the provided app ID was found.";
                                            Constants.isInPermanentFailureState = true;
                                            Constants.isNoop = true;
                                        }
                                        else if (error.StartsWith("Invalid access key"))
                                        {
                                            error = "The access key you provided is not valid for this app.";
                                            Constants.isInPermanentFailureState = true;
                                            Constants.isNoop = true;
                                        }
                                        else if (error.StartsWith("Development mode requested but not permitted"))
                                        {
                                            error = "A call to Leanplum.setAppIdForDevelopmentMode with your production key was made, which is not permitted.";
                                            Constants.isInPermanentFailureState = true;
                                            Constants.isNoop = true;
                                        }

                                        if (Constants.isInPermanentFailureState)
                                        {
                                            RequestBatchFactory.DeleteFinishedBatch(batch);
                                        }

                                        errorMessage += ", message: " + error;
                                    }
                                }
                            }
                            if (errorMessage != Constants.NETWORK_TIMEOUT_MESSAGE)
                            {
                                LeanplumNative.CompatibilityLayer.LogError(errorMessage);
                            }
                        }
                        eventDataManager.InvokeAllCallbacksWithError(new LeanplumException("Error sending request: " + errorMessage));
                    }
                    else
                    {
                        IDictionary<string, object> responseDictionary = Util.GetLastResponse(response.GetResponseBodyAsJson()) as IDictionary<string, object>;
                        LeanplumNative.CompatibilityLayer.LogDebug("received: " + response.GetResponseBody());
                        if (IsResponseSuccess(responseDictionary))
                        {
                            eventDataManager.InvokeCallbacks(response.GetResponseBodyAsJson());
                        }
                        else
                        {
                            string errorMessage = GetResponseError(responseDictionary);
                            LeanplumNative.CompatibilityLayer.LogError(errorMessage);
                            eventDataManager.InvokeAllCallbacksWithError(new LeanplumException(errorMessage));
                        }
                    }
                }
            );
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

        internal static bool IsResponseSuccess(IDictionary<string, object> response)
        {
            object success;
            if (response != null && response.TryGetValue(Constants.Keys.SUCCESS, out success))
            {
                return (bool)success;
            }
            LeanplumNative.CompatibilityLayer.LogError("Invalid response (missing field: success)");
            return false;
        }

        internal static string GetResponseError(IDictionary<string, object> response)
        {
            if (response.ContainsKey(Constants.Keys.ERROR))
            {
                IDictionary<string, object> error = response[Constants.Keys.ERROR] as IDictionary<string, object>;
                if (error != null && error.ContainsKey(Constants.Keys.MESSAGE))
                {
                    return error[Constants.Keys.MESSAGE] as string;
                }
            }
            LeanplumNative.CompatibilityLayer.LogError("Invalid response (missing field: error)");
            return null;
        }
    }
}