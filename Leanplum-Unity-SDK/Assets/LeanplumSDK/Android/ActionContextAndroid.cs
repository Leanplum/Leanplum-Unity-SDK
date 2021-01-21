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
        public override string Name { get; }

        internal ActionContextAndroid(string key)
        {
            Name = key;
            nativeHandle = new AndroidJavaClass("com.leanplum.UnityActionContextBridge");
        }

        public override void Track(string eventName, double value, IDictionary<string, object> param)
        {
            var paramJson = param != null ? Json.Serialize(param) : "";
            nativeHandle.CallStatic("track", Name, eventName, value, paramJson);
        }

        public override void TrackMessageEvent(string eventName, double value, string info, IDictionary<string, object> param)
        {
            var paramJson = param != null ? Json.Serialize(param) : "";
            nativeHandle.CallStatic("trackMessageEvent", Name, eventName, value, info, paramJson);
        }

        public override void RunActionNamed(string name)
        {
            nativeHandle.CallStatic("runActionNamed", Name, name);
        }

        public override void RunTrackedActionNamed(string name)
        {
            nativeHandle.CallStatic("runTrackedActionNamed", Name, name);
        }

        public override string GetStringNamed(string name)
        {
            return nativeHandle.CallStatic<string>("getStringNamed", Name, name);
        }

        public override bool? GetBooleanNamed(string name)
        {
            return nativeHandle.CallStatic<bool>("getBooleanNamed", Name, name);
        }

        public override T GetObjectNamed<T>(string name)
        {
            Type t = typeof(T);
            if (t == typeof(IDictionary))
            {
                var json = nativeHandle.CallStatic<string>("getObjectNamed", Name, name);
                if (json != null)
                {
                    return (T) Json.Deserialize(json);
                }
            }
            else if (t == typeof(IList))
            {
                var json = nativeHandle.CallStatic<string>("getObjectNamed", Name, name);
                if (json != null)
                {
                    return (T) Json.Deserialize(json);
                }
            }

            return default(T);
        }

        public override T GetNumberNamed<T>(string name)
        {
            Type t = typeof(T);
            if (t == typeof(int))
            {
                return (T) (object) nativeHandle.CallStatic<int>("getIntNamed", Name, name);
            }
            else if (t == typeof(double))
            {
                return (T) (object) nativeHandle.CallStatic<double>("getDoubleNamed", Name, name);
            }
            else if (t == typeof(float))
            {
                return (T) (object) nativeHandle.CallStatic<float>("getFloatNamed", Name, name);
            }
            else if (t == typeof(long))
            {
                return (T) (object) nativeHandle.CallStatic<long>("getLongNamed", Name, name);
            }
            else if (t == typeof(short))
            {
                return (T) (object) nativeHandle.CallStatic<short>("getShortNamed", Name, name);
            }
            else if (t == typeof(byte))
            {
                return (T) (object) nativeHandle.CallStatic<byte>("getByteNamed", Name, name);
            }
            return default(T);
        }

        public override void MuteForFutureMessagesOfSameKind()
        {
            nativeHandle.CallStatic("muteFutureMessagesOfSameKind", Name);
        }
    }
}
#endif