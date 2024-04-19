using CleverTapSDK.Utilities;
using System;
using UnityEngine;

namespace CleverTapSDK.Common {
    public abstract class CleverTapCallbackHandler : MonoBehaviour {

        #region Callback Events

        public event CleverTapCallbackWithMessageDelegate OnCleverTapDeepLinkCallback;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapProfileInitializedCallback;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapProfileUpdatesCallback;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapPushOpenedCallback;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapInitCleverTapIdCallback;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapInAppNotificationDismissedCallback;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapInAppNotificationShowCallback;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapOnPushPermissionResponseCallback;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapInAppNotificationButtonTapped;

        public event CleverTapCallbackDelegate OnCleverTapInboxDidInitializeCallback;

        public event CleverTapCallbackDelegate OnCleverTapInboxMessagesDidUpdateCallback;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapInboxCustomExtrasButtonSelect;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapInboxItemClicked;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapNativeDisplayUnitsUpdated;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapProductConfigFetched;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapProductConfigActivated;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapProductConfigInitialized;

        public event CleverTapCallbackWithMessageDelegate OnCleverTapFeatureFlagsUpdated;

        public event CleverTapCallbackDelegate OnVariablesChanged;

        public event CleverTapCallbackDelegate OnOneTimeVariablesChanged;

        #endregion

        #region Default - Callback Methods

        public virtual void CleverTapDeepLinkCallback(string url) {
            CleverTapLogger.Log("unity received deep link: " + (!String.IsNullOrEmpty(url) ? url : "NULL"));
            if (OnCleverTapDeepLinkCallback != null) {
                OnCleverTapDeepLinkCallback(url);
            }
        }

        // called when then the CleverTap user profile is initialized
        // returns {"CleverTapID":<CleverTap unique user id>}
        public virtual void CleverTapProfileInitializedCallback(string message) {
            CleverTapLogger.Log("unity received profile initialized: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));

            if (String.IsNullOrEmpty(message)) {
                return;
            }

            try {
                JSONClass json = (JSONClass)JSON.Parse(message);
                CleverTapLogger.Log(String.Format("unity parsed profile initialized {0}", json));
                if (OnCleverTapProfileInitializedCallback != null) {
                    OnCleverTapProfileInitializedCallback(message);
                }
            } catch {
                CleverTapLogger.LogError("unable to parse json");
            }
        }

        // called when the user profile is updated as a result of a server sync
        /**
            returns dict in the form:
            {
                "profile":{"<property1>":{"oldValue":<value>, "newValue":<value>}, ...},
                "events:{"<eventName>":
                            {"count":
                                {"oldValue":(int)<old count>, "newValue":<new count>},
                            "firstTime":
                                {"oldValue":(double)<old first time event occurred>, "newValue":<new first time event occurred>},
                            "lastTime":
                                {"oldValue":(double)<old last time event occurred>, "newValue":<new last time event occurred>},
                        }, ...
                    }
            }
        */
        public virtual void CleverTapProfileUpdatesCallback(string message) {
            CleverTapLogger.Log("unity received profile updates: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (String.IsNullOrEmpty(message)) {
                return;
            }

            try {
                JSONClass json = (JSONClass)JSON.Parse(message);
                CleverTapLogger.Log(String.Format("unity parsed profile updates {0}", json));
                if (OnCleverTapProfileUpdatesCallback != null) {
                    OnCleverTapProfileUpdatesCallback(message);
                }
            } catch {
                CleverTapLogger.LogError("unable to parse json");
            }
        }

        // returns the data associated with the push notification
        public virtual void CleverTapPushOpenedCallback(string message) {
            CleverTapLogger.Log("unity received push opened: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (String.IsNullOrEmpty(message)) {
                return;
            }

            try {
                JSONClass json = (JSONClass)JSON.Parse(message);
                CleverTapLogger.Log(String.Format("push notification data is {0}", json));
                if (OnCleverTapPushOpenedCallback != null) {
                    OnCleverTapPushOpenedCallback(message);
                }
            } catch {
                CleverTapLogger.LogError("unable to parse json");
            }
        }

        // returns a unique CleverTap identifier suitable for use with install attribution providers.
        public virtual void CleverTapInitCleverTapIdCallback(string message) {
            CleverTapLogger.Log("unity received clevertap id: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (OnCleverTapInitCleverTapIdCallback != null) {
                OnCleverTapInitCleverTapIdCallback(message);
            }
        }

        // returns the custom data associated with an in-app notification click
        public virtual void CleverTapInAppNotificationDismissedCallback(string message) {
            CleverTapLogger.Log("unity received inapp notification dismissed: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (OnCleverTapInAppNotificationDismissedCallback != null) {
                OnCleverTapInAppNotificationDismissedCallback(message);
            }
        }

        // returns the custom data associated with an in-app notification click
        public virtual void CleverTapInAppNotificationShowCallback(string message) {
            CleverTapLogger.Log("unity received inapp notification onShow(): " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (OnCleverTapInAppNotificationShowCallback != null) {
                OnCleverTapInAppNotificationShowCallback(message);
            }
        }

        // returns the status of push permission response after it's granted/denied
        public virtual void CleverTapOnPushPermissionResponseCallback(string message) {
            //Ensure to create call the `CreateNotificationChannel` once notification permission is granted to register for receiving push notifications for Android 13+ devices.
            CleverTapLogger.Log("unity received push permission response: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (OnCleverTapOnPushPermissionResponseCallback != null) {
                OnCleverTapOnPushPermissionResponseCallback(message);
            }
        }

        // returns when an in-app notification is dismissed by a call to action with custom extras
        public virtual void CleverTapInAppNotificationButtonTapped(string message) {
            CleverTapLogger.Log("unity received inapp notification button tapped: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (OnCleverTapInAppNotificationButtonTapped != null) {
                OnCleverTapInAppNotificationButtonTapped(message);
            }
        }

        // returns callback for InitializeInbox
        public virtual void CleverTapInboxDidInitializeCallback() {
            CleverTapLogger.Log("unity received inbox initialized");
            if (OnCleverTapInboxDidInitializeCallback != null) {
                OnCleverTapInboxDidInitializeCallback();
            }
        }

        public virtual void CleverTapInboxMessagesDidUpdateCallback() {
            CleverTapLogger.Log("unity received inbox messages updated");
            if (OnCleverTapInboxMessagesDidUpdateCallback != null) {
                OnCleverTapInboxMessagesDidUpdateCallback();
            }
        }

        // returns on the click of app inbox message with a map of custom Key-Value pairs
        public virtual void CleverTapInboxCustomExtrasButtonSelect(string message) {
            CleverTapLogger.Log("unity received inbox message button with custom extras select: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (OnCleverTapInboxCustomExtrasButtonSelect != null) {
                OnCleverTapInboxCustomExtrasButtonSelect(message);
            }
        }

        // returns on the click of app inbox message with a string of the inbox payload along with page index and button index
        public virtual void CleverTapInboxItemClicked(string message) {
            CleverTapLogger.Log("unity received inbox message clicked callback: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (OnCleverTapInboxItemClicked != null) {
                OnCleverTapInboxItemClicked(message);
            }
        }

        // returns native display units data
        public virtual void CleverTapNativeDisplayUnitsUpdated(string message) {
            CleverTapLogger.Log("unity received native display units updated: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (OnCleverTapNativeDisplayUnitsUpdated != null) {
                OnCleverTapNativeDisplayUnitsUpdated(message);
            }
        }

        // invoked when Product Experiences - Product Config are fetched 
        public virtual void CleverTapProductConfigFetched(string message) {
            CleverTapLogger.Log("unity received product config fetched: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (OnCleverTapProductConfigFetched != null) {
                OnCleverTapProductConfigFetched(message);
            }
        }

        // invoked when Product Experiences - Product Config are activated
        public virtual void CleverTapProductConfigActivated(string message) {
            CleverTapLogger.Log("unity received product config activated: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (OnCleverTapProductConfigActivated != null) {
                OnCleverTapProductConfigActivated(message);
            }
        }

        // invoked when Product Experiences - Product Config are initialized
        public virtual void CleverTapProductConfigInitialized(string message) {
            CleverTapLogger.Log("unity received product config initialized: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (OnCleverTapProductConfigInitialized != null) {
                OnCleverTapProductConfigInitialized(message);
            }
        }

        // invoked when Product Experiences - Feature Flags are updated 
        public virtual void CleverTapFeatureFlagsUpdated(string message) {
            CleverTapLogger.Log("unity received feature flags updated: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            if (OnCleverTapFeatureFlagsUpdated != null) {
                OnCleverTapFeatureFlagsUpdated(message);
            }
        }

        #endregion

        #region Default - Variables Callbacks

        // invoked when any variable changed
        public virtual void CleverTapVariablesChanged() {
            CleverTapLogger.Log("Unity received variables changed");
            if (OnVariablesChanged != null) {
                OnVariablesChanged();
            }

            if (OnOneTimeVariablesChanged != null) {
                OnOneTimeVariablesChanged();
                OnOneTimeVariablesChanged = null;
            }
        }

        // invoked when an variable value changed
        public virtual void CleverTapVariableValueChanged(string variableName) {
            CleverTapLogger.Log("Unity received variables changed: " + (!String.IsNullOrEmpty(variableName) ? variableName : "NULL"));
            VariableFactory.CleverTapVariable.VariableChanged(variableName);
        }

        public virtual void CleverTapVariablesFetched(string message) {
            CleverTapLogger.Log("unity received variables fetched response: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            try {
                JSONClass json = (JSONClass)JSON.Parse(message);
                VariableFactory.CleverTapVariable.VariablesFetched(json["callbackId"].AsInt, json["isSuccess"].AsBool);
            } catch {
                CleverTapLogger.LogError("unable to parse json");
            }
        }

        #endregion

        #region Default - Variables Callbacks

        public virtual void CleverTapInAppsFetched(string message) {
            CleverTapLogger.Log("unity received InApps fetched response: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            try {
                JSONClass json = (JSONClass)JSON.Parse(message);
                InAppsFactory.CleverTapInApps.InAppsFetched(json["callbackId"].AsInt, json["isSuccess"].AsBool);
            } catch {
                CleverTapLogger.LogError("unable to parse json");
            }
        }

        #endregion
    }
}