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

        private static readonly object attributesLock = new object();
        private static Queue<IDictionary<string, object>> changedUserAttributes = new Queue<IDictionary<string, object>>();
        private static IDictionary<string, object> userAttributes;

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
        public static IDictionary<string, object> Messages
        {
            get { return messages; }
            private set { messages = value; }
        }
        internal static Queue<IDictionary<string, object>> ChangedUserAttributes
        {
            get { return changedUserAttributes; }
            private set { changedUserAttributes = value; }
        }
        public static IDictionary<string, object> UserAttributes
        {
            get
            {
                lock (attributesLock)
                {
                    if (userAttributes == null)
                         userAttributes = new Dictionary<string, object>();

                    return userAttributes;
                }
            }
        }

        public static int downloadsPending;

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

            string messagesCipher = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.MESSAGES_KEY, "{}");

            string userIdCipher = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.USERID_KEY);
            if (!String.IsNullOrEmpty(userIdCipher))
            {
                LeanplumRequest.UserId = AESCrypt.Decrypt(userIdCipher, LeanplumRequest.Token);
            }

            ApplyVariableDiffs(
                Json.Deserialize(variablesCipher == "{}" ? variablesCipher :
                                   AESCrypt.Decrypt(variablesCipher, LeanplumRequest.Token))
                                 as IDictionary<string, object>,
                Json.Deserialize(messagesCipher == "{}" ? messagesCipher : AESCrypt.Decrypt(messagesCipher, LeanplumRequest.Token)) as IDictionary<string, object>,
                Json.Deserialize(fileAttributesCipher == "{}" ? fileAttributesCipher :
                                    AESCrypt.Decrypt(fileAttributesCipher, LeanplumRequest.Token))
                                 as IDictionary<string, object>);

            LoadUserAttributes(token);
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

            string messagesCipher = Json.Serialize(Messages);
            LeanplumNative.CompatibilityLayer.StoreSavedString(Constants.Defaults.MESSAGES_KEY,
    AESCrypt.Encrypt(messagesCipher, LeanplumRequest.Token));

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
                                              IDictionary<string, object> messages,
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
            var parameters = new Dictionary<string, string>();
            parameters[Constants.Params.VARIABLES] = Json.Serialize(valuesFromClient);
            parameters[Constants.Params.KINDS] = Json.Serialize(defaultKinds);
            LeanplumUnityHelper.QueueOnMainThread(() => {
                LeanplumRequest setVarsReq = LeanplumRequest.Post(Constants.Methods.SET_VARS, parameters);
                setVarsReq.Response += delegate (object response)
                {
                    completedHandler?.Invoke(true);
                };
                setVarsReq.Error += delegate (Exception ex)
                {
                    LeanplumNative.CompatibilityLayer.LogError("Leanplum Error: ForceSyncVariables", ex);
                    completedHandler?.Invoke(false);
                };
                setVarsReq.SendNow();
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

        internal static void UpdateUserAttributes()
        {
            lock (attributesLock)
            {
                bool madeChanges = false;
                foreach (var attributes in ChangedUserAttributes)
                {
                    var existingAttributes = UserAttributes;
                    if (existingAttributes == null)
                    {
                        existingAttributes = new Dictionary<string, object>();
                    }
                    foreach (string attributeName in attributes.Keys)
                    {
                        object existingValue = Util.GetValueOrDefault(existingAttributes, attributeName);
                        object value = attributes[attributeName];
                        if ((existingValue == null && value != null) ||
                            (existingValue != null && !existingValue.Equals(value)))
                        {
                            existingAttributes[attributeName] = value;
                            Trigger trigger = new Trigger(ActionTrigger.UserAttribute)
                            {
                                EventName = attributeName,
                                UserAttributeValue = value,
                                UserAttributePreviousValue = existingValue
                            };
                            LeanplumActionManager.MaybePerformActions(trigger);

                            madeChanges = true;
                        }
                    }
                }
                changedUserAttributes.Clear();
                if (madeChanges)
                {
                    SaveUserAttributes();
                }
            }
        }

        private static void LoadUserAttributes(string token)
        {
            if (token != null)
            {
                string userAttributesCipher = LeanplumNative.CompatibilityLayer.GetSavedString(Constants.Defaults.USER_ATTRIBUTES_KEY, "{}");

                var attributes = Json.Deserialize(userAttributesCipher == "{}" ? userAttributesCipher : AESCrypt.Decrypt(userAttributesCipher, token)) as IDictionary<string, object>;
                if (attributes != null)
                    userAttributes = attributes;
            }

            if (userAttributes == null)
                userAttributes = new Dictionary<string, object>();
        }

        private static void SaveUserAttributes()
        {
            string userAttributesCipher = AESCrypt.Encrypt(Json.Serialize(UserAttributes), LeanplumRequest.Token);
            LeanplumNative.CompatibilityLayer.StoreSavedString(Constants.Defaults.USER_ATTRIBUTES_KEY, userAttributesCipher);
            LeanplumNative.CompatibilityLayer.FlushSavedSettings();
        }
    }
}
