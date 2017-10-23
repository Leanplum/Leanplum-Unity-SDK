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
using LeanplumSDK.MiniJSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace LeanplumSDK
{
    /// <summary>
    ///     Leanplum request class.
    /// </summary>
    internal class LeanplumRequest
    {
        public delegate void NoPendingDownloadsHandler();

        private static object padLock = new object();
        private readonly string apiMethod;
        private readonly string httpMethod;
        private readonly string resourceUrl;
        private readonly IDictionary<string, string> parameters;

        public LeanplumRequest(string httpMethod, string resourceUrl)
        {
            this.httpMethod = httpMethod;
            this.resourceUrl = resourceUrl;
        }

        public LeanplumRequest(string httpMethod, string apiMethod, IDictionary<string, string> parameters)
        {
            this.httpMethod = httpMethod;
            this.apiMethod = apiMethod;
            this.parameters = parameters ?? new Dictionary<string, string>();
        }

        public static string AppId { get; private set; }

        public static string DeviceId { get; internal set; }

        public static string AccesssKey { get; private set; }

        public static string Token { get; set; }

        public static string UserId { get; set; }

        public static int PendingDownloads { get; private set; }

        private static event NoPendingDownloadsHandler noPendingDownloads;

        public static event NoPendingDownloadsHandler NoPendingDownloads
        {
            add
            {
                noPendingDownloads += value;
                if (PendingDownloads == 0)
                {
                    value();
                }
            }
            remove { noPendingDownloads -= value; }
        }

        public event Action<object> Response;
        public event Action<Exception> Error;

        protected virtual void OnError(Exception obj)
        {
            Action<Exception> handler = Error;
            if (handler != null)
                handler(obj);
        }

        protected virtual void OnResponse(object obj)
        {
            Action<object> handler = Response;
            if (handler != null)
                handler(obj);
        }

        public static void ClearNoPendingDownloads()
        {
            noPendingDownloads = null;
        }

        public static void SetAppId(string appId, string accessKey)
        {
            AppId = appId;
            AccesssKey = accessKey;
        }

        public static LeanplumRequest Get(string resourceUrl)
        {
            return new LeanplumRequest("GET", resourceUrl);
        }

        public static LeanplumRequest Get(string apiMethod, IDictionary<string, string> parameters)
        {
            return new LeanplumRequest("GET", apiMethod, parameters);
        }

        public static LeanplumRequest Post(string apiMethod, IDictionary<string, string> parameters)
        {
            return new LeanplumRequest("POST", apiMethod, parameters);
        }

        internal virtual IDictionary<string, string> CreateArgsDictionary()
        {
            IDictionary<string, string> args = new Dictionary<string, string>();
            args [Constants.Params.DEVICE_ID] = DeviceId;
            args [Constants.Params.ACTION] = apiMethod;
            args [Constants.Params.USER_ID] = UserId;
            args [Constants.Params.SDK_VERSION] = SharedConstants.SDK_VERSION;
            args [Constants.Params.DEV_MODE] = Constants.isDevelopmentModeEnabled.ToString();
            args [Constants.Params.TIME] = Util.GetUnixTimestamp().ToString();
            if (Token != null)
            {
                args [Constants.Params.TOKEN] = Token;
            }

            foreach (var param in parameters)
            {
                args [param.Key] = param.Value;
            }

            return args;
        }

        internal virtual void DownloadAssetNow()
        {
            PendingDownloads++;
            Util.CreateWebRequest(Constants.API_HOST_NAME, resourceUrl, null, httpMethod,
                                  Constants.API_SSL, Constants.NETWORK_TIMEOUT_SECONDS).GetAssetBundle(
                delegate(WebResponse response)
            {
                PendingDownloads--;
                if (response.GetError() != null)
                {
                    OnError(new LeanplumException("Error sending request: " + response.GetError()));
                }
                else
                {
                    OnResponse(response.GetResponseAsAsset());
                }
                if (PendingDownloads == 0)
                {
                    if (noPendingDownloads != null)
                    {
                        noPendingDownloads();
                    }
                }
            }
            );
        }

        internal static void SaveRequestForLater(IDictionary<string, string> args)
        {
            lock (padLock)
            {
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
				LeanplumNative.CompatibilityLayer.StoreSavedString(itemKey, Json.Serialize(args));
				LeanplumNative.CompatibilityLayer.StoreSavedInt(Constants.Defaults.COUNT_KEY, count);
            }
        }

        internal virtual void Send()
        {
			if (Constants.isDevelopmentModeEnabled && LeanplumNative.CompatibilityLayer.IsConnected())
            {
                SendNow();
            }
            else
            {
                SendEventually();
            }
        }

        internal virtual void SendIfConnected()
        {
			if (LeanplumNative.CompatibilityLayer.IsConnected())
            {
                SendNow();
            }
            else
            {
                SendEventually();
				OnError(new Exception("Device is offline"));
            }
        }

        internal virtual bool AttachApiKeys(IDictionary<string, string> dict)
        {
            if (String.IsNullOrEmpty(AppId) || String.IsNullOrEmpty(AccesssKey))
            {
				LeanplumNative.CompatibilityLayer.LogError("API keys are not set. Please use " +
                    "Leanplum.SetAppIdForDevelopmentMode or " +
                    "Leanplum.SetAppIdForProductionMode.");
                return false;
            }
            dict [Constants.Params.APP_ID] = AppId;
            dict [Constants.Params.CLIENT_KEY] = AccesssKey;
            dict [Constants.Params.CLIENT] = Constants.CLIENT_PREFIX + '-' +
				LeanplumNative.CompatibilityLayer.GetPlatformName().ToLower();
            return true;
        }

        internal virtual void SendNow()
        {
            if (Constants.isNoop)
            {
                return;
            }
            if (AppId == null)
            {
				LeanplumNative.CompatibilityLayer.LogError("Cannot send request. appId is not set.");
                return;
            }
            if (AccesssKey == null)
            {
				LeanplumNative.CompatibilityLayer.LogError("Cannot send request. accessKey is not set.");
                return;
            }

            SendEventually();

            IList<IDictionary<string, string>> requestsToSend = PopUnsentRequests();
            IDictionary<string, string> multiRequestArgs = new Dictionary<string, string>();
            multiRequestArgs [Constants.Params.DATA] = JsonEncodeUnsentRequests(requestsToSend);
            multiRequestArgs [Constants.Params.SDK_VERSION] = SharedConstants.SDK_VERSION;
            multiRequestArgs [Constants.Params.ACTION] = Constants.Methods.MULTI;
            multiRequestArgs [Constants.Params.TIME] = Util.GetUnixTimestamp().ToString();
            if (!AttachApiKeys(multiRequestArgs))
            {
                return;
            }

			LeanplumNative.CompatibilityLayer.LogDebug("sending: " + Json.Serialize(multiRequestArgs));

            Util.CreateWebRequest(Constants.API_HOST_NAME, Constants.API_SERVLET, multiRequestArgs, httpMethod,
                Constants.API_SSL, Constants.NETWORK_TIMEOUT_SECONDS).GetResponseAsync(
                    delegate(WebResponse response)
            {
                if (!String.IsNullOrEmpty(response.GetError()))
                {
                    // Parse out the http error code by taking the first 3 characters.
					string errorMessage = null;
                    string statusCode = null;
                    if (response.GetError() [0] == '4' || response.GetError() [0] == '5')
                    {
                        statusCode = response.GetError().Substring(0, 3);
                    }
                    else
                    {
                        statusCode = response.GetError();
                    }
					bool connectionError = response.GetError() != null &&
						response.GetError().Contains("Could not resolve host");
					if (statusCode == "408" || statusCode == "502" || statusCode == "503" || statusCode == "504")
                    {
                        errorMessage = "Server is busy; will retry later";
						LeanplumNative.CompatibilityLayer.LogWarning(errorMessage);
                        PushUnsentRequests(requestsToSend);
                    }
					else if (connectionError)
					{
						LeanplumNative.CompatibilityLayer.LogWarning("Could not connect to Leanplum. Will retry later.");
						PushUnsentRequests(requestsToSend);
					}
                    else
                    {
                        errorMessage = statusCode;
                        object json = response.GetResponseBodyAsJson();
                        if (json != null && json.GetType() == typeof(IDictionary<string, object>))
                        {
                            IDictionary<string, object> responseDictionary =
                                        Util.GetLastResponse(response.GetResponseBodyAsJson()) as IDictionary<string, object>;
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
                                    errorMessage += ", message: " + error;
                                }
                            }
                        }
						if (errorMessage != Constants.NETWORK_TIMEOUT_MESSAGE)
						{
							LeanplumNative.CompatibilityLayer.LogError(errorMessage);
						}
                    }
                    OnError(new LeanplumException("Error sending request: " + errorMessage));
                }
                else
                {
                    IDictionary<string, object> responseDictionary =
                                Util.GetLastResponse(response.GetResponseBodyAsJson()) as IDictionary<string, object>;
					LeanplumNative.CompatibilityLayer.LogDebug("received: " + response.GetResponseBody());
                    if (IsResponseSuccess(responseDictionary))
                    {
                        OnResponse(response.GetResponseBodyAsJson());
                    }
                    else
                    {
                        string errorMessage = GetResponseError(responseDictionary);
						LeanplumNative.CompatibilityLayer.LogError(errorMessage);
                        OnError(new LeanplumException(errorMessage));
                    }
                }
            }
            );
        }

        internal virtual void SendEventually()
        {
            if (Constants.isNoop)
            {
                return;
            }
            IDictionary<string, string> args = CreateArgsDictionary();
            SaveRequestForLater(args);
        }

        internal static IList<IDictionary<string, string>> PopUnsentRequests()
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
                    string itemKey = String.Format(Constants.Defaults.ITEM_KEY, i);
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
		                        	requestArgsAsStrings [entry.Key] = entry.Value.ToString();
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

        internal static string JsonEncodeUnsentRequests(IList<IDictionary<string, string>> requestData)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data [Constants.Params.DATA] = requestData;
            return Json.Serialize(data);
        }

        internal static void PushUnsentRequests(IList<IDictionary<string, string>> requestData)
        {
            foreach (var args in requestData)
            {
                SaveRequestForLater(args);
            }
        }

        internal static bool IsResponseSuccess(IDictionary<string, object> response)
        {
            object success;
            if (response.TryGetValue(Constants.Keys.SUCCESS, out success))
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
                IDictionary<string, object> error = response [Constants.Keys.ERROR] as IDictionary<string, object>;
                if (error != null && error.ContainsKey(Constants.Keys.MESSAGE))
                {
                    return error [Constants.Keys.MESSAGE] as string;
                }
            }
			LeanplumNative.CompatibilityLayer.LogError("Invalid response (missing field: error)");
            return null;
        }
    }
}
