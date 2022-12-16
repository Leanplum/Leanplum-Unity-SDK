namespace LeanplumSDK
{
    /// <summary>
    ///     Supports simple triggers without Contextual Data such as event parameters or attribute values
    /// </summary>
    internal class ActionTrigger
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