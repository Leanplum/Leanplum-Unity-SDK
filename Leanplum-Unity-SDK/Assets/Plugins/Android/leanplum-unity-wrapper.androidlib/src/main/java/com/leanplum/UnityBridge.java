//
//  Copyright (c) 2023 Leanplum. All rights reserved.
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

import android.content.Context;
import android.location.Location;
import android.util.Log;

import androidx.annotation.NonNull;

import com.clevertap.android.sdk.CleverTapAPI;
import com.clevertap.unity.CleverTapUnityAPI;
import com.clevertap.unity.CleverTapUnityPlugin;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.leanplum.actions.LeanplumActions;
import com.leanplum.actions.MessageDisplayController;
import com.leanplum.actions.MessageDisplayControllerImpl;
import com.leanplum.actions.MessageDisplayListener;
import com.leanplum.actions.MessageDisplayListenerImpl;
import com.leanplum.actions.internal.ActionManagerTriggeringKt;
import com.leanplum.actions.internal.ActionsTrigger;
import com.leanplum.actions.internal.Priority;
import com.leanplum.callbacks.ActionCallback;
import com.leanplum.callbacks.CleverTapInstanceCallback;
import com.leanplum.callbacks.ForceContentUpdateCallback;
import com.leanplum.callbacks.InboxChangedCallback;
import com.leanplum.callbacks.InboxSyncedCallback;
import com.leanplum.callbacks.StartCallback;
import com.leanplum.callbacks.VariableCallback;
import com.leanplum.callbacks.VariablesChangedCallback;
import com.leanplum.internal.ActionManager;
import com.leanplum.internal.CollectionUtil;
import com.leanplum.internal.Constants;
import com.leanplum.internal.OperationQueue;
import com.leanplum.internal.VarCache;
import com.leanplum.json.JsonConverter;
import com.leanplum.migration.MigrationManager;
import com.leanplum.migration.model.MigrationConfig;
import com.unity3d.player.UnityPlayer;

import java.lang.reflect.Method;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

public class UnityBridge {
  private static String unityGameObject;
  private static boolean isDeveloper = false;
  private static Gson gson = new Gson();
  private static Context bridgeContext;
  private static IJavaBridge javaBridge;
  private static MessageDisplayController messageController;
  private static MessageDisplayListener messageListener;

  private static final String CLIENT = "unity-nativeandroid";

  static void makeCallbackToUnity(final String message) {
    final Runnable sendMessageOperation = new Runnable() {
      @Override
      public void run() {
        UnityPlayer.UnitySendMessage(unityGameObject, "NativeCallback", message);
      }
    };
    OperationQueue.sharedInstance().addParallelOperation(sendMessageOperation);
  }

  public static void initialize(
      String gameObject,
      String sdkVersion,
      String defaultDeviceId) {
    if (Leanplum.getContext() == null) {
      Log.e("Leanplum", "It is mandatory to use LeanplumUnityApplication as base class for your Application class or call LeanplumActivityHelper.enableLifecycleCallbacks(this) from your Application.onCreate() method.");
    }
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

  /**
   * For internal purposes only.
   */
  public static void forceNewDeviceId(String deviceId) {
    Leanplum.forceNewDeviceId(deviceId);
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

    Leanplum.addCleverTapInstanceCallback(new CleverTapInstanceCallback() {
      @Override
      public void onInstance(@NonNull CleverTapAPI cleverTapInstance) {
        CleverTapUnityAPI.setCleverTapApiInstance(cleverTapInstance);
        // Create the instance of the CT plugin. This would ensure the message
        // buffers get disabled. The outside "start" method is called from the Unity
        // Leanplum class, so this callback will be called after Unity is initialized.
        CleverTapUnityPlugin.getInstance(bridgeContext);
        String accountId = MigrationConfig.INSTANCE.getAccountId();
        makeCallbackToUnity("CleverTapInstance:" + accountId);
      }
    });

    final Map<String, Object> attributes = JsonConverter.fromJson(jsonAttributes);

    Leanplum.start(bridgeContext, userId, attributes);
  }

  public static void setLogLevel(int logLevel) {
    Leanplum.setLogLevel(logLevel);
  }

  /**
   * @deprecated MiPush implementation of Leanplum will be replaced with CleverTap SDK.
   */
  @Deprecated
  public static void setMiPushApplication(String miAppId, String miAppKey) {
    try {
      Class.forName("com.leanplum.LeanplumMiPushHandler").getDeclaredMethod("setApplication", String.class, String.class).invoke((Object)null, miAppId, miAppKey);
    } catch (Throwable var3) {
      com.leanplum.internal.Log.e("Failed to set up MiPush", var3);
    }
  }

  public static void setPushDeliveryTracking(boolean value) {
    Leanplum.setPushDeliveryTracking(value);
  }

  public static void trackPurchase(String eventName, double value, String currencyCode,
      String jsonParameters) {
    Leanplum.trackPurchase(eventName, value, currencyCode,
        JsonConverter.fromJson(jsonParameters));
  }

  public static void trackGooglePlayPurchase(String item, long priceMicros,
                                             String currencyCode, String purchaseData, String dataSignature,
                                             String jsonParameters) {
    Map<String, Object> params = JsonConverter.fromJson(jsonParameters);
    Leanplum.trackGooglePlayPurchase(item, priceMicros, currencyCode,
            purchaseData, dataSignature, params);
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
    Map<String, Object> attributesMap = JsonConverter.fromJsonWithConvertedDateValues(jsonAttributes);
    Leanplum.setUserAttributes(userId, attributesMap);
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

  public static String securedVars() {
    return gson.toJson(Leanplum.securedVars());
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
    Leanplum.forceContentUpdate(new ForceContentUpdateCallback() {
      @Override
      public void onContentUpdated(boolean success) {
        String message = String.format("ForceContentUpdateWithCallback:%d:%d", key, success ? 1 : 0);
        makeCallbackToUnity(message);
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

    Leanplum.defineAction(
        name,
        kind,
        actionArgs,
        new ActionCallback() { // present handler
          @Override
          public boolean onResponse(ActionContext context) {
            sendMessageActionContext("ActionResponder", context);
            return true;
          }
        },
        new ActionCallback() { // dismiss handler
          @Override
          public boolean onResponse(ActionContext context) {
            sendMessageActionContext("ActionDismiss", context);
            return true;
          }
        }
    );
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
            actionId,
            actionId,
            Constants.Messaging.DEFAULT_PRIORITY);

    String key = getActionContextKey(context);
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
      ActionsTrigger trigger = new ActionsTrigger(
          "manual-trigger",
          Collections.singletonList("manual-trigger"),
          null);
      ActionManagerTriggeringKt.trigger(
          ActionManager.getInstance(),
          Collections.singletonList(context),
          Priority.DEFAULT,
          trigger);
      return true;
    }

    return false;
  }

  public static String getActionContextKey(ActionContext actionContext) {
    StringBuilder sb = new StringBuilder();
    sb.append(String.format("%s:%s", actionContext.actionName(), actionContext.getMessageId()));

    ActionContext parent = actionContext.getParentContext();
    while (parent != null) {
      sb.insert(0, String.format("%s:%s:", parent.actionName(), parent.getMessageId()));
      parent = parent.getParentContext();
    }
    return sb.toString();
  }

  private static void sendMessageActionContext(String message, ActionContext context){
    if (context != null) {
      String key = getActionContextKey(context);
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
      // Use the map directly instead of msg.getData()
      // gson.toJson of JSONObject produces "nameValuePairs"
      Map<String, ?> mapData =
              CollectionUtil.uncheckedCast(msg.getContext().objectNamed(Constants.Keys.DATA));
      message.put("data", mapData);
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

  public static void addOnceVariablesChangedAndNoDownloadsPendingHandler(final int key) {
    Leanplum.addOnceVariablesChangedAndNoDownloadsPendingHandler(new VariablesChangedCallback() {
      @Override
      public void variablesChanged() {
        String message = String.format("OnceVariablesChangedAndNoDownloadsPendingHandler:%d", key);
        makeCallbackToUnity(message);
      }
    });
  }

  public static void setJavaBridge(IJavaBridge bridge) {
    javaBridge = bridge;
    if (javaBridge != null) {
      messageController = new MessageDisplayControllerImpl(javaBridge);
      LeanplumActions.setMessageDisplayController(messageController);
      messageListener = new MessageDisplayListenerImpl(javaBridge);
      LeanplumActions.setMessageDisplayListener(messageListener);
    } else {
      Log.e("Leanplum", "IJavaBridge instance is null");
      javaBridge = null;
      messageController = null;
      messageListener = null;
    }
  }

  public static void triggerDelayedMessages() {
    LeanplumActions.triggerDelayedMessages();
  }

  public static void setActionManagerPaused(boolean paused) {
    LeanplumActions.setQueuePaused(paused);
  }

  public static void setActionManagerEnabled(boolean enabled) {
    LeanplumActions.setQueueEnabled(enabled);
  }

  public static void useWorkerThreadForDecisionHandlers(boolean useAsyncHandlers) {
    Log.i("Leanplum", "Using worker thread for decision handlers: " + useAsyncHandlers);
    LeanplumActions.setUseWorkerThreadForDecisionHandlers(useAsyncHandlers);
  }

  public static String getMigrationConfig() {
    Map<String, Object> config = new HashMap<>();
    String state = "0";
    switch (MigrationManager.getState()) {
      case Undefined: state = "0"; break;
      case LeanplumOnly: state = "1"; break;
      case CleverTapOnly: state = "3"; break;
      case Duplicate: state = "2"; break;
    }
    config.put("state", state);
    config.put("accountId", MigrationConfig.INSTANCE.getAccountId());
    config.put("accountToken", MigrationConfig.INSTANCE.getAccountToken());
    config.put("accountRegion", MigrationConfig.INSTANCE.getAccountRegion());
    config.put("attributeMappings", MigrationConfig.INSTANCE.getAttributeMap());
    config.put("identityKeys", MigrationConfig.INSTANCE.getIdentityList());
    return JsonConverter.toJson(config);
  }
}
