
using System;
using System.Collections.Generic;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK
{
    /// <summary>
    /// The LeanplumInbox mirrors the native iOS and Android implementation in Unity.
    /// It is responsible for calling native methods and parsing the results exposing
    /// inbox and its methods to Unity.
    /// 
    /// Some of the methods will be platform dependent, i.e imageURLPath.
    /// </summary>
    public abstract class LeanplumInbox
    {
        public delegate void OnInboxChanged();
        public delegate void OnForceContentUpdate(bool success);

        /// <summary>
        /// Function to call when the inbox receive new values from the server.
        /// This will be called on start, and also later on if the user is in an experiment
        /// that can update in realtime.
        /// </summary>
        public abstract event OnInboxChanged InboxChanged;

        /// <summary>
        /// Function to call when ForceContentUpdate was called.
        /// </summary>
        /// <param name="success">Returns true if syncing was successful.</param>
        public abstract event OnForceContentUpdate ForceContentUpdate;

        /// <summary>
        /// Message class encapsulating a message delivered from platform.
        /// </summary>
        [Serializable]
        public class Message
        {
            /// <summary>
            /// Returns the message identifier of the inbox message.
            /// </summary>
            public string Id;

            /// <summary>
            /// Returns the title of the inbox message.
            /// </summary>
            public string Title;

            /// <summary>
            /// Returns the subtitle of the inbox message.
            /// </summary>
            public string Subtitle;

            /// <summary>
            /// Returns the image path of the inbox message. Can be nil.
            /// </summary>
            public string ImageFilePath;

            /// <summary>
            /// Returns the image URL of the inbox message.
            /// You can safely use this with prefetching enabled.
            /// It will return the file URL path instead if the image is in cache.
            /// </summary>
            public string ImageURL;

            /// <summary>
            /// Return the expiration timestamp of the inbox message.
            /// </summary>
            public DateTime? ExpirationTimestamp = null;

            /// <summary>
            /// Returns the delivery timestamp of the inbox message.
            /// </summary>
            public DateTime? DeliveryTimestamp = null;

            /// <summary>
            /// Returns true if the inbox message is read.
            /// </summary>
            public bool IsRead = false;

            /// <summary>
            /// ActionContext containing message data
            /// </summary>
            internal IDictionary<string, object> ActionContext = null;

            /// <summary>
            /// Checks whether the message is still active.
            /// </summary>
            /// <returns>true if it is, false otherwise</returns>
            internal bool IsActive()
            {
                if (ExpirationTimestamp == null)
                {
                    return true;
                }
                var now = DateTime.Now;
                return now.CompareTo(ExpirationTimestamp) < 0;
            }

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

        /// <summary>
        /// Returns the number of all inbox messages on the device.
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Returns the number of the unread inbox messages on the device.
        /// </summary>
        public abstract int UnreadCount { get; }

        /// <summary>
        /// Returns the identifiers of all inbox messages on the device sorted in ascending
        /// chronological order, i.e.the id of the oldest message is the first one, and the most
        /// recent one is the last one in the array.
        /// </summary>
        public abstract List<string> MessageIds { get; }

        /// <summary>
        /// Returns an array containing all of the inbox messages (as LeanplumMessage objects)
        /// on the device, sorted in ascending chronological order, i.e.the oldest message is the
        /// first one, and the most recent one is the last one in the array.
        /// </summary>
        public abstract List<Message> Messages { get; }

        /// <summary>
        /// Returns an array containing all of the unread inbox messages on the device, sorted
        /// in ascending chronological order, i.e.the oldest message is the first one, and the
        /// most recent one is the last one in the array.
        /// </summary>
        public abstract List<Message> UnreadMessages { get; }

        /// <summary>
        /// Returns the inbox message associated with the given messageId identifier.
        /// </summary>
        /// <param name="id">Id of the wanted message.</param>
        /// <returns>LeanplumMessage or null if not found.</returns>
        public Message MessageForId(string id)
        {
            return Messages.Find(msg =>
            {
                return msg.Id == id;
            });
        }

        /// <summary>
        /// Read the inbox message, marking it as read and invoking its open action.
        /// </summary>
        /// <param name="messageId">ID of the message to read.</param>
        public abstract void Read(string messageId);

        /// <summary>
        /// Read the inbox message, marking it as read and invoking its open action.
        /// </summary>
        /// <param name="message">LeanplumMessage to read.</param>
        public abstract void Read(Message message);

        /// <summary>
        /// Mark the inbox message as read without invoking its open action.
        /// </summary>
        /// <param name="messageId">ID of the message to read.</param>
        internal abstract void MarkAsRead(string messageId);

        /// <summary>
        /// Mark the inbox message as read without invoking its open action.
        /// </summary>
        /// <param name="message">LeanplumMessage to mark as read.</param>
        internal abstract void MarkAsRead(Message message);

        /// <summary>
        /// Remove the inbox message from the inbox.
        /// </summary>
        /// <param name="messageId">ID of the message to remove.</param>
        public abstract void Remove(string messageId);

        /// <summary>
        /// Remove the inbox message from the inbox.
        /// </summary>
        /// <param name="message">LeanplumMessage to remove.</param>
        public abstract void Remove(Message message);

        /// <summary>
        /// Call this method if you don't want Inbox images to be prefetched.
        /// Useful if you only want to deal with image URL.
        /// </summary>
        public abstract void DisableImagePrefetching();

        /// <summary>
        /// Invoked by the platform specific implementation to invoke callbacks
        /// </summary>
        /// <param name="message">Message to invoke.</param>
        internal abstract void NativeCallback(string message);

        /// <summary>
        /// Parses JSON containing message description from native platforms.
        /// </summary>
        /// <param name="json">JSON containing messages</param>
        /// <returns>List of LeanplumMessages</returns>
        internal List<Message> ParseMessages(string json)
        {
            var msgs = (List<object>) Json.Deserialize(json);
            var messages = new List<Message>();

            foreach (var msg in msgs)
            {
                var dict = msg as Dictionary<string, object>;
                var leanpluMessage = new Message();

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
