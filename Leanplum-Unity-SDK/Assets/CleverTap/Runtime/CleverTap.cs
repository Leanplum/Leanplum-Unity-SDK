using CleverTapSDK.Common;
using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;
using System;
using System.Collections.Generic;

namespace CleverTapSDK {
    public static class CleverTap {

        private static CleverTapCallbackHandler cleverTapCallbackHandler = BindingFactory.CleverTapBinding.CallbackHandler;
        private static CleverTapPlatformBindings cleverTapBinding = BindingFactory.CleverTapBinding;
        private static CleverTapPlatformVariable cleverTapVariable = VariableFactory.CleverTapVariable;
        private static CleverTapPlatformInApps cleverTapInApps = InAppsFactory.CleverTapInApps;
        private static CleverTapPlatformCustomTemplates cleverTapCustomInApps = CustomTemplatesFactory.CleverTapCustomTemplates;

        #region Constants - CleverTap Version

        public const string VERSION = CleverTapVersion.CLEVERTAP_SDK_VERSION;

        #endregion

        #region Looger - Clever Tap Looger

        public static void SetLogLevel(LogLevel level) =>
            CleverTapLogger.SetLogLevel(level);

        #endregion

        #region Events - CleverTap Callback Handler

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapDeepLinkCallback {
            add => cleverTapCallbackHandler.OnCleverTapDeepLinkCallback += value;
            remove => cleverTapCallbackHandler.OnCleverTapDeepLinkCallback -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapProfileInitializedCallback {
            add => cleverTapCallbackHandler.OnCleverTapProfileInitializedCallback += value;
            remove => cleverTapCallbackHandler.OnCleverTapProfileInitializedCallback -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapProfileUpdatesCallback {
            add => cleverTapCallbackHandler.OnCleverTapProfileUpdatesCallback += value;
            remove => cleverTapCallbackHandler.OnCleverTapProfileUpdatesCallback -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapPushOpenedCallback {
            add => cleverTapCallbackHandler.OnCleverTapPushOpenedCallback += value;
            remove => cleverTapCallbackHandler.OnCleverTapPushOpenedCallback -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapPushNotificationTappedWithCustomExtrasCallback
        {
            add => cleverTapCallbackHandler.OnCleverTapPushNotificationTappedWithCustomExtrasCallback += value;
            remove => cleverTapCallbackHandler.OnCleverTapPushNotificationTappedWithCustomExtrasCallback -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapInitCleverTapIdCallback {
            add => cleverTapCallbackHandler.OnCleverTapInitCleverTapIdCallback += value;
            remove => cleverTapCallbackHandler.OnCleverTapInitCleverTapIdCallback -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapInAppNotificationDismissedCallback {
            add => cleverTapCallbackHandler.OnCleverTapInAppNotificationDismissedCallback += value;
            remove => cleverTapCallbackHandler.OnCleverTapInAppNotificationDismissedCallback -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapInAppNotificationShowCallback {
            add => cleverTapCallbackHandler.OnCleverTapInAppNotificationShowCallback += value;
            remove => cleverTapCallbackHandler.OnCleverTapInAppNotificationShowCallback -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapOnPushPermissionResponseCallback {
            add => cleverTapCallbackHandler.OnCleverTapOnPushPermissionResponseCallback += value;
            remove => cleverTapCallbackHandler.OnCleverTapOnPushPermissionResponseCallback -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapPushNotificationPermissionStatusCallback
        {
            add => cleverTapCallbackHandler.OnCleverTapPushNotificationPermissionStatusCallback += value;
            remove => cleverTapCallbackHandler.OnCleverTapPushNotificationPermissionStatusCallback -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapInAppNotificationButtonTapped {
            add => cleverTapCallbackHandler.OnCleverTapInAppNotificationButtonTapped += value;
            remove => cleverTapCallbackHandler.OnCleverTapInAppNotificationButtonTapped -= value;
        }

        public static event CleverTapCallbackDelegate OnCleverTapInboxDidInitializeCallback {
            add => cleverTapCallbackHandler.OnCleverTapInboxDidInitializeCallback += value;
            remove => cleverTapCallbackHandler.OnCleverTapInboxDidInitializeCallback -= value;
        }

        public static event CleverTapCallbackDelegate OnCleverTapInboxMessagesDidUpdateCallback {
            add => cleverTapCallbackHandler.OnCleverTapInboxMessagesDidUpdateCallback += value;
            remove => cleverTapCallbackHandler.OnCleverTapInboxMessagesDidUpdateCallback -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapInboxCustomExtrasButtonSelect {
            add => cleverTapCallbackHandler.OnCleverTapInboxCustomExtrasButtonSelect += value;
            remove => cleverTapCallbackHandler.OnCleverTapInboxCustomExtrasButtonSelect -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapInboxItemClicked {
            add => cleverTapCallbackHandler.OnCleverTapInboxItemClicked += value;
            remove => cleverTapCallbackHandler.OnCleverTapInboxItemClicked -= value;
        }

        public static event CleverTapCallbackWithMessageDelegate OnCleverTapNativeDisplayUnitsUpdated {
            add => cleverTapCallbackHandler.OnCleverTapNativeDisplayUnitsUpdated += value;
            remove => cleverTapCallbackHandler.OnCleverTapNativeDisplayUnitsUpdated -= value;
        }

        [Obsolete("Product config is deprecated, use variables instead.")]
        public static event CleverTapCallbackWithMessageDelegate OnCleverTapProductConfigFetched {
            add => cleverTapCallbackHandler.OnCleverTapProductConfigFetched += value;
            remove => cleverTapCallbackHandler.OnCleverTapProductConfigFetched -= value;
        }

        [Obsolete("Product config is deprecated, use variables instead.")]
        public static event CleverTapCallbackWithMessageDelegate OnCleverTapProductConfigActivated {
            add => cleverTapCallbackHandler.OnCleverTapProductConfigActivated += value;
            remove => cleverTapCallbackHandler.OnCleverTapProductConfigActivated -= value;
        }

        [Obsolete("Product config is deprecated, use variables instead.")]
        public static event CleverTapCallbackWithMessageDelegate OnCleverTapProductConfigInitialized {
            add => cleverTapCallbackHandler.OnCleverTapProductConfigInitialized += value;
            remove => cleverTapCallbackHandler.OnCleverTapProductConfigInitialized -= value;
        }

        [Obsolete("Feature Flags are deprecated, use variables instead.")]
        public static event CleverTapCallbackWithMessageDelegate OnCleverTapFeatureFlagsUpdated {
            add => cleverTapCallbackHandler.OnCleverTapFeatureFlagsUpdated += value;
            remove => cleverTapCallbackHandler.OnCleverTapFeatureFlagsUpdated -= value;
        }

        public static event CleverTapCallbackDelegate OnVariablesChanged {
            add => cleverTapCallbackHandler.OnVariablesChanged += value;
            remove => cleverTapCallbackHandler.OnVariablesChanged -= value;
        }

        public static event CleverTapCallbackDelegate OnOneTimeVariablesChanged {
            add => cleverTapCallbackHandler.OnOneTimeVariablesChanged += value;
            remove => cleverTapCallbackHandler.OnOneTimeVariablesChanged -= value;
        }

        public static event CleverTapCallbackDelegate OnVariablesChangedAndNoDownloadsPending {
            add => cleverTapCallbackHandler.OnVariablesChangedAndNoDownloadsPending += value;
            remove => cleverTapCallbackHandler.OnVariablesChangedAndNoDownloadsPending -= value;
        }
        
        public static event CleverTapCallbackDelegate OnOneTimeVariablesChangedAndNoDownloadsPending {
            add => cleverTapCallbackHandler.OnOneTimeVariablesChangedAndNoDownloadsPending += value;
            remove => cleverTapCallbackHandler.OnOneTimeVariablesChangedAndNoDownloadsPending -= value;
        }

        public static event CleverTapCallbackWithTemplateContext OnCustomTemplatePresent
        {
            add => cleverTapCallbackHandler.OnCustomTemplatePresent += value;
            remove => cleverTapCallbackHandler.OnCustomTemplatePresent -= value;
        }

        public static event CleverTapCallbackWithTemplateContext OnCustomTemplateClose
        {
            add => cleverTapCallbackHandler.OnCustomTemplateClose += value;
            remove => cleverTapCallbackHandler.OnCustomTemplateClose -= value;
        }

        public static event CleverTapCallbackWithTemplateContext OnCustomFunctionPresent
        {
            add => cleverTapCallbackHandler.OnCustomFunctionPresent += value;
            remove => cleverTapCallbackHandler.OnCustomFunctionPresent -= value;
        }
        #endregion

        #region Methods - CleverTap Platform Bindings

        [Obsolete("Product config is deprecated, use variables instead.")]
        public static void ActivateProductConfig() =>
            cleverTapBinding.ActivateProductConfig();

        public static void CreateNotificationChannel(string channelId, string channelName, string channelDescription, int importance, bool showBadge) =>
            cleverTapBinding.CreateNotificationChannel(channelId, channelName, channelDescription, importance, showBadge);

        public static void CreateNotificationChannelGroup(string groupId, string groupName) =>
            cleverTapBinding.CreateNotificationChannelGroup(groupId, groupName);

        public static void CreateNotificationChannelWithGroup(string channelId, string channelName, string channelDescription, int importance, string groupId, bool showBadge) =>
            cleverTapBinding.CreateNotificationChannelWithGroup(channelId, channelName, channelDescription, importance, groupId, showBadge);

        public static void CreateNotificationChannelWithGroupAndSound(string channelId, string channelName, string channelDescription, int importance, string groupId, bool showBadge, string sound) =>
            cleverTapBinding.CreateNotificationChannelWithGroupAndSound(channelId, channelName, channelDescription, importance, groupId, showBadge, sound);

        public static void CreateNotificationChannelWithSound(string channelId, string channelName, string channelDescription, int importance, bool showBadge, string sound) =>
            cleverTapBinding.CreateNotificationChannelWithSound(channelId, channelName, channelDescription, importance, showBadge, sound);

        public static void DeleteInboxMessageForID(string messageId) =>
            cleverTapBinding.DeleteInboxMessageForID(messageId);

        public static void DeleteInboxMessagesForIDs(string[] messageIds) =>
            cleverTapBinding.DeleteInboxMessagesForIDs(messageIds);

        public static void DeleteNotificationChannel(string channelId) =>
            cleverTapBinding.DeleteNotificationChannel(channelId);

        public static void DeleteNotificationChannelGroup(string groupId) =>
            cleverTapBinding.DeleteNotificationChannelGroup(groupId);

        public static void DisablePersonalization() =>
            cleverTapBinding.DisablePersonalization();

        public static void DiscardInAppNotifications() =>
            cleverTapBinding.DiscardInAppNotifications();

        public static void DismissAppInbox() =>
            cleverTapBinding.DismissAppInbox();

        public static void EnableDeviceNetworkInfoReporting(bool enabled) =>
            cleverTapBinding.EnableDeviceNetworkInfoReporting(enabled);

        public static void EnablePersonalization() =>
            cleverTapBinding.EnablePersonalization();

        [Obsolete("This method is deprecated, use GetUserEventLog(eventName, callback) instead.")]
        public static JSONClass EventGetDetail(string eventName) =>
            cleverTapBinding.EventGetDetail(eventName);

        [Obsolete("This method is deprecated, use GetUserEventLog(eventName, callback) instead.")]
        public static int EventGetFirstTime(string eventName) =>
            cleverTapBinding.EventGetFirstTime(eventName);

        [Obsolete("This method is deprecated, use GetUserEventLog(eventName, callback) instead.")]
        public static int EventGetLastTime(string eventName) =>
            cleverTapBinding.EventGetLastTime(eventName);

        [Obsolete("This method is deprecated, use GetUserEventLog(eventName, callback) instead.")]
        public static int EventGetOccurrences(string eventName) =>
            cleverTapBinding.EventGetOccurrences(eventName);

        [Obsolete("Product config is deprecated, use variables instead.")]
        public static void FetchAndActivateProductConfig() =>
            cleverTapBinding.FetchAndActivateProductConfig();

        [Obsolete("Product config is deprecated, use variables instead.")]
        public static void FetchProductConfig() =>
            cleverTapBinding.FetchProductConfig();

        [Obsolete("Product config is deprecated, use variables instead.")]
        public static void FetchProductConfigWithMinimumInterval(double minimumInterval) =>
            cleverTapBinding.FetchProductConfigWithMinimumInterval(minimumInterval);

        public static JSONArray GetAllDisplayUnits() =>
            cleverTapBinding.GetAllDisplayUnits();

        public static JSONArray GetAllInboxMessages() =>
            cleverTapBinding.GetAllInboxMessages();

        public static List<CleverTapInboxMessage> GetAllInboxMessagesParsed() =>
            cleverTapBinding.GetAllInboxMessagesParsed();

        public static string GetCleverTapID() =>
            cleverTapBinding.GetCleverTapID();

        public static JSONClass GetDisplayUnitForID(string unitID) =>
            cleverTapBinding.GetDisplayUnitForID(unitID);

        [Obsolete("Feature flags are deprecated, use variables instead.")]
        public static bool GetFeatureFlag(string key, bool defaultValue) =>
            cleverTapBinding.GetFeatureFlag(key, defaultValue);

        public static int GetInboxMessageCount() =>
            cleverTapBinding.GetInboxMessageCount();

        public static JSONClass GetInboxMessageForId(string messageId) =>
            cleverTapBinding.GetInboxMessageForId(messageId);

        public static CleverTapInboxMessage GetInboxMessageForIdParsed(string messageId) =>
            cleverTapBinding.GetInboxMessageForIdParsed(messageId);

        public static int GetInboxMessageUnreadCount() =>
            cleverTapBinding.GetInboxMessageUnreadCount();

        [Obsolete("Product config is deprecated, use variables instead.")]
        public static double GetProductConfigLastFetchTimeStamp() =>
            cleverTapBinding.GetProductConfigLastFetchTimeStamp();

        /// <summary>
        /// Get the value of a product config key. This method only works on iOS Platform.
        /// Use <see cref="GetProductConfigString(string)"/>, <see cref="GetProductConfigBoolean(string)"/>,
        /// <see cref="GetProductConfigLong(string)"/>, or <see cref="GetProductConfigDouble(string)"/>
        /// for Android Platform.
        /// </summary>
        /// <param name="key"> The key for the product config</param>
        /// <returns>The value for the key as JSONClass</returns>
        [Obsolete("Product config is deprecated, use variables instead.")]
        public static JSONClass GetProductConfigValueFor(string key) =>
            cleverTapBinding.GetProductConfigValueFor(key);

        /// <summary>
        /// Get the value of a string product config key. This method only works on Android Platform.
        /// Use <see cref="GetProductConfigValueFor(string)"/> for iOS Platform.
        /// </summary>
        /// <param name="key"> The key for the product config</param>
        [Obsolete("Product config is deprecated, use variables instead.")]
        public static string GetProductConfigString(string key) =>
            cleverTapBinding.GetProductConfigString(key);

        /// <summary>
        /// Get the value of a bool product config key. This method only works on Android Platform.
        /// Use <see cref="GetProductConfigValueFor(string)"/> for iOS Platform.
        /// </summary>
        /// <param name="key"> The key for the product config</param>
        [Obsolete("Product config is deprecated, use variables instead.")]
        public static bool? GetProductConfigBoolean(string key) =>
            cleverTapBinding.GetProductConfigBoolean(key);
        

        /// <summary>
        /// Get the value of a long product config key. This method only works on Android Platform.
        /// Use <see cref="GetProductConfigValueFor(string)"/> for iOS Platform.
        /// </summary>
        /// <param name="key"> The key for the product config</param>
        [Obsolete("Product config is deprecated, use variables instead.")]
        public static long? GetProductConfigLong(string key) =>
            cleverTapBinding.GetProductConfigLong(key);
        

        /// <summary>
        /// Get the value of a long product config key. This method only works on Android Platform.
        /// Use <see cref="GetProductConfigValueFor(string)"/> for iOS Platform.
        /// </summary>
        /// <param name="key"> The key for the product config</param>
        [Obsolete("Product config is deprecated, use variables instead.")]
        public static double? GetProductConfigDouble(string key) =>
            cleverTapBinding.GetProductConfigDouble(key);    

        public static JSONArray GetUnreadInboxMessages() =>
            cleverTapBinding.GetUnreadInboxMessages();

        public static List<CleverTapInboxMessage> GetUnreadInboxMessagesParsed() =>
            cleverTapBinding.GetUnreadInboxMessagesParsed();

        public static void InitializeInbox() =>
            cleverTapBinding.InitializeInbox();

        public static bool IsPushPermissionGranted() =>
            cleverTapBinding.IsPushPermissionGranted();

        public static void LaunchWithCredentials(string accountID, string token) =>
            cleverTapBinding.LaunchWithCredentials(accountID, token);

        public static void LaunchWithCredentialsForRegion(string accountID, string token, string region) =>
            cleverTapBinding.LaunchWithCredentialsForRegion(accountID, token, region);

        public static void LaunchWithCredentialsForProxyServer(string accountID, string token, string proxyDomain, string spikyProxyDomain) =>
            cleverTapBinding.LaunchWithCredentialsForProxyServer(accountID, token, proxyDomain, spikyProxyDomain);
        
        public static void MarkReadInboxMessageForID(string messageId) =>
            cleverTapBinding.MarkReadInboxMessageForID(messageId);

        public static void MarkReadInboxMessagesForIDs(string[] messageIds) =>
            cleverTapBinding.MarkReadInboxMessagesForIDs(messageIds);

        public static void OnUserLogin(Dictionary<string, string> properties) =>
            cleverTapBinding.OnUserLogin(properties);

        public static void OnUserLogin(Dictionary<string, object> properties) =>
            cleverTapBinding.OnUserLogin(properties);

        public static void ProfileAddMultiValueForKey(string key, string val) =>
            cleverTapBinding.ProfileAddMultiValueForKey(key, val);

        public static void ProfileAddMultiValuesForKey(string key, List<string> values) =>
            cleverTapBinding.ProfileAddMultiValuesForKey(key, values);

        public static void ProfileDecrementValueForKey(string key, double val) =>
            cleverTapBinding.ProfileDecrementValueForKey(key, val);

        public static void ProfileDecrementValueForKey(string key, int val) =>
            cleverTapBinding.ProfileDecrementValueForKey(key, val);

        public static string ProfileGet(string key) =>
            cleverTapBinding.ProfileGet(key);

        public static string ProfileGetCleverTapAttributionIdentifier() =>
            cleverTapBinding.ProfileGetCleverTapAttributionIdentifier();

        public static string ProfileGetCleverTapID() =>
            cleverTapBinding.ProfileGetCleverTapID();

        public static void ProfileIncrementValueForKey(string key, double val) =>
            cleverTapBinding.ProfileIncrementValueForKey(key, val);

        public static void ProfileIncrementValueForKey(string key, int val) =>
            cleverTapBinding.ProfileIncrementValueForKey(key, val);

        public static void ProfilePush(Dictionary<string, string> properties) =>
            cleverTapBinding.ProfilePush(properties);

        public static void ProfilePush(Dictionary<string, object> properties) =>
            cleverTapBinding.ProfilePush(properties);

        public static void ProfileRemoveMultiValueForKey(string key, string val) =>
            cleverTapBinding.ProfileRemoveMultiValueForKey(key, val);

        public static void ProfileRemoveMultiValuesForKey(string key, List<string> values) =>
            cleverTapBinding.ProfileRemoveMultiValuesForKey(key, values);

        public static void ProfileRemoveValueForKey(string key) =>
            cleverTapBinding.ProfileRemoveValueForKey(key);

        public static void ProfileSetMultiValuesForKey(string key, List<string> values) =>
            cleverTapBinding.ProfileSetMultiValuesForKey(key, values);

        public static void PromptForPushPermission(bool showFallbackSettings) =>
            cleverTapBinding.PromptForPushPermission(showFallbackSettings);

        public static void PromptPushPrimer(Dictionary<string, object> json) =>
            cleverTapBinding.PromptPushPrimer(json);

        public static void PushInstallReferrerSource(string source, string medium, string campaign) =>
            cleverTapBinding.PushInstallReferrerSource(source, medium, campaign);

        public static void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>> items) =>
            cleverTapBinding.RecordChargedEventWithDetailsAndItems(details, items);

        public static void RecordDisplayUnitClickedEventForID(string unitID) =>
            cleverTapBinding.RecordDisplayUnitClickedEventForID(unitID);

        public static void RecordDisplayUnitViewedEventForID(string unitID) =>
            cleverTapBinding.RecordDisplayUnitViewedEventForID(unitID);

        public static void RecordEvent(string eventName) =>
            cleverTapBinding.RecordEvent(eventName);

        public static void RecordEvent(string eventName, Dictionary<string, object> properties) =>
            cleverTapBinding.RecordEvent(eventName, properties);

        public static void RecordInboxNotificationClickedEventForID(string messageId) =>
            cleverTapBinding.RecordInboxNotificationClickedEventForID(messageId);

        public static void RecordInboxNotificationViewedEventForID(string messageId) =>
            cleverTapBinding.RecordInboxNotificationViewedEventForID(messageId);

        public static void RecordScreenView(string screenName) =>
            cleverTapBinding.RecordScreenView(screenName);

        public static void RegisterPush() =>
            cleverTapBinding.RegisterPush();

        [Obsolete("Product config is deprecated, use variables instead.")]
        public static void ResetProductConfig() =>
            cleverTapBinding.ResetProductConfig();

        public static void ResumeInAppNotifications() =>
            cleverTapBinding.ResumeInAppNotifications();

        public static int SessionGetTimeElapsed() =>
            cleverTapBinding.SessionGetTimeElapsed();

        public static JSONClass SessionGetUTMDetails() =>
            cleverTapBinding.SessionGetUTMDetails();

        public static void SetApplicationIconBadgeNumber(int num) =>
            cleverTapBinding.SetApplicationIconBadgeNumber(num);

        public static void SetDebugLevel(int level) =>
            cleverTapBinding.SetDebugLevel(level);

        public static void SetLocation(double lat, double lon) =>
            cleverTapBinding.SetLocation(lat, lon);

        public static void SetOffline(bool enabled) =>
            cleverTapBinding.SetOffline(enabled);

        public static void SetOptOut(bool enabled) =>
            cleverTapBinding.SetOptOut(enabled);

        [Obsolete("Product config is deprecated, use variables instead.")]
        public static void SetProductConfigDefaults(Dictionary<string, object> defaults) =>
            cleverTapBinding.SetProductConfigDefaults(defaults);

        [Obsolete("Product config is deprecated, use variables instead.")]
        public static void SetProductConfigDefaultsFromPlistFileName(string fileName) =>
            cleverTapBinding.SetProductConfigDefaultsFromPlistFileName(fileName);

        [Obsolete("Product config is deprecated, use variables instead.")]
        public static void SetProductConfigMinimumFetchInterval(double minimumFetchInterval) =>
            cleverTapBinding.SetProductConfigMinimumFetchInterval(minimumFetchInterval);

        public static void ShowAppInbox(Dictionary<string, object> styleConfig) =>
            cleverTapBinding.ShowAppInbox(styleConfig);

        public static void ShowAppInbox(string styleConfig) =>
            cleverTapBinding.ShowAppInbox(styleConfig);

        public static void SuspendInAppNotifications() =>
            cleverTapBinding.SuspendInAppNotifications();

        [Obsolete("This method is deprecated, use GetUserEventLogHistory() instead.")]
        public static JSONClass UserGetEventHistory() =>
            cleverTapBinding.UserGetEventHistory();

        [Obsolete("This method is deprecated, use GetUserLastVisitTs() instead.")]
        public static int UserGetPreviousVisitTime() =>
            cleverTapBinding.UserGetPreviousVisitTime();

        public static int UserGetScreenCount() =>
            cleverTapBinding.UserGetScreenCount();

        [Obsolete("This method is deprecated, use GetUserAppLaunchCount() instead.")]
        public static int UserGetTotalVisits() =>
            cleverTapBinding.UserGetTotalVisits();

        public static void GetUserEventLog(string eventName, CleverTapCallback<UserEventLog> callback) =>
            cleverTapBinding.GetUserEventLog(eventName, callback);

        public static void GetUserEventLogCount(string eventName, CleverTapCallback<int> callback) =>
            cleverTapBinding.GetUserEventLogCount(eventName, callback);

        public static void GetUserAppLaunchCount(CleverTapCallback<int> callback) =>
            cleverTapBinding.GetUserAppLaunchCount(callback);

        public static void GetUserEventLogHistory(CleverTapCallback<Dictionary<string, UserEventLog>> callback) =>
            cleverTapBinding.GetUserEventLogHistory(callback);

        public static long GetUserLastVisitTs() =>
            cleverTapBinding.GetUserLastVisitTs();

        #endregion

        #region Methods - CleverTap Platform Variables

        public static Var<T> GetVariable<T>(string name) =>
            cleverTapVariable.GetVariable<T>(name);

        public static Var<int> Define(string name, int defaultValue) =>
            cleverTapVariable.Define(name, defaultValue);

        public static Var<long> Define(string name, long defaultValue) => 
            cleverTapVariable.Define(name, defaultValue);
        
        public static Var<short> Define(string name, short defaultValue) =>
            cleverTapVariable.Define(name, defaultValue);

        public static Var<byte> Define(string name, byte defaultValue) =>
            cleverTapVariable.Define(name, defaultValue);

        public static Var<bool> Define(string name, bool defaultValue) =>
            cleverTapVariable.Define(name, defaultValue);

        public static Var<float> Define(string name, float defaultValue) =>
            cleverTapVariable.Define(name, defaultValue);

        public static Var<double> Define(string name, double defaultValue) =>
            cleverTapVariable.Define(name, defaultValue);

        public static Var<string> Define(string name, string defaultValue) =>
            cleverTapVariable.Define(name, defaultValue);

        public static Var<Dictionary<string, object>> Define(string name, Dictionary<string, object> defaultValue) =>
            cleverTapVariable.Define(name, defaultValue);

        public static Var<Dictionary<string, string>> Define(string name, Dictionary<string, string> defaultValue) =>
            cleverTapVariable.Define(name, defaultValue);

        public static Var<string> DefineFileVariable(string name) =>
            cleverTapVariable.DefineFileVariable(name);
        
        public static void SyncVariables() => 
            cleverTapVariable.SyncVariables();

        public static void SyncVariables(bool isProduction) =>
            cleverTapVariable.SyncVariables(isProduction);

        public static void FetchVariables(Action<bool> isSucessCallback) => 
            cleverTapVariable.FetchVariables(isSucessCallback);

        #endregion

        #region Methods - CleverTap Platform InApps

        public static void FetchInApps(Action<bool> isSucessCallback) =>
            cleverTapInApps.FetchInApps(isSucessCallback);

        public static void ClearInAppResources(bool expiredOnly) =>
            cleverTapInApps.ClearInAppResources(expiredOnly);

        #endregion

        #region Methods - CleverTap Platform Custom Templates

        /// <summary>
        /// Sync all currently defined templates (through the templates json definition) to the backend.
        /// Use this method to transfer template definitions from the SDK to the CT dashboard. This method can
        /// only be called from a debug build of the application and the current logged-in user must be marked as a
        /// test profile through the web dashboard.
        /// </summary>
        public static void SyncCustomTemplates() =>
            cleverTapCustomInApps.SyncCustomTemplates();

        /// <summary>
        /// iOS specific version of <see cref="SyncCustomTemplates"/>
        /// </summary>
        /// <param name="isProduction">Provide `true` if the templates must be synced in Production build/configuration.</param>
        public static void SyncCustomTemplates(bool isProduction) =>
            cleverTapCustomInApps.SyncCustomTemplates(isProduction);

        #endregion
    }
}
