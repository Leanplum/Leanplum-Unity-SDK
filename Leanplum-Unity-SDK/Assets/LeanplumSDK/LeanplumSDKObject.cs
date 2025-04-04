//
// Copryight 2023, Leanplum, Inc.
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
using UnityEngine;

namespace LeanplumSDK
{
    public abstract class LeanplumSDKObject
    {
        public const string PURCHASE_EVENT_NAME = "Purchase";

        public abstract event Leanplum.VariableChangedHandler VariablesChanged;
        public abstract event Leanplum.VariablesChangedAndNoDownloadsPendingHandler
            VariablesChangedAndNoDownloadsPending;
        public abstract event Leanplum.StartHandler Started;
        public abstract event Leanplum.CleverTapInstanceHandler CleverTapInstanceReady;

        #region Networking

        internal virtual ApiConfig ApiConfig { get; }
        internal virtual RequestSender RequestSender { get; }
        internal virtual FileTransferManager FileTransferManager { get; }

        internal virtual LeanplumActionManager LeanplumActionManager { get; }

        #endregion

        #region Accessors and Mutators

        /// <summary>
        ///     Gets a value indicating whether Leanplum has finished starting.
        /// </summary>
        /// <value><c>true</c> if this instance has started; otherwise, <c>false</c>.</value>
        public abstract bool HasStarted();

        /// <summary>
        ///     Gets a value indicating whether Leanplum has started and the device is registered as
        ///     a developer.
        /// </summary>
        /// <value>
        ///     <c>true</c> if Leanplum has started and the device registered as developer;
        ///     otherwise,
        ///     <c>false</c>.
        /// </value>
        public abstract bool HasStartedAndRegisteredAsDeveloper();

        /// <summary>
        ///     Gets whether or not developer mode is enabled.
        /// </summary>
        /// <value><c>true</c> if developer mode; otherwise, <c>false</c>.</value>
        public abstract bool IsDeveloperModeEnabled();


        /// <summary>
        ///     Gets the includeDefaults param value.
        /// </summary>
        public abstract bool GetIncludeDefaults();

        /// <summary>
        ///     Optional. Sets the API server. The API path is of the form
        ///     http[s]://hostname/servletName
        /// </summary>
        /// <param name="hostName"> The name of the API host, such as www.leanplum.com </param>
        /// <param name="servletName"> The name of the API servlet, such as api </param>
        /// <param name="useSSL"> Whether to use SSL </param>
        public abstract void SetApiConnectionSettings(string hostName, string servletName = "api",
            bool useSSL = true);

        /// <summary>
        ///     Optional. Sets the socket server path for Development mode. Path is of the form
        ///     hostName:port
        /// </summary>
        /// <param name="hostName"> The host name of the socket server. </param>
        /// <param name="port"> The port to connect to. </param>
        public abstract void SetSocketConnectionSettings(string hostName, int port);

        /// <summary>
        ///     The default timeout is 10 seconds for requests, and 15 seconds for file downloads.
        /// </summary>
        /// <param name="seconds"> Timeout in seconds for standard webrequests. </param>
        /// <param name="downloadSeconds"> Timeout in seconds for downloads. </param>
        public abstract void SetNetworkTimeout(int seconds, int downloadSeconds);

        /// <summary>
        ///     Sets the time interval between uploading events to server.
        ///     Default is <see cref="EventsUploadInterval.AtMost15Minutes"/>.
        /// </summary>
        /// <param name="uploadInterval"> The time between uploads. </param>
        public abstract void SetEventsUploadInterval(EventsUploadInterval uploadInterval);

        /// <summary>
        ///     Must call either this or SetAppIdForProductionMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your development key. </param>
        public abstract void SetAppIdForDevelopmentMode(string appId, string accessKey);

        /// <summary>
        ///     Must call either this or SetAppIdForDevelopmentMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your production key. </param>
        public abstract void SetAppIdForProductionMode(string appId, string accessKey);

        /// <summary>
        ///     Set the application version to be sent to Leanplum.
        /// </summary>
        /// <param name="version">Version.</param>
        public abstract void SetAppVersion(string version);

        /// <summary>
        ///     Sets a custom device ID. Device IDs should be unique across physical devices.
        /// </summary>
        /// <param name="deviceId">Device identifier.</param>
        public abstract void SetDeviceId(string deviceId);

        /// <summary>
        ///     Gets device id.
        /// </summary>
        /// <returns>device id</returns>
        public abstract string GetDeviceId();

        /// <summary>
        ///     This should be your first statement in a unit test. Setting this to true
        ///     will prevent Leanplum from communicating with the server.
        /// </summary>
        public abstract void SetTestMode(bool testModeEnabled);

        /// <summary>
        ///     Sets whether the API should return default ("defaults in code") values
        ///     or only the overridden ones.
        ///     Used only in Development mode. Always false in production.
        /// </summary>
        /// <param name="includeDefaults"> The value for includeDefaults param. </param>
        public abstract void SetIncludeDefaultsInDevelopmentMode(bool includeDefaults);

        /// <summary>
        ///     Sets whether realtime updates to the client are enabled in development mode.
        ///     This uses websockets which can have high CPU impact. Default: true.
        /// </summary>
        public abstract void SetRealtimeUpdatesInDevelopmentModeEnabled(bool enabled);

        public virtual void RegisterForIOSRemoteNotifications() { }

        public virtual void SetPushDeliveryTrackingEnabled(bool enabled) { }

        public virtual void SetMiPushApplication(string miAppId, string miAppKey) { }

        /// <summary>
        /// Sets the logging level.
        /// </summary>
        /// <param name="logLevel"> Level to set. </param>
        public virtual void SetLogLevel(Constants.LogLevel logLevel) { }

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public abstract object ObjectForKeyPath(params object[] components);

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public abstract object ObjectForKeyPathComponents(object[] pathComponents);

        /// <summary>
        ///     Set location manually. Calls SetDeviceLocationWithLatitude with cell type. Best if 
        ///     used in after calling DisableLocationCollection. Not supported on Native.
        /// </summary>
        /// <param name="latitude"> Device location latitude. </param>
        /// <param name="longitude"> Device location longitude. </param>
        public abstract void SetDeviceLocation(double latitude, double longitude);

        /// <summary>
        ///     Set location manually. Calls SetDeviceLocationWithLatitude with cell type. Best if 
        ///     used in after calling DisableLocationCollection. Not supported on Native.
        /// </summary>
        /// <param name="latitude"> Device location latitude. </param>
        /// <param name="longitude"> Device location longitude. </param>
        /// <param name="type"> Location accuracy type. </param>
        public abstract void SetDeviceLocation(double latitude, double longitude, LPLocationAccuracyType type);

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
        public abstract void SetDeviceLocation(double latitude, double longitude, string city, string region, string country, LPLocationAccuracyType type);

        /// <summary>
        ///    Disables collecting location automatically. Will do nothing if Leanplum-Location is 
        ///    not used. Not supported on Native.
        /// </summary>
        public abstract void DisableLocationCollection();

        /// <summary>
        /// Returns an instance to the LeanplumInbox object.
        /// </summary>
        public abstract LeanplumInbox Inbox { get; }
        #endregion

        #region API Calls
        /// <summary>
        ///     Call this when your application starts.
        ///     This will initiate a call to Leanplum's servers to get the values
        ///     of the variables used in your app.
        /// </summary>
        public abstract void Start(string userId, IDictionary<string, object> attributes,
            Leanplum.StartHandler startResponseAction);

        /// <summary>
        ///     Automatically tracks InApp purchase and does server side receipt validation.
        /// </summary>
        public virtual void TrackIOSInAppPurchases() { }

        /// <summary>
        ///     Logs in-app purchase data from Google Play.
        /// </summary>
        /// <param name="item">The name of the item purchased.</param>
        /// <param name="priceMicros">The purchase price in micros.</param>
        /// <param name="currencyCode">The ISO 4217 currency code.</param>
        /// <param name="purchaseData">Purchase data from
        ///     com.android.vending.billing.util.Purchase.getOriginalJson().
        /// </param>
        /// <param name="dataSignature">Purchase data signature from
        ///     com.android.vending.billing.util.Purchase.getSignature().
        /// </param>
        /// <param name="parameters">Optional event parameters.</param>
        public virtual void TrackGooglePlayPurchase(string item, long priceMicros,
            string currencyCode, string purchaseData, string dataSignature,
            IDictionary<string, object> parameters)
        {
            TrackPurchase(Leanplum.PURCHASE_EVENT_NAME, priceMicros, currencyCode, parameters);
        }

        /// <summary>
        ///     Logs in-app purchase data from iOS.
        /// </summary>
        /// <param name="item">The name of the item purchased.</param>
        /// <param name="unitPrice">
        ///     The purchase price (from [[SKProduct price] doubleValue].
        /// </param>
        /// <param name="quantity">
        ///     The quantity (from [[SKPaymentTransaction payment] quantity].
        /// </param>
        /// <param name="currencyCode">
        ///     The ISO 4217 currency code (from [SKProduct priceLocale].
        /// </param>
        /// <param name="transactionIdentifier">
        ///     Transaction identifier from [SKPaymentTransaction transactionIdentifier].
        /// </param>
        /// <param name="receiptData">
        ///     Receipt data from [[NSData dataWithContentsOfURL:
        ///     [[NSBundle mainBundle] appStoreReceiptURL]] base64EncodedStringWithOptions:0].
        /// </param>
        /// <param name="parameters">Optional event parameters.</param>
        [Obsolete("TrackIOSInAppPurchase is obsolete. Please use TrackPurchase.")]
        public virtual void TrackIOSInAppPurchase(string item, double unitPrice, int quantity,
            string currencyCode, string transactionIdentifier, string receiptData,
            IDictionary<string, object> parameters)
        {
            TrackPurchase(Leanplum.PURCHASE_EVENT_NAME, unitPrice, currencyCode, parameters);
        }

        /// <summary>
        ///     Logs a purchase event in your application. The string can be any
        ///     value of your choosing, however in most cases you will want to use
        ///     Leanplum.PURCHASE_EVENT_NAME
        /// </summary>
        public abstract void TrackPurchase(string eventName, double value, string currencyCode,
            IDictionary<string, object> parameters);

        /// <summary>
        ///     Logs a particular event in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     To track purchases, use Leanplum.PURCHASE_EVENT_NAME as the event name.
        /// </summary>
        public abstract void Track(string eventName, double value, string info,
            IDictionary<string, object> parameters);

        /// <summary>
        ///     Sets the traffic source info for the current user.
        ///     Keys in info must be one of: publisherId, publisherName, publisherSubPublisher,
        ///     publisherSubSite, publisherSubCampaign, publisherSubAdGroup, publisherSubAd.
        /// </summary>
        public abstract void SetTrafficSourceInfo(IDictionary<string, string> info);

        /// <summary>
        ///     Advances to a particular state in your application. The string can be
        ///     any value of your choosing, and will show up in the dashboard.
        ///     A state is a section of your app that the user is currently in.
        /// </summary>
        public abstract void AdvanceTo(string state, string info,
            IDictionary<string, object> parameters);

        /// <summary>
        ///     Updates the user ID.
        /// </summary>
        /// <param name="newUserId">New user identifier.</param>
        public void SetUserId(string newUserId)
        {
            this.SetUserAttributes(newUserId, null);
        }

        /// <summary>
        ///     Gets user id.
        /// </summary>
        /// <returns>user id</returns>
        public abstract string GetUserId();

        /// <summary>
        ///     Adds or modifies user attributes.
        /// </summary>
        /// <param name="value">User attributes.</param>
        public void SetUserAttributes(IDictionary<string, object> value)
        {
            this.SetUserAttributes(null, value);
        }

        /// <summary>
        ///     Updates the user ID and adds or modifies user attributes.
        /// </summary>
        /// <param name="newUserId">New user identifier.</param>
        /// <param name="value">User attributes.</param>
        public abstract void SetUserAttributes(string newUserId,
            IDictionary<string, object> value);

        /// <summary>
        ///     Pauses the current state.
        ///     You can use this if your game has a "pause" mode. You shouldn't call it
        ///     when someone switches out of your app because that's done automatically.
        /// </summary>
        public abstract void PauseState();

        /// <summary>
        ///     Resumes the current state.
        /// </summary>
        public abstract void ResumeState();

        /// <summary>
        ///     Returns variant ids.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        public abstract List<object> Variants();

        public abstract IDictionary<string, object> Vars();

        /// <summary>
        ///     Returns the last received signed variables.
        ///     If signature was not provided from server the
        ///     result of this method will be null.
        /// </summary>
        /// <returns> Returns <see cref="LeanplumSecuredVars"/> instance containing
        ///     variable's JSON and signature.
        ///     If signature was not downloaded from server, returns null.
        /// </returns>
        public abstract LeanplumSecuredVars SecuredVars();

        /// <summary>
        ///     Returns current Migration Configuration.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        /// <returns> Returns <see cref="MigrationConfig"/> with CleverTap settings.
        /// </returns>
        public abstract MigrationConfig MigrationConfig();

        /// <summary>
        ///     Returns metadata for all active in-app messages.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        public abstract Dictionary<string, object> MessageMetadata();

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        /// </summary>
        public abstract void ForceContentUpdate();

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        ///     The provided callback will always fire regardless
        ///     of whether the variables have changed.
        /// </summary>
        ///
        public abstract void ForceContentUpdate(Action callback);

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
        public abstract void ForceContentUpdate(Leanplum.ForceContentUpdateHandler handler);

        /// <summary>
        ///     Registers a handler to be called when variables are fetched and no downloads
        ///     are pending. The handler will be called only once.
        /// </summary>
        /// <param name="handler">
        ///     The handler to execute once variables are fetched and there
        ///     aren't any pending downloads.
        /// </param>
        public abstract void AddOnceVariablesChangedAndNoDownloadsPendingHandler(
            Leanplum.VariablesChangedAndNoDownloadsPendingHandler handler);

        public abstract void DefineAction(string name, Constants.ActionKind kind, ActionArgs args,
            IDictionary<string, object> options, ActionContext.ActionResponder responder);

        public abstract void DefineAction(string name, Constants.ActionKind kind, ActionArgs args,
            IDictionary<string, object> options, ActionContext.ActionResponder responder, ActionContext.ActionResponder dismissResponder);

        /// <summary>
        ///     Manually Trigger an In-App Message. Supported in Unity only.
        ///     The user must be eligible for the message and the message must be present on the device (requires a Start call).
        /// </summary>
        /// <param name="id"> The message Id. </param>
        public abstract bool ShowMessage(string id);

        public abstract void ShouldDisplayMessage(Leanplum.ShouldDisplayMessageHandler handler);

        public abstract void PrioritizeMessages(Leanplum.PrioritizeMessagesHandler handler);

        public abstract void TriggerDelayedMessages();

        public abstract void OnMessageDisplayed(Leanplum.MessageHandler handler);

        public abstract void OnMessageDismissed(Leanplum.MessageHandler handler);

        public abstract void OnMessageAction(Leanplum.MessageActionHandler handler);

        public abstract void SetActionManagerPaused(bool paused);

        public abstract void SetActionManagerEnabled(bool enabled);

        public abstract void SetActionManagerUseAsyncHandlers(bool enabled);
        #endregion

        public virtual void NativeCallback(string message) { }

        public abstract ActionContext CreateActionContextForId(string actionId);

        public abstract bool TriggerActionForId(string actionId);

        #region Dealing with Variables
        /// <summary>
        ///     Syncs the variables defined from code without Dashboard interaction.
        ///     Requires Development mode.
        ///     Not available on Android and iOS.
        /// </summary>
        /// <param name="completedHandler"> Handler to be called when request is completed. Returns true if sync was successful. </param>
        public abstract void ForceSyncVariables(Leanplum.SyncVariablesCompleted completedHandler);

        public virtual Var<int> Define(string name, int defaultValue)
        {
            return Define<int>(name, defaultValue);
        }

        public virtual Var<long> Define(string name, long defaultValue)
        {
            return Define<long>(name, defaultValue);
        }

        public virtual Var<short> Define(string name, short defaultValue)
        {
            return Define<short>(name, defaultValue);
        }

        public virtual Var<byte> Define(string name, byte defaultValue)
        {
            return Define<byte>(name, defaultValue);
        }

        public virtual Var<bool> Define(string name, bool defaultValue)
        {
            return Define<bool>(name, defaultValue);
        }

        public virtual Var<float> Define(string name, float defaultValue)
        {
            return Define<float>(name, defaultValue);
        }

        public virtual Var<double> Define(string name, double defaultValue)
        {
            return Define<double>(name, defaultValue);
        }

        public virtual Var<string> Define(string name, string defaultValue)
        {
            return Define<string>(name, defaultValue);
        }

        public virtual Var<List<object>> Define(string name, List<object> defaultValue)
        {
            return Define<List<object>>(name, defaultValue);
        }

        public virtual Var<List<string>> Define(string name, List<string> defaultValue)
        {
            return Define<List<string>>(name, defaultValue);
        }

        public virtual Var<Dictionary<string, object>> Define(string name,
            Dictionary<string, object> defaultValue)
        {
            return Define<Dictionary<string, object>>(name, defaultValue);
        }

        public virtual Var<Dictionary<string, string>> Define(string name,
            Dictionary<string, string> defaultValue)
        {
            return Define<Dictionary<string, string>>(name, defaultValue);
        }

        public virtual Var<U> Define<U>(string name, U defaultValue)
        {
            return null;
        }

        public abstract Var<AssetBundle> DefineAssetBundle(string name,
            bool realtimeUpdating = true, string iosBundleName = "", string androidBundleName = "",
            string standaloneBundleName = "");

        #endregion
    }
}

