#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LeanplumSDK.SocketIOClient.Messages
{
    internal class Heartbeat : Message
    {
        public static string HEARTBEAT = "2::";

        public Heartbeat()
        {
            this.MessageType = SocketIOMessageTypes.Heartbeat;
        }

        public override string Encoded
        {
            get
            {
                return HEARTBEAT;
            }
        }

    }
}
#endif
