#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
//using SimpleJson.Reflection;

namespace LeanplumSDK.SocketIOClient.Messages
{
    internal class JsonEncodedEventMessage
    {
         public string name { get; set; }

         public object[] args { get; set; }

        public JsonEncodedEventMessage()
        {
        }
        
		public JsonEncodedEventMessage(string name, object payload) : this(name, new[]{payload})
        {
        }
        
		public JsonEncodedEventMessage(string name, object[] payloads)
        {
            this.name = name;
            this.args = payloads;
        }

        public T GetFirstArgAs<T>()
        {
            try
            {
                var firstArg = this.args.FirstOrDefault();
                if (firstArg != null)
                    return (T) MiniJSON.Json.Deserialize(firstArg.ToString());
            }
            catch (Exception ex)
            {
                // add error logging here
                throw ex;
            }
            return default(T);
        }
        public IEnumerable<T> GetArgsAs<T>()
        {
            List<T> items = new List<T>();
            foreach (var i in this.args)
            {
                items.Add((T) MiniJSON.Json.Deserialize(i.ToString()));
            }
            return items.AsEnumerable();
        }

        public string ToJsonString()
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["name"] = this.name;
            dictionary["args"] = this.args;
            return MiniJSON.Json.Serialize(dictionary);
//            return SimpleJson.SimpleJson.SerializeObject(this);
        }

        public static JsonEncodedEventMessage Deserialize(string jsonString)
        {
			JsonEncodedEventMessage msg = null;
            try { msg = (JsonEncodedEventMessage) MiniJSON.Json.Deserialize(jsonString); }
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}
            return msg;
        }
    }
}
#endif
