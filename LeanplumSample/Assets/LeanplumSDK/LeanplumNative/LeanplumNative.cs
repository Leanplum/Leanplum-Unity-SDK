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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeanplumSDK
{
    /// <summary>
    ///     Leanplum Unity SDK.
    /// </summary>
    public class LeanplumNative : LeanplumSDKObject
    {
        internal static bool calledStart;
        private static bool startSuccessful;
        private static bool isPaused;
        private static string customDeviceId;
        internal static ICompatibilityLayer CompatibilityLayer = new UnityCompatibilityLayer();
#if !UNITY_WEBGL
        internal static LeanplumSocket leanplumSocket;
#endif
        internal static bool isStopped;

        #region Helpers
        private static void ValidateAttributes(IDictionary<string, object> attributes)
        {
            if (attributes != null)
            {
                foreach (object value in attributes.Values)
                {
                    if (!(Util.IsNumber(value)) && !(value is string) && !(value is bool?))
                    {
                        Util.MaybeThrow(new LeanplumException(
                            "userAttributes values must be of type string, number type, or bool."));
                    }
                }
            }
        }
        #endregion

        private static bool _hasStarted;

        #region Accessors and Mutators
        /// <summary>
        ///     Gets a value indicating whether Leanplum has finished starting.
        /// </summary>
        /// <value><c>true</c> if this instance has started; otherwise, <c>false</c>.</value>
        public override bool HasStarted()
        {
            return _hasStarted;
        }

        private static bool _HasStartedAndRegisteredAsDeveloper;
        /// <summary>
        ///     Gets a value indicating whether Leanplum has started and the device is registered as
        ///     a developer.
        /// </summary>
        /// <value>
        ///     <c>true</c> if Leanplum has started and the device registered as developer;
        ///     otherwise,
        ///     <c>false</c>.
        /// </value>
        public override bool HasStartedAndRegisteredAsDeveloper()
        {
            return _HasStartedAndRegisteredAsDeveloper;
        }

        /// <summary>
        ///     Gets whether or not developer mode is enabled.
        /// </summary>
        /// <value><c>true</c> if developer mode; otherwise, <c>false</c>.</value>
        public override bool IsDeveloperModeEnabled()
        {
            return Constants.isDevelopmentModeEnabled;
        }

        /// <summary>
        ///     Optional. Sets the API server. The API path is of the form
        ///     http[s]://hostname/servletName
        /// </summary>
        /// <param name="hostName"> The name of the API host, such as www.leanplum.com </param>
        /// <param name="servletName"> The name of the API servlet, such as api </param>
        /// <param name="useSSL"> Whether to use SSL </param>
        public override void SetApiConnectionSettings(string hostName, string servletName = "api",
            bool useSSL = true)
        {
            Constants.API_HOST_NAME = hostName;
            Constants.API_SERVLET = servletName;
            Constants.API_SSL = useSSL;
        }

        /// <summary>
        ///     Optional. Sets the socket server path for Development mode. Path is of the form
        ///     hostName:port
        /// </summary>
        /// <param name="hostName"> The host name of the socket server. </param>
        /// <param name="port"> The port to connect to. </param>
        public override void SetSocketConnectionSettings(string hostName, int port)
        {
            Constants.SOCKET_HOST = hostName;
            Constants.SOCKET_PORT = port;
        }

        /// <summary>
        ///     The default timeout is 10 seconds for requests, and 15 seconds for file downloads.
        /// </summary>
        /// <param name="seconds"> Timeout in seconds for standard webrequests. </param>
        /// <param name="downloadSeconds"> Timeout in seconds for downloads. </param>
        public override void SetNetworkTimeout(int seconds, int downloadSeconds)
        {
            Constants.NETWORK_TIMEOUT_SECONDS = seconds;
            Constants.NETWORK_TIMEOUT_SECONDS_FOR_DOWNLOADS = downloadSeconds;
        }

        /// <summary>
        ///     Must call either this or SetAppIdForProductionMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your development key. </param>
        public override void SetAppIdForDevelopmentMode(string appId, string accessKey)
        {
            Constants.isDevelopmentModeEnabled = true;
            LeanplumRequest.SetAppId(appId, accessKey);
        }

        /// <summary>
        ///     Must call either this or SetAppIdForDevelopmentMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your production key. </param>
        public override void SetAppIdForProductionMode(string appId, string accessKey)
        {
            Constants.isDevelopmentModeEnabled = false;
            LeanplumRequest.SetAppId(appId, accessKey);
        }

        /// <summary>
        ///     Set the application version to be sent to Leanplum.
        /// </summary>
        /// <param name="version">Version.</param>
        public override void SetAppVersion(string version)
        {
            CompatibilityLayer.VersionName = version;
        }

        /// <summary>
        ///     Sets a custom device ID. Device IDs should be unique across physical devices.
        /// </summary>
        /// <param name="deviceId">Device identifier.</param>
        public override void SetDeviceId(string deviceId)
        {
            customDeviceId = deviceId;
        }

        /// <summary>
        ///     This should be your first statement in a unit test. Setting this to true
        ///     will prevent Leanplum from communicating with the server.
        /// </summary>
        public override void SetTestMode(bool testModeEnabled)
        {
            if (calledStart)
            {
                CompatibilityLayer.LogWarning("Leanplum was already started. Call SetTestMode " +
                    "before calling Start.");
            }
            Constants.isNoop = testModeEnabled;
        }

        /// <summary>
        ///     Sets whether realtime updates to the client are enabled in development mode.
        ///     This uses websockets which can have high CPU impact. Default: true.
        /// </summary>
        public override void SetRealtimeUpdatesInDevelopmentModeEnabled(bool enabled)
        {
            Constants.EnableRealtimeUpdatesInDevelopmentMode = enabled;
        }

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public override object ObjectForKeyPath(params object[] components)
        {
            return VarCache.GetMergedValueFromComponentArray(components);
        }

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public override object ObjectForKeyPathComponents(object[] pathComponents)
        {
                return VarCache.GetMergedValueFromComponentArray(pathComponents);
        }
        #endregion

        #region Callbacks
        private static event Leanplum.VariableChangedHandler variablesChanged;
        private static event Leanplum.VariablesChangedAndNoDownloadsPendingHandler
            variablesChangedAndNoDownloadsPending;
        private static event Leanplum.StartHandler started;

        /// <summary>
        ///     Invoked when the variables receive new values from the server.
        ///     This will be called on start, and also later on if the user is in an
        ///     experiment that can update in realtime.
        ///     If you subscribe to this and variables have already been received, it will be
        ///     invoked immediately.
        /// </summary>
        public override event Leanplum.VariableChangedHandler VariablesChanged
        {
            add
            {
                variablesChanged += value;
                if (VarCache.HasReceivedDiffs)
                    value();
            }
            remove
            {
                variablesChanged -= value;
            }
        }

        /// <summary>
        ///     Invoked when no more file downloads are pending (either when
        ///     no files needed to be downloaded or all downloads have been completed).
        /// </summary>
        public override event Leanplum.VariablesChangedAndNoDownloadsPendingHandler
            VariablesChangedAndNoDownloadsPending
        {
            add
            {
                variablesChangedAndNoDownloadsPending += value;
                if (_hasStarted && VarCache.HasReceivedDiffs)
                    value();
            }
            remove
            {
                variablesChangedAndNoDownloadsPending -= value;
            }
        }

        /// <summary>
        ///     Invoked when the start call finishes, and variables are
        ///     returned back from the server.
        /// </summary>
        public override event Leanplum.StartHandler Started
        {
            add
            {
                started += value;
                if (_hasStarted)
                {
                    value(startSuccessful);
                }
            }
            remove
            {
                started -= value;
            }
        }

        private static void OnStarted(bool success)
        {
            if (started != null)
            {
                started(success);
            }
        }

        internal static void OnHasStartedAndRegisteredAsDeveloper()
        {
            _HasStartedAndRegisteredAsDeveloper = true;
        }

        private static void OnVariablesChanged()
        {
            if (variablesChanged != null)
            {
                variablesChanged();
            }
        }

        private static void OnVariablesChangedAndNoDownloadsPending()
        {
            if (variablesChangedAndNoDownloadsPending != null)
            {
                variablesChangedAndNoDownloadsPending();
            }
        }

        #endregion

        #region API Calls
        /// <summary>
        ///     Call this when your application starts.
        ///     This will initiate a call to Leanplum's servers to get the values
        ///     of the variables used in your app.
        /// </summary>
        public override void Start(string userId, IDictionary<string, object> attributes,
            Leanplum.StartHandler startResponseAction)
        {
            if (calledStart)
            {
                CompatibilityLayer.Log("Already called start");
                return;
            }
            if (String.IsNullOrEmpty(LeanplumRequest.AppId))
            {
                CompatibilityLayer.LogError("You cannot call Start without setting your app's " +
                    "API keys.");
                return;
            }

            if (CompatibilityLayer.GetPlatformName() == "Standalone")
            {
                // TODO: Fix this.
                // This is a workaround for SSL issues when running on
                // standalone when connecting with the production server.
                // No issue on staging.
                Constants.API_SSL = false;
            }

            if (startResponseAction != null)
            {
                Started += startResponseAction;
            }
            if (Constants.isNoop)
            {
                _hasStarted = true;
                startSuccessful = true;
                OnVariablesChanged();
                OnVariablesChangedAndNoDownloadsPending();
                OnStarted(true);
                VarCache.ApplyVariableDiffs(null, null);
                return;
            }

            ValidateAttributes(attributes);
            calledStart = true;

            // Load the variables that were stored on the device from the last session.
            VarCache.IsSilent = true;
            VarCache.LoadDiffs();
            VarCache.IsSilent = false;

            // Setup class members.
            VarCache.Update += delegate
            {
                OnVariablesChanged();
                if (LeanplumRequest.PendingDownloads == 0)
                {
                    OnVariablesChangedAndNoDownloadsPending();
                }
            };

            LeanplumRequest.NoPendingDownloads += delegate
            {
                OnVariablesChangedAndNoDownloadsPending();
            };

            string deviceId;
            if (customDeviceId != null)
            {
                deviceId = customDeviceId;
            }
            else
            {
                deviceId = CompatibilityLayer.GetDeviceId();
            }
            LeanplumRequest.DeviceId = deviceId;

            // Don't overwrite UserID if it was set previously if Start()
            // was called without a new UserID.
            if (!String.IsNullOrEmpty(userId))
            {
                LeanplumRequest.UserId = userId;
            }
            if (String.IsNullOrEmpty(LeanplumRequest.UserId))
            {
                LeanplumRequest.UserId = deviceId;
            }

            // Setup parameters.
            var parameters = new Dictionary<string, string>();
            parameters[Constants.Params.INCLUDE_DEFAULTS] = false.ToString();
            parameters[Constants.Params.VERSION_NAME] = CompatibilityLayer.VersionName ?? "";
            parameters[Constants.Params.DEVICE_NAME] = CompatibilityLayer.GetDeviceName();
            parameters[Constants.Params.DEVICE_MODEL] = CompatibilityLayer.GetDeviceModel();
            parameters[Constants.Params.DEVICE_SYSTEM_NAME] = CompatibilityLayer.GetSystemName();
            parameters[Constants.Params.DEVICE_SYSTEM_VERSION] =
                CompatibilityLayer.GetSystemVersion();
            TimeZone timezone = System.TimeZone.CurrentTimeZone;
            if (timezone.IsDaylightSavingTime(DateTime.UtcNow))
            {
                parameters[Constants.Keys.TIMEZONE] = timezone.DaylightName;
            }
            else
            {
                parameters[Constants.Keys.TIMEZONE] = timezone.StandardName;
            }
            parameters[Constants.Keys.TIMEZONE_OFFSET_SECONDS] =
                timezone.GetUtcOffset(DateTime.UtcNow).TotalSeconds.ToString();
            parameters[Constants.Keys.COUNTRY] = Constants.Values.DETECT;
            parameters[Constants.Keys.REGION] = Constants.Values.DETECT;
            parameters[Constants.Keys.CITY] = Constants.Values.DETECT;
            parameters[Constants.Keys.LOCATION] = Constants.Values.DETECT;
            if (attributes != null)
            {
                parameters[Constants.Params.USER_ATTRIBUTES] = Json.Serialize(attributes);
            }

            // Issue start API call.
            LeanplumRequest req = LeanplumRequest.Post(Constants.Methods.START, parameters);
            req.Response += delegate(object responsesObject)
            {
                IDictionary<string, object> response =
                    Util.GetLastResponse(responsesObject) as IDictionary<string, object>;
                IDictionary<string, object> values =
                    Util.GetValueOrDefault(response, Constants.Keys.VARS) as
                    IDictionary<string, object> ?? new Dictionary<string, object>();
                IDictionary<string, object> fileAttributes =
                    Util.GetValueOrDefault(response, Constants.Keys.FILE_ATTRIBUTES) as
                    IDictionary<string, object> ?? new Dictionary<string, object>();
                List<object> variants = Util.GetValueOrDefault(response, Constants.Keys.VARIANTS) as
                    List<object> ?? new List<object>();
                bool isRegistered = (bool) Util.GetValueOrDefault(response,
                    Constants.Keys.IS_REGISTERED, false);

                LeanplumRequest.Token = Util.GetValueOrDefault(response, Constants.Keys.TOKEN) as
                    string ?? "";

                // Allow bidirectional realtime variable updates.
                if (Constants.isDevelopmentModeEnabled)
                {
                    VarCache.SetDevModeValuesFromServer(
                        Util.GetValueOrDefault(response, Constants.Keys.VARS_FROM_CODE) as
                        Dictionary<string, object>);

#if !UNITY_WEBGL
                    if (Constants.EnableRealtimeUpdatesInDevelopmentMode &&
                        SocketUtilsFactory.Utils.AreSocketsAvailable)
                    {
                        leanplumSocket = new LeanplumSocket(delegate ()
                        {
                            // Callback when we receive an updateVars command through the
                            // development socket.
                            // Set a flag so that the next time VarCache.CheckVarsUpdate() is
                            // called the variables are updated.
                            VarCache.VarsNeedUpdate = true;
                        });
                    }
#endif
                    // Register device.
                    if (isRegistered)
                    {
                        // Check for updates.
                        string latestVersion = Util.GetValueOrDefault(response,
                            Constants.Keys.LATEST_VERSION) as string;
                        if (latestVersion != null)
                        {
                            CompatibilityLayer.Log("Leanplum Unity SDK " + latestVersion +
                                " available. Go to https://www.leanplum.com/dashboard to " +
                                "download it.");
                        }
                        OnHasStartedAndRegisteredAsDeveloper();
                    }
                }

                VarCache.ApplyVariableDiffs(values, fileAttributes, variants);
                _hasStarted = true;
                startSuccessful = true;
                OnStarted(true);
                CompatibilityLayer.Init();
            };
            req.Error += delegate
            {
                VarCache.ApplyVariableDiffs(null, null);
                _hasStarted = true;
                startSuccessful = false;
                OnStarted(false);
                CompatibilityLayer.Init();
            };
            req.SendIfConnected();
        }

        public override void TrackGooglePlayPurchase(string item, long priceMicros,
            string currencyCode, string purchaseData, string dataSignature,
            IDictionary<string, object> parameters)
        {
            IDictionary<string, object> modifiedParams;
            if (parameters == null)
            {
                modifiedParams = new Dictionary<string, object>();
            }
            else
            {
                modifiedParams = new Dictionary<string, object>(parameters);
            }
            modifiedParams.Add("item", item);
            IDictionary<string, string> arguments = new Dictionary<string, string>();
            arguments["googlePlayPurchaseData"] = purchaseData;
            arguments["googlePlayPurchaseDataSignature"] = dataSignature;
            arguments["currencyCode"] = currencyCode;
            Track(PURCHASE_EVENT_NAME, priceMicros / 1000000.0, null, modifiedParams, arguments);
        }

        public override void TrackIOSInAppPurchase(string item, double unitPrice, int quantity,
            string currencyCode, string transactionIdentifier, string receiptData,
            IDictionary<string, object> parameters)
        {
            IDictionary<string, object> modifiedParams;
            if (parameters == null)
            {
                modifiedParams = new Dictionary<string, object>();
            }
            else
            {
                modifiedParams = new Dictionary<string, object>(parameters);
            }
            modifiedParams.Add("item", item);
            modifiedParams.Add("quantity", quantity);
            IDictionary<string, string> arguments = new Dictionary<string, string>();
            arguments["iOSTransactionIdentifier"] = transactionIdentifier;
            arguments["iOSReceiptData"] = receiptData;
            arguments["iOSSandbox"] = IsDeveloperModeEnabled() ? "true" : "false";
            arguments["currencyCode"] = currencyCode;
            Track(PURCHASE_EVENT_NAME, unitPrice * quantity, null, modifiedParams, arguments);
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public override void Track(string eventName, double value, string info,
            IDictionary<string, object> parameters)
        {
            Track(eventName, value, info, parameters, null);
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public void Track(string eventName, double value, string info,
            IDictionary<string, object> parameters, IDictionary<string, string> arguments)
        {
            if (Constants.isNoop)
            {
                return;
            }
            if (!calledStart)
            {
                CompatibilityLayer.LogError("You cannot call Track before calling Start.");
                return;
            }
            IDictionary<string, string> requestParams = new Dictionary<string, string>();
            requestParams[Constants.Params.EVENT] = eventName;
            requestParams[Constants.Params.VALUE] = value.ToString();
            if (info != null)
            {
                requestParams [Constants.Params.INFO] = info;
            }
            if (parameters != null)
            {
                requestParams[Constants.Params.PARAMS] = Json.Serialize(parameters);
            }
            if (arguments != null)
            {
                foreach (string argName in arguments.Keys)
                {
                    requestParams[argName] = arguments[argName];
                }
            }
            LeanplumRequest.Post(Constants.Methods.TRACK, requestParams).Send();
        }

        /// <summary>
        ///     Sets the traffic source info for the current user.
        ///     Keys in info must be one of: publisherId, publisherName, publisherSubPublisher,
        ///     publisherSubSite, publisherSubCampaign, publisherSubAdGroup, publisherSubAd.
        /// </summary>
        public override void SetTrafficSourceInfo(IDictionary<string, string> info)
        {
            if (Constants.isNoop)
            {
                return;
            }
            if (!calledStart)
            {
                CompatibilityLayer.LogError("You cannot call SetTrafficSourceInfo before calling " +
                  "Start.");
                return;
            }
            IDictionary<string, string> requestParams = new Dictionary<string, string>();
            requestParams[Constants.Params.TRAFFIC_SOURCE] = Json.Serialize(info);
            LeanplumRequest.Post(Constants.Methods.SET_TRAFFIC_SOURCE_INFO, requestParams).Send();
        }

        /// <summary>
        ///     Advances to a particular state in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     A state is a section of your app that the user is currently in.
        /// </summary>
        public override void AdvanceTo(string state, string info,
            IDictionary<string, object> parameters)
        {
            if (Constants.isNoop)
            {
                return;
            }
            if (!calledStart)
            {
                CompatibilityLayer.LogError("You cannot call AdvanceTo before calling Start.");
                return;
            }

            IDictionary<string, string> requestParams = new Dictionary<string, string>();
            requestParams[Constants.Params.INFO] = info;
            if (state != null)
            {
                requestParams[Constants.Params.STATE] = state;
            }
            if (parameters != null)
            {
                requestParams[Constants.Params.PARAMS] = Json.Serialize(parameters);
            }
            LeanplumRequest.Post(Constants.Methods.ADVANCE, requestParams).Send();
        }

        /// <summary>
        ///     Updates the user ID and adds or modifies user attributes.
        /// </summary>
        /// <param name="newUserId">New user identifier.</param>
        /// <param name="value">User attributes.</param>
        public override void SetUserAttributes(string newUserId, IDictionary<string, object> value)
        {
            if (!calledStart)
            {
                CompatibilityLayer.LogWarning("Start was not called. Set user ID and attributes " +
                    "as the arguments when calling Start.");
                return;
            }
            if (Constants.isNoop)
            {
                return;
            }

            var parameters = new Dictionary<string, string>();
            if (value != null)
            {
                ValidateAttributes(value);
                parameters[Constants.Params.USER_ATTRIBUTES] = Json.Serialize(value);
            }
            if (!String.IsNullOrEmpty(newUserId))
            {
                parameters[Constants.Params.NEW_USER_ID] = newUserId;
                VarCache.SaveDiffs();
            }
            LeanplumRequest.Post(Constants.Methods.SET_USER_ATTRIBUTES, parameters).Send();

            if (!String.IsNullOrEmpty(newUserId))
            {
                LeanplumRequest.UserId = newUserId;
                if (_hasStarted)
                {
                    VarCache.SaveDiffs();
                }
            }
        }

        /// <summary>
        ///     Pauses the current state.
        ///     You can use this if your game has a "pause" mode. You shouldn't call it
        ///     when someone switches out of your app because that's done automatically.
        /// </summary>
        public override void PauseState()
        {
            if (Constants.isNoop)
            {
                return;
            }
            if (!calledStart)
            {
                CompatibilityLayer.LogError("You cannot call PauseState before calling start.");
                return;
            }
            LeanplumRequest.Post(Constants.Methods.PAUSE_STATE,
                new Dictionary<string, string>()).Send();
        }

        /// <summary>
        ///     Resumes the current state.
        /// </summary>
        public override void ResumeState()
        {
            if (Constants.isNoop)
            {
                return;
            }
            if (!calledStart)
            {
                CompatibilityLayer.LogError("You cannot call ResumeState before calling start.");
                return;
            }
            LeanplumRequest.Post(Constants.Methods.RESUME_STATE,
                new Dictionary<string, string>()).Send();
        }

        /// <summary>
        ///     Return variant ids.
        ///     Used only for debugging purposes and advanced use cases.
        /// </summary>
        public override List<object> Variants()
        {
            return VarCache.Variants;
        }

        /// <summary>
        ///     Return message metadata.
        ///     Used only for debugging purposes and advanced use cases.
        ///     Not supported on Native.
        /// </summary>
        public override Dictionary<string, object> MessageMetadata()
        {
            return new Dictionary<string, object>();
        }

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        /// </summary>
        public override void ForceContentUpdate()
        {
            ForceContentUpdate (null);
        }

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        ///     The provided callback will always fire regardless
        ///     of whether the variables have changed.
        /// </summary>
        ///
        public override void ForceContentUpdate(Action callback)
        {
            VarCache.CheckVarsUpdate (callback);
        }

        #endregion

        #region Internal API Calls
        /// <summary>
        ///     Call this when your application pauses.
        /// </summary>
        internal static void Pause()
        {
            if (Constants.isNoop)
            {
                return;
            }
            if (!calledStart)
            {
                CompatibilityLayer.LogError("You cannot call Pause before calling start.");
                return;
            }
            if (!isPaused)
            {
                isPaused = true;
                LeanplumRequest request =
                    LeanplumRequest.Post(Constants.Methods.PAUSE_SESSION, null);
                request.Response += (object obj) =>
                {
                    CompatibilityLayer.FlushSavedSettings();
                };
                request.Error += (Exception obj) =>
                {
                    CompatibilityLayer.FlushSavedSettings();
                };
                request.SendIfConnected();
            }
        }

        /// <summary>
        ///     Call this when your application resumes.
        /// </summary>
        internal static void Resume()
        {
            if (Constants.isNoop)
            {
                return;
            }
            if (!calledStart)
            {
                CompatibilityLayer.LogError("You cannot call Resume before calling start.");
                return;
            }
            if (isPaused)
            {
                isPaused = false;
                LeanplumRequest.Post(Constants.Methods.RESUME_SESSION, null).SendIfConnected();
            }
        }

        /// <summary>
        ///     Call this when your application stops.
        /// </summary>
        internal static void Stop()
        {
            if (Constants.isNoop)
            {
                return;
            }
            if (!calledStart)
            {
                CompatibilityLayer.LogError("You cannot call Stop before calling start.");
                return;
            }
#if !UNITY_WEBGL
            if (leanplumSocket != null)
            {
                leanplumSocket.Close();
            }
#endif
            LeanplumRequest.Post(Constants.Methods.STOP, null).SendIfConnected();
        }

        /// <summary>
        ///     Allows start to be called again.
        /// </summary>
        internal static void Reset()
        {
            if (calledStart)
            {
                Stop();
            }

            calledStart = false;
            _hasStarted = false;
            _HasStartedAndRegisteredAsDeveloper = false;
            startSuccessful = false;

            variablesChanged = null;
            variablesChangedAndNoDownloadsPending = null;
            started = null;
            LeanplumRequest.ClearNoPendingDownloads();
        }
        #endregion

        #region Dealing with Variables

        /// <summary>
        ///     Defines a new variable with a default value. If a Leanplum variable with the
        ///     same name and type exists, this will return the existing variable.
        /// </summary>
        /// <param name="name"> Name of the variable. </param>
        /// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
        public override Var<U> Define<U>(string name, U defaultValue)
        {
            string kind = null;
            if (defaultValue is int || defaultValue is long || defaultValue is short ||
                defaultValue is char || defaultValue is sbyte || defaultValue is byte)
            {
                kind = Constants.Kinds.INT;
            }
            else if (defaultValue is float || defaultValue is double || defaultValue is decimal)
            {
                kind = Constants.Kinds.FLOAT;
            }
            else if (defaultValue is string)
            {
                kind = Constants.Kinds.STRING;
            }
            else if (defaultValue is IList || defaultValue is Array)
            {
                kind = Constants.Kinds.ARRAY;
            }
            else if (defaultValue is IDictionary)
            {
                kind = Constants.Kinds.DICTIONARY;
            }
            else if (defaultValue is bool)
            {
                kind = Constants.Kinds.BOOLEAN;
            }
            else
            {
                Util.MaybeThrow(new LeanplumException(
                    "Default value for \"" + name + "\" not recognized or supported."));
                return null;
            }

            var variable = DefineHelper<U>(name, kind, defaultValue);

            if (variable != null)
            {
                variable.defaultClonedContainer = DeepCopyContainer(defaultValue);
                variable._defaultValue = defaultValue;

                VarCache.RegisterVariable(variable);
                variable.Update();
            }


            return variable;
        }

        private static NativeVar<U> DefineHelper<U>(string name, string kind, U defaultValue)
        {
            NativeVar<U> existing = (NativeVar<U>) VarCache.GetVariable<U>(name);
            if (existing != null)
            {
                existing.ClearValueChangedCallbacks();
                return existing;
            }
            // GetVariable(name) above will return null if the variable exists but of the wrong
            // type. Need to check if the name is not taken.
            if (VarCache.HasVariable(name))
            {
                LeanplumNative.CompatibilityLayer.LogWarning("Failed to define variable: \"" +
                    name + "\" refers to an " + "existing Leanplum variable of a different type.");
                return null;
            }

            var variable = new NativeVar<U>(name, kind, defaultValue);

            return variable;
        }

        // Recursive function to return a deep copy of the object and any nested subcontainers.
        private static object DeepCopyContainer(object container)
        {
            object copied = null;
            if (container is IDictionary)
            {
                copied = new Dictionary<object, object>();
                foreach (object key in ((IDictionary) container).Keys)
                {
                    ((IDictionary) copied)
                        .Add(key, DeepCopyContainer(((IDictionary) container)[key]));
                }
            }
            else if (container is IList)
            {
                copied = new List<object>();
                foreach (object value in (IList) container)
                {
                    ((IList) copied).Add(DeepCopyContainer(value));
                }
            }
            else
            {
                copied = container;
            }
            return copied;
        }


        /// <summary>
        ///     Defines an asset bundle. If a Leanplum variable with the same name and type exists,
        ///     this will return the existing variable.
        /// </summary>
        /// <returns>Leanplum variable.</returns>
        /// <param name="name">Name of variable.</param>
        /// <param name="realtimeUpdating">Setting it to <c>false</c> will prevent Leanplum from
        /// reloading assetbundles as they change in development mode.</param>
        /// <param name="iosBundleName">Filename of iOS assetbundle.</param>
        /// <param name="androidBundleName">Filename of Android assetbundle.</param>
        /// <param name="standaloneBundleName">Filename of Standalone assetbundle.</param>
        public override Var<AssetBundle> DefineAssetBundle(string name,
          bool realtimeUpdating = true, string iosBundleName = "", string androidBundleName = "",
          string standaloneBundleName = "")
        {
            string platform = LeanplumNative.CompatibilityLayer.GetPlatformName();
            string resourceName = Constants.Values.RESOURCES_VARIABLE + '.' + platform +
                " Assets." + name;
            string bundleName = String.Empty;
            if (platform == "iOS")
            {
                bundleName = iosBundleName;
            }
            else if (platform == "Android")
            {
                bundleName = androidBundleName;
            }
            else if (platform == "Standalone")
            {
                bundleName = standaloneBundleName;
            }

            var variable = DefineHelper<AssetBundle>(resourceName, Constants.Kinds.FILE, null);
            if (variable != null)
            {
                variable.SetFilename(bundleName);
                variable.fileReady = false;
                variable.realtimeAssetUpdating = realtimeUpdating;

                VarCache.RegisterVariable(variable);
                variable.Update();
            }
            return variable;
        }

        #endregion
    }
}
