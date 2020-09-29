//
// Copyright 2014, Leanplum, Inc.
//
//  Licensed to the Apache Software Foundation (ASF) under one
//  or more contributor license agreements.  See the NOTICE file
//  distributed with this work for additional information
//  regarding copyright ownership.  The ASF licenses this file
//  to you under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing,
//  software distributed under the License is distributed on an
//  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//  KIND, either express or implied.  See the License for the
//  specific language governing permissions and limitations
//  under the License.
#if UNITY_PRO_LICENSE && !UNITY_WEBGL
#define LEANPLUM_USE_SOCKETS
#endif

using System;
using System.IO;
using System.Net;
using LeanplumSDK;

namespace LeanplumSDK
{
#if LEANPLUM_USE_SOCKETS
	using System.Net.Sockets;

	public class SocketUtils : ISocketUtils
	{
		public bool AreSocketsAvailable
		{
			get { return true; }
		}

		public bool IsDataAvailable (Stream stream)
		{
			return ((NetworkStream)stream).DataAvailable;
		}
		
		public Stream CreateNetworkStream (ISocket socket, bool ownsSocket)
		{
			return new NetworkStream (((SocketWrapper) socket).WrappedSocket, ownsSocket);
		}
		
		public ISocket CreateSocket (IPAddress address)
		{
			return new SocketWrapper (address.AddressFamily);
		}
		
		public ISocketAsyncEventArgs CreateSocketAsyncEventArgs ()
		{
			return new SocketAsyncEventArgsWrapper ();
		}
		
		public ISocketAsyncEventArgs CreateSocketAsyncEventArgs (EventArgs e)
		{
			return new SocketAsyncEventArgsWrapper (e);
		}
		
		public ITcpClient CreateTcpClient (string host, int port)
		{
			return new TcpClientWrapper (host, port);
		}
	}
	
	public class SocketWrapper : ISocket
	{
		private Socket socket;
		
		public SocketWrapper (AddressFamily addressFamily)
		{
			socket = new Socket (addressFamily, SocketType.Stream, ProtocolType.Tcp);
		}
		
		public SocketWrapper (Socket socket)
		{
			this.socket = socket;
		}
		
		public void Bind (IPEndPoint endpoint)
		{
			socket.Bind (endpoint);
		}
		
		public void Listen (int backlog)
		{
			socket.Listen (backlog);
		}
		
		public void AcceptAsync (ISocketAsyncEventArgs args)
		{
			socket.AcceptAsync (((SocketAsyncEventArgsWrapper) args).WrappedArgs);
		}
		
		public Socket WrappedSocket {
			get { return socket; }
		}
		
		public void Close ()
		{
			socket.Close ();
		}
		
		public void Shutdown ()
		{
			socket.Shutdown (SocketShutdown.Both);
		}
		
		public EndPoint LocalEndPoint {
			get { return socket.LocalEndPoint; }
		}
		
		public EndPoint RemoteEndPoint {
			get { return socket.RemoteEndPoint; }
		}
	}
	
	public class SocketAsyncEventArgsWrapper : ISocketAsyncEventArgs
	{
		private SocketAsyncEventArgs args;
		
		public SocketAsyncEventArgsWrapper ()
		{
			args = new SocketAsyncEventArgs ();
			args.Completed += onAccept;
		}
		
		public SocketAsyncEventArgsWrapper (EventArgs e)
		{
			args = (SocketAsyncEventArgs)e;
		}
		
		public SocketAsyncEventArgs WrappedArgs {
			get { return args;}
		}
		
		public object UserToken {
			get { return args.UserToken; }
			set { args.UserToken = value; }
		}
		
		public bool IsSuccess ()
		{
			return args.SocketError == SocketError.Success;
		}
		
		public ISocket AcceptSocket {
			get
			{
				return new SocketWrapper (args.AcceptSocket);
			}
			set
			{
				if (value == null) {
					args.AcceptSocket = null;
				} else {
					args.AcceptSocket = ((SocketWrapper) value).WrappedSocket;
				}
			}
		}
		
		private void onAccept (object sender, EventArgs e)
		{
			Completed (sender, e);
		}
		
		public event EventHandler<EventArgs> Completed;
	}
	
	public class TcpClientWrapper : ITcpClient
	{
		private TcpClient tcpClient;
		
		public TcpClientWrapper ()
		{
			tcpClient = new TcpClient ();
		}
		
		public TcpClientWrapper (string host, int port)
		{
			tcpClient = new TcpClient (host, port);
		}
		
		public TcpClient GetClient
		{
			get { return tcpClient; }
		}
		
		public EndPoint GetLocalEndpoint ()
		{
			return tcpClient.Client.LocalEndPoint;
		}
		
		public EndPoint GetRemoteEndpoint ()
		{
			return tcpClient.Client.RemoteEndPoint;
		}
		
		public Stream GetStream ()
		{
			return tcpClient.GetStream ();
		}
		
		public void Close ()
		{
			tcpClient.Close ();
		}
	}
#else
	public class SocketUtils : DisabledSocketUtils
	{
	}
#endif
}
