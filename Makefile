IOS_SDK_VERSION?=6.1.1

ANDROID_SDK_VERSION?=7.2.0
# CT version must be the same as in the Leanplum Android SDK and the module clevertapsdk-unity-x.x.x
CT_ANDROID_SDK_VERSION?=5.0.0

UNITY_VERSION?=6.0.1-beta1

UNITY_EDITOR_VERSION?=2021.3.15f1

export ANDROID_HOME?=$(shell echo ${HOME})/Library/Android/sdk

unitypackage:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION} --ct-android-sdk-version=${CT_ANDROID_SDK_VERSION} --version=${UNITY_VERSION} --unity-editor-version=${UNITY_EDITOR_VERSION}

unitypackage-copy-beta:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION} --ct-android-sdk-version=${CT_ANDROID_SDK_VERSION} --version=${UNITY_VERSION} --unity-editor-version=${UNITY_EDITOR_VERSION} --apple-copy
