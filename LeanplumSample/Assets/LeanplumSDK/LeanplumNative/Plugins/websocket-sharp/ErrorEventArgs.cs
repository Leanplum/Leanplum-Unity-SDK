#region License
/*
 * ErrorEventArgs.cs
 *
 * The MIT License
 *
 * Copyright (c) 2012-2013 sta.blockhead
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System;

namespace LeanplumSDK.WebSocketSharp
{
  /// <summary>
  /// Contains the event data associated with a <see cref="WebSocket.OnError"/> event.
  /// </summary>
  /// <remarks>
  /// A <see cref="WebSocket.OnError"/> event occurs when the <see cref="WebSocket"/> gets an error.
  /// If you want to get the error message, you access the <see cref="ErrorEventArgs.Message"/> property.
  /// </remarks>
  internal class ErrorEventArgs : EventArgs
  {
    #region Internal Constructors

    internal ErrorEventArgs (string message)
    {
      Message = message;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the error message.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that contains an error message.
    /// </value>
    public string Message { get; private set; }

    #endregion
  }
}
