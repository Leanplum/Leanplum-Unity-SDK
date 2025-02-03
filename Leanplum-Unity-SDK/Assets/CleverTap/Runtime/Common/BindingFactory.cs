#if UNITY_ANDROID && !UNITY_EDITOR
using CleverTapSDK.Android;
#elif UNITY_IOS && !UNITY_EDITOR
using CleverTapSDK.IOS;
#else
using CleverTapSDK.Native;
#endif

namespace CleverTapSDK.Common {
    internal static class BindingFactory {
        private static CleverTapPlatformBindings cleverTapBinding;

        internal static CleverTapPlatformBindings CleverTapBinding { get => cleverTapBinding; }

        static BindingFactory() {
            #if UNITY_ANDROID && !UNITY_EDITOR
            cleverTapBinding = new AndroidPlatformBinding();
            #elif UNITY_IOS && !UNITY_EDITOR
            cleverTapBinding = new IOSPlatformBinding();
            #else
            cleverTapBinding = new UnityNativePlatformBinding();
            #endif
        }
    }
}
