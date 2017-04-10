// Copyright 2013, Leanplum, Inc.

namespace LeanplumSDK
{
    internal class UnityWebResponse : WebResponse
    {
        private readonly string error;
        private readonly string responseBody;
        private readonly object responseBodyAsAsset;

        public UnityWebResponse(string error, string text, object data)
        {
            if (string.IsNullOrEmpty(error))
            {
                responseBody = text;
                responseBodyAsAsset = data;
            }
            else
            {
                this.error = error;
            }
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
