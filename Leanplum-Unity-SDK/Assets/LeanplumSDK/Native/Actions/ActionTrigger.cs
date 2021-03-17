using System.Collections.Generic;

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
    }

    internal class Trigger
    {
        public ActionTrigger ActionTrigger { get; set; }
        public string EventName { get; set; }
        public IDictionary<string, object> Params { get; set; }
        public string UserAttributeValue { get; set; }
        private string userAttributePreviousValue;
        public string UserAttributePreviousValue
        {
            get
            {
                if (userAttributePreviousValue == null)
                {
                    // TODO: Read from VarCache
                    userAttributePreviousValue = string.Empty;
                }
                return userAttributePreviousValue;
            }

            set
            {
                userAttributePreviousValue = value;
            }
        }
    }
}