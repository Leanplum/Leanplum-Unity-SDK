# Leanplum Unity Android Integration Instructions
After you have imported the Leanplum SDK unity package do the following steps to complete the integration for the Android platform:
1. Enable custom main manifest file in Unity. In the Unity Editor, navigate to `Build Settings - Player Settings - Publishing Settings` and enable `Custom Main Manifest`.
2. Edit `Assets/Plugins/Android/AndroidManifest.xml` to set the application class. You can use the provided `LeanplumApplication` class, extend it with your own class, or have a completely separate class, which calls the appropriate Leanplum API.
  * Add the application class to the manifest file:
    ```diff
    <manifest
        xmlns:android="http://schemas.android.com/apk/res/android"
        xmlns:tools="http://schemas.android.com/tools">
    -    <application>
    +    <application android:name="com.leanplum.LeanplumUnityApplication">
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
  * If you are using a custom application class extend `LeanplumApplication`:
    ```java
    public class YourApplication extends LeanplumUnityApplication {
        ...
    }
    ```
    or call `LeanplumUnity.initialize(this)` in the `onCreate` method:
    ```java
    public class YourApplication extends Application {

        @Override
        public void onCreate() {
            super.onCreate();
            LeanplumUnity.initialize(this);
        }
    }
    ```
## Include Optional Leanplum Features
1. Enable main gradle template file in Unity. In the Unity Editor, navigate to `Build Settings - Player Settings - Publishing Settings` and enable `Custom Main Gradle Template`.
2. Edit `Assets/Plugins/Android/mainTemplate.gradle` to include only the desired optional features:
```groovy
dependencies {
    // Push
    implementation "com.leanplum:leanplum-push:${LP_VERSION}"

    // FCM
    implementation "com.leanplum:leanplum-fcm:${LP_VERSION}"
    implementation 'com.google.firebase:firebase-messaging:22.0.0'

    // HMS
    implementation "com.leanplum:leanplum-hms:${LP_VERSION}"

    // Location
    implementation "com.leanplum:leanplum-location:${LP_VERSION}"
    implementation "com.google.android.gms:play-services-location:18.0.0"
}
```
3. Use the same version for the optional features as the Leanplum version in `Assets/Plugins/Android/leanplum-unity-wrapper.androidlib/build.gradle`
```groovy
    def LP_VERSION = "X.X.X"
```
4. If you are using FCM, download `google-service.json` file and put it into root of `Assets` folder. When Android project is exported it will automatically parse the file.
## CleverTap Integration
1. Edit `Assets/Plugins/Android/AndroidManifest.xml` to set the launcher activity to the CleverTap class:
    ```diff
    <manifest
        xmlns:android="http://schemas.android.com/apk/res/android"
        xmlns:tools="http://schemas.android.com/tools">
        <application>
        <application android:name="com.leanplum.LeanplumUnityApplication">
    -        <activity android:name="com.unity3d.player.UnityPlayerActivity"
    +        <activity android:name="com.clevertap.unity.CleverTapOverrideActivity"
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
2. Use CleverTap Unity API after `Leanplum.CleverTapInstanceReady` event is received:
    ```csharp
        Leanplum.CleverTapInstanceReady += () =>
        {
            // CleverTap is ready to use
            CleverTap.CreateNotificationChannel(
                "YourChannelId",
                "Your Channel Name",
                "Your Channel Description",
                5,
                true);
        };
    ```
3. Do not use CleverTapSettings from the Unity editor and do not add CleverTap account data in the AndroidManifest.xml file, Leanplum will apply the correct values automatically.
4. See how to enable the optional CleverTap features [here](CleverTap-Optional-Features.md)