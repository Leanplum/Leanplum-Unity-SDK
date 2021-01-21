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
        internal static extern string get_dictionary_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern string get_array_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern string get_html_with_template_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern void run_action_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern void run_tracked_action_named(string id, string name);

        [DllImport("__Internal")]
        internal static extern void track_event(string id, string eventName, double value, string parameters);

        [DllImport("__Internal")]
        internal static extern void track_message_event(string id, string eventName, double value, string info, string parameters);

        [DllImport("__Internal")]
        internal static extern void mute_future_messages_of_same_kind(string id);
        
        public override string Name { get; }

        internal ActionContextApple(string name)
        {
            Name = name;
        }

        public override void TrackMessageEvent(string eventName, double value, string info, IDictionary<string, object> param)
        {
            var parameters = param != null ? Json.Serialize(param) : "";
            track_message_event(Name, eventName, value, info, parameters);
        }

        public override void Track(string eventName, double value, IDictionary<string, object> param)
        {
            var parameters = param != null ? Json.Serialize(param) : "";
            track_event(Name, eventName, value, parameters);
        }

        public override void RunActionNamed(string name)
        {
            run_action_named(Name, name);
        }

        public override void RunTrackedActionNamed(string name)
        {
            run_tracked_action_named(Name, name);
        }

        public override string GetStringNamed(string name)
        {
            return get_string_named(Name, name);
        }

        public override bool? GetBooleanNamed(string name)
        {
            return get_bool_named(Name, name);
        }

        public override T GetObjectNamed<T>(string name)
        {
            Type t = typeof(T);
            if (t == typeof(IDictionary))
            {
                var json = get_dictionary_named(Name, name);
                if (json != null)
                {
                    return (T) Json.Deserialize(json);
                }
            }
            else if (t == typeof(IList))
            {
                var json = get_array_named(Name, name);
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
                return (T) (object) get_int_named(Name, name);
            }
            else if (t == typeof(double))
            {
                return (T) (object) get_double_named(Name, name);
            }
            else if (t == typeof(float))
            {
                return (T) (object) get_float_named(Name, name);
            }
            else if (t == typeof(long))
            {
                return (T) (object) get_long_named(Name, name);
            }
            else if (t == typeof(short))
            {
                return (T) (object) get_string_named(Name, name);
            }
            else if (t == typeof(byte))
            {
                return (T) (object) get_byte_named(Name, name);
            }

            return default(T);
        }

        public override void MuteForFutureMessagesOfSameKind()
        {
            mute_future_messages_of_same_kind(Name);
        }
    }
}
#endif