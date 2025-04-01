#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using CleverTapSDK.Common;
using UnityEngine;

namespace CleverTapSDK.Native
{
    internal class UnityNativeCallbackHandler : CleverTapCallbackHandler
    {
        [HideInInspector]
        public UnityNativePlatformVariable platformVariable;

        internal UnityNativeCallbackHandler() : base()
        {
        }

        bool ShouldInvokeVariablesCallbackImmediately => platformVariable?.HasVarsRequestCompleted ?? false;

        protected CleverTapCallbackDelegate _OnVariablesChanged;
        public override event CleverTapCallbackDelegate OnVariablesChanged
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnVariablesChanged += value;
                    if (ShouldInvokeVariablesCallbackImmediately)
                    {
                        value.Invoke();
                    }
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnVariablesChanged -= value;
                }
            }
        }

        public override void CleverTapVariablesChanged(string message)
        {
            _OnVariablesChanged?.Invoke();
        }

        protected CleverTapCallbackDelegate _OnOneTimeVariablesChanged;
        public override event CleverTapCallbackDelegate OnOneTimeVariablesChanged
        {
            add
            {
                lock (CallbackLock)
                {
                    if (ShouldInvokeVariablesCallbackImmediately)
                    {
                        value.Invoke();
                    }
                    else
                    {
                        _OnOneTimeVariablesChanged += value;
                    }
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnOneTimeVariablesChanged -= value;
                }
            }
        }

        public override void OneTimeCleverTapVariablesChanged(string message)
        {
            _OnOneTimeVariablesChanged?.Invoke();
        }

        protected CleverTapCallbackDelegate _OnVariablesChangedAndNoDownloadsPending;
        public override event CleverTapCallbackDelegate OnVariablesChangedAndNoDownloadsPending
        {
            add
            {
                lock (CallbackLock)
                {
                    _OnVariablesChangedAndNoDownloadsPending += value;
                    if (ShouldInvokeVariablesCallbackImmediately)
                    {
                        value.Invoke();
                    }
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnVariablesChangedAndNoDownloadsPending -= value;
                }
            }
        }

        public override void CleverTapVariablesChangedAndNoDownloadsPending(string message)
        {
            _OnVariablesChangedAndNoDownloadsPending?.Invoke();
        }

        protected CleverTapCallbackDelegate _OnOneTimeVariablesChangedAndNoDownloadsPending;
        public override event CleverTapCallbackDelegate OnOneTimeVariablesChangedAndNoDownloadsPending
        {
            add
            {
                lock (CallbackLock)
                {
                    if (ShouldInvokeVariablesCallbackImmediately)
                    {
                        value.Invoke();
                    }
                    else
                    {
                        _OnOneTimeVariablesChangedAndNoDownloadsPending += value;
                    }
                }
            }
            remove
            {
                lock (CallbackLock)
                {
                    _OnOneTimeVariablesChangedAndNoDownloadsPending -= value;
                }
            }
        }

        public override void OneTimeCleverTapVariablesChangedAndNoDownloadsPending(string message)
        {
            _OnOneTimeVariablesChangedAndNoDownloadsPending?.Invoke();
        }
    }
}
#endif