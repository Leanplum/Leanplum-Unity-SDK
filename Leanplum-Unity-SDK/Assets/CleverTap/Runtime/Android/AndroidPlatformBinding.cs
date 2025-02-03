#if UNITY_ANDROID
using CleverTapSDK.Common;
using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;
using System;
using System.Collections.Generic;

namespace CleverTapSDK.Android {
    internal class AndroidPlatformBinding : CleverTapPlatformBindings {
        internal AndroidPlatformBinding()
        {
            CallbackHandler = CreateGameObjectAndAttachCallbackHandler<AndroidCallbackHandler>(CleverTapGameObjectName.ANDROID_CALLBACK_HANDLER);
            CleverTapAndroidJNI.OnInitCleverTapInstanceDelegate = cleverTapJniInstance =>
            {
                cleverTapJniInstance.Call("setInAppNotificationOnShowCallback",
                 new AndroidPluginCallback(message => { CallbackHandler.CleverTapInAppNotificationShowCallback(message); }
                 ));
                cleverTapJniInstance.Call("setInAppNotificationOnButtonTappedCallback",
                 new AndroidPluginCallback(message => { CallbackHandler.CleverTapInAppNotificationButtonTapped(message); }));
            };
            CleverTapLogger.Log("Start: CleverTap binding for Android.");
        }

        internal override void CreateNotificationChannel(string channelId, string channelName, string channelDescription, int importance, bool showBadge) {
            CleverTapAndroidJNI.CleverTapJNIStatic.CallStatic("createNotificationChannel", CleverTapAndroidJNI.ApplicationContext, channelId, channelName, channelDescription, importance, showBadge);
        }

        internal override void CreateNotificationChannelGroup(string groupId, string groupName) {
            CleverTapAndroidJNI.CleverTapJNIStatic.CallStatic("createNotificationChannelGroup", CleverTapAndroidJNI.ApplicationContext, groupId, groupName);
        }

        internal override void CreateNotificationChannelWithGroup(string channelId, string channelName, string channelDescription, int importance, string groupId, bool showBadge) {
            CleverTapAndroidJNI.CleverTapJNIStatic.CallStatic("createNotificationChannelWithGroup", CleverTapAndroidJNI.ApplicationContext, channelId, channelName, channelDescription, importance, groupId, showBadge);
        }

        internal override void CreateNotificationChannelWithGroupAndSound(string channelId, string channelName, string channelDescription, int importance, string groupId, bool showBadge, string sound) {
            CleverTapAndroidJNI.CleverTapJNIStatic.CallStatic("createNotificationChannelWithGroupAndSound", CleverTapAndroidJNI.ApplicationContext, channelId, channelName, channelDescription, importance, groupId, showBadge, sound);
        }

        internal override void CreateNotificationChannelWithSound(string channelId, string channelName, string channelDescription, int importance, bool showBadge, string sound) {
            CleverTapAndroidJNI.CleverTapJNIStatic.CallStatic("createNotificationChannelWithSound", CleverTapAndroidJNI.ApplicationContext, channelId, channelName, channelDescription, importance, showBadge, sound);
        }

        internal override void DeleteInboxMessageForID(string messageId) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("deleteInboxMessageForId", messageId);
        }

        internal override void DeleteInboxMessagesForIDs(string[] messageIds) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("deleteInboxMessagesForIDs", messageIds);
        }

        internal override void DeleteNotificationChannel(string channelId) {
            CleverTapAndroidJNI.CleverTapJNIStatic.CallStatic("deleteNotificationChannel", CleverTapAndroidJNI.ApplicationContext, channelId);
        }

        internal override void DeleteNotificationChannelGroup(string groupId) {
            CleverTapAndroidJNI.CleverTapJNIStatic.CallStatic("deleteNotificationChannelGroup", CleverTapAndroidJNI.ApplicationContext, groupId);
        }

        internal override void DisablePersonalization() {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("disablePersonalization");
        }

        /**
        * Suspends the display of InApp Notifications and discards any new InApp Notifications to be shown
        * after this method is called.
        * The InApp Notifications will be displayed only once resumeInAppNotifications() is called.
        */
        internal override void DiscardInAppNotifications() {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("discardInAppNotifications");
        }

        internal override void DismissAppInbox() {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("dismissAppInbox");
        }

        internal override void EnableDeviceNetworkInfoReporting(bool value) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("enableDeviceNetworkInfoReporting", value);
        }

        internal override void EnablePersonalization() {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("enablePersonalization");
        }

        [Obsolete]
        internal override JSONClass EventGetDetail(string eventName) {
            string jsonString = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("eventGetDetail", eventName);
            JSONClass json;
            try {
                json = (JSONClass)JSON.Parse(jsonString);
            } catch {
                CleverTapLogger.LogError("Unable to event detail json");
                json = new JSONClass();
            }
            return json;
        }

        [Obsolete]
        internal override int EventGetFirstTime(string eventName) {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("eventGetFirstTime", eventName);
        }

        [Obsolete]
        internal override int EventGetLastTime(string eventName) {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("eventGetLastTime", eventName);
        }

        [Obsolete]
        internal override int EventGetOccurrences(string eventName) {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("eventGetOccurrences", eventName);
        }

        /**
        * requests for a unique, asynchronous CleverTap identifier. The value will be available as json {"CleverTapID" : <value> } via
        * CleverTapUnity#CleverTapInitCleverTapIdCallback() function
        */
        internal override string GetCleverTapID() {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("getCleverTapID");
            return string.Empty;
        }

        internal override JSONArray GetAllInboxMessages()
        {
            string jsonString = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("getAllInboxMessages");
            JSONArray json;
            try
            {
                json = (JSONArray)JSON.Parse(jsonString);
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Unable to parse inbox messages JSON: {ex}.");
                json = new JSONArray();
            }

            return json;
        }

        internal override List<CleverTapInboxMessage> GetAllInboxMessagesParsed()
        {
            string jsonString = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("getAllInboxMessages");
            try
            {
                return CleverTapInboxMessageJSONParser.ParseJsonArray(jsonString);
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Unable to parse inbox messages to CleverTapInboxMessage list: {ex}.");
                return new List<CleverTapInboxMessage>();
            }
        }

        internal override JSONClass GetInboxMessageForId(string messageId)
        {
            string jsonString = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("getInboxMessageForId", messageId);
            JSONClass json;
            try
            {
                json = (JSONClass)JSON.Parse(jsonString);
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Unable to parse inbox message for id: {messageId}. Exception: {ex}.");
                json = new JSONClass();
            }

            return json;
        }

        internal override CleverTapInboxMessage GetInboxMessageForIdParsed(string messageId)
        {
            string jsonString = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("getInboxMessageForId", messageId);
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

        internal override JSONArray GetUnreadInboxMessages()
        {
            string jsonString = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("getUnreadInboxMessages");
            JSONArray json;
            try
            {
                json = (JSONArray)JSON.Parse(jsonString);
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Unable to parse unread inbox messages JSON: {ex}.");
                json = new JSONArray();
            }

            return json;
        }

        internal override List<CleverTapInboxMessage> GetUnreadInboxMessagesParsed()
        {
            string jsonString = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("getUnreadInboxMessages");
            try
            {
                return CleverTapInboxMessageJSONParser.ParseJsonArray(jsonString);
            }
            catch (Exception ex)
            {
                CleverTapLogger.LogError($"Unable to parse unread inbox messages to CleverTapInboxMessage list: {ex}.");
                return new List<CleverTapInboxMessage>();
            }
        }

        internal override int GetInboxMessageCount() {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("getInboxMessageCount");
        }

        internal override int GetInboxMessageUnreadCount() {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("getInboxMessageUnreadCount");
        }

        internal override void InitializeInbox() {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("initializeInbox");
        }

        internal override bool IsPushPermissionGranted() {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<bool>("isPushPermissionGranted");
        }

        internal override void MarkReadInboxMessageForID(string messageId) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("markReadInboxMessageForId", messageId);
        }

        internal override void MarkReadInboxMessagesForIDs(string[] messageIds) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("markReadInboxMessagesForIDs", messageIds);
        }

        internal override void OnUserLogin(Dictionary<string, object> properties) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("onUserLogin", Json.Serialize(properties.ConvertDateObjects()));
        }

        internal override void ProfileAddMultiValueForKey(string key, string val) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("profileAddMultiValueForKey", key, val);
        }

        internal override void ProfileAddMultiValuesForKey(string key, List<string> values) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("profileAddMultiValuesForKey", key, values.ToArray());
        }

        /**
        * This method is used to decrement the given value.Number should be in positive range
        */
        internal override void ProfileDecrementValueForKey(string key, double val) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("profileDecrementValueForKey", key, val);
        }

        /**
        * This method is used to decrement the given value.Number should be in positive range
        */
        internal override void ProfileDecrementValueForKey(string key, int val) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("profileDecrementValueForKey", key, val);
        }

        internal override string ProfileGet(string key) {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("profileGet", key);
        }

        /**
        * Returns a unique CleverTap identifier suitable for use with install attribution providers.
        * @return The attribution identifier currently being used to identify this user.
        *
        * Disclaimer: this method may take a long time to return, so you should not call it from the
        * application main thread
        *
        * NOTE: Deprecated as of clevertap android core sdk version 4.2.0 and will be removed
        *  in future versions .
        * instead listen for the id via CleverTapUnity#CleverTapInitCleverTapIdCallback() function
        */
        internal override string ProfileGetCleverTapAttributionIdentifier() {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("profileGetCleverTapAttributionIdentifier");
        }

        /**
        * Returns a unique CleverTap identifier suitable for use with install attribution providers.
        * @return The attribution identifier currently being used to identify this user.
        *
        * Disclaimer: this method may take a long time to return, so you should not call it from the
        * application main thread
        *
        * NOTE: Deprecated as of clevertap android core sdk version 4.2.0 and will be removed
        *  in future versions .
        * instead request for clevertapId via getCleverTapId() call and  listen for response
        * via CleverTapUnity#CleverTapInitCleverTapIdCallback() function
        */
        internal override string ProfileGetCleverTapID() {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("profileGetCleverTapID");
        }

        /**
        * This method is used to increment the given value.Number should be in positive range
        */
        internal override void ProfileIncrementValueForKey(string key, double val) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("profileIncrementValueForKey", key, val);
        }

        /**
        * This method is used to increment the given value.Number should be in positive range
        */
        internal override void ProfileIncrementValueForKey(string key, int val) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("profileIncrementValueForKey", key, val);
        }

        internal override void ProfilePush(Dictionary<string, object> properties) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("profilePush", Json.Serialize(properties.ConvertDateObjects()));
        }

        internal override void ProfileRemoveMultiValueForKey(string key, string val) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("profileRemoveMultiValueForKey", key, val);
        }

        internal override void ProfileRemoveMultiValuesForKey(string key, List<string> values) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("profileRemoveMultiValuesForKey", key, values.ToArray());
        }

        internal override void ProfileRemoveValueForKey(string key) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("profileRemoveValueForKey", key);
        }

        internal override void ProfileSetMultiValuesForKey(string key, List<string> values) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("profileSetMultiValuesForKey", key, values.ToArray());
        }

        internal override void PromptForPushPermission(bool showFallbackSettings) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("promptForPushPermission", showFallbackSettings);
        }

        internal override void PromptPushPrimer(Dictionary<string, object> details) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("promptPushPrimer", Json.Serialize(details));
        }

        internal override void PushInstallReferrerSource(string source, string medium, string campaign) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("pushInstallReferrer", source, medium, campaign);
        }

        internal override void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>> items) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("recordChargedEventWithDetailsAndItems", Json.Serialize(details.ConvertDateObjects()), Json.Serialize(items));
        }

        internal override void RecordEvent(string eventName) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("recordEvent", eventName, null);
        }

        internal override void RecordEvent(string eventName, Dictionary<string, object> properties) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("recordEvent", eventName, Json.Serialize(properties.ConvertDateObjects()));
        }

        internal override void RecordScreenView(string screenName) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("recordScreenView", screenName);
        }

        /**
        * Suspends the display of InApp Notifications and discards any new InApp Notifications to be shown
        * after this method is called.
        * The InApp Notifications will be displayed only once resumeInAppNotifications() is called.
        */
        internal override void ResumeInAppNotifications() {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("resumeInAppNotifications");
        }

        internal override int SessionGetTimeElapsed() {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("sessionGetTimeElapsed");
        }

        internal override JSONClass SessionGetUTMDetails() {
            string jsonString = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("sessionGetUTMDetails");
            JSONClass json;
            try {
                json = (JSONClass)JSON.Parse(jsonString);
            } catch {
                CleverTapLogger.LogError("Unable to parse session utm details json");
                json = new JSONClass();
            }
            return json;
        }

        internal override void SetDebugLevel(int level) {
            CleverTapAndroidJNI.CleverTapJNIStatic.CallStatic("setDebugLevel", level);
        }

        internal override void SetLocation(double lat, double lon) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("setLocation", lat, lon);
        }

        internal override void SetOffline(bool value) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("setOffline", value);
        }

        internal override void SetOptOut(bool value) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("setOptOut", value);
        }

        internal override void ShowAppInbox(string styleConfig) {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("showAppInbox", styleConfig);
        }

        /**
        * Suspends display of InApp Notifications.
        * The InApp Notifications are queued once this method is called
        * and will be displayed once resumeInAppNotifications() is called.
        */
        internal override void SuspendInAppNotifications() {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("suspendInAppNotifications");
        }

        [Obsolete]
        internal override JSONClass UserGetEventHistory() {
            string jsonString = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("userGetEventHistory");
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
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("userGetPreviousVisitTime");
        }

        internal override int UserGetScreenCount() {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("userGetScreenCount");
        }

        [Obsolete]
        internal override int UserGetTotalVisits() {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("userGetTotalVisits");
        }

        internal override void GetUserEventLog(string eventName, CleverTapCallback<UserEventLog> callback)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("getUserEventLog", eventName,
             new AndroidPluginCallback(message => { callback?.Invoke(UserEventLog.Parse(message)); }));
        }

        internal override void GetUserEventLogCount(string eventName, CleverTapCallback<int> callback)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("getUserEventLogCount", eventName,
             new AndroidPluginIntCallback(count => { callback?.Invoke(count); }));
        }

        internal override void GetUserAppLaunchCount(CleverTapCallback<int> callback)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("getUserAppLaunchCount",
             new AndroidPluginIntCallback(count => { callback?.Invoke(count); }));
        }

        internal override void GetUserEventLogHistory(CleverTapCallback<Dictionary<string, UserEventLog>> callback)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("getUserEventLogHistory",
             new AndroidPluginCallback(message => { callback?.Invoke(UserEventLog.ParseLogsDictionary(message)); }));
        }

        internal override long GetUserLastVisitTs()
        {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<long>("getUserLastVisitTs");
        }

        #region Feature Flags

        [Obsolete("Feature Flags are deprecated, use variables instead.")]
        internal override bool GetFeatureFlag(string key, bool defaultValue)
        {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<bool>("getFeatureFlag", key, defaultValue);
        }
        #endregion
        #region Product Config

        [Obsolete]
        internal override void SetProductConfigDefaults(Dictionary<string, object> defaults)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("setProductConfigMapDefaults", Json.Serialize(defaults.ConvertDateObjects()));
        }

        [Obsolete]
        internal override void SetProductConfigMinimumFetchInterval(double minimumFetchInterval)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("setProductConfigMinimumFetchInterval", (long) minimumFetchInterval);
        }

        [Obsolete]
        internal override void ResetProductConfig()
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("productConfigReset");
        }

        [Obsolete]
        internal override void ActivateProductConfig()
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("activateProductConfig");
        }

        [Obsolete]
        internal override void FetchAndActivateProductConfig()
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("fetchAndActivateProductConfig");
        }

               [Obsolete]
        internal override void FetchProductConfig()
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("fetchProductConfig");
        }

        [Obsolete]
        internal override void FetchProductConfigWithMinimumInterval(double minimumInterval)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("fetchProductConfigWithMinimalInterval", (long) minimumInterval);
        }

        [Obsolete]
        internal override double GetProductConfigLastFetchTimeStamp()
        {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<long>("getProductConfigLastFetchTimeStampInMillis");    
        }

        [Obsolete]
        internal override string GetProductConfigString(string key)
        {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("getProductConfigString", key);
        }

        [Obsolete]
        internal override bool? GetProductConfigBoolean(string key)
        {
            var stringValue = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("getProductConfigBoolean", key);
            return bool.TryParse(stringValue, out var result) ? result : null;
        }

        [Obsolete]
        internal override long? GetProductConfigLong(string key)
        {
            var stringValue = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("getProductConfigLong", key);
            return long.TryParse(stringValue, out var result) ? result : null;
        }

        [Obsolete]
        internal override double? GetProductConfigDouble(string key)
        {
            var stringValue = CleverTapAndroidJNI.CleverTapJNIInstance.Call<string>("getProductConfigDouble", key);
            return double.TryParse(stringValue, out var result) ? result : null;
        }
        #endregion
    }
}
#endif