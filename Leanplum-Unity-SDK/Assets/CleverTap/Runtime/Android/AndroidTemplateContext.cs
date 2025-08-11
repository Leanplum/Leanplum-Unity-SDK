#if UNITY_ANDROID
using CleverTapSDK.Common;
using CleverTapSDK.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CleverTapSDK.Android {
    internal class AndroidTemplateContext : CleverTapTemplateContext
    {
        public AndroidTemplateContext(string templateName) : base(templateName) { }

        public override bool? GetBoolean(string name)
        {
            var booleanResult = CleverTapAndroidJNI.CleverTapJNIInstance.Call<AndroidJavaObject>("customTemplateGetBooleanArg", TemplateName, name);
            return booleanResult?.Call<bool>("booleanValue");
        }

        public override Dictionary<string, object> GetDictionary(string name)
        {
            string json = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("customTemplateGetDictionaryJsonArg", TemplateName, name);

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
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("customTemplateGetFileArg", TemplateName, name);
        }

        public override byte? GetByte(string name)
        {
            return (byte?) GetDouble(name);
        }

        public override short? GetShort(string name)
        {
            return (short?) GetDouble(name);
        }

        public override int? GetInt(string name)
        {
            return (int?) GetDouble(name);
        }

        public override long? GetLong(string name)
        {
            return (long?) GetDouble(name);
        }

        public override float? GetFloat(string name)
        {
            return (float?) GetDouble(name);
        }

        public override double? GetDouble(string name)
        {
            var doubleResult = CleverTapAndroidJNI.CleverTapJNIInstance.Call<AndroidJavaObject>("customTemplateGetDoubleArg", TemplateName, name);
            return doubleResult?.Call<double>("doubleValue");
        }

        public override string GetString(string name)
        {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("customTemplateGetStringArg", TemplateName, name);
        }

        public override void SetDismissed()
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("customTemplateSetDismissed", TemplateName);
        }

        public override void SetPresented()
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("customTemplateSetPresented", TemplateName);
        }

        public override void TriggerAction(string name)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("customTemplateTriggerAction", TemplateName, name);
        }

        internal override string GetTemplateString()
        {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("customTemplateContextToString", TemplateName);
        }
    }
}
#endif