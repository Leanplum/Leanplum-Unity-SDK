#if UNITY_ANDROID && !UNITY_EDITOR
using CleverTapSDK.Android;
#elif UNITY_IOS && !UNITY_EDITOR
using CleverTapSDK.IOS;
#else
using CleverTapSDK.Native;
using CleverTapSDK.Utilities;
#endif

namespace CleverTapSDK.Common
{
    internal static class VariableFactory
    {
        private static CleverTapPlatformVariable cleverTapVariable;
        internal static CleverTapPlatformVariable CleverTapVariable { get => cleverTapVariable; }

        static VariableFactory()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            cleverTapVariable = new AndroidPlatformVariable();
#elif UNITY_IOS && !UNITY_EDITOR
            cleverTapVariable = new IOSPlatformVariable();
#else
            cleverTapVariable = new UnityNativePlatformVariable();
#endif
        }
    }
}
