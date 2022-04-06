//
// Copyright 2022, Leanplum, Inc.
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

        /// <summary>
        /// Downloads AssetBundle using a resource URL
        /// Uses the resource path
        /// Method: GET
        /// Example URL: https://api.leanplum.com/resource/resource/AMIfv94zoleE43w_3PLB0...
        /// </summary>
        /// <param name="resourceUrl">Resource URL</param>
        /// <param name="response">Response handler</param>
        /// <param name="error">Error handler</param>
        public void DownloadAssetResource(string resourceUrl, Action<object> response, Action<Exception> error)
        {
            Request request = RequestBuilder.withFileResource(resourceUrl)
                .CreateImmediate();

            request.Response += response;
            request.Error += error;

            PendingDownloads++;
            DownloadAsset(request);
        }

        internal virtual void DownloadAsset(Request request)
        {
            RequestUtil.CreateWebRequest(Leanplum.ApiConfig.ApiHost, request.ApiMethod, null, request.HttpMethod,
                                  Leanplum.ApiConfig.ApiSSL, Constants.NETWORK_TIMEOUT_SECONDS).GetAssetBundle(
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
