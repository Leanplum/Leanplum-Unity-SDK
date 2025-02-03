using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CleverTapSDK.Common {
    internal abstract class CleverTapPlatformBindings {
        internal const string VERSION = CleverTapVersion.CLEVERTAP_SDK_VERSION;

        public CleverTapCallbackHandler CallbackHandler { get; protected set; }

        protected T CreateGameObjectAndAttachCallbackHandler<T>(string objectName) where T : CleverTapCallbackHandler {
            var gameObject = new GameObject(objectName);
            gameObject.AddComponent<T>();
            GameObject.DontDestroyOnLoad(gameObject);
            return gameObject.GetComponent<T>();
        }

        #region Default - Platform Bindings

        [Obsolete]
        internal virtual void ActivateProductConfig() {
        }

        internal virtual void CreateNotificationChannel(string channelId, string channelName, string channelDescription, int importance, bool showBadge) {
        }

        internal virtual void CreateNotificationChannelGroup(string groupId, string groupName) {
        }

        internal virtual void CreateNotificationChannelWithGroup(string channelId, string channelName, string channelDescription, int importance, string groupId, bool showBadge) {
        }

        internal virtual void CreateNotificationChannelWithGroupAndSound(string channelId, string channelName, string channelDescription, int importance, string groupId, bool showBadge, string sound) {
        }

        internal virtual void CreateNotificationChannelWithSound(string channelId, string channelName, string channelDescription, int importance, bool showBadge, string sound) {
        }

        internal virtual void DeleteInboxMessageForID(string messageId) {
        }

        internal virtual void DeleteInboxMessagesForIDs(string[] messageIds) {
        }

        internal virtual void DeleteNotificationChannel(string channelId) {
        }

        internal virtual void DeleteNotificationChannelGroup(string groupId) {
        }

        internal virtual void DisablePersonalization() {
        }

        internal virtual void DiscardInAppNotifications() {
        }

        internal virtual void DismissAppInbox() {
        }

        internal virtual void EnableDeviceNetworkInfoReporting(bool enabled) {
        }

        internal virtual void EnablePersonalization() {
        }

        [Obsolete]
        internal virtual JSONClass EventGetDetail(string eventName) {
            return new JSONClass();
        }

        [Obsolete]
        internal virtual int EventGetFirstTime(string eventName) {
            return -1;
        }

        [Obsolete]
        internal virtual int EventGetLastTime(string eventName) {
            return -1;
        }

        [Obsolete]
        internal virtual int EventGetOccurrences(string eventName) {
            return -1;
        }

        [Obsolete]
        internal virtual void FetchAndActivateProductConfig() {
        }

        [Obsolete]
        internal virtual void FetchProductConfig() {
        }

        [Obsolete]
        internal virtual void FetchProductConfigWithMinimumInterval(double minimumInterval) {
        }

        internal virtual JSONArray GetAllDisplayUnits() {
            return new JSONArray();
        }

        internal virtual JSONArray GetAllInboxMessages() {
            return new JSONArray();
        }

        internal virtual List<CleverTapInboxMessage> GetAllInboxMessagesParsed()
        {
            return new List<CleverTapInboxMessage>();
        }

        internal virtual string GetCleverTapID() {
            return string.Empty;
        }

        internal virtual JSONClass GetDisplayUnitForID(string unitID) {
            return new JSONClass();
        }

        [Obsolete("Feature Flags are deprecated, use variables instead.")]
        internal virtual bool GetFeatureFlag(string key, bool defaultValue) {
            // Validate if this is ok?
            return defaultValue;
        }

        internal virtual int GetInboxMessageCount() {
            return -1;
        }

        internal virtual JSONClass GetInboxMessageForId(string messageId) {
            return new JSONClass();
        }

        internal virtual CleverTapInboxMessage GetInboxMessageForIdParsed(string messageId)
        {
            return null;
        }

        internal virtual int GetInboxMessageUnreadCount() {
            return -1;
        }

        [Obsolete]
        internal virtual double GetProductConfigLastFetchTimeStamp() {
            return -1;
        }

        [Obsolete]
        internal virtual JSONClass GetProductConfigValueFor(string key) {
            return new JSONClass();
        }

        [Obsolete]
        internal virtual string GetProductConfigString(string key)
        {
            return null;
        }

        [Obsolete]
        internal virtual bool? GetProductConfigBoolean(string key)
        {
            return null;
        }

        [Obsolete]
        internal virtual long? GetProductConfigLong(string key)
        {
            return null;
        }

        [Obsolete]
        internal virtual double? GetProductConfigDouble(string key)
        {
            return null;
        }

        internal virtual JSONArray GetUnreadInboxMessages() {
            return new JSONArray();
        }

        internal virtual List<CleverTapInboxMessage> GetUnreadInboxMessagesParsed()
        {
            return new List<CleverTapInboxMessage>();
        }

        internal virtual void InitializeInbox() {
        }

        internal virtual bool IsPushPermissionGranted() {
            return false;
        }

        internal virtual void LaunchWithCredentials(string accountID, string token) {
        }

        internal virtual void LaunchWithCredentialsForRegion(string accountID, string token, string region) {
        }
        
        internal virtual void LaunchWithCredentialsForProxyServer(string accountID, string token, string proxyDomain, string spikyProxyDomain) {
        }

        internal virtual void MarkReadInboxMessageForID(string messageId) {
        }

        internal virtual void MarkReadInboxMessagesForIDs(string[] messageIds) {
        }

        internal virtual void OnUserLogin(Dictionary<string, string> properties) {
            Dictionary<string, object> propsObjectValue = properties.ToDictionary(kv => kv.Key, kv => (object)kv.Value);
            OnUserLogin(propsObjectValue);
        }

        internal virtual void OnUserLogin(Dictionary<string, object> properties) {
        }

        internal virtual void ProfileAddMultiValueForKey(string key, string val) {
        }

        internal virtual void ProfileAddMultiValuesForKey(string key, List<string> values) {
        }

        internal virtual void ProfileDecrementValueForKey(string key, double val) {
        }

        internal virtual void ProfileDecrementValueForKey(string key, int val) {
        }

        internal virtual string ProfileGet(string key) {
            return string.Empty;
        }

        internal virtual string ProfileGetCleverTapAttributionIdentifier() {
            return string.Empty;
        }

        internal virtual string ProfileGetCleverTapID() {
            return string.Empty;
        }

        internal virtual void ProfileIncrementValueForKey(string key, double val) {
        }

        internal virtual void ProfileIncrementValueForKey(string key, int val) {
        }

        internal virtual void ProfilePush(Dictionary<string, string> properties) {
            Dictionary<string, object> propsObjectValue = properties.ToDictionary(kv => kv.Key, kv => (object)kv.Value);
            ProfilePush(propsObjectValue);
        }

        internal virtual void ProfilePush(Dictionary<string, object> properties) {
        }

        internal virtual void ProfileRemoveMultiValueForKey(string key, string val) {
        }

        internal virtual void ProfileRemoveMultiValuesForKey(string key, List<string> values) {
        }

        internal virtual void ProfileRemoveValueForKey(string key) {
        }

        internal virtual void ProfileSetMultiValuesForKey(string key, List<string> values) {
        }

        internal virtual void PromptForPushPermission(bool showFallbackSettings) {
        }

        internal virtual void PromptPushPrimer(Dictionary<string, object> json) {
        }

        internal virtual void PushInstallReferrerSource(string source, string medium, string campaign) {
        }

        internal virtual void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>> items) {
        }

        internal virtual void RecordDisplayUnitClickedEventForID(string unitID) {
        }

        internal virtual void RecordDisplayUnitViewedEventForID(string unitID) {
        }

        internal virtual void RecordEvent(string eventName) {
        }

        internal virtual void RecordEvent(string eventName, Dictionary<string, object> properties) {
        }

        internal virtual void RecordInboxNotificationClickedEventForID(string messageId) {
        }

        internal virtual void RecordInboxNotificationViewedEventForID(string messageId) {
        }

        internal virtual void RecordScreenView(string screenName) {
        }

        internal virtual void RegisterPush() {
        }

        [Obsolete]
        internal virtual void ResetProductConfig() {
        }

        internal virtual void ResumeInAppNotifications() {
        }

        internal virtual int SessionGetTimeElapsed() {
            return -1;
        }

        internal virtual JSONClass SessionGetUTMDetails() {
            return new JSONClass();
        }

        internal virtual void SetApplicationIconBadgeNumber(int num) {
        }

        internal virtual void SetDebugLevel(int level) {
        }

        internal virtual void SetLocation(double lat, double lon) {
        }

        internal virtual void SetOffline(bool enabled) {
        }

        internal virtual void SetOptOut(bool enabled) {
        }

        [Obsolete]
        internal virtual void SetProductConfigDefaults(Dictionary<string, object> defaults) {
        }

        [Obsolete]
        internal virtual void SetProductConfigDefaultsFromPlistFileName(string fileName) {
        }

        [Obsolete]
        internal virtual void SetProductConfigMinimumFetchInterval(double minimumFetchInterval) {
        }

        internal virtual void ShowAppInbox(Dictionary<string, object> styleConfig) {
        }

        internal virtual void ShowAppInbox(string styleConfig) {
        }

        internal virtual void SuspendInAppNotifications() {
        }

        [Obsolete]
        internal virtual JSONClass UserGetEventHistory() {
            return new JSONClass();
        }

        [Obsolete]
        internal virtual int UserGetPreviousVisitTime() {
            return -1;
        }

        internal virtual int UserGetScreenCount() {
            return -1;
        }

        [Obsolete]
        internal virtual int UserGetTotalVisits() {
            return -1;
        }

        internal virtual void GetUserEventLog(string eventName, CleverTapCallback<UserEventLog> callback) { }

        internal virtual void GetUserEventLogCount(string eventName, CleverTapCallback<int> callback) { }

        internal virtual void GetUserAppLaunchCount(CleverTapCallback<int> callback) { }

        internal virtual void GetUserEventLogHistory(CleverTapCallback<Dictionary<string, UserEventLog>> callback) { }

        internal virtual long GetUserLastVisitTs() {
            return -1;
        }

        #endregion
    }
}
