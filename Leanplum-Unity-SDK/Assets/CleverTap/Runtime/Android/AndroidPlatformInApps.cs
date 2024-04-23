#if UNITY_ANDROID
using CleverTapSDK.Common;

namespace CleverTapSDK.Android {
    internal class AndroidPlatformInApps : CleverTapPlatformInApps {
        private string METHOD_CLEAR_INAPP_RESOURCES = "clearInAppResources";
        internal override void FetchInApps(int callbackId) =>
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("fetchInApps", callbackId);
        
        internal override void ClearInAppResources(bool expiredOnly) =>
            CleverTapAndroidJNI.CleverTapJNIInstance.Call(METHOD_CLEAR_INAPP_RESOURCES,expiredOnly);
        
   }
}
#endif