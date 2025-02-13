package com.clevertap.unity;

import android.app.Activity;
import android.app.Application;
import android.os.Bundle;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.clevertap.android.sdk.InAppNotificationActivity;

public class CleverTapLifecycleCallbacks implements Application.ActivityLifecycleCallbacks {

    private static final CleverTapLifecycleCallbacks lifecycleCallbacks = new CleverTapLifecycleCallbacks();
    private static boolean fullScreenInApps;

    private CleverTapLifecycleCallbacks() {
    }

    public synchronized static void register(@NonNull Application application, boolean fullScreenInApps) {
        CleverTapLifecycleCallbacks.fullScreenInApps = fullScreenInApps;

        application.unregisterActivityLifecycleCallbacks(lifecycleCallbacks);
        application.registerActivityLifecycleCallbacks(lifecycleCallbacks);
    }

    @Override
    public void onActivityCreated(@NonNull Activity activity, @Nullable Bundle savedInstanceState) {
        if (activity instanceof InAppNotificationActivity) {
            Window window = activity.getWindow();
            if (window == null) {
                return;
            }

            if (fullScreenInApps) {
                window.addFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN);
                window.getDecorView().setSystemUiVisibility(View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                        | View.SYSTEM_UI_FLAG_FULLSCREEN);
            } else {
                window.clearFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN);
                window.getDecorView().setSystemUiVisibility(View.SYSTEM_UI_FLAG_VISIBLE);
            }
        }
    }

    @Override
    public void onActivityStarted(@NonNull Activity activity) {
    }

    @Override
    public void onActivityResumed(@NonNull Activity activity) {
    }

    @Override
    public void onActivityPaused(@NonNull Activity activity) {
    }

    @Override
    public void onActivityStopped(@NonNull Activity activity) {
    }

    @Override
    public void onActivitySaveInstanceState(@NonNull Activity activity, @NonNull Bundle outState) {
    }

    @Override
    public void onActivityDestroyed(@NonNull Activity activity) {
    }
}
