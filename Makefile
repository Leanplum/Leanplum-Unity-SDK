# Specify the iOS SDK version in EDM4U Assets/Plugins/Editor/LeanplumDependencies.xml

ANDROID_SDK_VERSION?=7.6.3

UNITY_VERSION?=7.1.0

UNITY_EDITOR_VERSION?=2022.3.50f1

unitypackage:
	./build.sh  --android-sdk-version=${ANDROID_SDK_VERSION} --version=${UNITY_VERSION} --unity-editor-version=${UNITY_EDITOR_VERSION}