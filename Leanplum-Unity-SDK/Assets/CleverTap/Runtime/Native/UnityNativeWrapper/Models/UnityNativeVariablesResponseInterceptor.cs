#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR 
using System;
using System.Collections.Generic;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native
{
    internal class UnityNativeVariablesResponseInterceptor : IUnityNativeResponseInterceptor
    {
        private readonly IVariablesResponseHandler responseHandler;

        public UnityNativeVariablesResponseInterceptor(IVariablesResponseHandler responseHandler)
        {
            this.responseHandler = responseHandler;
        }

        UnityNativeResponse IUnityNativeResponseInterceptor.Intercept(UnityNativeResponse response)
        {
            if (!response.IsSuccess())
            {
                responseHandler.HandleVariablesResponseError();
                return response;
            }

            if (response == null || string.IsNullOrWhiteSpace(response.Content))
                return response;

            try
            {
                Dictionary<string, object> responseContent = Json.Deserialize(response.Content) as Dictionary<string, object>;
                if (responseContent == null || responseContent.Count == 0)
                    return response;

                // If no variables are synced (defined in Dashboard), "vars" is omitted from the response payload
                // If the app is not enabled for PE, "vars" is omitted from the response payload
                // If "vars" is null, treat as if "vars" is omitted from the response payload
                // If variables have been synced (defined in Dashboard),
                // but there are no Overrides, "vars" is empty dictionary "{}".
                if (responseContent.TryGetValue("vars", out var vars))
                {
                    if (vars is IDictionary<string, object> varsDictionary)
                    {
                        responseHandler.HandleVariablesResponse(varsDictionary);
                    }
                    else
                    {
                        responseHandler.HandleVariablesResponseError();
                    }
                }
            }
            catch (Exception e)
            {
                CleverTapLogger.Log("Error parsing vars values, " + e.StackTrace);
            }

            return response;
        }
    }
}
#endif