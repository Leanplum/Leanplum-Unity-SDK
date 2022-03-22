using System;
using System.Collections.Generic;

namespace LeanplumSDK
{
    public class RequestBatch
    {
        private readonly IList<IDictionary<string, string>> requestsToSend;
        private readonly string jsonEncoded;

        public int EventsCount
        {
            get
            {
                return requestsToSend.Count;
            }
        }

        public bool IsFull
        {
            get
            {
                return EventsCount == Constants.MAX_STORED_API_CALLS;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return EventsCount == 0;
            }
        }

        public string JsonEncoded
        {
            get
            {
                return jsonEncoded;
            }
        }


        public RequestBatch(IList<IDictionary<string, string>> requestsToSend, string jsonEncoded)
        {
            this.requestsToSend = requestsToSend;
            this.jsonEncoded = jsonEncoded;
        }
    }
}