#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace CleverTapSDK.Native {
    internal class UnityNativeEventQueue {
        private const int _eventLimit = 49;
        
        private readonly List<UnityNativeEvent> _events;
        
        private long _firstEventAddedMilisecondsTimestamp;
        private long _lastEventAddedMilisecondsTimestamp;       
        
        internal UnityNativeEventQueue() {
            _events = new List<UnityNativeEvent>();
        }

        internal IReadOnlyList<UnityNativeEvent> Events => _events;
        internal long FirstEventAddedMilisecondsTimestamp => _firstEventAddedMilisecondsTimestamp;
        internal long LastEventAddedMilisecondsTimestamp => _lastEventAddedMilisecondsTimestamp;

        internal bool AddEvent(UnityNativeEvent newEvent) {
            if (_events.Count < _eventLimit) {
                if (_events.Count == 0) {
                    _firstEventAddedMilisecondsTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }

                _lastEventAddedMilisecondsTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                _events.Add(newEvent);
                return true;
            }

            return false;
        }

        internal bool IsEventLimitReached => 
            _events.Count == _eventLimit;
    }
}
#endif