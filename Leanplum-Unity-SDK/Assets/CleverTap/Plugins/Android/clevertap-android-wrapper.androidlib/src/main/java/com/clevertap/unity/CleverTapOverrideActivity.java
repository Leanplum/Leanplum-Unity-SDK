package com.clevertap.unity;

import android.content.Intent;
import android.os.Bundle;

import com.unity3d.player.UnityPlayerActivity;


public class CleverTapOverrideActivity extends UnityPlayerActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        CleverTapUnityAPI.onLauncherActivityCreate(this);
    }

    @Override
    public void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        CleverTapUnityAPI.onLauncherActivityNewIntent(this, intent);
    }
}
