#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR

namespace CleverTapSDK.Native
{
    internal class UnityNativeCoreState
    {
        private UnityNativeAccountInfo _accountInfo;
        private UnityNativeDeviceInfo _deviceInfo;
        private UnityNativeSessionManager _sessionManager;

        internal UnityNativeCoreState(UnityNativeAccountInfo accountInfo)
        {
            _accountInfo = accountInfo;
            _deviceInfo = new UnityNativeDeviceInfo(_accountInfo.AccountId);
            _sessionManager = new UnityNativeSessionManager(_accountInfo.AccountId);
        }

        internal UnityNativeDeviceInfo DeviceInfo => _deviceInfo;
        internal UnityNativeAccountInfo AccountInfo => _accountInfo;
        internal UnityNativeSessionManager SessionManager => _sessionManager;
    }
}
#endif