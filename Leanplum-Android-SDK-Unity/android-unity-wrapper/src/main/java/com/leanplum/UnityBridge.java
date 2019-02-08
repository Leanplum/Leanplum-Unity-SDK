//
//  Copyright (c) 2014 Leanplum. All rights reserved.
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
package com.leanplum;

import java.lang.reflect.Method;
import java.util.List;
import java.util.Map;

import org.json.JSONArray;
import org.json.JSONException;

import android.content.Context;
import android.util.Log;
import android.location.Location;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.leanplum.callbacks.StartCallback;
import com.leanplum.callbacks.VariableCallback;
import com.leanplum.callbacks.VariablesChangedCallback;
import com.leanplum.json.JsonConverter;
import com.unity3d.player.UnityPlayer;

public class UnityBridge {
  private static String unityGameObject;
  private static boolean isDeveloper = false;
  private static Gson gson = new Gson();
  private static Context bridgeContext;
  public static final String LEANPLUM_SENDER_ID = LeanplumPushService.LEANPLUM_SENDER_ID;

  private static final String CLIENT = "unity-nativeandroid";

  private static void makeCallbackToUnity(String message) {
    UnityPlayer.UnitySendMessage(unityGameObject, "NativeCallback", message);
  }

  public static void initialize(String gameObject, String sdkVersion, String defaultDeviceId) {
    if (bridgeContext != null) {
      return;
    }
    unityGameObject = gameObject;
    bridgeContext = UnityPlayer.currentActivity;
    try {
      final Method setClient = Leanplum.class.getDeclaredMethod(
          "setClient", String.class, String.class, String.class);
      setClient.setAccessible(true);
      setClient.invoke(null, CLIENT, sdkVersion, defaultDeviceId);
    } catch (Exception e) {
      Log.e("Leanplum", "Unable to set SDK version", e);
    }
    Leanplum.setApplicationContext(bridgeContext.getApplicationContext());
    LeanplumActivityHelper.enableLifecycleCallbacks(UnityPlayer.currentActivity
        .getApplication());
  }

  public static void setGcmSenderId(String senderId) {
    LeanplumPushService.setGcmSenderId(senderId);
  }

  public static void setGcmSenderIds(String senderIds) {
    final List<Object> ids;
    try {
      ids = JsonConverter.listFromJson(new JSONArray(senderIds));
    } catch (JSONException e) {
      Log.e("Leanplum", "Error parsing JSON data", e);
      return;
    }
    String[] idArray = new String[ids.size()];
    for (int i = 0; i < ids.size(); i++) {
      idArray[i] = ids.get(i).toString();
    }
    LeanplumPushService.setGcmSenderIds(senderIds);
  }

  public static boolean hasStarted() {
    return Leanplum.hasStarted();
  }

  public static boolean hasStartedAndRegisteredAsDeveloper() {
    return Leanplum.hasStartedAndRegisteredAsDeveloper();
  }

  public static boolean isDeveloperModeEnabled() {
    return Leanplum.hasStarted() && isDeveloper;
  }

  public static void setApiConnectionSettings(String hostName,
      String servletName, boolean ssl) {
    Leanplum.setApiConnectionSettings(hostName, servletName, ssl);
  }

  public static void setSocketConnectionSettings(String hostName, int port) {
    Leanplum.setSocketConnectionSettings(hostName, port);
  }

  public static void setNetworkTimeout(int seconds, int downloadSeconds) {
    Leanplum.setNetworkTimeout(seconds, downloadSeconds);
  }

  public static void setAppIdForDevelopmentMode(String appId, String accessKey) {
    isDeveloper = true;
    Leanplum.setAppIdForDevelopmentMode(appId, accessKey);
  }

  public static void setAppIdForProductionMode(String appId, String accessKey) {
    Leanplum.setAppIdForProductionMode(appId, accessKey);
  }

  public static void setDeviceId(String deviceId) {
    Leanplum.setDeviceId(deviceId);
  }

  public static void setTestModeEnabled(boolean enabled) {
    Leanplum.setIsTestModeEnabled(enabled);
  }

  public static void setFileUploadingEnabledInDevelopmentMode(boolean enabled) {
    Leanplum.setFileUploadingEnabledInDevelopmentMode(enabled);
  }

  public static String objectForKeyPath(String jsonComponents) {
    Object[] components = gson.fromJson(jsonComponents, Object[].class);
    return gson.toJson(Leanplum.objectForKeyPath(components));
  }

  public static String objectForKeyPathComponents(String jsonComponents) {
    Object[] components = gson.fromJson(jsonComponents, Object[].class);
    for (int i = 0; i < components.length; i++) {
      if (components[i] instanceof Number) {
        components[i] = ((Number) components[i]).intValue();
      }
    }
    return gson.toJson(Leanplum.objectForKeyPathComponents(components));
  }

  public static void start(final String userId, String jsonAttributes) {
    Leanplum.addVariablesChangedHandler(new VariablesChangedCallback() {
      @Override
      public void variablesChanged() {
        makeCallbackToUnity("VariablesChanged:");
      }
    });
    Leanplum.addVariablesChangedAndNoDownloadsPendingHandler(new VariablesChangedCallback() {
      @Override
      public void variablesChanged() {
        makeCallbackToUnity("VariablesChangedAndNoDownloadsPending:");
      }
    });
    Leanplum.addStartResponseHandler(new StartCallback() {
      @Override
      public void onResponse(boolean arg0) {
        makeCallbackToUnity("Started:" + arg0);
      }
    });

    final Map<String, Object> attributes = JsonConverter.fromJson(jsonAttributes);

    Leanplum.start(bridgeContext, userId, attributes);
  }

  public static void trackPurchase(String eventName, double value, String currencyCode,
      String jsonParameters) {
    Leanplum.trackPurchase(eventName, value, currencyCode,
        JsonConverter.fromJson(jsonParameters));
  }

  public static void track(String eventName, double value, String info,
      String jsonParameters) {
    Leanplum.track(eventName, value, info,
        JsonConverter.fromJson(jsonParameters));
  }

  public static void setTrafficSourceInfo(String jsonInfo) {
    // Because fromJson() returns Map<String, Object> need to cast to Map.
    Leanplum.setTrafficSourceInfo((Map) JsonConverter.fromJson(jsonInfo));
  }

  public static void advanceTo(String state, String info, String jsonParameters) {
    Leanplum.advanceTo(state, info, JsonConverter.fromJson(jsonParameters));
  }

  public static void setUserAttributes(String userId, String jsonAttributes) {
    Leanplum.setUserAttributes(userId, JsonConverter.fromJson(jsonAttributes));
  }

  public static void pauseState() {
    Leanplum.pauseState();
  }

  public static void resumeState() {
    Leanplum.resumeState();
  }

  public static String variants() {
    return gson.toJson(Leanplum.variants());
  }

  public static String messageMetadata() {
    return gson.toJson(Leanplum.messageMetadata());
  }

  public static void forceContentUpdate() {
    Leanplum.forceContentUpdate();
  }

  public static void forceContentUpdateWithCallback(final int key) {
    Leanplum.forceContentUpdate(new VariablesChangedCallback() {
      @Override
      public void variablesChanged() {
        makeCallbackToUnity("ForceContentUpdateWithCallback:" + key);
      }
    });
  }

  public static void defineVar(String name, String kind, String jsonValue) {
    if (kind.equals("integer")) {
      Long value = gson.fromJson(jsonValue, Long.class);
      Var.define(name, value);
    } else if (kind.equals("float")) {
      Double value = gson.fromJson(jsonValue, Double.class);
      Var.define(name, value);
    } else if (kind.equals("string")) {
      String value = gson.fromJson(jsonValue, String.class);
      Var.define(name, value);
    } else if (kind.equals("bool")) {
      Boolean value = gson.fromJson(jsonValue, Boolean.class);
      Var.define(name, value);
    } else if (kind.equals("group")) {
      Map<Object, Object> value = gson.fromJson(jsonValue,
          new TypeToken<Map<Object, Object>>() {
          }.getType());
      Var.define(name, value);
    } else if (kind.equals("list")) {
      List<Object> value = gson.fromJson(jsonValue,
          new TypeToken<List<Object>>() {
          }.getType());
      Var.define(name, value);
    } else if (kind.equals("file")) {
      Var.defineFile(name, jsonValue);
    }
  }

  public static void registerVarCallback(final String name) {
    Var<Object> variable = Var.define(name, null);
    variable.addValueChangedHandler(new VariableCallback<Object>() {
      @Override
      public void handle(Var<Object> arg0) {
        makeCallbackToUnity("VariableValueChanged:" + name);
      }
    });
  }

  public static String varValue(String name) {
    return gson.toJson(Var.define(name, null).value());
  }

  public static String fileValue(String name) {
    return Var.defineFile(name, "").fileValue();
  }

  public static String varNameComponents(String name) {
    return gson.toJson(Var.define(name, null).nameComponents());
  }

  public static void setDeviceLocation(double latitude, double longitude) {
    Location location = new Location("");
    location.setLatitude(latitude);
    location.setLongitude(longitude);
    Leanplum.setDeviceLocation(location);
  }

  public static void setDeviceLocation(double latitude, double longitude, int type) {
    Location location = new Location("");
    location.setLatitude(latitude);
    location.setLongitude(longitude);
    Leanplum.setDeviceLocation(location, LeanplumLocationAccuracyType.values()[type]);
  }

  public static void disableLocationCollection() {
    Leanplum.disableLocationCollection();
  }
}
