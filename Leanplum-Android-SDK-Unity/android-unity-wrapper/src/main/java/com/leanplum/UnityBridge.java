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
import java.text.Format;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

import org.json.JSONArray;
import org.json.JSONException;

import android.content.Context;
import android.util.Log;
import android.location.Location;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.leanplum.callbacks.ActionCallback;
import com.leanplum.callbacks.InboxChangedCallback;
import com.leanplum.callbacks.InboxSyncedCallback;
import com.leanplum.callbacks.StartCallback;
import com.leanplum.callbacks.VariableCallback;
import com.leanplum.callbacks.VariablesChangedCallback;
import com.leanplum.internal.ActionManager;
import com.leanplum.internal.CollectionUtil;
import com.leanplum.internal.Constants;
import com.leanplum.internal.LeanplumInternal;
import com.leanplum.internal.Util;
import com.leanplum.internal.VarCache;
import com.leanplum.json.JsonConverter;
import com.unity3d.player.UnityPlayer;

public class UnityBridge {
  private static String unityGameObject;
  private static boolean isDeveloper = false;
  private static Gson gson = new Gson();
  private static Context bridgeContext;

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

  public static void setEventsUploadInterval(int uploadInterval) {
    EventsUploadInterval interval = null;
    for (EventsUploadInterval i : EventsUploadInterval.values()) {
      if (i.getMinutes() == uploadInterval) {
        interval = i;
        break;
      }
    }
    
    if (interval != null) {
      Leanplum.setEventsUploadInterval(interval);
    }
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

  public static String getDeviceId() {
    return Leanplum.getDeviceId();
  }

  public static String getUserId() {
    return Leanplum.getUserId();
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

    Leanplum.getInbox().addChangedHandler(new InboxChangedCallback() {
      @Override
      public void inboxChanged() {
        makeCallbackToUnity("InboxOnChanged");
      }
    });

    Leanplum.getInbox().addSyncedHandler(new InboxSyncedCallback() {
      @Override
      public void onForceContentUpdate(boolean success) {
        makeCallbackToUnity("InboxForceContentUpdate:" + (success ? 1 : 0));
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

  public static String vars() {
    return gson.toJson(VarCache.getDiffs());
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

  public static void defineAction(final String name, int kind, String args, String options) {
    if (name == null) {
      return;
    }

    List<Object> argsArray = gson.fromJson(args,
            new TypeToken<List<Object>>() {
            }.getType());

    ActionArgs actionArgs = new ActionArgs();

    for (Object arg : argsArray) {
      if (arg instanceof Map) {
        Map<String, Object> dict = (Map<String, Object>) arg;
        String argName = (String) dict.get("name");
        String argKind = (String) dict.get("kind");
        Object defaultValue = dict.get("defaultValue");

        if (argName == null || argKind == null || (defaultValue == null && !argKind.equals("action"))) {
          continue;
        }

        if (argKind.equals("integer")) {
          actionArgs.with(argName, defaultValue);
        } else if (argKind.equals("float")) {
          actionArgs.with(argName, defaultValue);
        } else if (argKind.equals("string")) {
          actionArgs.with(argName, defaultValue);
        } else if (argKind.equals("bool")) {
          actionArgs.with(argName, defaultValue);
        } else if (argKind.equals("group")) {
          actionArgs.with(argName, defaultValue);
        } else if (argKind.equals("list")) {
          actionArgs.with(argName, defaultValue);
        } else if (argKind.equals("action")) {
          if (defaultValue instanceof String) {
            actionArgs.withAction(argName, (String) defaultValue);
          } else if (defaultValue == null) {
            actionArgs.withAction(argName, null);
          }
        } else if (argKind.equals("color")) {
          // defaultValue can come as a double with E7 notation
          if (defaultValue instanceof Double) {
            defaultValue = ((Double) defaultValue).intValue();
          }
          if (defaultValue instanceof Integer) {
            actionArgs.withColor(argName, (int) defaultValue);
          }
        }
        else if (argKind.equals("file")) {
            actionArgs.withFile(argName, (String) defaultValue);
        }
      }
    }

    Leanplum.defineAction(name, kind, actionArgs, new ActionCallback() {
      @Override
      public boolean onResponse(ActionContext context) {
        sendMessageActionContext("ActionResponder", name, context);
        return true;
      }
    });
  }

  public static void onAction(final String name) {
    Leanplum.onAction(name, new ActionCallback() {
      @Override
      public boolean onResponse(ActionContext context) {
        sendMessageActionContext("OnAction", name, context);
        return true;
      }
    });
  }

  public static String createActionContextForId(final String actionId) {
    Map<String, Object> messages = VarCache.messages();
    if (messages == null) return null;

    Map<String, Object> messageConfig = CollectionUtil.uncheckedCast(messages.get(actionId));
    if (messageConfig == null) return null;

    String actionName = (String) messageConfig.get("action");
    if (actionName == null) return null;

    Map<String, Object> vars = CollectionUtil.uncheckedCast(messageConfig.get(Constants
            .Keys.VARS));
    ActionContext context = new ActionContext(
            actionName,
            vars,
            null,
            actionId,
            Constants.Messaging.DEFAULT_PRIORITY);

    String key = String.format("%s:%s", actionName, context.getMessageId());
    UnityActionContextBridge.actionContexts.put(key, context);

    return key;
  }

  public static boolean triggerAction(final String actionId) {
    ActionContext context = UnityActionContextBridge.actionContexts.get(actionId);
    if (context == null) {
      Set<String> keys = UnityActionContextBridge.actionContexts.keySet();
      for (String key:keys) {
        if (key.contains(actionId)) {
          context = UnityActionContextBridge.actionContexts.get(key);
          break;
        }
      }
      if (context == null) {
        String key = createActionContextForId(actionId);
        context = UnityActionContextBridge.actionContexts.get(key);
      }
    }

    if (context != null) {
      final ActionContext actionContext = context;
      LeanplumInternal.triggerAction(actionContext, new VariablesChangedCallback() {
        @Override
        public void variablesChanged() {
          try {
            Leanplum.triggerMessageDisplayed(actionContext);
          } catch (Throwable t) {
          }
        }
      });
      return true;
    }

    return false;
  }

  private static void sendMessageActionContext(String message, String name, ActionContext context){
    if (name != null && context != null) {
      String key = String.format("%s:%s", name, context.getMessageId());
      UnityActionContextBridge.actionContexts.put(key, context);
      String callbackMessage = String.format("%s:%s", message, key);
      makeCallbackToUnity(callbackMessage);
    }
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

  public static int inboxCount() {
    return Leanplum.getInbox().count();
  }

  public static int inboxUnreadCount() {
    return Leanplum.getInbox().unreadCount();
  }

  public static String inboxMessageIds() {
    return gson.toJson(Leanplum.getInbox().messagesIds());
  }

  public static void downloadMessages() {
    Leanplum.getInbox().downloadMessages();
  }

  public static void downloadMessagesWithCallback() {
    Leanplum.getInbox().downloadMessages(new InboxSyncedCallback() {
      @Override
      public void onForceContentUpdate(boolean success) {
        int result = success ? 1 : 0;
        makeCallbackToUnity("InboxDownloadMessages:" + result);
      }
    });
  }

  public static String inboxMessages() {
    String pattern = "yyyy-MM-dd'T'HH:mm:ssZZZZZ";
    SimpleDateFormat formatter = new SimpleDateFormat(pattern);

    ArrayList<Map<String, Object>> messages = new ArrayList<>();
    for(LeanplumInboxMessage msg : Leanplum.getInbox().allMessages()) {
      Map<String, Object> message = new HashMap<>();
      message.put("id", msg.getMessageId());
      message.put("title", msg.getTitle());
      message.put("subtitle", msg.getSubtitle());
      message.put("imageFilePath", msg.getImageFilePath());
      message.put("imageURL", msg.getImageUrl());
      if (msg.getDeliveryTimestamp() != null) {
        message.put("deliveryTimestamp", formatter.format(msg.getDeliveryTimestamp()));
      }
      if (msg.getExpirationTimestamp() != null) {
        message.put("expirationTimestamp", formatter.format(msg.getExpirationTimestamp()));
      }
      message.put("isRead", msg.isRead());
      messages.add(message);
    }
    return gson.toJson(messages);
  }

  public static void inboxRead(String messageId) {
    LeanplumInboxMessage message = Leanplum.getInbox().messageForId(messageId);
    if (message != null) {
      message.read();
    }
  }

  public static void inboxMarkAsRead(String messageId) {
    LeanplumInboxMessage message = Leanplum.getInbox().messageForId(messageId);
    if (message != null) {
      message.markAsRead();
    }
  }

  public static void inboxRemove(String messageId) {
    LeanplumInboxMessage message = Leanplum.getInbox().messageForId(messageId);
    if (message != null) {
      message.remove();
    }
  }

  public static void inboxDisableImagePrefetching() {
    LeanplumInbox.disableImagePrefetching();
  }
}
