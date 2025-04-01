#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using CleverTapSDK.Utilities;
using UnityEngine.Networking;

namespace CleverTapSDK.Native
{
    internal class UnityNativeRequest {
        private readonly string _path;
        private readonly string _method;

        private int? _timeout;
        private IReadOnlyList<KeyValuePair<string, string>> _queryParameters;
        private string _requestBody;
        private IReadOnlyDictionary<string, string> _headers;
        private KeyValuePair<string, string>? _authorization;
        private IReadOnlyList<IUnityNativeRequestInterceptor> _requestInterceptors;
        private IReadOnlyList<IUnityNativeResponseInterceptor> _responseInterceptors;
        private IReadOnlyDictionary<string, string> _additionalProperties;

        internal UnityNativeRequest(string path, string method, Dictionary<string, string> additionalProperties = null) {
            _path = path;
            _method = method?.ToUpper();
            _additionalProperties = additionalProperties;
        }

        internal int? Timeout => _timeout;
        internal string Path => _path;
        internal IReadOnlyList<KeyValuePair<string, string>> QueryParameters => _queryParameters;
        internal string RequestBody => _requestBody;
        internal IReadOnlyDictionary<string, string> Headers => _headers;
        internal KeyValuePair<string, string>? Authorization => _authorization;
        internal IReadOnlyList<IUnityNativeRequestInterceptor> RequestInterceptors => _requestInterceptors;
        internal IReadOnlyList<IUnityNativeResponseInterceptor> ResponseInterceptors => _responseInterceptors;
        internal IReadOnlyDictionary<string, string> AdditionalProperties => _additionalProperties;

        internal UnityNativeRequest SetTimeout(int? timeout) {
            _timeout = timeout;
            return this;
        }

        internal UnityNativeRequest SetQueryParameters(List<KeyValuePair<string, string>> parameters) {
            _queryParameters = parameters;
            return this;
        }

        internal UnityNativeRequest SetRequestBody(string requestBody) {
            _requestBody = requestBody;
            return this;
        }

        internal UnityNativeRequest SetHeaders(Dictionary<string, string> headers) {
            _headers = headers;
            return this;
        }

        internal UnityNativeRequest SetAuthorization(KeyValuePair<string, string>? authorization) {
            _authorization = authorization;
            return this;
        }

        internal UnityNativeRequest SetRequestInterceptors(List<IUnityNativeRequestInterceptor> requestInterceptors) {
            _requestInterceptors = requestInterceptors;
            return this;
        }

        internal UnityNativeRequest SetResponseInterceptors(List<IUnityNativeResponseInterceptor> responseInterceptors) {
            _responseInterceptors = responseInterceptors;
            return this;
        }

        internal UnityWebRequest BuildRequest(string baseURI) {
            Uri uri = BuildURI(baseURI);
            UnityWebRequest request;
            if (_method == UnityNativeConstants.Network.REQUEST_GET) {
                request = UnityWebRequest.Get(uri);
            }
            else if (_method == UnityNativeConstants.Network.REQUEST_POST) {
#if UNITY_2022_1_OR_NEWER
                // Use the new overload available in Unity 2022+
                request = UnityWebRequest.Post(uri, _requestBody, "application/json");
#else
                // Unity 2021 and older: Use UploadHandlerRaw for JSON requests
                byte[] bodyRaw = Encoding.UTF8.GetBytes(_requestBody);
                request = new UnityWebRequest(uri, "POST");
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
#endif
            }
            else {
                throw new NotSupportedException("Http method is not supported");
            }

            if (_headers?.Count > 0) {
                foreach (var header in _headers) {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            if (_authorization.HasValue) {
                request.SetRequestHeader(_authorization.Value.Key, _authorization.Value.Value);
            }

            if (_timeout != null) {
                request.timeout = _timeout.Value;
            }
            else
            {
                request.timeout = UnityNativeConstants.Network.DEFAUL_REQUEST_TIMEOUT_SEC;
            }

            CleverTapLogger.Log($"Build Request: {uri}, with headers: {Json.Serialize(_headers)}, body: {_requestBody}, " +
                $"and query parameters: [{Json.Serialize(_queryParameters)}]");

            return request;
        }

        private Uri BuildURI(string baseURI) {
            var uriString = baseURI +"/"+ _path;

            if (_queryParameters?.Count > 0) {
                uriString += "?";
                for (int i = 0; i < _queryParameters.Count; i++) {
                    uriString += $"{_queryParameters[i].Key}={_queryParameters[i].Value}";
                    if (i != _queryParameters.Count - 1) {
                        uriString += "&";
                    }
                }
            }

            return new Uri(uriString);
        }
    }
}
#endif