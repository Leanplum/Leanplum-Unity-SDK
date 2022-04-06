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
using System.Collections.Generic;

namespace LeanplumSDK
{
    public class RequestBatch
    {
        private readonly IList<IDictionary<string, string>> requestsToSend;
        private readonly string jsonEncoded;

        public int EventsCount => requestsToSend.Count;

        public bool IsFull => EventsCount == Constants.MAX_STORED_API_CALLS;

        public bool IsEmpty => EventsCount == 0;

        public string JsonEncoded => jsonEncoded;

        public RequestBatch(IList<IDictionary<string, string>> requestsToSend, string jsonEncoded)
        {
            this.requestsToSend = requestsToSend;
            this.jsonEncoded = jsonEncoded;
        }
    }
}