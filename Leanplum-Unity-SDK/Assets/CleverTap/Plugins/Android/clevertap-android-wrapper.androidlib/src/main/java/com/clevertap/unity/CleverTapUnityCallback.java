package com.clevertap.unity;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.clevertap.unity.callback.PluginCallback;

public enum CleverTapUnityCallback {
    CLEVERTAP_PROFILE_INITIALIZED_CALLBACK("CleverTapProfileInitializedCallback", true),
    CLEVERTAP_PROFILE_UPDATES_CALLBACK("CleverTapProfileUpdatesCallback"),
    CLEVERTAP_DEEP_LINK_CALLBACK("CleverTapDeepLinkCallback", true),
    CLEVERTAP_PUSH_OPENED_CALLBACK("CleverTapPushOpenedCallback", true),
    CLEVERTAP_INAPP_NOTIFICATION_DISMISSED_CALLBACK("CleverTapInAppNotificationDismissedCallback", true),
    CLEVERTAP_INAPP_NOTIFICATION_SHOW_CALLBACK("CleverTapInAppNotificationShowCallback", true, Mode.DIRECT_CALLBACK),
    CLEVERTAP_ON_PUSH_PERMISSION_RESPONSE_CALLBACK("CleverTapOnPushPermissionResponseCallback"),
    CLEVERTAP_INBOX_DID_INITIALIZE("CleverTapInboxDidInitializeCallback", true),
    CLEVERTAP_INBOX_MESSAGES_DID_UPDATE("CleverTapInboxMessagesDidUpdateCallback"),
    CLEVERTAP_ON_INBOX_BUTTON_CLICKED("CleverTapInboxCustomExtrasButtonSelect"),
    CLEVERTAP_ON_INBOX_ITEM_CLICKED("CleverTapInboxItemClicked"),
    CLEVERTAP_ON_INAPP_BUTTON_CLICKED("CleverTapInAppNotificationButtonTapped", true, Mode.DIRECT_CALLBACK),
    CLEVERTAP_DISPLAY_UNITS_UPDATED("CleverTapNativeDisplayUnitsUpdated", true),
    @Deprecated
    CLEVERTAP_FEATURE_FLAG_UPDATED("CleverTapFeatureFlagsUpdated", true),
    @Deprecated
    CLEVERTAP_PRODUCT_CONFIG_INITIALIZED("CleverTapProductConfigInitialized", true),
    @Deprecated
    CLEVERTAP_PRODUCT_CONFIG_FETCHED("CleverTapProductConfigFetched"),
    @Deprecated
    CLEVERTAP_PRODUCT_CONFIG_ACTIVATED("CleverTapProductConfigActivated"),
    CLEVERTAP_INIT_CLEVERTAP_ID_CALLBACK("CleverTapInitCleverTapIdCallback"),
    CLEVERTAP_VARIABLES_CHANGED("CleverTapVariablesChanged"),
    CLEVERTAP_VARIABLE_VALUE_CHANGED("CleverTapVariableValueChanged"),
    CLEVERTAP_VARIABLES_FETCHED("CleverTapVariablesFetched"),
    CLEVERTAP_ONE_TIME_VARIABLES_CHANGED("OneTimeCleverTapVariablesChanged"),
    CLEVERTAP_ONE_TIME_VARIABLES_CHANGED_AND_NO_DOWNLOADS_PENDING("OneTimeCleverTapVariablesChangedAndNoDownloadsPending"),
    CLEVERTAP_INAPPS_FETCHED("CleverTapInAppsFetched"),
    CLEVERTAP_VARIABLES_CHANGED_AND_NO_DOWNLOADS_PENDING("CleverTapVariablesChangedAndNoDownloadsPending"),
    CLEVERTAP_FILE_VARIABLE_READY("CleverTapVariableFileIsReady"),
    CLEVERTAP_CUSTOM_TEMPLATE_PRESENT("CleverTapCustomTemplatePresent", true),
    CLEVERTAP_CUSTOM_FUNCTION_PRESENT("CleverTapCustomFunctionPresent", true),
    CLEVERTAP_CUSTOM_TEMPLATE_CLOSE("CleverTapCustomTemplateClose");


    @Nullable
    public static CleverTapUnityCallback fromName(String callbackName) {
        for (CleverTapUnityCallback callback : values()) {
            if (callback.callbackName.equals(callbackName)) {
                return callback;
            }
        }
        return null;
    }

    @NonNull
    public final String callbackName;
    public final boolean bufferable;
    @NonNull
    public final Mode mode;
    @Nullable
    public PluginCallback pluginCallback;

    CleverTapUnityCallback(@NonNull String callbackName, boolean bufferable, @NonNull Mode mode) {
        this.callbackName = callbackName;
        this.bufferable = bufferable;
        this.mode = mode;
    }

    CleverTapUnityCallback(String callbackName, boolean bufferable) {
        this(callbackName, bufferable, Mode.UNITY_PLAYER_MESSAGE);
    }

    CleverTapUnityCallback(String callbackName) {
        this(callbackName, false);
    }

    public enum Mode {
        UNITY_PLAYER_MESSAGE,
        DIRECT_CALLBACK
    }
}
