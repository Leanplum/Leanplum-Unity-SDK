#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
namespace CleverTapSDK.Native {
    internal class UnityNativeValidationResult {
        // TODO : Consider converting errorCode into appropriate enumration(Enum)
        
        private readonly int? _errorCode;
        private readonly string _errorMessage;

        internal UnityNativeValidationResult() {
            _errorCode = null;
            _errorMessage = null;
        }
        internal UnityNativeValidationResult( int errorCode, string errorMessage) { 
            _errorCode = errorCode;
            _errorMessage = errorMessage ?? string.Empty;
        }

        internal int? ErrorCode => _errorCode;
        internal string ErrorMessage => _errorMessage;
        internal bool IsSuccess => _errorCode == null;
    }
}
#endif