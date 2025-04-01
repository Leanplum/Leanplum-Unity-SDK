#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using CleverTapSDK.Common;
using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native
{
    internal static class UnityNativeVariableUtils
    {
        internal const string DOT = ".";

        /// <summary>
        /// Updates a value in a nested dictionary structure based on a hierarchical naming convention.
        /// </summary>
        /// <param name="name">The full dot-notation name of the variable</param>
        /// <param name="nameComponents">Array of name components (split by dots)</param>
        /// <param name="value">The new value to set</param>
        /// <param name="values">The dictionary to update</param>
        internal static void UpdateValues(string name, string[] nameComponents, object value, IDictionary values)
        {
            if (nameComponents == null || nameComponents.Length == 0)
                return;

            object valuesPtr = values;
            for (int i = 0; i < nameComponents.Length - 1; i++)
            {
                valuesPtr = Traverse(valuesPtr, nameComponents[i], true);
            }

            if (valuesPtr is IDictionary dictionary)
            {
                dictionary.TryGetValue(nameComponents[^1], out object currentValue);

                if (currentValue is IDictionary && value is IDictionary)
                {
                    value = MergeHelper(value, currentValue);
                }
                else if (currentValue != null && currentValue.Equals(value))
                {
                    CleverTapLogger.Log($"Variable with name {name} will override value: {currentValue}, with new value: {value}.");
                }

                dictionary[nameComponents[^1]] = value;
            }
        }

        internal static Dictionary<string, object> ConvertDictionaryToNestedDictionaries(IDictionary<string, object> values)
        {
            var result = new Dictionary<string, object>();
            foreach (var entry in values)
            {
                string key = entry.Key;
                if (key.Contains(DOT))
                {
                    string[] components = GetNameComponents(key);
                    int namePosition = components.Length - 1;
                    var currentMap = result;

                    for (int i = 0; i < components.Length; i++)
                    {
                        string component = components[i];

                        if (i == namePosition)
                        {
                            currentMap[component] = entry.Value;
                        }
                        else
                        {
                            bool containsKey = currentMap.TryGetValue(component, out var currentValue);
                            if (!containsKey || currentValue is not IDictionary<string, object>)
                            {
                                var nestedMap = new Dictionary<string, object>();
                                currentMap[component] = nestedMap;
                                currentMap = nestedMap;
                            }
                            else
                            {
                                currentMap = (Dictionary<string, object>)currentValue;
                            }
                        }
                    }
                }
                else
                {
                    result[key] = entry.Value;
                }
            }

            return result;
        }

        internal static void ConvertNestedDictionariesToFlat(string prefix, IDictionary map, IDictionary result)
        {
            if (map == null || result == null)
                return;

            foreach (var key in map.Keys)
            {
                string stringKey = key.ToString();
                object value = map[key];
                if (value is IDictionary dictionary)
                {
                    ConvertNestedDictionariesToFlat(prefix + stringKey + DOT, dictionary, result);
                }
                else
                {
                    result[prefix + stringKey] = value;
                }
            }
        }

        internal static Dictionary<string, object> GetFlatVarsPayload(IDictionary<string, IVar> varsDictionary)
        {
            Dictionary<string, object> result = new Dictionary<string, object>
            {
                { "type", "varsPayload" }
            };

            Dictionary<string, object> allVars = new Dictionary<string, object>();
            if (varsDictionary == null)
            {
                CleverTapLogger.LogError("GetFlatVarsPayload: vars are null. Returning empty vars payload.");
                result.Add("vars", allVars);
                return result;
            }

            foreach (var variable in varsDictionary)
            {
                var name = variable.Key;
                var defaultValue = variable.Value.DefaultObjectValue;
                if (defaultValue is IDictionary dictionary)
                {
                    Dictionary<string, object> valueMap = new Dictionary<string, object>
                    {
                        { name, defaultValue }
                    };
                    Dictionary<string, object> flattenedValueMap = new Dictionary<string, object>();
                    ConvertNestedDictionariesToFlat(string.Empty, valueMap, flattenedValueMap);
                    foreach (var entry in flattenedValueMap)
                    {
                        string flattenedKey = entry.Key;
                        object flattenedValue = entry.Value;
                        string flattenedValueKind = CleverTapPlatformVariable.GetKindNameFromType(flattenedValue.GetType());
                        Dictionary<string, object> varData = new Dictionary<string, object>
                        {
                            { "type", GetDefineTypeFromKind(flattenedValueKind) },
                            { "defaultValue", flattenedValue }
                        };
                        allVars.Add(flattenedKey, varData);
                    }
                }
                else
                {
                    Dictionary<string, object> varData = new Dictionary<string, object>
                    {
                        { "type", GetDefineTypeFromKind(variable.Value.Kind) }
                    };

                    // File variables do not have a default value
                    if (variable.Value.Kind != CleverTapVariableKind.FILE)
                    {
                        varData.Add("defaultValue", defaultValue);
                    }
                    allVars.Add(name, varData);
                }
            }
            result.Add("vars", allVars);
            return result;
        }

        internal static object MergeHelper(object vars, object diff)
        {
            if (diff == null)
            {
                return vars;
            }
            if (diff is ValueType || diff is string || vars is ValueType || vars is string)
            {
                return diff;
            }

            var varsMap = vars as IDictionary;
            var diffMap = diff as IDictionary;
            // Return null if neither vars nor diff is dictionary.
            if (varsMap == null && diffMap == null)
            {
                return null;
            }
            if (varsMap == null)
            {
                // diff is Dictionary.
                return diff;
            }

            ICollection varsKeys = varsMap.Keys;
            ICollection diffKeys = diffMap.Keys;

            // varsMap is not null, check diffMap only.
            if (diffMap != null)
            {
                IDictionary merged = Util.CreateNewDictionary(varsMap);

                foreach (var varKey in varsKeys)
                {
                    diffMap.TryGetValue(varKey, out object diffVar);
                    varsMap.TryGetValue(varKey, out object value);
                    if (diffVar == null)
                    {
                        merged[varKey] = value;
                    }
                }
                foreach (var diffKey in diffKeys)
                {
                    diffMap.TryGetValue(diffKey, out object diffsValue);
                    varsMap.TryGetValue(diffKey, out object varsValue);
                    object mergedValues = MergeHelper(varsValue, diffsValue);
                    merged[diffKey] = mergedValues;
                }
                return merged;
            }

            return null;
        }

        internal static object Traverse(object collection, object key, bool autoInsert)
        {
            if (collection == null || key == null)
                return null;

            string keyString = key.ToString();
            if (collection is IDictionary dictionary)
            {
                bool containsKey = dictionary.TryGetValue(keyString, out var result);
                if (!containsKey && autoInsert)
                {
                    result = new Dictionary<string, object>();
                    dictionary[keyString] = result;
                }
                return result;
            }

            return null;
        }

        internal static string[] GetNameComponents(string name)
        {
            return name.Split(DOT);
        }

        internal static IDictionary CopyDictionary(IDictionary dictionary)
        {
            IDictionary copy = Util.CreateNewDictionary(dictionary);
            foreach (var key in dictionary.Keys)
            {
                var value = dictionary[key];
                if (value is IDictionary valueDictionary)
                {
                    copy[key] = CopyDictionary(valueDictionary);
                }
                else
                {
                    copy[key] = value;
                }
            }

            return copy;
        }

        /// <summary>
        /// Get the variable type to use for the defineVars payload when syncing variables.
        /// </summary>
        /// <param name="cleverTapVariableKind">The <see cref="CleverTapVariableKind"/>
        /// kind.</param>
        /// <returns>The variable string type conforming to defineVars variable types.</returns>
        internal static string GetDefineTypeFromKind(string cleverTapVariableKind)
        {
            return cleverTapVariableKind switch
            {
                CleverTapVariableKind.INT or CleverTapVariableKind.FLOAT => "number",
                CleverTapVariableKind.BOOLEAN => "boolean",
                _ => cleverTapVariableKind,
            };
        }
    }
}
#endif