// Copyright 2013, Leanplum, Inc.

using System;
using System.Collections.Generic;

namespace LeanplumSDK
{
    public abstract class WebRequest
    {
        protected string url;
        protected int timeout;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WebRequest" /> class. SSL is controlled simply through the URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        internal WebRequest(string url, int timeout)
        {
            this.url = url;
            this.timeout = timeout;
        }

        internal void AttachGetParameters(IDictionary<string, string> parameters)
        {
            string queryParams = "";
            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> entry in parameters)
                {
                    if (entry.Value == null)
                    {
						LeanplumNative.CompatibilityLayer.LogWarning("Request param " + entry.Key + " is null");
                    }
                    else
                    {
                        queryParams += queryParams.Length == 0 ? '?' : '&';
						queryParams += entry.Key + "=" + LeanplumNative.CompatibilityLayer.URLEncode(entry.Value);
                    }
                }
                url += queryParams;
            }
        }

        /// <summary>
        ///     Attaches the post parameters. Does not change or remove already parameters unless new values are
        ///     specifically indicated in parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        internal abstract void AttachPostParameters(IDictionary<string, string> parameters);

        /// <summary>
        ///     Attaches binary data.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        internal abstract void AttachBinaryField(string key, byte[] data);

        /// <summary>
        ///     Executes the request if not ran already and returns immediately. Calls back
        ///     responseHandler when a response is received.
        /// </summary>
        /// <param name="responseHandler">The response handler.</param>
        internal abstract void GetResponseAsync(Action<WebResponse> responseHandler);

        /// <summary>
        ///     Executes the request to download and load an assetbundle.
        /// </summary>
        /// <param name="responseHandler">The response handler.</param>
        internal abstract void GetAssetBundle(Action<WebResponse> responseHandler);
    }
}
