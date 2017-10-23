//
// Copryight 2014, Leanplum, Inc.
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

        #region Accessors and Mutators
        public virtual string LeanplumGcmSenderId
        {
            get { return ""; }
        }

        /// <summary>
        ///     Gets a value indicating whether Leanplum has finished starting.
        /// </summary>
        /// <value><c>true</c> if this instance has started; otherwise, <c>false</c>.</value>
        public abstract bool HasStarted ();

        /// <summary>
        ///     Gets a value indicating whether Leanplum has started and the device is registered as
        ///     a developer.
        /// </summary>
        /// <value>
        ///     <c>true</c> if Leanplum has started and the device registered as developer;
        ///     otherwise,
        ///     <c>false</c>.
        /// </value>
        public abstract bool HasStartedAndRegisteredAsDeveloper ();

        /// <summary>
        ///     Gets whether or not developer mode is enabled.
        /// </summary>
        /// <value><c>true</c> if developer mode; otherwise, <c>false</c>.</value>
        public abstract bool IsDeveloperModeEnabled ();

        /// <summary>
        ///     Optional. Sets the API server. The API path is of the form
        ///     http[s]://hostname/servletName
        /// </summary>
        /// <param name="hostName"> The name of the API host, such as www.leanplum.com </param>
        /// <param name="servletName"> The name of the API servlet, such as api </param>
        /// <param name="useSSL"> Whether to use SSL </param>
        public abstract void SetApiConnectionSettings (string hostName, string servletName = "api",
            bool useSSL = true);

        /// <summary>
        ///     Optional. Sets the socket server path for Development mode. Path is of the form
        ///     hostName:port
        /// </summary>
        /// <param name="hostName"> The host name of the socket server. </param>
        /// <param name="port"> The port to connect to. </param>
        public abstract void SetSocketConnectionSettings (string hostName, int port);

        /// <summary>
        ///     The default timeout is 10 seconds for requests, and 15 seconds for file downloads.
        /// </summary>
        /// <param name="seconds"> Timeout in seconds for standard webrequests. </param>
        /// <param name="downloadSeconds"> Timeout in seconds for downloads. </param>
        public abstract void SetNetworkTimeout (int seconds, int downloadSeconds);

        /// <summary>
        ///     Must call either this or SetAppIdForProductionMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your development key. </param>
        public abstract void SetAppIdForDevelopmentMode (string appId, string accessKey);

        /// <summary>
        ///     Must call either this or SetAppIdForDevelopmentMode
        ///     before issuing any calls to the API, including start.
        /// </summary>
        /// <param name="appId"> Your app ID. </param>
        /// <param name="accessKey"> Your production key. </param>
        public abstract void SetAppIdForProductionMode (string appId, string accessKey);

        /// <summary>
        ///     Set the application version to be sent to Leanplum.
        /// </summary>
        /// <param name="version">Version.</param>
        public abstract void SetAppVersion (string version);

        /// <summary>
        ///     Sets a custom device ID. Device IDs should be unique across physical devices.
        /// </summary>
        /// <param name="deviceId">Device identifier.</param>
        public abstract void SetDeviceId (string deviceId);

        /// <summary>
        ///     This should be your first statement in a unit test. Setting this to true
        ///     will prevent Leanplum from communicating with the server.
        /// </summary>
        public abstract void SetTestMode (bool testModeEnabled);

        /// <summary>
        ///     Sets whether realtime updates to the client are enabled in development mode.
        ///     This uses websockets which can have high CPU impact. Default: true.
        /// </summary>
        public abstract void SetRealtimeUpdatesInDevelopmentModeEnabled (bool enabled);

        public virtual void SetGcmSenderId(string senderId) {}
        public virtual void SetGcmSenderIds(string[] senderIds) {}
        public virtual void RegisterForIOSRemoteNotifications() {}

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public abstract object ObjectForKeyPath (params object[] components);

        /// <summary>
        ///     Traverses the variable structure with the specified path.
        ///     Path components can be either strings representing keys in a dictionary,
        ///     or integers representing indices in a list.
        /// </summary>
        public abstract object ObjectForKeyPathComponents (object[] pathComponents);
        #endregion

        #region API Calls
        /// <summary>
        ///     Call this when your application starts.
        ///     This will initiate a call to Leanplum's servers to get the values
        ///     of the variables used in your app.
        /// </summary>
        public abstract void Start(string userId, IDictionary<string, object> attributes,
            Leanplum.StartHandler startResponseAction);

        public virtual void TrackIOSInAppPurchases() {}

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
            Debug.LogError("TrackGooglePlayPurchase is not supported on the current platform. " +
                "Call the method in the platform's native SDK.");
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
        public virtual void TrackIOSInAppPurchase(string item, double unitPrice, int quantity,
            string currencyCode, string transactionIdentifier, string receiptData,
            IDictionary<string, object> parameters)
        {
            Debug.LogError("TrackIOSInAppPurchase is not supported on the current platform. Call " +
                "the method in the platform's native SDK.");
        }

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
        public void SetUserId (string newUserId)
        {
            this.SetUserAttributes(newUserId, null);
        }

        /// <summary>
        ///     Adds or modifies user attributes.
        /// </summary>
        /// <param name="value">User attributes.</param>
        public void SetUserAttributes (IDictionary<string, object> value)
        {
            this.SetUserAttributes(null, value);
        }

        /// <summary>
        ///     Updates the user ID and adds or modifies user attributes.
        /// </summary>
        /// <param name="newUserId">New user identifier.</param>
        /// <param name="value">User attributes.</param>
        public abstract void SetUserAttributes (string newUserId,
            IDictionary<string, object> value);

        /// <summary>
        ///     Pauses the current state.
        ///     You can use this if your game has a "pause" mode. You shouldn't call it
        ///     when someone switches out of your app because that's done automatically.
        /// </summary>
        public abstract void PauseState ();

        /// <summary>
        ///     Resumes the current state.
        /// </summary>
        public abstract void ResumeState ();

        /// <summary>
        ///     Returns variant ids.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        public abstract List<object> Variants ();

        /// <summary>
        ///     Returns metadata for all active in-app messages.
        ///     Recommended only for debugging purposes and advanced use cases.
        /// </summary>
        public abstract Dictionary<string, object> MessageMetadata ();

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        /// </summary>
        public abstract void ForceContentUpdate ();

        /// <summary>
        ///     Forces content to update from the server. If variables have changed, the
        ///     appropriate callbacks will fire. Use sparingly as if the app is updated,
        ///     you'll have to deal with potentially inconsistent state or user experience.
        ///     The provided callback will always fire regardless
        ///     of whether the variables have changed.
        /// </summary>
        ///
        public abstract void ForceContentUpdate (Action callback);

        #endregion

        public virtual void NativeCallback(string message) {}

        #region Dealing with Variables
        public virtual Var<int> Define(string name, int defaultValue)
        {
            return Define<int> (name, defaultValue);
        }

        public virtual Var<long> Define(string name, long defaultValue)
        {
            return Define<long> (name, defaultValue);
        }

        public virtual Var<short> Define(string name, short defaultValue)
        {
            return Define<short> (name, defaultValue);
        }

        public virtual Var<byte> Define(string name, byte defaultValue)
        {
            return Define<byte> (name, defaultValue);
        }

        public virtual Var<bool> Define(string name, bool defaultValue)
        {
            return Define<bool> (name, defaultValue);
        }

        public virtual Var<float> Define(string name, float defaultValue)
        {
            return Define<float> (name, defaultValue);
        }

        public virtual Var<double> Define(string name, double defaultValue)
        {
            return Define<double> (name, defaultValue);
        }

        public virtual Var<string> Define(string name, string defaultValue)
        {
            return Define<string> (name, defaultValue);
        }

        public virtual Var<List<object>> Define(string name, List<object> defaultValue)
        {
            return Define<List<object>> (name, defaultValue);
        }

        public virtual Var<List<string>> Define(string name, List<string> defaultValue)
        {
            return Define<List<string>> (name, defaultValue);
        }

        public virtual Var<Dictionary<string, object>> Define(string name,
            Dictionary<string, object> defaultValue)
        {
            return Define<Dictionary<string, object>> (name, defaultValue);
        }

        public virtual Var<Dictionary<string, string>> Define(string name,
            Dictionary<string, string> defaultValue)
        {
            return Define<Dictionary<string, string>> (name, defaultValue);
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

