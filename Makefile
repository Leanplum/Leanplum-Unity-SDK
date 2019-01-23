
IOS_SDK_VERSION?=2.2.4
ANDROID_SDK_VERSION?=4.2.3
UNITY_VERSION?=1.6.5
export ANDROID_HOME?=$(shell echo ${HOME})/Library/Android/sdk
unitypackage:
	./build.sh --apple-sdk-version=${IOS_SDK_VERSION} --android-sdk-version=${ANDROID_SDK_VERSION} --version=${UNITY_VERSION}
