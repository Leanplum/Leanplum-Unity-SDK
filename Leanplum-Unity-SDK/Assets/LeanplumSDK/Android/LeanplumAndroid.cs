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
#if UNITY_ANDROID

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LeanplumSDK.MiniJSON;
using UnityEngine;
using static LeanplumSDK.Constants;
using static LeanplumSDK.Leanplum;

namespace LeanplumSDK
{
    public class LeanplumAndroid : LeanplumSDKObject
    {
        private static AndroidJavaClass nativeSdk = null;

        internal static AndroidJavaClass NativeSDK
        {
            get
            {
                return nativeSdk;
            }
            set
            {
                nativeSdk = value;
            }
        }

        private LeanplumInbox inbox;
        public override LeanplumInbox Inbox
        {
            get
            {
                if (inbox == null)
                {
                    inbox = new LeanplumInboxAndroid(NativeSDK);
                    return inbox;
                }
                return inbox;
            }
        }

        public override event Leanplum.VariableChangedHandler VariablesChanged;
        public override event Leanplum.VariablesChangedAndNoDownloadsPendingHandler VariablesChangedAndNoDownloadsPending;

        private event Leanplum.StartHandler started;
        private bool startSuccessful;

        public override event Leanplum.StartHandler Started
        {
            add
            {
                started += value;
                // If it has not started, event will be invoked
                // through the start response handler when Leanplum starts
                if (HasStarted())
                {
                    value(startSuccessful);
                }
            }
            remove
            {
                started -= value;
            }
        }

        private event CleverTapInstanceHandler cleverTapInstanceReady;
        private string accountId;
        public override event CleverTapInstanceHandler CleverTapInstanceReady
        {
            add
            {
                cleverTapInstanceReady += value;
                if (!string.IsNullOrEmpty(accountId))
                {
                    value?.Invoke();
                }
            }
            remove
            {
                cleverTapInstanceReady -= value;
            }
        }

        private Dictionary<int, Leanplum.ForceContentUpdateHandler> ForceContentUpdateCallbacksDictionary = new Dictionary<int, Leanplum.ForceContentUpdateHandler>();
        private Dictionary<int, Leanplum.VariablesChangedAndNoDownloadsPendingHandler> OnceVariablesChangedAndNoDownloadsPendingDict =
            new Dictionary<int, Leanplum.VariablesChangedAndNoDownloadsPendingHandler>();
        private Dictionary<string, ActionContext.ActionResponder> ActionRespondersDictionary = new Dictionary<string, ActionContext.ActionResponder>();
        private Dictionary<string, ActionContext.ActionResponder> DismissActionRespondersDictionary = new Dictionary<string, ActionContext.ActionResponder>();

        static private int DictionaryKey = 0;
        private string gameObjectName;
        private JavaBridge javaBridge;

        public LeanplumAndroid()
        {
            AndroidJNI.AttachCurrentThread();
            nativeSdk = new AndroidJavaClass("com.leanplum.UnityBridge");

            // This also constructs LeanplumUnityHelper and the game object.
            gameObjectName = LeanplumUnityHelper.Instance.gameObject.name;

            // Bridge is used to call C# from Java in a synchronous manner.
            javaBridge = new JavaBridge(this);

            NativeSDK.CallStatic("initialize", gameObjectName, Constants.SDK_VERSION, null, javaBridge);
        }

        #region Accessors and Mutators
        /// <summary>
        ///     Gets a value indicating whether Leanplum has finished starting.
        /// </summary>
        /// <value><c>true</c> if this instance has started; otherwise, <c>false</c>.</value>
        public override bool HasStarted()
        {
            return NativeSDK.CallStatic<bool>("hasStarted");
        }

        /// <summary>
        ///     Gets a value indicating whether Leanplum has started and the device is registered as a
        ///     developer.
        /// </summary>
        /// <value><c>true</c> if Leanplum has started and the device registered as developer;
        ///     otherwise, <c>false</c>.</value>
        public override bool HasStartedAndRegisteredAsDeveloper()
        {
            return NativeSDK.CallStatic<bool>("hasStartedAndRegisteredAsDeveloper");
        }

        /// <summary>
        ///     Gets whether or not developer mode is enabled.
        /// </summary>
        /// <value><c>true</c> if developer mode; otherwise, <c>false</c>.</value>
        public override bool IsDeveloperModeEnabled()
        {
            return NativeSDK.CallStatic<bool>("isDeveloperModeEnabled");
        }

        /// <summary>
        ///     Gets the includeDefaults param value.
        /// </summary>
        public override bool GetIncludeDefaults()
        {
            return false;
        }

        /// <summary>
        ///     Optional. Sets the API server.
        ///     The API path is of the form http[s]://hostname/servletName
        /// </summary>
        /// <param name="hostName"> The name of the API host, such as www.leanplum.com </param>
        /// <param name="servletName"> The name of the API servlet, such as api </param>
        /// <param name="useSSL"> Whether to use SSL </param>
        public override void SetApiConnectionSettings(string hostName, string servletName = "api",
            bool useSSL = true)
        {
            NativeSDK.CallStatic("setApiConnectionSettings", hostName, servletName, useSSL);
        }

        /// <summary>
        ///     Optional. Sets the socket server path for Development mode.
        ///     Path is of the form hostName:port
        /// </summary>
        /// <param name="hostName"> The host name of the socket server. </param>
        /// <param name="port"> The port to connect to. </param>
        public override void SetSocketConnectionSettings(string hostName, int port)
        {
            NativeSDK.CallStatic("setSocketConnectionSettings", hostName, port);
        }

        /// <summary>
        ///     The default timeout is 10 seconds for requests, and 15 seconds for file downloads.
        /// </summary>
        /// <param name="seconds"> Timeout in seconds for standard webrequests. </param>
        /// <param name="downloadSeconds"> Timeout in seconds for downloads. </param>
        public override void SetNetworkTimeout(int seconds, int downloadSeconds)
        {
            NativeSDK.CallStatic("setNetworkTimeout", seconds, downloadSeconds);
        }

        /// <summary>
        ///     Sets the time interval between uploading events to server.
        ///     Default is <see cref="EventsUploadInterval.AtMost15Minutes"/>.
        /// </summary>
        /// <param name="uploadInterval"> The time between uploads. </param>
        public override void SetEventsUploadInterval(EventsUploadInterval uploadInterval)
        {
            NativeSDK.CallStatic("setEventsUploadInterval", (int)uploadInterval);
        }

        /// <summary>
        ///     Must call either this or SetAppIdForProductionMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your development key. </param>
        public override void SetAppIdForDevelopmentMode(string appId, string accessKey)
        {
            NativeSDK.CallStatic("setAppIdForDevelopmentMode", appId, accessKey);
        }

        /// <summary>
        ///     Must call either this or SetAppIdForDevelopmentMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your production key. </param>
        public override void SetAppIdForProductionMode(string appId, string accessKey)
        {
            NativeSDK.CallStatic("setAppIdForProductionMode", appId, accessKey);
        }

        /// <summary>
        ///    Unsupported for Android.
        /// </summary>
        /// <param name="version">Version.</param>
        public override void SetAppVersion(string version)
        {
            // The Android SDK does not support this.
        }

        /// <summary>
        ///   Sets a custom device ID. Device IDs should be unique across physical devices.
        /// </summary>
        /// <param name="deviceId">Device identifier.</param>
        public override void SetDeviceId(string deviceId)
        {
            NativeSDK.CallStatic("setDeviceId", deviceId);
        }

        /// <summary>
        ///     Get device id.
        /// </summary>
        public override string GetDeviceId()
        {
            return NativeSDK.CallStatic<string>("getDeviceId");
        }

        /// <summary>
        ///     Get user id.
        /// </summary>
        public override string GetUserId()
        {
            return NativeSDK.CallStatic<string>("getUserId");
        }

        /// <summary>
        ///     This should be your first statement in a unit test. Setting this to true
        ///     will prevent Leanplum from communicating with the server.
        /// </summary>
        public override void SetTestMode(bool testModeEnabled)
        {
            NativeSDK.CallStatic("setTestModeEnabled", testModeEnabled);
        }

        /// <summary>
        ///     Sets whether the API should return default ("defaults in code") values
        ///     or only the overridden ones.
        ///     Used only in Development mode. Always false in production.
        /// </summary>
        /// <param name="includeDefaults"> The value for includeDefaults param. </param>
        public override void SetIncludeDefaultsInDevelopmentMode(bool includeDefaults)
        {
            // The Android SDK does not support this.
        }

        /// <summary>
        ///     Sets whether realtime updates to the client are enabled in development mode.
        ///     This uses websockets which can have high CPU impact. Default: true.
        /// </summary>
        public override void SetRealtimeUpdatesInDevelopmentModeEnabled(bool enabled)
        {
            NativeSDK.CallStatic("setFileUploadingEnabledInDevelopmentMode", enabled);
        }

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public override object ObjectForKeyPath(params object[] components)
        {
            string jsonString = NativeSDK.CallStatic<string>("objectForKeyPath",
                Json.Serialize(components));
            return Json.Deserialize(jsonString);
        }

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public override object ObjectForKeyPathComponents(object[] pathComponents)
        {
            string jsonString = NativeSDK.CallStatic<string>("objectForKeyPathComponents",
                Json.Serialize(pathComponents));
            return Json.Deserialize(jsonString);
        }

        /// <summary>
        ///     Set location manually. Calls SetDeviceLocationWithLatitude with cell type. Best if 
        ///     used in after calling DisableLocationCollection. Not supported on Native.
        /// </summary>
        /// <param name="latitude"> Device location latitude. </param>
        /// <param name="longitude"> Device location longitude. </param>
        public override void SetDeviceLocation(double latitude, double longitude)
        {
            NativeSDK.CallStatic("setDeviceLocation", latitude, longitude);
        }

        /// <summary>
        ///     Set location manually. Calls SetDeviceLocationWithLatitude with cell type. Best if 
        ///     used in after calling DisableLocationCollection. Not supported on Native.
        /// </summary>
        /// <param name="latitude"> Device location latitude. </param>
        /// <param name="longitude"> Device location longitude. </param>
        /// <param name="type"> Location accuracy type. </param>
        public override void SetDeviceLocation(double latitude, double longitude, LPLocationAccuracyType type)
        {
            NativeSDK.CallStatic("setDeviceLocation", latitude, longitude, (int)type);
        }

        /// <summary>
        ///     Set location manually. Calls SetDeviceLocationWithLatitude with cell type. Best if 
        ///     used in after calling DisableLocationCollection. Not supported on Native.
        /// </summary>
        /// <param name="latitude"> Device location latitude. </param>
        /// <param name="longitude"> Device location longitude. </param>
        /// <param name="city"> Location city. </param>
        /// <param name="region"> Location region. </param>
        /// <param name="country"> Country ISO code. </param>
        /// <param name="type"> Location accuracy type. </param>
        public override void SetDeviceLocation(double latitude, double longitude, string city, string region, string country, LPLocationAccuracyType type)
        {
            NativeSDK.CallStatic("setDeviceLocation", latitude, longitude, (int)type);
        }

        /// <summary>
        ///    Disables collecting location automatically. Will do nothing if Leanplum-Location is 
        ///    not used. Not supported on Native.
        /// </summary>
        public override void DisableLocationCollection()
        {
            NativeSDK.CallStatic("disableLocationCollection");
        }

        public override void SetPushDeliveryTrackingEnabled(bool enabled)
        {
            NativeSDK.CallStatic("setPushDeliveryTracking", enabled);
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
            // Invokes Started event through NativeCallback
            Started += startResponseAction;

            NativeSDK.CallStatic("start", userId, Json.Serialize(attributes));
        }

        public override void SetMiPushApplication(string miAppId, string miAppKey)
        {
            NativeSDK.CallStatic("setMiPushApplication", miAppId, miAppKey);
        }

        public override void SetLogLevel(Constants.LogLevel logLevel)
        {
            NativeSDK.CallStatic("setLogLevel", (int)logLevel);
        }

        public override void ForceSyncVariables(Leanplum.SyncVariablesCompleted completedHandler)
        {
            // The Android SDK does not support this.
        }

        public override void DefineAction(string name, Constants.ActionKind kind, ActionArgs args, IDictionary<string, object> options, ActionContext.ActionResponder responder)
        {
            DefineAction(name, kind, args, options, responder, null);
        }

        public override void DefineAction(
            string name,
            Constants.ActionKind kind,
            ActionArgs args,
            IDictionary<string, object> options,
            ActionContext.ActionResponder responder,
            ActionContext.ActionResponder dismissResponder)
        {
            if (name == null)
            {
                return;
            }
            if (responder != null)
            {
                ActionRespondersDictionary.Add(name, responder);
            }
            if (dismissResponder != null)
            {
                DismissActionRespondersDictionary.Add(name, dismissResponder);
            }

            string argString = args == null ? null : args.ToJSON();
            string optionString = options == null ? null : Json.Serialize(options);
            int kindInt = (int)kind;

            NativeSDK.CallStatic("defineAction", name, kindInt, argString, optionString);
        }

        // IJavaBridge methods are called by Java.
        private class JavaBridge : AndroidJavaProxy
        {
            private readonly LeanplumAndroid outer;

            public JavaBridge(LeanplumAndroid outer) : base("com.leanplum.IJavaBridge")
            {
                this.outer = outer;
            }

            int shouldDisplayMessage(String key)
            {
                if (outer.shouldDisplayMessageHandler == null)
                {
                    return (int)MessageDisplayChoice.DisplayChoice.SHOW;
                }
                var context = CreateActionContextFromKey(key);
                var result = outer.shouldDisplayMessageHandler(context);
                if (result.Choice.Equals(MessageDisplayChoice.DisplayChoice.DELAY))
                {
                    if (result.DelaySeconds <= -1)
                    {
                        return -1;
                    }
                    else
                    {
                        return (int)MessageDisplayChoice.DisplayChoice.DELAY + result.DelaySeconds;
                    }
                }
                return (int)result.Choice;
            }

            String prioritizeMessages(String contextsKeys, String actionsTrigger)
            {
                string[] keys = contextsKeys.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (outer.prioritizeMessagesHandler == null)
                {
                    // By default, add only one message to queue if `prioritizeMessages` is not implemented
                    // This ensures backwards compatibility
                    return keys[0];
                }

                List<ActionContext> contexts = new List<ActionContext>();
                foreach (string key in keys)
                {
                    var context = CreateActionContextFromKey(key);
                    contexts.Add(context);
                }

                var eligibleContexts = outer.prioritizeMessagesHandler(
                    contexts.ToArray(),
                    Json.Deserialize(actionsTrigger) as Dictionary<string, object>);
                var resultCsv = string.Join(",", eligibleContexts.Select(x => x.Key));
                return resultCsv;
            }

            void onMessageDisplayed(String key)
            {
                if (outer.messageDisplayedHandler != null)
                {
                    var context = CreateActionContextFromKey(key);
                    outer.messageDisplayedHandler(context);
                }
            }

            void onMessageDismissed(String key)
            {
                if (outer.messageDismissedHandler != null)
                {
                    var context = CreateActionContextFromKey(key);
                    outer.messageDismissedHandler(context);
                }
            }

            void onMessageAction(String name, String key)
            {
                if (outer.messageActionHandler != null)
                {
                    var context = CreateActionContextFromKey(key);
                    outer.messageActionHandler(name, context);
                }
            }
        }

        public override void ShouldDisplayMessage(Leanplum.ShouldDisplayMessageHandler handler)
        {
            shouldDisplayMessageHandler = handler;
        }

        public override void PrioritizeMessages(Leanplum.PrioritizeMessagesHandler handler)
        {
            prioritizeMessagesHandler = handler;
        }

        public override void OnMessageDisplayed(MessageHandler handler)
        {
            messageDisplayedHandler = handler;
        }

        public override void OnMessageDismissed(MessageHandler handler)
        {
            messageDismissedHandler = handler;
        }

        public override void OnMessageAction(MessageActionHandler handler)
        {
            messageActionHandler = handler;
        }

        public override void TriggerDelayedMessages()
        {
            NativeSDK.CallStatic("triggerDelayedMessages");
        }

        public override void SetActionManagerPaused(bool paused)
        {
            NativeSDK.CallStatic("setActionManagerPaused", paused);
        }

        public override void SetActionManagerEnabled(bool enabled)
        {
            NativeSDK.CallStatic("setActionManagerEnabled", enabled);
        }

        public override bool ShowMessage(string id)
        {
            // The Android SDK does not support this.
            return false;
        }

        /// <summary>
        ///     Logs a purchase event in your application. The string can be any
        ///     value of your choosing, however in most cases you will want to use
        ///     Leanplum.PURCHASE_EVENT_NAME
        /// </summary>
        public override void TrackPurchase(string eventName, double value, string currencyCode,
        IDictionary<string, object> parameters)
        {
            NativeSDK.CallStatic("trackPurchase", eventName, value, currencyCode, Json.Serialize(parameters));
        }

        /// <summary>
        ///     Logs in-app purchase data from Google Play.
        /// </summary>
        /// <param name="item">The name of the item purchased.</param>
        /// <param name="priceMicros">The purchase price in micros.</param>
        /// <param name="currencyCode">The ISO 4217 currency code.</param>
        /// <param name="purchaseData">Purchase data from
        /// com.android.vending.billing.util.Purchase.getOriginalJson().</param>
        /// <param name="dataSignature">Purchase data signature from
        /// com.android.vending.billing.util.Purchase.getSignature().</param>
        /// <param name="parameters">Optional event parameters.</param>
        public override void TrackGooglePlayPurchase(string item, long priceMicros,
            string currencyCode, string purchaseData, string dataSignature,
            IDictionary<string, object> parameters)
        {
            NativeSDK.CallStatic("trackGooglePlayPurchase", item, priceMicros, currencyCode,
                purchaseData, dataSignature, Json.Serialize(parameters));
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public override void Track(string eventName, double value, string info,
            IDictionary<string, object> parameters)
        {
            NativeSDK.CallStatic("track", eventName, value, info, Json.Serialize(parameters));
        }

        /// <summary>
        ///     Sets the traffic source info for the current user.
        ///     Keys in info must be one of: publisherId, publisherName, publisherSubPublisher,
        ///     publisherSubSite, publisherSubCampaign, publisherSubAdGroup, publisherSubAd.
        /// </summary>
        public override void SetTrafficSourceInfo(IDictionary<string, string> info)
        {
            NativeSDK.CallStatic("setTrafficSourceInfo", Json.Serialize(info));
        }

        /// <summary>
        ///     Advances to a particular state in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     A state is a section of your app that the user is currently in.
        /// </summary>
        public override void AdvanceTo(string state, string info,
            IDictionary<string, object> parameters)
        {
            NativeSDK.CallStatic("advanceTo", state, info, Json.Serialize(parameters));
        }

        /// <summary>
        ///     Updates the user ID and adds or modifies user attributes.
        /// </summary>
        /// <param name="newUserId">New user identifier.</param>
        /// <param name="value">User attributes.</param>
        public override void SetUserAttributes(string newUserId, IDictionary<string, object> value)
        {
            NativeSDK.CallStatic("setUserAttributes", newUserId, Json.Serialize(value));
        }

        /// <summary>
        ///     Pauses the current state.
        ///     You can use this if your game has a "pause" mode. You shouldn't call it
        ///     when someone switches out of your app because that's done automatically.
        /// </summary>
        public override void PauseState()
        {
            NativeSDK.CallStatic("pauseState");
        }

        /// <summary>
        ///     Resumes the current state.
        /// </summary>
        public override void ResumeState()
        {
            NativeSDK.CallStatic("resumeState");
        }

        /// <summary>
        ///     Returns variant ids.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        public override List<object> Variants()
        {
            string jsonString = NativeSDK.CallStatic<string>("variants");
            return (List<object>)Json.Deserialize(jsonString);
        }

        /// <summary>
        /// Returns the last received signed variables.
        /// If signature was not provided from server the
        /// result of this method will be null.
        /// </summary>
        /// <returns> Returns <see cref="LeanplumSecuredVars"/> instance containing
        /// variable's JSON and signature.
        /// If signature was not downloaded from server, returns null.
        /// </returns>
        public override LeanplumSecuredVars SecuredVars()
        {
            string jsonString = NativeSDK.CallStatic<string>("securedVars");
            if (!string.IsNullOrEmpty(jsonString))
            {
                var varsDict = (Dictionary<string, object>)Json.Deserialize(jsonString);
                return LeanplumSecuredVars.FromDictionary(varsDict);
            }
            return null;
        }

        public override IDictionary<string, object> Vars()
        {
            string jsonString = NativeSDK.CallStatic<string>("vars");
            return (Dictionary<string, object>)Json.Deserialize(jsonString);
        }

        /// <summary>
        ///     Returns metadata for all active in-app messages.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        public override Dictionary<string, object> MessageMetadata()
        {
            string jsonString = NativeSDK.CallStatic<string>("messageMetadata");
            return (Dictionary<string, object>)Json.Deserialize(jsonString);
        }

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        /// </summary>
        public override void ForceContentUpdate()
        {
            NativeSDK.CallStatic("forceContentUpdate");
        }

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        ///   The provided callback will always fire regardless
        ///   of whether the variables have changed.
        /// </summary>
        ///
        public override void ForceContentUpdate(Action callback)
        {

            Leanplum.ForceContentUpdateHandler handler = (success) =>
            {
                callback();
            };

            ForceContentUpdate(handler);
        }

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        ///     The provided handler will always fire regardless
        ///     of whether the variables have changed.
        ///     It provides a boolean value whether the update to the server was successful.
        /// </summary>
        /// <param name="handler">
        ///     The handler to execute once the update completed
        ///     with the corresponding success result.
        /// </param>
        public override void ForceContentUpdate(Leanplum.ForceContentUpdateHandler handler)
        {
            int key = DictionaryKey++;
            ForceContentUpdateCallbacksDictionary.Add(key, handler);
            NativeSDK.CallStatic("forceContentUpdateWithCallback", key);
        }

        #endregion

        public override void NativeCallback(string message)
        {
            const string VARIABLES_CHANGED = "VariablesChanged:";
            const string VARIABLES_CHANGED_NO_DOWNLOAD_PENDING = "VariablesChangedAndNoDownloadsPending:";
            const string ONCE_VARIABLES_CHANGED_NO_DOWNLOADS_PENDING = "OnceVariablesChangedAndNoDownloadsPendingHandler:";
            const string STARTED = "Started:";
            const string CLEVERTAP_INSTANCE = "CleverTapInstance:";
            const string VARIABLE_VALUE_CHANGED = "VariableValueChanged:";
            const string FORCE_CONTENT_UPDATE_WITH_CALLBACK = "ForceContentUpdateWithCallback:";
            const string DEFINE_ACTION_RESPONDER = "ActionResponder:";
            const string ACTION_DISMISS = "ActionDismiss:";
            const string RUN_ACTION_NAMED_RESPONDER = "OnRunActionNamed:";

            if (message.StartsWith(VARIABLES_CHANGED))
            {
                VariablesChanged?.Invoke();
            }
            else if (message.StartsWith(VARIABLES_CHANGED_NO_DOWNLOAD_PENDING))
            {
                VariablesChangedAndNoDownloadsPending?.Invoke();
            }
            else if (message.StartsWith(ONCE_VARIABLES_CHANGED_NO_DOWNLOADS_PENDING))
            {
                string[] values = message.Substring(ONCE_VARIABLES_CHANGED_NO_DOWNLOADS_PENDING.Length).Split(':');
                int key = Convert.ToInt32(values[0]);
                if (OnceVariablesChangedAndNoDownloadsPendingDict.TryGetValue(key, out Leanplum.VariablesChangedAndNoDownloadsPendingHandler callback))
                {
                    callback();
                    OnceVariablesChangedAndNoDownloadsPendingDict.Remove(key);
                }
            }
            else if (message.StartsWith(STARTED))
            {
                if (started != null)
                {
                    startSuccessful = message.EndsWith("true") || message.EndsWith("True");
                    started(startSuccessful);
                }
            }
            else if (message.StartsWith(CLEVERTAP_INSTANCE))
            {
                string id = message[CLEVERTAP_INSTANCE.Length..];
                if (accountId != id)
                {
                    accountId = id;
                    // TODO: CleverTap set instance with account id
                    cleverTapInstanceReady?.Invoke();
                }
            }
            else if (message.StartsWith(VARIABLE_VALUE_CHANGED))
            {
                // Drop the beginning of the message to get the name of the variable
                // Then dispatch to the correct variable
                LeanplumAndroid.VariableValueChanged(message.Substring(21));
            }
            else if (message.StartsWith(FORCE_CONTENT_UPDATE_WITH_CALLBACK))
            {
                string[] values = message.Substring(FORCE_CONTENT_UPDATE_WITH_CALLBACK.Length).Split(':');
                int key = Convert.ToInt32(values[0]);
                bool success = values[1] == "1";
                if (ForceContentUpdateCallbacksDictionary.TryGetValue(key, out Leanplum.ForceContentUpdateHandler callback))
                {
                    callback(success);
                    ForceContentUpdateCallbacksDictionary.Remove(key);
                }
            }
            else if (message.StartsWith(DEFINE_ACTION_RESPONDER))
            {
                string key = message.Substring(DEFINE_ACTION_RESPONDER.Length);
                string actionName = GetActionNameFromMessageKey(key);

                ActionContext.ActionResponder callback;
                if (ActionRespondersDictionary.TryGetValue(actionName, out callback))
                {
                    var context = CreateActionContextFromKey(key);
                    callback(context);
                }
            }
            else if (message.StartsWith(ACTION_DISMISS))
            {
                string key = message.Substring(ACTION_DISMISS.Length);
                string actionName = GetActionNameFromMessageKey(key);

                if (DismissActionRespondersDictionary.TryGetValue(actionName, out ActionContext.ActionResponder callback))
                {
                    var context = CreateActionContextFromKey(key);
                    callback(context);
                }
            }

            if (Inbox != null)
            {
                Inbox.NativeCallback(message);
            }
        }

        #region Action Manager

        private MessageHandler messageDisplayedHandler;
        private MessageHandler messageDismissedHandler;
        private MessageActionHandler messageActionHandler;

        private ShouldDisplayMessageHandler shouldDisplayMessageHandler;
        private PrioritizeMessagesHandler prioritizeMessagesHandler;

        #endregion

        #region Dealing with Variables

        protected static IDictionary<string, Var> AndroidVarCache = new Dictionary<string, Var>();

        /// <summary>
        ///     Defines a new variable with a default value. If a Leanplum variable with the same name
        ///     and type exists, this will return the existing variable.
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
                Debug.LogError("Leanplum Error: Default value for \"" + name +
                    "\" not recognized or supported.");
                return null;
            }

            // Check if existing
            if (AndroidVarCache.ContainsKey(name))
            {
                if (AndroidVarCache[name].Kind != kind)
                {
                    Debug.LogError("Leanplum Error: \"" + name +
                        "\" was already defined with a different kind");
                    return null;
                }
                return (Var<U>)AndroidVarCache[name];
            }

            // Define in native SDK
            LeanplumAndroid.NativeSDK.CallStatic("defineVar", name, kind, Json.Serialize(defaultValue));
            Var<U> result = new AndroidVar<U>(name, kind, defaultValue);
            // Register in Cache
            AndroidVarCache[name] = result;
            return result;
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
        public override Var<AssetBundle> DefineAssetBundle(string name, bool realtimeUpdating = true,
                                                           string iosBundleName = "",
                                                           string androidBundleName = "",
                                                           string standaloneBundleName = "")
        {
            string kind = Constants.Kinds.FILE;
            string actualName = "__Unity Resources.Android.Assets." + name;

            string fullPath = /*Application.dataPath + "/"*/ "assets/" + androidBundleName;
            // Check if existing
            if (AndroidVarCache.ContainsKey(actualName))
            {
                if (AndroidVarCache[actualName].Kind != kind)
                {
                    Debug.LogError("Leanplum Error: \"" + name +
                        "\" was already defined with a different kind");
                    return null;
                }
                return (Var<AssetBundle>)AndroidVarCache[actualName];
            }
            Var<AssetBundle> result = new AndroidVar<AssetBundle>(actualName, kind, null, fullPath);
            AndroidVarCache[actualName] = result;
            LeanplumAndroid.NativeSDK.CallStatic("defineVar", actualName, "file", fullPath);
            return result;
        }

        public static void VariableValueChanged(string name)
        {
            Var variable = AndroidVarCache[name];
            if (variable != null)
            {
                variable.OnValueChanged();
            }
        }

        public override ActionContext CreateActionContextForId(string actionId)
        {
            if (!string.IsNullOrEmpty(actionId))
            {
                string key = NativeSDK.CallStatic<string>("createActionContextForId", actionId);
                return CreateActionContextFromKey(key);
            }
            return null;
        }

        private static Tuple<string, string> GetActionNameMessageIdFromMessageKey(string key)
        {
            string actionName = GetActionNameFromMessageKey(key);
            string messageId = GetActionIdFromMessageKey(key);
            return new Tuple<string, string>(actionName, messageId);
        }

        private static ActionContextAndroid CreateActionContextFromKey(string key)
        {
            Tuple<string, string> actionNameMessageId = GetActionNameMessageIdFromMessageKey(key);
            return new ActionContextAndroid(key, actionNameMessageId.Item1, actionNameMessageId.Item2);
        }

        public override bool TriggerActionForId(string actionId)
        {
            return NativeSDK.CallStatic<bool>("triggerAction", actionId);
        }

        private static string GetActionNameFromMessageKey(string key)
        {
            // {parentActionName:parentMessageId:...:actionName:messageId}
            var keys = key.Split(':');
            return keys[keys.Length - 2];
        }

        private static string GetActionIdFromMessageKey(string key)
        {
            // {parentActionName:parentMessageId:...:actionName:messageId}
            var keys = key.Split(':');
            return keys[keys.Length - 1];
        }

        public override void AddOnceVariablesChangedAndNoDownloadsPendingHandler(
            Leanplum.VariablesChangedAndNoDownloadsPendingHandler handler)
        {
            int key = DictionaryKey++;
            OnceVariablesChangedAndNoDownloadsPendingDict.Add(key, handler);
            NativeSDK.CallStatic("addOnceVariablesChangedAndNoDownloadsPendingHandler", key);
        }

        #endregion

        public override MigrationConfig MigrationConfig()
        {
            string jsonString = NativeSDK.CallStatic<string>("getMigrationConfig");
            if (!string.IsNullOrEmpty(jsonString))
            {
                var varsDict = (Dictionary<string, string>)Json.Deserialize(jsonString);
                return new MigrationConfig(varsDict);
            }
            return null;
        }
    }
}

#endif
