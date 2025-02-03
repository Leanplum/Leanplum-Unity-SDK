#if UNITY_IOS
using System;
using System.Collections.Generic;
using AOT;
using CleverTapSDK.Common;
using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;
using static CleverTapSDK.IOS.IOSCallbackHandler;

namespace CleverTapSDK.IOS
{
    internal class IOSPlatformBinding : CleverTapPlatformBindings {

        private static CleverTapCallbackHandler staticCallbackHandler;

        internal IOSPlatformBinding() {
            CallbackHandler = CreateGameObjectAndAttachCallbackHandler<IOSCallbackHandler>(CleverTapGameObjectName.IOS_CALLBACK_HANDLER);
            staticCallbackHandler = CallbackHandler;
            CleverTapLogger.Log("Start: CleverTap binding for iOS.");

            IOSDllImport.CleverTap_onPlatformInit();
            IOSDllImport.CleverTap_setInAppNotificationButtonTappedCallback(InAppNotificationButtonTappedInternal);
        }

        // Must be static: IL2CPP does not support marshaling delegates that point to instance methods to native code.
        [MonoPInvokeCallback(typeof(InAppNotificationButtonTapped))]
        public static void InAppNotificationButtonTappedInternal(string customData)
        {
            if (staticCallbackHandler == null)
            {
                CleverTapLogger.LogError("CallbackHandler is null. Cannot call CleverTapInAppNotificationButtonTapped.");
                return;
            }

            staticCallbackHandler.CleverTapInAppNotificationButtonTapped(customData);
        }

        [Obsolete]
        internal override void ActivateProductConfig() {
            IOSDllImport.CleverTap_activateProductConfig();
        }

        internal override void DeleteInboxMessageForID(string messageId) {
            IOSDllImport.CleverTap_deleteInboxMessageForID(messageId);
        }

        internal override void DeleteInboxMessagesForIDs(string[] messageIds) {
            IOSDllImport.CleverTap_deleteInboxMessagesForIDs(messageIds, messageIds.Length);
        }

        internal override void DisablePersonalization() {
            IOSDllImport.CleverTap_disablePersonalization();
        }

        internal override void DiscardInAppNotifications() {
            IOSDllImport.CleverTap_discardInAppNotifications();
        }

        internal override void DismissAppInbox() {
            IOSDllImport.CleverTap_dismissAppInbox();
        }

        internal override void EnableDeviceNetworkInfoReporting(bool enabled) {
            IOSDllImport.CleverTap_enableDeviceNetworkInfoReporting(enabled);
        }

        internal override void EnablePersonalization() {
            IOSDllImport.CleverTap_enablePersonalization();
        }

        [Obsolete]
        internal override JSONClass EventGetDetail(string eventName) {
            string jsonString = IOSDllImport.CleverTap_eventGetDetail(eventName);
            JSONClass json;
            try {
                json = (JSONClass)JSON.Parse(jsonString);
            } catch {
                CleverTapLogger.LogError("Unable to parse event detail json");
                json = new JSONClass();
            }
            return json;
        }

        [Obsolete]
        internal override int EventGetFirstTime(string eventName) {
            return IOSDllImport.CleverTap_eventGetFirstTime(eventName);
        }

        [Obsolete]
        internal override int EventGetLastTime(string eventName) {
            return IOSDllImport.CleverTap_eventGetLastTime(eventName);
        }

        [Obsolete]
        internal override int EventGetOccurrences(string eventName) {
            return IOSDllImport.CleverTap_eventGetOccurrences(eventName);
        }

        [Obsolete]
        internal override void FetchAndActivateProductConfig() {
            IOSDllImport.CleverTap_fetchAndActivateProductConfig();
        }

        [Obsolete]
        internal override void FetchProductConfig() {
            IOSDllImport.CleverTap_fetchProductConfig();
        }

        [Obsolete]
        internal override void FetchProductConfigWithMinimumInterval(double minimumInterval) {
            IOSDllImport.CleverTap_fetchProductConfigWithMinimumInterval(minimumInterval);
        }

        internal override JSONArray GetAllDisplayUnits() {
            string jsonString = IOSDllImport.CleverTap_getAllDisplayUnits();
            JSONArray json;
            try {
                json = (JSONArray)JSON.Parse(jsonString);
            } catch {
                CleverTapLogger.LogError("Unable to parse native display units json");
                json = new JSONArray();
            }
            return json;
        }

        internal override JSONArray GetAllInboxMessages()
        {
            string jsonString = IOSDllImport.CleverTap_getAllInboxMessages();
            JSONArray json;
            try
            {
                json = (JSONArray)JSON.Parse(jsonString);
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Unable to parse app inbox messages json: {ex}");
                json = new JSONArray();
            }
            return json;
        }

        internal override List<CleverTapInboxMessage> GetAllInboxMessagesParsed()
        {
            string jsonString = IOSDllImport.CleverTap_getAllInboxMessages();
            try
            {
                return CleverTapInboxMessageJSONParser.ParseJsonArray(jsonString);
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Unable to parse app inbox messages to CleverTapInboxMessage list: {ex}");
                return new List<CleverTapInboxMessage>();
            }
        }

        internal override string GetCleverTapID() {
            return IOSDllImport.CleverTap_getCleverTapID();
        }

        internal override JSONClass GetDisplayUnitForID(string unitID) {
            string jsonString = IOSDllImport.CleverTap_getDisplayUnitForID(unitID);
            JSONClass json;
            try {
                json = (JSONClass)JSON.Parse(jsonString);
            } catch {
                CleverTapLogger.LogError("Unable to parse native display unit json");
                json = new JSONClass();
            }
            return json;
        }

        [Obsolete("Feature Flags are deprecated, use variables instead.")]
        internal override bool GetFeatureFlag(string key, bool defaultValue) {
            return IOSDllImport.CleverTap_getFeatureFlag(key, defaultValue);
        }

        internal override int GetInboxMessageCount() {
            return IOSDllImport.CleverTap_getInboxMessageCount();
        }

        internal override JSONClass GetInboxMessageForId(string messageId)
        {
            string jsonString = IOSDllImport.CleverTap_getInboxMessageForId(messageId);
            JSONClass json;
            try
            {
                json = (JSONClass)JSON.Parse(jsonString);
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Unable to parse app inbox message json for id: {messageId}. Exception: {ex}.");
                json = new JSONClass();
            }
            return json;
        }

        internal override CleverTapInboxMessage GetInboxMessageForIdParsed(string messageId)
        {
            string jsonString = IOSDllImport.CleverTap_getInboxMessageForId(messageId);
            try
            {
                return CleverTapInboxMessageJSONParser.ParseJsonMessage(jsonString);
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Unable to parse inbox message to CleverTapInboxMessage for id: {messageId}. Exception: {ex}.");
                return null;
            }
        }

        internal override int GetInboxMessageUnreadCount() {
            return IOSDllImport.CleverTap_getInboxMessageUnreadCount();
        }

        [Obsolete]
        internal override double GetProductConfigLastFetchTimeStamp() {
            return IOSDllImport.CleverTap_getProductConfigLastFetchTimeStamp();
        }

        [Obsolete]
        internal override JSONClass GetProductConfigValueFor(string key) {
            string jsonString = IOSDllImport.CleverTap_getProductConfigValueFor(key);
            JSONClass json;
            try {
                json = (JSONClass)JSON.Parse(jsonString);
            } catch {
                CleverTapLogger.LogError("Unable to parse product config value");
                json = new JSONClass();
            }
            return json;
        }

        internal override JSONArray GetUnreadInboxMessages()
        {
            string jsonString = IOSDllImport.CleverTap_getUnreadInboxMessages();
            JSONArray json;
            try
            {
                json = (JSONArray)JSON.Parse(jsonString);
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Unable to parse unread inbox messages json: {ex}.");
                json = new JSONArray();
            }
            return json;
        }

        internal override List<CleverTapInboxMessage> GetUnreadInboxMessagesParsed()
        {
            string jsonString = IOSDllImport.CleverTap_getUnreadInboxMessages();
            try
            {
                return CleverTapInboxMessageJSONParser.ParseJsonArray(jsonString);
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Unable to parse unread inbox messages to CleverTapInboxMessage list: {ex}");
                return new List<CleverTapInboxMessage>();
            }
        }

        internal override void InitializeInbox() {
            IOSDllImport.CleverTap_initializeInbox();
        }

        /// <summary>
        /// Checks if push permission is granted.
        /// Use the <see cref="OnCleverTapPushNotificationPermissionStatusCallback"/> to get the result.
        /// </summary>
        /// <returns>
        /// Do not use the returned result. Returns false.
        /// Use the permission status callback for result.
        /// </returns>
        internal override bool IsPushPermissionGranted() {
            IOSDllImport.CleverTap_isPushPermissionGranted();
            return false;
        }

#pragma warning disable CS0809
        [Obsolete("This method no longer does anything. " +
            "Replaced with initialization in the application:didFinishLaunchingWithOptions:")]
        internal override void LaunchWithCredentials(string accountID, string token) {
        }

        [Obsolete("This method no longer does anything. " +
    "Replaced with initialization in the application:didFinishLaunchingWithOptions:")]
        internal override void LaunchWithCredentialsForRegion(string accountID, string token, string region) {
        }

        [Obsolete("This method no longer does anything. " +
    "Replaced with initialization in the application:didFinishLaunchingWithOptions:")]
        internal override void LaunchWithCredentialsForProxyServer(string accountID, string token, string proxyDomain, string spikyProxyDomain) {
        }
#pragma warning restore CS0809

        internal override void MarkReadInboxMessageForID(string messageId) {
            IOSDllImport.CleverTap_markReadInboxMessageForID(messageId);
        }

        internal override void MarkReadInboxMessagesForIDs(string[] messageIds) {
            int arrLength = messageIds.Length;
            IOSDllImport.CleverTap_markReadInboxMessagesForIDs(messageIds, arrLength);
        }

        internal override void OnUserLogin(Dictionary<string, object> properties) {
            IOSDllImport.CleverTap_onUserLogin(Json.Serialize(properties.ConvertDateObjects()));
        }

        internal override void ProfileAddMultiValueForKey(string key, string val) {
            IOSDllImport.CleverTap_profileAddMultiValueForKey(key, val);
        }

        internal override void ProfileAddMultiValuesForKey(string key, List<string> values) {
            IOSDllImport.CleverTap_profileAddMultiValuesForKey(key, values.ToArray(), values.Count);
        }

        internal override void ProfileDecrementValueForKey(string key, double val) {
            IOSDllImport.CleverTap_profileDecrementDoubleValueForKey(key, val);
        }

        internal override void ProfileDecrementValueForKey(string key, int val) {
            IOSDllImport.CleverTap_profileDecrementIntValueForKey(key, val);
        }

        internal override string ProfileGet(string key) {
            return IOSDllImport.CleverTap_profileGet(key);
        }

        internal override string ProfileGetCleverTapAttributionIdentifier() {
            return IOSDllImport.CleverTap_profileGetCleverTapAttributionIdentifier(); ;
        }

        internal override string ProfileGetCleverTapID() {
            return IOSDllImport.CleverTap_profileGetCleverTapID(); ;
        }

        internal override void ProfileIncrementValueForKey(string key, double val) {
            IOSDllImport.CleverTap_profileIncrementDoubleValueForKey(key, val);
        }

        internal override void ProfileIncrementValueForKey(string key, int val) {
            IOSDllImport.CleverTap_profileIncrementIntValueForKey(key, val);
        }

        internal override void ProfilePush(Dictionary<string, object> properties) {
            IOSDllImport.CleverTap_profilePush(Json.Serialize(properties.ConvertDateObjects()));
        }

        internal override void ProfileRemoveMultiValueForKey(string key, string val) {
            IOSDllImport.CleverTap_profileRemoveMultiValueForKey(key, val);
        }

        internal override void ProfileRemoveMultiValuesForKey(string key, List<string> values) {
            IOSDllImport.CleverTap_profileRemoveMultiValuesForKey(key, values.ToArray(), values.Count);
        }

        internal override void ProfileRemoveValueForKey(string key) {
            IOSDllImport.CleverTap_profileRemoveValueForKey(key);
        }

        internal override void ProfileSetMultiValuesForKey(string key, List<string> values) {
            IOSDllImport.CleverTap_profileSetMultiValuesForKey(key, values.ToArray(), values.Count);
        }

        internal override void PromptForPushPermission(bool showFallbackSettings) {
            IOSDllImport.CleverTap_promptForPushPermission(showFallbackSettings);
        }

        internal override void PromptPushPrimer(Dictionary<string, object> json) {
            var jsonString = Json.Serialize(json);
            IOSDllImport.CleverTap_promptPushPrimer(jsonString);
        }

        internal override void PushInstallReferrerSource(string source, string medium, string campaign) {
            IOSDllImport.CleverTap_pushInstallReferrerSource(source, medium, campaign);
        }

        internal override void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>> items) {
            var detailsString = Json.Serialize(details.ConvertDateObjects());
            var itemsString = Json.Serialize(items);
            IOSDllImport.CleverTap_recordChargedEventWithDetailsAndItems(detailsString, itemsString);
        }

        internal override void RecordDisplayUnitClickedEventForID(string unitID) {
            IOSDllImport.CleverTap_recordDisplayUnitClickedEventForID(unitID);
        }

        internal override void RecordDisplayUnitViewedEventForID(string unitID) {
            IOSDllImport.CleverTap_recordDisplayUnitViewedEventForID(unitID);
        }

        internal override void RecordEvent(string eventName) {
            IOSDllImport.CleverTap_recordEvent(eventName, null);
        }

        internal override void RecordEvent(string eventName, Dictionary<string, object> properties) {
            var propertiesString = Json.Serialize(properties.ConvertDateObjects());
            IOSDllImport.CleverTap_recordEvent(eventName, propertiesString);
        }

        internal override void RecordInboxNotificationClickedEventForID(string messageId) {
            IOSDllImport.CleverTap_recordInboxNotificationClickedEventForID(messageId);
        }

        internal override void RecordInboxNotificationViewedEventForID(string messageId) {
            IOSDllImport.CleverTap_recordInboxNotificationViewedEventForID(messageId);
        }

        internal override void RecordScreenView(string screenName) {
            IOSDllImport.CleverTap_recordScreenView(screenName);
        }

        internal override void RegisterPush() {
            IOSDllImport.CleverTap_registerPush();
        }

        [Obsolete]
        internal override void ResetProductConfig() {
            IOSDllImport.CleverTap_resetProductConfig();
        }

        internal override void ResumeInAppNotifications() {
            IOSDllImport.CleverTap_resumeInAppNotifications();
        }

        internal override int SessionGetTimeElapsed() {
            return IOSDllImport.CleverTap_sessionGetTimeElapsed();
        }

        internal override JSONClass SessionGetUTMDetails() {
            string jsonString = IOSDllImport.CleverTap_sessionGetUTMDetails();
            JSONClass json;
            try {
                json = (JSONClass)JSON.Parse(jsonString);
            } catch {
                CleverTapLogger.LogError("Unable to parse session utm details json");
                json = new JSONClass();
            }
            return json;
        }

        internal override void SetApplicationIconBadgeNumber(int num) {
            IOSDllImport.CleverTap_setApplicationIconBadgeNumber(num);
        }

        internal override void SetDebugLevel(int level) {
            IOSDllImport.CleverTap_setDebugLevel(level);
        }

        internal override void SetLocation(double lat, double lon) {
            IOSDllImport.CleverTap_setLocation(lat, lon);
        }

        internal override void SetOffline(bool enabled) {
            IOSDllImport.CleverTap_setOffline(enabled);
        }

        internal override void SetOptOut(bool enabled) {
            IOSDllImport.CleverTap_setOptOut(enabled);
        }

        [Obsolete]
        internal override void SetProductConfigDefaults(Dictionary<string, object> defaults) {
            var defaultsString = Json.Serialize(defaults);
            IOSDllImport.CleverTap_setProductConfigDefaults(defaultsString);
        }

        [Obsolete]
        internal override void SetProductConfigDefaultsFromPlistFileName(string fileName) {
            IOSDllImport.CleverTap_setProductConfigDefaultsFromPlistFileName(fileName);
        }

        [Obsolete]
        internal override void SetProductConfigMinimumFetchInterval(double minimumFetchInterval) {
            IOSDllImport.CleverTap_setProductConfigMinimumFetchInterval(minimumFetchInterval);
        }

        internal override void ShowAppInbox(Dictionary<string, object> styleConfig) {
            var styleConfigString = Json.Serialize(styleConfig);
            IOSDllImport.CleverTap_showAppInbox(styleConfigString);
        }

        internal override void ShowAppInbox(string styleConfig) {
            // TODO : Validate if we can use it like this (following Android implemenation)
            var styleConfigString = Json.Serialize(new Dictionary<string, object> { { "showAppInbox", styleConfig } });
            IOSDllImport.CleverTap_showAppInbox(styleConfigString);
        }

        internal override void SuspendInAppNotifications() {
            IOSDllImport.CleverTap_suspendInAppNotifications();
        }

        [Obsolete]
        internal override JSONClass UserGetEventHistory() {
            string jsonString = IOSDllImport.CleverTap_userGetEventHistory();
            JSONClass json;
            try {
                json = (JSONClass)JSON.Parse(jsonString);
            } catch {
                CleverTapLogger.LogError("Unable to parse user event history json");
                json = new JSONClass();
            }
            return json;
        }

        [Obsolete]
        internal override int UserGetPreviousVisitTime() {
            return IOSDllImport.CleverTap_userGetPreviousVisitTime();
        }

        internal override int UserGetScreenCount() {
            return IOSDllImport.CleverTap_userGetScreenCount();
        }

        [Obsolete]
        internal override int UserGetTotalVisits() {
            return IOSDllImport.CleverTap_userGetTotalVisits();
        }

        private static readonly Dictionary<string, CleverTapCallback<UserEventLog>> userEventLogCallbacks = new Dictionary<string, CleverTapCallback<UserEventLog>>();
        private static readonly Dictionary<string, CleverTapCallback<int>> userEventLogCountCallbacks = new Dictionary<string, CleverTapCallback<int>>();
        private static readonly Dictionary<string, CleverTapCallback<Dictionary<string, UserEventLog>>> userEventLogHistoryCallbacks =
            new Dictionary<string, CleverTapCallback<Dictionary<string, UserEventLog>>>();

        [MonoPInvokeCallback(typeof(UserEventLogCallback))]
        public static void UserEventLogCallbackFunc(string key, string message)
        {
            if (userEventLogCallbacks.ContainsKey(key))
            {
                var callback = userEventLogCallbacks[key];
                callback.Invoke(UserEventLog.Parse(message));
                userEventLogCallbacks.Remove(key);
            }
        }

        [MonoPInvokeCallback(typeof(UserEventLogCallback))]
        public static void UserEventLogCountCallbackFunc(string key, string message)
        {
            if (userEventLogCountCallbacks.ContainsKey(key))
            {
                var callback = userEventLogCountCallbacks[key];
                if (int.TryParse(message, out int count))
                    callback.Invoke(count);
                userEventLogCountCallbacks.Remove(key);
            }
        }

        [MonoPInvokeCallback(typeof(UserEventLogCallback))]
        public static void UserEventLogHistoryCallbackFunc(string key, string message)
        {
            if (userEventLogHistoryCallbacks.ContainsKey(key))
            {
                var callback = userEventLogHistoryCallbacks[key];
                callback.Invoke(UserEventLog.ParseLogsDictionary(message));
                userEventLogHistoryCallbacks.Remove(key);
            }
        }

        internal override void GetUserEventLog(string eventName, CleverTapCallback<UserEventLog> callback)
        {
            string key = Guid.NewGuid().ToString();
            userEventLogCallbacks[key] = callback;
            IOSDllImport.CleverTap_getUserEventLog(eventName, key, UserEventLogCallbackFunc);
        }

        internal override void GetUserAppLaunchCount(CleverTapCallback<int> callback)
        {
            string key = Guid.NewGuid().ToString();
            userEventLogCountCallbacks[key] = callback;
            IOSDllImport.CleverTap_getUserAppLaunchCount(key, UserEventLogCountCallbackFunc);
        }

        internal override void GetUserEventLogCount(string eventName, CleverTapCallback<int> callback)
        {
            string key = Guid.NewGuid().ToString();
            userEventLogCountCallbacks[key] = callback;
            IOSDllImport.CleverTap_getUserEventLogCount(eventName, key, UserEventLogCountCallbackFunc);
        }

        internal override void GetUserEventLogHistory(CleverTapCallback<Dictionary<string, UserEventLog>> callback)
        {
            string key = Guid.NewGuid().ToString();
            userEventLogHistoryCallbacks[key] = callback;
            IOSDllImport.CleverTap_getUserEventLogHistory(key, UserEventLogHistoryCallbackFunc);
        }

        internal override long GetUserLastVisitTs()
        {
            return IOSDllImport.CleverTap_getUserLastVisitTs();
        }
    }
}
#endif