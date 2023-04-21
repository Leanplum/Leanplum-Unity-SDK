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
#if UNITY_IPHONE

using System;
using UnityEngine;
using LeanplumSDK.MiniJSON;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LeanplumSDK.Apple;
using AOT;
using System.Linq;
using static LeanplumSDK.Leanplum;

namespace LeanplumSDK
{
    public class LeanplumApple : LeanplumSDKObject
    {
        private bool isDeveloper = false;

        [DllImport("__Internal")]
        internal static extern void lp_setGameObject(string gameObject);

        [DllImport("__Internal")]
        internal static extern void lp_registerForNotifications();

        [DllImport("__Internal")]
        internal static extern void lp_setPushDeliveryTrackingEnabled(bool enabled);

        [DllImport("__Internal")]
        internal static extern void lp_setAppIdDeveloper(string appId, string accessKey);

        [DllImport("__Internal")]
        internal static extern void lp_setAppIdProduction(string appId, string accessKey);

        [DllImport("__Internal")]
        internal static extern bool lp_hasStarted();

        [DllImport("__Internal")]
        internal static extern bool lp_hasStartedAndRegisteredAsDeveloper();

        [DllImport("__Internal")]
        internal static extern void lp_addOnceVariablesChangedAndNoDownloadsPendingHandler(int key);

        [DllImport("__Internal")]
        internal static extern void lp_start(string sdkVersion, string userId, string dictStringJSON);

        [DllImport("__Internal")]
        internal static extern void lp_trackIOSInAppPurchases();

        [DllImport("__Internal")]
        internal static extern void lp_trackPurchase(string lp_event, double value, string currencyCode,
          string dictStringJSON);

        [DllImport("__Internal")]
        internal static extern void lp_track(string lp_event, double value, string info,
          string dictStringJSON);

        [DllImport("__Internal")]
        internal static extern void lp_setApiHostName(string hostName, string servletName,
          int useSSL);

        [DllImport("__Internal")]
        internal static extern void lp_setNetworkTimeout(int seconds, int downloadSeconds);

        [DllImport("__Internal")]
        internal static extern void lp_setEventsUploadInterval(int uploadInterval);

        [DllImport("__Internal")]
        internal static extern void lp_setLogLevel(int logLevel);

        [DllImport("__Internal")]
        internal static extern void lp_setAppVersion(string version);

        [DllImport("__Internal")]
        internal static extern void lp_setDeviceId(string deviceId);

        [DllImport("__Internal")]
        internal static extern string lp_getDeviceId();

        [DllImport("__Internal")]
        internal static extern string lp_getUserId();

        [DllImport("__Internal")]
        internal static extern void lp_setTestModeEnabled(bool enabled);

        [DllImport("__Internal")]
        internal static extern void lp_setTrafficSourceInfo(string dictStringJSON);

        [DllImport("__Internal")]
        internal static extern void lp_advanceTo(string state, string info, string dictStringJSON);

        [DllImport("__Internal")]
        internal static extern void lp_setUserAttributes(string newUserId, string dictStringJSON);

        [DllImport("__Internal")]
        internal static extern void lp_pauseState();

        [DllImport("__Internal")]
        internal static extern void lp_resumeState();

        [DllImport("__Internal")]
        internal static extern string lp_variants();

        [DllImport("__Internal")]
        internal static extern string lp_securedVars();

        [DllImport("__Internal")]
        internal static extern string lp_vars();

        [DllImport("__Internal")]
        internal static extern string lp_messageMetadata();

        [DllImport("__Internal")]
        internal static extern void lp_forceContentUpdate();

        [DllImport("__Internal")]
        internal static extern void lp_defineAction(string name, int kind, string argsJSON, string optionsJSON);

        [DllImport("__Internal")]
        internal static extern string lp_createActionContextForId(string actionId);

        [DllImport("__Internal")]
        internal static extern bool lp_triggerAction(string actionId);

        [DllImport("__Internal")]
        internal static extern bool lp_triggerDelayedMessages();

        [DllImport("__Internal")]
        internal static extern bool lp_onMessageDisplayed();

        [DllImport("__Internal")]
        internal static extern bool lp_onMessageDismissed();

        [DllImport("__Internal")]
        internal static extern bool lp_onMessageAction();

        [DllImport("__Internal")]
        internal static extern bool lp_setActionManagerPaused(bool paused);

        [DllImport("__Internal")]
        internal static extern bool lp_setActionManagerEnabled(bool enabled);

        [DllImport("__Internal")]
        internal static extern bool lp_setActionManagerUseAsyncHandlers(bool enabled);

        [DllImport("__Internal")]
        internal static extern void lp_forceContentUpdateWithHandler(int key);

        [DllImport("__Internal")]
        internal static extern string lp_objectForKeyPath(string dictStringJSON);

        [DllImport("__Internal")]
        internal static extern string lp_objectForKeyPathComponents(string dictStringJSON);

        [DllImport("__Internal")]
        internal static extern void lp_setDeviceLocationWithLatitude(double latitude, double longitude);

        [DllImport("__Internal")]
        internal static extern void lp_setDeviceLocationWithLatitude(double latitude, double longitude, int type);

        [DllImport("__Internal")]
        internal static extern void lp_setDeviceLocationWithLatitude(double latitude, double longitude, string city, string region, string country, int type);

        [DllImport("__Internal")]
        internal static extern void lp_disableLocationCollection();

        private LeanplumInbox inbox;
        public override LeanplumInbox Inbox
        {
            get
            {
                if (inbox == null)
                {
                    inbox = new LeanplumInboxApple();
                    return inbox;
                }
                return inbox;
            }
        }

        public LeanplumApple()
        {
        }

        public override event VariableChangedHandler VariablesChanged;
        public override event VariablesChangedAndNoDownloadsPendingHandler VariablesChangedAndNoDownloadsPending;

        private event StartHandler started;
        private bool startSuccessful;

        public override event StartHandler Started
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

        private Dictionary<int, ForceContentUpdateHandler> ForceContentUpdateHandlersDictionary = new Dictionary<int, ForceContentUpdateHandler>();
        private Dictionary<string, ActionContext.ActionResponder> ActionRespondersDictionary = new Dictionary<string, ActionContext.ActionResponder>();
        private Dictionary<string, ActionContext.ActionResponder> DismissActionRespondersDictionary = new Dictionary<string, ActionContext.ActionResponder>();

        private Dictionary<int, Leanplum.VariablesChangedAndNoDownloadsPendingHandler> OnceVariablesChangedAndNoDownloadsPendingDict =
    new Dictionary<int, Leanplum.VariablesChangedAndNoDownloadsPendingHandler>();

        static private int DictionaryKey = 0;

        #region Accessors and Mutators

        public override void RegisterForIOSRemoteNotifications()
        {
            lp_registerForNotifications();
        }

        public override void SetPushDeliveryTrackingEnabled(bool enabled)
        {
            lp_setPushDeliveryTrackingEnabled(enabled);
        }

        /// <summary>
        ///     Gets a value indicating whether Leanplum has finished starting.
        /// </summary>
        /// <value><c>true</c> if this instance has started; otherwise, <c>false</c>.</value>
        public override bool HasStarted()
        {
            return lp_hasStarted();
        }

        /// <summary>
        ///     Gets a value indicating whether Leanplum has started and the device is registered
        ///     as a developer.
        /// </summary>
        /// <value>
        ///     <c>true</c> if Leanplum has started and the device registered as developer;
        ///     otherwise,
        ///     <c>false</c>.
        /// </value>
        public override bool HasStartedAndRegisteredAsDeveloper()
        {
            return lp_hasStartedAndRegisteredAsDeveloper();
        }

        /// <summary>
        ///     Gets whether or not developer mode is enabled.
        /// </summary>
        /// <value><c>true</c> if developer mode; otherwise, <c>false</c>.</value>
        public override bool IsDeveloperModeEnabled()
        {
            return lp_hasStarted() && isDeveloper;
        }

        /// <summary>
        ///     Gets the includeDefaults param value.
        /// </summary>
        public override bool GetIncludeDefaults()
        {
            return false;
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
            lp_setApiHostName(hostName, servletName, useSSL ? 1 : 0);
        }

        /// <summary>
        ///     Optional. Sets the socket server path for Development mode. Path is of the form
        ///     hostName:port
        /// </summary>
        /// <param name="hostName"> The host name of the socket server. </param>
        /// <param name="port"> The port to connect to. </param>
        public override void SetSocketConnectionSettings(string hostName, int port)
        {
            // Not supported by iOS SDK
        }

        /// <summary>
        ///     The default timeout is 10 seconds for requests, and 15 seconds for file downloads.
        /// </summary>
        /// <param name="seconds"> Timeout in seconds for standard webrequests. </param>
        /// <param name="downloadSeconds"> Timeout in seconds for downloads. </param>
        public override void SetNetworkTimeout(int seconds, int downloadSeconds)
        {
            lp_setNetworkTimeout(seconds, downloadSeconds);
        }

        /// <summary>
        ///     Sets the time interval between uploading events to server.
        ///     Default is <see cref="EventsUploadInterval.AtMost15Minutes"/>.
        /// </summary>
        /// <param name="uploadInterval"> The time between uploads. </param>
        public override void SetEventsUploadInterval(EventsUploadInterval uploadInterval)
        {
            lp_setEventsUploadInterval((int)uploadInterval);
        }

        /// <summary>
        ///     Must call either this or SetAppIdForProductionMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your development key. </param>
        public override void SetAppIdForDevelopmentMode(string appId, string accessKey)
        {
            lp_setAppIdDeveloper(appId, accessKey);
            isDeveloper = true;
        }

        /// <summary>
        ///     Must call either this or SetAppIdForDevelopmentMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your production key. </param>
        public override void SetAppIdForProductionMode(string appId, string accessKey)
        {
            lp_setAppIdProduction(appId, accessKey);
            isDeveloper = false;
        }

        /// <summary>
        ///    By default, Leanplum reports the version of your app using CFBundleVersion, which
        ///    can be used for reporting and targeting on the Leanplum dashboard.
        ///    If you wish to use CFBundleShortVersionString or any other string as the version,
        ///    you can call this before your call to [Leanplum start]
        /// </summary>
        /// <param name="version">Version.</param>
        public override void SetAppVersion(string version)
        {
            lp_setAppVersion(version);
        }

        public override void SetLogLevel(Constants.LogLevel logLevel)
        {
            lp_setLogLevel((int)logLevel);
        }

        /// <summary>
        ///     Sets a custom device ID. Device IDs should be unique across physical devices.
        /// </summary>
        /// <param name="deviceId">Device identifier.</param>
        public override void SetDeviceId(string deviceId)
        {
            lp_setDeviceId(deviceId);
        }

        /// <summary>
        ///     Gets current Device ID
        /// </summary>
        public override string GetDeviceId()
        {
            return lp_getDeviceId();
        }

        /// <summary>
        ///     Gets current User ID
        /// </summary>
        public override string GetUserId()
        {
            return lp_getUserId();
        }

        /// <summary>
        ///     This should be your first statement in a unit test. Setting this to true
        ///     will prevent Leanplum from communicating with the server.
        /// </summary>
        public override void SetTestMode(bool testModeEnabled)
        {
            lp_setTestModeEnabled(testModeEnabled);
        }

        /// <summary>
        ///     Sets whether the API should return default ("defaults in code") values
        ///     or only the overridden ones.
        ///     Used only in Development mode. Always false in production.
        /// </summary>
        /// <param name="includeDefaults"> The value for includeDefaults param. </param>
        public override void SetIncludeDefaultsInDevelopmentMode(bool includeDefaults)
        {
            // Not supported by iOS SDK.
        }

        /// <summary>
        ///     Sets whether realtime updates to the client are enabled in development mode.
        ///     This uses websockets which can have high CPU impact. Default: true.
        /// </summary>
        public override void SetRealtimeUpdatesInDevelopmentModeEnabled(bool enabled)
        {
            // Not supported by iOS SDK.
        }

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public override object ObjectForKeyPath(params object[] components)
        {
            string jsonString = lp_objectForKeyPath(Json.Serialize(components));
            return Json.Deserialize(jsonString);
        }

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public override object ObjectForKeyPathComponents(object[] pathComponents)
        {
            string jsonString = lp_objectForKeyPathComponents(Json.Serialize(pathComponents));
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
            lp_setDeviceLocationWithLatitude(latitude, longitude);
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
            lp_setDeviceLocationWithLatitude(latitude, longitude, (int)type);
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
            lp_setDeviceLocationWithLatitude(latitude, longitude, city, region, country, (int)type);
        }

        /// <summary>
        ///    Disables collecting location automatically. Will do nothing if Leanplum-Location is 
        ///    not used. Not supported on Native.
        /// </summary>
        public override void DisableLocationCollection()
        {
            lp_disableLocationCollection();
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
            lp_setGameObject(LeanplumUnityHelper.Instance.gameObject.name);

            // Invokes Started event through NativeCallback
            Started += startResponseAction;
            string attributesString = attributes == null ? null : Json.Serialize(attributes);
            lp_start(Constants.SDK_VERSION, userId, attributesString);
        }

        public override void ForceSyncVariables(Leanplum.SyncVariablesCompleted completedHandler)
        {
            // Not supported by iOS SDK.
        }

        public override void DefineAction(string name, Constants.ActionKind kind, ActionArgs args, IDictionary<string, object> options, ActionContext.ActionResponder responder)
        {
            DefineAction(name, kind, args, options, responder, null);
        }

        public override void DefineAction(string name, Constants.ActionKind kind, ActionArgs args, IDictionary<string, object> options,
            ActionContext.ActionResponder responder, ActionContext.ActionResponder dismissResponder)
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

            lp_defineAction(name, (int) kind, argString, optionString);
        }

        public override bool ShowMessage(string id)
        {
            // Not supported by iOS SDK.
            return false;
        }

        /// <summary>
        ///     Automatically tracks InApp purchase and does server side receipt validation.
        /// </summary>
        public override void TrackIOSInAppPurchases()
        {
            lp_trackIOSInAppPurchases();
        }

        /// <summary>
        ///     Logs a purchase event in your application. The string can be any
        ///     value of your choosing, however in most cases you will want to use
        ///     Leanplum.PURCHASE_EVENT_NAME
        /// </summary>
        public override void TrackPurchase(string eventName, double value, string currencyCode,
            IDictionary<string, object> parameters)
        {
            string parametersString = parameters == null ? null : Json.Serialize(parameters);
            lp_trackPurchase(eventName, value, currencyCode, parametersString);
        }

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public override void Track(string eventName, double value, string info,
            IDictionary<string, object> parameters)
        {
            string parametersString = parameters == null ? null : Json.Serialize(parameters);
            lp_track(eventName, value, info, parametersString);
        }

        /// <summary>
        ///     Sets the traffic source info for the current user.
        ///     Keys in info must be one of: publisherId, publisherName, publisherSubPublisher,
        ///     publisherSubSite, publisherSubCampaign, publisherSubAdGroup, publisherSubAd.
        /// </summary>
        public override void SetTrafficSourceInfo(IDictionary<string, string> info)
        {
            string infoString = (info == null) ? null : Json.Serialize(info);
            lp_setTrafficSourceInfo(infoString);
        }

        /// <summary>
        ///     Advances to a particular state in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     A state is a section of your app that the user is currently in.
        /// </summary>
        public override void AdvanceTo(string state, string info,
            IDictionary<string, object> parameters)
        {
            string parametersString = parameters == null ? null : Json.Serialize(parameters);
            lp_advanceTo(state, info, parametersString);
        }

        /// <summary>
        ///     Updates the user ID and adds or modifies user attributes.
        /// </summary>
        /// <param name="newUserId">New user identifier.</param>
        /// <param name="value">User attributes.</param>
        public override void SetUserAttributes(string newUserId,
            IDictionary<string, object> attributes)
        {
            string attributesString = attributes == null ? null : Json.Serialize(attributes);
            lp_setUserAttributes(newUserId, attributesString);
        }

        /// <summary>
        ///     Pauses the current state.
        ///     You can use this if your game has a "pause" mode. You shouldn't call it
        ///     when someone switches out of your app because that's done automatically.
        /// </summary>
        public override void PauseState()
        {
            lp_pauseState();
        }

        /// <summary>
        ///     Resumes the current state.
        /// </summary>
        public override void ResumeState()
        {
            lp_resumeState();
        }

        /// <summary>
        ///     Returns variant ids.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        public override List<object> Variants()
        {
            return (List<object>)Json.Deserialize(lp_variants());
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
            string jsonString = lp_securedVars();
            if (!string.IsNullOrEmpty(jsonString))
            {
                var varsDict = (Dictionary<string, object>)Json.Deserialize(jsonString);
                return LeanplumSecuredVars.FromDictionary(varsDict);
            }
            return null;
        }

        public override IDictionary<string, object> Vars()
        {
            string jsonString = lp_vars();
            return (Dictionary<string, object>)Json.Deserialize(jsonString);
        }

        /// <summary>
        ///     Returns metadata for all active in-app messages.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        public override Dictionary<string, object> MessageMetadata()
        {
            return (Dictionary<string, object>)Json.Deserialize(lp_messageMetadata());
        }

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        /// </summary>
        public override void ForceContentUpdate()
        {
            lp_forceContentUpdate();
        }

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        ///     The provided callback will always fire regardless
        ///     of whether the variables have changed.
        /// </summary>
        /// <param name="callback">The action to execute once the update completed.</param>
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
            ForceContentUpdateHandlersDictionary.Add(key, handler);
            lp_forceContentUpdateWithHandler(key);
        }

        #endregion

        public override void NativeCallback(string message)
        {
            const string VARIABLES_CHANGED = "VariablesChanged:";
            const string VARIABLES_CHANGED_NO_DOWNLOAD_PENDING = "VariablesChangedAndNoDownloadsPending:";
            const string ONCE_VARIABLES_CHANGED_NO_DOWNLOADS_PENDING = "OnceVariablesChangedAndNoDownloadsPending:";
            const string STARTED = "Started:";
            const string VARIABLE_VALUE_CHANGED = "VariableValueChanged:";
            const string FORCE_CONTENT_UPDATE_WITH_HANDLER = "ForceContentUpdateWithHandler:";
            const string DEFINE_ACTION_RESPONDER = "ActionResponder:";
            const string ACTION_DISMISS = "ActionDismiss:";
            const string ON_MESSAGE_DISPLAYED = "OnMessageDisplayed:";
            const string ON_MESSAGE_DISMISSED = "OnMessageDismissed:";
            const string ON_MESSAGE_ACTION = "OnMessageAction:";

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
                    callback?.Invoke();
                    OnceVariablesChangedAndNoDownloadsPendingDict.Remove(key);
                }
            }
            else if (message.StartsWith(STARTED))
            {
                if (started != null)
                {
                    startSuccessful = message.EndsWith("1");
                    started(startSuccessful);
                }
            }
            else if (message.StartsWith(VARIABLE_VALUE_CHANGED))
            {
                // Drop the beginning of the message to get the name of the variable
                // Then dispatch to the correct variable
                VariableValueChanged(message.Substring(21));
            }
            else if (message.StartsWith(FORCE_CONTENT_UPDATE_WITH_HANDLER))
            {
                string[] values = message.Substring(FORCE_CONTENT_UPDATE_WITH_HANDLER.Length).Split(':');
                int key = Convert.ToInt32(values[0]);
                bool success = values[1] == "1";
                if (ForceContentUpdateHandlersDictionary.TryGetValue(key, out Leanplum.ForceContentUpdateHandler handler))
                {
                    handler(success);
                    ForceContentUpdateHandlersDictionary.Remove(key);
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
            else if (message.StartsWith(ON_MESSAGE_DISPLAYED))
            {
                if (messageDisplayedHandler != null)
                {
                    string key = message.Substring(ON_MESSAGE_DISPLAYED.Length);
                    var context = CreateActionContextFromKey(key);
                    messageDisplayedHandler(context);
                }
            }
            else if (message.StartsWith(ON_MESSAGE_DISMISSED))
            {
                if (messageDismissedHandler != null)
                {
                    string key = message.Substring(ON_MESSAGE_DISMISSED.Length);
                    var context = CreateActionContextFromKey(key);
                    messageDismissedHandler(context);
                }
            }
            else if (message.StartsWith(ON_MESSAGE_ACTION))
            {
                if (messageActionHandler != null)
                {
                    string data = message.Substring(ON_MESSAGE_ACTION.Length);
                    // {actionName|parentActionName:parentMessageId:...:actionName:messageId}
                    char keysSeparator = '|';
                    string[] keys = data.Split(new char[] { keysSeparator }, StringSplitOptions.RemoveEmptyEntries);
                    if (keys.Length != 2)
                    {
                        return;
                    }

                    string actionName = keys[0];
                    string actionKey = keys[1];
                    var context = CreateActionContextFromKey(actionKey);
                    messageActionHandler.Invoke(actionName, context);
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

        private static ShouldDisplayMessageHandler shouldDisplayMessageHandler;

        [DllImport("__Internal")]
        private static extern void lp_onShouldDisplayMessage(ShouldDisplayMessageDelegate callback);

        private delegate int ShouldDisplayMessageDelegate(string key);

        // Must be static: IL2CPP does not support marshaling delegates that point to instance methods to native code.
        [MonoPInvokeCallback(typeof(ShouldDisplayMessageDelegate))]
        public static int ShouldDisplayMessageInternal(string key)
        {
            if (shouldDisplayMessageHandler == null)
            {
                return (int)MessageDisplayChoice.DisplayChoice.SHOW;
            }

            var context = CreateActionContextFromKey(key);

            var result = shouldDisplayMessageHandler(context);
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

        // The method the customer will call
        public override void ShouldDisplayMessage(ShouldDisplayMessageHandler handler)
        {
            shouldDisplayMessageHandler = handler;
            lp_onShouldDisplayMessage(ShouldDisplayMessageInternal);
        }

        private static PrioritizeMessagesHandler prioritizeMessagesHandler;

        [DllImport("__Internal")]
        private static extern void lp_onPrioritizeMessages(PrioritizeMessagesDelegate callback);

        private delegate string PrioritizeMessagesDelegate(string contexts, string actionTrigger);

        // Must be static: IL2CPP does not support marshaling delegates that point to instance methods to native code.
        [MonoPInvokeCallback(typeof(PrioritizeMessagesDelegate))]
        public static string PrioritizeMessagesDelegateInternal(string contextsKeys, string actionTrigger)
        {
            if (prioritizeMessagesHandler == null)
            {
                return contextsKeys;
            }

            string[] keys = contextsKeys.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            List<ActionContext> contexts = new List<ActionContext>();
            foreach (string key in keys)
            {
                var context = CreateActionContextFromKey(key);
                contexts.Add(context);
            }

            var eligibleContexts = prioritizeMessagesHandler(contexts.ToArray(), Json.Deserialize(actionTrigger) as Dictionary<string, object>);
            return string.Join(",", eligibleContexts.Select(x => x.Key));
        }

        // The method the customer will call
        public override void PrioritizeMessages(PrioritizeMessagesHandler handler)
        {
            prioritizeMessagesHandler = handler;
            if (handler != null) { 
                lp_onPrioritizeMessages(PrioritizeMessagesDelegateInternal);
            }
            else
            {
                lp_onPrioritizeMessages(null);
            }
        }

        public override void TriggerDelayedMessages()
        {
            lp_triggerDelayedMessages();
        }

        public override void OnMessageDisplayed(MessageHandler handler)
        {
            messageDisplayedHandler = handler;
            lp_onMessageDisplayed();
        }

        public override void OnMessageDismissed(MessageHandler handler)
        {
            messageDismissedHandler = handler;
            lp_onMessageDismissed();
        }

        public override void OnMessageAction(MessageActionHandler handler)
        {
            messageActionHandler = handler;
            lp_onMessageAction();
        }

        public override void SetActionManagerPaused(bool paused)
        {
            lp_setActionManagerPaused(paused);
        }

        public override void SetActionManagerEnabled(bool enabled)
        {
            lp_setActionManagerEnabled(enabled);
        }

        public override void SetActionManagerUseAsyncHandlers(bool enabled)
        {
            lp_setActionManagerUseAsyncHandlers(enabled);
        }

        public override ActionContext CreateActionContextForId(string actionId)
        {
            if (!string.IsNullOrEmpty(actionId))
            {
                string key = lp_createActionContextForId(actionId);
                return CreateActionContextFromKey(key);
            }
            return null;
        }

        public override bool TriggerActionForId(string actionId)
        {
            return lp_triggerAction(actionId);
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

        private static Tuple<string, string> GetActionNameMessageIdFromMessageKey(string key)
        {
            string actionName = GetActionNameFromMessageKey(key);
            string messageId = GetActionIdFromMessageKey(key);
            return new Tuple<string, string>(actionName, messageId);
        }

        private static ActionContextApple CreateActionContextFromKey(string key)
        {
            Tuple<string, string> actionNameMessageId = GetActionNameMessageIdFromMessageKey(key);
            return new ActionContextApple(key, actionNameMessageId.Item1, actionNameMessageId.Item2);
        }

        #endregion

        #region Dealing with Variables

        [DllImport("__Internal")]
        internal static extern void lp_defineVariable(string name, string kind, string jsonValue);

        [DllImport("__Internal")]
        internal static extern void lp_registerVariableCallback(string name);

        [DllImport("__Internal")]
        internal static extern string lp_getVariableValue(string name, string kind);

        public static IDictionary<string, Var> IOSVarCache = new Dictionary<string, Var>();

        private Var GetOrDefineVariable(string name, string kind, object defaultValue)
        {
            if (IOSVarCache.ContainsKey(name))
            {
                if (IOSVarCache[name].Kind != kind)
                {
                    Debug.LogError("\"" + name + "\" was already defined with a different kind");
                    return null;
                }
                return IOSVarCache[name];
            }
            lp_defineVariable(name, kind, Json.Serialize(defaultValue));
            return null;
        }

        public override Var<int> Define(string name, int defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.INT, defaultValue);
            return (cached != null) ? (Var<int>)cached :
                new AppleVar<int>(name, Constants.Kinds.INT, defaultValue);
        }

        public override Var<long> Define(string name, long defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.INT, defaultValue);
            return (cached != null) ? (Var<long>)cached :
                new AppleVar<long>(name, Constants.Kinds.INT, defaultValue);
        }

        public override Var<short> Define(string name, short defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.INT, defaultValue);
            return (cached != null) ? (Var<short>)cached :
                new AppleVar<short>(name, Constants.Kinds.INT, defaultValue);
        }

        public override Var<byte> Define(string name, byte defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.INT, defaultValue);
            return (cached != null) ? (Var<byte>)cached :
                new AppleVar<byte>(name, Constants.Kinds.INT, defaultValue);
        }

        public override Var<bool> Define(string name, bool defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.BOOLEAN, defaultValue);
            return (cached != null) ? (Var<bool>)cached :
                new AppleVar<bool>(name, Constants.Kinds.BOOLEAN, defaultValue);
        }

        public override Var<float> Define(string name, float defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.FLOAT, defaultValue);
            return (cached != null) ? (Var<float>)cached :
                new AppleVar<float>(name, Constants.Kinds.FLOAT, defaultValue);
        }

        public override Var<double> Define(string name, double defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.FLOAT, defaultValue);
            return (cached != null) ? (Var<double>)cached :
                new AppleVar<double>(name, Constants.Kinds.FLOAT, defaultValue);
        }

        public override Var<string> Define(string name, string defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.STRING, defaultValue);
            return (cached != null) ? (Var<string>)cached :
                new AppleVar<string>(name, Constants.Kinds.STRING, defaultValue);
        }

        public override Var<List<object>> Define(string name, List<object> defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.ARRAY, defaultValue);
            return (cached != null) ? (Var<List<object>>)cached :
                new AppleVar<List<object>>(name, Constants.Kinds.ARRAY, defaultValue);
        }

        public override Var<List<string>> Define(string name, List<string> defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.ARRAY, defaultValue);
            return (cached != null) ? (Var<List<string>>)cached :
                new AppleVar<List<string>>(name, Constants.Kinds.ARRAY, defaultValue);
        }

        public override Var<Dictionary<string, object>> Define(string name,
            Dictionary<string, object> defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.DICTIONARY, defaultValue);
            return (cached != null) ? (Var<Dictionary<string, object>>)cached :
                new AppleVar<Dictionary<string, object>>(name, Constants.Kinds.DICTIONARY,
                    defaultValue);
        }

        public override Var<Dictionary<string, string>> Define(string name,
            Dictionary<string, string> defaultValue)
        {
            Var cached = GetOrDefineVariable(name, Constants.Kinds.DICTIONARY, defaultValue);
            return (cached != null) ? (Var<Dictionary<string, string>>)cached :
                new AppleVar<Dictionary<string, string>>(name, Constants.Kinds.DICTIONARY,
                    defaultValue);
        }

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

            Var cached = GetOrDefineVariable(name, kind, defaultValue);
            return (cached != null) ? (Var<U>)cached : new AppleVar<U>(name, kind, defaultValue);
        }

        public override Var<AssetBundle> DefineAssetBundle(string name,
            bool realtimeUpdating = true, string iosBundleName = "", string androidBundleName = "",
            string standaloneBundleName = "")
        {
            // TODO: Not implemented.
            return null;
        }

        public static void VariableValueChanged(string name)
        {
            Var variable = IOSVarCache[name];
            if (variable != null)
            {
                variable.OnValueChanged();
            }
        }

        public override void AddOnceVariablesChangedAndNoDownloadsPendingHandler(Leanplum.VariablesChangedAndNoDownloadsPendingHandler handler)
        {
            int key = DictionaryKey++;
            OnceVariablesChangedAndNoDownloadsPendingDict.Add(key, handler);
            lp_addOnceVariablesChangedAndNoDownloadsPendingHandler(key);
        }
        #endregion
    }
}

#endif
