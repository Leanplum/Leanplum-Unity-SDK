#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeanplumSDK.SocketIOClient.Messages;

namespace LeanplumSDK.SocketIOClient
{
	internal class MessageEventArgs : EventArgs
	{
		public IMessage Message { get; private set; }

		public MessageEventArgs(IMessage msg)
			: base()
		{
			this.Message = msg;
		}
	}
}
#endif
