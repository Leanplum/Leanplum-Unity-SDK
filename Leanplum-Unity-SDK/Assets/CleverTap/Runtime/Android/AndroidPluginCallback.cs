#if UNITY_ANDROID
namespace CleverTapSDK.Android
{
    internal delegate void PluginCallbackDelegate<T>(T value);

    internal class AndroidPluginCallback : UnityEngine.AndroidJavaProxy
    {
        private readonly PluginCallbackDelegate<string> CallbackDelegate;

        internal AndroidPluginCallback(PluginCallbackDelegate<string> callbackDelegate) : base("com.clevertap.unity.callback.PluginCallback")
        {
            CallbackDelegate = callbackDelegate;
        }

        internal void Invoke(string message)
        {
            CallbackDelegate?.Invoke(message);
        }
    }

    internal class AndroidPluginIntCallback : UnityEngine.AndroidJavaProxy
    {
        private readonly PluginCallbackDelegate<int> CallbackDelegate;

        internal AndroidPluginIntCallback(PluginCallbackDelegate<int> callbackDelegate) : base("com.clevertap.unity.callback.PluginIntCallback")
        {
            CallbackDelegate = callbackDelegate;
        }

        internal void Invoke(int value)
        {
            CallbackDelegate?.Invoke(value);
        }
    }
}
#endif
