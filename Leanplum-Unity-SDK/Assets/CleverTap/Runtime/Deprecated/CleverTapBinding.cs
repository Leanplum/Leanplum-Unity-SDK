using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using CleverTapStatic = CleverTapSDK.CleverTap;
#if UNITY_ANDROID
using CleverTapSDK.Android;
#endif

/// <summary>
/// These methods can be called by Unity applications to record
/// events and set and get user profile attributes.
/// </summary>

namespace CleverTapSDK 
{
    [Obsolete]
    public class CleverTapBinding : MonoBehaviour 
    {
        [Obsolete("Please use CleverTap.VERSION instead.")]
        public const string Version = CleverTapVersion.CLEVERTAP_SDK_VERSION;

#if UNITY_IOS
        
        [Obsolete("Please use CleverTap.LaunchWithCredentials(string, string) instead.")]
        public static void LaunchWithCredentials(string accountID, string token) =>
            CleverTapStatic.LaunchWithCredentials(accountID, token);

        [Obsolete("Please use CleverTap.LaunchWithCredentialsForRegion(string, string, string) instead.")]
        public static void LaunchWithCredentialsForRegion(string accountID, string token, string region) =>
            CleverTapStatic.LaunchWithCredentialsForRegion(accountID, token, region);

        [Obsolete("Please use CleverTap.OnUserLogin(Dictionary<string, string>) instead.")]
        public static void OnUserLogin(Dictionary<string, string> properties) =>
            CleverTapStatic.OnUserLogin(properties);

        [Obsolete("Please use CleverTap.ProfilePush(Dictionary<string, string>) instead.")]
        public static void ProfilePush(Dictionary<string, string> properties) =>
            CleverTapStatic.ProfilePush(properties);

        [Obsolete("Please use CleverTap.OnUserLogin(Dictionary<string, object>) instead.")]
        public static void OnUserLogin(Dictionary<string, object> properties) =>
            CleverTapStatic.OnUserLogin(properties);

        [Obsolete("Please use CleverTap.ProfilePush(Dictionary<string, object>) instead.")]
        public static void ProfilePush(Dictionary<string, object> properties) =>
            CleverTapStatic.ProfilePush(properties);

        [Obsolete("Please use CleverTap.ProfileGet(string) instead.")]
        public static string ProfileGet(string key) =>
            CleverTapStatic.ProfileGet(key);

        [Obsolete("Please use CleverTap.ProfileGetCleverTapAttributionIdentifier() instead.")]
        public static string ProfileGetCleverTapAttributionIdentifier() =>
            CleverTapStatic.ProfileGetCleverTapAttributionIdentifier();

        [Obsolete("Please use CleverTap.ProfileGetCleverTapID() instead.")]
        public static string ProfileGetCleverTapID() =>
            CleverTapStatic.ProfileGetCleverTapID();

        [Obsolete("Please use CleverTap.ProfileRemoveValueForKey(string) instead.")]
        public static void ProfileRemoveValueForKey(string key) =>
            CleverTapStatic.ProfileRemoveValueForKey(key);

        [Obsolete("Please use CleverTap.ProfileSetMultiValuesForKey(string, List<string>) instead.")]
        public static void ProfileSetMultiValuesForKey(string key, List<string> values) =>
            CleverTapStatic.ProfileSetMultiValuesForKey(key, values);

        [Obsolete("Please use CleverTap.ProfileAddMultiValuesForKey(string, List<string>) instead.")]
        public static void ProfileAddMultiValuesForKey(string key, List<string> values) =>
            CleverTapStatic.ProfileAddMultiValuesForKey(key, values);

        [Obsolete("Please use CleverTap.ProfileRemoveMultiValuesForKey(string, List<string>) instead.")]
        public static void ProfileRemoveMultiValuesForKey(string key, List<string> values) =>
            CleverTapStatic.ProfileRemoveMultiValuesForKey(key, values);

        [Obsolete("Please use CleverTap.ProfileAddMultiValueForKey(string, string) instead.")]
        public static void ProfileAddMultiValueForKey(string key, string val) =>
            CleverTapStatic.ProfileAddMultiValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileRemoveMultiValueForKey(string, string) instead.")]
        public static void ProfileRemoveMultiValueForKey(string key, string val) =>
            CleverTapStatic.ProfileRemoveMultiValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileIncrementValueForKey(string, double) instead.")]
        public static void ProfileIncrementValueForKey(string key, double val) =>
            CleverTapStatic.ProfileIncrementValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileIncrementValueForKey(string, int) instead.")]
        public static void ProfileIncrementValueForKey(string key, int val) =>
            CleverTapStatic.ProfileIncrementValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileDecrementValueForKey(string, double) instead.")]
        public static void ProfileDecrementValueForKey(string key, double val) =>
            CleverTapStatic.ProfileDecrementValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileDecrementValueForKey(string, int) instead.")]
        public static void ProfileDecrementValueForKey(string key, int val) =>
            CleverTapStatic.ProfileDecrementValueForKey(key, val);

        [Obsolete("Please use CleverTap.SuspendInAppNotifications() instead.")]
        public static void SuspendInAppNotifications() =>
            CleverTapStatic.SuspendInAppNotifications();

        [Obsolete("Please use CleverTap.DiscardInAppNotifications() instead.")]
        public static void DiscardInAppNotifications() =>
            CleverTapStatic.DiscardInAppNotifications();

        [Obsolete("Please use CleverTap.ResumeInAppNotifications() instead.")]
        public static void ResumeInAppNotifications() =>
            CleverTapStatic.ResumeInAppNotifications();

        [Obsolete("Please use CleverTap.GetCleverTapID() instead.")]
        public static string GetCleverTapID() =>
            CleverTapStatic.GetCleverTapID();

        [Obsolete("Please use CleverTap.RecordScreenView(string) instead.")]
        public static void RecordScreenView(string screenName) =>
            CleverTapStatic.RecordScreenView(screenName);

        [Obsolete("Please use CleverTap.RecordEvent(string) instead.")]
        public static void RecordEvent(string eventName) =>
            CleverTapStatic.RecordEvent(eventName);

        [Obsolete("Please use CleverTap.RecordEvent(string, Dictionary<string, object>) instead.")]
        public static void RecordEvent(string eventName, Dictionary<string, object> properties) =>
            CleverTapStatic.RecordEvent(eventName, properties);

        [Obsolete("Please use CleverTap.RecordChargedEventWithDetailsAndItems(Dictionary<string, object>, List<Dictionary<string, object>>) instead.")]
        public static void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>> items) =>
            CleverTapStatic.RecordChargedEventWithDetailsAndItems(details, items);

        [Obsolete("Please use CleverTap.EventGetFirstTime(string) instead.")]
        public static int EventGetFirstTime(string eventName) =>
            CleverTapStatic.EventGetFirstTime(eventName);

        [Obsolete("Please use CleverTap.EventGetLastTime(string) instead.")]
        public static int EventGetLastTime(string eventName) =>
            CleverTapStatic.EventGetLastTime(eventName);

        [Obsolete("Please use CleverTap.EventGetOccurrences(string) instead.")]
        public static int EventGetOccurrences(string eventName) =>
            CleverTapStatic.EventGetOccurrences(eventName);

        [Obsolete("Please use CleverTap.UserGetEventHistory() instead.")]
        public static JSONClass UserGetEventHistory() =>
            CleverTapStatic.UserGetEventHistory();

        [Obsolete("Please use CleverTap.SessionGetUTMDetails() instead.")]
        public static JSONClass SessionGetUTMDetails() =>
            CleverTapStatic.SessionGetUTMDetails();

        [Obsolete("Please use CleverTap.SessionGetTimeElapsed() instead.")]
        public static int SessionGetTimeElapsed() =>
            CleverTapStatic.SessionGetTimeElapsed();

        [Obsolete("Please use CleverTap.EventGetDetail(string eventName) instead.")]
        public static JSONClass EventGetDetail(string eventName) =>
            CleverTapStatic.EventGetDetail(eventName);

        [Obsolete("Please use CleverTap.UserGetTotalVisits() instead.")]
        public static int UserGetTotalVisits() =>
            CleverTapStatic.UserGetTotalVisits();

        [Obsolete("Please use CleverTap.UserGetScreenCount() instead.")]
        public static int UserGetScreenCount() =>
            CleverTapStatic.UserGetScreenCount();

        [Obsolete("Please use CleverTap.UserGetPreviousVisitTime() instead.")]
        public static int UserGetPreviousVisitTime() =>
            CleverTapStatic.UserGetPreviousVisitTime();

        [Obsolete("Please use CleverTap.RegisterPush() instead.")]
        public static void RegisterPush() =>
            CleverTapStatic.RegisterPush();

        [Obsolete("Please use CleverTap.SetApplicationIconBadgeNumber(int) instead.")]
        public static void SetApplicationIconBadgeNumber(int num) =>
            CleverTapStatic.SetApplicationIconBadgeNumber(num);

        [Obsolete("Please use CleverTap.SetDebugLevel(int) instead.")]
        public static void SetDebugLevel(int level) =>
            CleverTapStatic.SetDebugLevel(level);

        [Obsolete("Please use CleverTap.EnablePersonalization() instead.")]
        public static void EnablePersonalization() =>
            CleverTapStatic.EnablePersonalization();

        [Obsolete("Please use CleverTap.DisablePersonalization() instead.")]
        public static void DisablePersonalization() =>
            CleverTapStatic.DisablePersonalization();

        [Obsolete("Please use CleverTap.SetLocation(double, double) instead.")]
        public static void SetLocation(double lat, double lon) =>
            CleverTapStatic.SetLocation(lat, lon);

        [Obsolete("Please use CleverTap.PushInstallReferrerSource(string, string, string) instead.")]
        public static void PushInstallReferrerSource(string source, string medium, string campaign) =>
            CleverTapStatic.PushInstallReferrerSource(source, medium, campaign);

        [Obsolete("Please use CleverTap.SetOffline(bool) instead.")]
        public static void SetOffline(bool enabled) =>
            CleverTapStatic.SetOffline(enabled);

        [Obsolete("Please use CleverTap.SetOptOut(bool) instead.")]
        public static void SetOptOut(bool enabled) =>
            CleverTapStatic.SetOptOut(enabled);

        [Obsolete("Please use CleverTap.EnableDeviceNetworkInfoReporting(bool) instead.")]
        public static void EnableDeviceNetworkInfoReporting(bool enabled) =>
            CleverTapStatic.EnableDeviceNetworkInfoReporting(enabled);

        [Obsolete("Please use CleverTap.InitializeInbox() instead.")]
        public static void InitializeInbox() =>
            CleverTapStatic.InitializeInbox();

        [Obsolete("Please use CleverTap.ShowAppInbox(Dictionary<string, object>) instead.")]
        public static void ShowAppInbox(Dictionary<string, object> styleConfig) =>
            CleverTapStatic.ShowAppInbox(styleConfig);

        [Obsolete("Please use CleverTap.DismissAppInbox() instead.")]
        public static void DismissAppInbox() =>
            CleverTapStatic.DismissAppInbox();

        [Obsolete("Please use CleverTap.GetInboxMessageCount() instead.")]
        public static int GetInboxMessageCount() =>
            CleverTapStatic.GetInboxMessageCount();

        [Obsolete("Please use CleverTap.GetInboxMessageUnreadCount() instead.")]
        public static int GetInboxMessageUnreadCount() =>
            CleverTapStatic.GetInboxMessageUnreadCount();

        [Obsolete("Please use CleverTap.GetAllInboxMessages() instead.")]
        public static JSONArray GetAllInboxMessages() =>
            CleverTapStatic.GetAllInboxMessages();

        [Obsolete("Please use CleverTap.GetUnreadInboxMessages() instead.")]
        public static JSONArray GetUnreadInboxMessages() =>
            CleverTapStatic.GetUnreadInboxMessages();

        [Obsolete("Please use CleverTap.GetInboxMessageForId(string) instead.")]
        public static JSONClass GetInboxMessageForId(string messageId) =>
            CleverTapStatic.GetInboxMessageForId(messageId);

        [Obsolete("Please use CleverTap.DeleteInboxMessageForID(string) instead.")]
        public static void DeleteInboxMessageForID(string messageId) =>
            CleverTapStatic.DeleteInboxMessageForID(messageId);

        [Obsolete("Please use CleverTap.DeleteInboxMessagesForIDs(string[]) instead.")]
        public static void DeleteInboxMessagesForIDs(string[] messageIds) =>
            CleverTapStatic.DeleteInboxMessagesForIDs(messageIds);

        [Obsolete("Please use CleverTap.MarkReadInboxMessageForID(string) instead.")]
        public static void MarkReadInboxMessageForID(string messageId) =>
            CleverTapStatic.MarkReadInboxMessageForID(messageId);

        [Obsolete("Please use CleverTap.MarkReadInboxMessagesForIDs(string[]) instead.")]
        public static void MarkReadInboxMessagesForIDs(string[] messageIds) =>
            CleverTapStatic.MarkReadInboxMessagesForIDs(messageIds);

        [Obsolete("Please use CleverTap.RecordInboxNotificationViewedEventForID(string) instead.")]
        public static void RecordInboxNotificationViewedEventForID(string messageId) =>
            CleverTapStatic.RecordInboxNotificationViewedEventForID(messageId);

        [Obsolete("Please use CleverTap.RecordInboxNotificationClickedEventForID(string) instead.")]
        public static void RecordInboxNotificationClickedEventForID(string messageId) =>
            CleverTapStatic.RecordInboxNotificationClickedEventForID(messageId);

        [Obsolete("Please use CleverTap.GetAllDisplayUnits() instead.")]
        public static JSONArray GetAllDisplayUnits() =>
            CleverTapStatic.GetAllDisplayUnits();

        [Obsolete("Please use CleverTap.GetDisplayUnitForID(string) instead.")]
        public static JSONClass GetDisplayUnitForID(string unitID) =>
            CleverTapStatic.GetDisplayUnitForID(unitID);

        [Obsolete("Please use CleverTap.RecordDisplayUnitViewedEventForID(string) instead.")]
        public static void RecordDisplayUnitViewedEventForID(string unitID) =>
            CleverTapStatic.RecordDisplayUnitViewedEventForID(unitID);

        [Obsolete("Please use CleverTap.RecordDisplayUnitClickedEventForID(string) instead.")]
        public static void RecordDisplayUnitClickedEventForID(string unitID) =>
            CleverTapStatic.RecordDisplayUnitClickedEventForID(unitID);

        [Obsolete("Please use CleverTap.FetchProductConfig() instead.")]
        public static void FetchProductConfig() =>
            CleverTapStatic.FetchProductConfig();

        [Obsolete("Please use CleverTap.FetchProductConfigWithMinimumInterval(double) instead.")]
        public static void FetchProductConfigWithMinimumInterval(double minimumInterval) =>
            CleverTapStatic.FetchProductConfigWithMinimumInterval(minimumInterval);

        [Obsolete("Please use CleverTap.SetProductConfigMinimumFetchInterval(double) instead.")]
        public static void SetProductConfigMinimumFetchInterval(double minimumFetchInterval) =>
            CleverTapStatic.SetProductConfigMinimumFetchInterval(minimumFetchInterval);

        [Obsolete("Please use CleverTap.ActivateProductConfig() instead.")]
        public static void ActivateProductConfig() =>
            CleverTapStatic.ActivateProductConfig();

        [Obsolete("Please use CleverTap.FetchAndActivateProductConfig() instead.")]
        public static void FetchAndActivateProductConfig() =>
            CleverTapStatic.FetchAndActivateProductConfig();

        [Obsolete("Please use CleverTap.SetProductConfigDefaults(Dictionary<string, object>) instead.")]
        public static void SetProductConfigDefaults(Dictionary<string, object> defaults) =>
            CleverTapStatic.SetProductConfigDefaults(defaults);

        [Obsolete("Please use CleverTap.SetProductConfigDefaultsFromPlistFileName(string) instead.")]
        public static void SetProductConfigDefaultsFromPlistFileName(string fileName) =>
            CleverTapStatic.SetProductConfigDefaultsFromPlistFileName(fileName);

        [Obsolete("Please use CleverTap.GetProductConfigValueFor(string) instead.")]
        public static JSONClass GetProductConfigValueFor(string key) =>
            CleverTapStatic.GetProductConfigValueFor(key);

        [Obsolete("Please use CleverTap.GetProductConfigLastFetchTimeStamp() instead.")]
        public static double GetProductConfigLastFetchTimeStamp() =>
            CleverTapStatic.GetProductConfigLastFetchTimeStamp();

        [Obsolete("Please use CleverTap.ResetProductConfig() instead.")]
        public static void ResetProductConfig() =>
            CleverTapStatic.ResetProductConfig();

        [Obsolete("Please use CleverTap.GetFeatureFlag(string, bool) instead.")]
        public static bool GetFeatureFlag(string key, bool defaultValue) =>
            CleverTapStatic.GetFeatureFlag(key, defaultValue);

        [Obsolete("Please use CleverTap.PromptPushPrimer(Dictionary<string, object>) instead.")]
        public static void PromptPushPrimer(Dictionary<string, object> json) =>
            CleverTapStatic.PromptPushPrimer(json);

        [Obsolete("Please use CleverTap.PromptForPushPermission(bool) instead.")]
        public static void PromptForPushPermission(bool showFallbackSettings) =>
            CleverTapStatic.PromptForPushPermission(showFallbackSettings);

        [Obsolete("Please use CleverTap.IsPushPermissionGranted() instead.")]
        public static void IsPushPermissionGranted() =>
            CleverTapStatic.IsPushPermissionGranted();

#elif UNITY_ANDROID

        #region Properties
        public static AndroidJavaObject unityCurrentActivity =>
            CleverTapAndroidJNI.UnityActivity;

        public static AndroidJavaObject CleverTapAPI =>
            CleverTapAndroidJNI.CleverTapJNIStatic;

        public static AndroidJavaObject CleverTap =>
            CleverTapAndroidJNI.CleverTapJNIInstance;
        #endregion

        [Obsolete("Please use CleverTap.SetDebugLevel(int) instead.")]
        public static void SetDebugLevel(int level) =>
            CleverTapStatic.SetDebugLevel(level);

        [Obsolete("Please use CleverTap.LaunchWithCredentials(string, string) instead.")]
        public static void Initialize(string accountID, string accountToken) =>
            CleverTapStatic.LaunchWithCredentials(accountID, accountToken);

        [Obsolete("Please use CleverTap.LaunchWithCredentialsForRegion(string, string, string) instead.")]
        public static void Initialize(string accountID, string accountToken, string region) =>
            CleverTapStatic.LaunchWithCredentialsForRegion(accountID, accountToken, region);

        [Obsolete("Not supported for Android.")]
        public static void LaunchWithCredentials(string accountID, string token)
        {
            //no op only supported on ios
        }

        [Obsolete("Not supported for Android.")]
        public static void LaunchWithCredentials(string accountID, string token, string region)
        {
            //no op only supported on ios
        }

        [Obsolete("Not supported for Android.")]
        public static void RegisterPush()
        {
            //no op only supported on ios
        }

        [Obsolete("Please use CleverTap.CreateNotificationChannel(string, string, string, int, bool) instead.")]
        public static void CreateNotificationChannel(string channelId, string channelName, string channelDescription, int importance, bool showBadge) =>
            CleverTapStatic.CreateNotificationChannel(channelId, channelName, channelDescription, importance, showBadge);

        [Obsolete("Please use CleverTap.CreateNotificationChannelWithSound(string, string, string, int, bool, string) instead.")]
        public static void CreateNotificationChannelWithSound(string channelId, string channelName, string channelDescription, int importance, bool showBadge, string sound) =>
            CleverTapStatic.CreateNotificationChannelWithSound(channelId, channelName, channelDescription, importance, showBadge, sound);

        [Obsolete("Please use CleverTap.CreateNotificationChannelWithGroup(string, string, string, int, string, bool) instead.")]
        public static void CreateNotificationChannelWithGroup(string channelId, string channelName, string channelDescription, int importance, string groupId, bool showBadge) =>
            CleverTapStatic.CreateNotificationChannelWithGroup(channelId, channelName, channelDescription, importance, groupId, showBadge);

        [Obsolete("Please use CleverTap.CreateNotificationChannelWithGroupAndSound(string, string, string, int, string, bool, string) instead.")]
        public static void CreateNotificationChannelWithGroupAndSound(string channelId, string channelName, string channelDescription, int importance, string groupId, bool showBadge, string sound) =>
            CleverTapStatic.CreateNotificationChannelWithGroupAndSound(channelId, channelName, channelDescription, importance, groupId, showBadge, sound);

        [Obsolete("Please use CleverTap.CreateNotificationChannelGroup(string, string) instead.")]
        public static void CreateNotificationChannelGroup(string groupId, string groupName) =>
            CleverTapStatic.CreateNotificationChannelGroup(groupId, groupName);

        [Obsolete("Please use CleverTap.DeleteNotificationChannel(string) instead.")]
        public static void DeleteNotificationChannel(string channelId) =>
            CleverTapStatic.DeleteNotificationChannel(channelId);

        [Obsolete("Please use CleverTap.DeleteNotificationChannelGroup(string) instead.")]
        public static void DeleteNotificationChannelGroup(string groupId) =>
            CleverTapStatic.DeleteNotificationChannelGroup(groupId);

        [Obsolete("Please use CleverTap.SetOptOut(bool) instead.")]
        public static void SetOptOut(bool value) =>
            CleverTapStatic.SetOptOut(value);

        [Obsolete("Please use CleverTap.SetOffline(bool) instead.")]
        public static void SetOffline(bool value) =>
            CleverTapStatic.SetOffline(value);

        [Obsolete("Please use CleverTap.EnableDeviceNetworkInfoReporting(bool) instead.")]
        public static void EnableDeviceNetworkInfoReporting(bool value) =>
            CleverTapStatic.EnableDeviceNetworkInfoReporting(value);

        [Obsolete("Please use CleverTap.EnablePersonalization() instead.")]
        public static void EnablePersonalization() =>
            CleverTapStatic.EnablePersonalization();

        [Obsolete("Please use CleverTap.DisablePersonalization() instead.")]
        public static void DisablePersonalization() =>
            CleverTapStatic.DisablePersonalization();

        [Obsolete("Please use CleverTap.SetLocation(double, double) instead.")]
        public static void SetLocation(double lat, double lon) =>
            CleverTapStatic.SetLocation(lat, lon);

        [Obsolete("Please use CleverTap.OnUserLogin(Dictionary<string, string>) instead.")]
        public static void OnUserLogin(Dictionary<string, string> properties) =>
            CleverTapStatic.OnUserLogin(properties);

        [Obsolete("Please use CleverTap.ProfilePush(Dictionary<string, string>) instead.")]
        public static void ProfilePush(Dictionary<string, string> properties) =>
            CleverTapStatic.ProfilePush(properties);

        [Obsolete("Please use CleverTap.OnUserLogin(Dictionary<string, object>) instead.")]
        public static void OnUserLogin(Dictionary<string, object> properties) =>
            CleverTapStatic.OnUserLogin(properties);

        [Obsolete("Please use CleverTap.ProfilePush(Dictionary<string, object>) instead.")]
        public static void ProfilePush(Dictionary<string, object> properties) =>
            CleverTapStatic.ProfilePush(properties);

        [Obsolete("Please use CleverTap.ProfileGet(string) instead.")]
        public static string ProfileGet(string key) =>
            CleverTapStatic.ProfileGet(key);

        [Obsolete("Please use CleverTap.ProfileGetCleverTapAttributionIdentifier() instead.")]
        public static string ProfileGetCleverTapAttributionIdentifier() =>
            CleverTapStatic.ProfileGetCleverTapAttributionIdentifier();

        [Obsolete("Please use CleverTap.ProfileGetCleverTapID() instead.")]
        public static string ProfileGetCleverTapID() =>
            CleverTapStatic.ProfileGetCleverTapID();

        [Obsolete("Please use CleverTap.GetCleverTapId() instead.")]
        public static void GetCleverTapId() =>
            CleverTapStatic.GetCleverTapID();

        [Obsolete("Please use CleverTap.ProfileIncrementValueForKey(string, double) instead.")]
        public static void ProfileIncrementValueForKey(string key, double val) =>
            CleverTapStatic.ProfileIncrementValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileIncrementValueForKey(string, int) instead.")]
        public static void ProfileIncrementValueForKey(string key, int val) =>
            CleverTapStatic.ProfileIncrementValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileDecrementValueForKey(string, double) instead.")]
        public static void ProfileDecrementValueForKey(string key, double val) =>
            CleverTapStatic.ProfileDecrementValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileDecrementValueForKey(string, int) instead.")]
        public static void ProfileDecrementValueForKey(string key, int val) =>
            CleverTapStatic.ProfileDecrementValueForKey(key, val);

        [Obsolete("Please use CleverTap.SuspendInAppNotifications() instead.")]
        public static void SuspendInAppNotifications() =>
            CleverTapStatic.SuspendInAppNotifications();

        [Obsolete("Please use CleverTap.DiscardInAppNotifications() instead.")]
        public static void DiscardInAppNotifications() =>
            CleverTapStatic.DiscardInAppNotifications();

        [Obsolete("Please use CleverTap.ResumeInAppNotifications() instead.")]
        public static void ResumeInAppNotifications() =>
            CleverTapStatic.ResumeInAppNotifications();

        [Obsolete("Please use CleverTap.ProfileRemoveValueForKey(string) instead.")]
        public static void ProfileRemoveValueForKey(string key) =>
            CleverTapStatic.ProfileRemoveValueForKey(key);

        [Obsolete("Please use CleverTap.ProfileSetMultiValuesForKey(string, List<string>) instead.")]
        public static void ProfileSetMultiValuesForKey(string key, List<string> values) =>
            CleverTapStatic.ProfileSetMultiValuesForKey(key, values);

        [Obsolete("Please use CleverTap.ProfileAddMultiValuesForKey(string, List<string>) instead.")]
        public static void ProfileAddMultiValuesForKey(string key, List<string> values) =>
            CleverTapStatic.ProfileAddMultiValuesForKey(key, values);

        [Obsolete("Please use CleverTap.ProfileRemoveMultiValuesForKey(string, List<string>) instead.")]
        public static void ProfileRemoveMultiValuesForKey(string key, List<string> values) =>
            CleverTapStatic.ProfileRemoveMultiValuesForKey(key, values);

        [Obsolete("Please use CleverTap.ProfileAddMultiValueForKey(string, string) instead.")]
        public static void ProfileAddMultiValueForKey(string key, string val) =>
            CleverTapStatic.ProfileAddMultiValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileRemoveMultiValueForKey(string, string) instead.")]
        public static void ProfileRemoveMultiValueForKey(string key, string val) =>
            CleverTapStatic.ProfileRemoveMultiValueForKey(key, val);

        [Obsolete("Please use CleverTap.RecordScreenView(string) instead.")]
        public static void RecordScreenView(string screenName) =>
            CleverTapStatic.RecordScreenView(screenName);

        [Obsolete("Please use CleverTap.RecordEvent(string) instead.")]
        public static void RecordEvent(string eventName) =>
            CleverTapStatic.RecordEvent(eventName);

        [Obsolete("Please use CleverTap.RecordEvent(string, Dictionary<string, object>) instead.")]
        public static void RecordEvent(string eventName, Dictionary<string, object> properties) =>
            CleverTapStatic.RecordEvent(eventName, properties);

        [Obsolete("Please use CleverTap.RecordChargedEventWithDetailsAndItems(Dictionary<string, object>, List<Dictionary<string, object>>) instead.")]
        public static void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>> items) =>
            CleverTapStatic.RecordChargedEventWithDetailsAndItems(details, items);

        [Obsolete("Please use CleverTap.EventGetFirstTime(string) instead.")]
        public static int EventGetFirstTime(string eventName) =>
            CleverTapStatic.EventGetFirstTime(eventName);

        [Obsolete("Please use CleverTap.EventGetLastTime(string) instead.")]
        public static int EventGetLastTime(string eventName) =>
            CleverTapStatic.EventGetLastTime(eventName);

        [Obsolete("Please use CleverTap.EventGetOccurrences(string) instead.")]
        public static int EventGetOccurrences(string eventName) =>
            CleverTapStatic.EventGetOccurrences(eventName);

        [Obsolete("Please use CleverTap.EventGetDetail(string) instead.")]
        public static JSONClass EventGetDetail(string eventName) =>
            CleverTapStatic.EventGetDetail(eventName);

        [Obsolete("Please use CleverTap.UserGetEventHistory() instead.")]
        public static JSONClass UserGetEventHistory() =>
            CleverTapStatic.UserGetEventHistory();

        [Obsolete("Please use CleverTap.SessionGetUTMDetails() instead.")]
        public static JSONClass SessionGetUTMDetails() =>
            CleverTapStatic.SessionGetUTMDetails();

        [Obsolete("Please use CleverTap.SessionGetTimeElapsed() instead.")]
        public static int SessionGetTimeElapsed() =>
            CleverTapStatic.SessionGetTimeElapsed();

        [Obsolete("Please use CleverTap.UserGetTotalVisits() instead.")]
        public static int UserGetTotalVisits() =>
            CleverTapStatic.UserGetTotalVisits();

        [Obsolete("Please use CleverTap.UserGetScreenCount() instead.")]
        public static int UserGetScreenCount() =>
            CleverTapStatic.UserGetScreenCount();

        [Obsolete("Please use CleverTap.UserGetPreviousVisitTime() instead.")]
        public static int UserGetPreviousVisitTime() =>
            CleverTapStatic.UserGetPreviousVisitTime();

        [Obsolete("Not supported for Android.")]
        public static void SetApplicationIconBadgeNumber(int num)
        {
            // no-op for Android
        }

        [Obsolete("Please use CleverTap.PushInstallReferrerSource(string, string, string) instead.")]
        public static void PushInstallReferrerSource(string source, string medium, string campaign) =>
            CleverTapStatic.PushInstallReferrerSource(source, medium, campaign);

        [Obsolete("Please use CleverTap.InitializeInbox() instead.")]
        public static void InitializeInbox() =>
            CleverTapStatic.InitializeInbox();

        [Obsolete("Please use CleverTap.ShowAppInbox(string) instead.")]
        public static void ShowAppInbox(string styleConfig) =>
            CleverTapStatic.ShowAppInbox(styleConfig);

        [Obsolete("Please use CleverTap.DismissAppInbox() instead.")]
        public static void DismissAppInbox() =>
            CleverTapStatic.DismissAppInbox();

        [Obsolete("Please use CleverTap.GetInboxMessageCount() instead.")]
        public static int GetInboxMessageCount() =>
            CleverTapStatic.GetInboxMessageCount();

        [Obsolete("Please use CleverTap.DeleteInboxMessagesForIDs(string[]) instead.")]
        public static void DeleteInboxMessagesForIDs(string[] messageIds) =>
            CleverTapStatic.DeleteInboxMessagesForIDs(messageIds);

        [Obsolete("Please use CleverTap.DeleteInboxMessageForID(string) instead.")]
        public static void DeleteInboxMessageForID(string messageId) =>
            CleverTapStatic.DeleteInboxMessageForID(messageId);

        [Obsolete("Please use CleverTap.MarkReadInboxMessagesForIDs(string[]) instead.")]
        public static void MarkReadInboxMessagesForIDs(string[] messageIds) =>
            CleverTapStatic.MarkReadInboxMessagesForIDs(messageIds);

        [Obsolete("Please use CleverTap.MarkReadInboxMessageForID(string) instead.")]
        public static void MarkReadInboxMessageForID(string messageId) =>
            CleverTapStatic.MarkReadInboxMessageForID(messageId);

        [Obsolete("Please use CleverTap.GetInboxMessageUnreadCount() instead. ")]
        public static int GetInboxMessageUnreadCount() =>
            CleverTapStatic.GetInboxMessageUnreadCount();

        [Obsolete("Please use CleverTap.PromptPushPrimer(Dictionary<string, object>) instead.")]
        public static void PromptPushPrimer(Dictionary<string, object> details) =>
            CleverTapStatic.PromptPushPrimer(details);

        [Obsolete("Please use CleverTap.PromptForPushPermission(bool) instead.")]
        public static void PromptForPushPermission(bool showFallbackSettings) =>
            CleverTapStatic.PromptForPushPermission(showFallbackSettings);

        [Obsolete("Please use CleverTap.IsPushPermissionGranted() instead.")]
        public static bool IsPushPermissionGranted() =>
            CleverTapStatic.IsPushPermissionGranted();

#else

        [Obsolete("Please use CleverTap.LaunchWithCredentialsForRegion(string, string, string) instead.")]
        public static void LaunchWithCredentials(string accountID, string token, string region) =>
            CleverTapStatic.LaunchWithCredentialsForRegion(accountID, token, region);

        [Obsolete("Please use CleverTap.OnUserLogin(Dictionary<string, string>) instead.")]
        public static void OnUserLogin(Dictionary<string, string> properties) =>
            CleverTapStatic.OnUserLogin(properties);

        [Obsolete("Please use CleverTap.ProfilePush(Dictionary<string, string>) instead.")]
        public static void ProfilePush(Dictionary<string, string> properties) =>
            CleverTapStatic.ProfilePush(properties);

        [Obsolete("Please use CleverTap.OnUserLogin(Dictionary<string, object>) instead.")]
        public static void OnUserLogin(Dictionary<string, object> properties) =>
            CleverTapStatic.OnUserLogin(properties);

        [Obsolete("Please use CleverTap.ProfilePush(Dictionary<string, object>) instead.")]
        public static void ProfilePush(Dictionary<string, object> properties) =>
            CleverTapStatic.ProfilePush(properties);

        [Obsolete("Please use CleverTap.ProfileGet(string) instead.")]
        public static string ProfileGet(string key) =>
            CleverTapStatic.ProfileGet(key);

        [Obsolete("Please use CleverTap.ProfileGetCleverTapAttributionIdentifier() instead.")]
        public static string ProfileGetCleverTapAttributionIdentifier() =>
            CleverTapStatic.ProfileGetCleverTapAttributionIdentifier();

        [Obsolete("Please use CleverTap.ProfileGetCleverTapID() instead.")]
        public static string ProfileGetCleverTapID() =>
            CleverTapStatic.ProfileGetCleverTapID();

        [Obsolete("Please use CleverTap.ProfileRemoveValueForKey(string) instead.")]
        public static void ProfileRemoveValueForKey(string key) =>
            CleverTapStatic.ProfileRemoveValueForKey(key);

        [Obsolete("Please use CleverTap.ProfileSetMultiValuesForKey(string, List<string>) instead.")]
        public static void ProfileSetMultiValuesForKey(string key, List<string> values) =>
            CleverTapStatic.ProfileSetMultiValuesForKey(key, values);

        [Obsolete("Please use CleverTap.ProfileAddMultiValuesForKey(string, List<string>) instead.")]
        public static void ProfileAddMultiValuesForKey(string key, List<string> values) =>
            CleverTapStatic.ProfileAddMultiValuesForKey(key, values);

        [Obsolete("Please use CleverTap.ProfileRemoveMultiValuesForKey(string, List<string>) instead.")]
        public static void ProfileRemoveMultiValuesForKey(string key, List<string> values) =>
            CleverTapStatic.ProfileRemoveMultiValuesForKey(key, values);

        [Obsolete("Please use CleverTap.ProfileAddMultiValueForKey(string, string) instead.")]
        public static void ProfileAddMultiValueForKey(string key, string val) =>
            CleverTapStatic.ProfileAddMultiValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileRemoveMultiValueForKey(string, string) instead.")]
        public static void ProfileRemoveMultiValueForKey(string key, string val) =>
            CleverTapStatic.ProfileRemoveMultiValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileIncrementValueForKey(string, double) instead.")]
        public static void ProfileIncrementValueForKey(string key, double val) =>
            CleverTapStatic.ProfileIncrementValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileIncrementValueForKey(string, int) instead.")]
        public static void ProfileIncrementValueForKey(string key, int val) =>
            CleverTapStatic.ProfileIncrementValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileDecrementValueForKey(string, double) instead.")]
        public static void ProfileDecrementValueForKey(string key, double val) =>
            CleverTapStatic.ProfileDecrementValueForKey(key, val);

        [Obsolete("Please use CleverTap.ProfileDecrementValueForKey(string, int) instead.")]
        public static void ProfileDecrementValueForKey(string key, int val) =>
            CleverTapStatic.ProfileDecrementValueForKey(key, val);

        [Obsolete("Please use CleverTap.SuspendInAppNotifications() instead.")]
        public static void SuspendInAppNotifications() =>
            CleverTapStatic.SuspendInAppNotifications();

        [Obsolete("Please use CleverTap.DiscardInAppNotifications() instead.")]
        public static void DiscardInAppNotifications() =>
            CleverTapStatic.DiscardInAppNotifications();

        [Obsolete("Please use CleverTap.ResumeInAppNotifications() instead.")]
        public static void ResumeInAppNotifications() =>
            CleverTapStatic.ResumeInAppNotifications();

        [Obsolete("Please use CleverTap.GetCleverTapID() instead.")]
        public static string GetCleverTapID() =>
            CleverTapStatic.GetCleverTapID();

        [Obsolete("Please use CleverTap.RecordScreenView(string) instead.")]
        public static void RecordScreenView(string screenName) =>
            CleverTapStatic.RecordScreenView(screenName);

        [Obsolete("Please use CleverTap.RecordEvent(string) instead.")]
        public static void RecordEvent(string eventName) =>
            CleverTapStatic.RecordEvent(eventName);

        [Obsolete("Please use CleverTap.RecordEvent(string, Dictionary<string, object>) instead.")]
        public static void RecordEvent(string eventName, Dictionary<string, object> properties) =>
            CleverTapStatic.RecordEvent(eventName, properties);

        [Obsolete("Please use CleverTap.RecordChargedEventWithDetailsAndItems(Dictionary<string, object>, List<Dictionary<string, object>>) instead.")]
        public static void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>> items) =>
            CleverTapStatic.RecordChargedEventWithDetailsAndItems(details, items);

        [Obsolete("Please use CleverTap.EventGetFirstTime(string) instead.")]
        public static int EventGetFirstTime(string eventName) =>
            CleverTapStatic.EventGetFirstTime(eventName);

        [Obsolete("Please use CleverTap.EventGetLastTime(string) instead.")]
        public static int EventGetLastTime(string eventName) =>
            CleverTapStatic.EventGetLastTime(eventName);

        [Obsolete("Please use CleverTap.EventGetOccurrences(string) instead.")]
        public static int EventGetOccurrences(string eventName) =>
            CleverTapStatic.EventGetOccurrences(eventName);

        [Obsolete("Please use CleverTap.EventGetDetail(string) instead.")]
        public static JSONClass EventGetDetail(string eventName) =>
            CleverTapStatic.EventGetDetail(eventName);

        [Obsolete("Please use CleverTap.UserGetEventHistory() instead.")]
        public static JSONClass UserGetEventHistory() =>
            CleverTapStatic.UserGetEventHistory();

        [Obsolete("Please use CleverTap.SessionGetUTMDetails() instead.")]
        public static JSONClass SessionGetUTMDetails() =>
            CleverTapStatic.SessionGetUTMDetails();

        [Obsolete("Please use CleverTap.SessionGetTimeElapsed() instead.")]
        public static int SessionGetTimeElapsed() =>
            CleverTapStatic.SessionGetTimeElapsed();

        [Obsolete("Please use CleverTap.UserGetTotalVisits() instead.")]
        public static int UserGetTotalVisits() =>
            CleverTapStatic.UserGetTotalVisits();

        [Obsolete("Please use CleverTap.UserGetScreenCount() instead.")]
        public static int UserGetScreenCount() =>
            CleverTapStatic.UserGetScreenCount();

        [Obsolete("Please use CleverTap.UserGetPreviousVisitTime() instead.")]
        public static int UserGetPreviousVisitTime() =>
            CleverTapStatic.UserGetPreviousVisitTime();

        [Obsolete("Please use CleverTap.EnablePersonalization() instead.")]
        public static void EnablePersonalization() =>
            CleverTapStatic.EnablePersonalization();

        [Obsolete("Please use CleverTap.DisablePersonalization() instead.")]
        public static void DisablePersonalization() =>
            CleverTapStatic.DisablePersonalization();

        [Obsolete("Please use CleverTap.RegisterPush() instead.")]
        public static void RegisterPush() =>
            CleverTapStatic.RegisterPush();

        [Obsolete("Please use CleverTap.SetApplicationIconBadgeNumber(int) instead.")]
        public static void SetApplicationIconBadgeNumber(int num) =>
            CleverTapStatic.SetApplicationIconBadgeNumber(num);

        [Obsolete("Please use CleverTap.SetDebugLevel(int) instead.")]
        public static void SetDebugLevel(int level) =>
            CleverTapStatic.SetDebugLevel(level);

        [Obsolete("Please use CleverTap.SetLocation(double, double) instead.")]
        public static void SetLocation(double lat, double lon) =>
            CleverTapStatic.SetLocation(lat, lon);

        [Obsolete("Please use CleverTap.PushInstallReferrerSource(string, string, string) instead.")]
        public static void PushInstallReferrerSource(string source, string medium, string campaign) =>
            CleverTapStatic.PushInstallReferrerSource(source, medium, campaign);

        [Obsolete("Please use CleverTap.EnableDeviceNetworkInfoReporting(bool) instead.")]
        public static void EnableDeviceNetworkInfoReporting(bool value) =>
            CleverTapStatic.EnableDeviceNetworkInfoReporting(value);

        [Obsolete("Please use CleverTap.SetOptOut(bool) instead.")]
        public static void SetOptOut(bool value) =>
            CleverTapStatic.SetOptOut(value);

        [Obsolete("Please use CleverTap.SetOffline(bool) instead.")]
        public static void SetOffline(bool value) =>
            CleverTapStatic.SetOffline(value);

        [Obsolete("Please use CleverTap.CreateNotificationChannel(string, string, string, int, bool) instead.")]
        public static void CreateNotificationChannel(string channelId, string channelName, string channelDescription, int importance, bool showBadge) =>
            CleverTapStatic.CreateNotificationChannel(channelId, channelName, channelDescription, importance, showBadge);

        [Obsolete("Please use CleverTap.CreateNotificationChannelWithSound(string, string, string, int, bool, string) instead.")]
        public static void CreateNotificationChannelWithSound(string channelId, string channelName, string channelDescription, int importance, bool showBadge, string sound) =>
            CleverTapStatic.CreateNotificationChannelWithSound(channelId, channelName, channelDescription, importance, showBadge, sound);

        [Obsolete("Please use CleverTap.CreateNotificationChannelWithGroup(string, string, string, int, string, bool) instead.")]
        public static void CreateNotificationChannelWithGroup(string channelId, string channelName, string channelDescription, int importance, string groupId, bool showBadge) =>
            CleverTapStatic.CreateNotificationChannelWithGroup(channelId, channelName, channelDescription, importance, groupId, showBadge);

        [Obsolete("Please use CleverTap.CreateNotificationChannelWithGroupAndSound(string, string, string, int, string, bool, string) instead.")]
        public static void CreateNotificationChannelWithGroupAndSound(string channelId, string channelName, string channelDescription, int importance, string groupId, bool showBadge, string sound) =>
            CleverTapStatic.CreateNotificationChannelWithGroupAndSound(channelId, channelName, channelDescription, importance, groupId, showBadge, sound);

        [Obsolete("Please use CleverTap.CreateNotificationChannelGroup(string, string) instead.")]
        public static void CreateNotificationChannelGroup(string groupId, string groupName) =>
            CleverTapStatic.CreateNotificationChannelGroup(groupId, groupName);

        [Obsolete("Please use CleverTap.DeleteNotificationChannel(string) instead.")]
        public static void DeleteNotificationChannel(string channelId) =>
            CleverTapStatic.DeleteNotificationChannel(channelId);

        [Obsolete("Please use CleverTap.DeleteNotificationChannelGroup(string) instead.")]
        public static void DeleteNotificationChannelGroup(string groupId) =>
            CleverTapStatic.DeleteNotificationChannelGroup(groupId);

        [Obsolete("Please use CleverTap.InitializeInbox() instead.")]
        public static void InitializeInbox() =>
            CleverTapStatic.InitializeInbox();

        [Obsolete("Please use CleverTap.ShowAppInbox(string) instead.")]
        public static void ShowAppInbox(string styleConfig) =>
            CleverTapStatic.ShowAppInbox(styleConfig);

        [Obsolete("Please use CleverTap.DismissAppInbox() instead.")]
        public static void DismissAppInbox() =>
            CleverTapStatic.DismissAppInbox();

        [Obsolete("Please use CleverTap.GetInboxMessageCount() instead.")]
        public static int GetInboxMessageCount() =>
            CleverTapStatic.GetInboxMessageCount();

        [Obsolete("Please use CleverTap.GetInboxMessageUnreadCount() instead.")]
        public static int GetInboxMessageUnreadCount() =>
            CleverTapStatic.GetInboxMessageUnreadCount();

        [Obsolete("Please use CleverTap.DeleteInboxMessagesForIDs(string[]) instead.")]
        public static void DeleteInboxMessagesForIDs(string[] messageIds) =>
            CleverTapStatic.DeleteInboxMessagesForIDs(messageIds);

        [Obsolete("Please use CleverTap.DeleteInboxMessageForID(string) instead.")]
        public static void DeleteInboxMessageForID(string messageId) =>
            CleverTapStatic.DeleteInboxMessageForID(messageId);

        [Obsolete("Please use CleverTap.MarkReadInboxMessagesForIDs(string[]) instead.")]
        public static void MarkReadInboxMessagesForIDs(string[] messageIds) =>
            CleverTapStatic.MarkReadInboxMessagesForIDs(messageIds);

        [Obsolete("Please use CleverTap.MarkReadInboxMessageForID(string) instead.")]
        public static void MarkReadInboxMessageForID(string messageId) =>
            CleverTapStatic.MarkReadInboxMessageForID(messageId);

        [Obsolete("Please use CleverTap.PromptPushPrimer(Dictionary<string, object>) instead.")]
        public static void PromptPushPrimer(Dictionary<string, object> json) =>
            CleverTapStatic.PromptPushPrimer(json);

        [Obsolete("Please use CleverTap.PromptForPushPermission(bool) instead.")]
        public static void PromptForPushPermission(bool showFallbackSettings) =>
            CleverTapStatic.PromptForPushPermission(showFallbackSettings);

        [Obsolete("Please use CleverTap.IsPushPermissionGranted() instead.")]
        public static bool IsPushPermissionGranted() =>
            CleverTapStatic.IsPushPermissionGranted();

#endif
    }
}