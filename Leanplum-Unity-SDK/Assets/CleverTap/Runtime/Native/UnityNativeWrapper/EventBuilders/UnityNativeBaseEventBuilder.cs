#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System;
using System.Collections.Generic;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native {
    internal class UnityNativeEventBuilder {

        private UnityNativeDeviceInfo _deviceInfo;
        private UnityNativeSessionManager _sessionManager;
        private UnityNativeNetworkEngine _networkEngine;
        
        internal UnityNativeEventBuilder(UnityNativeCoreState coreState, UnityNativeNetworkEngine networkEngine) {
            _deviceInfo = coreState.DeviceInfo;
            _sessionManager = coreState.SessionManager;
            _networkEngine = networkEngine;
        }

        internal Dictionary<string, object> BuildEvent(UnityNativeEventType eventType, Dictionary<string, object> eventDetails) {
           
            if (_networkEngine.IsMuted()) {
                CleverTapLogger.Log("Not building event. CleverTap is Muted.");
                return null;
            }
                
            var eventData = new Dictionary<string, object>(eventDetails);
            switch (eventType) {
                case UnityNativeEventType.ProfileEvent:
                    eventData.Add(UnityNativeConstants.Event.EVENT_TYPE, UnityNativeConstants.Event.EVENT_TYPE_PROFILE);
                    break;
                case UnityNativeEventType.RaisedEvent:
                case UnityNativeEventType.FetchEvent:
                    eventData.Add(UnityNativeConstants.Event.EVENT_TYPE, UnityNativeConstants.Event.EVENT_TYPE_EVENT);
                    break;
                case UnityNativeEventType.DefineVarsEvent:
                    break;
                default:
                    // NOT Supported YET
                    throw new NotImplementedException();
            }

            var currentSession = _sessionManager.CurrentSession;

            eventData.Add(UnityNativeConstants.Event.UNIX_EPOCH_TIME, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            eventData.Add(UnityNativeConstants.Event.SESSION, currentSession.SessionId);
            eventData.Add(UnityNativeConstants.Event.SCREEN_COUNT, _sessionManager.GetScreenCount());
            eventData.Add(UnityNativeConstants.Event.LAST_SESSION_LENGTH_SECONDS, _sessionManager.GetLastSessionLength());
            eventData.Add(UnityNativeConstants.Event.IS_FIRST_SESSION, _sessionManager.IsFirstSession());

            string screenName = _sessionManager.GetScreenName();
            if (!string.IsNullOrEmpty(screenName)) {
                eventData.Add("n", screenName);
            }

            return eventData;
        }

        internal Dictionary<string, object> BuildEventWithAppFields(UnityNativeEventType eventType, Dictionary<string, object> eventDetails) {
            var eventData = BuildEvent(eventType, eventDetails);
            eventData.Add(UnityNativeConstants.Event.EVENT_DATA, BuildAppFields(_deviceInfo));

            return eventData;
        }

        internal static Dictionary<string, object> BuildAppFields(UnityNativeDeviceInfo deviceInfo) {
            var data = new Dictionary<string, object>
            {
                { UnityNativeConstants.Event.APP_VERSION, deviceInfo.AppVersion },
                { UnityNativeConstants.Event.BUILD, deviceInfo.AppBuild },
                { UnityNativeConstants.Event.SDK_VERSION, deviceInfo.SdkVersion },
                { UnityNativeConstants.Event.OS_VERSION, deviceInfo.OsVersion },
                { UnityNativeConstants.Event.OS_NAME, deviceInfo.OsName },
                { UnityNativeConstants.Event.SCREEN_WIDTH, deviceInfo.DeviceWidth },
                { UnityNativeConstants.Event.SCREEN_HEIGHT, deviceInfo.DeviceHeight }
            };

            if (!string.IsNullOrEmpty(deviceInfo.Model)) {
                data.Add(UnityNativeConstants.Event.MODEL, deviceInfo.Model);
            }

            if (!string.IsNullOrEmpty(deviceInfo.Manufacturer)) {
                data.Add(UnityNativeConstants.Event.MANUFACTURER, deviceInfo.Manufacturer);
            }

            if (!string.IsNullOrEmpty(deviceInfo.Carrier)) {
                data.Add(UnityNativeConstants.Event.CARRIER, deviceInfo.Carrier);
            }

            var useIp = deviceInfo.EnableNetworkInfoReporting;
            data.Add(UnityNativeConstants.Event.USE_IP, useIp);
            if (useIp)
            {
                if (!string.IsNullOrEmpty(deviceInfo.Radio)) {
                    data.Add(UnityNativeConstants.Event.NETWORK_TYPE, deviceInfo.Radio);
                }
                if (deviceInfo.Wifi != -1) {
                    data.Add(UnityNativeConstants.Event.CONNECTED_TO_WIFI, deviceInfo.Wifi);
                }
            }

            if (!string.IsNullOrEmpty(deviceInfo.CountryCode)) {
                data.Add(UnityNativeConstants.Event.COUNTRY_CODE, deviceInfo.CountryCode);
            }

            if (!string.IsNullOrEmpty(deviceInfo.Locale)) {
                data.Add(UnityNativeConstants.Event.LOCALE_IDENTIFIER, deviceInfo.Locale);
            }

            return data;
        }
    }
}
#endif