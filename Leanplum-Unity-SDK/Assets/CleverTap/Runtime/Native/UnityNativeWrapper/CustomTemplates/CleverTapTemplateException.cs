#if UNITY_EDITOR
using System;

namespace CleverTapSDK.Native
{
    public class CleverTapTemplateException : Exception
    {
        public CleverTapTemplateException(string message) : base(message) { }
    }
}
#endif