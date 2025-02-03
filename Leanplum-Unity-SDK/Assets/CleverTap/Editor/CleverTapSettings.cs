using System.IO;
using UnityEngine;

namespace CleverTapSDK.Private
{
    [System.Serializable]
    public class CleverTapSettings : ScriptableObject
    {
        #region Project Settings
        /// <summary>
        /// CleverTap project Account Id (Project ID).
        /// </summary>
        public string CleverTapAccountId;

        /// <summary>
        /// CleverTap project Account token (Project Token).
        /// </summary>
        public string CleverTapAccountToken;

        /// <summary>
        /// CleverTap Region Code.
        /// </summary>
        public string CleverTapAccountRegion;

        /// <summary>
        /// Custom Proxy Domain.
        /// </summary>
        public string CleverTapProxyDomain;

        /// <summary>
        /// Spiky Proxy Domain.
        /// </summary>
        public string CleverTapSpikyProxyDomain;
        #endregion

        #region IOS Specific Settings
        /// <summary>
        /// Disable IDFV on iOS.
        /// </summary>
        public bool CleverTapDisableIDFV;

        /// <summary>
        /// Boolean whether to use auto integrate on iOS.
        /// Default value is true.
        /// </summary>
        public bool CleverTapIOSUseAutoIntegrate { get; set; } = true;

        /// <summary>
        /// Boolean whether to set UNUserNotificationCenter delegate on iOS.
        /// Default value is true.
        /// If set to false, you must implement the center delegate
        /// methods yourself and call the CleverTap methods.
        /// </summary>
        public bool CleverTapIOSUseUNUserNotificationCenter { get; set; } = true;

        /// <summary>
        /// Boolean whether to present remote notifications while app is on foreground on iOS.
        /// Default value is false. The UNNotificationPresentationOptionNone is used.
        /// If changed to true, notification will be presented using UNNotificationPresentationOptionBanner/Alert |
        /// UNNotificationPresentationOptionBadge | UNNotificationPresentationOptionSound.
        /// </summary>
        public bool CleverTapIOSPresentNotificationOnForeground;
        #endregion

        #region Other Settings
        /// <summary>
        /// Boolean whether the CleverTap settings should be saved to the streaming assets as JSON.
        /// Default value is false.
        /// Set this to true if you need to use the CleverTapSettings runtime.
        /// </summary>
        public bool CleverTapSettingsSaveToJSON;
        #endregion

        public override string ToString()
        {
            return $"CleverTapSettings:\n" +
                   $"CleverTapAccountId: {CleverTapAccountId}\n" +
                   $"CleverTapAccountToken: {CleverTapAccountToken}\n" +
                   $"CleverTapAccountRegion: {CleverTapAccountRegion}\n" +
                   $"CleverTapProxyDomain: {CleverTapProxyDomain}\n" +
                   $"CleverTapSpikyProxyDomain: {CleverTapSpikyProxyDomain}\n" +
                   $"CleverTapDisableIDFV: {CleverTapDisableIDFV}\n" +
                   $"CleverTapIOSUseAutoIntegrate: {CleverTapIOSUseAutoIntegrate}\n" +
                   $"CleverTapIOSUseUNUserNotificationCenter: {CleverTapIOSUseUNUserNotificationCenter}\n" +
                   $"CleverTapIOSPresentNotificationOnForeground: {CleverTapIOSPresentNotificationOnForeground}\n" +
                   $"CleverTapSettingsSaveToJSON: {CleverTapSettingsSaveToJSON}";
        }

        internal static readonly string settingsPath = Path.Combine("Assets", "CleverTapSettings.asset");
        internal static readonly string jsonPath = Path.Combine(Application.streamingAssetsPath, "CleverTapSettings.json");
    }
}