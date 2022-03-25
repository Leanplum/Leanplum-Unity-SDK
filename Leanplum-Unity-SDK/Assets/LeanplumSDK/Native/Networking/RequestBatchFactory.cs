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
using LeanplumSDK;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK
{
    public class RequestBatchFactory
    {
        internal EventDataManager eventDataManager = new EventDataManager();

        private static readonly int MAX_EVENTS_PER_API_CALL = 10000;

        public RequestBatchFactory()
        {
        }

        public RequestBatch CreateNextBatch()
        {
            IList<IDictionary<string, string>> requestsToSend = GetUnsentRequests();
            return new RequestBatch(requestsToSend, JsonEncodeRequests(requestsToSend));
        }

        protected virtual IList<IDictionary<string, string>> GetUnsentRequests()
        {
            return eventDataManager.GetEvents(MAX_EVENTS_PER_API_CALL); ;
        }

        internal void DeleteFinishedBatch(RequestBatch batch)
        {
            eventDataManager.DeleteEvents(batch.EventsCount);
        }

        internal static string JsonEncodeRequests(IList<IDictionary<string, string>> requestData)
        {
            IDictionary<string, object> data = new Dictionary<string, object>
            {
                [Constants.Params.DATA] = requestData
            };
            return Json.Serialize(data);
        }
    }
}