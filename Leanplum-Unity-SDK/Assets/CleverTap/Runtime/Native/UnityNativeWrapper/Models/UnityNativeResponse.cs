#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;
using System.Net;

namespace CleverTapSDK.Native {
    internal class UnityNativeResponse {
        private HttpStatusCode _statusCode;
        private Dictionary<string, string> _headers;
        private string _content;
        private string _errorMessage;

        internal UnityNativeResponse(HttpStatusCode statusCode, Dictionary<string, string> headers, string content, string errorMessage = null) {
            _statusCode = statusCode;
            _headers = headers;
            _content = content;
            _errorMessage = errorMessage;
        }

        internal HttpStatusCode StatusCode => _statusCode;
        internal Dictionary<string, string> Headers => _headers;
        internal string Content => _content;
        internal string ErrorMessage => _errorMessage;

        internal bool IsSuccess()
        {
            return _statusCode >= HttpStatusCode.OK && _statusCode <= HttpStatusCode.Accepted;
        }
    }
}
#endif