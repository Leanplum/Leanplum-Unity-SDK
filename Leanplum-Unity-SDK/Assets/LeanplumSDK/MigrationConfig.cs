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

        public MigrationConfig(Dictionary<string, string> config)
        {
            if (int.TryParse(Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_STATE_KEY), out int state))
            {
                State = (MigrationState)state;
            }
            else
            {
                State = MigrationState.Undefined;
            }

            AccountId = Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_ID_KEY);
            AccountToken = Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_TOKEN_KEY);
            AccountRegion = Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_REGION_KEY);

            string attributeMappingsJSON = Util.GetValueOrDefault(config, Constants.Keys.MIGRATION_ATTRIBUTES_KEY);
            AttributeMappings = MiniJSON.Json.Deserialize(attributeMappingsJSON)
                as Dictionary<string, string> ?? new Dictionary<string, string>();
        }

        public MigrationState State { get; }
        public string AccountId { get; }
        public string AccountToken { get; }
        public string AccountRegion { get; }
        public Dictionary<string, string> AttributeMappings { get; }
    }
}