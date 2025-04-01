using System;
using System.Collections;
using System.Collections.Generic;

namespace CleverTapSDK.Utilities
{
    internal static class Util
    {
        internal static void FillInValues(object source, object destination, bool shouldCopyCollection = true)
        {
            if (source == null || Json.Serialize(source) == Json.Serialize(destination))
            {
                return;
            }

            var sourceDictionary = source as IDictionary;
            var destinationDictionary = destination as IDictionary;
            if (sourceDictionary == null && destinationDictionary == null)
            {
                return;
            }

            foreach (object key in sourceDictionary.Keys)
            {
                object typedKey = Convert.ChangeType(key, destination.GetKeyType());
                if (sourceDictionary[key] is IDictionary dictionary)
                {
                    if (destinationDictionary.Contains(typedKey))
                    {
                        FillInValues(sourceDictionary[key],
                                     destinationDictionary[typedKey]);
                    }
                    else if (shouldCopyCollection)
                    {
                        IDictionary copy = CreateNewDictionary(dictionary);
                        FillInValues(dictionary, copy);
                        destinationDictionary[typedKey] = copy;
                    }
                    else
                    {
                        destinationDictionary[typedKey] = dictionary;
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

        internal static K CreateNewDictionary<K>(K obj) where K : class
        {
            if (obj is IDictionary<object, object>)
            {
                return (K)Activator.CreateInstance(obj.GetType());
            }

            var type = obj.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keyType = type.GetGenericArguments()[0];
                Type valueType = type.GetGenericArguments()[1];

                Type newDictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                return (K)Activator.CreateInstance(newDictType);
            }

            return null;
        }

        #region Extensions

        internal static Type GetKeyType(this object dictionary)
        {
            return dictionary.GetType().GetGenericArguments()[0];
        }

        internal static Type GetValueType(this object dictionary)
        {
            return dictionary.GetType().GetGenericArguments()[1];
        }

        internal static bool TryGetValue(this IDictionary dictionary,
            object key, out object value)
        {
            if (dictionary.Contains(key))
            {
                value = dictionary[key];
                return true;
            }

            value = null;
            return false;
        }

        internal static Dictionary<string, object> ConvertDateObjects(this Dictionary<string, object> dictionary)
        {
            Dictionary<string, object> converted = new Dictionary<string, object>(dictionary);

            foreach (KeyValuePair<string, object> entry in dictionary)
            {
                if (entry.Value is DateTime)
                {
                    converted[entry.Key] = ((DateTime)entry.Value).GetCleverTapUnixTimestamp();
                }
            }
            return converted;
        }

        internal static string GetCleverTapUnixTimestamp(this DateTime dateTime)
        {
            // Get the offset from current time in UTC time
            DateTimeOffset dto = new DateTimeOffset(dateTime);
            // Get the unix timestamp in seconds, and add the milliseconds
            return "ct_date_" + dto.ToUnixTimeMilliseconds().ToString();
        }

        #endregion
    }
}
