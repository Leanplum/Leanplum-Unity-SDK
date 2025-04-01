using CleverTapSDK.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CleverTapSDK.Common {
    public abstract class CleverTapCallbackHandler : MonoBehaviour {

        protected readonly object CallbackLock = new();

        internal virtual void OnCallbackAdded(Action<string> callbackMethod) { }

        internal virtual void OnVariablesCallbackAdded(Action<string> callbackMethod, int callbackId) { }
        internal virtual void OnVariablesCallbackRemoved(int callbackId) { }

        #region Callback Events

        private CleverTapCallbackWithMessageDelegate _OnCleverTapDeepLinkCallback;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapDeepLinkCallback
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapDeepLinkCallback += value;
                    OnCallbackAdded(CleverTapDeepLinkCallback);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapDeepLinkCallback -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapProfileInitializedCallback;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapProfileInitializedCallback
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapProfileInitializedCallback += value;
                    OnCallbackAdded(CleverTapProfileInitializedCallback);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapProfileInitializedCallback -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapProfileUpdatesCallback;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapProfileUpdatesCallback
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapProfileUpdatesCallback += value;
                    OnCallbackAdded(CleverTapProfileUpdatesCallback);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapProfileUpdatesCallback -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapPushOpenedCallback;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapPushOpenedCallback
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapPushOpenedCallback += value;
                    OnCallbackAdded(CleverTapPushOpenedCallback);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapPushOpenedCallback -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapPushNotificationTappedWithCustomExtrasCallback;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapPushNotificationTappedWithCustomExtrasCallback
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapPushNotificationTappedWithCustomExtrasCallback += value;
                    OnCallbackAdded(CleverTapPushNotificationTappedWithCustomExtrasCallback);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapPushNotificationTappedWithCustomExtrasCallback -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapInitCleverTapIdCallback;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapInitCleverTapIdCallback
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInitCleverTapIdCallback += value;
                    OnCallbackAdded(CleverTapInitCleverTapIdCallback);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInitCleverTapIdCallback -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapInAppNotificationDismissedCallback;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapInAppNotificationDismissedCallback
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInAppNotificationDismissedCallback += value;
                    OnCallbackAdded(CleverTapInAppNotificationDismissedCallback);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInAppNotificationDismissedCallback -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapInAppNotificationShowCallback;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapInAppNotificationShowCallback
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInAppNotificationShowCallback += value;
                    OnCallbackAdded(CleverTapInAppNotificationShowCallback);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInAppNotificationShowCallback -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapOnPushPermissionResponseCallback;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapOnPushPermissionResponseCallback
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapOnPushPermissionResponseCallback += value;
                    OnCallbackAdded(CleverTapOnPushPermissionResponseCallback);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapOnPushPermissionResponseCallback -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapPushNotificationPermissionStatusCallback;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapPushNotificationPermissionStatusCallback
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapPushNotificationPermissionStatusCallback += value;
                    OnCallbackAdded(CleverTapPushNotificationPermissionStatus);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapPushNotificationPermissionStatusCallback -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapInAppNotificationButtonTapped;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapInAppNotificationButtonTapped
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInAppNotificationButtonTapped += value;
                    OnCallbackAdded(CleverTapInAppNotificationButtonTapped);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInAppNotificationButtonTapped -= value;
                }
            }
        }

        private CleverTapCallbackDelegate _OnCleverTapInboxDidInitializeCallback;
        public event CleverTapCallbackDelegate OnCleverTapInboxDidInitializeCallback
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInboxDidInitializeCallback += value;
                    OnCallbackAdded(CleverTapInboxDidInitializeCallback);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInboxDidInitializeCallback -= value;
                }
            }
        }

        private CleverTapCallbackDelegate _OnCleverTapInboxMessagesDidUpdateCallback;
        public event CleverTapCallbackDelegate OnCleverTapInboxMessagesDidUpdateCallback
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInboxMessagesDidUpdateCallback += value;
                    OnCallbackAdded(CleverTapInboxMessagesDidUpdateCallback);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInboxMessagesDidUpdateCallback -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapInboxCustomExtrasButtonSelect;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapInboxCustomExtrasButtonSelect
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInboxCustomExtrasButtonSelect += value;
                    OnCallbackAdded(CleverTapInboxCustomExtrasButtonSelect);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInboxCustomExtrasButtonSelect -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapInboxItemClicked;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapInboxItemClicked
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInboxItemClicked += value;
                    OnCallbackAdded(CleverTapInboxItemClicked);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapInboxItemClicked -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapNativeDisplayUnitsUpdated;
        public event CleverTapCallbackWithMessageDelegate OnCleverTapNativeDisplayUnitsUpdated
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapNativeDisplayUnitsUpdated += value;
                    OnCallbackAdded(CleverTapNativeDisplayUnitsUpdated);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapNativeDisplayUnitsUpdated -= value;
                }
            }
        }

        [Obsolete]
        private CleverTapCallbackWithMessageDelegate _OnCleverTapProductConfigFetched;

        [Obsolete]
        public event CleverTapCallbackWithMessageDelegate OnCleverTapProductConfigFetched
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapProductConfigFetched += value;
                    OnCallbackAdded(CleverTapProductConfigFetched);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapProductConfigFetched -= value;
                }
            }
        }

        [Obsolete]
        private CleverTapCallbackWithMessageDelegate _OnCleverTapProductConfigActivated;

        [Obsolete]
        public event CleverTapCallbackWithMessageDelegate OnCleverTapProductConfigActivated
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapProductConfigActivated += value;
                    OnCallbackAdded(CleverTapProductConfigActivated);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapProductConfigActivated -= value;
                }
            }
        }

        [Obsolete]
        private CleverTapCallbackWithMessageDelegate _OnCleverTapProductConfigInitialized;

        [Obsolete]
        public event CleverTapCallbackWithMessageDelegate OnCleverTapProductConfigInitialized
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapProductConfigInitialized += value;
                    OnCallbackAdded(CleverTapProductConfigInitialized);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapProductConfigInitialized -= value;
                }
            }
        }

        private CleverTapCallbackWithMessageDelegate _OnCleverTapFeatureFlagsUpdated;

        [Obsolete("Feature Flags are deprecated, use variables instead.")]
        public event CleverTapCallbackWithMessageDelegate OnCleverTapFeatureFlagsUpdated
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCleverTapFeatureFlagsUpdated += value;
                    OnCallbackAdded(CleverTapFeatureFlagsUpdated);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCleverTapFeatureFlagsUpdated -= value;
                }
            }
        }

        protected readonly IDictionary<int, CleverTapCallbackDelegate> variablesCallbacks = new Dictionary<int, CleverTapCallbackDelegate>();
        internal readonly CleverTapCounter counter = new CleverTapCounter();

        private void AddVariableCallback(CleverTapCallbackDelegate value, Action<string> action)
        {
            lock (CallbackLock)
            {
                int id = counter.GetNextAndIncreaseCounter();
                variablesCallbacks[id] = value;
                OnVariablesCallbackAdded(action, id);
            }
        }

        private void RemoveVariableCallback(CleverTapCallbackDelegate value)
        {
            lock (CallbackLock)
            {
                int id = -1;
                foreach (var kv in variablesCallbacks)
                {
                    if (kv.Value == value)
                    {
                        id = kv.Key;
                        break;
                    }
                }

                if (id != -1)
                {
                    variablesCallbacks.Remove(id);
                    OnVariablesCallbackRemoved(id);
                }
            }
        }

        public virtual event CleverTapCallbackDelegate OnVariablesChanged
        {
            add
            {
                AddVariableCallback(value, CleverTapVariablesChanged);
            }
            remove
            {
                RemoveVariableCallback(value);
            }
        }

        public virtual event CleverTapCallbackDelegate OnOneTimeVariablesChanged
        {
            add
            {
                AddVariableCallback(value, OneTimeCleverTapVariablesChanged);
            }
            remove
            {
                RemoveVariableCallback(value);
            }
        }

        public virtual event CleverTapCallbackDelegate OnOneTimeVariablesChangedAndNoDownloadsPending
        {
            add
            {
                AddVariableCallback(value, OneTimeCleverTapVariablesChangedAndNoDownloadsPending);
            }
            remove
            {
                RemoveVariableCallback(value);
            }
        }

        public virtual event CleverTapCallbackDelegate OnVariablesChangedAndNoDownloadsPending
        {
            add
            {
                AddVariableCallback(value, CleverTapVariablesChangedAndNoDownloadsPending);
            }
            remove
            {
                RemoveVariableCallback(value);
            }
        }

        private CleverTapCallbackWithTemplateContext _OnCustomTemplatePresent;
        public event CleverTapCallbackWithTemplateContext OnCustomTemplatePresent
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCustomTemplatePresent += value;
                    OnCallbackAdded(CleverTapCustomTemplatePresent);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCustomTemplatePresent -= value;
                }
            }
        }

        private CleverTapCallbackWithTemplateContext _OnCustomTemplateClose;
        public event CleverTapCallbackWithTemplateContext OnCustomTemplateClose
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCustomTemplateClose += value;
                    OnCallbackAdded(CleverTapCustomTemplateClose);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCustomTemplateClose -= value;
                }
            }
        }

        private CleverTapCallbackWithTemplateContext _OnCustomFunctionPresent;
        public event CleverTapCallbackWithTemplateContext OnCustomFunctionPresent
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnCustomFunctionPresent += value;
                    OnCallbackAdded(CleverTapCustomFunctionPresent);
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnCustomFunctionPresent -= value;
                }
            }
        }
        #endregion

        #region Default - Callback Methods

        public virtual void CleverTapDeepLinkCallback(string url) {
            CleverTapLogger.Log("unity received deep link: " + (!String.IsNullOrEmpty(url) ? url : "NULL"));
            _OnCleverTapDeepLinkCallback?.Invoke(url);
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
                _OnCleverTapProfileInitializedCallback?.Invoke(message);
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
                _OnCleverTapProfileUpdatesCallback?.Invoke(message);
            } catch {
                CleverTapLogger.LogError("unable to parse json");
            }
        }

        /// <summary>
        /// Callback when a push notitication is opened.
        /// </summary>
        /// <param name="message">The data associated with the push notification message.</param>
        public virtual void CleverTapPushOpenedCallback(string message)
        {
            CleverTapLogger.Log("unity received push opened: " + (!string.IsNullOrEmpty(message) ? message : "NULL"));
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            try
            {
                JSONClass json = (JSONClass)JSON.Parse(message);
                CleverTapLogger.Log(string.Format("push notification data is {0}", json));
                _OnCleverTapPushOpenedCallback?.Invoke(message);
            }
            catch
            {
                CleverTapLogger.LogError("unable to parse json");
            }
        }

        /// <summary>
        /// IOS only callback. Called when a push notitication is opened.
        /// </summary>
        /// <param name="message">
        /// The push notification user info without the "aps" param.
        /// It contains extra key/value pairs set in the CleverTap dashboard for this notification
        /// </param>
        public virtual void CleverTapPushNotificationTappedWithCustomExtrasCallback(string message)
        {
            CleverTapLogger.Log("unity received push tapped with custom extras: " + (!string.IsNullOrEmpty(message) ? message : "NULL"));
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            try
            {
                JSONClass json = (JSONClass)JSON.Parse(message);
                CleverTapLogger.Log(string.Format("push notification data is {0}", json));
                _OnCleverTapPushNotificationTappedWithCustomExtrasCallback?.Invoke(message);
            }
            catch
            {
                CleverTapLogger.LogError("unable to parse json");
            }
        }

        // returns a unique CleverTap identifier suitable for use with install attribution providers.
        public virtual void CleverTapInitCleverTapIdCallback(string message) {
            CleverTapLogger.Log("unity received clevertap id: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapInitCleverTapIdCallback?.Invoke(message);
        }

        // returns the custom data associated with an in-app notification click
        public virtual void CleverTapInAppNotificationDismissedCallback(string message) {
            CleverTapLogger.Log("unity received inapp notification dismissed: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapInAppNotificationDismissedCallback?.Invoke(message);
        }

        // returns the custom data associated with an in-app notification click
        public virtual void CleverTapInAppNotificationShowCallback(string message) {
            CleverTapLogger.Log("unity received inapp notification onShow(): " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapInAppNotificationShowCallback?.Invoke(message);
        }

        /// <summary>
        /// Returns the status of push permission response after it's granted/denied
        /// </summary>
        /// <param name="message">String boolean if permission is accepted.</param>
        public virtual void CleverTapOnPushPermissionResponseCallback(string message) {
            // Ensure to create call the `CreateNotificationChannel` once notification permission is granted to register for receiving push notifications for Android 13+ devices.
            CleverTapLogger.Log("unity received push permission response: " + (!string.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapOnPushPermissionResponseCallback?.Invoke(message);
        }

        /// <summary>
        /// IOS Only callback. Use when checking if push permission is granted.
        /// </summary>
        /// <param name="message">String boolean if status is enabled.</param>
        public virtual void CleverTapPushNotificationPermissionStatus(string message)
        {
            CleverTapLogger.Log("unity received push status response: " + (!string.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapPushNotificationPermissionStatusCallback?.Invoke(message);
        }

        // returns when an in-app notification is dismissed by a call to action with custom extras
        public virtual void CleverTapInAppNotificationButtonTapped(string message) {
            CleverTapLogger.Log("unity received inapp notification button tapped: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapInAppNotificationButtonTapped?.Invoke(message);
        }

        // returns callback for InitializeInbox
        public virtual void CleverTapInboxDidInitializeCallback(string message) {
            CleverTapLogger.Log("unity received inbox initialized");
            _OnCleverTapInboxDidInitializeCallback?.Invoke();
        }

        public virtual void CleverTapInboxMessagesDidUpdateCallback(string message) {
            CleverTapLogger.Log("unity received inbox messages updated");
            _OnCleverTapInboxMessagesDidUpdateCallback?.Invoke();
        }

        // returns on the click of app inbox message with a map of custom Key-Value pairs
        public virtual void CleverTapInboxCustomExtrasButtonSelect(string message) {
            CleverTapLogger.Log("unity received inbox message button with custom extras select: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapInboxCustomExtrasButtonSelect?.Invoke(message);
        }

        // returns on the click of app inbox message with a string of the inbox payload along with page index and button index
        public virtual void CleverTapInboxItemClicked(string message) {
            CleverTapLogger.Log("unity received inbox message clicked callback: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapInboxItemClicked?.Invoke(message);
        }

        // returns native display units data
        public virtual void CleverTapNativeDisplayUnitsUpdated(string message) {
            CleverTapLogger.Log("unity received native display units updated: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapNativeDisplayUnitsUpdated?.Invoke(message);
        }

        [Obsolete("Product config is deprecated, use variables instead.")]
        public virtual void CleverTapProductConfigFetched(string message) {
            CleverTapLogger.Log("unity received product config fetched: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapProductConfigFetched?.Invoke(message);
        }

        [Obsolete("Product config is deprecated, use variables instead.")]
        public virtual void CleverTapProductConfigActivated(string message) {
            CleverTapLogger.Log("unity received product config activated: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapProductConfigActivated?.Invoke(message);
        }

        [Obsolete("Product config is deprecated, use variables instead.")]
        public virtual void CleverTapProductConfigInitialized(string message) {
            CleverTapLogger.Log("unity received product config initialized: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapProductConfigInitialized?.Invoke(message);
        }

        [Obsolete("Feature Flags are deprecated, use variables instead.")]
        public virtual void CleverTapFeatureFlagsUpdated(string message) {
            CleverTapLogger.Log("unity received feature flags updated: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
            _OnCleverTapFeatureFlagsUpdated?.Invoke(message);
        }

        #endregion

        #region Default - Variables Callbacks

        private void InvokeVariablesCallback(string message, bool isOnce = false)
        {
            try
            {
                JSONClass json = (JSONClass)JSON.Parse(message);
                int id = json["callbackId"].AsInt;

                if (variablesCallbacks.TryGetValue(id, out CleverTapCallbackDelegate callback))
                {
                    callback?.Invoke();
                    if (isOnce)
                    {
                        variablesCallbacks.Remove(id);
                    }
                }
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Exception invoking variables callback. Message: {message}, Exception: {ex}");
            }
        }

        // Invoked when variables changed
        public virtual void CleverTapVariablesChanged(string message) {
            CleverTapLogger.Log("Unity received variables changed");
            InvokeVariablesCallback(message);
        }
        
        // Invoked when variable values are changed and the files associated with them are downloaded and ready to be used.
        public virtual void CleverTapVariablesChangedAndNoDownloadsPending(string message) {
            CleverTapLogger.Log("Unity received variables changed and no downloads pending ");
            InvokeVariablesCallback(message);
        }

        public virtual void OneTimeCleverTapVariablesChanged(string message)
        {
            CleverTapLogger.Log("Unity received one time variables changed");
            InvokeVariablesCallback(message, true);
        }

        public virtual void OneTimeCleverTapVariablesChangedAndNoDownloadsPending(string message)
        {
            CleverTapLogger.Log("Unity received one time variables changed and no downloads pending ");
            InvokeVariablesCallback(message, true);
        }

        public virtual void CleverTapVariableFileIsReady(string variableName) {
            CleverTapLogger.Log("Unity received file is ready: " + (!string.IsNullOrEmpty(variableName) ? variableName : "NULL"));
            VariableFactory.CleverTapVariable.VariableFileIsReady(variableName);
        }
        
        // Invoked when a variable value changed
        public virtual void CleverTapVariableValueChanged(string variableName) {
            CleverTapLogger.Log("Unity received variable changed: " + (!String.IsNullOrEmpty(variableName) ? variableName : "NULL"));
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

        #region Default - InApps Callbacks

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

        #region Default - Custom Templates Callbacks
        public virtual void CleverTapCustomTemplatePresent(string templateName)
        {
            CleverTapLogger.Log($"Unity received template present: {templateName}");
            CleverTapTemplateContext context = CustomTemplatesFactory.CleverTapCustomTemplates.CreateContext(templateName);
            _OnCustomTemplatePresent?.Invoke(context);
        }

        public virtual void CleverTapCustomTemplateClose(string templateName)
        {
            CleverTapLogger.Log($"Unity received template close: {templateName}");
            CleverTapTemplateContext context = CustomTemplatesFactory.CleverTapCustomTemplates.CreateContext(templateName);
            _OnCustomTemplateClose?.Invoke(context);
        }

        public virtual void CleverTapCustomFunctionPresent(string templateName)
        {
            CleverTapLogger.Log($"Unity received function present: {templateName}");
            CleverTapTemplateContext context = CustomTemplatesFactory.CleverTapCustomTemplates.CreateContext(templateName);
            _OnCustomFunctionPresent?.Invoke(context);
        }
        #endregion
    }
}