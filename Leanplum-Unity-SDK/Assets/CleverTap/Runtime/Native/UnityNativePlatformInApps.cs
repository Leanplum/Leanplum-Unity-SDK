#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using CleverTapSDK.Common;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native {
    internal class UnityNativePlatformInApps : CleverTapPlatformInApps {
        internal override void FetchInApps(int callbackId) =>
            CleverTapLogger.LogError("CleverTap Error: FetchInApps is not supported for this platform.");

        internal override void ClearInAppResources(bool expiredOnly) =>
            CleverTapLogger.LogError("CleverTap Error: ClearInAppResources is not supported for this platform.");
    }
}
#endif
