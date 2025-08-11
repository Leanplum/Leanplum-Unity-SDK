#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;
using System.Threading.Tasks;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native
{
    internal class UnityNativeEventQueueManager
    {
        private readonly UnityNativeDatabaseStore _databaseStore;

        private readonly UnityNativeBaseEventQueue _userEventsQueue;
        private readonly UnityNativeBaseEventQueue _raisedEventsQueue;
        private readonly UnityNativeBaseEventQueue _singleEventsQueue;
        private readonly UnityNativeBaseEventQueue _notificationViewedEventQueue;

        internal UnityNativeEventQueueManager(UnityNativeCoreState coreState, UnityNativeNetworkEngine networkEngine, UnityNativeDatabaseStore databaseStore)
        {
            _databaseStore = databaseStore;
            _databaseStore.OnEventStored += OnDatabaseEventStored;

            _userEventsQueue = new UnityNativeUserEventQueue(coreState, networkEngine);
            _userEventsQueue.OnEventTimerTick += OnUserEventTimerTick;
            _userEventsQueue.OnEventsProcessed += OnEventsProcessed;

            _raisedEventsQueue = new UnityNativeRaisedEventQueue(coreState, networkEngine);
            _raisedEventsQueue.OnEventTimerTick += OnRaisedEventTimerTick;
            _raisedEventsQueue.OnEventsProcessed += OnEventsProcessed;

            _singleEventsQueue = new UnityNativeSingleEventQueue(coreState, networkEngine);
            _singleEventsQueue.OnEventTimerTick += OnSingleEventTimerTick;
            _singleEventsQueue.OnEventsProcessed += OnEventsProcessed;

            _notificationViewedEventQueue = new UnityNativeNotificationViewedEventQueue(coreState, networkEngine);
            _notificationViewedEventQueue.OnEventTimerTick += OnNotificationViewEventTimerTick;
            _notificationViewedEventQueue.OnEventsProcessed += OnEventsProcessed;

            // Add the events stored in the DB
            _databaseStore.AddEventsFromDB();
        }

        private void OnEventsProcessed(List<UnityNativeEvent> flushedEvents)
        {
            _databaseStore.DeleteEvents(flushedEvents);
        }

        private void OnDatabaseEventStored(UnityNativeEvent newEvent)
        {
            QueueEvent(newEvent);
        }

        internal void QueueEvent(UnityNativeEvent newEvent)
        {
            switch (newEvent.EventType)
            {
                case UnityNativeEventType.ProfileEvent:
                    _userEventsQueue.QueueEvent(newEvent);
                    break;
                case UnityNativeEventType.RaisedEvent:
                case UnityNativeEventType.FetchEvent:
                    _raisedEventsQueue.QueueEvent(newEvent);
                    break;
                case UnityNativeEventType.DefineVarsEvent:
                case UnityNativeEventType.DefineCustomTemplatesEvent:
                    _singleEventsQueue.QueueEvent(newEvent);
                    break;
                case UnityNativeEventType.NotificationViewEvent:
                    _notificationViewedEventQueue.QueueEvent(newEvent);
                    break;
                default:
                    CleverTapLogger.Log($"Unhandled event type: {newEvent.EventType}");
                    break;
            }
        }

        internal async void FlushQueues()
        {
            CleverTapLogger.Log("Flushing queues");
            await FlushUserEvents();
            await FlushRaisedEvents();
        }

        private async void OnUserEventTimerTick()
        {
            await FlushUserEvents();
        }

        private async void OnRaisedEventTimerTick()
        {
            await FlushRaisedEvents();
        }

        private async void OnSingleEventTimerTick()
        {
            await FlushSingleEvents();
        }

        private async void OnNotificationViewEventTimerTick()
        {
            await FlushNotificationViewEvents();
        }

        private async Task FlushUserEvents()
        {
            CleverTapLogger.Log("Flushing user events");
            await _userEventsQueue.FlushEvents();
        }

        private async Task FlushRaisedEvents()
        {
            CleverTapLogger.Log("Flushing raised events");
            await _raisedEventsQueue.FlushEvents();
        }

        private async Task FlushSingleEvents()
        {
            CleverTapLogger.Log("Flushing single events");
            await _singleEventsQueue.FlushEvents();
        }

        private async Task FlushNotificationViewEvents()
        {
            CleverTapLogger.Log("Flushing notification view events");
            await _notificationViewedEventQueue.FlushEvents();
        }
    }
}
#endif