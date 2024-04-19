using UnityEditor;

namespace CleverTapSDK.Private
{
    // iOS Configuration settings class
    [System.Serializable]
    public class IOSConfigSettings
    {
        public string CleverTapAccountId;
        public string CleverTapAccountToken;
        public string CleverTapAccountRegion;
        public bool CleverTapEnablePersonalization;
        public bool CleverTapDisableIDFV;

        private static readonly string ACCOUNT_ID_KEY = "CleverTapAccountId";
        private static readonly string ACCOUNT_TOKEN_KEY = "CleverTapAccountToken";
        private static readonly string ACCOUNT_REGION_KEY = "CleverTapAccountRegion";
        private static readonly string ENABLE_PERSONALIATION_KEY = "CleverTapEnablePersonalization";
        private static readonly string DISABLE_IDFV_KEY = "CleverTapDisableIDFV";

        /// <summary>
        /// Loads the iOS Settings from EditorPrefs.
        /// </summary>
        /// <returns>Instance with the settings.</returns>
        public static IOSConfigSettings Load()
        {
            // Load settings from EditorPrefs
            return new IOSConfigSettings
            {
                CleverTapAccountId = EditorPrefs.GetString(ACCOUNT_ID_KEY, ""),
                CleverTapAccountToken = EditorPrefs.GetString(ACCOUNT_TOKEN_KEY, ""),
                CleverTapAccountRegion = EditorPrefs.GetString(ACCOUNT_REGION_KEY, ""),
                CleverTapEnablePersonalization = EditorPrefs.GetBool(ENABLE_PERSONALIATION_KEY, true),
                CleverTapDisableIDFV = EditorPrefs.GetBool(DISABLE_IDFV_KEY, false)
            };
        }

        /// <summary>
        /// Saves the settings to EditorPrefs.
        /// Beware that each settings instance writes to the same keys in EditorPrefs.
        /// </summary>
        internal void Save()
        {
            EditorPrefs.SetString(ACCOUNT_ID_KEY, CleverTapAccountId);
            EditorPrefs.SetString(ACCOUNT_TOKEN_KEY, CleverTapAccountToken);
            EditorPrefs.SetString(ACCOUNT_REGION_KEY, CleverTapAccountRegion);
            EditorPrefs.SetBool(ENABLE_PERSONALIATION_KEY, CleverTapEnablePersonalization);
            EditorPrefs.SetBool(DISABLE_IDFV_KEY, CleverTapDisableIDFV);
        }
    }
}