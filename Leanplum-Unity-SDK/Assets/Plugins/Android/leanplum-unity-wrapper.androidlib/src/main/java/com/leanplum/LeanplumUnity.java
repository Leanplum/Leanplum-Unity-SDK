package com.leanplum;

import android.app.Application;

import com.clevertap.unity.CleverTapCustomTemplates;
import com.leanplum.annotations.Parser;

public class LeanplumUnity {

    /**
     * Initializes the Leanplum SDK for Unity. It is recommended to call this method
     * in {@link android.app.Application#onCreate()}, extend or directly use
     * {@link LeanplumUnityApplication} as application class to ensure proper
     * initialization.
     */
    public static void initialize(Application application) {
        CleverTapCustomTemplates.registerCustomTemplates(application);
        Leanplum.setApplicationContext(application);
        Parser.parseVariables(application);
        LeanplumActivityHelper.enableLifecycleCallbacks(application);
    }
}
