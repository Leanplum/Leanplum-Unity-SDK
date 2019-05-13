IOS_SDK_VERSION?=2.4.1

ANDROID_SDK_VERSION?=5.2.2
ANDROID_SDK_VERSION_PRE_X?=4.2.5

UNITY_VERSION?=2.0.2
UNITY_VERSION_PRE_X?=1.6.7

export ANDROID_HOME?=$(shell echo ${HOME})/Library/Android/sdk

unitypackage:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION} --version=${UNITY_VERSION}

unitypackage-pre-x:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION_PRE_X} --version=${UNITY_VERSION_PRE_X}