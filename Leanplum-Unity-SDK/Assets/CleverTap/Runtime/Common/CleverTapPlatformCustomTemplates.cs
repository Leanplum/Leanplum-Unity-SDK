namespace CleverTapSDK.Common
{
    internal abstract class CleverTapPlatformCustomTemplates
    {
        internal abstract void SyncCustomTemplates();
        internal abstract void SyncCustomTemplates(bool isProduction);

        internal abstract CleverTapTemplateContext CreateContext(string name);
    }
}
