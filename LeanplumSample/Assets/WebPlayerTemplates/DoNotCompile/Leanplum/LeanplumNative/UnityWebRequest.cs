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
using UnityEngine;

namespace LeanplumSDK
{
    internal sealed class UnityWebRequest : WebRequest
    {
        private WWWForm wwwForm;

        public UnityWebRequest(string url, int timeout) : base(url, timeout)
        {
        }

        internal override void AttachPostParameters(IDictionary<string, string> parameters)
        {
            if (wwwForm == null)
            {
                wwwForm = new WWWForm();
            }
            foreach (KeyValuePair<string, string> entry in parameters)
            {
                wwwForm.AddField(entry.Key, entry.Value);
            }
        }

        internal override void AttachBinaryField(string key, byte[] data)
        {
            wwwForm.AddBinaryData(key, data);
        }

        internal override void GetResponseAsync(Action<WebResponse> responseHandler)
        {
            LeanplumUnityHelper.Instance.StartRequest(url, wwwForm, responseHandler, timeout);
        }

        internal override void GetAssetBundle(Action<WebResponse> responseHandler)
        {
            LeanplumUnityHelper.Instance.StartRequest(url, wwwForm, responseHandler, timeout, true);
        }
    }
}
