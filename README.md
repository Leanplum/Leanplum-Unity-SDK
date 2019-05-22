# Leanplum-Unity-SDK
**Please refer to the active development branch develop to see the latest source code.**

Native, iOS & Android Leanplum SDK for Unity3D.
## Installation
- Make sure you have following [Homebrew](https://brew.sh) packages installed:
```
brew install maven
brew install gradle
brew install bash
brew install wget
brew install Caskroom/cask/android-sdk
brew install Caskroom/cask/unity
```
- Download iOS and Android module in Unity.
- Go to Unity's preference to set Android SDK path. It should be same as $ANDROID_HOME, e.g. `~/Library/Android/sdk`

## Build
To build a `unitypackage` from source, execute the build shell script from the root of this repository:

`./build.sh --apple-sdk-version=2.3.0 --android-sdk-version=4.3.1 --version=1.7.0`

or to use latest version of Android/iOS SDK (this uses public github api to make request)

`./build.sh`

or

`make unitypackage`

This will create a new `unitypackage` in `Leanplum-Unity-Plugin` that you can import into other projects.

## Release

Open up `Makefile` and update versions of the SDK to desired version.

To manually release new build run:

`make unitypackage`

or

`make release`

which will build new version of the Unity SDK, tag it with `UNITY_VERSION` specified in `Makefile` and push new commit to master branch.

## Usage

Import `unitypackage` into your project, make sure you select `Editor, LeanplumSDK and Plugins` folders, `LeanplumSample` is optional.

### Android Export

Make sure you add 
```
android.enableJetifier=true
android.useAndroidX=true
```
in your `gradle.properties` file, because latest Leanplum SDK is using `androidx` support libraries.

You can choose Leanplum dependencies in your `gradle.build` file of the exported project.

If you are using FCM, download `google-service.json` file and put it into root of `Assets` folder. When Android project is exported it will automatically parse the file.

## iOS Export

No additional setup is needed.

## Development
To make changes to this SDK, open the `LeanplumSample` Unity project that is included in this repository. That project contains the Leanplum SDK itself as well as a small sample application that you can use to test your changes as you iterate.

To run the sample and test your changes, open the scene `Assets/LeanplumSample/LeanplumSample.unity`. Populate your Leanplum App Id and keys from the Leanplum dashboard into the `Leanplum` GameObject in the hierarchy, then press Play. You can experiment with changing variables by changing the value of the `rainEmissionRate` variable on your Leanplum dashbaord. If everything's working properly, you should see the rain particles' emission change in realtime while the app is playing in your editor.

You can make changes to this SDK directly in the project as needed, testing them with the sample scene as explained above. When you're satisfied with your changes, you can use the `Tools/Leanplum/Export Package` menu option to export a new unitypackage containing your latest changes. This menu option assumes you've run the `build.sh` script at least once.

#### Optimizations
You may opt-in to *significantly* higher performance with the Leanplum SDK by defining one or both of these preprocessor symbols in your Unity project:

`LP_UNENCRYPTED`: Defining this symbol will cause Leanplum to use Unity's optimized `PlayerPrefs` for local storage instead of its own string-concantenation-heavy, legacy-Hashtable-based, GC-hungry version. However, the price you pay is that local storage is no longer encrypted. The choice is yours to make on a per-project basis.

`LP_UNITYWEBREQUEST`: Defining this symbol will cause the Leanplum SDK to use [Unity's modern web stack](https://docs.unity3d.com/Manual/UnityWebRequest.html) instead of the legacy `www` class for network operations. This yields some important memory and performance benefits on mobile devices for some applications.

#### Normal Workflow
This release now supports the normal Unity workflow: open Unity, make changes, test your changes, then publish.
#### Scripts are Exported
The unitypackage that is generated for this SDK no longer contains a .DLL that was compiled with a fixed version of `gmcs` and fixed preprocessor symbols. Instead, the package now contains the source `.cs` files for Leanplum. This means you *can* now successfully rely on preprocessor symbols within this SDK, such as `UNITY_5_6_OR_NEWER`, the per-platform symbols (e.g. `UNITY_IOS`, `UNITY_ANDROID`, etc.) as needed and those conditionals *will* work properly in end users' projects.
#### UnityWebRequest
This release of the SDK adds support for UnityWebRequest and friends to reap the memory benefits and optimizations of moving away from the old `www` class in Unity.

### Overview of the files
TLDR; Unity <-> Leanplum.cs <-> SDKObject <-> [PLATFORM].cs <-> Bridge <-> iOS/Android SDK

First you declare your functions in Leanplum.cs which is just calling the SDK factory method. In order for Factory to work you need to have an abstract methods (interface/protocol), which is the LeanplumSDKObject.cs. You need to implement these methods for each platform: native, ios, and android. For Android/iOS there are bridge files. This is helpful when sending custom objects such as List<object> to C# by using JSON.
### Test out the code in Unity
`open "/Unity SDK/LeanplumSample/Assets/Standard Assets/Leanplum/LeanplumWrapper.cs"`

Before calling Leanplum.Start() add 
```
Leanplum.Started += delegate(bool success) {
  Debug.Log("Variants: " + Leanplum.Variants().Count);
  Debug.Log("Messages: " + Leanplum.MessageMetadata().Keys.Count);
};
```

You can see more at https://www.leanplum.com/dashboard#/5019738286063616/help/docs/unity

## Contributing
Please follow the guidelines under https://github.com/Leanplum/Leanplum-Unity-SDK/blob/master/CONTRIBUTING.md

Once you have done that, in general:
1. Fork it!
2. Create your feature branch: `git checkout -b feature/my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin feature/my-new-feature`
5. Submit a pull request :D

## License
See LICENSE file.

Leanplum does not support custom modifications to the SDK, without an approved pull request. Please send a pull request if you wish to include your changes.

