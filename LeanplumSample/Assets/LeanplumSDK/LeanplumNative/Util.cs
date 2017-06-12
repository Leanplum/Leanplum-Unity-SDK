// Copyright 2013, Leanplum, Inc.

using LeanplumSDK.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LeanplumSDK
{
    /// <summary>
    ///     Leanplum utilities.
    /// </summary>
    public static class Util
    {
        public static TValue GetValueOrDefault<TKey, TValue>
            (IDictionary<TKey, TValue> dictionary,
                TKey key,
                TValue defaultValue = null) where TValue : class
        {
            TValue value;
            if (dictionary != null && dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultValue;
        }

        public static string Capitalize(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return String.Empty;
            }

            char first = s[0];
            if (char.IsUpper(first))
            {
                return s;
            }
            return char.ToUpper(first) + s.Substring(1);
        }

        public static WebRequest CreateWebRequest(string hostName, string path, IDictionary<string, string> parameters,
            string httpMethod, bool ssl, int timeout)
        {
            WebRequest request = CreateWebRequest(hostName, path, ssl, timeout);
            if (httpMethod.Equals("GET"))
            {
                request.AttachGetParameters(parameters);
            }
            else
            {
                request.AttachPostParameters(parameters);
            }

            return request;
        }

        public static WebRequest CreateWebRequest(string hostName, string path, bool ssl, int timeout)
        {
            string fullPath;
            if (path.StartsWith("http"))
            {
                fullPath = path;
            }
            else
            {
                fullPath = (ssl ? "https://" : "http://") + hostName + "/" + path;
            }
			return LeanplumNative.CompatibilityLayer.CreateWebRequest(fullPath, timeout);
        }

        internal static int NumResponses(object response)
        {
            try
            {
                return ((response as IDictionary<string, object>)[Constants.Keys.RESPONSE] as IList<object>).Count;
            }
            catch (KeyNotFoundException e)
            {
				LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", e);
                return 0;
            }
            catch (NullReferenceException e)
            {
				LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", e);
                return 0;
            }
        }

        internal static object GetResponseAt(object response, int index)
        {
            try
            {
                return ((response as IDictionary<string, object>)[Constants.Keys.RESPONSE] as IList<object>)[index];
            }
            catch (KeyNotFoundException e)
            {
				LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", e);
                return null;
            }
            catch (NullReferenceException e)
            {
				LeanplumNative.CompatibilityLayer.LogError("Could not parse JSON response", e);
                return null;
            }
        }

        internal static object GetLastResponse(object response)
        {
            int numResponses = Util.NumResponses(response);
            return numResponses > 0 ? GetResponseAt(response, numResponses - 1) : null;
        }

        internal static int GetUnixTimestamp()
        {
            TimeSpan timeDelta = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            return Convert.ToInt32(timeDelta.TotalSeconds);
        }

        public static void MaybeThrow(LeanplumException exception)
        {
            if (Constants.isDevelopmentModeEnabled)
            {
                throw exception;
            }
            else
            {
				LeanplumNative.CompatibilityLayer.LogError(exception.ToString());
            }
        }

        internal static bool IsNumber(object value)
        {
            return value is sbyte
                || value is byte
                || value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong
                || value is float
                || value is double
                || value is decimal;
        }
    }
}
