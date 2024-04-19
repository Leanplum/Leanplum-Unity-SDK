#if UNITY_ANDROID
using CleverTapSDK.Android;
#elif UNITY_IOS
using CleverTapSDK.IOS;
#else
using CleverTapSDK.Native;
#endif

namespace CleverTapSDK.Common {
    internal static class InAppsFactory {
        private static CleverTapPlatformInApps cleverTapInApps;

        internal static CleverTapPlatformInApps CleverTapInApps { get => cleverTapInApps; }

        static InAppsFactory() {
#if UNITY_ANDROID
            cleverTapInApps = new AndroidPlatformInApps();
#elif UNITY_IOS
            cleverTapInApps = new IOSPlatformInApps();
#else
            cleverTapInApps = new UnityNativePlatformInApps();
#endif
        }
    }
}
