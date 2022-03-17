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
    public enum RequestType
    {
        DEFAULT,
        IMMEDIATE
    }

    public class Request
    {
        //private readonly string httpMethod;
        //private readonly string apiMethod;
        //private readonly LPRequestType type;
        private readonly string requestId = Guid.NewGuid().ToString().ToLower();
        //private readonly IDictionary<string, object> parameters;

        public string Id
        {
            get
            {
                return requestId;
            }
        }

        public RequestType Type
        {
            get;
            private set;
        }

        public string ApiMethod
        {
            get;
            private set;
        }


        public string HttpMethod
        {
            get;
            private set;
        }

        public IDictionary<string, object> Parameters
        {
            get;
            private set;
        }

        public Request(string httpMethod, string apiMethod, RequestType type, IDictionary<string, object> parameters)
        {
            this.HttpMethod = httpMethod;
            this.ApiMethod = apiMethod;
            this.Type = type;
            this.Parameters = parameters ?? new Dictionary<string, object>();
        }

        public event Action<object> Response;
        public event Action<Exception> Error;

        internal virtual void OnError(Exception obj)
        {
            Error?.Invoke(obj);
        }

        internal virtual void OnResponse(object obj)
        {
            Response?.Invoke(obj);
        }

    }
}