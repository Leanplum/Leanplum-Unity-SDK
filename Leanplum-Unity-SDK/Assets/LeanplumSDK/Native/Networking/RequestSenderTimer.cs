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
using System.Collections;
using UnityEngine;

namespace LeanplumSDK
{
    public class RequestSenderTimer
    {
        public EventsUploadInterval TimerInterval { get; set; } = EventsUploadInterval.AtMost15Minutes;

        private Coroutine timerCoroutine;

        public RequestSenderTimer()
        {
        }

        public void Stop()
        {
            if (timerCoroutine != null)
            {
                LeanplumUnityHelper.Instance.StopCoroutine(timerCoroutine);
            }
        }

        public void Start()
        {
            // Use Coroutines instead of InvokeRepeating
            // InvokeRepeating requires the method invoked to be implemented in the MonoBehavior sender
            timerCoroutine = LeanplumUnityHelper.Instance.StartCoroutine(TimerCoroutine());
        }

        private void SendRequestsHeartbeat()
        {
            Request request = RequestBuilder.withHeartbeatAction().CreateImmediate();
            Leanplum.RequestSender.Send(request);
        }

        private IEnumerator TimerCoroutine()
        {
            yield return new WaitForSeconds(((int)TimerInterval) * 60);
            // Send heartbeat
            SendRequestsHeartbeat();
            // Restart timer
            Start();
        }
    }
}