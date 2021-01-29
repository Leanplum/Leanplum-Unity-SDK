IOS_SDK_VERSION?=3.1.1

ANDROID_SDK_VERSION?=5.5.0
ANDROID_SDK_VERSION_PRE_X?=4.2.9

UNITY_VERSION?=3.1.0-beta6
UNITY_VERSION_PRE_X?=1.6.17

export ANDROID_HOME?=$(shell echo ${HOME})/Library/Android/sdk

unitypackage:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION} --version=${UNITY_VERSION}

unitypackage-pre-x:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION_PRE_X} --version=${UNITY_VERSION_PRE_X}