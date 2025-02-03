#if UNITY_ANDROID
using UnityEngine;

namespace CleverTapSDK.Android {
    internal static class CleverTapAndroidJNI {
        private const string UNITY_PLAYER_CLASS = "com.unity3d.player.UnityPlayer";
        private const string CURRENT_ACTIVITY = "currentActivity";
        private const string CLEVERTAP_UNITY_PLUGIN_CLASS = "com.clevertap.unity.CleverTapUnityPlugin";
        private const string GET_APPLICATION_CONTEXT_METHOD = "getApplicationContext";
        private const string GET_INSTANCE_METHOD = "getInstance";

        private static AndroidJavaObject _unityActivity;
        private static AndroidJavaObject _cleverTapJNI;
        private static AndroidJavaObject _cleverTapClass;

        internal delegate void OnInitCleverTapJNI(AndroidJavaObject cleverTapInstance);
        internal static OnInitCleverTapJNI OnInitCleverTapInstanceDelegate;

        internal static AndroidJavaObject UnityActivity {
            get {
                if (_unityActivity == null) {
                    using (AndroidJavaClass unityPlayer = new AndroidJavaClass(UNITY_PLAYER_CLASS)) {
                        _unityActivity = unityPlayer.GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY);
                    }
                }
                return _unityActivity;
            }
        }

        internal static AndroidJavaObject CleverTapJNIStatic {
            get {
                if (_cleverTapClass == null) {
                    _cleverTapClass = new AndroidJavaClass(CLEVERTAP_UNITY_PLUGIN_CLASS);
                }
                return _cleverTapClass;
            }
        }

        internal static AndroidJavaObject ApplicationContext =>
            UnityActivity.Call<AndroidJavaObject>(GET_APPLICATION_CONTEXT_METHOD);

        internal static AndroidJavaObject CleverTapJNIInstance {
            get {
                if (_cleverTapJNI == null) {
                    _cleverTapJNI = CleverTapJNIStatic.CallStatic<AndroidJavaObject>(GET_INSTANCE_METHOD, ApplicationContext);
                    OnInitCleverTapInstanceDelegate?.Invoke(_cleverTapJNI);
                }
                return _cleverTapJNI;
            }
        }
    }
}
#endif