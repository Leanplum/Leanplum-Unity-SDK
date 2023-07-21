using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CleverTap.Utilities;

/// <summary>
/// These methods can be called by Unity applications to record
/// events and set and get user profile attributes.
/// </summary>

namespace CleverTap {
  public class CleverTapBinding : MonoBehaviour {
      
  public const string Version = "2.4.0";

#if UNITY_IOS
    void Start() {
        Debug.Log("Start: CleverTap binding for iOS.");
    }

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_launchWithCredentials(string accountID, string token);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_launchWithCredentialsForRegion(string accountID, string token, string region);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_onUserLogin(string properties);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_profilePush(string properties);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_profileGet(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_profileGetCleverTapAttributionIdentifier();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_profileGetCleverTapID();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_profileRemoveValueForKey(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_profileSetMultiValuesForKey(string key, string[] array, int size);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_profileAddMultiValuesForKey(string key, string[] array, int size);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_profileRemoveMultiValuesForKey(string key, string[] array, int size);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_profileAddMultiValueForKey(string key, string val);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_profileRemoveMultiValueForKey(string key, string val);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_profileIncrementDoubleValueForKey(string key, double val);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_profileIncrementIntValueForKey(string key, int val);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_profileDecrementDoubleValueForKey(string key, double val);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_profileDecrementIntValueForKey(string key, int val);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_suspendInAppNotifications();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_discardInAppNotifications();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_resumeInAppNotifications();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_getCleverTapID();
        
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_recordScreenView(string screenName);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_recordEvent(string eventName, string properties);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_recordChargedEventWithDetailsAndItems(string details, string items);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_setOffline(bool enabled);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_setOptOut(bool enabled);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_enableDeviceNetworkInfoReporting(bool enabled);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_registerPush();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_setApplicationIconBadgeNumber(int num);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_setDebugLevel(int level);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_enablePersonalization();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_disablePersonalization();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_setLocation(double lat, double lon);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int CleverTap_eventGetFirstTime(string eventName);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int CleverTap_eventGetLastTime(string eventName);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int CleverTap_eventGetOccurrences(string eventName);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_userGetEventHistory();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_sessionGetUTMDetails();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int CleverTap_sessionGetTimeElapsed();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_eventGetDetail(string eventName);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int CleverTap_userGetTotalVisits();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int CleverTap_userGetScreenCount();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int CleverTap_userGetPreviousVisitTime();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_pushInstallReferrerSource(string source, string medium, string campaign);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_showAppInbox(string styleConfig);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_dismissAppInbox();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int CleverTap_getInboxMessageCount();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int CleverTap_getInboxMessageUnreadCount();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int CleverTap_initializeInbox();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_getAllInboxMessages();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_getUnreadInboxMessages();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_getInboxMessageForId(string messageId);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_deleteInboxMessageForID(string messageId);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_deleteInboxMessagesForIDs(string[] messageIds,int arrLength);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_markReadInboxMessageForID(string messageId);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_markReadInboxMessagesForIDs(string[] messageIds,int arrLength);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_recordInboxNotificationViewedEventForID(string messageId);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_recordInboxNotificationClickedEventForID(string messageId);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_getAllDisplayUnits();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_getDisplayUnitForID(string unitID);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_recordDisplayUnitViewedEventForID(string unitID);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_recordDisplayUnitClickedEventForID(string unitID);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_fetchProductConfig();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_fetchProductConfigWithMinimumInterval(double minimumInterval);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_setProductConfigMinimumFetchInterval(double minimumFetchInterval);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_activateProductConfig();
    
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_fetchAndActivateProductConfig();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_setProductConfigDefaults(string defaults);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_setProductConfigDefaultsFromPlistFileName(string fileName);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CleverTap_getProductConfigValueFor(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern double CleverTap_getProductConfigLastFetchTimeStamp();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_resetProductConfig();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern bool CleverTap_getFeatureFlag(string key, bool defaultValue);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_promptForPushPermission(bool showFallbackSettings);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_promptPushPrimer(string json);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CleverTap_isPushPermissionGranted();

    public static void LaunchWithCredentials(string accountID, string token) {
        CleverTap_launchWithCredentials(accountID, token);
    }

    public static void LaunchWithCredentialsForRegion(string accountID, string token, string region) {
        CleverTap_launchWithCredentialsForRegion(accountID, token, region);
    }

    public static void OnUserLogin(Dictionary<string, string> properties) {
        var propertiesString = Json.Serialize(properties);
        CleverTap_onUserLogin(propertiesString);
    }

    public static void ProfilePush(Dictionary<string, string> properties) {
        var propertiesString = Json.Serialize(properties);
        CleverTap_profilePush(propertiesString);
    }

    public static string ProfileGet(string key) {
        string ret = CleverTap_profileGet(key);
        return ret;
    }

    public static string ProfileGetCleverTapAttributionIdentifier() {
        string ret = CleverTap_profileGetCleverTapAttributionIdentifier();
        return ret;
    }

    public static string ProfileGetCleverTapID() {
        string ret = CleverTap_profileGetCleverTapID();
        return ret;
    }

    public static void ProfileRemoveValueForKey(string key) {
        CleverTap_profileRemoveValueForKey(key);
    }

    public static void ProfileSetMultiValuesForKey(string key, List<string> values) {
        CleverTap_profileSetMultiValuesForKey(key, values.ToArray(), values.Count);
    }

    public static void ProfileAddMultiValuesForKey(string key, List<string> values) {
        CleverTap_profileAddMultiValuesForKey(key, values.ToArray(), values.Count);
    }

    public static void ProfileRemoveMultiValuesForKey(string key, List<string> values) {
        CleverTap_profileRemoveMultiValuesForKey(key, values.ToArray(), values.Count);
    }

    public static void ProfileAddMultiValueForKey(string key, string val) {
        CleverTap_profileAddMultiValueForKey(key, val);
    }

    public static void ProfileRemoveMultiValueForKey(string key, string val) {
        CleverTap_profileRemoveMultiValueForKey(key, val);
    }

    public static void ProfileIncrementValueForKey(string key, double val) {
        CleverTap_profileIncrementDoubleValueForKey(key, val);
    }

    public static void ProfileIncrementValueForKey(string key, int val) {
        CleverTap_profileIncrementIntValueForKey(key, val);
    }

    public static void ProfileDecrementValueForKey(string key, double val) {
        CleverTap_profileDecrementDoubleValueForKey(key, val);
    }

    public static void ProfileDecrementValueForKey(string key, int val) {
        CleverTap_profileDecrementIntValueForKey(key, val);
    }

    public static void SuspendInAppNotifications() {
        CleverTap_suspendInAppNotifications();
    }

    public static void DiscardInAppNotifications() {
        CleverTap_discardInAppNotifications();
    }

    public static void ResumeInAppNotifications() {
        CleverTap_resumeInAppNotifications();
    }

    public static string GetCleverTapID() {
         string ret = CleverTap_getCleverTapID();
         return ret;
    }

    public static void RecordScreenView(string screenName) {
        CleverTap_recordScreenView(screenName);
    }

    public static void RecordEvent(string eventName) {
        CleverTap_recordEvent(eventName, null);
    }

    public static void RecordEvent(string eventName, Dictionary<string, object> properties) {
        var propertiesString = Json.Serialize(properties);
        CleverTap_recordEvent(eventName, propertiesString);
    }

    public static void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>>items) {
        var detailsString = Json.Serialize(details);
        var itemsString = Json.Serialize(items);
        CleverTap_recordChargedEventWithDetailsAndItems(detailsString, itemsString);
    }

    public static int EventGetFirstTime(string eventName) {
        return CleverTap_eventGetFirstTime(eventName);
    }

    public static int EventGetLastTime(string eventName) {
        return CleverTap_eventGetLastTime(eventName);
    }

    public static int EventGetOccurrences(string eventName) {
        return CleverTap_eventGetOccurrences(eventName);
    }

    public static JSONClass UserGetEventHistory() {
        string jsonString = CleverTap_userGetEventHistory();
        JSONClass json;
        try {
            json = (JSONClass)JSON.Parse(jsonString);
        } catch {
            Debug.LogError("Unable to parse user event history json");
            json = new JSONClass();
        }
        return json;
    }

    public static JSONClass SessionGetUTMDetails() {
        string jsonString = CleverTap_sessionGetUTMDetails();
        JSONClass json;
        try {
            json = (JSONClass)JSON.Parse(jsonString);
        } catch {
            Debug.LogError("Unable to parse session utm details json");
            json = new JSONClass();
        }
        return json;
    }

    public static int SessionGetTimeElapsed() {
        return CleverTap_sessionGetTimeElapsed();
    }

    public static JSONClass EventGetDetail(string eventName) {
        string jsonString = CleverTap_eventGetDetail(eventName);
        JSONClass json;
        try {
            json = (JSONClass)JSON.Parse(jsonString);
        } catch {
            Debug.LogError("Unable to parse event detail json");
            json = new JSONClass();
        }
        return json;
    }

    public static int UserGetTotalVisits() {
        return CleverTap_userGetTotalVisits();
    }

    public static int UserGetScreenCount() {
        return CleverTap_userGetScreenCount();
    }

    public static int UserGetPreviousVisitTime() {
        return CleverTap_userGetPreviousVisitTime();
    }

    public static void RegisterPush() {
        CleverTap_registerPush();
    }

    public static void SetApplicationIconBadgeNumber(int num) {
        CleverTap_setApplicationIconBadgeNumber(num);
    }

    public static void SetDebugLevel(int level) {
        CleverTap_setDebugLevel(level);
    }

    public static void EnablePersonalization() {
        CleverTap_enablePersonalization();
    }

    public static void DisablePersonalization() {
        CleverTap_disablePersonalization();
    }

    public static void SetLocation(double lat, double lon) {
        CleverTap_setLocation(lat, lon);
    }

    public static void PushInstallReferrerSource(string source, string medium, string campaign) {
        CleverTap_pushInstallReferrerSource(source, medium, campaign);
    }

    public static void SetOffline(bool enabled) {
        CleverTap_setOffline(enabled);
    }

    public static void SetOptOut(bool enabled) {
        CleverTap_setOptOut(enabled);
    }

    public static void EnableDeviceNetworkInfoReporting(bool enabled) {
        CleverTap_enableDeviceNetworkInfoReporting(enabled);
    }

    public static void InitializeInbox() {
        CleverTap_initializeInbox();
    }

    public static void ShowAppInbox(Dictionary<string, object> styleConfig) {
        var styleConfigString = Json.Serialize(styleConfig);
        CleverTap_showAppInbox(styleConfigString);
    }

    public static void DismissAppInbox() {
        CleverTap_dismissAppInbox();
    }

    public static int GetInboxMessageCount() {
        return CleverTap_getInboxMessageCount();
    }

    public static int GetInboxMessageUnreadCount() {
        return CleverTap_getInboxMessageUnreadCount();
    }

    public static JSONArray GetAllInboxMessages() {
        string jsonString = CleverTap_getAllInboxMessages();
        JSONArray json;
        try {
            json = (JSONArray)JSON.Parse(jsonString);
        } catch {
            Debug.LogError("Unable to parse app inbox messages json");  
            json = new JSONArray();
        }
        return json;
    }

    public static JSONArray GetUnreadInboxMessages() {
        string jsonString = CleverTap_getUnreadInboxMessages();
        JSONArray json;
        try {
            json = (JSONArray)JSON.Parse(jsonString);
        } catch {
            Debug.LogError("Unable to parse unread app inbox messages json");  
            json = new JSONArray();
        }
        return json;
    }

    public static JSONClass GetInboxMessageForId(string messageId) {
        string jsonString = CleverTap_getInboxMessageForId(messageId);
        JSONClass json;
        try {
            json = (JSONClass)JSON.Parse(jsonString);
        } catch {
                Debug.LogError("Unable to parse app inbox message json");
                json = new JSONClass();
        }
        return json;
    }

    public static void DeleteInboxMessageForID(string messageId) {
        CleverTap_deleteInboxMessageForID(messageId);   
    }

    public static void DeleteInboxMessagesForIDs(string[] messageIds) {
        int arrLength = messageIds.Length;
        CleverTap_deleteInboxMessagesForIDs(messageIds, arrLength);
    }

    public static void MarkReadInboxMessageForID(string messageId) {
        CleverTap_markReadInboxMessageForID(messageId);
    }

    public static void MarkReadInboxMessagesForIDs(string[] messageIds) {
        int arrLength = messageIds.Length;
        CleverTap_markReadInboxMessagesForIDs(messageIds, arrLength);
    }

    public static void RecordInboxNotificationViewedEventForID(string messageId) {
        CleverTap_recordInboxNotificationViewedEventForID(messageId);
    }

    public static void RecordInboxNotificationClickedEventForID(string messageId) {
        CleverTap_recordInboxNotificationClickedEventForID(messageId);
    }

    public static JSONArray GetAllDisplayUnits() {
        string jsonString = CleverTap_getAllDisplayUnits();
        JSONArray json;
        try {
            json = (JSONArray)JSON.Parse(jsonString);
        } catch {
            Debug.LogError("Unable to parse native display units json");  
            json = new JSONArray();
        }
        return json;
    }

    public static JSONClass GetDisplayUnitForID(string unitID) {
        string jsonString = CleverTap_getDisplayUnitForID(unitID);
        JSONClass json;
        try {
            json = (JSONClass)JSON.Parse(jsonString);
        } catch {
                Debug.LogError("Unable to parse native display unit json");
                json = new JSONClass();
        }
        return json;
    }

    public static void RecordDisplayUnitViewedEventForID(string unitID) {
        CleverTap_recordDisplayUnitViewedEventForID(unitID);
    }

    public static void RecordDisplayUnitClickedEventForID(string unitID) {
        CleverTap_recordDisplayUnitClickedEventForID(unitID);
    }

    public static void FetchProductConfig() {
        CleverTap_fetchProductConfig();
    }

    public static void FetchProductConfigWithMinimumInterval(double minimumInterval) {
        CleverTap_fetchProductConfigWithMinimumInterval(minimumInterval);
    }

    public static void SetProductConfigMinimumFetchInterval(double minimumFetchInterval) {
        CleverTap_setProductConfigMinimumFetchInterval(minimumFetchInterval);
    }

    public static void ActivateProductConfig() {
        CleverTap_activateProductConfig();
    }

    public static void FetchAndActivateProductConfig() {
        CleverTap_fetchAndActivateProductConfig();
    }

    public static void SetProductConfigDefaults(Dictionary<string, object> defaults) {
        var defaultsString = Json.Serialize(defaults);
        CleverTap_setProductConfigDefaults(defaultsString);
    }

    public static void SetProductConfigDefaultsFromPlistFileName(string fileName) {
        CleverTap_setProductConfigDefaultsFromPlistFileName(fileName);
    }

    public static JSONClass GetProductConfigValueFor(string key) {
        string jsonString = CleverTap_getProductConfigValueFor(key);
        JSONClass json;
        try {
            json = (JSONClass)JSON.Parse(jsonString);
        } catch {
                Debug.LogError("Unable to parse product config value");
                json = new JSONClass();
        }
        return json;
    }

    public static double GetProductConfigLastFetchTimeStamp() {
        return CleverTap_getProductConfigLastFetchTimeStamp();
    }
    
    public static void ResetProductConfig() {
        CleverTap_resetProductConfig();
    }

    public static bool GetFeatureFlag(string key, bool defaultValue) {
        return CleverTap_getFeatureFlag(key, defaultValue);
    }

    public static void PromptPushPrimer(Dictionary<string, object> json) {
        var jsonString = Json.Serialize(json);
        CleverTap_promptPushPrimer(jsonString);
    }


    public static void PromptForPushPermission(bool showFallbackSettings) {
        CleverTap_promptForPushPermission(showFallbackSettings);
    }

    public static void IsPushPermissionGranted() {
        CleverTap_isPushPermissionGranted();
    }

#elif UNITY_ANDROID
    private static AndroidJavaObject unityActivity;
    private static AndroidJavaObject clevertap;
    private static AndroidJavaObject CleverTapClass;

    void Start() {
        Debug.Log("Start: CleverTap binding for Android.");
    }

        #region Properties
    public static AndroidJavaObject unityCurrentActivity {
        get {
            if (unityActivity == null) {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                    unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }
            }
            return unityActivity;
        }
    }

    public static AndroidJavaObject CleverTapAPI {
        get {
            if (CleverTapClass == null) {
                CleverTapClass = new AndroidJavaClass("com.clevertap.unity.CleverTapUnityPlugin");
            }
            return CleverTapClass;
        }
    }

    public static AndroidJavaObject CleverTap {
        get {
            if (clevertap == null) {
                AndroidJavaObject context = unityCurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
                clevertap = CleverTapAPI.CallStatic<AndroidJavaObject>("getInstance", context);
            }
            return clevertap;
        }
    }
        #endregion

    public static void SetDebugLevel(int level) {
        CleverTapAPI.CallStatic("setDebugLevel", level);
    }

    public static void Initialize(string accountID, string accountToken) {
        CleverTapAPI.CallStatic("initialize", accountID, accountToken, unityCurrentActivity);
    }

    public static void Initialize(string accountID, string accountToken, string region) {
        CleverTapAPI.CallStatic("initialize", accountID, accountToken, region, unityCurrentActivity);
    }

    public static void LaunchWithCredentials(string accountID, string token, string region) {
        //no op only supported on ios
    }

    public static void LaunchWithCredentials(string accountID, string token) {
        //no op only supported on ios
    }

    public static void RegisterPush() {
        //no op only supported on ios
    }

    public static void CreateNotificationChannel(string channelId,string channelName, string channelDescription, int importance, bool showBadge){
        AndroidJavaObject context = unityCurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
        CleverTapAPI.CallStatic("createNotificationChannel",context,channelId,channelName,channelDescription,importance,showBadge);
    }

    public static void CreateNotificationChannelWithSound(string channelId,string channelName, string channelDescription, int importance, bool showBadge, string sound){
        AndroidJavaObject context = unityCurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
        CleverTapAPI.CallStatic("createNotificationChannelWithSound",context,channelId,channelName,channelDescription,importance,showBadge,sound);
    }

    public static void CreateNotificationChannelWithGroup(string channelId,string channelName, string channelDescription, int importance, string groupId, bool showBadge){
        AndroidJavaObject context = unityCurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
        CleverTapAPI.CallStatic("createNotificationChannelWithGroup",context,channelId,channelName,channelDescription,importance,groupId,showBadge);
    }

    public static void CreateNotificationChannelWithGroupAndSound(string channelId,string channelName, string channelDescription, int importance, string groupId, bool showBadge, string sound){
        AndroidJavaObject context = unityCurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
        CleverTapAPI.CallStatic("createNotificationChannelWithGroupAndSound",context,channelId,channelName,channelDescription,importance,groupId,showBadge,sound);
    }

    public static void CreateNotificationChannelGroup(string groupId, string groupName){
        AndroidJavaObject context = unityCurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
        CleverTapAPI.CallStatic("createNotificationChannelGroup",context,groupId,groupName);
    }

    public static void DeleteNotificationChannel(string channelId){
        AndroidJavaObject context = unityCurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
        CleverTapAPI.CallStatic("deleteNotificationChannel",context,channelId);
    }

    public static void DeleteNotificationChannelGroup(string groupId){
        AndroidJavaObject context = unityCurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
        CleverTapAPI.CallStatic("deleteNotificationChannelGroup",context,groupId);
    }

    public static void SetOptOut(bool value){
        CleverTap.Call("setOptOut",value);
    }

    public static void SetOffline(bool value){
        CleverTap.Call("setOffline",value);
    }

    public static void EnableDeviceNetworkInfoReporting(bool value){
        CleverTap.Call("enableDeviceNetworkInfoReporting",value);
    }

    public static void EnablePersonalization() {
        CleverTap.Call("enablePersonalization");
    }

    public static void DisablePersonalization() {
        CleverTap.Call("disablePersonalization");
    }

    public static void SetLocation(double lat, double lon) {
        CleverTap.Call("setLocation", lat, lon);
    }

    public static void OnUserLogin(Dictionary<string, string> properties) {
        CleverTap.Call("onUserLogin", Json.Serialize(properties));
    }

    public static void ProfilePush(Dictionary<string, string> properties) {
        CleverTap.Call("profilePush", Json.Serialize(properties));
    }

    public static string ProfileGet(string key) {
        return CleverTap.Call<string>("profileGet", key);
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
    public static string ProfileGetCleverTapAttributionIdentifier() {
        return CleverTap.Call<string>("profileGetCleverTapAttributionIdentifier");
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
    public static string ProfileGetCleverTapID() {
        return CleverTap.Call<string>("profileGetCleverTapID");
    }

    /*
     * requests for a unique, asynchronous CleverTap identifier. The value will be available as json {"cleverTapID" : <value> } via 
     * CleverTapUnity#CleverTapInitCleverTapIdCallback() function
     */
    public static void GetCleverTapId(){
        CleverTap.Call("getCleverTapID");
    }

    /**
     * This method is used to increment the given value.Number should be in positive range
     */
    public static void ProfileIncrementValueForKey(string key, double val) {
        CleverTap.Call("profileIncrementValueForKey",key, val);
    }

    /**
     * This method is used to increment the given value.Number should be in positive range
     */
    public static void ProfileIncrementValueForKey(string key, int val) {
        CleverTap.Call("profileIncrementValueForKey",key, val);
    }

    /**
     * This method is used to decrement the given value.Number should be in positive range
     */
    public static void ProfileDecrementValueForKey(string key, double val) {
        CleverTap.Call("profileDecrementValueForKey",key, val);
    }

    /**
     * This method is used to decrement the given value.Number should be in positive range
     */
    public static void ProfileDecrementValueForKey(string key, int val) {
        CleverTap.Call("profileDecrementValueForKey",key, val);
    }

    /**
     * Suspends display of InApp Notifications.
     * The InApp Notifications are queued once this method is called
     * and will be displayed once resumeInAppNotifications() is called.
     */
    public static void SuspendInAppNotifications() {
        CleverTap.Call("suspendInAppNotifications");
    }

    /**
     * Suspends the display of InApp Notifications and discards any new InApp Notifications to be shown
     * after this method is called.
     * The InApp Notifications will be displayed only once resumeInAppNotifications() is called.
     */
    public static void DiscardInAppNotifications() {
        CleverTap.Call("discardInAppNotifications");
    }

    /**
     * Suspends the display of InApp Notifications and discards any new InApp Notifications to be shown
     * after this method is called.
     * The InApp Notifications will be displayed only once resumeInAppNotifications() is called.
     */
    public static void ResumeInAppNotifications() {
        CleverTap.Call("resumeInAppNotifications");
    }

    public static void ProfileRemoveValueForKey(string key) {
        CleverTap.Call("profileRemoveValueForKey", key);
    }

    public static void ProfileSetMultiValuesForKey(string key, List<string> values) {
        CleverTap.Call("profileSetMultiValuesForKey", key, values.ToArray());
    }

    public static void ProfileAddMultiValuesForKey(string key, List<string> values) {
        CleverTap.Call("profileAddMultiValuesForKey", key, values.ToArray());
    }

    public static void ProfileRemoveMultiValuesForKey(string key, List<string> values) {
        CleverTap.Call("profileRemoveMultiValuesForKey", key, values.ToArray());
    }

    public static void ProfileAddMultiValueForKey(string key, string val) {
        CleverTap.Call("profileAddMultiValueForKey", key, val);
    }

    public static void ProfileRemoveMultiValueForKey(string key, string val) {
        CleverTap.Call("profileRemoveMultiValueForKey", key, val);
    }

    public static void RecordScreenView(string screenName) {
        CleverTap.Call("recordScreenView", screenName);
    }

    public static void RecordEvent(string eventName) {
        CleverTap.Call("recordEvent", eventName, null);
    }

    public static void RecordEvent(string eventName, Dictionary<string, object> properties) {
        CleverTap.Call("recordEvent", eventName, Json.Serialize(properties));
    }

    public static void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>>items) {
        CleverTap.Call("recordChargedEventWithDetailsAndItems", Json.Serialize(details), Json.Serialize(items));
    }

    public static int EventGetFirstTime(string eventName) {
        return CleverTap.Call<int>("eventGetFirstTime", eventName);
    }

    public static int EventGetLastTime(string eventName) {
        return CleverTap.Call<int>("eventGetLastTime", eventName);
    }

    public static int EventGetOccurrences(string eventName) {
        return CleverTap.Call<int>("eventGetOccurrences", eventName);
    }

    public static JSONClass EventGetDetail(string eventName) {
        string jsonString = CleverTap.Call<string>("eventGetDetail", eventName);
        JSONClass json;
        try {
            json = (JSONClass)JSON.Parse(jsonString);
        } catch {
            Debug.Log("Unable to event detail json");
            json = new JSONClass();
        }
        return json;
    }

    public static JSONClass UserGetEventHistory() {
        string jsonString = CleverTap.Call<string>("userGetEventHistory");
        JSONClass json;
        try {
            json = (JSONClass)JSON.Parse(jsonString);
        } catch {
            Debug.Log("Unable to parse user event history json");
            json = new JSONClass();
        }
        return json;
    }

    public static JSONClass SessionGetUTMDetails() {
        string jsonString = CleverTap.Call<string>("sessionGetUTMDetails");
        JSONClass json;
        try {
            json = (JSONClass)JSON.Parse(jsonString);
        } catch {
            Debug.Log("Unable to parse session utm details json");
            json = new JSONClass();
        }
        return json;
    }

    public static int SessionGetTimeElapsed() {
        return CleverTap.Call<int>("sessionGetTimeElapsed");
    }

    public static int UserGetTotalVisits() {
        return CleverTap.Call<int>("userGetTotalVisits");
    }

    public static int UserGetScreenCount() {
        return CleverTap.Call<int>("userGetScreenCount");
    }

    public static int UserGetPreviousVisitTime() {
        return CleverTap.Call<int>("userGetPreviousVisitTime");
    }

    public static void SetApplicationIconBadgeNumber(int num) {
        // no-op for Android
    }

    public static void PushInstallReferrerSource(string source, string medium, string campaign) {
        CleverTap.Call("pushInstallReferrer",source, medium, campaign);
    }

    public static void InitializeInbox(){
        CleverTap.Call("initializeInbox");
    }

    public static void ShowAppInbox(string styleConfig){
         CleverTap.Call("showAppInbox", styleConfig);
    }

    public static void DismissAppInbox(){
         CleverTap.Call("dismissAppInbox");
    }

    public static int GetInboxMessageCount(){
        return CleverTap.Call<int>("getInboxMessageCount");
    }

    public static void DeleteInboxMessagesForIDs(string[] messageIds) {
        CleverTap.Call("deleteInboxMessagesForIDs", messageIds);
    }

    public static void DeleteInboxMessageForID(string messageId) {
        CleverTap.Call("deleteInboxMessageForId", messageId);
    }

    public static void MarkReadInboxMessagesForIDs(string[] messageIds) {
        CleverTap.Call("markReadInboxMessagesForIDs", messageIds);
    }

    public static void MarkReadInboxMessageForID(string messageId) {
        CleverTap.Call("markReadInboxMessageForId", messageId);
    }

    public static int GetInboxMessageUnreadCount(){
        return CleverTap.Call<int>("getInboxMessageUnreadCount");
    }

    public static void PromptPushPrimer(Dictionary<string, object> details){
         CleverTap.Call("promptPushPrimer", Json.Serialize(details));
    }

    public static void PromptForPushPermission(bool showFallbackSettings){
         CleverTap.Call("promptForPushPermission", showFallbackSettings);
    }

    public static bool IsPushPermissionGranted(){
        return CleverTap.Call<bool>("isPushPermissionGranted");
    }
#else

   // Empty implementations of the API, in case the application is being compiled for a platform other than iOS or Android.
    void Start() {
        Debug.Log("Start: no-op CleverTap binding for non iOS/Android.");
    }

    public static void LaunchWithCredentials(string accountID, string token, string region) {
    }

    public static void OnUserLogin(Dictionary<string, string> properties) {
    }

    public static void ProfilePush(Dictionary<string, string> properties) {
    }

    public static string ProfileGet(string key) {
        return "test";
    }

    public static string ProfileGetCleverTapAttributionIdentifier() {
        return "testAttributionIdentifier";
    }

    public static string ProfileGetCleverTapID() {
        return "testCleverTapID";
    }

    public static void ProfileRemoveValueForKey(string key) {
    }

    public static void ProfileSetMultiValuesForKey(string key, List<string> values) {
    }

    public static void ProfileAddMultiValuesForKey(string key, List<string> values) {
    }

    public static void ProfileRemoveMultiValuesForKey(string key, List<string> values) {
    }

    public static void ProfileAddMultiValueForKey(string key, string val) {
    }

    public static void ProfileRemoveMultiValueForKey(string key, string val) {
    }

    public static void ProfileIncrementValueForKey(string key, double val) {
    }

    public static void ProfileIncrementValueForKey(string key, int val) {
    }

    public static void ProfileDecrementValueForKey(string key, double val) {
    }

    public static void ProfileDecrementValueForKey(string key, int val) {
    }

    public static void SuspendInAppNotifications() {
    }

    public static void DiscardInAppNotifications() {
    }

    public static void ResumeInAppNotifications() {
    }

    public static string GetCleverTapID() {
        return "testCleverTapID";
    }

    public static void RecordScreenView(string screenName) {
    }

    public static void RecordEvent(string eventName) {
    }

    public static void RecordEvent(string eventName, Dictionary<string, object> properties) {
    }

    public static void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>>items) {
    }

    public static int EventGetFirstTime(string eventName) {
        return -1;
    }

    public static int EventGetLastTime(string eventName) {
        return -1;
    }

    public static int EventGetOccurrences(string eventName) {
        return -1;
    }

    public static JSONClass EventGetDetail(string eventName) {
        return new JSONClass();
    }

    public static JSONClass UserGetEventHistory() {
        return new JSONClass();
    }

    public static JSONClass SessionGetUTMDetails() {
        return new JSONClass();
    }

    public static int SessionGetTimeElapsed() {
        return -1;
    }

    public static int UserGetTotalVisits() {
        return -1;
    }

    public static int UserGetScreenCount() {
        return -1;
    }

    public static int UserGetPreviousVisitTime() {
        return -1;
    }

    public static void EnablePersonalization() {
    }

    public static void DisablePersonalization() {
    }

    public static void RegisterPush() {
    }

    public static void SetApplicationIconBadgeNumber(int num) {
    }

    public static void SetDebugLevel(int level) {
    }

    public static void SetLocation(double lat, double lon) {
    }

    public static void PushInstallReferrerSource(string source, string medium, string campaign) {
    }

    public static void EnableDeviceNetworkInfoReporting(bool value){
    }

    public static void SetOptOut(bool value){
    }

    public static void SetOffline(bool value){
    }

    public static void CreateNotificationChannel(string channelId,string channelName, string channelDescription, int importance, bool showBadge){
    }

    public static void CreateNotificationChannelWithSound(string channelId,string channelName, string channelDescription, int importance, bool showBadge, string sound){
    }

    public static void CreateNotificationChannelWithGroup(string channelId,string channelName, string channelDescription, int importance, string groupId, bool showBadge){
    }

    public static void CreateNotificationChannelWithGroupAndSound(string channelId,string channelName, string channelDescription, int importance, string groupId, bool showBadge, string sound){
    }

    public static void CreateNotificationChannelGroup(string groupId, string groupName){
    }

    public static void DeleteNotificationChannel(string channelId){
    }

    public static void DeleteNotificationChannelGroup(string groupId){
    }

    public static void InitializeInbox(){
    }

    public static void ShowAppInbox(string styleConfig){
    }

    public static void DismissAppInbox(){
    }

    public static int GetInboxMessageCount(){
        return -1;
    }

    public static int GetInboxMessageUnreadCount(){
        return -1;
    }

    public static void DeleteInboxMessagesForIDs(string[] messageIds) {
    }

    public static void DeleteInboxMessageForID(string messageId) {
    }

    public static void MarkReadInboxMessagesForIDs(string[] messageIds) {
    }

    public static void MarkReadInboxMessageForID(string messageId) {
    }

    public static void PromptPushPrimer(string json){
    }

    public static void PromptForPushPermission(bool showFallbackSettings){
    }

    public static void IsPushPermissionGranted(){
    }
#endif
    }
}