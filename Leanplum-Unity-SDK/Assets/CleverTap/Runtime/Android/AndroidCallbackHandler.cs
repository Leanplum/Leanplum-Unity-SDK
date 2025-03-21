#if UNITY_ANDROID
using System;
using CleverTapSDK.Common;

namespace CleverTapSDK.Android
{
    public class AndroidCallbackHandler : CleverTapCallbackHandler
    {
        internal override void OnCallbackAdded(Action<string> callbackMethod)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("onCallbackAdded", callbackMethod.Method.Name);
        }


        internal override void OnVariablesCallbackAdded(Action<string> callbackMethod, int callbackId)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("onVariablesCallbackAdded", callbackMethod.Method.Name, callbackId);
        }

        internal override void OnVariablesCallbackRemoved(int callbackId)
        {
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("onVariablesCallbackRemoved", callbackId);
        }
    }
}
#endif