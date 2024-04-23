package com.clevertap.unity;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.location.Location;
import android.net.Uri;
import android.os.Build;
import android.os.Build.VERSION_CODES;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import androidx.annotation.Nullable;
import androidx.annotation.RequiresApi;
import com.clevertap.android.sdk.ActivityLifecycleCallback;
import com.clevertap.android.sdk.CTFeatureFlagsListener;
import com.clevertap.android.sdk.CTInboxListener;
import com.clevertap.android.sdk.CTInboxStyleConfig;
import com.clevertap.android.sdk.CleverTapAPI;
import com.clevertap.android.sdk.InAppNotificationButtonListener;
import com.clevertap.android.sdk.InAppNotificationListener;
import com.clevertap.android.sdk.InboxMessageButtonListener;
import com.clevertap.android.sdk.InboxMessageListener;
import com.clevertap.android.sdk.PushPermissionResponseListener;
import com.clevertap.android.sdk.SyncListener;
import com.clevertap.android.sdk.UTMDetail;
import com.clevertap.android.sdk.displayunits.DisplayUnitListener;
import com.clevertap.android.sdk.displayunits.model.CleverTapDisplayUnit;
import com.clevertap.android.sdk.events.EventDetail;
import com.clevertap.android.sdk.inapp.CTInAppNotification;
import com.clevertap.android.sdk.inapp.CTLocalInApp;
import com.clevertap.android.sdk.inbox.CTInboxMessage;
import com.clevertap.android.sdk.interfaces.OnInitCleverTapIDListener;
import com.clevertap.android.sdk.product_config.CTProductConfigListener;
import com.clevertap.android.sdk.pushnotification.PushConstants;
import com.clevertap.android.sdk.variables.Var;
import com.clevertap.android.sdk.variables.callbacks.VariableCallback;
import com.clevertap.android.sdk.variables.callbacks.VariablesChangedCallback;
import com.clevertap.android.sdk.variables.callbacks.FetchVariablesCallback;
import com.clevertap.android.sdk.inapp.callbacks.FetchInAppsCallback;
import com.unity3d.player.UnityPlayer;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class CleverTapUnityPlugin implements SyncListener, InAppNotificationListener,
        CTInboxListener, InAppNotificationButtonListener,
        InboxMessageButtonListener, DisplayUnitListener, CTFeatureFlagsListener, CTProductConfigListener,
        OnInitCleverTapIDListener, InboxMessageListener, PushPermissionResponseListener {

    private static final String LOG_TAG = "CleverTapUnityPlugin";

    private static final String CLEVERTAP_GAME_OBJECT_NAME = "AndroidCallbackHandler";

    private static final String CLEVERTAP_PROFILE_INITIALIZED_CALLBACK = "CleverTapProfileInitializedCallback";

    private static final String CLEVERTAP_PROFILE_UPDATES_CALLBACK = "CleverTapProfileUpdatesCallback";

    private static final String CLEVERTAP_DEEP_LINK_CALLBACK = "CleverTapDeepLinkCallback";

    private static final String CLEVERTAP_PUSH_OPENED_CALLBACK = "CleverTapPushOpenedCallback";

    private static final String CLEVERTAP_INAPP_NOTIFICATION_DISMISSED_CALLBACK
            = "CleverTapInAppNotificationDismissedCallback";

    private static final String CLEVERTAP_INAPP_NOTIFICATION_SHOW_CALLBACK
            = "CleverTapInAppNotificationShowCallback";

    private static final String CLEVERTAP_ON_PUSH_PERMISSION_RESPONSE_CALLBACK
            = "CleverTapOnPushPermissionResponseCallback";

    private static final String CLEVERTAP_INBOX_DID_INITIALIZE = "CleverTapInboxDidInitializeCallback";

    private static final String CLEVERTAP_INBOX_MESSAGES_DID_UPDATE = "CleverTapInboxMessagesDidUpdateCallback";

    private static final String CLEVERTAP_ON_INBOX_BUTTON_CLICKED = "CleverTapInboxCustomExtrasButtonSelect";

    private static final String CLEVERTAP_ON_INBOX_ITEM_CLICKED = "CleverTapInboxItemSelect";

    private static final String CLEVERTAP_ON_INAPP_BUTTON_CLICKED = "CleverTapInAppNotificationButtonTapped";

    private static final String CLEVERTAP_DISPLAY_UNITS_UPDATED = "CleverTapNativeDisplayUnitsUpdated";

    private static final String CLEVERTAP_FEATURE_FLAG_UPDATED = "CleverTapFeatureFlagsUpdated";

    private static final String CLEVERTAP_PRODUCT_CONFIG_INITIALIZED = "CleverTapProductConfigInitialized";

    private static final String CLEVERTAP_PRODUCT_CONFIG_FETCHED = "CleverTapProductConfigFetched";

    private static final String CLEVERTAP_PRODUCT_CONFIG_ACTIVATED = "CleverTapProductConfigActivated";

    private static final String CLEVERTAP_INIT_CLEVERTAP_ID_CALLBACK = "CleverTapInitCleverTapIdCallback";

    private static final String CLEVERTAP_VARIABLES_CHANGED = "CleverTapVariablesChanged";

    private static final String CLEVERTAP_VARIABLE_VALUE_CHANGED = "CleverTapVariableValueChanged";

    private static final String CLEVERTAP_VARIABLES_FETCHED = "CleverTapVariablesFetched";
	
	private static final String CLEVERTAP_INAPPS_FETCHED = "CleverTapInAppsFetched";

    private static CleverTapUnityPlugin instance = null;

    private CleverTapAPI clevertap = null;

    private static void changeCredentials(final String accountID, final String accountToken, final String region) {
        CleverTapAPI.changeCredentials(accountID, accountToken, region);
    }

    static void handleIntent(Intent intent, Activity activity) {
        if (intent == null) {
            return;
        }
        if (intent.getAction() == null) {
            return;
        }

        if (intent.getAction().equals(Intent.ACTION_VIEW)) {
            Uri data = intent.getData();
            if (data != null) {
                handleDeepLink(data);
            }
        } else {
            Bundle extras = intent.getExtras();
            boolean isPushNotification = (extras != null && extras.get("wzrk_pn") != null);
            if (isPushNotification) {

                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
                    CleverTapAPI.getDefaultInstance(activity).pushNotificationClickedEvent(extras);
                }

                JSONObject data = new JSONObject();

                for (String key : extras.keySet()) {
                    try {
                        Object value = extras.get(key);
                        if (value instanceof Map) {
                            JSONObject jsonObject = new JSONObject((Map) value);
                            data.put(key, jsonObject);
                        } else if (value instanceof List) {
                            JSONArray jsonArray = new JSONArray((List) value);
                            data.put(key, jsonArray);
                        } else {
                            data.put(key, extras.get(key));
                        }

                    } catch (JSONException e) {
                        // no-op
                    }
                }
                handlePushNotification(data);
            }
        }
    }

    static private void handlePushNotification(final JSONObject data) {
        final String json = data.toString();
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_PUSH_OPENED_CALLBACK, json);
    }

    static private void handleDeepLink(final Uri data) {
        final String json = data.toString();
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_DEEP_LINK_CALLBACK, json);
    }

    public static void initialize(final String accountID, final String accountToken, final Activity activity) {
        initialize(accountID, accountToken, null, activity);
    }

    public static void initialize(final String accountID, final String accountToken, final String region,
                                  final Activity activity) {
        try {
            changeCredentials(accountID, accountToken, region);
            ActivityLifecycleCallback.register(activity.getApplication());
            CleverTapAPI.setAppForeground(true);
            getInstance(activity.getApplicationContext());
            CleverTapAPI.onActivityResumed(activity);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "initialize error", t);
        }
    }

    public static void setDebugLevel(int level) {
        CleverTapAPI.setDebugLevel(level);
    }

    public static synchronized CleverTapUnityPlugin getInstance(final Context context) {
        if (instance == null && context != null) {
            instance = new CleverTapUnityPlugin(context.getApplicationContext());
        }
        return instance;
    }

    private CleverTapUnityPlugin(final Context context) {
        try {
            clevertap = CleverTapAPI.getDefaultInstance(context);
            if (clevertap != null) {
                Log.d(LOG_TAG, "getDefaultInstance-" + clevertap);
                clevertap.registerPushPermissionNotificationResponseListener(this);
                clevertap.setInAppNotificationListener(this);
                clevertap.setSyncListener(this);
                clevertap.setCTNotificationInboxListener(this);
                clevertap.setInboxMessageButtonListener(this);
                clevertap.setCTInboxMessageListener(this);
                clevertap.setInAppNotificationButtonListener(this);
                clevertap.setDisplayUnitListener(this);
                clevertap.setCTFeatureFlagsListener(this);
                clevertap.setCTProductConfigListener(this);
                clevertap.setLibrary("Unity");
                clevertap.addVariablesChangedCallback(new VariablesChangedCallback() {
                    @Override
                    public void variablesChanged() {
                        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_VARIABLES_CHANGED, "{ Variables Changed Callback }");
                    }
                });
            }
        } catch (Throwable t) {
            Log.e(LOG_TAG, "initialization error", t);
        }
    }

    public static void createNotificationChannel(Context context, String channelId, String channelName,
            String channelDescription, int importance, boolean showBadge) {
        try {
            CleverTapAPI.createNotificationChannel(context, channelId, channelName, channelDescription, importance,
                    showBadge);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "Error creating Notification Channel", t);
        }
    }

    @RequiresApi(api = VERSION_CODES.O)
    public static void createNotificationChannelWithSound(Context context, String channelId, String channelName,
            String channelDescription, int importance, boolean showBadge, String sound) {
        try {
            CleverTapAPI.createNotificationChannel(context, channelId, channelName, channelDescription, importance,
                    showBadge, sound);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "Error creating Notification Channel", t);
        }
    }

    @RequiresApi(api = VERSION_CODES.O)
    public static void createNotificationChannelWithGroup(Context context, String channelId, String channelName,
            String channelDescription, int importance, String groupId, boolean showBadge) {
        try {
            CleverTapAPI.createNotificationChannel(context, channelId, channelName, channelDescription, importance,
                    groupId, showBadge);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "Error creating Notification Channel with groupId", t);
        }
    }

    @RequiresApi(api = VERSION_CODES.O)
    public static void createNotificationChannelWithGroupAndSound(Context context, String channelId,
            String channelName, String channelDescription, int importance, String groupId, boolean showBadge,
            String sound) {
        try {
            CleverTapAPI.createNotificationChannel(context, channelId, channelName, channelDescription, importance,
                    groupId, showBadge, sound);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "Error creating Notification Channel with groupId", t);
        }
    }

    @RequiresApi(api = VERSION_CODES.O)
    public static void createNotificationChannelGroup(Context context, String groupId, String groupName) {
        try {
            CleverTapAPI.createNotificationChannelGroup(context, groupId, groupName);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "Error creating Notification Channel Group", t);
        }
    }

    @RequiresApi(api = VERSION_CODES.O)
    public static void deleteNotificationChannel(Context context, String channelId) {
        try {
            CleverTapAPI.deleteNotificationChannel(context, channelId);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "Error deleting Notification Channel", t);
        }
    }

    @RequiresApi(api = VERSION_CODES.O)
    public static void deleteNotificationChannelGroup(Context context, String groupId) {
        try {
            CleverTapAPI.deleteNotificationChannelGroup(context, groupId);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "Error deleting Notification Channel Group", t);
        }
    }

    public void setPushToken(String token, String region, String type) {
        if (PushConstants.PushType.valueOf(type.toLowerCase()).equals(PushConstants.PushType.FCM)) {
            clevertap.pushFcmRegistrationId(token, true);
        } else if (PushConstants.PushType.valueOf(type.toLowerCase()).equals(PushConstants.PushType.XPS)) {
            clevertap.pushXiaomiRegistrationId(token, region,true);
        } else if (PushConstants.PushType.valueOf(type.toLowerCase()).equals(PushConstants.PushType.BPS)) {
            clevertap.pushBaiduRegistrationId(token, true);
        } else if (PushConstants.PushType.valueOf(type.toLowerCase()).equals(PushConstants.PushType.HPS)) {
            clevertap.pushHuaweiRegistrationId(token, true);
        }
    }

    public void setOptOut(boolean value) {
        try {
            clevertap.setOptOut(value);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "setOptOut error", t);
        }
    }

    public void enableDeviceNetworkInfoReporting(boolean value) {
        try {
            clevertap.enableDeviceNetworkInfoReporting(value);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "enableDeviceNetworkInfoReporting error", t);
        }
    }

    public void enablePersonalization() {
        try {
            clevertap.enablePersonalization();
        } catch (Throwable t) {
            Log.e(LOG_TAG, "enablePersonalization error", t);
        }
    }

    public void disablePersonalization() {
        try {
            clevertap.disablePersonalization();
        } catch (Throwable t) {
            Log.e(LOG_TAG, "disablePersonalization error", t);
        }
    }

    public void setLocation(final double lat, final double lon) {
        try {
            final Location location = new Location("CleverTapUnityPlugin");
            location.setLatitude(lat);
            location.setLongitude(lon);
            clevertap.setLocation(location);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "setLocation error", t);
        }
    }

    public void onUserLogin(final String jsonString) {
        try {
            Map<String, Object> profile = JsonConverter.fromJsonWithConvertedDateValues(jsonString);
            clevertap.onUserLogin(profile);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "onUserLogin error", t);
        }
    }

    public void profilePush(final String jsonString) {
        try {
            Map<String, Object> profile = JsonConverter.fromJsonWithConvertedDateValues(jsonString);
            clevertap.pushProfile(profile);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "profilePush error", t);
        }
    }
/*
    public void profilePushFacebookUser(final String jsonString) {
        try {
            clevertap.pushFacebookUser(new JSONObject(jsonString));
        } catch (Throwable t) {
            Log.e(LOG_TAG, "profilePushFacebookUser error", t);
        }
    }*/

    public String profileGet(final String key) {
        try {
            String val = null;
            Object _val = clevertap.getProperty(key);
            if (_val != null) {
                val = _val.toString();
            }
            return val;

        } catch (Throwable t) {
            Log.e(LOG_TAG, "profileGet error", t);
            return null;
        }
    }

    public String profileGetCleverTapAttributionIdentifier() {
        try {
            return clevertap.getCleverTapAttributionIdentifier();
        } catch (Throwable t) {
            Log.e(LOG_TAG, "profileGetCleverTapAttributionIdentifier error", t);
            return null;
        }
    }

    public String profileGetCleverTapID() {
        try {
            return clevertap.getCleverTapID();
        } catch (Throwable t) {
            Log.e(LOG_TAG, "profileGetCleverTapID error", t);
            return null;
        }
    }

    public void profileRemoveValueForKey(final String key) {
        clevertap.removeValueForKey(key);
    }

    public void profileAddMultiValueForKey(final String key, final String val) {
        clevertap.addMultiValueForKey(key, val);
    }

    public void profileRemoveMultiValueForKey(final String key, final String val) {
        clevertap.removeMultiValueForKey(key, val);
    }

    public void profileSetMultiValuesForKey(final String key, final String[] values) {
        try {
            clevertap.setMultiValuesForKey(key, new ArrayList<String>(Arrays.asList(values)));
        } catch (Throwable t) {
            Log.e(LOG_TAG, "profileSetMultiValuesForKey error", t);
        }
    }

    public void profileAddMultiValuesForKey(final String key, final String[] values) {
        try {
            clevertap.addMultiValuesForKey(key, new ArrayList<String>(Arrays.asList(values)));
        } catch (Throwable t) {
            Log.e(LOG_TAG, "profileAddMultiValuesForKey error", t);
        }
    }

    public void profileRemoveMultiValuesForKey(final String key, final String[] values) {
        try {
            clevertap.removeMultiValuesForKey(key, new ArrayList<String>(Arrays.asList(values)));
        } catch (Throwable t) {
            Log.e(LOG_TAG, "profileRemoveMultiValuesForKey error", t);
        }
    }

    @Override
    public void onInitCleverTapID(String cleverTapID){
        final String json = "{cleverTapID:" + cleverTapID + "}";
        try {
             messageUnity(CLEVERTAP_GAME_OBJECT_NAME,CLEVERTAP_INIT_CLEVERTAP_ID_CALLBACK,json);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "onInitCleverTapID error", t);
        }
    }

    public void getCleverTapID(){
        try {
            clevertap.getCleverTapID(this);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "getCleverTapID error", t);
        }
    }


    public void profileIncrementValueForKey(final String key, final double value){
        try {
            clevertap.incrementValue(key, value);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "profileIncrementForKey(double) error", t);
        }
    }

    public void profileIncrementValueForKey(final String key, final int value){
        try {
            clevertap.incrementValue(key, value);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "profileIncrementValueForKey(int) error", t);
        }
    }

    public void profileDecrementValueForKey(final String key,final double value) {
        try {
            clevertap.decrementValue(key, value);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "profileDecrementValueForKey(double) error", t);
        }
    }

    public void profileDecrementValueForKey(final String key,final int value) {
        try {
            clevertap.decrementValue(key, value);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "profileDecrementValueForKey(int) error", t);
        }
    }

    public void suspendInAppNotifications(){
        try {
            clevertap.suspendInAppNotifications();
        } catch (Throwable t) {
            Log.e(LOG_TAG, "Unable to suspendInAppNotification", t);
        }
    }


    public void discardInAppNotifications(){
        try {
            clevertap.discardInAppNotifications();
        } catch (Throwable t) {
            Log.e(LOG_TAG, "Unable to discardInAppNotification", t);
        }
    }


    public void resumeInAppNotifications(){
        try {
            clevertap.resumeInAppNotifications();
        } catch (Throwable t) {
            Log.e(LOG_TAG, "Unable to resumeInAppNotification", t);
        }
    }

    public void recordScreenView(final String screenName) {
        try {
            clevertap.recordScreen(screenName);
        } catch (Throwable t) {
            Log.e(LOG_TAG, "recordScreenView error", t);
        }
    }

    public void recordEvent(final String eventName, final String propertiesJsonString) {

        if (eventName == null) {
            return;
        }

        if (propertiesJsonString == null) {
            clevertap.pushEvent(eventName);
        } else {
            try {
                Map<String, Object> props = JsonConverter.fromJsonWithConvertedDateValues(propertiesJsonString);
                clevertap.pushEvent(eventName, props);
            } catch (Throwable t) {
                Log.e(LOG_TAG, "recordEvent error", t);
            }
        }
    }

    public void recordChargedEventWithDetailsAndItems(final String detailsJSON, final String itemsJSON) {

        try {
            HashMap<String, Object> details = new HashMap<String, Object>(JsonConverter.fromJsonWithConvertedDateValues(detailsJSON));
            JSONArray items = new JSONArray(itemsJSON);
            clevertap.pushChargedEvent(details, toArrayListOfStringObjectMaps(items));
        } catch (Throwable t) {
            Log.e(LOG_TAG, "recordChargedEventWithDetailsAndItems error", t);
        }
    }

    public int eventGetFirstTime(final String event) {
        return clevertap.getFirstTime(event);
    }

    public int eventGetLastTime(final String event) {
        return clevertap.getLastTime(event);
    }

    public int eventGetOccurrences(final String event) {
        return clevertap.getCount(event);
    }

    public String eventGetDetail(final String event) {
        try {
            EventDetail details = clevertap.getDetails(event);
            return eventDetailsToJSON(details).toString();
        } catch (Throwable t) {
            Log.e(LOG_TAG, "eventGetDetail error", t);
            return null;
        }
    }

    public String userGetEventHistory() {
        try {
            Map<String, EventDetail> history = clevertap.getHistory();
            return eventHistoryToJSON(history).toString();
        } catch (Throwable t) {
            Log.e(LOG_TAG, "userGetEventHistory error", t);
            return null;
        }
    }

    public String sessionGetUTMDetails() {
        try {
            UTMDetail details = clevertap.getUTMDetails();
            return utmDetailsToJSON(details).toString();
        } catch (Throwable t) {
            Log.e(LOG_TAG, "sessionGetUTMDetails error", t);
            return null;
        }
    }

    public int sessionGetTimeElapsed() {
        return clevertap.getTimeElapsed();
    }

    public int userGetTotalVisits() {
        return clevertap.getTotalVisits();
    }

    public int userGetScreenCount() {
        return clevertap.getScreenCount();
    }

    public int userGetPreviousVisitTime() {
        return clevertap.getPreviousVisitTime();
    }

    //Notification Inbox
    public void initializeInbox() {
        clevertap.initializeInbox();
    }

    public int getInboxMessageCount() {
        return clevertap.getInboxMessageCount();
    }

    public int getInboxMessageUnreadCount() {
        return clevertap.getInboxMessageCount();
    }

    public void showAppInbox(final String jsonString) {
        try {
            if (!TextUtils.isEmpty(jsonString)) {
                CTInboxStyleConfig styleConfig = toStyleConfig(new JSONObject(jsonString));
                clevertap.showAppInbox(styleConfig);
            } else {
                clevertap.showAppInbox();
            }
        } catch (JSONException e) {
            Log.e(LOG_TAG, "JSON Exception in converting style config", e);
        }
    }

    public void dismissAppInbox() {
        try {
            clevertap.dismissAppInbox();
        } catch (Throwable t) {
            Log.e(LOG_TAG, "Unable to dismissAppInbox", t);
        }
    }

    public String getAllInboxMessages() {
        try {
            return inboxMessageListToJSONArray(clevertap.getAllInboxMessages()).toString();
        } catch (JSONException e) {
            e.printStackTrace();
            return null;
        }
    }

    public String getUnreadInboxMessages() {
        try {
            return inboxMessageListToJSONArray(clevertap.getUnreadInboxMessages()).toString();
        } catch (JSONException e) {
            e.printStackTrace();
            return null;
        }
    }

    public String getInboxMessageForId(String messageId) {
        return clevertap.getInboxMessageForId(messageId).getData().toString();
    }

    public void deleteInboxMessageForId(String messageId) {
        clevertap.deleteInboxMessage(messageId);
    }

    public void deleteInboxMessagesForIDs(String[] messageIds) {
        List<String> messageIdsList = Arrays.asList(messageIds);
        ArrayList<String> messageIdsArrayList = new ArrayList<>(messageIdsList);
        clevertap.deleteInboxMessagesForIDs(messageIdsArrayList);
    }

    public void markReadInboxMessageForId(String messageId) {
        clevertap.markReadInboxMessage(messageId);
    }

    public void markReadInboxMessagesForIDs(String[] messageIds) {
        List<String> messageIdsList = Arrays.asList(messageIds);
        ArrayList<String> messageIdsArrayList = new ArrayList<>(messageIdsList);
        clevertap.markReadInboxMessagesForIDs(messageIdsArrayList);
    }

    public void pushInboxNotificationViewedEventForId(String messageId) {
        clevertap.pushInboxNotificationViewedEvent(messageId);
    }

    public void pushInboxNotificationClickedEventForId(String messageId) {
        clevertap.pushInboxNotificationClickedEvent(messageId);
    }

    public void setLibrary(String library) {
        try {
            clevertap.setLibrary(library);
        } catch (Exception e) {
            Log.e(LOG_TAG, "setLibrary error", e);
        }
    }

    public void pushInstallReferrer(String source, String medium, String campaign) {
        try {
            clevertap.pushInstallReferrer(source, medium, campaign);
        } catch (Exception e) {
            Log.e(LOG_TAG, "pushInstallReferrer error", e);
        }
    }

    // Variables

    public void defineVar(String name, String kind, String jsonValue) {
        Var<?> variable = null;
        if (kind.equals("integer")) {
            Long value = Long.valueOf(jsonValue);
            variable = clevertap.defineVariable(name, value);
        } else if (kind.equals("float")) {
            Double value = Double.valueOf(jsonValue);
            variable = clevertap.defineVariable(name, value);
        } else if (kind.equals("string")) {
            String value = jsonValue.substring(1, jsonValue.length() - 1);
            variable = clevertap.defineVariable(name, value);
        } else if (kind.equals("bool")) {
            Boolean value = Boolean.valueOf(jsonValue);
            variable = clevertap.defineVariable(name, value);
        } else if (kind.equals("group")) {
            try {
                JSONObject jsonObj = new JSONObject(jsonValue);
                Map<String, Object> value = toMap(jsonObj);
                variable = clevertap.defineVariable(name, value);
            } catch (Throwable t) {
                Log.e(LOG_TAG, "defineVar error", t);
            }
        }

        if (variable != null) {
            variable.addValueChangedCallback(new VariableCallback() {
                @Override
                public void onValueChanged(Var variable) {
                    messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_VARIABLE_VALUE_CHANGED, variable.name());
                }
            });
        }
    }

    public String getVariableValue(String variableName) {
        Object value = clevertap.getVariableValue(variableName);
		if (value == null) {
			return null;
		}
		
		return (value instanceof Map) ? new JSONObject((Map<?, ?>) value).toString() : value.toString();
    }

    public void syncVariables() {
        clevertap.syncVariables();
    }

    public void fetchVariables(final int callbackId) {
        clevertap.fetchVariables(new FetchVariablesCallback() {
            @Override
            public void onVariablesFetched(boolean isSuccess) {
                JSONObject json = new JSONObject();
                try {
                    json.put("callbackId", callbackId);
                    json.put("isSuccess", isSuccess);
                } catch (JSONException e) {
                    throw new RuntimeException(e);
                }

                messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_VARIABLES_FETCHED, json.toString());
            }
        });
    }
	
	// InApps	
	public void fetchInApps(final int callbackId) {
		clevertap.fetchInApps(new FetchInAppsCallback() {
            @Override
            public void onInAppsFetched(boolean isSuccess) {
                JSONObject json = new JSONObject();
                try {
                    json.put("callbackId", callbackId);
                    json.put("isSuccess", isSuccess);
                } catch (JSONException e) {
                    throw new RuntimeException(e);
                }

                messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_INAPPS_FETCHED, json.toString());
            }
        });
	}

   /**
     * Deletes all images and gifs which are preloaded for inapps in cs mode
     *
     * @param expiredOnly to clear only assets which will not be needed further for inapps
     */
    public void clearInAppResources(boolean expiredOnly) {
		clevertap.clearInAppResources(expiredOnly);
	}

    //Native Display Units
    public String getAllDisplayUnits() {
        try {
            ArrayList<CleverTapDisplayUnit> arrayList = clevertap.getAllDisplayUnits();
            JSONArray jsonArray = new JSONArray();
            if (arrayList != null) {
                jsonArray = displayUnitListToJSONArray(arrayList);
            }
            return jsonArray.toString();
        } catch (JSONException e) {
            e.printStackTrace();
            return null;
        }
    }

    public String getDisplayUnitForId(String unitId) {
        CleverTapDisplayUnit displayUnit = clevertap.getDisplayUnitForId(unitId);
        JSONObject jsonObject = new JSONObject();
        if (displayUnit != null) {
            jsonObject = displayUnit.getJsonObject();
        }
        return jsonObject.toString();
    }

    public void pushDisplayUnitViewedEventForID(String unitId) {
        clevertap.pushDisplayUnitViewedEventForID(unitId);
    }

    public void pushDisplayUnitClickedEventForID(String unitId) {
        clevertap.pushDisplayUnitClickedEventForID(unitId);
    }

    //Feature Flags
    public Boolean isFeatureFlagInitialized() {
        return clevertap.featureFlag().isInitialized();
    }

    public void fetchFeatureFlags() {
        clevertap.featureFlag().fetchFeatureFlags();
    }

    public Boolean getFeatureFlag(String key, Boolean defaultValue) {
        return clevertap.featureFlag().get(key, defaultValue);
    }

    //Product Config
    public Boolean isProductConfigInitialized() {
        return clevertap.productConfig().isInitialized();
    }

    public void setMapDefaults(HashMap<String, Object> map) {
        clevertap.productConfig().setDefaults(map);
    }

    public void fetch() {
        clevertap.productConfig().fetch();
    }

    public void fetch(long minimumIntervalInSeconds) {
        clevertap.productConfig().fetch(minimumIntervalInSeconds);
    }

    public void fetchAndActivate() {
        clevertap.productConfig().fetchAndActivate();
    }

    public void activate() {
        clevertap.productConfig().activate();
    }

    public String getString(String key) {
        return clevertap.productConfig().getString(key);
    }

    public Boolean getBoolean(String key) {
        return clevertap.productConfig().getBoolean(key);
    }

    public long getLong(String key) {
        return clevertap.productConfig().getLong(key);
    }

    public Double getDouble(String key) {
        return clevertap.productConfig().getDouble(key);
    }

    public void productConfigReset() {
        clevertap.productConfig().reset();
    }

    public long getLastFetchTimeStampInMillis() {
        return clevertap.productConfig().getLastFetchTimeStampInMillis();
    }

    //Push Primer
    public boolean isPushPermissionGranted() {
        return clevertap.isPushPermissionGranted();
    }

    public void promptForPushPermission(boolean showFallbackSettings){
        clevertap.promptForPushPermission(showFallbackSettings);
    }

    public void promptPushPrimer(String jsonStr){
        try {
            Map<String, Object> localInAppMap = toMap(new JSONObject(jsonStr));
            JSONObject jsonObject = localInAppFromMap(localInAppMap);
            clevertap.promptPushPrimer(jsonObject);
        } catch (JSONException e) {
            Log.e(LOG_TAG, "JSON Exception in converting local InApp", e);
        }
    }

    //Push primer listener
    @Override
    public void onPushPermissionResponse(boolean accepted) {
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_ON_PUSH_PERMISSION_RESPONSE_CALLBACK,
                String.valueOf(accepted));
    }

    // InAppNotificationListener
    public boolean beforeShow(Map<String, Object> var1) {
        return true;
    }

    @SuppressLint("RestrictedApi")
    public void onShow(CTInAppNotification ctInAppNotification) {
        if (ctInAppNotification != null && ctInAppNotification.getJsonDescription() != null) {
            final String json = "{inApp onShow() json payload:" + ctInAppNotification.getJsonDescription().toString() + "}";
            messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_INAPP_NOTIFICATION_SHOW_CALLBACK, json);
        }
    }

    public void onDismissed(Map<String, Object> var1, @Nullable Map<String, Object> var2) {
        if (var1 == null && var2 == null) {
            return;
        }

        JSONObject extras = var1 != null ? new JSONObject(var1) : new JSONObject();
        String _json = "{extras:" + extras.toString() + ",";

        JSONObject actionExtras = var2 != null ? new JSONObject(var2) : new JSONObject();
        _json += "actionExtras:" + actionExtras.toString() + "}";

        final String json = _json;
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_INAPP_NOTIFICATION_DISMISSED_CALLBACK, json);
    }

    // SyncListener
    public void profileDataUpdated(JSONObject updates) {

        if (updates == null) {
            return;
        }

        final String json = "{updates:" + updates.toString() + "}";
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_PROFILE_UPDATES_CALLBACK, json);
    }

    public void profileDidInitialize(String CleverTapID) {

        if (CleverTapID == null) {
            return;
        }

        final String json = "{CleverTapID:" + CleverTapID + "}";
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_PROFILE_INITIALIZED_CALLBACK, json);
    }

    //Inbox Listeners
    public void inboxDidInitialize() {
        final String json = "{CleverTap App Inbox Initialized}";
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_INBOX_DID_INITIALIZE, json);
    }

    public void inboxMessagesDidUpdate() {
        final String json = "{CleverTap App Inbox Messages Updated}";
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_INBOX_MESSAGES_DID_UPDATE, json);
    }

    //Inbox Button Click Listener
    public void onInboxButtonClick(HashMap<String, String> payload) {
        JSONObject jsonObject = new JSONObject(payload);
        final String json = "{inbox button payload:" + jsonObject.toString() + "}";
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_ON_INBOX_BUTTON_CLICKED, json);
    }


    public void onInboxItemClicked(CTInboxMessage message, int contentPageIndex, int buttonIndex) {
        if (message != null && message.getData() != null) {
            JSONObject jsonObject = new JSONObject();
            try {
                jsonObject.put("ContentPageIndex",contentPageIndex);
                jsonObject.put("ButtonIndex",buttonIndex);
                jsonObject.put("CTInboxMessagePayload",message.getData());
                messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_ON_INBOX_ITEM_CLICKED,
                    jsonObject.toString());
            } catch (JSONException e) {
                throw new RuntimeException(e);
            }
        }
    }

    //InApp Button Click Listener
    public void onInAppButtonClick(HashMap<String, String> payload) {
        JSONObject jsonObject = new JSONObject(payload);
        final String json = "{inapp button payload:" + jsonObject.toString() + "}";
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_ON_INAPP_BUTTON_CLICKED, json);
    }

    //Native Display Listener
    public void onDisplayUnitsLoaded(ArrayList<CleverTapDisplayUnit> units) {
        try {
            JSONArray jsonArray = displayUnitListToJSONArray(units);
            final String json = "{display units:" + jsonArray.toString() + "}";
            messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_DISPLAY_UNITS_UPDATED, json);
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    //Feature Flag Listener
    public void featureFlagsUpdated() {
        final String json = "{CleverTap App Feature Flags Updated}";
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_FEATURE_FLAG_UPDATED, json);
    }

    //Product Config Listener
    public void onInit() {
        final String json = "{CleverTap App Product Config Initialized}";
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_PRODUCT_CONFIG_INITIALIZED, json);
    }

    public void onFetched() {
        final String json = "{CleverTap App Product Config Fetched}";
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_PRODUCT_CONFIG_FETCHED, json);
    }

    public void onActivated() {
        final String json = "{CleverTap App Product Config Activated}";
        messageUnity(CLEVERTAP_GAME_OBJECT_NAME, CLEVERTAP_PRODUCT_CONFIG_ACTIVATED, json);
    }

    /*******************
     * Helpers
     ******************/

    private static void messageUnity(final String gameObject, final String method, final String data) {
        UnityPlayer.UnitySendMessage(gameObject, method, data);
    }

    private static Object fromJson(Object json) throws JSONException {
        if (json == JSONObject.NULL) {
            return null;
        } else if (json instanceof JSONObject) {
            return toMap((JSONObject) json);
        } else {
            return json;
        }
    }

    private static HashMap<String, Object> toMap(JSONObject object) throws JSONException {
        HashMap<String, Object> map = new HashMap<String, Object>();
        Iterator keys = object.keys();
        while (keys.hasNext()) {
            String key = (String) keys.next();
            map.put(key, fromJson(object.get(key)));
        }
        return map;
    }

    private static ArrayList<HashMap<String, Object>> toArrayListOfStringObjectMaps(JSONArray array)
            throws JSONException {
        ArrayList<HashMap<String, Object>> aList = new ArrayList<HashMap<String, Object>>();

        for (int i = 0; i < array.length(); i++) {
            aList.add(toMap((JSONObject) array.get(i)));
        }

        return aList;
    }

    private static JSONObject eventDetailsToJSON(EventDetail details) throws JSONException {

        JSONObject json = new JSONObject();

        if (details != null) {
            json.put("name", details.getName());
            json.put("firstTime", details.getFirstTime());
            json.put("lastTime", details.getLastTime());
            json.put("count", details.getCount());
        }

        return json;
    }

    private static JSONObject utmDetailsToJSON(UTMDetail details) throws JSONException {

        JSONObject json = new JSONObject();

        if (details != null) {
            json.put("campaign", details.getCampaign());
            json.put("source", details.getSource());
            json.put("medium", details.getMedium());
        }

        return json;
    }

    private static JSONObject eventHistoryToJSON(Map<String, EventDetail> history) throws JSONException {

        JSONObject json = new JSONObject();

        if (history != null) {
            for (Object key : history.keySet()) {
                json.put(key.toString(), eventDetailsToJSON(history.get((String) key)));
            }
        }

        return json;
    }

    static JSONObject localInAppFromMap(Map<String, Object> objectMap) {
        if (objectMap == null) {
            Log.e("CleverTapError", "LocalInApp map is null or empty");
            return null;
        }
        CTLocalInApp.InAppType inAppType = null;
        String titleText = null, messageText = null, positiveBtnText = null,
                negativeBtnText = null, backgroundColor = null, btnBorderColor = null,
                titleTextColor = null, messageTextColor = null,
                btnTextColor = null, imageUrl = null, btnBackgroundColor = null, btnBorderRadius = null;
        boolean fallbackToSettings = false, followDeviceOrientation = false;

        for (Map.Entry<String, Object> entry : objectMap.entrySet()) {
            try {
                String configKey = entry.getKey();
                if ("inAppType".equals(configKey)) {
                    inAppType = inAppTypeFromString((String) entry.getValue());
                }
                if ("titleText".equals(configKey)) {
                    titleText = (String) entry.getValue();
                }
                if ("messageText".equals(configKey)) {
                    messageText = (String) entry.getValue();
                }
                if ("followDeviceOrientation".equals(configKey)) {
                    followDeviceOrientation = (Boolean) entry.getValue();
                }
                if ("positiveBtnText".equals(configKey)) {
                    positiveBtnText = (String) entry.getValue();
                }
                if ("negativeBtnText".equals(configKey)) {
                    negativeBtnText = (String) entry.getValue();
                }
                if ("fallbackToSettings".equals(configKey)) {
                    fallbackToSettings = (Boolean) entry.getValue();
                }
                if ("backgroundColor".equals(configKey)) {
                    backgroundColor = (String) entry.getValue();
                }
                if ("btnBorderColor".equals(configKey)) {
                    btnBorderColor = (String) entry.getValue();
                }
                if ("titleTextColor".equals(configKey)) {
                    titleTextColor = (String) entry.getValue();
                }
                if ("messageTextColor".equals(configKey)) {
                    messageTextColor = (String) entry.getValue();
                }
                if ("btnTextColor".equals(configKey)) {
                    btnTextColor = (String) entry.getValue();
                }
                if ("imageUrl".equals(configKey)) {
                    imageUrl = (String) entry.getValue();
                }
                if ("btnBackgroundColor".equals(configKey)) {
                    btnBackgroundColor = (String) entry.getValue();
                }
                if ("btnBorderRadius".equals(configKey)) {
                    btnBorderRadius = (String) entry.getValue();
                }
            } catch (Throwable t) {
                Log.e("CleverTapError", "Invalid parameters in LocalInApp config"
                        + t.getLocalizedMessage());
                return null;
            }
        }


        //creates the builder instance of localInApp with all the required parameters
        CTLocalInApp.Builder.Builder6 builderWithRequiredParams = getLocalInAppBuilderWithRequiredParam(
                inAppType, titleText, messageText, followDeviceOrientation, positiveBtnText,
                negativeBtnText
        );

        //adds the optional parameters to the builder instance
        if (backgroundColor != null) {
            builderWithRequiredParams.setBackgroundColor(backgroundColor);
        }
        if (btnBorderColor != null) {
            builderWithRequiredParams.setBtnBorderColor(btnBorderColor);
        }
        if (titleTextColor != null) {
            builderWithRequiredParams.setTitleTextColor(titleTextColor);
        }
        if (messageTextColor != null) {
            builderWithRequiredParams.setMessageTextColor(messageTextColor);
        }
        if (btnTextColor != null) {
            builderWithRequiredParams.setBtnTextColor(btnTextColor);
        }
        if (imageUrl != null) {
            builderWithRequiredParams.setImageUrl(imageUrl);
        }
        if (btnBackgroundColor != null) {
            builderWithRequiredParams.setBtnBackgroundColor(btnBackgroundColor);
        }
        if (btnBorderRadius != null) {
            builderWithRequiredParams.setBtnBorderRadius(btnBorderRadius);
        }
        builderWithRequiredParams.setFallbackToSettings(fallbackToSettings);

        JSONObject localInAppConfig = builderWithRequiredParams.build();
        Log.i("CTLocalInAppConfig", "LocalInAppConfig for push primer prompt: "
                + localInAppConfig);
        return localInAppConfig;
    }

    /**
     * Creates an instance of the {@link CTLocalInApp.Builder.Builder6} with the required parameters.
     *
     * @return the {@link CTLocalInApp.Builder.Builder6} instance
     */
    private static CTLocalInApp.Builder.Builder6
    getLocalInAppBuilderWithRequiredParam(CTLocalInApp.InAppType inAppType,
                                          String titleText, String messageText,
                                          boolean followDeviceOrientation, String positiveBtnText,
                                          String negativeBtnText) {

        //throws exception if any of the required parameter is missing
        if (inAppType == null || titleText == null || messageText == null || positiveBtnText == null
                || negativeBtnText == null) {
            throw new IllegalArgumentException("Mandatory parameters are missing for LocalInApp config");
        }

        CTLocalInApp.Builder builder = CTLocalInApp.builder();
        return builder.setInAppType(inAppType)
                .setTitleText(titleText)
                .setMessageText(messageText)
                .followDeviceOrientation(followDeviceOrientation)
                .setPositiveBtnText(positiveBtnText)
                .setNegativeBtnText(negativeBtnText);
    }

    private static CTLocalInApp.InAppType inAppTypeFromString(String inAppType) {
        if (inAppType == null) {
            return null;
        }
        switch (inAppType) {
            case "half-interstitial":
                return CTLocalInApp.InAppType.HALF_INTERSTITIAL;
            case "alert":
                return CTLocalInApp.InAppType.ALERT;
            default:
                return null;
        }
    }

    @SuppressWarnings({"rawtypes", "unchecked"})
    private static CTInboxStyleConfig toStyleConfig(JSONObject object) throws JSONException {
        CTInboxStyleConfig styleConfig = new CTInboxStyleConfig();

        if (object.has("noMessageText")) {
            styleConfig.setNoMessageViewText(object.getString("noMessageText"));
        }
        if (object.has("noMessageTextColor")) {
            styleConfig.setNoMessageViewTextColor(object.getString("noMessageTextColor"));
        }
        if (object.has("navBarColor")) {
            styleConfig.setNavBarColor(object.getString("navBarColor"));
        }
        if (object.has("navBarTitle")) {
            styleConfig.setNavBarTitle(object.getString("navBarTitle"));
        }
        if (object.has("navBarTitleColor")) {
            styleConfig.setNavBarTitleColor(object.getString("navBarTitleColor"));
        }
        if (object.has("inboxBackgroundColor")) {
            styleConfig.setInboxBackgroundColor(object.getString("inboxBackgroundColor"));
        }
        if (object.has("backButtonColor")) {
            styleConfig.setBackButtonColor(object.getString("backButtonColor"));
        }
        if (object.has("selectedTabColor")) {
            styleConfig.setSelectedTabColor(object.getString("selectedTabColor"));
        }
        if (object.has("unselectedTabColor")) {
            styleConfig.setUnselectedTabColor(object.getString("unselectedTabColor"));
        }
        if (object.has("selectedTabIndicatorColor")) {
            styleConfig.setSelectedTabIndicatorColor(object.getString("selectedTabIndicatorColor"));
        }
        if (object.has("tabBackgroundColor")) {
            styleConfig.setTabBackgroundColor(object.getString("tabBackgroundColor"));
        }
        if (object.has("firstTabTitle")) {
            styleConfig.setFirstTabTitle(object.getString("firstTabTitle"));
        }
        if (object.has("tabs")) {
            JSONArray tabsArray = object.getJSONArray("tabs");
            ArrayList tabs = new ArrayList();
            for (int i = 0; i < tabsArray.length(); i++) {
                tabs.add(tabsArray.getString(i));
            }
            styleConfig.setTabs(tabs);
        }
        return styleConfig;
    }

    private JSONArray inboxMessageListToJSONArray(ArrayList<CTInboxMessage> messageList) throws JSONException {
        JSONArray array = new JSONArray();

        for (int i = 0; i < messageList.size(); i++) {
            array.put(messageList.get(i).getData());
        }

        return array;
    }

    private JSONArray displayUnitListToJSONArray(ArrayList<CleverTapDisplayUnit> displayUnits) throws JSONException {
        JSONArray array = new JSONArray();

        for (int i = 0; i < displayUnits.size(); i++) {
            array.put(displayUnits.get(i).getJsonObject());
        }

        return array;
    }
}