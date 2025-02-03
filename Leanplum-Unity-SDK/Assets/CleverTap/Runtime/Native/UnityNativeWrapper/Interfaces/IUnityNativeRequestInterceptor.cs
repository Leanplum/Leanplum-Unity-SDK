#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
namespace CleverTapSDK.Native {
    internal interface IUnityNativeRequestInterceptor {
        internal UnityNativeRequest Intercept(UnityNativeRequest request);
    }
}
#endif