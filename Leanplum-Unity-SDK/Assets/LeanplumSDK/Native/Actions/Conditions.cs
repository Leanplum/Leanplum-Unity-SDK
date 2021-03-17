﻿using System.Collections.Generic;
using System.Linq;

namespace LeanplumSDK
{
    // TODO: Verify conditions matching

    // TODO: Values Matching is case insensitive
    internal class Condition : ICondition
    {
        // Handles triggers and changes
        public string Subject { get; set; }
        public string Noun { get; set; }
        public string Verb { get; set; }

        internal Condition()
        {
        }

        internal Condition(ICondition condition) : base()
        {
            Subject = condition.Subject;
            Noun = condition.Noun;
            Verb = condition.Verb;
        }

        public virtual bool IsMatch(Trigger trigger)
        {
            bool isMatch = true;
            isMatch = isMatch && trigger.ActionTrigger.Value.Contains(Subject);

            // Evaluate even if Noun or EventName is null
            // EventName will be null for start/resume triggers and message View events
            // Noun is also not present (null) in the whenTrigger for start/resume
            isMatch = isMatch && Noun == trigger.EventName;

            return isMatch;
        }
    }

    internal class TriggersWithParameterCondition : Condition
    {
        internal static readonly string Name = "triggersWithParameter";
        internal static readonly int Operands = 2;

        internal string Param { get; set; }
        internal string Value { get; set; }

        internal TriggersWithParameterCondition()
        { }

        internal TriggersWithParameterCondition(ICondition condition, IList<object> objects) : base(condition)
        {
            if (objects != null && objects.Count == Operands)
            {
                Param = objects[0]?.ToString();
                Value = objects[1]?.ToString();
            }
        }

        public override bool IsMatch(Trigger trigger)
        {
            if (trigger.Params == null)
                return false;

            bool isMatch = base.IsMatch(trigger);

            var val = Util.GetValueOrDefault(trigger.Params, Param);
            if (val != null && Value != null)
            {
                isMatch = isMatch && val.ToString().ToLowerInvariant().Equals(Value.ToLowerInvariant());
            }
            else
            {
                isMatch = false;
            }

            return isMatch;
        }
    }

    internal class ChangesToCondition : Condition
    {
        internal static readonly string Name = "changesTo";
        internal static readonly int Operands = 1;

        internal string Value { get; set; }

        internal ChangesToCondition(ICondition condition, IList<object> objects) : base(condition)
        {
            if (objects != null && objects.Count == Operands)
            {
                Value = objects[0]?.ToString();
            }
        }

        public override bool IsMatch(Trigger trigger)
        {
            bool isMatch = base.IsMatch(trigger);

            isMatch = isMatch && Value == trigger.UserAttributeValue;

            return isMatch;
        }
    }

    internal class ChangesFromToCondition : Condition
    {
        internal static readonly string Name = "changesFromTo";
        internal static readonly int Operands = 2;

        internal string Value { get; set; }

        internal string PreviousValue { get; set; }

        internal ChangesFromToCondition(ICondition condition, IList<object> objects) : base(condition)
        {
            if (objects != null && objects.Count == Operands)
            {
                PreviousValue = objects[0]?.ToString();
                Value = objects[1]?.ToString();
            }
        }

        public override bool IsMatch(Trigger trigger)
        {
            bool isMatch = base.IsMatch(trigger);

            if (Value == trigger.UserAttributeValue && PreviousValue == trigger.UserAttributePreviousValue)
            {
                isMatch = isMatch && true;
            }
            else
            {
                isMatch = false;
            }

            return isMatch;
        }
    }
}