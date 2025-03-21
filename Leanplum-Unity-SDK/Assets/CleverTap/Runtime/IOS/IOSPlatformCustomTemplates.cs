#if UNITY_IOS
using CleverTapSDK.Common;

namespace CleverTapSDK.IOS
{
    internal class IOSPlatformCustomTemplates : CleverTapPlatformCustomTemplates
    {
        internal override CleverTapTemplateContext CreateContext(string name)
        {
            return new IOSTemplateContext(name);
        }

        internal override void SyncCustomTemplates()
        {
            IOSDllImport.CleverTap_syncCustomTemplates(false);
        }

        internal override void SyncCustomTemplates(bool isProduction)
        {
            IOSDllImport.CleverTap_syncCustomTemplates(isProduction);
        }
    }
}
#endif