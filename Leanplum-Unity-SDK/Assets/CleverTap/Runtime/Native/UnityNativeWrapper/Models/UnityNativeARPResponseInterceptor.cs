#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System;
using System.Collections.Generic;
using CleverTapSDK.Utilities;
using System.Linq;

namespace CleverTapSDK.Native
{
    internal class UnityNativeARPResponseInterceptor : IUnityNativeResponseInterceptor
    {
        private readonly UnityNativeEventValidator _eventValidator;
        private readonly UnityNativePreferenceManager _preferenceManager;
        private readonly string _namespaceARPKey;
        private readonly string _discardedEventsKey;

        internal UnityNativeARPResponseInterceptor(string accountId, string deviceId, UnityNativeEventValidator eventValidator)
        {
            _preferenceManager = UnityNativePreferenceManager.GetPreferenceManager(accountId);
            _namespaceARPKey = string.Format(UnityNativeConstants.EventMeta.ARP_NAMESPACE_KEY, deviceId);
            _discardedEventsKey = string.Format(UnityNativeConstants.EventMeta.DISCARDED_EVENTS_NAMESPACE_KEY, deviceId);
            _eventValidator = eventValidator;
        }

        UnityNativeResponse IUnityNativeResponseInterceptor.Intercept(UnityNativeResponse response)
        {
            if (response == null || string.IsNullOrEmpty(response.Content))
            {
                // Response or response content is null. This is the case for the handshake response.
                return response;
            }

            var result = Json.Deserialize(response.Content) as Dictionary<string, object>;
            try
            {
                if (result != null && result.ContainsKey(UnityNativeConstants.EventMeta.ARP_KEY))
                {
                    if (result[UnityNativeConstants.EventMeta.ARP_KEY] is Dictionary<string, object> { Count: > 0 } arp)
                    {
                        // Handle Discarded events in ARP
                        try
                        {
                            ProcessDiscardedEventsList(arp);
                        }
                        catch (Exception discardedEventsException)
                        {
                            CleverTapLogger.Log($"Failed to process ARP discarded events," +
                                $" Exception: {discardedEventsException.Message}, Stack Trace: {discardedEventsException.StackTrace}");
                        }

                        HandleARPUpdate(arp);
                    }
                }
            }
            catch (Exception exception)
            {
                CleverTapLogger.Log($"Failed to process ARP, Exception: {exception.Message}, Stack Trace: {exception.StackTrace}");
            }

            return response;
        }

        private void HandleARPUpdate(Dictionary<string, object> arp)
        {
            if (arp == null || arp.Count == 0 || string.IsNullOrEmpty(_namespaceARPKey))
                return;

            Dictionary<string, object> currentARP = Json.Deserialize(_preferenceManager.GetString(_namespaceARPKey, "{}")) as Dictionary<string, object>;

            if (currentARP == null)
            {
                currentARP = new Dictionary<string, object>();
            }

            foreach (var keyValuePair in arp)
            {
                string key = keyValuePair.Key;
                object value = keyValuePair.Value;

                switch (value)
                {
                    case int i:
                        if (i == -1)
                        {
                            CleverTapLogger.Log($"ARP remove {key} (value is -1)");
                            currentARP.Remove(key);
                        }
                        else
                        {
                            currentARP[key] = i;
                        }
                        break;
                    case string s:
                        if (s.Length > 100)
                        {
                            CleverTapLogger.Log($"ARP update for {key} rejected (string value too long)");
                            currentARP.Remove(key);
                        }
                        else
                        {
                            currentARP[key] = s;
                        }
                        currentARP[key] = s;
                        break;
                    case long l:
                        if (l == -1)
                        {
                            CleverTapLogger.Log($"ARP remove {key} (value is -1)");
                            currentARP.Remove(key);
                        }
                        else
                        {
                            currentARP[key] = l;
                        }
                        break;
                    case float f:
                        currentARP[key] = f;
                        break;
                    case double d:
                        currentARP[key] = d;
                        break;
                    case bool b:
                        currentARP[key] = b;
                        break;
                    default:
                        CleverTapLogger.Log($"ARP update for key {key} rejected (invalid data type)");
                        break;
                }
            }

            _preferenceManager.SetString(_namespaceARPKey, Json.Serialize(currentARP));
        }

        private void ProcessDiscardedEventsList(Dictionary<string, object> arp)
        {
            if (!arp.ContainsKey(UnityNativeConstants.EventMeta.DISCARDED_EVENTS_KEY))
            {
                return;
            }

            try
            {
                List<object> discardedEventsList = arp[UnityNativeConstants.EventMeta.DISCARDED_EVENTS_KEY] as List<object>;
                List<string> discardedEventNames = new List<string>();
                if (discardedEventsList != null && discardedEventsList.Count > 0)
                {
                    discardedEventNames = discardedEventsList.Select(e => e.ToString()).ToList();
                }
                if (_eventValidator != null)
                {
                    CleverTapLogger.Log($"Setting discarded events: {string.Join(", ", discardedEventNames)}.");
                    _eventValidator.SetDiscardedEvents(discardedEventNames);
                    _preferenceManager.SetString(_discardedEventsKey, Json.Serialize(discardedEventNames));
                }
                else
                {
                    CleverTapLogger.Log("Validator object is NULL");
                } 
            }
            catch (Exception e)
            {
                CleverTapLogger.Log("Error parsing discarded events list: " + e.StackTrace);
            }
        }
    }
}
#endif