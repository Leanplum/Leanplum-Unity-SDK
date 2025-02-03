#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using CleverTapSDK.Common;
using CleverTapSDK.Constants;
using System.Collections.Generic;

namespace CleverTapSDK.Native {
    internal class UnityNativePlatformBinding : CleverTapPlatformBindings {
        private readonly UnityNativeWrapper _unityNativeWrapper;
       
        internal UnityNativePlatformBinding() {
            UnityNativeCallbackHandler handler = CreateGameObjectAndAttachCallbackHandler<UnityNativeCallbackHandler>(CleverTapGameObjectName.UNITY_NATIVE_CALLBACK_HANDLER);
            CallbackHandler = handler;
            _unityNativeWrapper = new UnityNativeWrapper(handler);
        }

        internal override void LaunchWithCredentials(string accountID, string token) {
            _unityNativeWrapper.LaunchWithCredentials(accountID, token);
        }

        internal override void LaunchWithCredentialsForRegion(string accountID, string token, string region) {
            _unityNativeWrapper.LaunchWithCredentials(accountID, token, region);
        }

        internal override void LaunchWithCredentialsForProxyServer(string accountID, string token, string proxyDomain, string spikyProxyDomain) {
            UnityEngine.Debug.LogWarning("LaunchWithCredentialsForProxyServer is not supported on Unity Native." +
                " Using LaunchWithCredentials instead.");
            _unityNativeWrapper.LaunchWithCredentials(accountID, token);
        }

        internal override void OnUserLogin(Dictionary<string, object> properties) {
            _unityNativeWrapper.OnUserLogin(properties);
        }

        internal override void ProfilePush(Dictionary<string, object> properties) {
            _unityNativeWrapper.ProfilePush(properties);
        }

        internal override void ProfileRemoveValueForKey(string key) {
            _unityNativeWrapper.ProfilePush(key, 1, UnityNativeConstants.Commands.COMMAND_DELETE);
        }

        internal override void RecordEvent(string eventName) {
            _unityNativeWrapper.RecordEvent(eventName);
        }

        internal override void RecordEvent(string eventName, Dictionary<string, object> properties) {
            _unityNativeWrapper.RecordEvent(eventName, properties);
        }

        internal override void RecordChargedEventWithDetailsAndItems(Dictionary<string, object> details, List<Dictionary<string, object>> items) {
            _unityNativeWrapper.RecordChargedEventWithDetailsAndItems(details, items);
        }
      
        internal override void ProfileAddMultiValuesForKey(string key, List<string> values) {
            _unityNativeWrapper.ProfilePush(key, values, UnityNativeConstants.Commands.COMMAND_ADD);
        }

        internal override void ProfileAddMultiValueForKey(string key, string val) {
            ProfileAddMultiValuesForKey(key, new List<string> { val });
        }

        internal override void ProfileSetMultiValuesForKey(string key, List<string> values) {
            _unityNativeWrapper.ProfilePush(key, values, UnityNativeConstants.Commands.COMMAND_SET);
        }

        internal override void ProfileRemoveMultiValuesForKey(string key, List<string> values) {
            _unityNativeWrapper.ProfilePush(key, values, UnityNativeConstants.Commands.COMMAND_REMOVE);
        }

        internal override void ProfileIncrementValueForKey(string key, double val) {
            _unityNativeWrapper.ProfilePush(key, val, UnityNativeConstants.Commands.COMMAND_INCREMENT);
        }

        internal override void ProfileIncrementValueForKey(string key, int val) {
            _unityNativeWrapper.ProfilePush(key, val, UnityNativeConstants.Commands.COMMAND_INCREMENT);
        }

        internal override void ProfileDecrementValueForKey(string key, double val) {
            _unityNativeWrapper.ProfilePush(key, val, UnityNativeConstants.Commands.COMMAND_DECREMENT);
        }

        internal override void ProfileDecrementValueForKey(string key, int val) {
            _unityNativeWrapper.ProfilePush(key, val, UnityNativeConstants.Commands.COMMAND_DECREMENT);
        }

        internal override string GetCleverTapID() {
            return _unityNativeWrapper.GetCleverTapID();
        }

        internal override string ProfileGetCleverTapID() {
            return GetCleverTapID();
        }

        internal override void EnableDeviceNetworkInfoReporting(bool enabled) {
            _unityNativeWrapper.EnableDeviceNetworkInfoReporting(enabled);
        }
    }
}
#endif
