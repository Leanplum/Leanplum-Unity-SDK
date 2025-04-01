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
            _unityNativeEventManager = new UnityNativeEventManager(handler, platformVariable);
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
    }
}
#endif
