# Specify the iOS SDK version in EDM4U Assets/Plugins/Editor/LeanplumDependencies.xml

ANDROID_SDK_VERSION?=7.4.1
# CleverTap version must be the same as in the Leanplum Android SDK and the module clevertapsdk-unity-x.x.x
CT_ANDROID_SDK_VERSION?=6.1.1

UNITY_VERSION?=6.1.0

UNITY_EDITOR_VERSION?=2022.3.50f1

export ANDROID_HOME?=$(shell echo ${HOME})/Library/Android/sdk

unitypackage:
	./build.sh  --android-sdk-version=${ANDROID_SDK_VERSION} --ct-android-sdk-version=${CT_ANDROID_SDK_VERSION} --version=${UNITY_VERSION} --unity-editor-version=${UNITY_EDITOR_VERSION}