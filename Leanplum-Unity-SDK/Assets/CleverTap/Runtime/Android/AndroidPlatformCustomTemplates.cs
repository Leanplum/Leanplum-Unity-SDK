#if UNITY_ANDROID
using CleverTapSDK.Common;

namespace CleverTapSDK.Android {
    internal class AndroidPlatformCustomTemplates : CleverTapPlatformCustomTemplates
    {
        internal override CleverTapTemplateContext CreateContext(string name)
        {
            return new AndroidTemplateContext(name);
        }

        internal override void SyncCustomTemplates()
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("syncCustomTemplates");
        }

        internal override void SyncCustomTemplates(bool isProduction)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("syncCustomTemplates");
        }
    }
}
#endif
