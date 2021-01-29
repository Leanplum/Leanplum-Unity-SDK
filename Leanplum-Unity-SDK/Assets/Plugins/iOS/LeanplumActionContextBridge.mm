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

#import <Foundation/Foundation.h>
#import <Leanplum/Leanplum.h>
#import "LeanplumActionContextBridge.h"
#import "LeanplumUnityHelper.h"

static NSMutableDictionary<NSString *, LPActionContext *> *actionContexts;

@implementation LeanplumActionContextBridge

+ (NSMutableDictionary<NSString *, LPActionContext *> *) sharedActionContexts
{
    if (actionContexts == nil) actionContexts = [[NSMutableDictionary alloc] init];
    return actionContexts;
}

@end

extern "C"
{

char * get_action_name(const char *contextId)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return lp::to_string([context actionName]);
}

char * get_string_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return lp::to_string([context stringNamed:lp::to_nsstring(name)]);
}

int get_int_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return [[context numberNamed:lp::to_nsstring(name)] intValue];
}

float get_float_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return [[context numberNamed:lp::to_nsstring(name)] floatValue];
}

double get_double_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return [[context numberNamed:lp::to_nsstring(name)] doubleValue];
}

int64_t get_long_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return [[context numberNamed:lp::to_nsstring(name)] longLongValue];
}

int8_t get_byte_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return [[context numberNamed:lp::to_nsstring(name)] charValue];
}

int16_t get_short_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return [[context numberNamed:lp::to_nsstring(name)] charValue];
}

bool get_bool_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return [context boolNamed:lp::to_nsstring(name)];
}

long get_color_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    UIColor *color = [context colorNamed:lp::to_nsstring(name)];
    long intVal = lp::leanplum_colorToInt(color);
    return intVal;
}

char * get_file_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return lp::to_string([context fileNamed:lp::to_nsstring(name)]);
}

char *get_dictionary_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return lp::to_json_string([context dictionaryNamed:lp::to_nsstring(name)]);
}

char *get_array_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return lp::to_json_string([context arrayNamed:lp::to_nsstring(name)]);
}

char *get_html_with_template_named(const char *contextId, const char *name)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    return lp::to_string([[context htmlWithTemplateNamed:lp::to_nsstring(name)] absoluteString]);
}

void run_action_named(const char *contextId, const char *actionName)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    [context runActionNamed:lp::to_nsstring(actionName)];
}

void run_tracked_action_named(const char *contextId, const char *actionName)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    [context runActionNamed:lp::to_nsstring(actionName)];
}

void track_event(const char *contextId, const char *event, double value, const char *params)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];

    NSData *paramData = [lp::to_nsstring(params) dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *parameters = [NSJSONSerialization JSONObjectWithData:paramData
                                                               options:NSUTF8StringEncoding
                                                                 error:nil];

    [context track:lp::to_nsstring(event) withValue:value andParameters:parameters];
}

void track_message_event(const char *contextId, const char *event, double value, const char *info, const char *params)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];

    NSData *paramData = [lp::to_nsstring(params) dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *parameters = [NSJSONSerialization JSONObjectWithData:paramData
                                                               options:NSUTF8StringEncoding
                                                                 error:nil];

    [context trackMessageEvent:lp::to_nsstring(event) withValue:value andInfo:lp::to_nsstring(info) andParameters:parameters];
}

void mute_future_messages_of_same_kind(const char *contextId)
{
    LPActionContext *context = [actionContexts objectForKey:lp::to_nsstring(contextId)];
    [context muteFutureMessagesOfSameKind];
}

}
