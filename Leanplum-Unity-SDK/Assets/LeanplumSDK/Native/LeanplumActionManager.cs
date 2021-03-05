using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanplumSDK
{
    public class LeanplumActionManager
    {
        internal static bool ShouldPerformActions { get; set; }

        internal LeanplumActionManager()
        {
        }

        internal static void MaybePerformActions(ActionTrigger actionTrigger, string eventName = null)
        {
            if (!ShouldPerformActions)
                return;

            var condition = VarCache.Messages.Select<KeyValuePair<string, object>, WhenTrigger>(WhenTrigger.FromKV)
                .OrderBy(w => w.Priority)
                .FirstOrDefault(w => w.Conditions.Any(x => actionTrigger.Value.Contains(x.Subject) && x.Noun == eventName));

            if (condition != null)
            {
                var msg = Util.GetValueOrDefault(VarCache.Messages, condition.Id) as IDictionary<string, object>;
                TriggerAction(condition.Id, msg);
            }
        }

        internal static void TriggerPreview(IDictionary<string, object> packetData)
        {
            var actionData = Util.GetValueOrDefault(packetData, Constants.Args.ACTION) as IDictionary<string, object>;
            if (actionData != null)
            {
                string actionName = Util.GetValueOrDefault(actionData, Constants.Args.ACTION_NAME)?.ToString();
                string messageId = Util.GetValueOrDefault(packetData, Constants.Args.MESSAGE_ID)?.ToString();
                LeanplumNative.CompatibilityLayer.Log($"Preview of {actionName} Message with Id: {messageId}");
                if (!string.IsNullOrWhiteSpace(actionName))
                {
                    var newVars = VarCache.MergeMessage(actionData);
                    NativeActionContext ac = new NativeActionContext(messageId, actionName, newVars);
                    TriggerAction(ac, newVars);
                }
            }
        }

        internal static void TriggerAction(string id, IDictionary<string, object> message)
        {
            if (!ShouldPerformActions)
                return;

            string actionName = Util.GetValueOrDefault(message, Constants.Args.ACTION) as string;
            IDictionary<string, object> vars = Util.GetValueOrDefault(message, Constants.Args.VARS) as IDictionary<string, object>;
            if (!string.IsNullOrEmpty(actionName) && vars != null)
            {
                NativeActionContext actionContext = new NativeActionContext(id, actionName, vars);
                TriggerAction(actionContext, message);
            }
        }

        internal static void TriggerAction(NativeActionContext context, IDictionary<string, object> messageConfig)
        {
            if (!ShouldPerformActions)
                return;

            ActionDefinition actionDefinition;

            if (!VarCache.actionDefinitions.TryGetValue(context.Name, out actionDefinition))
            {
                // If no matching action definition is found, use the Generic one if such is registered 
                if (VarCache.actionDefinitions.TryGetValue(Constants.Args.GENERIC_DEFINITION_NAME, out actionDefinition))
                {
                    IDictionary<string, object> args = new Dictionary<string, object>();
                    args.Add(Constants.Args.GENERIC_DEFINITION_CONFIG, messageConfig);
                    var genericActionContext = new NativeActionContext(context.Id, Constants.Args.GENERIC_DEFINITION_NAME, args);
                    context = genericActionContext;
                }
            }

            if (actionDefinition != null)
            {
                if (!string.IsNullOrEmpty(context.Id))
                {
                    // The View event is tracked using the messageId only, without an event name
                    context.TrackMessageEvent(null, 0, null, null);
                }
                actionDefinition.Responder?.Invoke(context);
            }
        }

        internal class WhenTrigger
        {
            /*
              "whenTriggers": {
                            "children": [
                                {
                                    "subject": "start",
                                    "objects": [],
                                    "verb": "",
                                    "secondaryVerb": "="
                                }
                            ],
                            "verb": "OR"
                        },
            */

            /*
              "whenTriggers": {
                            "children": [
                                {
                                    "subject": "event",
                                    "objects": [],
                                    "verb": "triggers",
                                    "noun": "myEvent",
                                    "secondaryVerb": "="
                                }
                            ],
                            "verb": "OR"
                        },
            */

            internal int Priority { get; set; }
            internal string Id { get; set; }
            internal List<Condition> Conditions { get; set; }

            private WhenTrigger()
            {
                Conditions = new List<Condition>();
            }

            internal static Func<KeyValuePair<string, object>, WhenTrigger> FromKV
                => (x) =>
                {
                    WhenTrigger whenCon = new WhenTrigger();
                    whenCon.Id = x.Key;
                    var message = x.Value as IDictionary<string, object>;
                    if (message != null)
                    {
                        object priority = Util.GetValueOrDefault(message, "priority", "1000");
                        whenCon.Priority = int.Parse(priority.ToString());
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
                                    whenCon.Conditions.Add(new Condition()
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

        internal class Condition
        {
            internal string Subject { get; set; }
            internal string Noun { get; set; }
        }
    }

    internal struct ActionTrigger
    {
        private ActionTrigger(string[] value) { Value = value; }

        internal string[] Value { get; set; }

        internal static ActionTrigger StartOrResume { get { return new ActionTrigger(new string[] { "start", "resume" }); } }
        internal static ActionTrigger Resume { get { return new ActionTrigger(new string[] { "resume" }); } }
        internal static ActionTrigger Event { get { return new ActionTrigger(new string[] { "event" }); } }
        internal static ActionTrigger State { get { return new ActionTrigger(new string[] { "state" }); } }
        internal static ActionTrigger UserAttribute { get { return new ActionTrigger(new string[] { "userAttribute" }); } }
    }
}