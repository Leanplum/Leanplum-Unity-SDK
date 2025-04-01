#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using CleverTapSDK.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace CleverTapSDK.Native {
    internal class UnityNativeNetworkEngine : MonoBehaviour {
        private SynchronizationContext _context;

        private void Awake()
        {
            _context = SynchronizationContext.Current;
        }

        public Task RunOnMainThread(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();
            _context.Post(_ => {
                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, null);
            return tcs.Task;
        }

        public Task<T> RunOnMainThread<T>(Func<T> function)
        {
            var tcs = new TaskCompletionSource<T>();
            _context.Post(_ => {
                try
                {
                    var result = function();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, null);

            return tcs.Task;
        }

        public Task<T> RunOnMainThread<T>(Func<Task<T>> function)
        {
            var tcs = new TaskCompletionSource<T>();
            _context.Post(async _ => {
                try
                {
                    var result = await function();
                    tcs.SetResult(result);

                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, null);
            return tcs.Task;
        }
        
        private string _baseURI;
        private string _region;
        private bool _mute;
        private int? _timeout;
        private IReadOnlyDictionary<string, string> _headers;
        private KeyValuePair<string, string>? _authorization;
        private IReadOnlyList<IUnityNativeRequestInterceptor> _requestInterceptors;
        private IReadOnlyList<IUnityNativeResponseInterceptor> _responseInterceptors;

        private int responseFailureCount;

        private UnityNativePreferenceManager _preferenceManager;

        private UnityNativeNetworkEngine() {
        }

        internal static UnityNativeNetworkEngine Create(string accountId) {
            var gameObject = new GameObject($"{accountId}_UnityNativeNetworkEngine");
            gameObject.AddComponent<UnityNativeNetworkEngine>();
            DontDestroyOnLoad(gameObject);

            UnityNativeNetworkEngine component = gameObject.GetComponent<UnityNativeNetworkEngine>();
            component._preferenceManager = UnityNativePreferenceManager.GetPreferenceManager(accountId);
            return gameObject.GetComponent<UnityNativeNetworkEngine>();
        }

        internal UnityNativeNetworkEngine SetBaseURI(string baseUri) {
            _baseURI = "https://" + baseUri;
            return this;
        }

        internal UnityNativeNetworkEngine SetRegion(string region)
        {
            _region = region;
            return this;
        }

        internal UnityNativeNetworkEngine SetTimeout(int? timeout) {
            _timeout = timeout;
            return this;
        }

        internal UnityNativeNetworkEngine SetMute(bool mute)
        {
            _mute = mute;
            return this;
        }

        internal bool IsMuted() { return _mute; }

        internal UnityNativeNetworkEngine SetHeaders(Dictionary<string, string> headers) {
            _headers = headers;
            return this;
        }

        internal UnityNativeNetworkEngine SetAuthorization(KeyValuePair<string, string>? authorization) {
            _authorization = authorization;
            return this;
        }

        internal UnityNativeNetworkEngine SetRequestInterceptors(List<IUnityNativeRequestInterceptor> requestInterceptors) {
            _requestInterceptors = requestInterceptors;
            return this;
        }

        internal UnityNativeNetworkEngine SetResponseInterceptors(List<IUnityNativeResponseInterceptor> responseInterceptors) {
            _responseInterceptors = responseInterceptors;
            return this;
        }

        internal bool IsNetworkReachable => 
            Application.internetReachability != NetworkReachability.NotReachable;


        internal bool NeedHandshakeForDomain()
        {
            bool needHandshakeDueToFailure = responseFailureCount > 5;

            if (needHandshakeDueToFailure)
            {
                SetBaseURI(null);
                SetRedirectDomain(null);
            }

            if (!string.IsNullOrEmpty(_region))
            {
                string region = _region.Trim().ToLower();
                string uri = $"{region}.{UnityNativeConstants.Network.CT_BASE_URL}";
                CleverTapLogger.Log($"Setting domain name with region: {uri}");
                SetBaseURI(uri);
                return false;
            }

            string domain = GetRedirectDomain();
            if (!string.IsNullOrEmpty(domain))
            {
                SetBaseURI(domain);
            }

            return _baseURI == null || needHandshakeDueToFailure; 
        }

        internal async Task<bool> InitHandShake()
        {
            SetBaseURI(UnityNativeConstants.Network.CT_BASE_URL);
            var request = new UnityNativeRequest(UnityNativeConstants.Network.REQUEST_PATH_HAND_SHAKE, UnityNativeConstants.Network.REQUEST_GET);
            var response = await ExecuteRequest(request);

            return response.IsSuccess();
        }

        internal bool ProcessIncomingHeaders(UnityNativeResponse response) {
            if (response != null && response.Headers != null)
            {
                if (response.Headers.ContainsKey(UnityNativeConstants.Network.HEADER_DOMAIN_MUTE))
                {
                    _mute = bool.Parse(response.Headers[UnityNativeConstants.Network.HEADER_DOMAIN_MUTE]);
                    if (_mute)
                        return false;
                }

                if (response.Headers.ContainsKey(UnityNativeConstants.Network.HEADER_DOMAIN_NAME))
                {
                    string newDomain = response.Headers[UnityNativeConstants.Network.HEADER_DOMAIN_NAME];
                    if (!string.IsNullOrEmpty(newDomain))
                    {
                        CleverTapLogger.Log($"Setting new domain name: {newDomain}");
                        SetBaseURI(newDomain);
                        SetRedirectDomain(newDomain);
                    }
                }
            }

            return true;
        }

        internal void SetRedirectDomain(string newDomain) {
            if (newDomain == null)
            {
                _preferenceManager.DeleteKey(UnityNativeConstants.Network.REDIRECT_DOMAIN_KEY);
                return;
            }
            _preferenceManager.SetString(UnityNativeConstants.Network.REDIRECT_DOMAIN_KEY, newDomain);
        }

        internal string GetRedirectDomain() {
            return _preferenceManager.GetString(UnityNativeConstants.Network.REDIRECT_DOMAIN_KEY, null);
        }

        internal async Task<UnityNativeResponse> ExecuteRequest(UnityNativeRequest request) {
            return await RunOnMainThread(async () =>
            {
                if (request == null)
                {
                    return null;
                }

                if (NeedHandshakeForDomain())
                {
                    bool success = await InitHandShake();
                    if (success)
                    {
                        return await ExecuteRequestAfterHandshake(request);
                    }
                    else
                    {
                        return new UnityNativeResponse(HttpStatusCode.InternalServerError, null, null, "Internet connection is not reachable");
                    }
                }
                else
                {
                    return await ExecuteRequestAfterHandshake(request);
                }
            });
        }

        private async Task<UnityNativeResponse> ExecuteRequestAfterHandshake(UnityNativeRequest request) {
            if (request == null)
            {
                return null;
            }

            ApplyNetworkEngineRequestConfiguration(request);

            // Intercept request before sending
            if (request.RequestInterceptors?.Count > 0)
            {
                foreach (var requestInterceptors in request.RequestInterceptors)
                {
                    request = requestInterceptors.Intercept(request);
                }
            }

            var response = await SendRequest(request);
            CleverTapLogger.Log($"Response: content: {response.Content} headers: {Json.Serialize(response.Headers)}");

            if (response.IsSuccess())
            {
                ProcessIncomingHeaders(response);
            }

            // Intercept response before returning
            if (request.ResponseInterceptors?.Count > 0)
            {
                foreach (var responseInterceptors in request.ResponseInterceptors)
                {
                    response = responseInterceptors.Intercept(response);
                }
            }

            return response;
        }

        private void ApplyNetworkEngineRequestConfiguration(UnityNativeRequest request) {
            // Set Headers
            if (_headers?.Count > 0) {
                if (request.Headers == null) {
                    request.SetHeaders(_headers.ToDictionary(x => x.Key, x => x.Value));
                } else {
                    var allHeaders = request.Headers.ToDictionary(x => x.Key, x => x.Value);
                    foreach (var header in _headers) {
                        // Do not overwrite existing headers
                        if (!allHeaders.ContainsKey(header.Key)) {
                            allHeaders.Add(header.Key, header.Value);
                        }
                    }
                    request.SetHeaders(allHeaders);
                }
            }

            // Set Timeout
            if (_timeout != null && request.Timeout == null) {
                request.SetTimeout(_timeout);
            }

            // Set Authorization
            if (_authorization.HasValue && !request.Authorization.HasValue) {
                request.SetAuthorization(_authorization.Value);
            }

            // Set Request Interceptors
            if (_requestInterceptors?.Count > 0) {
                if (request.RequestInterceptors == null) {
                    request.SetRequestInterceptors(_requestInterceptors.ToList());
                } else {
                    var allRequestInterceptors = request.RequestInterceptors.ToList();
                    allRequestInterceptors.AddRange(_requestInterceptors.ToList());
                    request.SetRequestInterceptors(allRequestInterceptors);
                }
            }

            // Set Response Interceptors
            if (_responseInterceptors?.Count > 0) {
                if (request.ResponseInterceptors == null) {
                    request.SetResponseInterceptors(_responseInterceptors.ToList());
                } else {
                    var allResponseInterceptors = request.ResponseInterceptors.ToList();
                    allResponseInterceptors.AddRange(_responseInterceptors.ToList());
                    request.SetResponseInterceptors(allResponseInterceptors);
                }
            }
        }

        private async Task<UnityNativeResponse> SendRequest(UnityNativeRequest request) {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                CleverTapLogger.LogError("Internet connection is not reachable!");
                return new UnityNativeResponse(HttpStatusCode.InternalServerError, null, null, "Internet connection is not reachable");
            }

            UnityWebRequest unityWebRequest = null;
            try {
                unityWebRequest = request.BuildRequest(_baseURI);
                CleverTapLogger.Log("Sending web request");
                // Workaround for async
                var unityWebRequestAsyncOperation = unityWebRequest.SendWebRequest();
                while (!unityWebRequestAsyncOperation.isDone) {
                    await Task.Yield();
                }

                if (unityWebRequest.result == UnityWebRequest.Result.Success)
                {
                    responseFailureCount = 0;
                }
                else
                {
                    responseFailureCount++;
                }
            
                switch (unityWebRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        return new UnityNativeResponse((HttpStatusCode)unityWebRequest.responseCode, unityWebRequest.GetResponseHeaders(), unityWebRequest.downloadHandler.text);

                    case UnityWebRequest.Result.ConnectionError:
                        CleverTapLogger.LogError($"Failed ConnectionError: {(HttpStatusCode)unityWebRequest.responseCode}, error: {unityWebRequest.downloadHandler.text}, request: {request.RequestBody}");
                        return new UnityNativeResponse(HttpStatusCode.InternalServerError, null, null, "Internet connection is not reachable");

                    case UnityWebRequest.Result.ProtocolError:
                        CleverTapLogger.LogError($"Failed ProtocolError: {(HttpStatusCode)unityWebRequest.responseCode}, error: {unityWebRequest.downloadHandler?.text}, request: {request.RequestBody}");
                        return new UnityNativeResponse((HttpStatusCode)unityWebRequest.responseCode, unityWebRequest.GetResponseHeaders(), unityWebRequest.downloadHandler?.text, "Protocol Error");

                    case UnityWebRequest.Result.DataProcessingError:
                        CleverTapLogger.LogError("Failed Data Processing");
                        return new UnityNativeResponse((HttpStatusCode)unityWebRequest.responseCode, null, null, "Failed to Process Data");

                    default:
                        CleverTapLogger.LogError("Failed");
                        return new UnityNativeResponse((HttpStatusCode)unityWebRequest.responseCode, unityWebRequest.GetResponseHeaders(), unityWebRequest.downloadHandler.text, errorMessage: unityWebRequest.error);
                }

            } catch (Exception ex) {
                CleverTapLogger.LogError($"Failed: {ex.Message}, Stack Trace: {ex.StackTrace}, Data: {ex.Data}");
                return new UnityNativeResponse(HttpStatusCode.InternalServerError, null, null, ex.Message);
            } finally {
                unityWebRequest?.Dispose();
            }
        }
    }
}
#endif