package com.clevertap.unity;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;

import com.clevertap.android.sdk.CleverTapAPI;

public class CleverTapUnityAPI {

    /**
     * Initializes the CleverTap SDK for Unity. It is recommended to call this method
     * in {@link android.app.Application#onCreate()} or extend
     * {@link CleverTapUnityApplication} to ensure proper initialization.
     */
    public static void initialize(Context context) {
        CleverTapCustomTemplates.registerCustomTemplates(context);
        CleverTapAPI clevertap = CleverTapAPI.getDefaultInstance(context);
        if (clevertap != null) {
            clevertap.setLibrary("Unity");
            CleverTapUnityCallbackHandler.getInstance().attachToApiInstance(clevertap);
        }
    }

    /**
     * Handle new intents of a launcher Activity. Call this method in
     * {@link Activity#onNewIntent(Intent)} or extend {@link CleverTapOverrideActivity}.
     *
     * @param activity The launcher Activity
     * @param intent The intent received in {@link Activity#onNewIntent(Intent)}
     */
    public static void onLauncherActivityNewIntent(Activity activity, Intent intent) {
        handleIntent(activity, intent, true);
    }

    /**
     * Handle the starting intent of a launcher Activity. Call this method in
     * {@link Activity#onCreate(Bundle)} or extend {@link CleverTapOverrideActivity}.
     *
     * @param activity The launcher Activity
     */
    public static void onLauncherActivityCreate(Activity activity) {
        handleIntent(activity, activity.getIntent(), false);
    }

    private static void handleIntent(Activity activity, Intent intent, boolean isOnNewIntent) {
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
                    CleverTapAPI clevertap = CleverTapAPI.getDefaultInstance(activity);
                    if (clevertap != null) {
                        clevertap.pushNotificationClickedEvent(extras);
                    }
                }
            }
        }
    }
}
