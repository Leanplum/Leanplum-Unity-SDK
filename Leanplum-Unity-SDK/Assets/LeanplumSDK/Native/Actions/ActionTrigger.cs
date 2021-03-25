using System.Collections.Generic;
using System.Linq;

namespace LeanplumSDK
{
    internal struct ActionTrigger
    {
        private ActionTrigger(string[] value) { Value = value; }

        internal string[] Value { get; set; }

        internal static ActionTrigger StartOrResume { get { return new ActionTrigger(new string[] { "start", "resume" }); } }
        internal static ActionTrigger Resume { get { return new ActionTrigger(new string[] { "resume" }); } }
        internal static ActionTrigger Event { get { return new ActionTrigger(new string[] { "event" }); } }
        internal static ActionTrigger State { get { return new ActionTrigger(new string[] { "state" }); } }
        internal static ActionTrigger UserAttribute { get { return new ActionTrigger(new string[] { "userAttribute" }); } }

        internal bool Equals(ActionTrigger at)
        {
            return Value.SequenceEqual(at.Value);
        }
    }

    internal class Trigger
    {
        internal Trigger(ActionTrigger actionTrigger)
        {
            ActionTrigger = actionTrigger;
        }

        public ActionTrigger ActionTrigger { get; set; }
        public string EventName { get; set; }
        // Contextual Data
        public IDictionary<string, object> Params { get; set; }
        public object UserAttributeValue { get; set; }
        public object UserAttributePreviousValue { get; set; }
    }
}