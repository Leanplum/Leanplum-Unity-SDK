IOS_SDK_VERSION?=6.0.4

ANDROID_SDK_VERSION?=7.1.1
# CT version must be the same as in the Leanplum Android SDK and the module clevertapsdk-unity-x.x.x
CT_ANDROID_SDK_VERSION?=4.7.4

UNITY_VERSION?=5.0.0

UNITY_EDITOR_VERSION?=2021.3.15f1

export ANDROID_HOME?=$(shell echo ${HOME})/Library/Android/sdk

unitypackage:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION} --ct-android-sdk-version=${CT_ANDROID_SDK_VERSION} --version=${UNITY_VERSION} --unity-editor-version=${UNITY_EDITOR_VERSION}

unitypackage-copy-beta:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION} --ct-android-sdk-version=${CT_ANDROID_SDK_VERSION} --version=${UNITY_VERSION} --unity-editor-version=${UNITY_EDITOR_VERSION} --apple-copy
