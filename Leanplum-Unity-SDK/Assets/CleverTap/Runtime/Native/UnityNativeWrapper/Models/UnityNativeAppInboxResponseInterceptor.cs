#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR 
using System.Collections.Generic;
using System.Net;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native
{
    internal class UnityNativeAppInboxResponseInterceptor : IUnityNativeResponseInterceptor
    {
        private List<object> appInboxMessages = null;

        // Internal property to expose inbox messages for unit testing
        internal List<object> AppInboxMessages => appInboxMessages;

        private readonly UnityNativeEventManager _unityNativeEventManager = null;

        public UnityNativeAppInboxResponseInterceptor(UnityNativeEventManager unityNativeEventManager)
        {
            _unityNativeEventManager = unityNativeEventManager ?? throw new System.ArgumentNullException(nameof(unityNativeEventManager));
        }

        UnityNativeResponse IUnityNativeResponseInterceptor.Intercept(UnityNativeResponse response)
        {
            this.appInboxMessages = null;

            if (response == null || response.StatusCode != HttpStatusCode.OK || string.IsNullOrEmpty(response.Content))
            {
                // Response or response content is null. This is the case for the handshake response.
                return response;
            }

            Dictionary<string, object> result = null;

            try
            {
                result = Json.Deserialize(response.Content) as Dictionary<string, object>;
            }
            catch (System.Exception ex)
            {
                CleverTapLogger.LogError($"Failed to deserialize response content: {ex.Message}");
                return response;
            }

            if (result != null && result.ContainsKey(UnityNativeConstants.AppInbox.INBOX_NOTIFS_KEY))
            {
                if (result[UnityNativeConstants.AppInbox.INBOX_NOTIFS_KEY] is List<object> messages)
                {
                    if (messages == null || messages.Count == 0)
                    {
                        return response;
                    }
                    else
                    {
                        HandleAppInboxMessages(messages);
                    }
                }
            }

            return response;
        }

        public void HandleAppInboxMessages(List<object> appInboxMessages)
        {
            this.appInboxMessages = appInboxMessages;
            _unityNativeEventManager?.OnInboxMessagesReceived(appInboxMessages);
        }
    }
}
#endif