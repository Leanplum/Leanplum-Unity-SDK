#if UNITY_IPHONE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK.Apple
{
    public class ActionContextApple : ActionContext
    {
        [DllImport("__Internal")]
        internal static extern string get_action_name(string id);

        [DllImport("__Internal")]
        internal static extern string get_string_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern int get_int_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern float get_float_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern double get_double_named(string id, string name);
        
        [DllImport("__Internal")]
        internal static extern long get_long_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern byte get_byte_named(string id, string name);
        
        [DllImport("__Internal")]
        internal static extern short get_short_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern bool get_bool_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern long get_color_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern string get_file_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern string get_dictionary_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern string get_array_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern string get_html_with_template_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern string get_object_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern void run_action_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern void run_tracked_action_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern void track_event(string id, string eventName, double value, string parameters);

        [DllImport("__Internal")]
        internal static extern void track_message_event(string id, string eventName, double value, string info, string parameters);

        [DllImport("__Internal")]
        internal static extern void dismiss(string id);

        internal override string Key { get; }
        public override string Id { get; }
        public override string Name { get; }

        internal ActionContextApple(string key, string actionName, string messageId)
        {
            Key = key;
            Id = messageId;
            Name = actionName;
        }

        public override void TrackMessageEvent(string eventName, double value, string info, IDictionary<string, object> param)
        {
            var parameters = param != null ? Json.Serialize(param) : "";
            track_message_event(Key, eventName, value, info, parameters);
        }

        public override void Track(string eventName, double value, IDictionary<string, object> param)
        {
            var parameters = param != null ? Json.Serialize(param) : "";
            track_event(Key, eventName, value, parameters);
        }

        public override void RunActionNamed(string name)
        {
            run_action_named(Key, name);
        }

        public override void RunTrackedActionNamed(string name)
        {
            run_tracked_action_named(Key, name);
        }

        public override string GetStringNamed(string name)
        {
            return get_string_named(Key, name);
        }

        public override bool? GetBooleanNamed(string name)
        {
            return get_bool_named(Key, name);
        }

        public override T GetObjectNamed<T>(string name)
        {
            Type returnType = typeof(T);
            string json;
            if (typeof(IDictionary).IsAssignableFrom(returnType))
            {
                json = get_dictionary_named(Key, name);
            }
            else if (typeof(IList).IsAssignableFrom(returnType))
            {
                json = get_array_named(Key, name);
            }
            else
            {
                json = get_object_named(Key, name);
            }

            try
            {
                if (json != null)
                {
                    var value = Json.Deserialize(json);
                    // Defaults to (T)value if not a collection
                    return Util.ConvertCollectionToType<T>(value);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log($"Leanplum: Error getting object value for name: {name}. Exception: {ex.Message}");
            }

            return default;
        }

        public override UnityEngine.Color GetColorNamed(string name)
        {
            long color = get_color_named(Key, name);
            return Util.IntToColor(color);
        }

        public override string GetFile(string name)
        {
            return get_file_named(Key, name);
        }

        public override T GetNumberNamed<T>(string name)
        {
            Type t = typeof(T);
            if (t == typeof(int))
            {
                return (T) (object) get_int_named(Key, name);
            }
            else if (t == typeof(double))
            {
                return (T) (object) get_double_named(Key, name);
            }
            else if (t == typeof(float))
            {
                return (T) (object) get_float_named(Key, name);
            }
            else if (t == typeof(long))
            {
                return (T) (object) get_long_named(Key, name);
            }
            else if (t == typeof(short))
            {
                return (T) (object) get_string_named(Key, name);
            }
            else if (t == typeof(byte))
            {
                return (T) (object) get_byte_named(Key, name);
            }

            return default(T);
        }

        public override void Dismissed()
        {
            dismiss(Key);
        }

        public override string ToString()
        {
            return $"{Key}";
        }
    }
}
#endif