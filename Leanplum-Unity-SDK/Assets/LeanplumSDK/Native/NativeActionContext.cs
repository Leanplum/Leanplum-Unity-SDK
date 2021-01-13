using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanplumSDK
{
    public class NativeActionContext : ActionContext
    {
        private readonly string name;
        private readonly string id;
        private readonly IDictionary<string, object> vars;

        public NativeActionContext(string id, string name, IDictionary<string, object> vars)
        {
            this.id = id;
            this.name = name;
            this.vars = vars;
        }

        public override string Name => name;
        public string Id => id;

        public object Traverse(string name)
        {
            if (!name.Contains('.'))
            {
                return Util.GetValueOrDefault(vars, name);
            }

            string[] parts = name.Split('.');
            object components = vars;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                components = VarCache.Traverse(vars, parts[i], false);
            }
            return VarCache.Traverse(components, parts[parts.Length - 1], false);
        }

        public override bool? GetBooleanNamed(string name)
        {
            return Traverse(name) as bool?;
        }

        public override T GetNumberNamed<T>(string name)
        {
            var value = Traverse(name);

            if (value != null && Util.IsNumber(value))
            {
                Type t = typeof(T);
                try
                {
                    // Use the type TryParse method, i.e. int.TryParse, decimal.TryParse
                    Type[] argTypes = { typeof(string), t.MakeByRefType() };
                    var tryParse = t.GetMethod("TryParse", argTypes);
                    if (tryParse != null)
                    {
                        object[] parameters = new object[] { value.ToString(), null };
                        object result = tryParse.Invoke(null, parameters);
                        bool blResult = (bool)result;
                        if (blResult)
                        {
                            return (T)parameters[1];
                        }
                    }
                }
                catch (Exception ex)
                {
                    LeanplumNative.CompatibilityLayer.LogError($"Failed to parse number: {value}, with name: {name}. Exception: {ex.Message}");
                }
            }

            return default;
        }

        public override T GetObjectNamed<T>(string name)
        {
            return (T)Traverse(name);
        }

        public override string GetStringNamed(string name)
        {
            var value = Traverse(name);
            return value != null ? value.ToString() : string.Empty;
        }

        public override void MuteForFutureMessagesOfSameKind()
        {
            throw new NotImplementedException();
        }

        public override void RunActionNamed(string name)
        {
            IDictionary<object, object> action = Traverse(name) as IDictionary<object, object>;
            if (action != null)
            {
                var actionName = action[Constants.Args.ACTION_NAME];
                if (!string.IsNullOrEmpty(actionName?.ToString()))
                {
                    Dictionary<string, object> actions = action.ToDictionary(kv => kv.Key.ToString(),
                                                         kv => kv.Value);

                    if (actionName.Equals(Constants.Args.CHAIN_TO_EXISTING) && actions.ContainsKey(Constants.Args.CHAIN_MESSAGE))
                    {
                        string messageId = actions[Constants.Args.CHAIN_MESSAGE]?.ToString();
                        if (!Leanplum.ShowMessage(messageId))
                        {
                            // Try to fetch the chained message if not on the device
                            Leanplum.ForceContentUpdate(() =>
                            {
                                Leanplum.ShowMessage(messageId);
                            });
                        }
                    }
                    else
                    {
                        NativeActionContext actionContext = new NativeActionContext(null, actionName.ToString(), actions);
                        LeanplumActionManager.TriggerAction(actionContext, actions);
                    }
                }
            }
        }

        public override void RunTrackedActionNamed(string name)
        {
            RunActionNamed(name);
            TrackMessageEvent(name, 0, null, null);
        }

        public override void Track(string eventName, double value, IDictionary<string, object> param)
        {
            Leanplum.Track(eventName, value, null, param);
        }

        public override void TrackMessageEvent(string eventName, double value, string info, IDictionary<string, object> param)
        {
            if (LeanplumFactory.SDK is LeanplumNative)
            {
                // The action name comes as "{Name} action" and should be tracked as "{Name}"
                eventName = eventName?.Replace(" action", "");
                var args = new Dictionary<string, string>
                {
                    { Constants.Args.MESSAGE_ID, id }
                };
                (LeanplumFactory.SDK as LeanplumNative).Track(eventName, value, info, param, args);
            }
        }

        public static string GetFileURL(string fileName)
        {
            /*
               "fileAttributes": {
                  "myImage.jpg": {
                      "": {
                          "size": 89447,
                          "hash": null,
                          "servingUrl": "http://lh3.googleusercontent.com/aBc345dE...",
                          "url": "/resource/aBc345dE..."
                      }
                  },
            */
            IDictionary<string, object> file = Util.GetValueOrDefault(VarCache.FileAttributes, fileName) as IDictionary<string, object>;
            if (file != null)
            {
                IDictionary<string, object> fileAttributes = Util.GetValueOrDefault(file, string.Empty) as IDictionary<string, object>;
                if (fileAttributes != null)
                {
                    return Util.GetValueOrDefault(fileAttributes, "servingUrl") as string;
                }
            }

            return null;
        }
    }
}