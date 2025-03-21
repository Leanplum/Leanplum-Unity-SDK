#if UNITY_ANDROID
using CleverTapSDK.Android;
#elif UNITY_IOS
using CleverTapSDK.IOS;
#else
using CleverTapSDK.Native;
#endif

namespace CleverTapSDK.Common
{
    internal static class CustomTemplatesFactory
    {
        private static CleverTapPlatformCustomTemplates cleverTapCustomTemplates;

        internal static CleverTapPlatformCustomTemplates CleverTapCustomTemplates { get => cleverTapCustomTemplates; }

        static CustomTemplatesFactory()
        {
#if UNITY_ANDROID
            cleverTapCustomTemplates = new AndroidPlatformCustomTemplates();
#elif UNITY_IOS
            cleverTapCustomTemplates = new IOSPlatformCustomTemplates();
#else
            cleverTapCustomTemplates = new UnityNativePlatformCustomTemplates();
#endif
        }
    }
}
