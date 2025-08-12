#if UNITY_ANDROID && !UNITY_EDITOR
using CleverTapSDK.Android;
#elif UNITY_IOS && !UNITY_EDITOR
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
#if UNITY_ANDROID && !UNITY_EDITOR
            cleverTapCustomTemplates = new AndroidPlatformCustomTemplates();
#elif UNITY_IOS && !UNITY_EDITOR
            cleverTapCustomTemplates = new IOSPlatformCustomTemplates();
#else
            cleverTapCustomTemplates = new UnityNativePlatformCustomTemplates();
#endif
        }
    }
}
