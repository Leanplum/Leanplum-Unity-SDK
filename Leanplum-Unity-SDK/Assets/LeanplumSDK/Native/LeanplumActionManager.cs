//
// Copyright 2022, Leanplum, Inc.
//
//  Licensed to the Apache Software Foundation (ASF) under one
//  or more contributor license agreements.  See the NOTICE file
//  distributed with this work for additional information
//  regarding copyright ownership.  The ASF licenses this file
//  to you under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing,
//  software distributed under the License is distributed on an
//  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//  KIND, either express or implied.  See the License for the
//  specific language governing permissions and limitations
//  under the License.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LeanplumSDK
{
    public class LeanplumActionManager
    {
        internal enum Priority
        {
            DEFAULT,
            HIGH
        }

        internal static bool ShouldPerformActions { get; set; }

        private Leanplum.ShouldDisplayMessageHandler shouldDisplay;
        private Leanplum.PrioritizeMessagesHandler prioritizeHandler;

        private Leanplum.MessageHandler displayMessageHandler;
        private Leanplum.MessageHandler dismissMessageHandler;
        private Leanplum.MessageActionHandler actionMessageHandler;

        private bool enabled = true;
        private bool paused = false;

        private ActionContext currentAction;

        private LinkedList<ActionContext> Queue = new LinkedList<ActionContext>();
        private Queue<ActionContext> DelayedQueue = new Queue<ActionContext>();

        internal LeanplumActionManager()
        {
        }

        void PerformAvailableActions()
        {
            if (!enabled || paused || currentAction != null || Queue.Count == 0)
                return;

            currentAction = Queue.First();
            if (currentAction == null)
                return;

            Queue.RemoveFirst();

            LeanplumNative.CompatibilityLayer.LogDebug($"[ActionManager]: running action with name: {currentAction}");

            MessageDisplayChoice messageDecision = shouldDisplay?.Invoke(currentAction) ?? MessageDisplayChoice.Show();

            if (messageDecision.Choice == MessageDisplayChoice.DisplayChoice.DISCARD)
            {
                currentAction = null;
                PerformAvailableActions();
                return;
            }

            if (messageDecision.Choice == MessageDisplayChoice.DisplayChoice.DELAY)
            {
                LeanplumNative.CompatibilityLayer.LogDebug($"[ActionManager]: delaying action: {currentAction} for {messageDecision.DelaySeconds}s");
                if (messageDecision.DelaySeconds < 0)
                {
                    DelayedQueue.Enqueue(currentAction);
                }
                else
                {
                    // Schedule
                    LeanplumUnityHelper.Instance.StartCoroutine(Schedule(currentAction, messageDecision.DelaySeconds));
                }

                currentAction = null;
                PerformAvailableActions();
                return;
            }

            // ActionDidExecute
            currentAction.ActionExecute += CurrentAction_ActionExecute;

            // ActionDidDismiss
            currentAction.Dismiss += LeanplumActionManager_Dismiss;

            // Show message
            if (VarCache.actionDefinitions.TryGetValue(currentAction.Name, out ActionDefinition actionDefinition))
            {
                LeanplumNative.CompatibilityLayer.LogDebug($"[ActionManager]: action presented: {currentAction}");
                RecordImpression(currentAction);
                actionDefinition.Responder?.Invoke(currentAction);
                displayMessageHandler?.Invoke(currentAction);
            }
        }

        private void RecordImpression(ActionContext context)
        {
            // The View event is tracked using the messageId only, without an event name
            context?.TrackMessageEvent(null, 0, null, null);
        }

        private void CurrentAction_ActionExecute(string actionName, ActionContext context)
        {
            LeanplumNative.CompatibilityLayer.LogDebug($"[ActionManager]: actionDidExecute: {context}");
            actionMessageHandler?.Invoke(actionName, context);
        }

        private void LeanplumActionManager_Dismiss(ActionContext context)
        {
            LeanplumNative.CompatibilityLayer.LogDebug($"[ActionManager]: actionDidDismiss: {context}");
            dismissMessageHandler?.Invoke(context);
            currentAction = null;
            PerformAvailableActions();
        }

        IEnumerator Schedule(ActionContext context, int seconds)
        {
            yield return new WaitForSeconds(seconds);

            AppendActions(new ActionContext[] { context });
        }

        internal void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
        }

        internal void SetPaused(bool paused)
        {
            this.paused = paused;
        }

        internal void SetShouldDisplayHandler(Leanplum.ShouldDisplayMessageHandler handler)
        {
            shouldDisplay = handler;
        }

        internal void SetPrioritizeMessagesHandler(Leanplum.PrioritizeMessagesHandler handler)
        {
            prioritizeHandler = handler;
        }

        internal void SetOnDisplayMessageHandler(Leanplum.MessageHandler handler)
        {
            displayMessageHandler = handler;
        }

        internal void SetOnDismissMessageHandler(Leanplum.MessageHandler handler)
        {
            dismissMessageHandler = handler;
        }

        internal void SetOnActionMessageHandler(Leanplum.MessageActionHandler handler)
        {
            actionMessageHandler = handler;
        }

        internal void TriggerContexts(ActionContext[] contexts, Priority priority, ActionTrigger trigger, string eventName)
        {
            if (contexts == null || contexts.Length == 0)
                return;

            var actionTrigger = new Dictionary<string, object>
            {
                { "eventName", eventName },
                { "condition", trigger?.Value },
                { "contextualValues", new Dictionary<string, object>() }
            };

            ActionContext[] filteredContexts = prioritizeHandler?.Invoke(contexts, actionTrigger);
            if (filteredContexts == null)
            {
                filteredContexts = new[] { contexts.First() };
            }

            switch (priority)
            {
                case Priority.DEFAULT:
                    AppendActions(filteredContexts);
                    break;
                case Priority.HIGH:
                    InsertActions(filteredContexts);
                    break;
            }
        }

        void InsertActions(ActionContext[] actions)
        {
            if (!enabled)
                return;

            for (int i = actions.Length - 1; i >= 0; i--)
            {
                Queue.AddFirst(actions[i]);
            }
            PerformAvailableActions();
        }

        void AppendActions(ActionContext[] actions)
        {
            if (!enabled)
                return;

            foreach (var action in actions)
            {
                Queue.AddLast(action);
            }
            PerformAvailableActions();
        }

        internal void TriggerDelayedMessages()
        {
            // warning: not locked
            var contexts = DelayedQueue.ToArray();
            DelayedQueue = new Queue<ActionContext>();
            AppendActions(contexts);
        }

        internal void MaybePerformActions(ActionTrigger actionTrigger, string eventName = null)
        {
            if (!ShouldPerformActions)
                return;

            var condition = VarCache.Messages.Select(WhenTrigger.FromKV)
                .OrderBy(w => w.Priority)
                .Where(w => w.Conditions.Any(x => actionTrigger.Value.Contains(x.Subject) && x.Noun == eventName))
                .ToArray();

            List<ActionContext> contexts = new List<ActionContext>(condition.Length);
            for (int i = 0; i < condition.Length; i++)
            {
                string id = condition[i].Id;
                ActionContext actionContext = CreateActionContext(id);
                if (actionContext != null)
                {
                    contexts.Add(actionContext);
                }
            }

            TriggerContexts(contexts.ToArray(), Priority.DEFAULT, actionTrigger, eventName);
        }

        internal void TriggerPreview(IDictionary<string, object> packetData)
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
                    NativeActionContext context = new NativeActionContext(messageId, actionName, newVars);
                    TriggerContexts(new ActionContext[] { context }, Priority.HIGH, null, null);
                }
            }
        }

        /// <summary>
        ///     Creates ActionContext.
        ///     Ensures there is an action definition for the message action name.
        ///     Uses the GENERIC_DEFINITION_NAME as a fallback, if such is registered.
        /// </summary>
        /// <param name="id">The action id.</param>
        /// <returns>ActionContext otherwise null.</returns>
        internal ActionContext CreateActionContext(string id)
        {
            var message = Util.GetValueOrDefault(VarCache.Messages, id) as IDictionary<string, object>;
            string actionName = Util.GetValueOrDefault(message, Constants.Args.ACTION) as string;
            if (!string.IsNullOrEmpty(actionName)
                && Util.GetValueOrDefault(message, Constants.Args.VARS) is IDictionary<string, object> vars)
            {
                if (VarCache.actionDefinitions.ContainsKey(actionName))
                {
                    return new NativeActionContext(id, actionName, vars);
                }
                // If no matching action definition is found, use the Generic one if such is registered 
                else if (VarCache.actionDefinitions.ContainsKey(Constants.Args.GENERIC_DEFINITION_NAME))
                {
                    IDictionary<string, object> args = new Dictionary<string, object>
                        {
                            { Constants.Args.GENERIC_DEFINITION_CONFIG, message }
                        };
                    return new NativeActionContext(id, Constants.Args.GENERIC_DEFINITION_NAME, args);
                }
            }

            return null;
        }
    }
}