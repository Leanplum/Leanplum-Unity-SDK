//
//  Copyright (c) 2014 Leanplum. All rights reserved.
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
#import <Leanplum/LPPushNotificationsManager.h>

#define LEANPLUM_CLIENT @"unity-nativeios"

namespace lp 
{
    static char *copy_string(const char *str)
    {
        if (str == NULL) 
        {
            return NULL;
        }
        char *res = (char *) malloc(strlen(str) + 1);
        strcpy(res, str);
        return res;
    }

    static NSString *to_nsstring(const char *str)
    {
        if (str != NULL) {
            return [NSString stringWithUTF8String:str];
        } else {
            return [NSString stringWithUTF8String:""];
        }
    }

    static char *to_string(NSString *str)
    {
        return copy_string([str UTF8String]);
    }

    static char *to_json_string(id obj)
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
}

typedef void (*LeanplumRequestAuthorization)
(id, SEL, unsigned long long, void (^)(BOOL, NSError *__nullable));

@interface Leanplum()
+ (void)setClient:(NSString *)client withVersion:(NSString *)version;
@end

__attribute__ ((__constructor__)) static void initialize_bridge(void)
{
    [LPPushNotificationsManager sharedManager];
}

static char *__LPgameObject;
static NSMutableArray *__LPVariablesCache = [NSMutableArray array];

// Variable Delegate class
@interface LPUnityVarDelegate : NSObject <LPVarDelegate>
@end

@implementation LPUnityVarDelegate
/**
 * Called when the value of the variable changes.
 */
- (void)valueDidChange:(LPVar *)var
{
    UnitySendMessage(__LPgameObject, "NativeCallback",
                     [[NSString stringWithFormat:@"VariableValueChanged:%@", var.name] UTF8String]);
}

@end
// Variable Delegate class END

extern "C"
{
    /**
     * Leanplum bridge public methods implementation
     */

    void _registerForNotifications()
    {
        [[LPPushNotificationsManager sharedManager] enableSystemPush];
    }

    void _setAppIdDeveloper(const char *appId, const char *accessKey)
    {
        
        NSString *NSSAppId = lp::to_nsstring(appId);
        NSString *NSSAccessKey = lp::to_nsstring(accessKey);

        [Leanplum setAppId:NSSAppId withDevelopmentKey:NSSAccessKey];
    }

    void _setAppIdProduction(const char *appId, const char *accessKey)
    {
        NSString *NSSAppId = lp::to_nsstring(appId);
        NSString *NSSAccessKey = lp::to_nsstring(accessKey);

        [Leanplum setAppId:NSSAppId withProductionKey:NSSAccessKey];
    }

    bool _hasStarted()
    {
        return [Leanplum hasStarted];
    }

    bool _hasStartedAndRegisteredAsDeveloper()
    {
        return [Leanplum hasStartedAndRegisteredAsDeveloper];
    }

    void _setApiHostName(const char *hostName, const char *servletName, int useSSL)
    {
        [Leanplum setApiHostName:lp::to_nsstring(hostName)
                 withServletName:lp::to_nsstring(servletName) usingSsl:[@(useSSL) boolValue]];
    }

    void _setNetworkTimeout(int seconds, int downloadSeconds)
    {
        [Leanplum setNetworkTimeoutSeconds:seconds forDownloads:downloadSeconds];
    }

    void _setAppVersion(const char *version)
    {
        [Leanplum setAppVersion:lp::to_nsstring(version)];
    }

    void _setDeviceId(const char *deviceId)
    {
        [Leanplum setDeviceId:lp::to_nsstring(deviceId)];
    }

    const char *_getDeviceId()
    {
        return lp::to_string([Leanplum deviceId]);
    }

    const char *_getUserId()
    {
        return lp::to_string([Leanplum userId]);
    }

    void _setTestModeEnabled(bool isTestModeEnabled)
    {
        [Leanplum setTestModeEnabled:isTestModeEnabled];
    }

    void _setTrafficSourceInfo(const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON)
                        dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        [Leanplum setTrafficSourceInfo:dictionary];
    }

    void _advanceTo(const char *state, const char *info, const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        [Leanplum advanceTo:lp::to_nsstring(state)
                   withInfo:lp::to_nsstring(info) andParameters:dictionary];
    }

    void _setUserAttributes(const char *newUserId, const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        [Leanplum setUserId:lp::to_nsstring(newUserId) withUserAttributes:dictionary];
    }

    void _pauseState()
    {
        [Leanplum pauseState];
    }

    void _resumeState()
    {
        [Leanplum resumeState];
    }

    const char * _variants()
    {
        return lp::to_json_string([Leanplum variants]);
    }

    const char * _messageMetadata()
    {
        return lp::to_json_string([Leanplum messageMetadata]);
    }

    void _forceContentUpdate()
    {
        [Leanplum forceContentUpdate];
    }

    void _forceContentUpdateWithCallback(int key)
    {
        [Leanplum forceContentUpdate:^() {
            UnitySendMessage(__LPgameObject, "NativeCallback",
                             [[NSString stringWithFormat:@"ForceContentUpdateWithCallback:%d", key] UTF8String]);
        }];
    }
    
    void _setDeviceLocationWithLatitude(double latitude, double longitude)
    {
        [Leanplum setDeviceLocationWithLatitude: latitude
                                      longitude: longitude];
    }
    
    void _disableLocationCollection()
    {
        [Leanplum disableLocationCollection];
    }

    void _setGameObject(const char *gameObject)
    {
        __LPgameObject = (char *)malloc(strlen(gameObject) + 1);
        strcpy(__LPgameObject, gameObject);
    }

    // Leanplum start actions
    void LeanplumSetupCallbackBlocks()
    {
        [Leanplum onVariablesChanged:^{
            UnitySendMessage(__LPgameObject, "NativeCallback", "VariablesChanged:");
        }];

        [Leanplum onVariablesChangedAndNoDownloadsPending:^{
            UnitySendMessage(__LPgameObject, "NativeCallback",
                             "VariablesChangedAndNoDownloadsPending:");
        }];

        [[Leanplum inbox] onChanged:^{
            UnitySendMessage(__LPgameObject, "NativeCallback",
                             [@"InboxOnChanged" UTF8String]);
        }];

        [[Leanplum inbox] onForceContentUpdate:^(BOOL success) {
            int res = [@(success) intValue];
            UnitySendMessage(__LPgameObject, "NativeCallback",
                                 [[NSString stringWithFormat:@"InboxForceContentUpdate:%d", res] UTF8String]);
        }];
    }

    void _start(const char *sdkVersion, const char *userId, const char *dictStringJSON)
    {
        [Leanplum setClient:LEANPLUM_CLIENT withVersion:lp::to_nsstring(sdkVersion)];

        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        
        NSString *userIdString = userId != NULL ? [NSString stringWithUTF8String:userId] : nil;
        [Leanplum startWithUserId:userIdString userAttributes:dictionary
                  responseHandler:^(BOOL success) {
                      int res = [@(success) intValue];
                      UnitySendMessage(__LPgameObject, "NativeCallback",
                                       [[NSString stringWithFormat:@"Started:%d", res] UTF8String]);
                  }];
        LeanplumSetupCallbackBlocks();
    }

    void _trackIOSInAppPurchases()
    {
        [Leanplum trackInAppPurchases];
    }

    void _trackPurchase(const char *event, double value, const char *currencyCode, const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];

        [Leanplum trackPurchase:lp::to_nsstring(event) withValue:value andCurrencyCode:lp::to_nsstring(currencyCode)
          andParameters:dictionary];
    }

    void _track(const char *event, double value, const char *info, const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];

        [Leanplum track:lp::to_nsstring(event) withValue:value andInfo:lp::to_nsstring(info)
          andParameters:dictionary];
    }

    const char *_objectForKeyPath(const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        return lp::to_json_string([Leanplum objectForKeyPath:dictionary, nil]);
    }

    const char *_objectForKeyPathComponents(const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        id object = [NSJSONSerialization JSONObjectWithData:data
                                                    options:NSUTF8StringEncoding
                                                      error:nil];
        return lp::to_json_string([Leanplum objectForKeyPathComponents:object]);
    }

    void _registerVariableCallback(const char *name)
    {
        NSString *varName = lp::to_nsstring(name);
        for (int i = 0; i < __LPVariablesCache.count; i++) {
            LPVar *var = [__LPVariablesCache objectAtIndex:i];
            if ([var.name isEqualToString:varName]) {
                // Create a delegate and set it to the variable.
                [var setDelegate:[[LPUnityVarDelegate alloc] init]];
                return;
            }
        }
    }

    void _defineAction(const char* name, int kind, const char *args, const char *options)
    {
        if (name == nil) {
            NSLog(@"_defineAction: name provided is nil");
            return;
        }

        NSString *actionName = lp::to_nsstring(name);
        LeanplumActionKind actionKind = (LeanplumActionKind) kind;
        
        NSData *argsData = [lp::to_nsstring(args) dataUsingEncoding:NSUTF8StringEncoding];
        NSArray *argsArray = [NSJSONSerialization JSONObjectWithData:argsData
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];

        NSData *optionsData = [lp::to_nsstring(options) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *optionsDictionary = [NSJSONSerialization JSONObjectWithData:optionsData
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        if (optionsDictionary == nil) {
            optionsDictionary = @{};
        }

        
        NSMutableArray *arguments = [NSMutableArray new];
        
        static NSString *LP_KIND_INT = @"integer";
        static NSString *LP_KIND_FLOAT = @"float";
        static NSString *LP_KIND_STRING = @"string";
        static NSString *LP_KIND_BOOLEAN = @"bool";
        static NSString *LP_KIND_DICTIONARY = @"group";
        static NSString *LP_KIND_ARRAY = @"list";
        static NSString *LP_KIND_ACTION = @"action";
        static NSString *LP_KIND_COLOR = @"color";
        
        for (NSDictionary* arg in argsArray)
        {
            NSString* argName = arg[@"name"];
            NSString* argKind = arg[@"kind"];
            id defaultValue = arg[@"defaultValue"];
            
            if (argName == nil || argKind == nil || defaultValue == nil)
            {
                continue;
            }
            
            if ([argKind isEqualToString:LP_KIND_ACTION] && [defaultValue isKindOfClass:[NSString class]])
            {
                NSString* actionValue = (NSString*) defaultValue;
                [arguments addObject:[LPActionArg argNamed:argName withAction:actionValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_INT] && [defaultValue isKindOfClass:[NSNumber class]])
            {
                NSNumber* intValue = (NSNumber*) defaultValue;
                [arguments addObject:[LPActionArg argNamed:argName withNumber:intValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_FLOAT] && [defaultValue isKindOfClass:[NSNumber class]])
            {
                NSNumber* floatValue = (NSNumber*) defaultValue;
                [arguments addObject:[LPActionArg argNamed:argName withNumber:floatValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_STRING] && [defaultValue isKindOfClass:[NSString class]])
            {
                NSString* stringValue = (NSString*) defaultValue;
                [arguments addObject:[LPActionArg argNamed:argName withString:stringValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_BOOLEAN])
            {
                BOOL boolValue = [defaultValue boolValue];
                [arguments addObject:[LPActionArg argNamed:argName withBool:boolValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_DICTIONARY])
            {
                [arguments addObject:[LPActionArg argNamed:argName withDict:defaultValue]];
            }
            else if ([argKind isEqualToString:LP_KIND_ARRAY])
            {
                [arguments addObject:[LPActionArg argNamed:argName withArray:defaultValue]];
            }
        }
        
        [Leanplum defineAction:actionName
                        ofKind:actionKind
                 withArguments:arguments
                   withOptions:optionsDictionary
                 withResponder:^BOOL(LPActionContext *context) {
                     // Propagate back event to unity layer
                     UnitySendMessage(__LPgameObject, "NativeCallback",
                                      [[NSString stringWithFormat:@"ActionResponder:%@", actionName] UTF8String]);

                     return YES;
                 }];
    }

    // Leanplum Content
    void _defineVariable(const char *name, const char *kind, const char *jsonValue)
    {
        LPVar *var = nil;
        NSString *nameString = lp::to_nsstring(name);
        NSData *data = [lp::to_nsstring(jsonValue) dataUsingEncoding:NSUTF8StringEncoding];
        NSObject *object = [NSJSONSerialization JSONObjectWithData:data
                                                           options:NSUTF8StringEncoding error:nil];

        if (strcmp(kind, "integer") == 0) {
            if (![object.class isSubclassOfClass:NSNumber.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withInteger:[(NSNumber *)object integerValue]];
        } else if (strcmp(kind, "float") == 0) {
            if (![object.class isSubclassOfClass:NSNumber.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withFloat:[(NSNumber *)object floatValue]];
        } else if (strcmp(kind, "bool") == 0) {
            if (![object.class isSubclassOfClass:NSNumber.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withBool:[(NSNumber *)object boolValue]];
        } else if (strcmp(kind, "file") == 0) {
            if (![object.class isSubclassOfClass:NSString.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withFile:(NSString *) object];
        } else if (strcmp(kind, "group") == 0) {
            if (![object.class isSubclassOfClass:NSDictionary.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withDictionary:(NSDictionary *)object];
        } else if (strcmp(kind, "list") == 0) {
            if (![object.class isSubclassOfClass:NSArray.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withArray:(NSArray *)object];
        } else if (strcmp(kind, "string") == 0) {
            if (![object.class isSubclassOfClass:NSString.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:lp::to_nsstring(name) withString:(NSString *) object];
        } else {
            NSLog(@"Leanplum: Unsupported type %s", kind);
            return;
        }

        static LPUnityVarDelegate* delegate = nil;
        if (!delegate) {
            delegate = [[LPUnityVarDelegate alloc] init];
        }

        [__LPVariablesCache addObject:var];

        [var setDelegate:delegate];
    }

    const char *_getVariableValue(const char *name, const char *kind)
    {
        LPVar *var = [LPVar define:lp::to_nsstring(name)];

        if (var == nil) {
            return NULL;
        }
        return lp::to_json_string([var objectForKeyPath:nil]);
    }

    int _inbox_count()
    {
        return (int) [Leanplum inbox].count;
    }
    
    int _inbox_unreadCount()
    {
        return (int) [Leanplum inbox].unreadCount;
    }

    const char *_inbox_messageIds()
    {
        return lp::to_json_string([Leanplum inbox].messagesIds);
    }

    const char *_inbox_messages()
    {
        NSMutableArray<NSDictionary *> *messageData = [NSMutableArray new];
        NSArray<LPInboxMessage *> *messages = [Leanplum inbox].allMessages;

        NSDateFormatter *formatter = [NSDateFormatter new];
        [formatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ssZZZZZ"];

        for (LPInboxMessage *message : messages) {
            NSString *expirationTimestamp = nil;
            NSString *deliveryTimestamp = nil;

            if (message.deliveryTimestamp) {
                deliveryTimestamp = [formatter stringFromDate:message.deliveryTimestamp];
            }
            if (message.expirationTimestamp) {
                expirationTimestamp = [formatter stringFromDate:message.expirationTimestamp];
            }

            expirationTimestamp = nil;

            NSDictionary *data = @{
                @"id": message.messageId,
                @"title": message.title,
                @"subtitle": message.subtitle,
                @"imageFilePath": message.imageFilePath ?: @"",
                @"imageURL": [message.imageURL absoluteString] ?: @"",
                @"deliveryTimestamp": deliveryTimestamp ?: [NSNull null],
                @"expirationTimestamp": expirationTimestamp ?: [NSNull null],
                @"isRead": @(message.isRead),
            };
            [messageData addObject:data];
        }
        return lp::to_json_string(messageData);
    }

    void _inbox_read(const char *messageId)
    {
        NSString *msgId = lp::to_nsstring(messageId);
        LPInboxMessage *msg = [[Leanplum inbox] messageForId:msgId];
        if (msg) {
            [msg read];
        }
    }

    void _inbox_markAsRead(const char *messageId)
    {
        NSString *msgId = lp::to_nsstring(messageId);
        LPInboxMessage *msg = [[Leanplum inbox] messageForId:msgId];
        if (msg) {
            // todo: fix to mark message as read
            [msg read];
        }
    }

    void _inbox_remove(const char *messageId)
    {
        NSString *msgId = lp::to_nsstring(messageId);
        LPInboxMessage *msg = [[Leanplum inbox] messageForId:msgId];
        if (msg) {
            [msg remove];
        }
    }

    void _inbox_disableImagePrefetching()
    {
        [[Leanplum inbox] disableImagePrefetching];
    }
} // extern "C"
