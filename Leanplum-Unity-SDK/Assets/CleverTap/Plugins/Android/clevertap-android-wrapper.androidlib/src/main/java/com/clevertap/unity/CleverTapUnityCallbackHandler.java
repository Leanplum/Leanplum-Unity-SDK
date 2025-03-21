package com.clevertap.unity;

import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_DEEP_LINK_CALLBACK;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_DISPLAY_UNITS_UPDATED;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_FEATURE_FLAG_UPDATED;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_FILE_VARIABLE_READY;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_INAPPS_FETCHED;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_INAPP_NOTIFICATION_DISMISSED_CALLBACK;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_INAPP_NOTIFICATION_SHOW_CALLBACK;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_INBOX_DID_INITIALIZE;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_INBOX_MESSAGES_DID_UPDATE;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_INIT_CLEVERTAP_ID_CALLBACK;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_ON_INAPP_BUTTON_CLICKED;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_ON_INBOX_BUTTON_CLICKED;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_ON_INBOX_ITEM_CLICKED;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_ON_PUSH_PERMISSION_RESPONSE_CALLBACK;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_PRODUCT_CONFIG_ACTIVATED;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_PRODUCT_CONFIG_FETCHED;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_PRODUCT_CONFIG_INITIALIZED;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_PROFILE_INITIALIZED_CALLBACK;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_PROFILE_UPDATES_CALLBACK;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_PUSH_OPENED_CALLBACK;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_VARIABLES_CHANGED;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_VARIABLES_CHANGED_AND_NO_DOWNLOADS_PENDING;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_VARIABLES_FETCHED;
import static com.clevertap.unity.CleverTapUnityCallback.CLEVERTAP_VARIABLE_VALUE_CHANGED;
import static com.clevertap.unity.CleverTapUnityPlugin.LOG_TAG;

import android.annotation.SuppressLint;
import android.net.Uri;
import android.util.Log;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.clevertap.android.sdk.CTFeatureFlagsListener;
import com.clevertap.android.sdk.CTInboxListener;
import com.clevertap.android.sdk.CleverTapAPI;
import com.clevertap.android.sdk.InAppNotificationButtonListener;
import com.clevertap.android.sdk.InAppNotificationListener;
import com.clevertap.android.sdk.InboxMessageButtonListener;
import com.clevertap.android.sdk.InboxMessageListener;
import com.clevertap.android.sdk.PushPermissionResponseListener;
import com.clevertap.android.sdk.SyncListener;
import com.clevertap.android.sdk.displayunits.DisplayUnitListener;
import com.clevertap.android.sdk.displayunits.model.CleverTapDisplayUnit;
import com.clevertap.android.sdk.inapp.CTInAppNotification;
import com.clevertap.android.sdk.inapp.callbacks.FetchInAppsCallback;
import com.clevertap.android.sdk.inbox.CTInboxMessage;
import com.clevertap.android.sdk.interfaces.OnInitCleverTapIDListener;
import com.clevertap.android.sdk.product_config.CTProductConfigListener;
import com.clevertap.android.sdk.pushnotification.CTPushNotificationListener;
import com.clevertap.android.sdk.variables.Var;
import com.clevertap.android.sdk.variables.callbacks.FetchVariablesCallback;
import com.clevertap.android.sdk.variables.callbacks.VariableCallback;
import com.clevertap.android.sdk.variables.callbacks.VariablesChangedCallback;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

class CleverTapUnityCallbackHandler implements SyncListener, InAppNotificationListener,
        CTInboxListener, InAppNotificationButtonListener, InboxMessageButtonListener,
        DisplayUnitListener, CTFeatureFlagsListener, CTProductConfigListener,
        OnInitCleverTapIDListener, InboxMessageListener, PushPermissionResponseListener,
        CTPushNotificationListener {

    private static CleverTapUnityCallbackHandler instance = null;

    public synchronized static CleverTapUnityCallbackHandler getInstance() {
        if (instance == null) {
            instance = new CleverTapUnityCallbackHandler();
        }
        return instance;
    }

    static void handleDeepLink(Uri data) {
        final String json = data.toString();
        sendToUnity(CLEVERTAP_DEEP_LINK_CALLBACK, json);
    }

    private static void sendToUnity(@NonNull CleverTapUnityCallback callback, @NonNull String data) {
        CleverTapMessageSender.getInstance().send(callback, data);
    }

    private final Map<Integer, UnityVariablesChangedCallback> variableChangedCallbacks = new HashMap<>();

    public void attachToApiInstance(CleverTapAPI clevertap) {
        clevertap.unregisterPushPermissionNotificationResponseListener(this);
        clevertap.registerPushPermissionNotificationResponseListener(this);
        clevertap.setCTPushNotificationListener(this);
        clevertap.setInAppNotificationListener(this);
        clevertap.setSyncListener(this);
        clevertap.setCTNotificationInboxListener(this);
        clevertap.setInboxMessageButtonListener(this);
        clevertap.setCTInboxMessageListener(this);
        clevertap.setInAppNotificationButtonListener(this);
        clevertap.setDisplayUnitListener(this);
        clevertap.setCTFeatureFlagsListener(this);
        clevertap.setCTProductConfigListener(this);
    }

    public void detachFromApiInstance(CleverTapAPI clevertap) {
        clevertap.unregisterPushPermissionNotificationResponseListener(this);
        clevertap.setCTPushNotificationListener(null);
        clevertap.setInAppNotificationListener(null);
        clevertap.setSyncListener(null);
        clevertap.setCTNotificationInboxListener(null);
        clevertap.setInboxMessageButtonListener(null);
        clevertap.setCTInboxMessageListener(null);
        clevertap.setInAppNotificationButtonListener(null);
        clevertap.setDisplayUnitListener(null);
        clevertap.setCTFeatureFlagsListener(null);
        clevertap.setCTProductConfigListener(null);
        clevertap.removeAllVariablesChangedCallbacks();
    }

    //OnInitCleverTapIDListener
    @Override
    public void onInitCleverTapID(String cleverTapID) {
        try {
            final String json = new JSONObject().put("CleverTapID", cleverTapID).toString();
            sendToUnity(CLEVERTAP_INIT_CLEVERTAP_ID_CALLBACK, json);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "onInitCleverTapID error", t);
        }
    }

    //Push primer listener
    @Override
    public void onPushPermissionResponse(boolean accepted) {
        sendToUnity(CLEVERTAP_ON_PUSH_PERMISSION_RESPONSE_CALLBACK, String.valueOf(accepted));
    }

    // CTPushNotificationListener
    @Override
    public void onNotificationClickedPayloadReceived(HashMap<String, Object> payload) {
        try {
            String payloadJson = new JSONObject(payload).toString();
            sendToUnity(CLEVERTAP_PUSH_OPENED_CALLBACK, payloadJson);
        } catch (Exception e) {
            Log.e(LOG_TAG, "onNotificationClickedPayloadReceived error", e);
        }
    }

    // InAppNotificationListener
    public boolean beforeShow(Map<String, Object> var1) {
        return true;
    }

    @SuppressLint("RestrictedApi")
    @Override
    public void onShow(CTInAppNotification ctInAppNotification) {
        if (ctInAppNotification != null && ctInAppNotification.getJsonDescription() != null) {
            sendToUnity(CLEVERTAP_INAPP_NOTIFICATION_SHOW_CALLBACK,
                    ctInAppNotification.getJsonDescription().toString());
        } else {
            Log.e(LOG_TAG, "Could not trigger onShow for InApp with null json description");
        }
    }

    @Override
    public void onDismissed(Map<String, Object> extras, @Nullable Map<String, Object> actionExtras) {
        try {
            JSONObject json = new JSONObject();
            if (extras != null) {
                json.put("extras", new JSONObject(extras));
            }
            if (actionExtras != null) {
                json.put("actionExtras", actionExtras);
            }

            sendToUnity(CLEVERTAP_INAPP_NOTIFICATION_DISMISSED_CALLBACK, json.toString());
        } catch (JSONException e) {
            Log.e(LOG_TAG, "Could not convert in app extras to json ", e);
        }
    }

    // SyncListener
    @Override
    public void profileDataUpdated(JSONObject updates) {

        if (updates == null) {
            return;
        }

        sendToUnity(CLEVERTAP_PROFILE_UPDATES_CALLBACK, updates.toString());
    }

    @Override
    public void profileDidInitialize(String CleverTapID) {

        if (CleverTapID == null) {
            return;
        }

        try {
            final String json = new JSONObject().put("CleverTapID", CleverTapID).toString();
            sendToUnity(CLEVERTAP_PROFILE_INITIALIZED_CALLBACK, json);
        } catch (JSONException e) {
            Log.e(LOG_TAG, "profileDidInitialize json error", e);
        }
    }

    //Inbox Listeners
    @Override
    public void inboxDidInitialize() {
        final String message = "CleverTap App Inbox Initialized";
        sendToUnity(CLEVERTAP_INBOX_DID_INITIALIZE, message);
    }

    @Override
    public void inboxMessagesDidUpdate() {
        final String message = "CleverTap App Inbox Messages Updated";
        sendToUnity(CLEVERTAP_INBOX_MESSAGES_DID_UPDATE, message);
    }

    //Inbox Button Click Listener
    @Override
    public void onInboxButtonClick(HashMap<String, String> payload) {
        try {
            JSONObject json = new JSONObject();
            if (payload != null) {
                json.put("customExtras", new JSONObject(payload));
            }
            sendToUnity(CLEVERTAP_ON_INBOX_BUTTON_CLICKED, json.toString());
        } catch (JSONException e) {
            Log.e(LOG_TAG, "Could not convert inbox extras to json ", e);
        }
    }

    @Override
    public void onInboxItemClicked(CTInboxMessage message, int contentPageIndex, int buttonIndex) {
        if (message != null && message.getData() != null) {
            JSONObject jsonObject = new JSONObject();
            try {
                jsonObject.put("ContentPageIndex", contentPageIndex);
                jsonObject.put("ButtonIndex", buttonIndex);
                jsonObject.put("CTInboxMessagePayload", message.getData());
                sendToUnity(CLEVERTAP_ON_INBOX_ITEM_CLICKED, jsonObject.toString());
            } catch (JSONException e) {
                throw new RuntimeException(e);
            }
        }
    }

    // Variables callbacks
    public FetchVariablesCallback getFetchVariablesCallback(int callbackId) {
        return isSuccess -> {
            JSONObject json = new JSONObject();
            try {
                json.put("callbackId", callbackId);
                json.put("isSuccess", isSuccess);
            } catch (JSONException e) {
                throw new RuntimeException(e);
            }

            sendToUnity(CLEVERTAP_VARIABLES_FETCHED, json.toString());
        };
    }

    public <T> VariableCallback<T> getVariableCallback() {
        return new VariableCallback<T>() {
            @Override
            public void onValueChanged(Var variable) {
                sendToUnity(CLEVERTAP_VARIABLE_VALUE_CHANGED, variable.name());
            }
        };
    }

    public VariableCallback<String> getFileVariableCallback() {
        return new VariableCallback<String>() {
            @Override
            public void onValueChanged(Var<String> variable) {
                sendToUnity(CLEVERTAP_FILE_VARIABLE_READY, variable.name());
            }
        };
    }

    public UnityVariablesChangedCallback createVariablesChangedCallback(
            CleverTapUnityCallback callbackType,
            int callbackId) {
        UnityVariablesChangedCallback callback = variableChangedCallbacks.get(callbackId);
        if (callback == null) {
            callback = new UnityVariablesChangedCallback(callbackId, callbackType);
            variableChangedCallbacks.put(callbackId, callback);
        }
        return callback;
    }

    public UnityVariablesChangedCallback getVariablesChangedCallback(int callbackId) {
        return variableChangedCallbacks.get(callbackId);
    }

    //FetchInAppsCallback
    public FetchInAppsCallback getFetchInAppsCallback(int callbackId) {
        return isSuccess -> {
            JSONObject json = new JSONObject();
            try {
                json.put("callbackId", callbackId);
                json.put("isSuccess", isSuccess);
            } catch (JSONException e) {
                throw new RuntimeException(e);
            }

            sendToUnity(CLEVERTAP_INAPPS_FETCHED, json.toString());
        };
    }

    @Override
    public void onInAppButtonClick(HashMap<String, String> payload) {
        try {
            JSONObject json = new JSONObject();
            if (payload != null) {
                json.put("customExtras", new JSONObject(payload));
            }
            sendToUnity(CLEVERTAP_ON_INAPP_BUTTON_CLICKED, json.toString());
        } catch (JSONException e) {
            Log.e(LOG_TAG, "Could not convert in app button extras to json ", e);
        }
    }

    //Native Display Listener
    @Override
    public void onDisplayUnitsLoaded(ArrayList<CleverTapDisplayUnit> units) {
        try {
            JSONArray jsonArray = JsonConverter.displayUnitListToJSONArray(units);
            JSONObject json = new JSONObject().put("displayUnits", jsonArray);
            sendToUnity(CLEVERTAP_DISPLAY_UNITS_UPDATED, json.toString());
        } catch (JSONException e) {
            Log.e(LOG_TAG, "Could not convert display units to json ", e);
        }
    }

    //Feature Flag Listener
    @Override
    @Deprecated
    public void featureFlagsUpdated() {
        final String message = "CleverTap App Feature Flags Updated";
        sendToUnity(CLEVERTAP_FEATURE_FLAG_UPDATED, message);
    }

    //Product Config Listener
    @Override
    @Deprecated
    public void onInit() {
        final String message = "CleverTap App Product Config Initialized";
        sendToUnity(CLEVERTAP_PRODUCT_CONFIG_INITIALIZED, message);
    }

    @Override
    @Deprecated
    public void onFetched() {
        final String message = "CleverTap App Product Config Fetched";
        sendToUnity(CLEVERTAP_PRODUCT_CONFIG_FETCHED, message);
    }

    @Override
    @Deprecated
    public void onActivated() {
        final String message = "CleverTap App Product Config Activated";
        sendToUnity(CLEVERTAP_PRODUCT_CONFIG_ACTIVATED, message);
    }

    static class UnityVariablesChangedCallback extends VariablesChangedCallback {
        public final int callbackId;
        public final CleverTapUnityCallback callbackType;

        UnityVariablesChangedCallback(int callbackId, CleverTapUnityCallback callbackType) {
            this.callbackId = callbackId;
            this.callbackType = callbackType;
        }

        @Override
        public void variablesChanged() {
            JSONObject json = new JSONObject();
            try {
                json.put("callbackId", callbackId);
            } catch (JSONException e) {
                throw new RuntimeException(e);
            }
            sendToUnity(callbackType, json.toString());
        }
    }
}
