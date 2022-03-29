using System;
using System.Collections.Generic;

namespace LeanplumSDK
{
    public class FileTransferManager
    {
        public delegate void NoPendingDownloadsHandler();
        public FileTransferManager()
        {
        }

        public static int PendingDownloads { get; private set; }

        private static event NoPendingDownloadsHandler noPendingDownloads;

        public static event NoPendingDownloadsHandler NoPendingDownloads
        {
            add
            {
                noPendingDownloads += value;
                if (PendingDownloads == 0)
                {
                    value();
                }
            }
            remove { noPendingDownloads -= value; }
        }

        // Uses resources endpoint
        // Method: GET
        // Example URL: https://api.leanplum.com/resource/resource/AMIfv94zoleE43w_3PLB0...
        public void DownloadFileResource(string resourceUrl, Action<object> response, Action<Exception> error)
        {
            LeanplumNative.CompatibilityLayer.LogDebug($"DownloadFileResource: {PendingDownloads}");
            Request request = RequestBuilder.withFileResource(resourceUrl)
                .CreateImmediate();

            request.Response += response;
            request.Error += error;

            DownloadAsset(request);
        }

        internal virtual void DownloadAsset(Request request)
        {
            PendingDownloads++;
            RequestUtil.CreateWebRequest(Leanplum.ApiConfig.apiHost, request.ApiMethod, null, request.HttpMethod,
                                  Leanplum.ApiConfig.apiSSL, Constants.NETWORK_TIMEOUT_SECONDS).GetAssetBundle(
                delegate (WebResponse response)
            {
                PendingDownloads--;
                if (response.GetError() != null)
                {
                    request.OnError(new LeanplumException("Error sending request: " + response.GetError()));
                }
                else
                {
                    request.OnResponse(response.GetResponseAsAsset());
                }

                if (PendingDownloads == 0)
                {
                    noPendingDownloads?.Invoke();
                }
            });
        }

        public static void ClearNoPendingDownloads()
        {
            noPendingDownloads = null;
        }
    }
}
