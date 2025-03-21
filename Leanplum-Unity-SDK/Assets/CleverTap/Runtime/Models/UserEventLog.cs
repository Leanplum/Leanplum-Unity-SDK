using System;
using System.Collections.Generic;
using CleverTapSDK.Utilities;

namespace CleverTapSDK
{
    public class UserEventLog
    {
        /// <summary>
        /// The name of the event.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// The normalized event name.
        /// </summary>
        public string NormalizedEventName { get; }

        /// <summary>
        /// The first time the event was recorded as timestamp.
        /// </summary>
        public long FirstTS { get; }

        /// <summary>
        /// The last time the event was recorded as timestamp.
        /// </summary>
        public long LastTS { get; }

        /// <summary>
        /// The number of times the event was recorded.
        /// </summary>
        public int CountOfEvents { get; }

        /// <summary>
        /// The deviceID the event is recorded for.
        /// </summary>
        public string DeviceID { get; }

        public UserEventLog(string eventName, string normalizedEventName, long firstTS, long lastTS, int countOfEvents, string deviceID)
        {
            EventName = eventName;
            NormalizedEventName = normalizedEventName;
            FirstTS = firstTS;
            LastTS = lastTS;
            CountOfEvents = countOfEvents;
            DeviceID = deviceID;
        }

        public override string ToString()
        {
            return $"UserEventLog:\n" +
                $"EventName: {EventName},\n" +
                $"NormalizedEventName: {NormalizedEventName},\n" +
                $"FirstTS: {FirstTS},\n" +
                $"LastTS: {LastTS},\n" +
                $"CountOfEvents: {CountOfEvents},\n" +
                $"DeviceID: {DeviceID}";
        }

        public static UserEventLog Parse(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                CleverTapLogger.LogError("Cannot parse null or empty UserEventLog JSON string.");
                return null;
            }

            try
            {
                var json = JSON.Parse(message);
                if (json.Count == 0)
                {
                    // JSON is an empty object, return null.
                    return null;
                }

                var userEventLog = new UserEventLog(
                    json["eventName"],
                    json["normalizedEventName"],
                    json["firstTS"].AsLong,
                    json["lastTS"].AsLong,
                    json["countOfEvents"].AsInt,
                    json["deviceID"]
                );
                return userEventLog;
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Unable to parse UserEventLog JSON: {ex}.");
            }
            return null;
        }

        public static Dictionary<string, UserEventLog> ParseLogsDictionary(string jsonString)
        {
            var userEventLogs = new Dictionary<string, UserEventLog>();
            if (string.IsNullOrEmpty(jsonString))
            {
                CleverTapLogger.LogError("Cannot parse null or empty UserEventLogs History JSON string.");
                return userEventLogs;
            }

            try
            {
                var json = JSON.Parse(jsonString);
                foreach (KeyValuePair<string, JSONNode> kvp in json.AsObject)
                {
                    var key = kvp.Key;
                    var userEventLogJson = kvp.Value.ToString();
                    var userEventLog = Parse(userEventLogJson);

                    if (userEventLog != null)
                    {
                        userEventLogs[key] = userEventLog;
                    }
                }
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Error parsing UserEventLog dictionary: {ex}");
            }

            return userEventLogs;
        }
    }
}
