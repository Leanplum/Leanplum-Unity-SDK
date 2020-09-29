#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanplumSDK.SocketIOClient.Messages
{
	/// <summary>
	/// Defined as No operation. Used for example to close a poll after the polling duration times out.
	/// </summary>
    internal class NoopMessage : Message
    {
        public NoopMessage()
        {
            this.MessageType = SocketIOMessageTypes.Noop;
        }
        public static NoopMessage Deserialize(string rawMessage)
        {
			return new NoopMessage();
        }
    }
}
#endif
