#if UNITY_IOS
using System;
using System.Collections.Generic;
using CleverTapSDK.Common;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.IOS
{
    internal class IOSTemplateContext : CleverTapTemplateContext
    {
        public IOSTemplateContext(string templateName) : base(templateName) { }

        public override void SetDismissed()
        {
            IOSDllImport.CleverTap_customTemplateSetDismissed(TemplateName);
        }

        public override void SetPresented()
        {
            IOSDllImport.CleverTap_customTemplateSetPresented(TemplateName);
        }

        public override string GetString(string name)
        {
            return IOSDllImport.CleverTap_customTemplateGetStringArg(TemplateName, name);
        }

        public override bool? GetBoolean(string name)
        {
            return IOSDllImport.CleverTap_customTemplateGetBooleanArg(TemplateName, name);
        }

        public override Dictionary<string, object> GetDictionary(string name)
        {
            string json = IOSDllImport.CleverTap_customTemplateGetDictionaryArg(TemplateName, name);

            try
            {
                if (json != null)
                {
                    var value = Json.Deserialize(json);
                    // Defaults to (T)value if not a collection
                    return value as Dictionary<string, object>;
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log($"CustomTemplates: Error getting dictionary value for name: {name}. Exception: {ex.Message}");
            }

            return default;
        }

        public override string GetFile(string name)
        {
            return IOSDllImport.CleverTap_customTemplateGetFileArg(TemplateName, name);
        }

        public override byte? GetByte(string name)
        {
            return IOSDllImport.CleverTap_customTemplateGetByteArg(TemplateName, name);
        }

        public override short? GetShort(string name)
        {
            return IOSDllImport.CleverTap_customTemplateGetShortArg(TemplateName, name);
        }

        public override int? GetInt(string name)
        {
            return IOSDllImport.CleverTap_customTemplateGetIntArg(TemplateName, name);
        }

        public override long? GetLong(string name)
        {
            return IOSDllImport.CleverTap_customTemplateGetLongArg(TemplateName, name);
        }

        public override float? GetFloat(string name)
        {
            return IOSDllImport.CleverTap_customTemplateGetFloatArg(TemplateName, name);
        }

        public override double? GetDouble(string name)
        {
            return IOSDllImport.CleverTap_customTemplateGetDoubleArg(TemplateName, name);
        }

        public override void TriggerAction(string name)
        {
            IOSDllImport.CleverTap_customTemplateTriggerAction(TemplateName, name);
        }

        internal override string GetTemplateString()
        {
            return IOSDllImport.CleverTap_customTemplateContextToString(TemplateName);
        }
    }
}
#endif

