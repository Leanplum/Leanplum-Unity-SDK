#if UNITY_IOS
using CleverTapSDK.Common;
using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;
using System.Collections.Generic;

namespace CleverTapSDK.IOS {
    internal class IOSPlatformBinding : CleverTapPlatformBindings {
        internal IOSPlatformBinding() {
            CallbackHandler = CreateGameObjectAndAttachCallbackHandler<IOSCallbackHandler>(CleverTapGameObjectName.IOS_CALLBACK_HANDLER);
            CleverTapLogger.Log("Start: CleverTap binding for iOS.");
        }

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

        internal override int EventGetFirstTime(string eventName) {
            return IOSDllImport.CleverTap_eventGetFirstTime(eventName);
        }

        internal override int EventGetLastTime(string eventName) {
            return IOSDllImport.CleverTap_eventGetLastTime(eventName);
        }

        internal override int EventGetOccurrences(string eventName) {
            return IOSDllImport.CleverTap_eventGetOccurrences(eventName);
        }

        internal override void FetchAndActivateProductConfig() {
            IOSDllImport.CleverTap_fetchAndActivateProductConfig();
        }

        internal override void FetchProductConfig() {
            IOSDllImport.CleverTap_fetchProductConfig();
        }

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

        internal override JSONArray GetAllInboxMessages() {
            string jsonString = IOSDllImport.CleverTap_getAllInboxMessages();
            JSONArray json;
            try {
                json = (JSONArray)JSON.Parse(jsonString);
            } catch {
                CleverTapLogger.LogError("Unable to parse app inbox messages json");
                json = new JSONArray();
            }
            return json;
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

        internal override bool GetFeatureFlag(string key, bool defaultValue) {
            return IOSDllImport.CleverTap_getFeatureFlag(key, defaultValue);
        }

        internal override int GetInboxMessageCount() {
            return IOSDllImport.CleverTap_getInboxMessageCount();
        }

        internal override JSONClass GetInboxMessageForId(string messageId) {
            string jsonString = IOSDllImport.CleverTap_getInboxMessageForId(messageId);
            JSONClass json;
            try {
                json = (JSONClass)JSON.Parse(jsonString);
            } catch {
                CleverTapLogger.LogError("Unable to parse app inbox message json");
                json = new JSONClass();
            }
            return json;
        }

        internal override int GetInboxMessageUnreadCount() {
            return IOSDllImport.CleverTap_getInboxMessageUnreadCount();
        }

        internal override double GetProductConfigLastFetchTimeStamp() {
            return IOSDllImport.CleverTap_getProductConfigLastFetchTimeStamp();
        }

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

        internal override JSONArray GetUnreadInboxMessages() {
            string jsonString = IOSDllImport.CleverTap_getUnreadInboxMessages();
            JSONArray json;
            try {
                json = (JSONArray)JSON.Parse(jsonString);
            } catch {
                CleverTapLogger.LogError("Unable to parse unread app inbox messages json");
                json = new JSONArray();
            }
            return json;
        }

        internal override void InitializeInbox() {
            IOSDllImport.CleverTap_initializeInbox();
        }

        internal override bool IsPushPermissionGranted() {
            IOSDllImport.CleverTap_isPushPermissionGranted();
            // Added for iOS
            return true;
        }

        internal override void LaunchWithCredentials(string accountID, string token) {
            IOSDllImport.CleverTap_launchWithCredentials(accountID, token);
        }

        internal override void LaunchWithCredentialsForRegion(string accountID, string token, string region) {
            IOSDllImport.CleverTap_launchWithCredentialsForRegion(accountID, token, region);
        }

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

        internal override void SetProductConfigDefaults(Dictionary<string, object> defaults) {
            var defaultsString = Json.Serialize(defaults);
            IOSDllImport.CleverTap_setProductConfigDefaults(defaultsString);
        }

        internal override void SetProductConfigDefaultsFromPlistFileName(string fileName) {
            IOSDllImport.CleverTap_setProductConfigDefaultsFromPlistFileName(fileName);
        }

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

        internal override int UserGetPreviousVisitTime() {
            return IOSDllImport.CleverTap_userGetPreviousVisitTime();
        }

        internal override int UserGetScreenCount() {
            return IOSDllImport.CleverTap_userGetScreenCount();
        }

        internal override int UserGetTotalVisits() {
            return IOSDllImport.CleverTap_userGetTotalVisits();
        }
    }
}
#endif