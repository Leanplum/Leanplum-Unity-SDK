#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanplumSDK.SocketIOClient
{
    
	internal class ErrorEventArgs : EventArgs
	{
		
		public string Message { get; set; }
		public string Exception { get; set; }

		public ErrorEventArgs (string message) :base()
		{
			this.Message = message;
		}
		public ErrorEventArgs (string message, string exception) : base()
		{
			this.Message = message;
			this.Exception = exception;
		}
	}
}
#endif
