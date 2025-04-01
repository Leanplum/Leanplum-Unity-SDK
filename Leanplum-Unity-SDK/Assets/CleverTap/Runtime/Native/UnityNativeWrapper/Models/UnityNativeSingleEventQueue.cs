#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native
{
    internal class UnityNativeSingleEventQueue : UnityNativeBaseEventQueue
    {
        protected override string QueueName => "SINGLE_EVENTS";

        private const int QUEUE_LIMIT = 1;

        internal UnityNativeSingleEventQueue(UnityNativeCoreState coreState,
            UnityNativeNetworkEngine networkEngine, int queueLimit = QUEUE_LIMIT, int defaultTimerInterval = 1) :
            base(coreState, networkEngine, queueLimit, defaultTimerInterval)
        { }

        protected override string GetRequestPath(List<UnityNativeEvent> events)
        {
            string path = UnityNativeConstants.Network.REQUEST_PATH_RECORD;
            if (events.Count != QUEUE_LIMIT)
            {
                CleverTapLogger.LogError($"Queue {QueueName} expects {QUEUE_LIMIT} event only but got {events.Count}.");
                return path;
            }

            UnityNativeEvent @event = events[0];
            if (@event.EventType == UnityNativeEventType.DefineVarsEvent)
            {
                return UnityNativeConstants.Network.REQUEST_PATH_DEFINE_VARIABLES;
            }

            return path;
        }

        internal override async Task<List<UnityNativeEvent>> FlushEvents()
        {
            return await FlushEventsCore(request => networkEngine.ExecuteRequest(request));
        }

        protected override bool CanProcessEventResponse(UnityNativeResponse response, UnityNativeRequest request, List<UnityNativeEvent> sentEvents)
        {
            if (sentEvents.Count != QUEUE_LIMIT)
            {
                CleverTapLogger.LogError($"Queue {QueueName} expects {QUEUE_LIMIT} event only but got {sentEvents.Count}.");
                return response.IsSuccess();
            }

            UnityNativeEvent @event = sentEvents[0];
            if (@event.EventType == UnityNativeEventType.DefineVarsEvent)
            {
                return CanProcessSyncVarsResponse(response);
            }

            return response.IsSuccess();
        }

        private static bool CanProcessSyncVarsResponse(UnityNativeResponse response)
        {
            if (!response.IsSuccess())
            {
                CleverTapLogger.LogError("Error Message: " + response.ErrorMessage);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        string errorMessage = null;
                        if (!string.IsNullOrEmpty(response.Content))
                        {
                            if (Json.Deserialize(response.Content) is Dictionary<string, object> content)
                            {
                                content.TryGetValue("error", out object error);
                                errorMessage = error as string;
                            }
                        }
                        CleverTapLogger.LogError($"Error while syncing (BadRequest). {errorMessage ?? $"Error: {errorMessage}."}");
                        break;
                    case HttpStatusCode.Unauthorized:
                        CleverTapLogger.LogError($"Error while syncing (Unauthorized). " +
                            $"Unauthorized access from a non-test profile. " +
                            $"Please mark this profile as a test profile from the CleverTap dashboard.");
                        break;
                    default:
                        CleverTapLogger.LogError($"Error while syncing ({response.StatusCode})");
                        break;
                }
            }
            else
            {
                CleverTapLogger.Log("Variables Successfully synced");
            }
            // Do not retry defineVars requests
            return true;
        }

        protected override bool ShouldRetryOnException(Exception ex, List<UnityNativeEvent> sentEvents)
        {
            if (sentEvents.Count != QUEUE_LIMIT)
            {
                CleverTapLogger.LogError($"Queue {QueueName} expects {QUEUE_LIMIT} event only but got {sentEvents.Count}.");
                return false;
            }

            UnityNativeEvent @event = sentEvents[0];
            if (@event.EventType == UnityNativeEventType.DefineVarsEvent)
            {
                // Do not retry defineVars requests
                return false;
            }

            return base.ShouldRetryOnException(ex, sentEvents);
        }

        internal override void QueueEvent(UnityNativeEvent newEvent)
        {
            if (!eventsQueue.TryPeek(out List<UnityNativeEvent> currentList) || currentList.Count == queueLimit)
            {
                currentList = new List<UnityNativeEvent>(QUEUE_LIMIT);
                eventsQueue.Enqueue(currentList);
            }

            currentList.Add(newEvent);
            // Send immediately
            _ = FlushEvents();
        }
    }
}
#endif