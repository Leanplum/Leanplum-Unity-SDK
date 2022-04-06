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
using UnityEngine;

namespace LeanplumSDK
{
    internal class RequestSender
    {
        private EventDataManager eventDataManager;
        protected virtual EventDataManager EventDataManager
        {
            get
            {
                if (eventDataManager == null)
                {
                    eventDataManager = new EventDataManager();
                    return eventDataManager;
                }
                return eventDataManager;
            }
        }

        private RequestBatchFactory requestBatchFactory;
        protected virtual RequestBatchFactory RequestBatchFactory
        {
            get
            {
                if (requestBatchFactory == null)
                {
                    requestBatchFactory = new RequestBatchFactory(EventDataManager);
                    return requestBatchFactory;
                }
                return requestBatchFactory;
            }
        }

        private RequestSenderTimer requestSenderTimer;
        internal virtual RequestSenderTimer RequestSenderTimer
        {
            get
            {
                if (requestSenderTimer == null)
                {
                    requestSenderTimer = new RequestSenderTimer();
                    return requestSenderTimer;
                }
                return requestSenderTimer;
            }
        }

        private string UUID
        {
            get
            {
                string uuid = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.UUID_KEY);
                if (string.IsNullOrEmpty(uuid))
                {
                    uuid = Guid.NewGuid().ToString().ToLower();
                    UUID = uuid;
                }

                return uuid;
            }
            set
            {
                LeanplumNative.CompatibilityLayer.StoreSavedString(Constants.Defaults.UUID_KEY, value);
            }
        }

        internal RequestSender()
        {
        }

        internal void SaveRequest(Request request)
        {
            var args = CreateArgsDictionary(request);
            AttachUuid(args);

            EventDataManager.AddEvent(Json.Serialize(args));
            EventDataManager.AddCallbacks(request);
        }

        internal void AttachUuid(IDictionary<string, object> args)
        {
            int eventsCount = EventDataManager.GetEventsCount();
            if (eventsCount % RequestBatchFactory.MAX_EVENTS_PER_API_CALL == 0)
            {
                UUID = Guid.NewGuid().ToString().ToLower();
            }
            args[Constants.Params.UUID] = UUID;
        }

        public void Send(Request request)
        {
            if (Constants.isNoop)
                return;

            LeanplumUnityHelper.Instance.Enqueue(() => { SendSync(request); });
        }

        public void SendSync(Request request)
        {
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

            if (!Leanplum.ApiConfig.AttachApiKeys(multiRequestArgs))
            {
                return;
            }

            multiRequestArgs[Constants.Params.DATA] = batch.JsonEncoded;
            multiRequestArgs[Constants.Params.SDK_VERSION] = Constants.SDK_VERSION;
            multiRequestArgs[Constants.Params.ACTION] = Constants.Methods.MULTI;
            multiRequestArgs[Constants.Params.TIME] = Util.GetUnixTimestamp().ToString();

            LeanplumNative.CompatibilityLayer.LogDebug($"Sending Request to" +
                $" {Leanplum.ApiConfig.ApiHost}/{Leanplum.ApiConfig.ApiPath}:{Leanplum.ApiConfig.ApiSSL} with Parameters: " +
                Json.Serialize(multiRequestArgs));

            RequestUtil.CreateWebRequest(Leanplum.ApiConfig.ApiHost,
                Leanplum.ApiConfig.ApiPath,
                multiRequestArgs,
                RequestBuilder.POST,
                Leanplum.ApiConfig.ApiSSL,
                Constants.NETWORK_TIMEOUT_SECONDS).GetResponseAsync(delegate (WebResponse response)
                {
                    string responseError = response.GetError();
                    long statusCode = response.GetStatusCode();
                    string log = $"Received response with status code: {statusCode}";

                    if (string.IsNullOrEmpty(responseError))
                    {
                        // Success
                        LeanplumNative.CompatibilityLayer.LogDebug(log);

                        object json = response.GetResponseBodyAsJson();
                        LeanplumNative.CompatibilityLayer.LogDebug($"Received response body: {response.GetResponseBody()}");

                        if (RequestUtil.UpdateApiConfig(json))
                        {
                            // API config is changed and we need to send requests again
                            SendRequests();
                            return;
                        }

                        EventDataManager.InvokeCallbacks(json);

                        RequestBatchFactory.DeleteFinishedBatch(batch);

                        LeanplumNative.CompatibilityLayer.LogDebug("Response Done");
                    }
                    else
                    {
                        // Error
                        LeanplumNative.CompatibilityLayer.LogDebug($"{log} and error: {responseError}");
                        bool connectionError = !string.IsNullOrEmpty(responseError)
                        && responseError.Contains("Could not resolve host");

                        if (statusCode == 408 || statusCode == 429
                        || statusCode == 502 || statusCode == 503 || statusCode == 504)
                        {
                            responseError = "Server is busy; will retry later";
                            LeanplumNative.CompatibilityLayer.LogWarning(responseError);
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
                                IDictionary<string, object> responseDictionary = RequestUtil.GetLastResponse(response.GetResponseBodyAsJson()) as IDictionary<string, object>;
                                if (responseDictionary != null)
                                {
                                    string error = RequestUtil.GetResponseError(responseDictionary);
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

                                        responseError += ", message: " + error;
                                    }
                                }
                            }
                            if (responseError != Constants.NETWORK_TIMEOUT_MESSAGE)
                            {
                                LeanplumNative.CompatibilityLayer.LogError(responseError);
                            }
                        }
                        eventDataManager.InvokeAllCallbacksWithError(new LeanplumException("Error sending request: " + responseError));

                        LeanplumNative.CompatibilityLayer.LogDebug("Response Done");
                    }
                }
            );
        }

        private bool Validate(Request request)
        {
            if (string.IsNullOrEmpty(Leanplum.ApiConfig.AppId))
            {
                LeanplumNative.CompatibilityLayer.LogError("Cannot send request. appId is not set.");
                return false;
            }

            if (string.IsNullOrEmpty(Leanplum.ApiConfig.AccessKey))
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
                [Constants.Params.DEVICE_ID] = Leanplum.ApiConfig.DeviceId,
                [Constants.Params.USER_ID] = Leanplum.ApiConfig.UserId,
                [Constants.Params.SDK_VERSION] = Constants.SDK_VERSION,
                [Constants.Params.DEV_MODE] = Constants.isDevelopmentModeEnabled.ToString(),
                [Constants.Params.TIME] = Util.GetUnixTimestamp().ToString(),
                [Constants.Params.REQUEST_ID] = request.Id
            };
            if (!string.IsNullOrEmpty(Leanplum.ApiConfig.Token))
            {
                args[Constants.Params.TOKEN] = Leanplum.ApiConfig.Token;
            }

            args.Merge(request.Parameters);
            return args;
        }
    }
}