//
// Copyright 2023, Leanplum, Inc.
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
using System.Collections.Generic;

namespace LeanplumSDK
{
    internal class RequestBuilder
    {
        public static string GET = "GET";
        public static string POST = "POST";

        internal class ApiMethods
        {
            internal const string ADVANCE = "advance";
            internal const string DOWNLOAD_FILE = "downloadFile";
            internal const string GET_VARS = "getVars";
            internal const string MULTI = "multi";
            internal const string PAUSE_SESSION = "pauseSession";
            internal const string PAUSE_STATE = "pauseState";
            internal const string REGISTER_FOR_DEVELOPMENT = "registerDevice";
            internal const string RESUME_SESSION = "resumeSession";
            internal const string RESUME_STATE = "resumeState";
            internal const string SET_USER_ATTRIBUTES = "setUserAttributes";
            internal const string SET_DEVICE_ATTRIBUTES = "setDeviceAttributes";
            internal const string SET_VARS = "setVars";
            internal const string START = "start";
            internal const string STOP = "stop";
            internal const string TRACK = "track";
            internal const string UPLOAD_FILE = "uploadFile";
            internal const string SET_TRAFFIC_SOURCE_INFO = "setTrafficSourceInfo";
            internal const string GET_INBOX_MESSAGES = "getNewsfeedMessages";
            internal const string MARK_INBOX_MESSAGE_AS_READ = "markNewsfeedMessageAsRead";
            internal const string DELETE_INBOX_MESSAGE = "deleteNewsfeedMessage";
            internal const string LOG = "log";
            internal const string HEARTBEAT = "heartbeat";
        }

        private readonly string httpMethod;
        private readonly string apiMethod;
        private RequestType type = RequestType.DEFAULT;
        private IDictionary<string, object> parameters;

        protected RequestBuilder(string httpMethod, string apiMethod)
        {
            this.httpMethod = httpMethod;
            this.apiMethod = apiMethod;
        }

        internal static RequestBuilder WithStartAction()
        {
            return new RequestBuilder(POST, ApiMethods.START);
        }

        public static RequestBuilder WithGetVarsAction()
        {
            return new RequestBuilder(POST, ApiMethods.GET_VARS);
        }

        public static RequestBuilder WithSetVarsAction()
        {
            return new RequestBuilder(POST, ApiMethods.SET_VARS);
        }

        public static RequestBuilder WithStopAction()
        {
            return new RequestBuilder(POST, ApiMethods.STOP);
        }

        public static RequestBuilder WithTrackAction()
        {
            return new RequestBuilder(POST, ApiMethods.TRACK);
        }

        public static RequestBuilder WithAdvanceAction()
        {
            return new RequestBuilder(POST, ApiMethods.ADVANCE);
        }

        public static RequestBuilder WithPauseSessionAction()
        {
            return new RequestBuilder(POST, ApiMethods.PAUSE_SESSION);
        }

        public static RequestBuilder WithPauseStateAction()
        {
            return new RequestBuilder(POST, ApiMethods.PAUSE_STATE);
        }

        public static RequestBuilder WithResumeSessionAction()
        {
            return new RequestBuilder(POST, ApiMethods.RESUME_SESSION);
        }

        public static RequestBuilder WithResumeStateAction()
        {
            return new RequestBuilder(POST, ApiMethods.RESUME_STATE);
        }

        public static RequestBuilder WithMultiAction()
        {
            return new RequestBuilder(POST, ApiMethods.MULTI);
        }

        public static RequestBuilder WithRegisterForDevelopmentAction()
        {
            return new RequestBuilder(POST, ApiMethods.REGISTER_FOR_DEVELOPMENT);
        }

        public static RequestBuilder WithSetUserAttributesAction()
        {
            return new RequestBuilder(POST, ApiMethods.SET_USER_ATTRIBUTES);
        }

        public static RequestBuilder WithSetDeviceAttributesAction()
        {
            return new RequestBuilder(POST, ApiMethods.SET_DEVICE_ATTRIBUTES);
        }

        public static RequestBuilder WithSetTrafficSourceInfoAction()
        {
            return new RequestBuilder(POST, ApiMethods.SET_TRAFFIC_SOURCE_INFO);
        }

        public static RequestBuilder WithUploadFileAction()
        {
            return new RequestBuilder(POST, ApiMethods.UPLOAD_FILE);
        }

        public static RequestBuilder WithDownloadFileAction()
        {
            return new RequestBuilder(GET, ApiMethods.DOWNLOAD_FILE);
        }

        public static RequestBuilder WithHeartbeatAction()
        {
            return new RequestBuilder(POST, ApiMethods.HEARTBEAT);
        }

        public static RequestBuilder WithLogAction()
        {
            return new RequestBuilder(POST, ApiMethods.LOG);
        }

        public static RequestBuilder WithGetInboxMessagesAction()
        {
            return new RequestBuilder(POST, ApiMethods.GET_INBOX_MESSAGES);
        }

        public static RequestBuilder WithMarkInboxMessageAsReadAction()
        {
            return new RequestBuilder(POST, ApiMethods.MARK_INBOX_MESSAGE_AS_READ);
        }

        public static RequestBuilder WithDeleteInboxMessageAction()
        {
            return new RequestBuilder(POST, ApiMethods.DELETE_INBOX_MESSAGE);
        }

        public static RequestBuilder WithFileResource(string resourceUrl)
        {
            return new RequestBuilder(GET, resourceUrl);
        }

        public RequestBuilder AndParam(string param, object value)
        {
            if (parameters == null)
                parameters = new Dictionary<string, object>();

            parameters.Add(param, value);
            return this;
        }

        public RequestBuilder AndParameters(IDictionary<string, object> parameters)
        {
            if (this.parameters == null)
            {
                this.parameters = parameters;
            }
            else
            {
                this.parameters.Merge(parameters);
            }

            return this;
        }

        public RequestBuilder AndType(RequestType type)
        {
            this.type = type;
            return this;
        }

        public Request Create()
        {
            LeanplumNative.CompatibilityLayer.LogDebug($"Will call API method: {apiMethod}. Request Type: {type}");
            return new Request(httpMethod, apiMethod, type, parameters);
        }

        public Request CreateImmediate()
        {
            return AndType(RequestType.IMMEDIATE).Create();
        }
    }
}