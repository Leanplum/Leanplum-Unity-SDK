#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using CleverTapSDK.Common;

namespace CleverTapSDK.Native {
    internal class UnityNativeVar<T> : Var<T> {
        internal UnityNativeVar(string name, string kind, T defaultValue) : base(name, kind, defaultValue) {
        }
    }
}
#endif