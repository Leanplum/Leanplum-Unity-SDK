#if UNITY_ANDROID
using System;
using CleverTapSDK.Common;

namespace CleverTapSDK.Android
{
    public class AndroidCallbackHandler : CleverTapCallbackHandler
    {
        internal override void OnCallbackAdded(Action<string> methodCallback)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("onCallbackAdded", methodCallback.Method.Name);
        }
    }
}
#endif