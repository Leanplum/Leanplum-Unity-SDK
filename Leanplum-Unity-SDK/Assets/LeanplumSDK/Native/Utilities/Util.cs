//
// Copyright 2022, Leanplum, Inc.
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
using UnityEngine;

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
			
			if (source is IDictionary sourceDictionary)
			{
				if (destination is IDictionary destinationDictionary)
				{
					foreach (object key in sourceDictionary.Keys)
					{
                        object typedKey = Convert.ChangeType(key, destination.GetKeyType());
						if (sourceDictionary[key] is IDictionary ||
						    sourceDictionary[key] is IList)
						{
							if (destinationDictionary.Contains(typedKey))
							{
								FillInValues(sourceDictionary[key],
								             destinationDictionary[typedKey]);
							}
							else
							{
								destinationDictionary[typedKey] = sourceDictionary[key];
							}
						}
						else
						{
                            destinationDictionary[typedKey] =
								Convert.ChangeType(sourceDictionary[key],
                                                   destination.GetValueType());
						}
					}
				}
				else if (destination is IList destinationList)
				{
					foreach (object varSubscript in sourceDictionary.Keys)
					{
						var strSubscript = (string) varSubscript;
                        // assumes key is in format "[index]"
                        int subscript = Convert.ToInt32(strSubscript.Substring(1, strSubscript.Length - 1 - 1));
						FillInValues(sourceDictionary[varSubscript],
						             destinationList[subscript]);
					}
				}
			}
			else if (source is IList || source is Array)
			{
				int index = 0;
				var sourceList = (IList) source;
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
                                               destination.GetKeyType());
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
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            char first = s[0];
            if (char.IsUpper(first))
            {
                return s;
            }
            return char.ToUpper(first) + s.Substring(1);
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

        /// <summary>
        /// Handles Lists, Arrays and Dictionaries of primitives
        /// Casts the collection and elements to the requested Type and elements type
        /// Example of supported structures: int[], List<string>, Dictionary<string, double>
        /// Does not support complex structures: Dictionary<string, List<double>>, List<List<int>>
        /// For complex structures use Dictionary<string, object> and parse manually.
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="collection">The collection to cast</param>
        /// <returns></returns>
        internal static T ConvertCollectionToType<T>(object collection)
        {
            Type resultType = typeof(T);
            if (collection is IList && typeof(IList).IsAssignableFrom(resultType))
            {
                Type elementType;
                var valuesList = (collection as IList);
                IList newList;

                // Arrays i.e string[], int[]
                if (resultType.IsArray && !resultType.IsGenericType)
                {
                    elementType = resultType.GetElementType();
                    newList = Array.CreateInstance(elementType, valuesList.Count);
                    for (int i = 0; i < valuesList.Count; i++)
                    {
                        var newEl = Convert.ChangeType(valuesList[i], elementType);
                        newList[i] = newEl;
                    }
                }
                else
                {
                    // Generic Lists i.e List<string>
                    elementType = resultType.GetGenericArguments()[0];
                    newList = (IList)Activator.CreateInstance(resultType);
                    foreach (var el in valuesList)
                    {
                        var newEl = Convert.ChangeType(el, elementType);
                        newList.Add(newEl);
                    }
                }
                return (T)newList;
            }
            else if (collection is IDictionary && typeof(IDictionary).IsAssignableFrom(resultType))
            {
                Type keyType = typeof(T).GetGenericArguments()[0];
                Type elementType = typeof(T).GetGenericArguments()[1];

                var valuesDictionary = (collection as IDictionary);
                IDictionary newDict = (IDictionary)Activator.CreateInstance(resultType);
                foreach (var el in valuesDictionary.Keys)
                {
                    var newKey = Convert.ChangeType(el, keyType);
                    if (elementType == typeof(object))
                    {
                        newDict.Add(newKey, valuesDictionary[el]);
                    }
                    else
                    {
                        var newEl = Convert.ChangeType(valuesDictionary[el], elementType);
                        newDict.Add(newKey, newEl);
                    }
                }
                return (T)newDict;
            }

            return (T)collection;
        }

        internal static long ColorToInt(Color32 color32)
        {
            long color = (color32.a & 0xff) << 24 | (color32.r & 0xff) << 16 | (color32.g & 0xff) << 8 | (color32.b & 0xff);
            return color;
        }

        internal static Color IntToColor(long colorVal)
        {
            Color32 c = new Color32
            {
                b = (byte)((colorVal) & 0xFF),
                g = (byte)((colorVal >> 8) & 0xFF),
                r = (byte)((colorVal >> 16) & 0xFF),
                a = (byte)((colorVal >> 24) & 0xFF)
            };
            return c;
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

    static class UtilExtensions
    {
        internal static Type GetKeyType(this object dictionary)
        {
            return dictionary.GetType().GetGenericArguments()[0];
        }

        internal static Type GetValueType(this object dictionary)
        {
            return dictionary.GetType().GetGenericArguments()[1];
        }
    }
}
