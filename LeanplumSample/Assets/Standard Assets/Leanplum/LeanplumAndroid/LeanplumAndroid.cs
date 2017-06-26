//
// Copyright 2014, Leanplum, Inc.
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

    public override event Leanplum.VariableChangedHandler VariablesChanged;
    public override event Leanplum.VariablesChangedAndNoDownloadsPendingHandler
        VariablesChangedAndNoDownloadsPending;
    public override event Leanplum.StartHandler Started;
    private Dictionary <int, Action> ForceContentUpdateCallbackDictionary =
        new Dictionary<int, Action>();
    static private int DictionaryKey = 0;
    private string gameObjectName;

    public LeanplumAndroid()
    {
      AndroidJNI.AttachCurrentThread();
      nativeSdk = new AndroidJavaClass("com.leanplum.UnityBridge");

      // This also constructs LeanplumUnityHelper and the game object.
      gameObjectName = LeanplumUnityHelper.Instance.gameObject.name;
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
    ///     This should be your first statement in a unit test. Setting this to true
    ///     will prevent Leanplum from communicating with the server.
    /// </summary>
    public override void SetTestMode(bool testModeEnabled)
    {
      NativeSDK.CallStatic("setTestModeEnabled", testModeEnabled);
    }

    /// <summary>
    ///     Sets whether realtime updates to the client are enabled in development mode.
    ///     This uses websockets which can have high CPU impact. Default: true.
    /// </summary>
    public override void SetRealtimeUpdatesInDevelopmentModeEnabled(bool enabled)
    {
      NativeSDK.CallStatic("setFileUploadingEnabledInDevelopmentMode", enabled);
    }

    public override string LeanplumGcmSenderId
    {
      get { return NativeSDK.GetStatic<string>("LEANPLUM_SENDER_ID"); }
        }

        public override void SetGcmSenderId(string senderId)
    {
      NativeSDK.CallStatic("setGcmSenderId", senderId);
        }

    public override void SetGcmSenderIds(string[] senderIds)
    {
      NativeSDK.CallStatic("setGcmSenderIds", Json.Serialize(senderIds));
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
      Started += startResponseAction;
      NativeSDK.CallStatic("initialize", gameObjectName, SharedConstants.SDK_VERSION, null);
      NativeSDK.CallStatic("start", userId, Json.Serialize(attributes));
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
      int key = DictionaryKey++;
      ForceContentUpdateCallbackDictionary.Add(key, callback);
      NativeSDK.CallStatic("forceContentUpdateWithCallback", key);
    }

    #endregion

    public override void NativeCallback(string message)
    {
      if (message.StartsWith("VariablesChanged:"))
      {
        if (VariablesChanged != null)
        {
          VariablesChanged();
        }
      }
      else if (message.StartsWith("VariablesChangedAndNoDownloadsPending:"))
      {
        if (VariablesChangedAndNoDownloadsPending != null)
        {
          VariablesChangedAndNoDownloadsPending();
        }
      }
      else if (message.StartsWith("Started:"))
      {
        if (Started != null)
        {
          bool success = message.EndsWith("true") || message.EndsWith("True");
          Started(success);
        }
      }
      else if (message.StartsWith("VariableValueChanged:"))
      {
        // Drop the beginning of the message to get the name of the variable
        // Then dispatch to the correct variable
        LeanplumAndroid.VariableValueChanged(message.Substring(21));
      }
      else if (message.StartsWith("ForceContentUpdateWithCallback:"))
      {
        int key = Convert.ToInt32(message.Substring(31));
        Action callback;
        if (ForceContentUpdateCallbackDictionary.TryGetValue(key, out callback)) {
          callback();
          ForceContentUpdateCallbackDictionary.Remove(key);
        }
      }
    }

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
        return (Var<U>) AndroidVarCache[name];
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
        return (Var<AssetBundle>) AndroidVarCache[actualName];
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

    #endregion

  }
}

#endif
