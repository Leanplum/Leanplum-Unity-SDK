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
#if UNITY_IPHONE
using System.Collections.Generic;
using LeanplumSDK.MiniJSON;
using System.Runtime.InteropServices;
using System.Linq;

namespace LeanplumSDK
{
    public class LeanplumInboxApple : LeanplumInbox
    {
        public override event OnInboxChanged InboxChanged;
        public override event OnForceContentUpdate ForceContentUpdate;

        public event OnForceContentUpdate OneTimeUpdate;

        [DllImport("__Internal")]
        internal static extern int lp_inbox_count();

        [DllImport("__Internal")]
        internal static extern int lp_inbox_unreadCount();

        [DllImport("__Internal")]
        internal static extern string lp_inbox_messageIds();

        [DllImport("__Internal")]
        internal static extern string lp_inbox_messages();

        [DllImport("__Internal")]
        internal static extern void lp_inbox_read(string messageId);

        [DllImport("__Internal")]
        internal static extern void lp_inbox_markAsRead(string messageId);

        [DllImport("__Internal")]
        internal static extern void lp_inbox_remove(string messageId);

        [DllImport("__Internal")]
        internal static extern int lp_inbox_disableImagePrefetching();

        [DllImport("__Internal")]
        internal static extern void lp_inbox_downloadMessages();

        [DllImport("__Internal")]
        internal static extern void lp_inbox_downloadMessagesWithCallback();

        internal LeanplumInboxApple()
        {

        }

        public override int Count
        {
            get
            {
                return lp_inbox_count();
            }
        }

        public override int UnreadCount
        {
            get
            {
                return lp_inbox_unreadCount();
            }
        }

        public override List<string> MessageIds
        {
            get
            {
                var ids = (List<object>)Json.Deserialize(lp_inbox_messageIds());
                return ids.OfType<string>().ToList();
            }
        }

        public override List<Message> Messages
        {
            get
            {
                var json = lp_inbox_messages();
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

        public override void DownloadMessages()
        {
            lp_inbox_downloadMessages();
        }

        public override void DownloadMessages(OnForceContentUpdate completedHandler)
        {
            OneTimeUpdate = completedHandler;
            lp_inbox_downloadMessagesWithCallback();
        }

        public override void Read(string messageId)
        {
            if (messageId != null)
            {
                lp_inbox_read(messageId);
            }
        }

        public override void Read(Message message)
        {
            if (message != null)
            {
                Read(message.Id);
            }
        }

        public override void MarkAsRead(string messageId)
        {
            if (messageId != null)
            {
                lp_inbox_markAsRead(messageId);
            }

            InboxChanged?.Invoke();
        }

        public override void MarkAsRead(Message message)
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
                lp_inbox_remove(messageId);
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
            lp_inbox_disableImagePrefetching();
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
            else if (message.StartsWith("InboxDownloadMessages:"))
            {
                bool success = message.EndsWith("1");
                OneTimeUpdate?.Invoke(success);
                OneTimeUpdate = null;
            }
        }
    }
}

#endif
