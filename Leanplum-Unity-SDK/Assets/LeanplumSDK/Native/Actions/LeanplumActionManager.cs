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

        internal static void MaybePerformActions(ActionTrigger actionTrigger, string eventName, IDictionary<string, object> parameters)
        {
            if (!ShouldPerformActions)
                return;

            // TODO: change initialization in trigger track/advance etc.
            var contextualData = new LeanplumContextualData
            {
                Params = parameters
            };
            Trigger trigger = new Trigger()
            {
                ActionTrigger = actionTrigger,
                EventName = eventName,
                ContextualData = contextualData
            };

            var condition = VarCache.Messages.Select(WhenTrigger.FromKV)
                .OrderBy(w => w.Priority)
                .FirstOrDefault(w => w.Conditions.Any(x => x.IsMatch(trigger)));

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

            /*
             "whenTriggers": {
                        "children": [
                            {
                                "subject": "state",
                                "objects": [
                                    "param",
                                    "notnull"
                                ],
                                "verb": "triggersWithParameter",
                                "noun": "stateWithParameter",
                                "secondaryVerb": "="
                            }
                        ],
                        "verb": "OR"
                    },
            */

            internal int Priority { get; set; }
            internal string Id { get; set; }
            internal List<ICondition> Conditions { get; set; }

            private WhenTrigger()
            {
                Conditions = new List<ICondition>();
            }

            // TODO: bool isMatch? // depending on AND / OR between the conditions

            //TODO: Move strings to constants
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
                                    childDict.TryGetValue("verb", out object verb);


                                    ICondition condition = new Condition()
                                    {
                                        Subject = subject?.ToString(),
                                        Noun = noun?.ToString(),
                                        Verb = verb?.ToString()
                                    };

                                    var objects = Util.GetValueOrDefault(childDict, "objects") as IList<object>;

                                    string verbStr = verb?.ToString();
                                    if (verbStr == TriggersWithParameterCondition.Name)
                                    {
                                        condition = new TriggersWithParameterCondition(condition, objects);
                                    }
                                    if (verbStr == ChangesToCondition.Name)
                                    {
                                        condition = new ChangesToCondition(condition, objects);
                                    }
                                    if (verbStr == ChangesFromToCondition.Name)
                                    {
                                        condition = new ChangesFromToCondition(condition, objects);
                                    }

                                    whenCon.Conditions.Add(condition);
                                }
                            }
                        }
                    }
                    return whenCon;
                };

        }
}