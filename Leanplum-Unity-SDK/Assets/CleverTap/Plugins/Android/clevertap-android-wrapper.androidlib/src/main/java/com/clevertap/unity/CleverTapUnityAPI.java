package com.clevertap.unity;

import android.app.Activity;
import android.app.Application;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.view.Window;
import android.view.WindowManager;

import androidx.annotation.NonNull;

import com.clevertap.android.sdk.CleverTapAPI;

public class CleverTapUnityAPI {

    /**
     * Initializes the CleverTap SDK for Unity. It is recommended to call this method
     * in {@link android.app.Application#onCreate()} or extend
     * {@link CleverTapUnityApplication} to ensure proper initialization.
     */
    public static void initialize(Context context) {
        CleverTapCustomTemplates.registerCustomTemplates(context);
        setCleverTapApiInstance(CleverTapAPI.getDefaultInstance(context));
    }

    /**
     * Set a custom instance of {@link CleverTapAPI} that CleverTapUnity will use.
     */
    public static synchronized void setCleverTapApiInstance(CleverTapAPI cleverTapApi) {
        if (cleverTapApi != null) {
            cleverTapApi.setLibrary("Unity");
            CleverTapUnityPlugin.setCleverTapApiInstance(cleverTapApi);
            CleverTapUnityCallbackHandler.getInstance().attachToApiInstance(cleverTapApi);
        }
    }

    /**
     * Handle new intents of a launcher Activity. Call this method in
     * {@link Activity#onNewIntent(Intent)} or extend {@link CleverTapOverrideActivity}.
     *
     * @param activity The launcher Activity
     * @param intent   The intent received in {@link Activity#onNewIntent(Intent)}
     */
    public static void onLauncherActivityNewIntent(@NonNull Activity activity, Intent intent) {
        handleIntent(intent, true);
    }

    /**
     * Handle the starting intent of a launcher Activity. Call this method in
     * {@link Activity#onCreate(Bundle)} or extend {@link CleverTapOverrideActivity}.
     *
     * @param activity The launcher Activity
     */
    public static void onLauncherActivityCreate(@NonNull Activity activity) {
        handleIntent(activity.getIntent(), false);
        setInAppActivityFullScreenFromOtherActivity(activity);
    }

    /**
     * Show CleverTap in-app notifications in full screen.
     *
     * @param application  The application instance.
     * @param isFullScreen Whether to show in-app notifications in full screen or not.
     */
    public static void setInAppsFullScreen(@NonNull Application application, boolean isFullScreen) {
        CleverTapLifecycleCallbacks.register(application, isFullScreen);
    }

    private static void setInAppActivityFullScreenFromOtherActivity(@NonNull Activity activity) {
        Window window = activity.getWindow();
        if (window == null) {
            return;
        }

        int flags = window.getAttributes().flags;
        boolean isFullScreen = (flags & WindowManager.LayoutParams.FLAG_FULLSCREEN) != 0;
        setInAppsFullScreen(activity.getApplication(), isFullScreen);
    }

    private static void handleIntent(Intent intent, boolean isOnNewIntent) {
        if (intent == null || intent.getAction() == null) {
            return;
        }

        if (intent.getAction().equals(Intent.ACTION_VIEW)) {
            Uri data = intent.getData();
            if (data != null) {
                CleverTapUnityCallbackHandler.handleDeepLink(data);
            }
        } else if (isOnNewIntent) {
            // notify the CT SDK if a push notification payload is found since Activity#onNewIntent
            // is not part of the ActivityLifecycleCallback and the Unity activity has singleTask
            // launch mode
            Bundle extras = intent.getExtras();
            boolean isPushNotification = (extras != null && extras.get("wzrk_pn") != null);
            if (isPushNotification) {

                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
                    CleverTapAPI clevertap = CleverTapUnityPlugin.getCleverTapApiInstance();
                    if (clevertap != null) {
                        clevertap.pushNotificationClickedEvent(extras);
                    }
                }
            }
        }
    }
}
