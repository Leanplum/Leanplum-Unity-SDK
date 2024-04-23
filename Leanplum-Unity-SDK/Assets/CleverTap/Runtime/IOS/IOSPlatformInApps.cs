#if UNITY_IOS
using CleverTapSDK.Common;

namespace CleverTapSDK.IOS {
    internal class IOSPlatformInApps : CleverTapPlatformInApps {
        internal override void FetchInApps(int callbackId) =>
            IOSDllImport.CleverTap_fetchInApps(callbackId);

        internal override void ClearInAppResources(bool expiredOnly) =>
            IOSDllImport.CleverTap_clearInAppResources(expiredOnly);
    }
}
#endif
