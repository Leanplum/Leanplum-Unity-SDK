#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System;
using SQLite4Unity3d;
using UnityEngine;

namespace CleverTapSDK.Native
{
    internal class UnityNativeEventDBEntry
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        public int Id { get; set; }
#else
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
#endif

        public UnityNativeEventType EventType { get; set; }
        public string JsonContent { get; set; }
        public long Timestamp { get; set; }

        public UnityNativeEventDBEntry()
        {
        }

        public UnityNativeEventDBEntry(UnityNativeEventType eventType, string jsonContent, long timestamp)
        {
            EventType = eventType;
            JsonContent = jsonContent;
            Timestamp = timestamp;
        }
    }
}
#endif
