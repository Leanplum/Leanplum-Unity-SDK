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
using LeanplumSDK.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace LeanplumSDK
{
    /// <summary>
    ///     Variable cache.
    /// </summary>
    internal static class VarCache
    {
        private static readonly IDictionary<string, Var> vars = new Dictionary<string, Var>();
        private static readonly IDictionary<string, object> valuesFromClient = new Dictionary<string, object>();
        private static readonly IDictionary<string, string> defaultKinds = new Dictionary<string, string>();
        private static IDictionary<string, object> diffs = new Dictionary<string, object>();
        private static IDictionary<string, object> devModeValuesFromServer;
        private static IDictionary<string, object> fileAttributes = new Dictionary<string, object>();
        private static List<object> variants = new List<object>();
        private static object merged;

        public static bool HasReceivedDiffs { get; private set; }
        public static bool IsSilent { get; set; }
        public static bool VarsNeedUpdate { get; set; }
        public static IDictionary<string, object> FileAttributes
        {
            get { return fileAttributes; }
            private set { fileAttributes = value; }
        }
        public static IDictionary<string, object> Diffs
        {
            get { return diffs; }
            private set { diffs = value; }
        }
        public static List<object> Variants
        {
            get { return variants; }
            private set { variants = value; }
        }
        public static int downloadsPending;

        public delegate void updateEventHandler();
        public static event updateEventHandler Update;

        public static void SetDevModeValuesFromServer(IDictionary<string, object> value)
        {
            devModeValuesFromServer = value;
        }

        private static object Traverse(object collection, object key, bool autoInsert)
        {
            if (collection == null)
            {
                return null;
            }
            if (collection is IDictionary)
            {
                object result = null;
                IDictionary<object, object> dict = collection as Dictionary<object, object>;
                if (dict != null)
                {
                    dict.TryGetValue(key, out result);
                    if (result == null && autoInsert)
                    {
                        result = new Dictionary<object, object>();
                        ((IDictionary<object, object>) collection)[key] = result;
                    }
                }
                else
                {
                    // Traverse grouped variables.
                    IDictionary<string, object> dictgroup = collection as Dictionary<string, object>;
                    dictgroup.TryGetValue((string) key, out result);
                    if (result == null && autoInsert)
                    {
                        result = new Dictionary<string, object>();
                        ((IDictionary<string, object>) collection)[(string) key] = result;
                    }
                }
                return result;
            }
            if (collection is IList)
            {
                IList<object> list = collection as IList<object>;
                int? index = key as int?;
                if (list != null && index != null && index >= 0 && index < list.Count)
                {
                    return list[index.Value];
                }
            }
            return null;
        }

        public static void RegisterVariable(Var lpVariable)
        {
            vars[lpVariable.Name] = lpVariable;
            if (lpVariable.GetDefaultValue() != null)
            {
                object valuesPointer = valuesFromClient;
                for (int i = 0; i < lpVariable.NameComponents.Length - 1; i++)
                {
                    valuesPointer = Traverse(valuesPointer, lpVariable.NameComponents[i], true);
                }
                ((IDictionary<string, object>) valuesPointer).Add(
                    new KeyValuePair<string, object>(lpVariable.NameComponents[lpVariable.NameComponents.Length - 1],
                                                     lpVariable.GetDefaultValue()));
            }
            defaultKinds[lpVariable.Name] = lpVariable.Kind;
        }

        public static Var<T> GetVariable<T>(string name)
        {
            return HasVariable(name) ? vars[name] as Var<T> : null;
        }

        public static bool HasVariable(string name)
        {
            return vars.ContainsKey(name);
        }

        private static void ComputeMergedDictionary()
        {
            merged = MergeHelper(valuesFromClient, Diffs);
        }

        private static object MergeHelper(object vars, object diff)
        {
            if (Util.IsNumber(diff) || diff is bool? || diff is string || diff is char?)
            {
                return diff;
            }
            if (diff == null)
            {
                return vars;
            }

            // Merge arrays.
            if (vars is IList)
            {
                IDictionary diffMap = diff is IDictionary ? diff as IDictionary: null;
                IEnumerable diffKeys = diffMap != null ? diffMap.Keys : diff as IEnumerable;
                IList varsList = vars is IList ? vars as IList : null;

                var merged = new List<object>();
                foreach (object lpVariable in varsList)
                {
                    merged.Add(MergeHelper(lpVariable, null));
                }
                foreach (object varSubscript in diffKeys)
                {
                    var strSubscript = (string) varSubscript;
                    int subscript = Convert.ToInt32(strSubscript.Substring(1, strSubscript.Length - 1 - 1));
                    object lpVariable = diffMap.Contains(varSubscript) ? diffMap[varSubscript] : null;
                    while (subscript >= merged.Count)
                    {
                        merged.Add(null);
                    }
                    merged[subscript] = MergeHelper(merged[subscript], lpVariable);
                }
                return merged;
            }

            // Merge dictionaries.
            if (vars is IDictionary || diff is IDictionary)
            {
                IDictionary diffMap = diff is IDictionary ? diff as IDictionary: null;
                IDictionary varsMap = vars is IDictionary ? vars as IDictionary: null;
                IEnumerable diffKeys = diffMap != null ? diffMap.Keys : diff as IEnumerable;
                IEnumerable varsKeys = varsMap != null ? varsMap.Keys : vars as IEnumerable;

                var merged = new Dictionary<object, object>();
                if (varsKeys != null)
                {
                    foreach (object lpVariable in varsKeys)
                    {
                        if (!diffMap.Contains(lpVariable))
                        {
                            merged[lpVariable] = MergeHelper(varsMap.Contains(lpVariable) ? varsMap[lpVariable] : null, null);
                        }
                    }
                }
                foreach (object lpVariable in diffKeys)
                {
                    IDictionary<string, object> varsDic = Json.Deserialize(Json.Serialize(varsMap)) as IDictionary<string, object>;
                    // If varsDic is null, there is a nested container - retrieve value from original varsMap.
                    if (varsDic == null)
                    {
                        merged[lpVariable] =
                            MergeHelper(varsMap != null && varsMap.Contains(lpVariable) ? varsMap[lpVariable] : null,
                                        diffMap != null && diffMap.Contains(lpVariable) ? diffMap[lpVariable] : null);
                    }
                    else
                    {
                        merged[lpVariable] = 
                            MergeHelper(varsDic != null && varsDic.ContainsKey((string) lpVariable)
                                            ? varsDic[(string) lpVariable] : null,
                                        diffMap != null && diffMap.Contains(lpVariable)
                                            ? diffMap[lpVariable] : null);
                    }
                }
                return merged;
            }
            return null;
        }

        public static object GetMergedValueFromComponentArray(object[] components)
        {
            object mergedPtr = merged ?? valuesFromClient;
            foreach (object component in components)
            {
                mergedPtr = Traverse(mergedPtr, component, false);
            }
            return mergedPtr;
        }

        public static void LoadDiffs()
        {
            if (Constants.isNoop)
            {
                return;
            }
            string token = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.TOKEN_KEY);
            if (token == null)
            {
                return;
            }
            LeanplumRequest.Token = token;
            string variablesCipher = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.VARIABLES_KEY, "{}");
            string fileAttributesCipher =
                LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.FILE_ATTRIBUTES_KEY, "{}");

            string userIdCipher = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.USERID_KEY);
            if (!String.IsNullOrEmpty(userIdCipher))
            {
                LeanplumRequest.UserId = AESCrypt.Decrypt(userIdCipher, LeanplumRequest.Token);
            }

            ApplyVariableDiffs(
                Json.Deserialize(variablesCipher == "{}" ? variablesCipher :
                                   AESCrypt.Decrypt(variablesCipher, LeanplumRequest.Token))
                                 as IDictionary<string, object>,
                Json.Deserialize(fileAttributesCipher == "{}" ? fileAttributesCipher :
                                    AESCrypt.Decrypt(fileAttributesCipher, LeanplumRequest.Token))
                                 as IDictionary<string, object>);
        }

        public static void SaveDiffs()
        {
            if (Constants.isNoop)
            {
                return;
            }
            if (String.IsNullOrEmpty(LeanplumRequest.Token))
            {
                return;
            }

            string variablesCipher = Json.Serialize(diffs);
            string fileAttributeCipher = Json.Serialize(fileAttributes);
            LeanplumNative.CompatibilityLayer.StoreSavedString(Constants.Defaults.VARIABLES_KEY,
                AESCrypt.Encrypt(variablesCipher, LeanplumRequest.Token));
            LeanplumNative.CompatibilityLayer.StoreSavedString(Constants.Defaults.FILE_ATTRIBUTES_KEY,
                 AESCrypt.Encrypt(fileAttributeCipher, LeanplumRequest.Token));
            if (!String.IsNullOrEmpty(LeanplumRequest.UserId))
            {
                LeanplumNative.CompatibilityLayer.StoreSavedString(Constants.Defaults.USERID_KEY,
                                                             AESCrypt.Encrypt(LeanplumRequest.UserId, LeanplumRequest.Token));
            }
            LeanplumNative.CompatibilityLayer.StoreSavedString(Constants.Defaults.TOKEN_KEY, LeanplumRequest.Token);
            LeanplumNative.CompatibilityLayer.FlushSavedSettings();
        }

        public static void ApplyVariableDiffs(IDictionary<string, object> diffs,
                                              IDictionary<string, object> fileAttributes = null,
                                              List<object> variants = null)
        {
            if (fileAttributes != null)
            {
                foreach (var attribute in fileAttributes)
                {
                    FileAttributes[attribute.Key] = attribute.Value;
                }
            }
            if (diffs != null)
            {
                Diffs = diffs;
                ComputeMergedDictionary();
            }
            if (variants != null)
            {
                Variants = variants;
            }

            foreach (Var lpVariable in vars.Values)
            {
                lpVariable.Update();
            }

            if (!IsSilent)
            {
                SaveDiffs();

                HasReceivedDiffs = true;
                OnUpdate();
            }
        }

        public static void OnUpdate()
        {
            if (Update != null)
            {
                Update();
            }
        }

        internal static void CheckVarsUpdate()
        {
            CheckVarsUpdate (null);
        }

        internal static void CheckVarsUpdate(Action callback)
        {
            IDictionary<string, string> updateVarsParams = new Dictionary<string, string>();
            updateVarsParams[Constants.Params.INCLUDE_DEFAULTS] = false.ToString();

            LeanplumRequest updateVarsReq = LeanplumRequest.Post(Constants.Methods.GET_VARS, updateVarsParams);
            updateVarsReq.Response += delegate(object varsUpdate)
            {
                var getVariablesResponse = Util.GetLastResponse(varsUpdate) as IDictionary<string, object>;
                var newVarValues = Util.GetValueOrDefault(getVariablesResponse, Constants.Keys.VARS) as IDictionary<string, object>;
                var newVarFileAttributes = Util.GetValueOrDefault(getVariablesResponse, Constants.Keys.FILE_ATTRIBUTES) as IDictionary<string, object>;
                var newVariants = Util.GetValueOrDefault(getVariablesResponse, Constants.Keys.VARIANTS) as List<object> ?? new List<object>();
				
                ApplyVariableDiffs(newVarValues, newVarFileAttributes, newVariants);

                if (callback != null)
                {
                    callback();
                }
            };
            updateVarsReq.Error += delegate
            {
                if (callback != null)
                {
                    callback();
                }
            };
            updateVarsReq.SendNow();
            VarsNeedUpdate = false;
        }

        internal static bool SendVariablesIfChanged()
        {
            if (devModeValuesFromServer != null && valuesFromClient != devModeValuesFromServer)
            {
                var parameters = new Dictionary<string, string>();
                parameters[Constants.Params.VARIABLES] = Json.Serialize(valuesFromClient);
                parameters[Constants.Params.KINDS] = Json.Serialize(defaultKinds);
                LeanplumUnityHelper.QueueOnMainThread(() => LeanplumRequest.Post(Constants.Methods.SET_VARS, parameters).SendNow());
                return true;
            }
            return false;
        }
    }
}
