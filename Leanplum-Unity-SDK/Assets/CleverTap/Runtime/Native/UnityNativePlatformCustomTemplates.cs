#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using CleverTapSDK.Common;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native {
    internal class UnityNativePlatformCustomTemplates : CleverTapPlatformCustomTemplates {
        internal override CleverTapTemplateContext CreateContext(string name) {
            CleverTapLogger.LogError("CleverTap Error: CreateContext is not supported for this platform.");
            return null;
        }

        internal override void SyncCustomTemplates() =>
            CleverTapLogger.LogError("CleverTap Error: SyncCustomTemplates is not supported for this platform.");

        internal override void SyncCustomTemplates(bool isProduction) =>
            CleverTapLogger.LogError("CleverTap Error: SyncCustomTemplates(isProduction) is not supported for this platform.");
    }
}
#endif
