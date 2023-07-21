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
using System.Collections.Generic;
using System.Text;

namespace LeanplumSDK
{
    public enum MigrationState
    {
        Undefined = 0,
        Leanplum,
        Duplicate,
        CleverTap
    }

    public class MigrationConfig
    {
        public MigrationConfig(MigrationState state, string accountId, string accountToken,
            string accountRegion, Dictionary<string, string> attributeMappings, string[] identityKeys)
        {
            State = state;
            AccountId = accountId;
            AccountToken = accountToken;
            AccountRegion = accountRegion;
            AttributeMappings = attributeMappings;
            IdentityKeys = identityKeys;
        }

        public MigrationConfig(Dictionary<string, object> config)
        {
            if (int.TryParse(Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_STATE_KEY)?.ToString(), out int state))
            {
                State = (MigrationState)state;
            }
            else
            {
                State = MigrationState.Undefined;
            }

            AccountId = Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_ID_KEY)?.ToString();
            AccountToken = Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_TOKEN_KEY)?.ToString();
            AccountRegion = Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_REGION_KEY)?.ToString();

            object attributeValue = Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_ATTRIBUTES_KEY);
            if (attributeValue is Dictionary<string, object> attributesDictionary)
            {
                AttributeMappings = new Dictionary<string, string>();
                foreach (KeyValuePair<string, object> entry in attributesDictionary) {
                    if (entry.Value is string strValue)
                    {
                        AttributeMappings.Add(entry.Key, strValue);
                    }
                }
            }
            else if (attributeValue != null)
            {
                UnityEngine.Debug.Log($"MigrationConfig: expected Dictionary<string,object> but got {attributeValue.GetType()}");
            }

            IdentityKeys = new string[0];
            var identityKeysFromConfig = Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_IDENTITY_KEYS_KEY);
            if (identityKeysFromConfig is List<object> keysList)
            {
                IdentityKeys = new string[keysList.Count];
                for (int i = 0; i < keysList.Count; i++)
                {
                    if (keysList[i] is string identityKey)
                    {
                        IdentityKeys[i] = identityKey;
                    }
                }
            }
            else if (identityKeysFromConfig != null)
            {
                UnityEngine.Debug.Log($"MigrationConfig: expected List<object> but got {identityKeysFromConfig.GetType()}");
            }
        }

        public MigrationState State { get; }
        public string AccountId { get; }
        public string AccountToken { get; }
        public string AccountRegion { get; }
        public Dictionary<string, string> AttributeMappings { get; }
        public string[] IdentityKeys { get; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"State: {State}");
            builder.AppendLine($"AccountId: {AccountId}");
            builder.AppendLine($"AccountToken: {AccountToken}");
            builder.AppendLine($"AccountRegion: {AccountRegion}");
            builder.AppendLine($"AttributeMappings: {MiniJSON.Json.Serialize(AttributeMappings)}");
            builder.AppendLine($"IdentityKeys: [{string.Join(",", IdentityKeys)}]");
            return builder.ToString();
        }
    }
}