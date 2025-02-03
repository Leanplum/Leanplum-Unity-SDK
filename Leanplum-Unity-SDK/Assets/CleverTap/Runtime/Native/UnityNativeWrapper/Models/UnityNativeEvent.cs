#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System;

namespace CleverTapSDK.Native {
    internal class UnityNativeEvent {
        private readonly int? _id;
        private readonly UnityNativeEventType _eventType;
        private readonly string _jsonContent;
        private readonly long _timestamp;

        internal UnityNativeEvent(int id, UnityNativeEventType eventType, string jsonContent, long? timestamp = null) {
            _id = id;
            _eventType = eventType;
            _jsonContent = jsonContent;
            _timestamp = timestamp ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        internal UnityNativeEvent(UnityNativeEventType eventType, string jsonContent, long? timestamp = null) {
            _id = null;
            _eventType = eventType;
            _jsonContent = jsonContent;
            _timestamp = timestamp ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        internal UnityNativeEvent(int id,UnityNativeEventDBEntry eventDBEntry)
        {
            _id = id;
            _eventType = eventDBEntry.EventType;
            _jsonContent = eventDBEntry.JsonContent;
            _timestamp = eventDBEntry.Timestamp;
        }

        internal int? Id => _id;
        internal UnityNativeEventType EventType => _eventType;
        internal string JsonContent => _jsonContent;
        internal long Timestamp => _timestamp;
    }
}
#endif