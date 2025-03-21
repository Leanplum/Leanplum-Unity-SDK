# Enable CleverTap optional features
## Audio and video content within in-app notifications
Add the following dependencies to `Assets/Plugins/Android/mainTemplate.gradle`:
```diff
dependencies {
    //...
+    // ExoPlayer for CT
+    implementation 'androidx.media3:media3-exoplayer:1.1.1'
+    implementation 'androidx.media3:media3-exoplayer-hls:1.1.1'
+    implementation 'androidx.media3:media3-ui:1.1.1'
    //...
}
```
## Notification Inbox
Add the following dependencies to `Assets/Plugins/Android/mainTemplate.gradle`:
```diff
dependencies {
    //...
+    implementation("com.github.bumptech.glide:glide:4.12.0")
+    implementation("com.google.android.material:material:1.12.0")
+    implementation("androidx.fragment:fragment:1.5.4")
+    implementation("androidx.appcompat:appcompat:1.7.0")
+    implementation("androidx.recyclerview:recyclerview:1.3.2")
+    implementation("androidx.viewpager:viewpager:1.0.0")
    //...
}
```
## RenderMax
Add the following dependency to `Assets/Plugins/Android/mainTemplate.gradle`:
```diff
dependencies {
    //...
+    implementation "com.clevertap.android:clevertap-rendermax-sdk:1.0.3"
    //...
}
```
## Custom Code Templates
1. Follow the instructions in the CleverTap documentation [here](https://github.com/CleverTap/clevertap-unity-sdk/blob/master/docs/CustomCodeTemplates.md) to define your templates.
2. Register the custom code delegates in `Leanplum.CleverTapInstanceReady`:
```csharp
    private void InitLeanplum()
    {
        Leanplum.CleverTapInstanceReady += () =>
        {
            // Call SyncCustomTemplates once to sync the templates to the dashboard. Make sure to call it in a debug build.
            // CleverTap.SyncCustomTemplates();
            CleverTap.OnCustomTemplatePresent += CleverTapCustomTemplatePresent;
            CleverTap.OnCustomFunctionPresent += CleverTapCustomFunctionPresent;
            CleverTap.OnCustomTemplateClose += CleverTapCustomTemplateClose;
        };

    }

    private void CleverTapCustomTemplatePresent(CleverTapTemplateContext context)
    {
        // show the UI for the template, be sure to keep the context as long as the template UI
        // is being displayed so that context.setDismissed() can be called when the UI is closed.
        ShowTemplateUi(context);
        // call customTemplateSetPresented when the UI has become visible to the user
        context.SetPresented();
    }

    private void CleverTapCustomFunctionPresent(CleverTapTemplateContext context)
    {
        // show the UI for the template, be sure to keep the context as long as the template UI
        // is being displayed so that context.setDismissed() can be called when the UI is closed.
        ShowTemplateUi(context);
        // call customTemplateSetPresented when the UI has become visible to the user
        context.SetPresented();
    }

    private void CleverTapCustomTemplateClose(CleverTapTemplateContext context)
    {
        // close the corresponding UI before calling customTemplateSetDismissed
        context.SetDismissed();
    }
```
## Push Templates
1. Add the following dependency to `Assets/Plugins/Android/mainTemplate.gradle`:
```diff
dependencies {
    //...
+    implementation 'com.clevertap.android:push-templates:1.2.4'
}
```
2. Create a custom Application Java class in `Assets/Plugins/Android/` with the push templates initialization call:
```java
package com.example.app;

import com.clevertap.android.pushtemplates.PushTemplateNotificationHandler;
import com.clevertap.android.sdk.CleverTapAPI;
import com.leanplum.LeanplumUnityApplication;

public class ExampleApplication extends LeanplumUnityApplication {

    @Override
    public void onCreate() {
        CleverTapAPI.setNotificationHandler(new PushTemplateNotificationHandler());
        super.onCreate();
    }
}
```
3. Set your custom Application class in the AndroidManifest.xml:
```diff
    <manifest
        xmlns:android="http://schemas.android.com/apk/res/android"
        xmlns:tools="http://schemas.android.com/tools">
-        <application android:name="com.leanplum.LeanplumUnityApplication">
+        <application android:name="com.example.app.ExampleApplication">
            <activity android:name="com.unity3d.player.UnityPlayerActivity"
                      android:theme="@style/UnityThemeSelector">
                <intent-filter>
                    <action android:name="android.intent.action.MAIN" />
                    <category android:name="android.intent.category.LAUNCHER" />
                </intent-filter>
                <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
            </activity>
        </application>
    </manifest>
```
