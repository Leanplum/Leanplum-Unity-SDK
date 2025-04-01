#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CleverTapSDK.Common;
using CleverTapSDK.Utilities;
using UnityEngine;

namespace CleverTapSDK.Native
{
    internal class UnityNativeEventManager {
        private static readonly string NATIVE_EVENTS_DB_CACHE = "NativeEventsDbCache";
        private static readonly int DEFER_EVENT_UNTIL_APP_LAUNCHED_SECONDS = 2;

        private UnityNativePreferenceManager _preferenceManager;
        private UnityNativeDatabaseStore _databaseStore;
        private UnityNativeEventQueueManager _eventQueueManager;
        private UnityNativeCallbackHandler _callbackHandler;
        private UnityNativeCoreState _coreState;
        private UnityNativeNetworkEngine _networkEngine;
        private UnityNativeEventValidator _eventValidator;
        private string _accountId;
        private int _enableNetworkInfoReporting = -1;

        private readonly UnityNativePlatformVariable _platformVariable;

        internal UnityNativeEventManager(UnityNativeCallbackHandler callbackHandler) : this(callbackHandler, null) { }

        internal UnityNativeEventManager(UnityNativeCallbackHandler callbackHandler, UnityNativePlatformVariable platformVariable)
        {
            _callbackHandler = callbackHandler;
            _platformVariable = platformVariable;
        }

        private void Initialize(string accountId, string token, string region = null) {
            _accountId = accountId;
            UnityNativeAccountInfo accountInfo = new UnityNativeAccountInfo(accountId, token, region);
            _coreState = new UnityNativeCoreState(accountInfo);

            _preferenceManager = UnityNativePreferenceManager.GetPreferenceManager(_accountId);
            _databaseStore = new UnityNativeDatabaseStore($"{_accountId}_{NATIVE_EVENTS_DB_CACHE}");

            _eventValidator = new UnityNativeEventValidator(LoadDiscardedEvents());
            _networkEngine = UnityNativeNetworkEngine.Create(_accountId);

            _platformVariable?.Load(this, _callbackHandler, _coreState);

            // Requires network engine
            SetRequestInterceptors();
            SetResponseInterceptors();
            _eventQueueManager = new UnityNativeEventQueueManager(_coreState, _networkEngine, _databaseStore);
        }

        private List<string> LoadDiscardedEvents() {
            string deKey = string.Format(UnityNativeConstants.EventMeta.DISCARDED_EVENTS_NAMESPACE_KEY, _coreState.DeviceInfo.DeviceId);
            var discardedEventsSerialized = _preferenceManager.GetString(deKey, "[]");
            List<string> discardedEventNames = new List<string>();
            if (Json.Deserialize(discardedEventsSerialized) is List<object> discardedEvents && discardedEvents.Count > 0)
            {
                discardedEventNames = discardedEvents.Select(e => e.ToString()).ToList();
            }
            return discardedEventNames;
        }

        /// <summary>
        /// Sets response interceptors.
        /// Requires network engine to be initialized.
        /// </summary>
        private void SetResponseInterceptors() {
            List<IUnityNativeResponseInterceptor> responseInterceptors = new List<IUnityNativeResponseInterceptor>
            {
                new UnityNativeARPResponseInterceptor(_accountId, _coreState.DeviceInfo.DeviceId, _eventValidator),
                new UnityNativeMetadataResponseInterceptor(_preferenceManager)
            };

            if (_platformVariable != null)
            {
                responseInterceptors.Add(new UnityNativeVariablesResponseInterceptor(_platformVariable));
            }
            else
            {
                CleverTapLogger.LogError("PlatformVariable is null, cannot process variables responses.");
            }

            _networkEngine.SetResponseInterceptors(responseInterceptors);
        }

        /// <summary>
        /// Sets request interceptors.
        /// Requires network engine to be initialized.
        /// </summary>
        private void SetRequestInterceptors() {
#if UNITY_WEBGL && !UNITY_EDITOR
            _networkEngine.SetRequestInterceptors(new List<IUnityNativeRequestInterceptor>
            {
                new UnityNativeVariablesRequestInterceptor()
            });
#endif
        }

        #region Launch

        internal void LaunchWithCredentials(string accountId, string token, string region = null) {
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("Cannot record App Launched. AccountId and/or AccountToken are not set.");
            }

            Initialize(accountId, token, region);
            _networkEngine.SetRegion(region);
            // Set enable network reporting before calling App Launched so location is updated
            if (_enableNetworkInfoReporting > -1)
            {
                EnableDeviceNetworkInfoReporting(_enableNetworkInfoReporting == 1);
            }
            RecordAppLaunch();
            NotifyUserProfileInitialized();
        }

        internal void RecordAppLaunch() {
            if (_coreState.SessionManager.CurrentSession.IsAppLaunched) {
                return;
            }

            _networkEngine.SetHeaders(new Dictionary<string, string>() {
                { UnityNativeConstants.Network.HEADER_ACCOUNT_ID_NAME, _accountId },
            });

            _coreState.SessionManager.CurrentSession.SetIsAppLaunched(true);

            var eventDetails = new Dictionary<string, object> {
                { UnityNativeConstants.Event.EVENT_NAME, UnityNativeConstants.Event.EVENT_APP_LUNACH }
            };

            UnityNativeEvent @event = BuildEventWithAppFields(UnityNativeEventType.RaisedEvent, eventDetails, false);
            StoreEvent(@event);
            _eventQueueManager.FlushQueues();
        }
        #endregion

        #region Profile Events

        private readonly HashSet<string> IdentityKeys = new HashSet<string>() {
            UnityNativeConstants.Profile.EMAIL.ToLower(),
            UnityNativeConstants.Profile.IDENTITY.ToLower(),
            UnityNativeConstants.Profile.PHONE.ToLower()
        };

        internal UnityNativeEvent OnUserLogin(Dictionary<string, object> profile) {
            if (profile == null || profile.Count == 0) {
                return null;
            }

            if (ShouldDeferEvent(() =>
            {
                OnUserLogin(profile);
            }))
            {
                return null;
            }

            return _OnUserLogin(profile);
        }

        private UnityNativeEvent _OnUserLogin(Dictionary<string, object> profile) {
			try {
				string currentGUID = _coreState.DeviceInfo.DeviceId;
				bool haveIdentifier = false;
				string cachedGUID = null;

				foreach (var key in profile.Keys) {
					if (IdentityKeys.Contains(key.ToLower())) {
                        var value = profile[key];
                        string identifier = value?.ToString();
						if (!string.IsNullOrEmpty(identifier)) {
							haveIdentifier = true;
							cachedGUID = GetGUIDForIdentifier(key, identifier);
							if (cachedGUID != null) {
								break;
							}
						}
					}
				}

				// No Identifier or anonymous
				if (!haveIdentifier || IsAnonymousUser()) {
                    CleverTapLogger.Log($"OnUserLogin: No identifier OR device is anonymous, associating profile with current user profile: {currentGUID}");
                    return ProfilePush(profile);
				}
				// Same Profile
				if (cachedGUID != null && cachedGUID.Equals(currentGUID)) {
                    CleverTapLogger.Log($"OnUserLogin: Profile maps to current device id {currentGUID}, using current user profile.");
                    return ProfilePush(profile);
				}

                // New Profile
				SwitchOrCreateProfile(profile, cachedGUID);
			} catch (Exception e) {
				CleverTapLogger.LogError("OnUserLogin failed: " + e);
			}

			return null;
		}

		private bool IsAnonymousUser() {
            return string.IsNullOrEmpty(_preferenceManager.GetUserIdentities());
        }

        private string GetGUIDForIdentifier(string key, string identifier) {
            return _preferenceManager.GetGUIDForIdentifier(key,identifier);
        }

        private void SwitchOrCreateProfile(Dictionary<string, object> profile, string cacheGuid) {
            try
            {
                CleverTapLogger.Log($"asyncProfileSwitchUser:[profile {string.Join(Environment.NewLine, profile)}]" +
                    $" with Cached GUID {cacheGuid ?? "null"}");

                // Flush the queues
                _eventQueueManager.FlushQueues();

                // Reset the session
                _coreState.SessionManager.ResetSession();

                if (cacheGuid != null)
                {
                    _coreState.DeviceInfo.ForceUpdateDeviceId(cacheGuid);
                }
                else
                {
                    _coreState.DeviceInfo.ForceNewDeviceID();
                }

                // Load Variables for new user
                _platformVariable?.ReloadCache();

                // Load discarded events for new user
                _eventValidator = new UnityNativeEventValidator(LoadDiscardedEvents());
                // Set interceptors for new user
                SetResponseInterceptors();

                NotifyUserProfileInitialized();

                RecordAppLaunch();

                if (profile != null)
                {
                    ProfilePush(profile);
                }
            }
            catch (Exception e)
            {
                CleverTapLogger.LogError("Reset Profile error: " + e);
            }
        }

        internal void NotifyUserProfileInitialized() {
            var eventInfo = new Dictionary<string, string> {
                { "CleverTapID",  _coreState.DeviceInfo.DeviceId },
                { "CleverTapAccountID", _accountId }
            };
            _callbackHandler.CleverTapProfileInitializedCallback(Json.Serialize(eventInfo));
        }

        internal UnityNativeEvent ProfilePush(Dictionary<string, object> properties) {
            if (properties == null || properties.Count == 0) {
                return null;
            }

            if (ShouldDeferEvent(() =>
            {
                ProfilePush(properties);
            }))
            {
                return null;
            }

            // Updating Identity 
            foreach (var key in properties.Keys)
            {
                if (IdentityKeys.Contains(key.ToLower()))
                {
                    var value = properties[key];
                    string identifier = value?.ToString();
                    if (!string.IsNullOrEmpty(identifier))
                    {
                        _preferenceManager.SetGUIDForIdentifier(_coreState.DeviceInfo.DeviceId, key, identifier);
                    }
                }
            }

            var eventBuilderResult = new UnityNativeProfileEventBuilder(_eventValidator).BuildPushEvent(properties);
            if (eventBuilderResult.EventResult.SystemFields == null || eventBuilderResult.EventResult.CustomFields == null) {
                return null;
            }

            var eventDetails = new List<IDictionary<string, object>>() {
                eventBuilderResult.EventResult.SystemFields,
                eventBuilderResult.EventResult.CustomFields
            }.SelectMany(d => d).ToDictionary(d => d.Key, d => d.Value);

            Dictionary<string, object> profile = (Dictionary<string, object>)eventDetails["profile"];
            foreach (var key in properties.Keys)
            {
                if (!eventDetails.ContainsKey(key) && !profile.ContainsKey(key))
                {
                    profile.Add(key, properties[key]);
                }
            }
            eventDetails["profile"] = profile;
            return BuildEvent(UnityNativeEventType.ProfileEvent, eventDetails);
        }

        internal UnityNativeEvent ProfilePush(string key, object value, string command) {
            if (key == null || value == null || command == null)
            {
                return null;
            }

            var commandObj = new Dictionary<string, object>
            {
                { command, value }
            };

            var properties = new Dictionary<string, object>
            {
                { key, commandObj }
            };

            return ProfilePush(properties);
        }

        internal string GetCleverTapID() {
            if (_coreState == null || _coreState.DeviceInfo == null) {
                CleverTapLogger.LogError("Launch CleverTap before calling GetCleverTapID");
                return string.Empty;
            }
            return _coreState.DeviceInfo.DeviceId;
        }

        internal void EnableDeviceNetworkInfoReporting(bool enabled) {
            if (_coreState == null || _coreState.DeviceInfo == null)
            {
                _enableNetworkInfoReporting = enabled ? 1 : 0;
                return;
            }
            _coreState.DeviceInfo.EnableNetworkInfoReporting = enabled;
        }
        #endregion

        #region Record Events

        internal UnityNativeEvent RecordEvent(string eventName, Dictionary<string, object> properties = null) {
            if (ShouldDeferEvent(() =>
            {
                RecordEvent(eventName, properties);
            }))
            {
                return null;
            }

            var eventBuilderResult = new UnityNativeRaisedEventBuilder(_eventValidator).Build(eventName, properties);
            if(eventBuilderResult.EventResult == null)
                return null;
            var eventDetails = eventBuilderResult.EventResult;
            return BuildEvent(UnityNativeEventType.RaisedEvent, eventDetails);
        }

        internal UnityNativeEvent RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>> items) {
            if (ShouldDeferEvent(() =>
            {
                RecordChargedEventWithDetailsAndItems(details, items);
            }))
            {
                return null;
            }

            var eventBuilderResult = new UnityNativeRaisedEventBuilder(_eventValidator).BuildChargedEvent(details, items);
            var eventDetails = eventBuilderResult.EventResult;
            return BuildEvent(UnityNativeEventType.RaisedEvent, eventDetails);
        }

        #endregion

        #region Variables

        internal void SyncVariables(Dictionary<string, object> varsSyncPayload)
        {
            if (ShouldDeferEvent(() =>
            {
                SyncVariables(varsSyncPayload);
            }))
            {
                return;
            }

            UnityNativeEvent @event = BuildEvent(UnityNativeEventType.DefineVarsEvent, varsSyncPayload, false);
            _eventQueueManager.QueueEvent(@event);
        }

        internal void FetchVariables()
        {
            if (ShouldDeferEvent(() =>
            {
                FetchVariables();
            }))
            {
                return;
            }

            var eventBuilderResult = new UnityNativeRaisedEventBuilder(_eventValidator)
                .BuildFetchEvent(UnityNativeConstants.Event.WZRK_FETCH_TYPE_VARIABLES);
            if (eventBuilderResult.EventResult == null || eventBuilderResult.ValidationResults.Any(vr => !vr.IsSuccess))
            {
                CleverTapLogger.LogError($"Failed to build fetch event: " +
                    $"{ string.Join(", ", eventBuilderResult.ValidationResults.Select(vr => vr.ErrorMessage)) }");
                return;
            }
            var eventDetails = eventBuilderResult.EventResult;
            UnityNativeEvent @event = BuildEvent(UnityNativeEventType.FetchEvent, eventDetails, true);
            _eventQueueManager.QueueEvent(@event);
        }

        #endregion

        #region Private

        private bool ShouldDeferEvent(Action action) {
            if (_coreState == null ||
                _coreState.SessionManager == null ||
                !_coreState.SessionManager.CurrentSession.IsAppLaunched)
            {
                CleverTapLogger.Log($"App Launched not yet processed, re-queuing event after {DEFER_EVENT_UNTIL_APP_LAUNCHED_SECONDS}s.");
                MonoHelper.Instance.StartCoroutine(DeferEventCoroutine(action));
                return true;
            }
            return false;
        }

        private IEnumerator DeferEventCoroutine(Action action) {
            yield return new WaitForSeconds(DEFER_EVENT_UNTIL_APP_LAUNCHED_SECONDS);
            action();
        }

        private UnityNativeEvent BuildEvent(UnityNativeEventType eventType, Dictionary<string, object> eventDetails, bool storeEvent = true) {
            var eventData = new UnityNativeEventBuilder(_coreState, _networkEngine).BuildEvent(eventType, eventDetails);
            var eventDataJSONContent = Json.Serialize(eventData);
            var @event = new UnityNativeEvent(eventType, eventDataJSONContent);
            if (storeEvent)
            {
                StoreEvent(@event);
            }
            return @event;
        }

        private UnityNativeEvent BuildEventWithAppFields(UnityNativeEventType eventType, Dictionary<string, object> eventDetails, bool storeEvent = true) {
            var eventData = new UnityNativeEventBuilder(_coreState, _networkEngine).BuildEventWithAppFields(eventType, eventDetails);
            var eventDataJSONContent = Json.Serialize(eventData);
            var @event = new UnityNativeEvent(eventType, eventDataJSONContent);
            if (storeEvent)
            {
                StoreEvent(@event);
            }
            return @event;
        }

        private void StoreEvent(UnityNativeEvent evt) {
            _coreState.SessionManager.UpdateSessionTimestamp();
            _databaseStore.AddEvent(evt);
        }

        #endregion
    }
}
#endif