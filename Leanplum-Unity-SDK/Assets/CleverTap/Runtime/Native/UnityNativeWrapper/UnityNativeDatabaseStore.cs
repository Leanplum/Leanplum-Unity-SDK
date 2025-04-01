#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native {
    internal delegate void EventStored(UnityNativeEvent newEvent);
 
    internal class UnityNativeDatabaseStore {

        internal event EventStored OnEventStored;
        private IDataService dataService;

        internal UnityNativeDatabaseStore(string databaseName) {
            InitializeDatabase(databaseName);
        }

        internal void AddEvent(UnityNativeEvent newEvent) {

            if(newEvent == null || newEvent.JsonContent == null || dataService == null)
            {
                CleverTapLogger.LogError("Database not intiallised");
                return;
            }

            var dbEventId = dataService.Insert(new UnityNativeEventDBEntry(newEvent.EventType, newEvent.JsonContent, newEvent.Timestamp));
            var storedEvent = new UnityNativeEvent(dbEventId, newEvent.EventType, newEvent.JsonContent, newEvent.Timestamp);

            CleverTapLogger.Log($"Event added to Queue id: {dbEventId} type: {newEvent.EventType} jsonContent: {newEvent.JsonContent}");
            OnEventStored?.Invoke(storedEvent);
        }

        internal void AddEventsFromDB() {
            if (dataService == null)
            {
                CleverTapLogger.LogError("Database not intiallised");
                return;
            }

            CleverTapLogger.Log("Adding events to processing queue from Database");

            List<UnityNativeEventDBEntry> entries = dataService.GetAllEntries<UnityNativeEventDBEntry>();

            if (OnEventStored != null)
            {
                foreach (var entry in entries)
                {
                    CleverTapLogger.Log($"Event added to Queue id: {entry.Id} type: {entry.EventType} jsonContent: {entry.JsonContent}");
                    OnEventStored(new UnityNativeEvent(entry.Id, entry));
                }
            }
        }

        internal void DeleteEvents(List<UnityNativeEvent> eventsToRemove) {
            if (dataService == null)
            {
                CleverTapLogger.LogError("Database not initialized");
                return;
            }

            if (eventsToRemove == null)
            {
                return;
            }

            foreach (var events in eventsToRemove)
            {
                int id = events.Id ?? -1;
                if (id != -1)
                {
                    dataService.Delete<UnityNativeEventDBEntry>(id);
                }
            }
        }

        private void InitializeDatabase(string databaseName)
        {
            if (dataService == null)
            {   
                dataService = DataServiceFactory.CreateDataService(databaseName);
                CleverTapLogger.Log("Database connection created");
            }

            // Create table if it does not exist
            dataService.CreateTable<UnityNativeEventDBEntry>();
        }
    }
}
#endif