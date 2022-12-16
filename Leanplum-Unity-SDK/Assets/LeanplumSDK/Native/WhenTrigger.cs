using System;
using System.Collections.Generic;

namespace LeanplumSDK
{
    internal class Condition
    {
        internal string Subject { get; set; }
        internal string Noun { get; set; }
    }

    /// <summary>
    ///     Supports simple triggers without Contextual Data such as event parameters or attribute values
    /// </summary>
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
}