//
// Copyright 2013, Leanplum, Inc.
//
//  Licensed to the Apache Software Foundation (ASF) under one
//  or more contributor license agreements.  See the NOTICE file
//  distributed with this work for additional information
//  regarding copyright ownership.  The ASF licenses this file
//  to you under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing,
//  software distributed under the License is distributed on an
//  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//  KIND, either express or implied.  See the License for the
//  specific language governing permissions and limitations
//  under the License.
using System;
using System.Collections;
using System.Collections.Generic;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK
{
    /// <summary>
    ///     Leanplum utilities.
    /// </summary>
    public static class Util
    {
        		/// <summary>
		///     Helper function to copy and cast values from containers of object types
		///     to containers of primitive types.
		/// </summary>
		/// <param name="source">Source container.</param>
		/// <param name="destination">Destination container.</param>
		public static void FillInValues(object source, object destination)
		{
			if (source == null || Json.Serialize(source) == Json.Serialize(destination))
			{
				return;
			}
			
			if (source is IDictionary)
			{
				if (destination is IDictionary)
				{
					bool cleared = false;
					foreach (object key in ((IDictionary) source).Keys)
					{
						object typedKey = Convert.ChangeType(key, destination.GetType().GetGenericArguments()[0]);
						if (((IDictionary) source)[key] is IDictionary ||
						    ((IDictionary) source)[key] is IList)
						{
							if (((IDictionary) destination).Contains(typedKey))
							{
								FillInValues(((IDictionary) source)[key],
								             ((IDictionary) destination)[typedKey]);
							}
							else
							{
								((IDictionary) destination)[typedKey] = ((IDictionary) source)[key];
							}
						}
						else
						{
							if (!cleared)
							{
								cleared = true;
								((IDictionary) destination).Clear();
							}
							
							((IDictionary) destination)[typedKey] =
								Convert.ChangeType(((IDictionary) source)[key],
								                   destination.GetType().GetGenericArguments()[1]);
						}
					}
				}
				else if (destination is IList)
				{
					foreach (object varSubscript in ((IDictionary) source).Keys)
					{
						var strSubscript = (string) varSubscript;
						int subscript = Convert.ToInt32(strSubscript.Substring(1, strSubscript.Length - 1 - 1));
						FillInValues(((IDictionary) source)[varSubscript],
						             ((IList) destination)[subscript]);
					}
				}
			}
			else if (source is IList || source is Array)
			{
				int index = 0;
				var sourceList = (IList) source;
				//        foreach (object value in (IList) source)
				for (int sourceIndex = 0; sourceIndex < sourceList.Count; sourceIndex++)
				{
					object value = sourceList[sourceIndex];
					
					if (value is IDictionary || value is IList)
					{
						FillInValues(value, ((IList) destination)[index]);
					}
					else
					{
						((IList) destination)[index] =
							Convert.ChangeType(value,
							                   destination.GetType().IsArray ?
							                   destination.GetType().GetElementType() :
							                   destination.GetType().GetGenericArguments()[0]);
					}
					index++;
				}
			}
			else
			{
				destination = Convert.ChangeType(source, source.GetType());
			}
		}

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
        
        /// <summary>
        /// Gets timestamp in milliseconds
        /// </summary>
        /// <param name="date">DateTime to convert</param>
        /// <returns>ts in milliseconds </returns>
        internal static long GetUnixTimestampFromDate(DateTime date)
        {
            TimeSpan timeDelta = (date - new DateTime(1970, 1, 1));
            return (long) timeDelta.TotalMilliseconds;
        }

        /// <summary>
        /// Gets DateTime from milliseconds ts
        /// </summary>
        /// <param name="timestamp">ts in milliseconds</param>
        /// <returns>DateTime from ts</returns>
        internal static DateTime GetDateFromUnixTimestamp(long timestamp)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(timestamp);
            return new DateTime(1970, 1, 1).AddTicks(ts.Ticks);
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
