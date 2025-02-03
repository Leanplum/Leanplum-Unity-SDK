#if UNITY_IOS
using System;
using CleverTapSDK.Common;

namespace CleverTapSDK.IOS {
    public class IOSCallbackHandler : CleverTapCallbackHandler
    {
        internal override void OnCallbackAdded(Action<string> callbackMethod)
        {
            IOSDllImport.CleverTap_onCallbackAdded(callbackMethod.Method.Name);
        }

        internal delegate void InAppNotificationButtonTapped(string customData);

        internal delegate void UserEventLogCallback(string key, string message);
    }
}
#endif