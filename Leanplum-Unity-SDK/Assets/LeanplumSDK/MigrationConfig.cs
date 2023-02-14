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
            string accountRegion, Dictionary<string, string> attributeMappings)
        {
            State = state;
            AccountId = accountId;
            AccountToken = accountToken;
            AccountRegion = accountRegion;
            AttributeMappings = attributeMappings;
        }

        public MigrationConfig(Dictionary<string, object> config)
        {
            if (int.TryParse((string)Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_STATE_KEY), out int state))
            {
                State = (MigrationState)state;
            }
            else
            {
                State = MigrationState.Undefined;
            }

            AccountId = (string)Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_ID_KEY);
            AccountToken = (string)Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_TOKEN_KEY);
            AccountRegion = (string)Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_REGION_KEY);

            object attributeValue = Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_ATTRIBUTES_KEY);
            if (attributeValue is Dictionary<string, object>)
            {
                AttributeMappings = new Dictionary<string, string>();
                foreach (KeyValuePair<string, object> entry in (attributeValue as Dictionary<string, object>)) {
                    if (entry.Value is string)
                    {
                        AttributeMappings.Add(entry.Key, entry.Value as string);
                    }
                }
            }
            else if (attributeValue != null)
            {
                UnityEngine.Debug.Log($"MigrationConfig: expected Dictionary<string,object> but got {attributeValue.GetType()}");
            }
        }

        public MigrationState State { get; }
        public string AccountId { get; }
        public string AccountToken { get; }
        public string AccountRegion { get; }
        public Dictionary<string, string> AttributeMappings { get; }
    }
}