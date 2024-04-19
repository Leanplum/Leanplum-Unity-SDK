#if UNITY_ANDROID
using CleverTapSDK.Common;
using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;
using System.Collections.Generic;

namespace CleverTapSDK.Android {
    internal class AndroidPlatformBinding : CleverTapPlatformBindings {
        internal AndroidPlatformBinding() {
            CallbackHandler = CreateGameObjectAndAttachCallbackHandler<AndroidCallbackHandler>(CleverTapGameObjectName.ANDROID_CALLBACK_HANDLER);
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

        internal override int EventGetFirstTime(string eventName) {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("eventGetFirstTime", eventName);
        }

        internal override int EventGetLastTime(string eventName) {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("eventGetLastTime", eventName);
        }

        internal override int EventGetOccurrences(string eventName) {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("eventGetOccurrences", eventName);
        }

        /**
        * requests for a unique, asynchronous CleverTap identifier. The value will be available as json {"cleverTapID" : <value> } via
        * CleverTapUnity#CleverTapInitCleverTapIdCallback() function
        */
        internal override string GetCleverTapID() {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("getCleverTapID");
            return string.Empty;
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

        internal override void LaunchWithCredentials(string accountID, string token) {
            CleverTapAndroidJNI.CleverTapJNIStatic.CallStatic("initialize", accountID, token, CleverTapAndroidJNI.UnityActivity);
        }

        internal override void LaunchWithCredentialsForRegion(string accountID, string token, string region) {
            CleverTapAndroidJNI.CleverTapJNIStatic.CallStatic("initialize", accountID, token, region, CleverTapAndroidJNI.UnityActivity);
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

        internal override int UserGetPreviousVisitTime() {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("userGetPreviousVisitTime");
        }

        internal override int UserGetScreenCount() {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("userGetScreenCount");
        }

        internal override int UserGetTotalVisits() {
            return CleverTapAndroidJNI.CleverTapJNIInstance.Call<int>("userGetTotalVisits");
        }
    }
}
#endif