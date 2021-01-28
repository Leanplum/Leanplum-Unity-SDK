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

#import "LeanplumUnityHelper.h"

namespace lp 
{
    char *copy_string(const char *str)
    {
        if (str == NULL) 
        {
            return NULL;
        }
        char *res = (char *) malloc(strlen(str) + 1);
        strcpy(res, str);
        return res;
    }

    NSString *to_nsstring(const char *str)
    {
        if (str != NULL) {
            return [NSString stringWithUTF8String:str];
        } else {
            return [NSString stringWithUTF8String:""];
        }
    }

    char *to_string(NSString *str)
    {
        return copy_string([str UTF8String]);
    }

    char *to_json_string(id obj)
    {
        if (!obj) {
            return NULL;
        }

        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:obj
                                                           options:NSUTF8StringEncoding
                                                             error:nil];
        NSString *jsonString = [[NSString alloc] initWithData:jsonData
                                                     encoding:NSUTF8StringEncoding];
        return copy_string([jsonString UTF8String]);
    }

    long long leanplum_colorToInt(UIColor *value)
    {
        if (value == nil) {
            return 0;
        }
        const CGFloat *components = CGColorGetComponents(value.CGColor);
        if (CGColorGetNumberOfComponents(value.CGColor) < 4) {
            NSUInteger w = (NSUInteger)(255 * components[0]);
            NSUInteger a = (NSUInteger)(255 * components[1]);
            return (a << 24) + (w << 16) + (w << 8) + w;
        } else {
            NSUInteger r = (NSUInteger)(255 * components[0]);
            NSUInteger g = (NSUInteger)(255 * components[1]);
            NSUInteger b = (NSUInteger)(255 * components[2]);
            NSUInteger a = (NSUInteger)(255 * components[3]);
            return (a << 24) + (r << 16) + (g << 8) + b;
        }
    }

    UIColor *leanplum_intToColor(long long value)
    {
        int b = value & 0xFF;
        int g = (value >> 8) & 0XFF;
        int r = (value >> 16) & 0XFF;
        int a = (value >> 24) & 0XFF;
        return [UIColor colorWithRed:r / 255.0 green:g / 255.0 blue:b / 255.0   alpha:a / 255.0];
    }
}
