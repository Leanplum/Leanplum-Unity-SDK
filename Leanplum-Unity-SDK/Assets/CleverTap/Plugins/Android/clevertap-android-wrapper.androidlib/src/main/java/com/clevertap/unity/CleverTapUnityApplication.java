package com.clevertap.unity;

import android.app.Application;

import com.clevertap.android.sdk.ActivityLifecycleCallback;

public class CleverTapUnityApplication extends Application {

    @Override
    public void onCreate() {
        ActivityLifecycleCallback.register(this);
        super.onCreate();
        CleverTapUnityAPI.initialize(this);
    }
}
