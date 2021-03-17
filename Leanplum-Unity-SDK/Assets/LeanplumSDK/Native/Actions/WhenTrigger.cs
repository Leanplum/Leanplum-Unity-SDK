using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanplumSDK
{
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
        internal bool IsMatch(Trigger trigger)
        {
            return Conditions.Any(c => c.IsMatch(trigger));
        }

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