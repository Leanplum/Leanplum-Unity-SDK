#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System;
using UnityEngine;
using CleverTapSDK.Constants;

namespace CleverTapSDK.Native {

    internal class UnityNativeDeviceInfo {
        private readonly int _sdkVersion;
        private readonly string _appVersion;
        private readonly string _appBuild;
        private readonly string _osName;
        private readonly string _osVersion;
        private readonly string _manufacturer;
        private readonly string _model;
        private readonly string _carrier;
        private readonly string _countryCode;
        private readonly string _timeZone;
        private readonly string _radio;
        private readonly string _vendorIdentifier;
        private string _deviceWidth;
        private string _deviceHeight;
        private readonly string _library;
        private readonly int _wifi;
        private readonly string _locale;
        private readonly UnityNativePreferenceManager _preferenceManager;

        internal UnityNativeDeviceInfo(string accountId) {
            _preferenceManager = UnityNativePreferenceManager.GetPreferenceManager(accountId);
            _sdkVersion = CleverTapVersion.CLEVERTAP_SDK_REVISION; // Use the SDK Version Revision
            _appVersion = Application.version;
            _appBuild = Application.buildGUID;//Application.productName;
            _osName = GetOSName();
            _osVersion = SystemInfo.operatingSystem;
            _manufacturer = SystemInfo.deviceModel;
            _model = SystemInfo.deviceModel;
            _carrier = null;
            _countryCode = null;
            _timeZone = null;
            _radio = null;
            _vendorIdentifier = null;
            _deviceWidth = GetDeviceWidth();
            _deviceHeight = GetDeviceHeight();
            _library = null;
            _wifi = -1;
            _locale = null;
        }

        internal int SdkVersion => _sdkVersion;

        internal string AppVersion => _appVersion;

        internal string AppBuild => _appBuild;

        internal string OsName => _osName;

        internal string OsVersion => _osVersion;

        internal string Manufacturer => _manufacturer;

        internal string Model => _model;

        internal string Carrier => _carrier;

        internal string CountryCode => _countryCode;

        internal string TimeZone => _timeZone;

        internal string Radio => _radio;

        internal string VendorIdentifier => _vendorIdentifier;

        internal string DeviceWidth => _deviceWidth;

        internal string DeviceHeight => _deviceHeight;

        internal string DeviceId => GetDeviceId();

        internal string Library => _library;

        internal int Wifi => _wifi;

        internal string Locale => _locale;

        internal bool EnableNetworkInfoReporting {
            get
            {
                return _preferenceManager.GetInt(UnityNativeConstants.Profile.NETWORK_INFO_KEY, 0) == 1;
            }
            set
            {
                _preferenceManager.SetInt(UnityNativeConstants.Profile.NETWORK_INFO_KEY, value ? 1 : 0);
            }
        }

        private string GetOSName() {
#if UNITY_WEBGL
            return UnityNativeConstants.OSNames.OTHERS;
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            return UnityNativeConstants.OSNames.MACOS;
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            return UnityNativeConstants.OSNames.WINDOWS;
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            return UnityNativeConstants.OSNames.LINUX;
#elif UNITY_IOS || UNITY_TVOS
            return UnityNativeConstants.OSNames.IOS;
#elif UNITY_ANDROID
            return UnityNativeConstants.OSNames.ANDROID;
#else
            return UnityNativeConstants.OSNames.OTHERS;
#endif
        }

        private string GetDeviceId() {
            string id = _preferenceManager.GetString(UnityNativeConstants.SDK.DEVICE_ID_KEY, null);
            if (string.IsNullOrEmpty(id))
            {
                ForceNewDeviceID();
            }
            return _preferenceManager.GetString(UnityNativeConstants.SDK.DEVICE_ID_KEY, null);
        }

        internal void ForceNewDeviceID() {
            string newDeviceID = GenerateGuid();
            ForceUpdateDeviceId(newDeviceID);
        }

        internal void ForceUpdateDeviceId(string id) {
            _preferenceManager.SetString(UnityNativeConstants.SDK.DEVICE_ID_KEY, id);
        }

        private string GenerateGuid() {
            string id = Guid.NewGuid().ToString().Replace("-", "");
            return $"{UnityNativeConstants.SDK.UNITY_GUID_PREFIX}{id}";
        }

        public string GetDeviceWidth() {
            if (_deviceWidth == null) {
                float dpi = Screen.dpi;
                if (dpi == 0) {
                    dpi = GetDefaultDPI();
                }

                float widthInPixels = Screen.width;
                float widthInInches = widthInPixels / dpi;

                _deviceWidth = string.Format("{0:F2}", widthInInches);
            }

            return _deviceWidth;
        }

        public string GetDeviceHeight() {
            if (_deviceHeight == null) {
                float dpi = Screen.dpi;
                if (dpi == 0) {
                    dpi = GetDefaultDPI();
                }

                float heightInPixels = Screen.height;
                float heightInInches = heightInPixels / dpi;

                _deviceHeight = string.Format("{0:F2}", heightInInches);
            }

            return _deviceHeight;
        }

        private float GetDefaultDPI() {
            switch (Application.platform) {
                case RuntimePlatform.IPhonePlayer:
                    if (SystemInfo.deviceModel.Contains("iPad"))
                        return 132f;
                    else
                        return 163f;

                case RuntimePlatform.Android:
                    return 160f;

                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    return GetMacOSDPI();

                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WebGLPlayer:
                case RuntimePlatform.LinuxPlayer:
                    return 96f;

                default:
                    return 96f;
            }
        }

        private float GetMacOSDPI() {
            if (SystemInfo.deviceModel.Contains("MacBookPro") ||
                SystemInfo.deviceModel.Contains("iMac") ||
                SystemInfo.deviceModel.Contains("MacBookAir"))
            {
                if (SystemInfo.graphicsDeviceName.Contains("Retina"))
                    return 144f;
            }
            return 110f;
        }
    }
}
#endif