#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;

namespace CleverTapSDK.Native {
    internal class UnityNativeWrapper {        
        private readonly UnityNativeEventManager _eventManager;

        internal UnityNativeWrapper(UnityNativeCallbackHandler callbackHandler)
        {
            _eventManager = new UnityNativeEventManager(callbackHandler);
        }

        internal void LaunchWithCredentials(string accountID, string token, string region = null) {
            _eventManager.LaunchWithCredentials(accountID, token, region);
        }
        internal void OnUserLogin(Dictionary<string, object> properties) {
            _eventManager.OnUserLogin(properties);
        }

        internal void ProfilePush(Dictionary<string, object> properties) {
            _eventManager.ProfilePush(properties);
        }

        internal void ProfilePush(string key,object value,string command)
        {
            _eventManager.ProfilePush(key, value, command);
        }

        internal void RecordEvent(string eventName, Dictionary<string, object> properties = null) {
            _eventManager.RecordEvent(eventName, properties);
        }

        internal void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>> items) {
            _eventManager.RecordChargedEventWithDetailsAndItems(details, items);
        }

        internal string GetCleverTapID() {
            return _eventManager.GetCleverTapID();
        }

        internal void EnableDeviceNetworkInfoReporting(bool enabled) {
            _eventManager.EnableDeviceNetworkInfoReporting(enabled);
        }
    }
}
#endif