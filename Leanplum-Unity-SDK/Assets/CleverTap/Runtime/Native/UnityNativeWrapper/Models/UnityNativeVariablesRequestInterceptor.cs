#if UNITY_WEBGL && !UNITY_EDITOR
using System.Collections.Generic;

namespace CleverTapSDK.Native
{
    internal class UnityNativeVariablesRequestInterceptor : IUnityNativeRequestInterceptor
    {
        UnityNativeRequest IUnityNativeRequestInterceptor.Intercept(UnityNativeRequest request)
        {
            if (request.Path == UnityNativeConstants.Network.REQUEST_PATH_DEFINE_VARIABLES)
            {
                request.SetHeaders(new Dictionary<string, string>());
            }
            return request;
        }
    }
}
#endif