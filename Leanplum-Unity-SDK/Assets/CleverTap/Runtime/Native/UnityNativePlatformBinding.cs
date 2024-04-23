#if !UNITY_IOS && !UNITY_ANDROID 
using CleverTapSDK.Common;
using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native {
    internal class UnityNativePlatformBinding : CleverTapPlatformBindings {
        internal UnityNativePlatformBinding() {
            CallbackHandler = CreateGameObjectAndAttachCallbackHandler<UnityNativeCallbackHandler>(CleverTapGameObjectName.UNITY_NATIVE_CALLBACK_HANDLER);
            CleverTapLogger.Log("Start: no-op CleverTap binding for non iOS/Android.");
        }
    }
}
#endif
