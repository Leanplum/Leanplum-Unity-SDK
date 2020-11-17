using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanplumSDK
{
    public class LeanplumActionManager
    {
        internal LeanplumActionManager()
        {
        }

        internal static void MaybePerformActions(string[] whenConditions, string eventName = null)
        {
            var condition = VarCache.Messages.Select<KeyValuePair<string, object>, WhenCondition>(WhenCondition.FromKV)
                .OrderBy(w => w.Priority)
                .FirstOrDefault(w => w.WhenTriggers.Any(x => whenConditions.Contains(x.Subject) && x.Noun == eventName));

            if (condition != null)
            {
                var msg = VarCache.Messages[condition.Id] as IDictionary<string, object>;
                TriggerAction(condition.Id, msg);
            }
        }

        internal static void TriggerAction(string id, IDictionary<string, object> message)
        {
            string actionName = message["action"].ToString();
            IDictionary<string, object> vars = (IDictionary<string, object>)message["vars"];
            var actionContext = new NativeActionContext(id, actionName, vars);

            TriggerAction(actionContext, message);
        }

        internal static void TriggerAction(NativeActionContext context, IDictionary<string, object> messageConfig)
        {
            ActionDefinition actionDefinition;

            if (!VarCache.unityActionDefinitions.TryGetValue(context.Name, out actionDefinition))
            {
                if (VarCache.unityActionDefinitions.TryGetValue("Generic", out actionDefinition))
                {
                    IDictionary<string, object> args = new Dictionary<string, object>();
                    args.Add("messageConfig", messageConfig);
                    var genericActionContext = new NativeActionContext(context.Id, "Generic", args);
                    context = genericActionContext;
                }
            }

            if (actionDefinition != null)
            {
                if (!string.IsNullOrEmpty(context.Id))
                {
                    context.TrackMessageEvent("View", 0, null, null);
                }
                actionDefinition.Responder?.Invoke(context);
            }
        }

        internal class WhenCondition
        {

            //"whenTriggers": {
            //                "children": [
            //                    {
            //                        "subject": "start",
            //                        "objects": [],
            //                        "verb": "",
            //                        "secondaryVerb": "="
            //                    }
            //                ],
            //                "verb": "OR"
            //            },

            //"whenTriggers": {
            //                "children": [
            //                    {
            //                        "subject": "event",
            //                        "objects": [],
            //                        "verb": "triggers",
            //                        "noun": "myEvent",
            //                        "secondaryVerb": "="
            //                    }
            //                ],
            //                "verb": "OR"
            //            },

            public int Priority { get; set; }
            public string Id { get; set; }
            public List<Condition> WhenTriggers { get; set; }

            public WhenCondition()
            {
                WhenTriggers = new List<Condition>();
            }

            public static Func<KeyValuePair<string, object>, WhenCondition> FromKV
                => (x) =>
                {
                    WhenCondition whenCon = new WhenCondition();
                    whenCon.Id = x.Key;
                    var message = x.Value as IDictionary<string, object>;
                    if (message != null)
                    {
                        whenCon.Priority = int.Parse(message["priority"].ToString());
                        var whenTriggers = Util.GetValueOrDefault(message, "whenTriggers") as IDictionary<string, object>;
                        if (whenTriggers != null)
                        {
                            var children = Util.GetValueOrDefault(whenTriggers, "children") as IList<object>;
                            if (children != null && children.Count > 0)
                            {
                                foreach (var child in children)
                                {
                                    var childDict = child as IDictionary<string, object>;
                                    childDict.TryGetValue("subject", out object subject);
                                    childDict.TryGetValue("noun", out object noun);
                                    whenCon.WhenTriggers.Add(new Condition()
                                    {
                                        Subject = subject?.ToString(),
                                        Noun = noun?.ToString(),
                                    });
                                }
                            }
                        }
                    }
                    return whenCon;
                };

        }

        public class Condition
        {
            public string Subject { get; set; }
            public string Noun { get; set; }
        }
    }
}