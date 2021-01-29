using System;
using System.Collections;
using System.Collections.Generic;

namespace LeanplumSDK
{
    internal abstract class ActionArg
    {
        protected string name;
        protected string kind;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Kind
        {
            get
            {
                return kind;
            }
        }

        public abstract object Value
        {
            get;
        }

        public abstract Dictionary<string, object> ToDictionary();
    }

    internal class ActionArg<T> : ActionArg
    {
        protected T defaultValue;

        public T DefaultValue
        {
            get
            {
                return defaultValue;
            }
        }

        public override object Value => defaultValue;

        private static ActionArg<T> ArgumentNamed(string name, T defaultValue, string kind)
        {
            var argument = new ActionArg<T>
            {
                name = name,
                defaultValue = defaultValue,
                kind = kind
            };

            return argument;
        }

        public static ActionArg<T> ArgumentNamed(string name, T defaultValue)
        {
            return ArgumentNamed(name, defaultValue, VarCache.KindFromValue(defaultValue));
        }

        public static ActionArg<T> ColorArgumentNamed(string name, T defaultValue)
        {
            return ArgumentNamed(name, defaultValue, Constants.Kinds.COLOR);
        }

        public static ActionArg<T> FileArgumentNamed(string name, T defaultValue)
        {
            return ArgumentNamed(name, defaultValue, Constants.Kinds.FILE);
        }

        public static ActionArg<T> ActionArgumentNamed(string name, T defaultValue)
        {
            return ArgumentNamed(name, defaultValue, Constants.Kinds.ACTION);
        }

        public override Dictionary<string, object> ToDictionary ()
        {
            var dictionary = new Dictionary<string, object>
            {
                { "name", name },
                { "kind", kind },
                { "defaultValue", defaultValue }
            };

            return dictionary;
        }
    }

    public class ActionArgs
    {
        private List<ActionArg> args = new List<ActionArg>();

        public ActionArgs With<T>(string name, T defaultValue)
        {
            if (name == null)
            {
                return this;
            }
            args.Add(ActionArg<T>.ArgumentNamed(name, defaultValue));
            return this;
        }

        public ActionArgs WithColor(string name, UnityEngine.Color defaultValue)
        {
            if (name == null)
            {
                return this;
            }

            long value = Util.ColorToInt(defaultValue);
            args.Add(ActionArg<long>.ColorArgumentNamed(name, value));
            return this;
        }

        public ActionArgs WithFile(string name)
        {
            if (name == null)
            {
                return this;
            }
            args.Add(ActionArg<string>.FileArgumentNamed(name, string.Empty));
            return this;
        }

        public ActionArgs WithAction<T>(string name, T defaultValue)
        {
            if (name == null)
            {
                return this;
            }
            args.Add(ActionArg<T>.ActionArgumentNamed(name, defaultValue));
            return this;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var arg in args)
            {
                var parts = arg.Name.Split('.');
                if (parts.Length > 1)
                {
                    if (!dict.ContainsKey(parts[0]))
                    {
                        dict[parts[0]] = new Dictionary<string, object>();
                    }
                    var component = dict[parts[0]] as Dictionary<string, object>;
                    if (component != null)
                    {
                        component[parts[1]] = arg.Value;
                    }
                }
                else
                {
                    dict[arg.Name] = arg.Value;
                }
            }

            return dict;
        }


        public string ToJSON()
        {
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            foreach(var arg in args)
            {
                var dict = arg.ToDictionary();
                data.Add(dict);
            }

            return MiniJSON.Json.Serialize(data);
        }
    }
}


