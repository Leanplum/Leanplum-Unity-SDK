# Leanplum-Unity-SDK
Native, iOS & Android Leanplum SDK for Unity3D.
## Installation & Usage
Use LPM for development environment setup & building the binary.
## Contributing
1. Fork it!
2. Create your feature branch: `git checkout -b feature/my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin feature/my-new-feature`
5. Submit a pull request :D
## History
- 1.4.1 - April 04, 2017 Includes all iOS 1.7.0 updates. Includes all Android 2.1.0 updates.
- 1.3.4 - February 06, 2017 Includes all iOS 1.4.3 updates. Includes all Android 1.3.3 updates.
- 1.3.3 - December 19, 2016 Includes all iOS 1.4.2 updates. Includes all Android 1.3.2 updates.
- 1.3.2 - November 21, 2016 Adds access to raw message meta data. Adds support for setTrafficSourceInfo and setAppVersion (iOS). Includes all iOS 1.4.1 updates. Includes all Android 1.3.1 updates.
- 1.3.1 - October 07, 2016 Includes all iOS 1.4.0.2 updates. Includes all Android 1.3.0.2 updates.
- 1.3.0 - October 04, 2016 Includes all iOS 1.4.0.2 updates. Includes all Android 1.3.0 updates.
- 1.2.20 - September 21, 2016 Fixed an issue where the "Allow Push Notifications” dialog was always presented after start.
- 1.2.19 - September 13, 2016 Includes all iOS 1.4.0.1 updates.
- 1.2.18 - July 18, 2016 Includes all iOS 1.3.12 updates.
- 1.2.17 - June 29, 2016 Fixes an issue where unwanted permissions were added to the Android Manifest file. Includes all Android 1.2.25 updates.
- 1.2.16 - May 27, 2016 Includes all iOS 1.3.11 updates.
- 1.2.15.1 - May 9, 2016 Fixes a bug that may cause the the SDK to not function properly on iOS version 7 or earlier.
- 1.2.15 - May 6, 2016 Fixes an issue when tracking "Open" actions for push notifications on iOS. Fixes an issue when retreiving variables values on Android. Includes all Android 1.2.24 and iOS 1.3.10 updates.
- 1.2.14 - April 4, 2016 Includes all Android 1.2.23 updates.
- 1.2.13 - March 16, 2016 Improves Leanplum.Start latency. Includes all Android 1.2.22 updates.
- 1.2.12 - March 7, 2016 Includes all Android 1.2.21 updates.
- 1.2.11 - November 10, 2015 Includes all iOS 1.3.9 updates.
- 1.2.10 Includes all iOS 1.3.8.1 updates.
- 1.2.9 Fixed issue where iOS SDK was reverted to an older version.
- 1.2.8 Fixes an issue on iOS where values cannot be retrieved for variables with names including dots ('.'). Includes all Android 1.2.16 updates.
- 1.2.7 Includes all Android 1.2.15 and iOS 1.3.7 updates.
- 1.2.6 Fixes synchronization with dashboard versioning. Includes all Android 1.2.14 and iOS 1.3.6 updates.
- 1.2.5 Includes all Android 1.2.14 and iOS 1.3.3 updates.
- 1.2.4 Update SDK for Unity 5.
- 1.2.3 Includes ForceContentUpdate which allows you to update your variables in the middle of a session. Fixes currency conversion on Android. Includes new marketing automation features on iOS and Android, including in app message triggers on user attributes and event parameters, and personalized fields based on the message trigger.
- 1.2.1 Fixes a compilation issue with the .NET Framework subset and splits the Android SDK into separate libraries to avoid duplicate dependencies.
- 1.2.0 The Unity SDK now supports in-app messaging and push notifications. The architecture of the SDK changed so that in iOS and Android, the native SDKs are used. This allows you to use the iOS or Android SDKs from your native code. When you upgrade to 1.2.0, the SDK will not remember user IDs set in previous versions of the SDK. Make sure you pass the user ID in the "Start" call if your user was logged in from a previous session.
- 1.0.7 Fixes an exception that can happen on network timeouts.
- 1.0.6 Various fixes and improvements.
- 1.0.5 Various fixes and improvements.
- 1.0.4 Fixed a websocket issue where the SDK could freeze the Unity Editor.
- 1.0.3 Fixes an issue with the Timezone API. Adds a method to set a custom device ID. Improves API error logging.
- 1.0.2 Removed registration prompt, added timezone support, and added methods to track in-app purchases on iOS and Google Play with receipt validation.
- 1.0.1 Added an option to turn off realtime updates in development mode. Load cached values immediately after calling Start instead of when the API call finishes.
- 1.0.0 Initial release.
## License
© 2017 Leanplum, Inc. All rights reserved.

You may not distribute this source code without prior written permission from Leanplum.
