#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;
using CleverTapSDK.Common;
using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native
{
    internal class UnityNativePlatformBinding : CleverTapPlatformBindings
    {
        internal UnityNativeEventManager UnityNativeEventManager => _unityNativeEventManager;

        private readonly UnityNativeEventManager _unityNativeEventManager;

        internal UnityNativePlatformBinding()
        {
            UnityNativeCallbackHandler handler = CreateGameObjectAndAttachCallbackHandler<UnityNativeCallbackHandler>(CleverTapGameObjectName.UNITY_NATIVE_CALLBACK_HANDLER);

            var platformVariable = VariableFactory.CleverTapVariable as UnityNativePlatformVariable;
            if (platformVariable == null)
                CleverTapLogger.LogError("CleverTapVariable must be UnityNativePlatformVariable.");

            handler.platformVariable = platformVariable;

            CallbackHandler = handler;

            var platformCustomTemplates = CustomTemplatesFactory.CleverTapCustomTemplates as UnityNativePlatformCustomTemplates;
            if (platformCustomTemplates == null)
                CleverTapLogger.LogError("CleverTapCustomTemplates must be UnityNativePlatformCustomTemplates.");

            _unityNativeEventManager = new UnityNativeEventManager(handler, platformVariable, platformCustomTemplates);
        }

        internal override void LaunchWithCredentials(string accountID, string token)
        {
            _unityNativeEventManager.LaunchWithCredentials(accountID, token);
        }

        internal override void LaunchWithCredentialsForRegion(string accountID, string token, string region)
        {
            _unityNativeEventManager.LaunchWithCredentials(accountID, token, region);
        }

        internal override void LaunchWithCredentialsForProxyServer(string accountID, string token, string proxyDomain, string spikyProxyDomain)
        {
            UnityEngine.Debug.LogWarning("LaunchWithCredentialsForProxyServer is not supported on Unity Native." +
                " Using LaunchWithCredentials instead.");
            _unityNativeEventManager.LaunchWithCredentials(accountID, token);
        }

        internal override void OnUserLogin(Dictionary<string, object> properties)
        {
            _unityNativeEventManager.OnUserLogin(properties);
        }

        internal override void ProfilePush(Dictionary<string, object> properties)
        {
            _unityNativeEventManager.ProfilePush(properties);
        }

        internal override void ProfileRemoveValueForKey(string key)
        {
            _unityNativeEventManager.ProfilePush(key, 1, UnityNativeConstants.Commands.COMMAND_DELETE);
        }

        internal override void RecordEvent(string eventName)
        {
            _unityNativeEventManager.RecordEvent(eventName);
        }

        internal override void RecordEvent(string eventName, Dictionary<string, object> properties)
        {
            _unityNativeEventManager.RecordEvent(eventName, properties);
        }

        internal override void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>> items)
        {
            _unityNativeEventManager.RecordChargedEventWithDetailsAndItems(details, items);
        }

        internal override void ProfileAddMultiValuesForKey(string key, List<string> values)
        {
            _unityNativeEventManager.ProfilePush(key, values, UnityNativeConstants.Commands.COMMAND_ADD);
        }

        internal override void ProfileAddMultiValueForKey(string key, string val)
        {
            ProfileAddMultiValuesForKey(key, new List<string> { val });
        }

        internal override void ProfileSetMultiValuesForKey(string key, List<string> values)
        {
            _unityNativeEventManager.ProfilePush(key, values, UnityNativeConstants.Commands.COMMAND_SET);
        }

        internal override void ProfileRemoveMultiValuesForKey(string key, List<string> values)
        {
            _unityNativeEventManager.ProfilePush(key, values, UnityNativeConstants.Commands.COMMAND_REMOVE);
        }

        internal override void ProfileIncrementValueForKey(string key, double val)
        {
            _unityNativeEventManager.ProfilePush(key, val, UnityNativeConstants.Commands.COMMAND_INCREMENT);
        }

        internal override void ProfileIncrementValueForKey(string key, int val)
        {
            _unityNativeEventManager.ProfilePush(key, val, UnityNativeConstants.Commands.COMMAND_INCREMENT);
        }

        internal override void ProfileDecrementValueForKey(string key, double val)
        {
            _unityNativeEventManager.ProfilePush(key, val, UnityNativeConstants.Commands.COMMAND_DECREMENT);
        }

        internal override void ProfileDecrementValueForKey(string key, int val)
        {
            _unityNativeEventManager.ProfilePush(key, val, UnityNativeConstants.Commands.COMMAND_DECREMENT);
        }

        internal override string GetCleverTapID()
        {
            return _unityNativeEventManager.GetCleverTapID();
        }

        internal override string ProfileGetCleverTapID()
        {
            return GetCleverTapID();
        }

        internal override void EnableDeviceNetworkInfoReporting(bool enabled)
        {
            _unityNativeEventManager.EnableDeviceNetworkInfoReporting(enabled);
        }

        internal override void InitializeInbox()
        {
            _unityNativeEventManager.InitializeInbox();     
        }

        internal override JSONArray GetAllInboxMessages()
        {
            return _unityNativeEventManager.GetAllInboxMessages();
        }

        internal override List<CleverTapInboxMessage> GetAllInboxMessagesParsed()
        {
            return _unityNativeEventManager.GetAllInboxMessagesParsed();
        }

        internal override JSONArray GetUnreadInboxMessages()
        {
            return _unityNativeEventManager.GetUnreadInboxMessages();
        }

        internal override List<CleverTapInboxMessage> GetUnreadInboxMessagesParsed()
        {
            return _unityNativeEventManager.GetUnreadInboxMessagesParsed();
        }

        internal override JSONClass GetInboxMessageForId(string messageId)
        {
            return _unityNativeEventManager.GetInboxMessageForId(messageId);
        }

        internal override CleverTapInboxMessage GetInboxMessageForIdParsed(string messageId)
        {
            return _unityNativeEventManager.GetInboxMessageForIdParsed(messageId);
        }

        internal override int GetInboxMessageCount()
        {
            return _unityNativeEventManager.GetInboxMessageCount();
        }

        internal override int GetInboxMessageUnreadCount()
        {
            return _unityNativeEventManager.GetInboxMessageUnreadCount();
        }

        internal override void MarkReadInboxMessageForID(string messageId)
        {
            _unityNativeEventManager.MarkReadInboxMessageForID(messageId);
        }

        internal override void MarkReadInboxMessagesForIDs(string[] messageIds)
        {
           _unityNativeEventManager.MarkReadInboxMessagesForIDs(messageIds);
        }

        internal override void DeleteInboxMessageForID(string messageId)
        {
           _unityNativeEventManager.DeleteInboxMessageForID(messageId);
        }

        internal override void DeleteInboxMessagesForIDs(string[] messageIds)
        {
            _unityNativeEventManager.DeleteInboxMessagesForIDs(messageIds);
        }

        internal override void RecordInboxNotificationClickedEventForID(string messageId)
        {
            _unityNativeEventManager.RecordInboxNotificationClickedEventForID(messageId);
        }

        internal override void RecordInboxNotificationViewedEventForID(string messageId)
        {
            _unityNativeEventManager.RecordInboxNotificationViewedEventForID(messageId);
        }
    }
}
#endif