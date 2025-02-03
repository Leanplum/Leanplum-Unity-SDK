#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using CleverTapSDK.Common;

namespace CleverTapSDK.Native {
    public class UnityNativeCallbackHandler : CleverTapCallbackHandler {
    }
}
#endif