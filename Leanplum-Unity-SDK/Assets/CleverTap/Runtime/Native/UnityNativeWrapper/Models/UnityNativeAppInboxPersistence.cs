#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native
{
    /// <summary>
    /// Handles persistent storage of inbox messages in Unity using a preference manager.
    /// Supports saving, retrieving, deleting, and tracking read/unread status.
    /// </summary>
    internal static class UnityNativeAppInboxPersistence
    {
        #region Private Static Variables

        /// <summary>
        /// Reference to the Unity preference manager used for storage.
        /// </summary>
        private static UnityNativePreferenceManager _preferenceManager = null;
        private static string _readPrefixKey = null;
        private static string _msgPrefixKey = null;
        private static string _inboxIdsKey = null;
        #endregion

        #region Public Static Methods

        /// <summary>
        /// Initializes the persistence manager with the provided preference manager instance.
        /// Must be called before using any persistence methods.
        /// </summary>
        /// <param name="preferenceManager">The preference manager used to persist data.</param>
        /// <param name="deviceId">A unique identifier for the device.</param>
        public static void Initialize(UnityNativePreferenceManager preferenceManager, string deviceId)
        {
            if (preferenceManager == null || string.IsNullOrEmpty(deviceId))
            {
                CleverTapLogger.LogError("UnityNativeAppInboxPersistence: preferenceManager or deviceId is null");
                return;
            }

            _preferenceManager = preferenceManager;
            _inboxIdsKey = $"INBOX_IDS_KEY_{deviceId}";
            _readPrefixKey = $"READ_PREFIX_{deviceId}";
            _msgPrefixKey = $"MSG_PREFIX_{deviceId}";
        }

        /// <summary>
        /// Updates internal keys used for storing and retrieving inbox-related data
        /// based on the provided device ID. This ensures all preferences are scoped
        /// to the current device.
        /// </summary>
        /// <param name="deviceId">
        /// The unique identifier for the device. Must not be null or empty.
        /// </param>
        public static void OnInboxStorageIdUpdate(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                CleverTapLogger.LogError("UnityNativeAppInboxPersistence: deviceId is null");
                return;
            }

            _inboxIdsKey = $"INBOX_IDS_KEY_{deviceId}";
            _readPrefixKey = $"READ_PREFIX_{deviceId}";
            _msgPrefixKey = $"MSG_PREFIX_{deviceId}";
        }

        /// <summary>
        /// Saves a message with the given ID and marks it as unread.
        /// Adds the message ID to the stored list if it's new.
        /// </summary>
        /// <param name="id">Unique identifier for the message.</param>
        /// <param name="message">Content of the message to be saved.</param>
        public static void SaveMessage(string id, string message)
        {
            if (!IsInitialized())
            {
                return;
            }

            _preferenceManager.SetString($"{_msgPrefixKey}_{id}", message);

            HashSet<string> ids = GetAllMessageIds();

            if (!ids.Contains(id))
            {
                ids.Add(id);
                SaveAllMessageIds(ids);
            }
        }

        /// <summary>
        /// Retrieves the message content for the given ID.
        /// </summary>
        /// <param name="id">The ID of the message to retrieve.</param>
        /// <returns>The message content, or null if not found or not initialized.</returns>
        public static string GetMessage(string id)
        {
            if (!IsInitialized())
            {
                return null;
            }

            return _preferenceManager.GetString($"{_msgPrefixKey}_{id}", string.Empty);
        }

        /// <summary>
        /// Marks a message as read using the given ID.
        /// </summary>
        /// <param name="id">The ID of the message to mark as read.</param>
        public static void MarkAsRead(string id)
        {
            if (!IsInitialized())
            {
                return;
            }

            string markId = $"{_readPrefixKey}_{id}";
            string msgId = $"{_msgPrefixKey}_{id}";
            string msg = _preferenceManager.GetString(msgId, string.Empty);

            if (!string.IsNullOrEmpty(msg))
            {
                // Updating the message to mark it as read
                Dictionary<string, object> message = Json.Deserialize(msg) as Dictionary<string, object>;
                
                if (message == null)
                {
                    CleverTapLogger.LogError($"Failed to deserialize message with id: {id}");
                    return;
                }

                message["isRead"] = true;
                _preferenceManager.SetString(msgId, Json.Serialize(message));
                _preferenceManager.SetInt(markId, 1);
            }
        }

        /// <summary>
        /// Checks whether a message is marked as read.
        /// </summary>
        /// <param name="id">The ID of the message to check.</param>
        /// <returns>True if the message is read, false otherwise.</returns>
        public static bool IsRead(string id)
        {
            if (!IsInitialized())
            {
                return false;
            }

            return _preferenceManager.GetInt($"{_readPrefixKey}_{id}", 0) == 1;
        }

        /// <summary>
        /// Deletes a message and its read status, and removes the ID from the stored list.
        /// </summary>
        /// <param name="id">The ID of the message to delete.</param>
        public static void DeleteMessage(string id)
        {
            if (!IsInitialized())
            {
                return;
            }

            _preferenceManager.DeleteKey($"{_msgPrefixKey}_{id}");
            _preferenceManager.DeleteKey($"{_readPrefixKey}_{id}");

            var ids = GetAllMessageIds();

            if (ids.Remove(id))
            {
                SaveAllMessageIds(ids);
            }
        }


        /// <summary>
        /// Deletes only the specified message IDs.
        /// </summary>
        public static void DeleteMessagesForIds(string[] ids)
        {
            if (!IsInitialized())
            {
                return;
            }

            for (int i = 0, count = ids.Length; i < count; i++)
            {
                DeleteMessage(ids[i]);
            }
        }

        /// <summary>
        /// Retrieves all stored messages.
        /// </summary>
        /// <returns>A dictionary containing all messages keyed by their IDs.</returns>
        public static Dictionary<string, string> GetAllMessages()
        {
            if (!IsInitialized())
            {
                return null;
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();

            HashSet<string> ids = GetAllMessageIds();

            foreach (string id in ids)
            {
                string msg = GetMessage(id);

                if (!string.IsNullOrEmpty(msg))
                {
                    dict[id] = msg;
                }
            }

            return dict;
        }

        /// <summary>
        /// Marks all stored messages as read.
        /// </summary>
        public static void MarkAllAsRead()
        {
            if (!IsInitialized())
            {
                return;
            }

            HashSet<string> ids = GetAllMessageIds();

            foreach (string id in ids)
            {
                MarkAsRead(id);
            }
        }

        /// <summary>
        /// Marks only the specified message IDs as read.
        /// </summary>
        public static void MarkMessagesAsReadForIds(string[] ids)
        {
            if (!IsInitialized())
            {
                return;
            }

            for (int i = 0, count = ids.Length; i < count; i++)
            {
                MarkAsRead(ids[i]);
            }
        }

        /// <summary>
        /// Deletes all stored messages and their metadata (IDs and read flags).
        /// </summary>
        public static void DeleteAllMessages()
        {
            if (!IsInitialized())
            {
                return;
            }

            HashSet<string> ids = GetAllMessageIds();

            foreach (string id in ids)
            {
                _preferenceManager.DeleteKey($"{_msgPrefixKey}_{id}");
                _preferenceManager.DeleteKey($"{_readPrefixKey}_{id}");
            }

            _preferenceManager.DeleteKey(_inboxIdsKey);
        }

        /// <summary>
        /// Retrieves only the unread messages.
        /// </summary>
        /// <returns>A dictionary of unread messages keyed by their IDs.</returns>
        public static Dictionary<string, string> GetUnreadMessages()
        {
            if (!IsInitialized())
            {
                return null;
            }

            var allMessages = GetAllMessages();
            var unreadMessages = new Dictionary<string, string>();

            foreach (var kvp in allMessages)
            {
                if (!IsRead(kvp.Key))
                {
                    unreadMessages[kvp.Key] = kvp.Value;
                }
            }

            return unreadMessages;
        }
        #endregion

        #region Private Static Methods

        /// <summary>
        /// Retrieves all stored message IDs as a HashSet.
        /// </summary>
        /// <returns>
        /// A HashSet containing all message IDs. Returns an empty HashSet if no IDs are stored,
        /// </returns>

        private static HashSet<string> GetAllMessageIds()
        {
            if (!IsInitialized())
            {
                return null;
            }

            string joined = _preferenceManager.GetString(_inboxIdsKey, string.Empty);

            if (string.IsNullOrEmpty(joined))
            {
                return new HashSet<string>();
            }

            return new HashSet<string>(joined.Split(','));
        }

        /// <summary>
        /// Stores the given list of message IDs as a comma-separated string.
        /// </summary>
        /// <param name="ids">List of message IDs to store.</param>
        private static void SaveAllMessageIds(HashSet<string> ids)
        {
            if (!IsInitialized())
            {
                return;
            }

            string joined = string.Join(",", ids);
            _preferenceManager.SetString(_inboxIdsKey, joined);
        }

        /// <summary>
        /// Checks if the UnityNativeInboxPersistence class has been properly initialized
        /// with a valid <see cref="UnityNativePreferenceManager"/>.
        /// Logs an error if not initialized.
        /// </summary>
        /// <returns>True if initialized; otherwise, false.</returns>
        private static bool IsInitialized()
        {
            if (_preferenceManager == null)
            {
                CleverTapLogger.LogError("UnityNativeInboxPersistence is not initialized. Call Initialize() first.");
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}
#endif