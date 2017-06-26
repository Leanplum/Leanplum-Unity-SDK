//
// Copyright 2013, Leanplum, Inc.
//
//  Licensed to the Apache Software Foundation (ASF) under one
//  or more contributor license agreements.  See the NOTICE file
//  distributed with this work for additional information
//  regarding copyright ownership.  The ASF licenses this file
//  to you under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing,
//  software distributed under the License is distributed on an
//  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//  KIND, either express or implied.  See the License for the
//  specific language governing permissions and limitations
//  under the License.
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
