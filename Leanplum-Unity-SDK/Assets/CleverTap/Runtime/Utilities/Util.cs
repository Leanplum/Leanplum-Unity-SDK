using System;
using System.Collections;
using System.Collections.Generic;

namespace CleverTapSDK.Utilities {
    internal static class Util {
        internal static void FillInValues(object source, object destination) {
            if (source == null || Json.Serialize(source) == Json.Serialize(destination)) {
                return;
            }

            if (source is IDictionary sourceDictionary) {
                if (destination is IDictionary destinationDictionary) {
                    foreach (object key in sourceDictionary.Keys) {
                        object typedKey = Convert.ChangeType(key, destination.GetKeyType());
                        if (sourceDictionary[key] is IDictionary ||
                            sourceDictionary[key] is IList) {
                            if (destinationDictionary.Contains(typedKey)) {
                                FillInValues(sourceDictionary[key],
                                             destinationDictionary[typedKey]);
                            } else {
                                destinationDictionary[typedKey] = sourceDictionary[key];
                            }
                        } else {
                            destinationDictionary[typedKey] =
                                Convert.ChangeType(sourceDictionary[key],
                                                   destination.GetValueType());
                        }
                    }
                } else if (destination is IList destinationList) {
                    foreach (object varSubscript in sourceDictionary.Keys) {
                        var strSubscript = (string)varSubscript;
                        // assumes key is in format "[index]"
                        int subscript = Convert.ToInt32(strSubscript.Substring(1, strSubscript.Length - 1 - 1));
                        FillInValues(sourceDictionary[varSubscript],
                                     destinationList[subscript]);
                    }
                }
            } else if (source is IList || source is Array) {
                int index = 0;
                var sourceList = (IList)source;
                for (int sourceIndex = 0; sourceIndex < sourceList.Count; sourceIndex++) {
                    object value = sourceList[sourceIndex];

                    if (value is IDictionary || value is IList) {
                        FillInValues(value, ((IList)destination)[index]);
                    } else {
                        ((IList)destination)[index] =
                            Convert.ChangeType(value,
                                               destination.GetType().IsArray ?
                                               destination.GetType().GetElementType() :
                                               destination.GetKeyType());
                    }
                    index++;
                }
            } else {
                destination = Convert.ChangeType(source, source.GetType());
            }
        }

        #region Extensions

        internal static Type GetKeyType(this object dictionary) {
            return dictionary.GetType().GetGenericArguments()[0];
        }

        internal static Type GetValueType(this object dictionary) {
            return dictionary.GetType().GetGenericArguments()[1];
        }

        internal static Dictionary<string, object> ConvertDateObjects(this Dictionary<string, object> dictionary) {
            Dictionary<string, object> converted = new Dictionary<string, object>(dictionary);

            foreach (KeyValuePair<string, object> entry in dictionary) {
                if (entry.Value is DateTime) {
                    converted[entry.Key] = ((DateTime)entry.Value).GetCleverTapUnixTimestamp();
                }
            }
            return converted;
        }

        internal static string GetCleverTapUnixTimestamp(this DateTime dateTime) {
            // Get the offset from current time in UTC time
            DateTimeOffset dto = new DateTimeOffset(dateTime);
            // Get the unix timestamp in seconds, and add the milliseconds
            return "ct_date_" + dto.ToUnixTimeMilliseconds().ToString();
        }

        #endregion
    }
}
