#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
namespace CleverTapSDK.Native {
    internal interface IUnityNativeResponseInterceptor {
        internal UnityNativeResponse Intercept(UnityNativeResponse response);
    }
}
#endif