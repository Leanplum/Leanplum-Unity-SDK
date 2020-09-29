//
// Copyright 2020, Leanplum, Inc.
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
using LeanplumSDK.MiniJSON;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

#if UNITY_IPHONE

namespace LeanplumSDK
{
    public class LeanplumInboxiOS : LeanplumInbox
    {
        public override event OnInboxChanged InboxChanged;
        public override event OnForceContentUpdate ForceContentUpdate;

        [DllImport("__Internal")]
        internal static extern int _inbox_count();

        [DllImport("__Internal")]
        internal static extern int _inbox_unreadCount();

        [DllImport("__Internal")]
        internal static extern string _inbox_messageIds();

        [DllImport("__Internal")]
        internal static extern string _inbox_messages();

        [DllImport("__Internal")]
        internal static extern void _inbox_read(string messageId);

        [DllImport("__Internal")]
        internal static extern void _inbox_markAsRead(string messageId);

        [DllImport("__Internal")]
        internal static extern void _inbox_remove(string messageId);

        [DllImport("__Internal")]
        internal static extern int _inbox_disableImagePrefetching();

        internal LeanplumInboxiOS()
        {

        }

        public override int Count
        {
            get
            {
                return _inbox_count();
            }
        }

        public override int UnreadCount
        {
            get
            {
                return _inbox_unreadCount();
            }
        }

        public override List<string> MessageIds
        {
            get
            {
                var ids = (List<object>)Json.Deserialize(_inbox_messageIds());
                return ids.OfType<string>().ToList();
            }
        }

        public override List<Message> Messages
        {
            get
            {
                var json = _inbox_messages();
                return ParseMessages(json);
            }
        }

        public override List<Message> UnreadMessages
        {
            get
            {
                return Messages.FindAll(msg => {
                    return msg.IsRead == false;
                }).ToList();
            }
        }

        public override void Read(string messageId)
        {
            if (messageId != null)
            {
                _inbox_read(messageId);
            }
        }

        public override void Read(Message message)
        {
            if (message != null)
            {
                Read(message.Id);
            }
        }

        internal override void MarkAsRead(string messageId)
        {
            if (messageId != null)
            {
                _inbox_markAsRead(messageId);
            }

            InboxChanged?.Invoke();
        }

        internal override void MarkAsRead(Message message)
        {
            if (message != null)
            {
                MarkAsRead(message.Id);
            }
        }

        public override void Remove(string messageId)
        {
            if (messageId != null)
            {
                _inbox_remove(messageId);
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
            _inbox_disableImagePrefetching();
        }

        internal override void NativeCallback(string message)
        {
            if (message.StartsWith("InboxOnChanged"))
            {
                InboxChanged?.Invoke();
            }
            else if (message.StartsWith("InboxForceContentUpdate:"))
            {
                bool success = message.EndsWith("1");
                ForceContentUpdate?.Invoke(success);
            }
        }
    }
}

#endif
