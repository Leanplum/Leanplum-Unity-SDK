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

        public ActionArgs WithColor<T>(string name, T defaultValue)
        {
            if (name == null)
            {
                return this;
            }
            args.Add(ActionArg<T>.ColorArgumentNamed(name, defaultValue));
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


