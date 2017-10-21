#if !UNITY_WEBGL
//
// CookieException.cs
//	Copied from System.Net.CookieException.cs
//
// Author:
//   Lawrence Pit (loz@cable.a2000.nl)
//
// Copyright (c) 2012-2013 sta.blockhead (sta.blockhead@gmail.com)
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
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace LeanplumSDK.WebSocketSharp.Net {

	/// <summary>
	/// The exception that is thrown when a <see cref="Cookie"/> gets an error.
	/// </summary>
	[Serializable]
    internal class CookieException : FormatException, ISerializable
	{
		#region Internal Constructors

		internal CookieException (string message)
			: base (message)
		{
		}

		internal CookieException (string message, Exception innerException)
			: base (message, innerException)
		{
		}

		#endregion

		#region Protected Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="CookieException"/> class
		/// with the specified <see cref="SerializationInfo"/> and <see cref="StreamingContext"/>.
		/// </summary>
		/// <param name="serializationInfo">
		/// A <see cref="SerializationInfo"/> that holds the serialized object data.
		/// </param>
		/// <param name="streamingContext">
		/// A <see cref="StreamingContext"/> that contains the contextual information about the source or destination.
		/// </param>
		protected CookieException (SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base (serializationInfo, streamingContext)
		{
		}

		#endregion

		#region Public Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="CookieException"/> class.
		/// </summary>
		public CookieException ()
			: base ()
		{
		}

		#endregion

		#region Explicit Interface Implementation

#pragma warning disable 0618
		/// <summary>
		/// Populates the specified <see cref="SerializationInfo"/> with the data needed to serialize the <see cref="CookieException"/>.
		/// </summary>
		/// <param name="serializationInfo">
		/// A <see cref="SerializationInfo"/> that holds the serialized object data.
		/// </param>
		/// <param name="streamingContext">
		/// A <see cref="StreamingContext"/> that specifies the destination for the serialization.
		/// </param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter, SerializationFormatter = true)]
		void ISerializable.GetObjectData (SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			base.GetObjectData (serializationInfo, streamingContext);
		}

		#endregion

		#region Public Method

		/// <summary>
		/// Populates the specified <see cref="SerializationInfo"/> with the data needed to serialize the <see cref="CookieException"/>.
		/// </summary>
		/// <param name="serializationInfo">
		/// A <see cref="SerializationInfo"/> that holds the serialized object data.
		/// </param>
		/// <param name="streamingContext">
		/// A <see cref="StreamingContext"/> that specifies the destination for the serialization.
		/// </param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData (SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			base.GetObjectData (serializationInfo, streamingContext);
		}
#pragma warning restore 0618

		#endregion
	}
}
#endif
