#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;

namespace CleverTapSDK.Native {
    internal class UnityNativePushEventResult {
        private readonly Dictionary<string, object> _systemFields;
        private readonly Dictionary<string, object> _customFields;

        public UnityNativePushEventResult(Dictionary<string, object> systemFields, Dictionary<string, object> customFields) { 
            _systemFields = systemFields;
            _customFields = customFields;
        }

        public Dictionary<string, object> SystemFields => _systemFields;
        public Dictionary<string, object> CustomFields => _customFields;
    }
}
#endif