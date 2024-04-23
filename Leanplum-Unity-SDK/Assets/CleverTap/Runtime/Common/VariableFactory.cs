#if UNITY_ANDROID
using CleverTapSDK.Android;
#elif UNITY_IOS
using CleverTapSDK.IOS;
#else
using CleverTapSDK.Native;
#endif

namespace CleverTapSDK.Common {
    internal static class VariableFactory {
        private static CleverTapPlatformVariable cleverTapVariable;

        internal static CleverTapPlatformVariable CleverTapVariable { get => cleverTapVariable; }

        static VariableFactory() {
            #if UNITY_ANDROID
            cleverTapVariable = new AndroidPlatformVariable();
            #elif UNITY_IOS
            cleverTapVariable = new IOSPlatformVariable();
            #else
            cleverTapVariable = new UnityNativePlatformVariable();
            #endif
        }
    }
}
