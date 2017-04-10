// Copyright 2013, Leanplum, Inc.

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
