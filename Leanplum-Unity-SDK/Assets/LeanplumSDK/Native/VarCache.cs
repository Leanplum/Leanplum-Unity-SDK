//
// Copyright 2023, Leanplum, Inc.
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
using System.Linq;

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

        public static IDictionary<string, ActionDefinition> actionDefinitions = new Dictionary<string, ActionDefinition>();
        private static IDictionary<string, object> messages = new Dictionary<string, object>();

        private static IDictionary<string, object> diffs = new Dictionary<string, object>();
        private static IDictionary<string, object> devModeValuesFromServer;
        private static IDictionary<string, object> fileAttributes = new Dictionary<string, object>();
        private static List<object> variants = new List<object>();
        private static string varsJson;
        private static string varsSignature;
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
        public static LeanplumSecuredVars SecuredVars
        {
            get
            {
                if (string.IsNullOrEmpty(varsJson) || string.IsNullOrEmpty(varsSignature))
                {
                    return null;
                }

                return new LeanplumSecuredVars(varsJson, varsSignature);
            }
        }
        public static IDictionary<string, object> Messages
        {
            get { return messages; }
            private set { messages = value; }
        }

        public delegate void updateEventHandler();
        public static event updateEventHandler Update;

        public static void SetDevModeValuesFromServer(IDictionary<string, object> value)
        {
            devModeValuesFromServer = value;
        }

        public static void RegisterActionDefinition(ActionDefinition actionDefinition)
        {
            actionDefinition.Vars = actionDefinition.Args.ToDictionary();
            actionDefinitions[actionDefinition.Name] = actionDefinition;
        }

        internal static object Traverse(object collection, object key, bool autoInsert)
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
                IDictionary diffMap = diff is IDictionary ? diff as IDictionary : null;
                IEnumerable diffKeys = diffMap != null ? diffMap.Keys : diff as IEnumerable;
                IList varsList = vars is IList ? vars as IList : null;

                var merged = new List<object>();
                foreach (object lpVariable in varsList)
                {
                    merged.Add(MergeHelper(lpVariable, null));
                }

                // Merge values from server
                //
                // Array values from server come as Dictionary
                // Example:
                // string[] items = new string[] { "Item 1", "Item 2"};
                // args.With<string[]>("Items", items); // Action Context arg value 
                // "vars": {
                //      "Items": {
                //                  "[1]": "Item 222", // Modified value from server
                //                  "[0]": "Item 111"  // Modified value from server
                //              }
                //  }
                if (diffMap != null)
                {
                    foreach (object varSubscript in diffKeys)
                    {
                        var strSubscript = (string)varSubscript;
                        int subscript = Convert.ToInt32(strSubscript.Substring(1, strSubscript.Length - 1 - 1));
                        object lpVariable = diffMap.Contains(varSubscript) ? diffMap[varSubscript] : null;
                        while (subscript >= merged.Count)
                        {
                            merged.Add(null);
                        }
                        merged[subscript] = MergeHelper(merged[subscript], lpVariable);
                    }
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
                            var varsMapValue = varsMap != null && varsMap.Contains(lpVariable) ? varsMap[lpVariable] : null;
                            merged[lpVariable] = MergeHelper(varsMapValue, null);
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
            if (string.IsNullOrEmpty(Leanplum.ApiConfig.Token))
            {
                return;
            }

            string variablesCipher = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.VARIABLES_KEY, "{}");
            string fileAttributesCipher =
                LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.FILE_ATTRIBUTES_KEY, "{}");

            string messagesCipher = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.MESSAGES_KEY, "{}");

            string userIdCipher = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.USERID_KEY);
            if (!string.IsNullOrEmpty(userIdCipher))
            {
                Leanplum.ApiConfig.UserId = AESCrypt.Decrypt(userIdCipher, Leanplum.ApiConfig.Token);
            }

            string varsJsonCipher = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.VARIABLES_JSON_KEY, null);
            string varsSignatureCipher = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.VARIABLES_SIGN_KEY, null);
            string varsJson = varsJsonCipher != null ? AESCrypt.Decrypt(varsJsonCipher, Leanplum.ApiConfig.Token) : null;
            string varsSignature = varsSignatureCipher != null ? AESCrypt.Decrypt(varsSignatureCipher, Leanplum.ApiConfig.Token) : null;

            ApplyVariableDiffs(
                DeserializeEncryptedData(variablesCipher),
                DeserializeEncryptedData(messagesCipher),
                DeserializeEncryptedData(fileAttributesCipher),
                null,
                varsJson,
                varsSignature);
        }

        private static IDictionary<string, object> DeserializeEncryptedData(string dataCipher)
        {
            if (dataCipher == "{}")
            {
                return new Dictionary<string, object>();
            }

            string dataJson = AESCrypt.Decrypt(dataCipher, Leanplum.ApiConfig.Token);
            if (string.IsNullOrEmpty(dataJson))
            {
                return new Dictionary<string, object>();
            }

            object data = Json.Deserialize(dataJson);
            return data as IDictionary<string, object>;
        }

        private static void StoreEncrypted(string key, object data)
        {
            string serializedData = Json.Serialize(data);
            StoreEncrypted(key, serializedData);
        }

        private static void StoreEncrypted(string key, string serializedData)
        {
            string encrypted = AESCrypt.Encrypt(serializedData, Leanplum.ApiConfig.Token);
            LeanplumNative.CompatibilityLayer.StoreSavedString(key, encrypted);
        }

        public static void SaveDiffs()
        {
            if (Constants.isNoop)
            {
                return;
            }
            if (string.IsNullOrEmpty(Leanplum.ApiConfig.Token))
            {
                return;
            }

            StoreEncrypted(Constants.Defaults.MESSAGES_KEY, Messages);
            StoreEncrypted(Constants.Defaults.VARIABLES_KEY, diffs);
            StoreEncrypted(Constants.Defaults.FILE_ATTRIBUTES_KEY, fileAttributes);

            if (!string.IsNullOrEmpty(Leanplum.ApiConfig.UserId))
            {
                StoreEncrypted(Constants.Defaults.USERID_KEY, Leanplum.ApiConfig.UserId);
            }

            StoreEncrypted(Constants.Defaults.VARIABLES_JSON_KEY, varsJson);
            StoreEncrypted(Constants.Defaults.VARIABLES_SIGN_KEY, varsSignature);

            LeanplumNative.CompatibilityLayer.FlushSavedSettings();
        }

        public static void ApplyVariableDiffs(IDictionary<string, object> diffs,
                                              IDictionary<string, object> messages,
                                              IDictionary<string, object> fileAttributes = null,
                                              List<object> variants = null,
                                              string varsJson = null,
                                              string varsSignature = null)
        {
            if (fileAttributes != null)
            {
                foreach (var attribute in fileAttributes)
                {
                    FileAttributes[attribute.Key] = attribute.Value;
                }
            }
            if (messages != null)
            {
                MergeMessages(messages);
                Messages = messages;
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

            VarCache.varsJson = varsJson ?? string.Empty;
            VarCache.varsSignature = varsSignature ?? string.Empty;

            if (!IsSilent)
            {
                SaveDiffs();

                HasReceivedDiffs = true;
                OnUpdate();
            }
        }

        public static void OnUpdate()
        {
            Update?.Invoke();
        }

        internal static void MergeMessages(IDictionary<string, object> messages)
        {
            if (actionDefinitions.Count > 0)
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    var message = messages.ElementAt(i);
                    var values = message.Value as Dictionary<string, object>;
                    if (values != null)
                    {
                        var name = Util.GetValueOrDefault(values, Constants.Args.ACTION) as string;
                        if (!string.IsNullOrEmpty(name) && actionDefinitions.Keys.Contains(name))
                        {
                            var messageConfig = Util.GetValueOrDefault(values, Constants.Args.VARS) as Dictionary<string, object>;
                            if (messageConfig != null)
                            {
                                // Merges the default argument values defined in the Action Definition with the values coming from the API.
                                // The API returns only the modified values from the dashboard.
                                // It does NOT merge Actions which could be part of the Action Definition since
                                // the Actions are themselves Action Definitions.
                                // i.e Confirm -> Accept action -> Open URL
                                // The Confirm Action Definition contains the Accept action ->
                                // The Accept action itself has an Action Definition with default values.
                                // The last are not merged here but when the action is triggered.
                                var mergedVars = MergeHelper(actionDefinitions[name].Vars, messageConfig);
                                var mergedVarsDict = mergedVars as Dictionary<object, object>;
                                var newVars = mergedVarsDict.ToDictionary(item => item.Key.ToString(), item => item.Value);
                                values[Constants.Args.VARS] = newVars;
                                messages[message.Key] = values;
                            }
                        }
                    }
                }
            }
        }

        internal static IDictionary<string, object> MergeMessage(IDictionary<string, object> messageConfig)
        {
            string actionName = Util.GetValueOrDefault(messageConfig, Constants.Args.ACTION_NAME)?.ToString();
            if (!string.IsNullOrWhiteSpace(actionName) && actionDefinitions.ContainsKey(actionName))
            {
                var defaultArgs = actionDefinitions[actionName].Vars;
                var actionArgs = MergeHelper(defaultArgs, new Dictionary<string, object>(messageConfig)) as IDictionary<object, object>;
                return actionArgs.ToDictionary(item => item.Key.ToString(), item => item.Value);
            }
            return messageConfig;
        }

        internal static bool SendVariablesIfChanged()
        {
            if (devModeValuesFromServer != null && valuesFromClient != devModeValuesFromServer)
            {
                ForceSendVariables(null);
                return true;
            }
            return false;
        }

        internal static void ForceSendVariables(Leanplum.SyncVariablesCompleted completedHandler)
        {
            if (!Leanplum.IsDeveloperModeEnabled)
            {
                LeanplumNative.CompatibilityLayer.LogError("Leanplum Error: ForceSendVariables requires Development mode");
                return;
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                [Constants.Params.VARIABLES] = Json.Serialize(valuesFromClient),
                [Constants.Params.KINDS] = Json.Serialize(defaultKinds)
            };
            LeanplumUnityHelper.QueueOnMainThread(() => {

                Request request = RequestBuilder.withSetVarsAction().AndParameters(parameters).CreateImmediate();
                request.Response += delegate (object response)
                {
                    completedHandler?.Invoke(true);
                };
                request.Error += delegate (Exception ex)
                {
                    LeanplumNative.CompatibilityLayer.LogError("Leanplum Error: ForceSyncVariables", ex);
                    completedHandler?.Invoke(false);
                };
                Leanplum.RequestSender.Send(request);
            });
        }

        public static string KindFromValue(object defaultValue)
        {
            string kind = null;
            if (defaultValue is int || defaultValue is long || defaultValue is short ||
                defaultValue is char || defaultValue is sbyte || defaultValue is byte)
            {
                kind = Constants.Kinds.INT;
            }
            else if (defaultValue is float || defaultValue is double || defaultValue is decimal)
            {
                kind = Constants.Kinds.FLOAT;
            }
            else if (defaultValue is string)
            {
                kind = Constants.Kinds.STRING;
            }
            else if (defaultValue is IList || defaultValue is Array)
            {
                kind = Constants.Kinds.ARRAY;
            }
            else if (defaultValue is IDictionary)
            {
                kind = Constants.Kinds.DICTIONARY;
            }
            else if (defaultValue is bool)
            {
                kind = Constants.Kinds.BOOLEAN;
            }
            else
            {
                return null;
            }
            return kind;
        }
    }
}
