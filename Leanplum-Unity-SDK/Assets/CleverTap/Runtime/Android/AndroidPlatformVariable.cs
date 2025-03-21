#if UNITY_ANDROID
using System;
using CleverTapSDK.Common;
using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Android {
    internal class AndroidPlatformVariable : CleverTapPlatformVariable {
        internal override void SyncVariables() =>
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("syncVariables");

        internal override void SyncVariables(bool isProduction) =>
            SyncVariables();

        internal override void FetchVariables(int callbackId) =>
            CleverTapAndroidJNI.CleverTapJNIInstance.Call("fetchVariables", callbackId);

        protected override Var<T> DefineVariable<T>(string name, string kind, T defaultValue) {
            if (kind == CleverTapVariableKind.FILE) {
                CleverTapAndroidJNI.CleverTapJNIInstance.Call("defineFileVariable", name);
            } else {
                CleverTapAndroidJNI.CleverTapJNIInstance.Call("defineVar", name, kind, Json.Serialize(defaultValue));
            }
            Var<T> result = new AndroidVar<T>(name, kind, defaultValue);
            varCache.Add(name, result);
            return result;
        }
    }
}
#endif