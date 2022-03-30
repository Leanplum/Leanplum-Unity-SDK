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
using System.Collections.Generic;

namespace LeanplumSDK
{
    public static class RequestUtil
    {
        public static WebRequest CreateWebRequest(string hostName,
            string path,
            IDictionary<string, string> parameters,
            string httpMethod,
            bool ssl,
            int timeout)
        {
            WebRequest request = CreateWebRequest(hostName, path, ssl, timeout);
            if (httpMethod.Equals("GET"))
            {
                request.AttachGetParameters(parameters);
            }
            else
            {
                request.AttachPostParameters(parameters);
            }

            return request;
        }

        public static WebRequest CreateWebRequest(string hostName,
            string path,
            bool ssl,
            int timeout)
        {
            string fullPath;
            if (path.StartsWith("http"))
            {
                fullPath = path;
            }
            else
            {
                fullPath = (ssl ? "https://" : "http://") + hostName + "/" + path;
            }
            return LeanplumNative.CompatibilityLayer.CreateWebRequest(fullPath, timeout);
        }

        internal static int NumResponses(object response)
        {
            try
            {
                return ((response as IDictionary<string, object>)[Constants.Keys.RESPONSE] as IList<object>).Count;
            }
            catch (KeyNotFoundException e)
            {
                LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", e);
                return 0;
            }
            catch (NullReferenceException e)
            {
                LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", e);
                return 0;
            }
        }

        internal static object GetResponseAt(object response, int index)
        {
            try
            {
                return ((response as IDictionary<string, object>)[Constants.Keys.RESPONSE] as IList<object>)[index];
            }
            catch (KeyNotFoundException e)
            {
                LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", e);
                return null;
            }
            catch (NullReferenceException e)
            {
                LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", e);
                return null;
            }
        }

        internal static IDictionary<string, object> GetResponseForId(object response, string reqId)
        {
            try
            {
                if ((response as IDictionary<string, object>)[Constants.Keys.RESPONSE] is IList<object> responses)
                {
                    foreach (var singleResponse in responses)
                    {
                        var responseValues = singleResponse as IDictionary<string, object>;
                        var val = Util.GetValueOrDefault(responseValues, Constants.Params.REQUEST_ID) as string;
                        if (reqId.Equals(val, StringComparison.OrdinalIgnoreCase))
                        {
                            return responseValues;
                        }
                    }
                }
            }
            catch (KeyNotFoundException e)
            {
                LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", e);
                return null;
            }
            catch (NullReferenceException e)
            {
                LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", e);
                return null;
            }

            return null;
        }

        internal static object GetLastResponse(object response)
        {
            int numResponses = NumResponses(response);
            return numResponses > 0 ? GetResponseAt(response, numResponses - 1) : null;
        }

        internal static bool IsResponseSuccess(IDictionary<string, object> response)
        {
            object success;
            if (response != null && response.TryGetValue(Constants.Keys.SUCCESS, out success))
            {
                return (bool)success;
            }
            LeanplumNative.CompatibilityLayer.LogError("Invalid response (missing field: success)");
            return false;
        }

        internal static string GetResponseError(IDictionary<string, object> response)
        {
            if (response.ContainsKey(Constants.Keys.ERROR))
            {
                if (response[Constants.Keys.ERROR] is IDictionary<string, object> error
                    && error.ContainsKey(Constants.Keys.MESSAGE))
                {
                    return error[Constants.Keys.MESSAGE] as string;
                }
            }
            LeanplumNative.CompatibilityLayer.LogError("Invalid response (missing field: error)");
            return null;
        }

        internal static bool UpdateApiConfig(object jsonResponse)
        {
            try
            {
                int numResponses = NumResponses(jsonResponse);

                for (int i = 0; i < numResponses; i++)
                {
                    IDictionary<string, object> response = GetResponseAt(jsonResponse, i) as IDictionary<string, object>;

                    if (IsResponseSuccess(response))
                    {
                        continue;
                    }

                    string apiHost = Util.GetValueOrDefault(response, Constants.Params.API_HOST) as string;
                    string apiPath = Util.GetValueOrDefault(response, Constants.Params.API_PATH) as string;
                    string devServerHost = Util.GetValueOrDefault(response, Constants.Params.DEV_SERVER_HOST) as string;
                    // Prevent setting the same API config and request retry loop
                    bool configUpdated = false;

                    bool hasNewApiHost = !string.IsNullOrEmpty(apiHost) && !apiHost.Equals(Leanplum.ApiConfig.ApiHost);
                    bool hasNewApiPath = !string.IsNullOrEmpty(apiPath) && !apiHost.Equals(Leanplum.ApiConfig.ApiHost);
                    bool hasNewSocketHost = !string.IsNullOrEmpty(devServerHost) && !apiHost.Equals(Leanplum.ApiConfig.SocketHost);

                    // API config
                    if (hasNewApiHost || hasNewApiPath)
                    {
                        configUpdated = true;
                        if (string.IsNullOrEmpty(apiHost))
                        {
                            apiHost = Leanplum.ApiConfig.ApiHost;
                        }
                        if (string.IsNullOrEmpty(apiPath))
                        {
                            apiPath = Leanplum.ApiConfig.ApiPath;
                        }

                        LeanplumNative.CompatibilityLayer.LogDebug($"Changing API endpoint to {apiHost}/{apiPath}");
                        Leanplum.SetApiConnectionSettings(apiHost, apiPath, Leanplum.ApiConfig.ApiSSL);
                    }

                    // Socket config
                    if (hasNewSocketHost)
                    {
                        configUpdated = true;
                        int socketPort = Leanplum.ApiConfig.SocketPort;
                        LeanplumNative.CompatibilityLayer.LogDebug($"Changing socket to {devServerHost}:{socketPort}");
                        Leanplum.SetSocketConnectionSettings(devServerHost, socketPort);
                    }

                    return configUpdated;
                }
            }
            catch (Exception e)
            {
                LeanplumNative.CompatibilityLayer.LogError("Error parsing response for API config", e);
            }
            return false;
        }
    }
}