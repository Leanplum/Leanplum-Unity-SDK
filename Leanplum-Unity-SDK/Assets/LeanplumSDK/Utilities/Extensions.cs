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
using System.Collections.Generic;

namespace LeanplumSDK
{
    public static class Extensions
    {
        public static void Merge<K, V>(this IDictionary<K, V> a,
            IDictionary<K, V> b,
            bool overwrite = true)
        {
            foreach (KeyValuePair<K, V> p in b)
            {
                if (overwrite || !a.ContainsKey(p.Key))
                {
                    a[p.Key] = p.Value;
                }
            }
        }

        private static string GetUnixTimestamp(System.DateTime dateTime)
        {
            // Get the offset from current time in UTC time
            System.DateTimeOffset dto = new System.DateTimeOffset(dateTime);
            // Get the unix timestamp in seconds, and add the milliseconds
            return dto.ToUnixTimeMilliseconds().ToString();
        }

        public static IDictionary<string, object> ConvertDateObjects(this IDictionary<string, object> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
                return dictionary;

            IDictionary<string, object> converted = new Dictionary<string, object>(dictionary);

            foreach (KeyValuePair<string, object> entry in dictionary)
            {
                if (entry.Value is System.DateTime time)
                {
                    converted[entry.Key] = "lp_date_" + GetUnixTimestamp(time);
                }
            }
            return converted;
        }
    }
}
