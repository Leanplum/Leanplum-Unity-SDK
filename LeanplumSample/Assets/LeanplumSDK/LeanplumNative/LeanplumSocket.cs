#if !UNITY_WEBGL
// Copyright 2013, Leanplum, Inc.
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
using LeanplumSDK.MiniJSON;
using LeanplumSDK.SocketIOClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

namespace LeanplumSDK
{
    /// <summary>
    ///     Leanplum socket class.
    /// </summary>
    internal class LeanplumSocket
    {
        private readonly Timer reconnectTimer;
        private readonly Client socketIOClient;
        private bool authSent;
        private bool connected;
        private bool connecting;
        private Action onUpdateVars;

        public LeanplumSocket(Action onUpdate)
        {
            onUpdateVars = onUpdate;
            socketIOClient = new Client("http://" + Constants.SOCKET_HOST + ":" + Constants.SOCKET_PORT);
            socketIOClient.Opened += OnSocketOpened;
            socketIOClient.Message += OnSocketMessage;
            socketIOClient.SocketConnectionClosed += OnSocketConnectionClosed;
            socketIOClient.Error += OnSocketError;
            
            // Initialize and start a timer to check periodically if the socket is still alive.
            reconnectTimer = new Timer(Constants.NETWORK_SOCKET_TIMEOUT_SECONDS * 1000);
            reconnectTimer.AutoReset = true;
            reconnectTimer.Elapsed += delegate
            {
                if (!connected && !connecting)
                {
                    Connect();
                }
            };

            Connect();
            reconnectTimer.Start();
        }

        private void Connect()
        {
            connecting = true;
            var socketThread = new System.Threading.Thread(socketIOClient.Connect);
            socketThread.Start();
        }

        public void Close()
        {
            socketIOClient.Close();
        }

        private void OnSocketOpened(object obj, EventArgs e)
        {
            if (connecting)
            {
                LeanplumNative.CompatibilityLayer.Log("Connected to development server.");
                connected = true;
                connecting = false;
                if (!authSent && connected)
                {
                    IDictionary<string, string> args = new Dictionary<string, string>();
                    args[Constants.Params.APP_ID] = LeanplumRequest.AppId;
                    args[Constants.Params.DEVICE_ID] = LeanplumRequest.DeviceId;
                    socketIOClient.Emit("auth", args);
                    authSent = true;
                }
            }
        }

        private void OnSocketMessage(object obj, MessageEventArgs e)
        {
            if (e.Message.MessageType == SocketIOMessageTypes.Event &&
                !String.IsNullOrEmpty(e.Message.MessageText))
            {
                IDictionary<string, object> messageReceived =
                    Json.Deserialize(e.Message.MessageText) as IDictionary<string, object>;
                string eventName = messageReceived.ContainsKey("name") ? messageReceived["name"] as string: "";

                if (eventName == "updateVars")
                {
                    onUpdateVars();
                }
                else if (eventName == "getVariables")
                {
                    bool sentValues = VarCache.SendVariablesIfChanged();
                    Dictionary<string, bool> response = new Dictionary<string, bool>();
                    response.Add("updated", sentValues);
                    socketIOClient.Emit("getContentResponse", response);
                }
                else if (eventName == "getActions")
                {
                    // Unsupported in LeanplumNative.
                    Dictionary<string, bool> response = new Dictionary<string, bool>();
                    response.Add("updated", false);
                    socketIOClient.Emit("getContentResponse", response);
                }
                else if (eventName == "registerDevice")
                {
                    IDictionary<string, object> packetData = (IDictionary<string, object>)
                        ((IList<object>) messageReceived["args"])[0];
                    string email = (string) packetData["email"];
                    LeanplumUnityHelper.QueueOnMainThread(() =>
                    {
                        LeanplumNative.OnHasStartedAndRegisteredAsDeveloper();
                        LeanplumNative.CompatibilityLayer.Log(
                            "Your device is registered to " + email + ".");
                    });
                }
            }
        }

        private void OnSocketConnectionClosed(object obj, EventArgs e)
        {
            if (connected)
            {
                LeanplumNative.CompatibilityLayer.Log("Disconnected from development server.");
                connected = false;
                connecting = false;
                authSent = false;
            }
        }
        
        private void OnSocketError(object obj, ErrorEventArgs e)
        {
            LeanplumNative.CompatibilityLayer.LogError(
                "Closing development socket with error: " + e.Message + ". If this problem " +
                "persists, please confirm that your Internet firewall allows WebSockets, " +
                "or try a different Internet connection.");
            socketIOClient.Close();
        }
    }
}
#endif
