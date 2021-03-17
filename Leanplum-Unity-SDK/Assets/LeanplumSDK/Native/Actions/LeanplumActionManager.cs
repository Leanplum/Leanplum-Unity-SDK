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

        internal static void MaybePerformActions(ActionTrigger actionTrigger)
        {
            MaybePerformActions(new Trigger(actionTrigger));
        }

        internal static void MaybePerformActions(Trigger trigger)
        {
            if (!ShouldPerformActions)
                return;

            bool isStartOrResume = trigger.ActionTrigger.Equals(ActionTrigger.StartOrResume) || trigger.ActionTrigger.Equals(ActionTrigger.Resume);
            if (trigger == null ||
                (string.IsNullOrWhiteSpace(trigger.EventName) && !isStartOrResume))
                return;

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
}