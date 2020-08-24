
using System;
using System.Collections.Generic;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK
{
    public abstract class LeanplumInbox
    {
        public delegate void OnInboxChanged();
        public delegate void OnForceContentUpdate(bool success);

        public abstract event OnInboxChanged InboxChanged;
        public abstract event OnForceContentUpdate ForceContentUpdate;

        public class LeanplumMessage
        {
            public string Id;
            public string Title;
            public string Subtitle;
            public string ImageFilePath;
            public string ImageURL;
            public DateTime? ExpirationTimestamp = null;
            public DateTime? DeliveryTimestamp = null;
            public bool IsRead;

            public override string ToString()
            {
                return string.Format("id: {0}, " +
                    "title: {1}, " +
                    "subtitle: {2}, " +
                    "imageFilePath: {3}, " +
                    "imageURL: {4}, " +
                    "expirationTimestamp: {5}, " +
                    "deliveryTimestamp: {6}, " +
                    "isRead: {7}",
                    Id, Title, Subtitle, ImageFilePath, ImageURL, ExpirationTimestamp, DeliveryTimestamp, IsRead);
            }
        }

        public abstract int Count { get; }
        public abstract int UnreadCount { get; }
        public abstract List<string> MessageIds { get; }
        public abstract List<LeanplumMessage> Messages { get; }
        public abstract List<LeanplumMessage> UnreadMessages { get; }

        public abstract void Read(string messageId);
        public abstract void Read(LeanplumMessage message);

        public abstract void MarkAsRead(string messageId);
        public abstract void MarkAsRead(LeanplumMessage message);

        public abstract void Remove(string messageId);
        public abstract void Remove(LeanplumMessage message);

        internal abstract void NativeCallback(string message);

        internal List<LeanplumMessage> ParseMessages(string json)
        {
            var msgs = (List<object>) Json.Deserialize(json);
            var messages = new List<LeanplumMessage>();

            foreach (var msg in msgs)
            {
                var dict = msg as Dictionary<string, object>;
                var leanpluMessage = new LeanplumMessage();

                if (dict.TryGetValue("id", out var id))
                {
                    leanpluMessage.Id = id as string;
                }
                if (dict.TryGetValue("title", out var title))
                {
                    leanpluMessage.Title = title as string;
                }
                if (dict.TryGetValue("subtitle", out var subtitle))
                {
                    leanpluMessage.Subtitle = subtitle as string;
                }
                if (dict.TryGetValue("imageFilePath", out var imageFilePath))
                {
                    if (imageFilePath is string value)
                    {
                        leanpluMessage.ImageFilePath = value;
                    }
                }
                if (dict.TryGetValue("imageURL", out var imageURL))
                {
                    if (imageURL is string value)
                    {
                        leanpluMessage.ImageURL = value;
                    }
                }
                if (dict.TryGetValue("deliveryTimestamp", out var deliveryTimestamp))
                {
                    if (deliveryTimestamp is string value)
                    {
                        leanpluMessage.DeliveryTimestamp = DateTime.Parse(value);
                    }
                }
                if (dict.TryGetValue("expirationTimestamp", out var expirationTimestamp))
                {
                    if (expirationTimestamp is string value)
                    {
                        leanpluMessage.ExpirationTimestamp = DateTime.Parse(value);
                    }
                }
                if (dict.TryGetValue("isRead", out var isRead))
                {
                    if (isRead is bool value)
                    {
                        leanpluMessage.IsRead = value;
                    }
                }

                messages.Add(leanpluMessage);
            }
            return messages;
        }
    }
}