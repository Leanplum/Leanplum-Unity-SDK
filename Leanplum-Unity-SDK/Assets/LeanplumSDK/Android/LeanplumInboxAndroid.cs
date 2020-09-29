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
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LeanplumSDK.MiniJSON;

#if UNITY_ANDROID

namespace LeanplumSDK
{
    public class LeanplumInboxAndroid : LeanplumInbox
    {
        public override event OnInboxChanged InboxChanged;
        public override event OnForceContentUpdate ForceContentUpdate;

        private AndroidJavaClass nativeSdk = null;

        internal LeanplumInboxAndroid(AndroidJavaClass native)
        {
            nativeSdk = native;
        }

        public override int Count
        {
            get
            {
                return nativeSdk.CallStatic<int>("inboxCount");
            }
        }

        public override int UnreadCount
        {
            get
            {
                return nativeSdk.CallStatic<int>("inboxUnreadCount");
            }
        }

        public override List<string> MessageIds
        {
            get
            {
                var ids = (List<object>) Json.Deserialize(nativeSdk.CallStatic<string>("inboxMessageIds"));
                return ids.OfType<string>().ToList();
            }
        }

        public override List<Message> Messages
        {
            get
            {
                var json = nativeSdk.CallStatic<string>("inboxMessages");
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
                nativeSdk.CallStatic("inboxRead", messageId);
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
                nativeSdk.CallStatic("inboxMarkAsRead", messageId);
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
                nativeSdk.CallStatic("inboxRemove", messageId);
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
            nativeSdk.CallStatic("inboxDisableImagePrefetching");
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
