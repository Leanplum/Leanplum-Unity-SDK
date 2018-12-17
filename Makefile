
IOS_SDK_VERSION?=2.2.0
ANDROID_SDK_VERSION?=4.2.1
UNITY_VERSION?=1.6.1
export ANDROID_HOME?=$(shell echo ${HOME})/Library/Android/sdk
unitypackage:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION} --version=${UNITY_VERSION}
