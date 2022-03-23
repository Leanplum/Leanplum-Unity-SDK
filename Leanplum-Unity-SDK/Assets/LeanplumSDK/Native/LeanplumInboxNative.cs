//
// Copyright 2022, Leanplum, Inc.
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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LeanplumSDK
{
    public class LeanplumInboxNative : LeanplumInbox
    {
        internal bool _isImagePrefetchingDisabled = false;
        internal List<Message> _messages = new List<Message>();

        public override event OnInboxChanged InboxChanged;
        public override event OnForceContentUpdate ForceContentUpdate;

        internal LeanplumInboxNative()
        {

        }

        public override int Count
        {
            get
            {
                return Messages.Count();
            }
        }

        public override int UnreadCount
        {
            get
            {
                return UnreadMessages.Count();
            }
        }

        public override List<string> MessageIds
        {
            get
            {
                return _messages.Select(msg => msg.Id)
                                .ToList();
            }
        }

        public override List<Message> Messages
        {
            get
            {
                return _messages;
            }
        }

        public override List<Message> UnreadMessages
        {
            get
            {
                return Messages.FindAll(msg => msg.IsRead == false)
                               .ToList();
            }
        }

        public override void Read(string messageId)
        {
            if (messageId != null)
            {
                var msg = _messages.Find(message => message.Id == messageId);

                if (msg != null)
                {
                    Read(msg);
                }
            }
        }

        public override void Read(Message message)
        {
            if (message != null)
            {
                MarkAsRead(message);
                Open(message);
            }
        }

        public override void MarkAsRead(string messageId)
        {
            if (messageId != null)
            {
                var msg = _messages.Find(message => message.Id == messageId);

                if (msg != null)
                {
                    MarkAsRead(msg);
                }
            }
        }

        public override void MarkAsRead(Message message)
        {
            if (message != null)
            {
                message.IsRead = true;
                Request request = RequestBuilder.withMarkInboxMessageAsReadAction()
                    .AndParam(Constants.Params.INBOX_MESSAGE_ID, message.Id)
                    .Create();
                Leanplum.RequestSender.Send(request);
            }
            InboxChanged?.Invoke();
        }

        public override void Remove(string messageId)
        {
            if (messageId != null)
            {
                var msgs = _messages.FindAll(msg => msg.Id != messageId).ToList();

                UpdateMessages(msgs);

                Request request = RequestBuilder.withDeleteInboxMessageAction()
                    .AndParam(Constants.Params.INBOX_MESSAGE_ID, messageId)
                    .Create();
                Leanplum.RequestSender.Send(request);
            }
        }

        public override void Remove(Message message)
        {
            if (message != null)
            {
                Remove(message.Id);
            }
        }

        public override void DisableImagePrefetching()
        {
            _isImagePrefetchingDisabled = true;
        }

        internal override void NativeCallback(string message)
        {
            // ignored
        }


        public override void DownloadMessages()
        {
            DownloadMessages(null);
        }

        public override void DownloadMessages(OnForceContentUpdate completedHandler)
        {
            Request request = RequestBuilder.withGetInboxMessagesAction().CreateImmediate();
            request.Response += delegate (object data)
            {
                try
                {
                    var response = Util.GetLastResponse(data) as IDictionary<string, object>;
                    var messages = Util.GetValueOrDefault(response, Constants.Keys.INBOX_MESSAGES) as IDictionary<string, object>;
                    var inboxMessages = new List<Message>();
                    if (messages != null)
                    {
                        foreach (var pair in messages)
                        {
                            var id = pair.Key;
                            if (pair.Value is IDictionary<string, object> value)
                            {
                                var message = new Message
                                {
                                    Id = id
                                };

                                if (value.TryGetValue(Constants.Keys.MESSAGE_IS_READ, out var isRead))
                                {
                                    if (isRead is bool b)
                                    {
                                        message.IsRead = b;
                                    }
                                }

                                if (value.TryGetValue(Constants.Keys.DELIVERY_TIMESTAMP, out var deliveryTimestamp))
                                {
                                    if (deliveryTimestamp is long l)
                                    {
                                        TimeSpan ts = TimeSpan.FromMilliseconds(l);
                                        DateTime date = new DateTime(1970, 1, 1).AddTicks(ts.Ticks);
                                        message.DeliveryTimestamp = date;
                                    }
                                }

                                if (value.TryGetValue(Constants.Keys.EXPIRATION_TIMESTAMP, out var expirationTimestamp))
                                {
                                    if (expirationTimestamp is long l)
                                    {
                                        TimeSpan ts = TimeSpan.FromMilliseconds(l);
                                        DateTime date = new DateTime(1970, 1, 1).AddTicks(ts.Ticks);
                                        message.ExpirationTimestamp = date;
                                    }
                                }

                                if (value.TryGetValue(Constants.Keys.MESSAGE_DATA, out var messageData))
                                {
                                    if (messageData is IDictionary<string, object> dict)
                                    {
                                        if (dict.TryGetValue(Constants.Keys.VARS, out var vars))
                                        {
                                            if (vars is Dictionary<string, object> varsDict)
                                            {
                                                message.ActionContext = varsDict;

                                                if (varsDict.TryGetValue(Constants.Keys.TITLE, out var title))
                                                {
                                                    message.Title = title as string;
                                                }

                                                if (varsDict.TryGetValue(Constants.Keys.SUBTITLE, out var subtitle))
                                                {
                                                    message.Subtitle = subtitle as string;
                                                }

                                                if (varsDict.TryGetValue(Constants.Keys.IMAGE, out var image))
                                                {
                                                    message.ImageURL = image as string;
                                                }

                                                if (varsDict.TryGetValue(Constants.Keys.DATA, out var advancedData))
                                                {
                                                    message.Data = advancedData as IDictionary<string, object>;
                                                }
                                            }
                                        }
                                    }
                                }

                                inboxMessages.Add(message);
                            }
                        }
                    }
                    UpdateMessages(inboxMessages);
                    TriggerInboxSyncedWithStatus(true, completedHandler);
                }
                catch (Exception exception)
                {
                    Debug.Log("exception getting messages: " + exception.Message);
                    TriggerInboxSyncedWithStatus(false, completedHandler);
                }
            };

            request.Error += delegate
            {
                TriggerInboxSyncedWithStatus(false, completedHandler);
            };

            Leanplum.RequestSender.Send(request);
        }

        internal void Open(Message message)
        {
            var messageVars = message.ActionContext;
            var openAction = Util.GetValueOrDefault(messageVars, Constants.Args.OPEN_ACTION) as IDictionary<string, object>;
            if (openAction != null)
            {
                // The inbox message id is with format <messageId>##<instance>
                string id = message.Id.Split(new string[] { "##" }, StringSplitOptions.RemoveEmptyEntries)[0];

                string actionName = Util.GetValueOrDefault(messageVars, Constants.Args.ACTION_NAME, string.Empty)?.ToString();

                var actionContext = new NativeActionContext(id, actionName, messageVars);
                actionContext.RunTrackedActionNamed(Constants.Args.OPEN_ACTION);
            }
        }

        internal void UpdateMessages(List<Message> messages)
        {
            lock(_messages)
            {
                _messages = messages;
                Save();
                InboxChanged?.Invoke();
            }
        }

        internal void Save()
        {
            var msgs = new List<IDictionary<string, object>>();
            foreach (var msg in _messages)
            {
                var msgData = new Dictionary<string, object>();
                msgData[Constants.Keys.ID] = msg.Id;
                msgData[Constants.Keys.TITLE] = msg.Title;
                msgData[Constants.Keys.SUBTITLE] = msg.Subtitle;
                msgData[Constants.Keys.MESSAGE_IS_READ] = msg.IsRead;
                msgData[Constants.Keys.IMAGE] = msg.ImageURL;
                if (msg.DeliveryTimestamp.HasValue)
                {
                    msgData[Constants.Keys.DELIVERY_TIMESTAMP] = 
                        Util.GetUnixTimestampFromDate(msg.DeliveryTimestamp.Value);
                }
                if (msg.ExpirationTimestamp.HasValue)
                {
                    msgData[Constants.Keys.EXPIRATION_TIMESTAMP] = 
                        Util.GetUnixTimestampFromDate(msg.ExpirationTimestamp.Value);
                }
                msgData[Constants.Keys.MESSAGE_DATA] = msg.ActionContext;

                msgs.Add(msgData);
            }

            var json = MiniJSON.Json.Serialize(msgs);
            LeanplumNative.CompatibilityLayer.StoreSavedString(Constants.Defaults.APP_INBOX_MESSAGES_KEY, json);
            LeanplumNative.CompatibilityLayer.FlushSavedSettings();
        }

        internal void Load()
        {
            var inboxMessage = new List<Message>();
            var json = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.APP_INBOX_MESSAGES_KEY, "{}");
            if (json != null)
            {
                var msgs = MiniJSON.Json.Deserialize(json) as List<object>;
                if (msgs == null) return;

                foreach (IDictionary<string, object> msgData in msgs)
                {
                    var message = new Message();

                    if (msgData.TryGetValue(Constants.Keys.ID, out var id))
                    {
                        message.Id = id as string;
                    }
                    if (msgData.TryGetValue(Constants.Keys.TITLE, out var title))
                    {
                        message.Title = title as string;
                    }
                    if (msgData.TryGetValue(Constants.Keys.SUBTITLE, out var subtitle))
                    {
                        message.Subtitle = subtitle as string;
                    }
                    if (msgData.TryGetValue(Constants.Keys.MESSAGE_IS_READ, out var isRead))
                    {
                        if (isRead is bool b)
                        {
                            message.IsRead = b;
                        }
                    }
                    if (msgData.TryGetValue(Constants.Keys.IMAGE, out var image))
                    {
                        message.ImageURL = image as string;
                    }
                    if (msgData.TryGetValue(Constants.Keys.DELIVERY_TIMESTAMP, out var deliveryTimestamp))
                    {
                        if (deliveryTimestamp is long l)
                        {
                            message.DeliveryTimestamp = Util.GetDateFromUnixTimestamp(l);
                        }
                    }
                    if (msgData.TryGetValue(Constants.Keys.EXPIRATION_TIMESTAMP, out var expirationTimestamp))
                    {
                        if (expirationTimestamp is long l)
                        {
                            message.ExpirationTimestamp = Util.GetDateFromUnixTimestamp(l);
                        }
                    }
                    if (msgData.TryGetValue(Constants.Keys.MESSAGE_DATA, out var actionContext))
                    {
                        message.ActionContext = actionContext as IDictionary<string, object>;
                        if (message.ActionContext != null &&
                            message.ActionContext.TryGetValue(Constants.Keys.DATA, out var advancedData))
                        {
                            message.Data = advancedData as IDictionary<string, object>;
                        }
                    }

                    inboxMessage.Add(message);
                }

                // remove inactive messages
                inboxMessage = inboxMessage.FindAll(msg => msg.IsActive() == true).ToList();

                UpdateMessages(inboxMessage);
            }
        }

        private void TriggerInboxSyncedWithStatus(bool status, OnForceContentUpdate completedHandler)
        {
            completedHandler?.Invoke(status);
            ForceContentUpdate?.Invoke(status);
        }
    }
}
