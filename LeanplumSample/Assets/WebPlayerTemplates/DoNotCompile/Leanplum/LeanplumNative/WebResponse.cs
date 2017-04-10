// Copyright 2013, Leanplum, Inc.

using LeanplumSDK.MiniJSON;

namespace LeanplumSDK
{
    internal abstract class WebResponse
    {
        public abstract string GetError();
        public abstract string GetResponseBody();
        public abstract object GetResponseAsAsset();

        public object GetResponseBodyAsJson()
        {
            return Json.Deserialize(GetResponseBody());
        }
    }
}
