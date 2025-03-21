#if UNITY_IOS
using System.Runtime.InteropServices;
using static CleverTapSDK.IOS.IOSCallbackHandler;

namespace CleverTapSDK.IOS {
    internal static class IOSDllImport {

        #region Bindings

        [DllImport("__Internal")]
        internal static extern void CleverTap_onPlatformInit();

        [DllImport("__Internal")]
        internal static extern void CleverTap_onCallbackAdded(string name);

        [DllImport("__Internal")]
        internal static extern void CleverTap_onVariablesCallbackAdded(string name, int callbackId);

        [DllImport("__Internal")]
        internal static extern void CleverTap_setInAppNotificationButtonTappedCallback(InAppNotificationButtonTapped callback);

        [DllImport("__Internal")]
        internal static extern void CleverTap_onUserLogin(string properties);

        [DllImport("__Internal")]
        internal static extern void CleverTap_profilePush(string properties);

        [DllImport("__Internal")]
        internal static extern string CleverTap_profileGet(string key);

        [DllImport("__Internal")]
        internal static extern string CleverTap_profileGetCleverTapAttributionIdentifier();

        [DllImport("__Internal")]
        internal static extern string CleverTap_profileGetCleverTapID();

        [DllImport("__Internal")]
        internal static extern void CleverTap_profileRemoveValueForKey(string key);

        [DllImport("__Internal")]
        internal static extern void CleverTap_profileSetMultiValuesForKey(string key, string[] array, int size);

        [DllImport("__Internal")]
        internal static extern void CleverTap_profileAddMultiValuesForKey(string key, string[] array, int size);

        [DllImport("__Internal")]
        internal static extern void CleverTap_profileRemoveMultiValuesForKey(string key, string[] array, int size);

        [DllImport("__Internal")]
        internal static extern void CleverTap_profileAddMultiValueForKey(string key, string val);

        [DllImport("__Internal")]
        internal static extern void CleverTap_profileRemoveMultiValueForKey(string key, string val);

        [DllImport("__Internal")]
        internal static extern void CleverTap_profileIncrementDoubleValueForKey(string key, double val);

        [DllImport("__Internal")]
        internal static extern void CleverTap_profileIncrementIntValueForKey(string key, int val);

        [DllImport("__Internal")]
        internal static extern void CleverTap_profileDecrementDoubleValueForKey(string key, double val);

        [DllImport("__Internal")]
        internal static extern void CleverTap_profileDecrementIntValueForKey(string key, int val);

        [DllImport("__Internal")]
        internal static extern void CleverTap_suspendInAppNotifications();

        [DllImport("__Internal")]
        internal static extern void CleverTap_discardInAppNotifications();

        [DllImport("__Internal")]
        internal static extern void CleverTap_resumeInAppNotifications();

        [DllImport("__Internal")]
        internal static extern string CleverTap_getCleverTapID();

        [DllImport("__Internal")]
        internal static extern void CleverTap_recordScreenView(string screenName);

        [DllImport("__Internal")]
        internal static extern void CleverTap_recordEvent(string eventName, string properties);

        [DllImport("__Internal")]
        internal static extern void CleverTap_recordChargedEventWithDetailsAndItems(string details, string items);

        [DllImport("__Internal")]
        internal static extern void CleverTap_setOffline(bool enabled);

        [DllImport("__Internal")]
        internal static extern void CleverTap_setOptOut(bool enabled);

        [DllImport("__Internal")]
        internal static extern void CleverTap_enableDeviceNetworkInfoReporting(bool enabled);

        [DllImport("__Internal")]
        internal static extern void CleverTap_registerPush();

        [DllImport("__Internal")]
        internal static extern void CleverTap_setApplicationIconBadgeNumber(int num);

        [DllImport("__Internal")]
        internal static extern void CleverTap_setDebugLevel(int level);

        [DllImport("__Internal")]
        internal static extern void CleverTap_enablePersonalization();

        [DllImport("__Internal")]
        internal static extern void CleverTap_disablePersonalization();

        [DllImport("__Internal")]
        internal static extern void CleverTap_setLocation(double lat, double lon);

        [DllImport("__Internal")]
        internal static extern int CleverTap_eventGetFirstTime(string eventName);

        [DllImport("__Internal")]
        internal static extern int CleverTap_eventGetLastTime(string eventName);

        [DllImport("__Internal")]
        internal static extern int CleverTap_eventGetOccurrences(string eventName);

        [DllImport("__Internal")]
        internal static extern string CleverTap_userGetEventHistory();

        [DllImport("__Internal")]
        internal static extern string CleverTap_sessionGetUTMDetails();

        [DllImport("__Internal")]
        internal static extern int CleverTap_sessionGetTimeElapsed();

        [DllImport("__Internal")]
        internal static extern string CleverTap_eventGetDetail(string eventName);

        [DllImport("__Internal")]
        internal static extern int CleverTap_userGetTotalVisits();

        [DllImport("__Internal")]
        internal static extern int CleverTap_userGetScreenCount();

        [DllImport("__Internal")]
        internal static extern int CleverTap_userGetPreviousVisitTime();

        [DllImport("__Internal")]
        internal static extern void CleverTap_pushInstallReferrerSource(string source, string medium, string campaign);

        [DllImport("__Internal")]
        internal static extern void CleverTap_showAppInbox(string styleConfig);

        [DllImport("__Internal")]
        internal static extern void CleverTap_dismissAppInbox();

        [DllImport("__Internal")]
        internal static extern int CleverTap_getInboxMessageCount();

        [DllImport("__Internal")]
        internal static extern int CleverTap_getInboxMessageUnreadCount();

        [DllImport("__Internal")]
        internal static extern int CleverTap_initializeInbox();

        [DllImport("__Internal")]
        internal static extern string CleverTap_getAllInboxMessages();

        [DllImport("__Internal")]
        internal static extern string CleverTap_getUnreadInboxMessages();

        [DllImport("__Internal")]
        internal static extern string CleverTap_getInboxMessageForId(string messageId);

        [DllImport("__Internal")]
        internal static extern void CleverTap_deleteInboxMessageForID(string messageId);

        [DllImport("__Internal")]
        internal static extern void CleverTap_deleteInboxMessagesForIDs(string[] messageIds, int arrLength);

        [DllImport("__Internal")]
        internal static extern void CleverTap_markReadInboxMessageForID(string messageId);

        [DllImport("__Internal")]
        internal static extern void CleverTap_markReadInboxMessagesForIDs(string[] messageIds, int arrLength);

        [DllImport("__Internal")]
        internal static extern void CleverTap_recordInboxNotificationViewedEventForID(string messageId);

        [DllImport("__Internal")]
        internal static extern void CleverTap_recordInboxNotificationClickedEventForID(string messageId);

        [DllImport("__Internal")]
        internal static extern string CleverTap_getAllDisplayUnits();

        [DllImport("__Internal")]
        internal static extern string CleverTap_getDisplayUnitForID(string unitID);

        [DllImport("__Internal")]
        internal static extern void CleverTap_recordDisplayUnitViewedEventForID(string unitID);

        [DllImport("__Internal")]
        internal static extern void CleverTap_recordDisplayUnitClickedEventForID(string unitID);

        [DllImport("__Internal")]
        internal static extern void CleverTap_fetchProductConfig();

        [DllImport("__Internal")]
        internal static extern void CleverTap_fetchProductConfigWithMinimumInterval(double minimumInterval);

        [DllImport("__Internal")]
        internal static extern void CleverTap_setProductConfigMinimumFetchInterval(double minimumFetchInterval);

        [DllImport("__Internal")]
        internal static extern void CleverTap_activateProductConfig();

        [DllImport("__Internal")]
        internal static extern void CleverTap_fetchAndActivateProductConfig();

        [DllImport("__Internal")]
        internal static extern void CleverTap_setProductConfigDefaults(string defaults);

        [DllImport("__Internal")]
        internal static extern void CleverTap_setProductConfigDefaultsFromPlistFileName(string fileName);

        [DllImport("__Internal")]
        internal static extern string CleverTap_getProductConfigValueFor(string key);

        [DllImport("__Internal")]
        internal static extern double CleverTap_getProductConfigLastFetchTimeStamp();

        [DllImport("__Internal")]
        internal static extern void CleverTap_resetProductConfig();

        [DllImport("__Internal")]
        internal static extern bool CleverTap_getFeatureFlag(string key, bool defaultValue);

        [DllImport("__Internal")]
        internal static extern void CleverTap_promptForPushPermission(bool showFallbackSettings);

        [DllImport("__Internal")]
        internal static extern void CleverTap_promptPushPrimer(string json);

        [DllImport("__Internal")]
        internal static extern void CleverTap_isPushPermissionGranted();

        [DllImport("__Internal")]
        internal static extern long CleverTap_getUserLastVisitTs();

        [DllImport("__Internal")]
        internal static extern void CleverTap_getUserEventLog(string eventName, string key, UserEventLogCallback callback);

        [DllImport("__Internal")]
        internal static extern void CleverTap_getUserAppLaunchCount(string key, UserEventLogCallback callback);

        [DllImport("__Internal")]
        internal static extern void CleverTap_getUserEventLogCount(string eventName, string key, UserEventLogCallback callback);

        [DllImport("__Internal")]
        internal static extern void CleverTap_getUserEventLogHistory(string key, UserEventLogCallback callback);

        #endregion

        #region Variables

        [DllImport("__Internal")]
        internal static extern string CleverTap_getVariableValue(string name);

        [DllImport("__Internal")]
        internal static extern string CleverTap_getFileVariableValue(string name);

        [DllImport("__Internal")]
        internal static extern void CleverTap_defineVar(string name, string kind, string jsonValue);

        [DllImport("__Internal")]
        internal static extern void CleverTap_defineFileVar(string name);

        [DllImport("__Internal")]
        internal static extern void CleverTap_syncVariables();

        [DllImport("__Internal")]
        internal static extern void CleverTap_syncVariablesProduction(bool isProduction);

        [DllImport("__Internal")]
        internal static extern void CleverTap_fetchVariables(int callbackId);

        #endregion

        #region InApps

        [DllImport("__Internal")]
        internal static extern void CleverTap_fetchInApps(int callbackId);

        [DllImport("__Internal")]
        internal static extern void CleverTap_clearInAppResources(bool expiredOnly);

        #endregion

        #region CustomTemplates

        [DllImport("__Internal")]
        internal static extern void CleverTap_customTemplateSetPresented(string templateName);

        [DllImport("__Internal")]
        internal static extern void CleverTap_customTemplateSetDismissed(string templateName);

        [DllImport("__Internal")]
        internal static extern string CleverTap_customTemplateContextToString(string templateName);

        [DllImport("__Internal")]
        internal static extern void CleverTap_customTemplateTriggerAction(string templateName, string argumentName);

        [DllImport("__Internal")]
        internal static extern string CleverTap_customTemplateGetStringArg(string templateName, string argumentName);

        [DllImport("__Internal")]
        internal static extern bool? CleverTap_customTemplateGetBooleanArg(string templateName, string argumentName);

        [DllImport("__Internal")]
        internal static extern string CleverTap_customTemplateGetFileArg(string templateName, string argumentName);

        [DllImport("__Internal")]
        internal static extern string CleverTap_customTemplateGetDictionaryArg(string templateName, string argumentName);

        [DllImport("__Internal")]
        internal static extern int CleverTap_customTemplateGetIntArg(string templateName, string argumentName);

        [DllImport("__Internal")]
        internal static extern double CleverTap_customTemplateGetDoubleArg(string templateName, string argumentName);

        [DllImport("__Internal")]
        internal static extern float CleverTap_customTemplateGetFloatArg(string templateName, string argumentName);

        [DllImport("__Internal")]
        internal static extern long CleverTap_customTemplateGetLongArg(string templateName, string argumentName);

        [DllImport("__Internal")]
        internal static extern short CleverTap_customTemplateGetShortArg(string templateName, string argumentName);

        [DllImport("__Internal")]
        internal static extern byte CleverTap_customTemplateGetByteArg(string templateName, string argumentName);

        [DllImport("__Internal")]
        internal static extern byte CleverTap_syncCustomTemplates(bool isProduction);

        #endregion
    }
}
#endif