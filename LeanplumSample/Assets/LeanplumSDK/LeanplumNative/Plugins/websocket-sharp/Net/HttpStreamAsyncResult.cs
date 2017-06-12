//
// HttpStreamAsyncResult.cs
//	Copied from System.Net.HttpStreamAsyncResult.cs
//
// Authors:
//	Gonzalo Paniagua Javier (gonzalo@novell.com)
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Threading;

namespace LeanplumSDK.WebSocketSharp.Net {

	class HttpStreamAsyncResult : IAsyncResult {

		#region Private Fields

		bool             completed;
		ManualResetEvent handle;
		object           locker = new object ();

		#endregion

		#region Internal Fields

		internal AsyncCallback Callback;
		internal int           Count;
		internal byte []       Buffer;
		internal Exception     Error;
		internal int           Offset;
		internal object        State;
		internal int           SyncRead;

		#endregion

		#region Properties

		public object AsyncState {
			get { return State; }
		}

		public WaitHandle AsyncWaitHandle {
			get {
				lock (locker) {
					if (handle == null)
						handle = new ManualResetEvent (completed);
				}

				return handle;
			}
		}

		public bool CompletedSynchronously {
			get { return (SyncRead == Count); }
		}

		public bool IsCompleted {
			get {
				lock (locker) {
					return completed;
				}
			}
		}

		#endregion

		#region Public Methods

		public void Complete ()
		{
			lock (locker) {
				if (completed)
					return;

				completed = true;
				if (handle != null)
					handle.Set ();

				if (Callback != null)
					Callback.BeginInvoke (this, null, null);
			}
		}

		public void Complete (Exception e)
		{
			Error = e;
			Complete ();
		}

		#endregion
	}
}
