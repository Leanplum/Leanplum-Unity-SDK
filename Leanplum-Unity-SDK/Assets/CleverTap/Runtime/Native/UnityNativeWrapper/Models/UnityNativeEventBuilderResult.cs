#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;

namespace CleverTapSDK.Native {
    internal class UnityNativeEventBuilderResult<T> {
        private List<UnityNativeValidationResult> _validationResults;
        private T _eventResult;

        public UnityNativeEventBuilderResult(List<UnityNativeValidationResult> validationResults, T eventResult) {
            _validationResults = validationResults;
            _eventResult = eventResult;
        }

        public List<UnityNativeValidationResult> ValidationResults => _validationResults;
        public T EventResult => _eventResult;
    }
}
#endif