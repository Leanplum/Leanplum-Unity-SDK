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
namespace LeanplumSDK
{
    internal class UnityWebResponse : WebResponse
    {
        private readonly string error;
        private long statusCode;
        private readonly string responseBody;
        private readonly object responseBodyAsAsset;

        public UnityWebResponse(long statusCode, string error, string text, object data)
        {
            this.statusCode = statusCode;
            this.error = error;
            this.responseBody = text;
            this.responseBodyAsAsset = data;
        }

        public override long GetStatusCode()
        {
#if !LP_UNITY_LEGACY_WWW
            return statusCode;
#else
            if (error != null)
            {
                if (error[0] == '4' || error[0] == '5')
                {

                    string code = error.Substring(0, 3);
                    if (!string.IsNullOrEmpty(code) && long.TryParse(code, out statusCode))
                    {
                        return statusCode;
                    }
                    else
                    {
                        return 400;
                    }

                }
                else
                {
                    return statusCode;
                }
            }
            return statusCode;
#endif
        }

        public override string GetError()
        {
            return error;
        }

        public override string GetResponseBody()
        {
            return responseBody;
        }

        public override object GetResponseAsAsset()
        {
            return responseBodyAsAsset;
        }
    }
}
