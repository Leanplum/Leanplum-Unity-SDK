
IOS_SDK_VERSION?=2.1.0
ANDROID_SDK_VERSION?=4.2.0
export UNITY_VERSION?=2.0.0
export ANDROID_HOME?=$(shell echo ${HOME})/Library/Android/sdk
unitypackage:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION}
