#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR 
using System;
using System.Collections.Generic;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native
{
    internal class UnityNativeMetadataResponseInterceptor : IUnityNativeResponseInterceptor
    {
        private readonly UnityNativePreferenceManager _preferenceManager;

        public UnityNativeMetadataResponseInterceptor(UnityNativePreferenceManager preferenceManager)
        {
            _preferenceManager = preferenceManager;
        }

        UnityNativeResponse IUnityNativeResponseInterceptor.Intercept(UnityNativeResponse response)
        {
            if (response == null || string.IsNullOrWhiteSpace(response.Content))
                return response;

            Dictionary<string,object> responseContent = Json.Deserialize(response.Content) as Dictionary<string,object>;
            if (responseContent == null || responseContent.Count == 0)
                return response;
            // Handle _i
            try {
                if (responseContent.TryGetValue("_i", out var value)) {
                    long i = long.Parse(value.ToString());
                    SetI(i);
                }
            } catch (Exception e) {
                CleverTapLogger.Log("Error parsing _i values, " + e.StackTrace);
            }

            // Handle _j
            try {
                if (responseContent.TryGetValue("_j", out var value)) {
                    long j = long.Parse(value.ToString());
                    SetJ(j);
                }
            } catch (Exception e) {
                CleverTapLogger.Log("Error parsing _j values, " + e.StackTrace);
            }

            return response;
        }
             
        public void SetI(long l)
        {
            _preferenceManager.SetLong(UnityNativeConstants.EventMeta.KEY_I, l);
        }
    
        public void SetJ(long l)
        {
            _preferenceManager.SetLong(UnityNativeConstants.EventMeta.KEY_J, l);
        }
    }
}
#endif