#if UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using LeanplumSDK.MiniJSON;
using UnityEngine;

namespace LeanplumSDK
{
    public class ActionContextAndroid : ActionContext
    {
        private AndroidJavaClass nativeHandle = null;
        internal override string Key { get; }
        public override string Id { get; }
        public override string Name { get; }

        internal ActionContextAndroid(string key, string actionName, string messageId)
        {
            Key = key;
            Id = messageId;
            Name = actionName;
            nativeHandle = new AndroidJavaClass("com.leanplum.UnityActionContextBridge");
        }

        public override void Track(string eventName, double value, IDictionary<string, object> param)
        {
            var paramJson = param != null ? Json.Serialize(param) : "";
            nativeHandle.CallStatic("track", Key, eventName, value, paramJson);
        }

        public override void TrackMessageEvent(string eventName, double value, string info, IDictionary<string, object> param)
        {
            var paramJson = param != null ? Json.Serialize(param) : "";
            nativeHandle.CallStatic("trackMessageEvent", Key, eventName, value, info, paramJson);
        }

        public override void RunActionNamed(string name)
        {
            nativeHandle.CallStatic("runActionNamed", Key, name);
        }

        public override void RunTrackedActionNamed(string name)
        {
            nativeHandle.CallStatic("runTrackedActionNamed", Key, name);
        }

        public override string GetStringNamed(string name)
        {
            return nativeHandle.CallStatic<string>("getStringNamed", Key, name);
        }

        public override bool? GetBooleanNamed(string name)
        {
            return nativeHandle.CallStatic<bool>("getBooleanNamed", Key, name);
        }

        public override T GetObjectNamed<T>(string name)
        {
            try
            {
                var json = nativeHandle.CallStatic<string>("getObjectNamed", Name, name);
                if (json != null) 
                {
                    var value = Json.Deserialize(json);
                    return Util.ConvertCollectionToType<T>(value);
                }
            }
            catch
            {
                UnityEngine.Debug.Log($"Leanplum: Error getting object value for name: {name}. Exception: {ex.Message}");
            }

            return default;
        }

        public override Color GetColorNamed(string name)
        {
            int intColor = GetNumberNamed<int>(name);
            return Util.IntToColor(intColor);
        }

        public override string GetFile(string name)
        {
            return nativeHandle.CallStatic<string>("getFileNamed", Key, name);
        }

        public override T GetNumberNamed<T>(string name)
        {
            Type t = typeof(T);
            if (t == typeof(int))
            {
                return (T) (object) nativeHandle.CallStatic<int>("getIntNamed", Key, name);
            }
            else if (t == typeof(double))
            {
                return (T) (object) nativeHandle.CallStatic<double>("getDoubleNamed", Key, name);
            }
            else if (t == typeof(float))
            {
                return (T) (object) nativeHandle.CallStatic<float>("getFloatNamed", Key, name);
            }
            else if (t == typeof(long))
            {
                return (T) (object) nativeHandle.CallStatic<long>("getLongNamed", Key, name);
            }
            else if (t == typeof(short))
            {
                return (T) (object) nativeHandle.CallStatic<short>("getShortNamed", Key, name);
            }
            else if (t == typeof(byte))
            {
                return (T) (object) nativeHandle.CallStatic<byte>("getByteNamed", Key, name);
            }
            return default(T);
        }

        public override void Dismissed()
        {
            nativeHandle.CallStatic("dismiss", Key);
        }
    }
}
#endif