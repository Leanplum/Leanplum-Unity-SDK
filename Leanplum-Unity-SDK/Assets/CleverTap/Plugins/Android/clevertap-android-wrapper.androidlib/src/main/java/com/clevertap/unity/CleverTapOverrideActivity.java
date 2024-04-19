package com.clevertap.unity;

import android.os.Bundle;
import android.content.Intent;
import com.unity3d.player.UnityPlayerActivity;


public class CleverTapOverrideActivity extends UnityPlayerActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        handleIntent(getIntent());
    }

    @Override
    public void onNewIntent(Intent intent){
        super.onNewIntent(intent);
        handleIntent(intent);
    }

    private void handleIntent(Intent intent) {
        if (intent != null) {
            CleverTapUnityPlugin.handleIntent(intent, this);
        }
    }
}