//
//  Copyright (c) 2020 Leanplum. All rights reserved.
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

#pragma once

#import <Foundation/Foundation.h>

namespace lp 
{
    char *copy_string(const char *str);
    NSString *to_nsstring(const char *str);
    char *to_string(NSString *str);
    char *to_json_string(id obj);

    // Copied from the iOS SDK otherwise cannot be linked
    long long leanplum_colorToInt(UIColor *value);
    UIColor *leanplum_intToColor(long long value);
}
