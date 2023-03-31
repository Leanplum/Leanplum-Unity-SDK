IOS_SDK_VERSION?=6.0.5-beta1

ANDROID_SDK_VERSION?=7.1.2-beta1

UNITY_VERSION?=5.1.0-beta1

UNITY_EDITOR_VERSION?=2020.3.30f1

export ANDROID_HOME?=$(shell echo ${HOME})/Library/Android/sdk

unitypackage:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION} --version=${UNITY_VERSION} --unity-editor-version=${UNITY_EDITOR_VERSION}

unitypackage-copy-beta:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION} --version=${UNITY_VERSION} --unity-editor-version=${UNITY_EDITOR_VERSION} --apple-copy