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

#define LEANPLUM_CLIENT @"unity-nativeios"

typedef void (*LeanplumRequestAuthorization)
(id, SEL, unsigned long long, void (^)(BOOL, NSError *__nullable));

@interface Leanplum()
+ (void)_initPush;
+ (void)setClient:(NSString *)client withVersion:(NSString *)version;
@end

__attribute__ ((__constructor__)) static void leanplum_push_swizzle(void) {
    [[NSNotificationCenter defaultCenter] addObserver:[Leanplum class] selector:@selector(_initPush)
                                                 name:@"UIApplicationDidFinishLaunchingNotification" object:nil];
}

static char *__LPgameObject;
static NSMutableArray *__LPVariablesCache = [NSMutableArray array];

static char *leanplum_cStringCopy(const char *string)
{
    if (string == NULL) {
        return NULL;
    }
    char *res = (char *)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

static NSString *leanplum_createNSString(const char *string)
{
    if (string != NULL) {
        return [NSString stringWithUTF8String:string];
    } else {
        return [NSString stringWithUTF8String:""];
    }
}

static char *leanplum_cStringFromObject(id obj)
{
    if (!obj) {
        return NULL;
    }

    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:obj
                                                       options:NSUTF8StringEncoding
                                                         error:nil];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData
                                                 encoding:NSUTF8StringEncoding];
    return leanplum_cStringCopy([jsonString UTF8String]);
}


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
        // iOS 10.0, tvOS 10.0, macOS 10.14
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 100000 || __TV_OS_VERSION_MAX_ALLOWED >= 100000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101400
        if (@available(iOS 10, tvOS 10, macOS 10.14, *))
        {
            UNUserNotificationCenter *notificationCenter = [UNUserNotificationCenter currentNotificationCenter];
            UNAuthorizationOptions options = UNAuthorizationOptionAlert | UNAuthorizationOptionBadge | UNAuthorizationOptionSound;
            [notificationCenter requestAuthorizationWithOptions:options completionHandler:^(BOOL granted, NSError * _Nullable error) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    if (granted) {
                        [[UIApplication sharedApplication] registerForRemoteNotifications];
                    } else {
                        NSLog(@"Leanplum: Failed to request authorization for user notifications: %@", error ? error : @"nil");
                    }
                });
            }];
            
            return;
        }
#endif
        
        // iOS 8.0
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 80000
        if (@available(iOS 8.0, *))
        {
            UIUserNotificationType notificationTypes = UIUserNotificationTypeAlert | UIUserNotificationTypeBadge | UIUserNotificationTypeSound;
            UIUserNotificationSettings *settings = [UIUserNotificationSettings settingsForTypes:notificationTypes categories:nil];
            [[UIApplication sharedApplication] registerUserNotificationSettings:settings];
            [[UIApplication sharedApplication] registerForRemoteNotifications];
            
            return;
        }
#endif
        
        // iOS 3.0
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 30000
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
        UIRemoteNotificationType remoteNotificationTypes = UIRemoteNotificationTypeAlert | UIRemoteNotificationTypeBadge | UIRemoteNotificationTypeSound;
        [[UIApplication sharedApplication] registerForRemoteNotificationTypes:remoteNotificationTypes];
#pragma clang diagnostic pop
#endif

    }

    void _setAppIdDeveloper(const char *appId, const char *accessKey)
    {
        NSString *NSSAppId = leanplum_createNSString(appId);
        NSString *NSSAccessKey = leanplum_createNSString(accessKey);

        [Leanplum setAppId:NSSAppId withDevelopmentKey:NSSAccessKey];
    }

    void _setAppIdProduction(const char *appId, const char *accessKey)
    {
        NSString *NSSAppId = leanplum_createNSString(appId);
        NSString *NSSAccessKey = leanplum_createNSString(accessKey);

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
        [Leanplum setApiHostName:leanplum_createNSString(hostName)
                 withServletName:leanplum_createNSString(servletName) usingSsl:[@(useSSL) boolValue]];
    }

    void _setNetworkTimeout(int seconds, int downloadSeconds)
    {
        [Leanplum setNetworkTimeoutSeconds:seconds forDownloads:downloadSeconds];
    }

    void _setAppVersion(const char *version)
    {
        [Leanplum setAppVersion:leanplum_createNSString(version)];
    }

    void _setDeviceId(const char *deviceId)
    {
        [Leanplum setDeviceId:leanplum_createNSString(deviceId)];
    }

    void _setTestModeEnabled(bool isTestModeEnabled)
    {
        [Leanplum setTestModeEnabled:isTestModeEnabled];
    }

    void _setTrafficSourceInfo(const char *dictStringJSON)
    {
        NSData *data = [leanplum_createNSString(dictStringJSON)
                        dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        [Leanplum setTrafficSourceInfo:dictionary];
    }

    void _advanceTo(const char *state, const char *info, const char *dictStringJSON)
    {
        NSData *data = [leanplum_createNSString(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        [Leanplum advanceTo:leanplum_createNSString(state)
                   withInfo:leanplum_createNSString(info) andParameters:dictionary];
    }

    void _setUserAttributes(const char *newUserId, const char *dictStringJSON)
    {
        NSData *data = [leanplum_createNSString(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        [Leanplum setUserId:leanplum_createNSString(newUserId) withUserAttributes:dictionary];
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
        return leanplum_cStringFromObject([Leanplum variants]);
    }

    const char * _messageMetadata()
    {
        return leanplum_cStringFromObject([Leanplum messageMetadata]);
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
    }

    void _start(const char *sdkVersion, const char *userId, const char *dictStringJSON)
    {
        [Leanplum setClient:LEANPLUM_CLIENT withVersion:leanplum_createNSString(sdkVersion)];

        NSData *data = [leanplum_createNSString(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
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
        NSData *data = [leanplum_createNSString(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];

        [Leanplum trackPurchase:leanplum_createNSString(event) withValue:value andCurrencyCode:leanplum_createNSString(currencyCode)
          andParameters:dictionary];
    }

    void _track(const char *event, double value, const char *info, const char *dictStringJSON)
    {
        NSData *data = [leanplum_createNSString(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];

        [Leanplum track:leanplum_createNSString(event) withValue:value andInfo:leanplum_createNSString(info)
          andParameters:dictionary];
    }

    const char *_objectForKeyPath(const char *dictStringJSON)
    {
        NSData *data = [leanplum_createNSString(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        return leanplum_cStringFromObject([Leanplum objectForKeyPath:dictionary, nil]);
    }

    const char *_objectForKeyPathComponents(const char *dictStringJSON)
    {
        NSData *data = [leanplum_createNSString(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        id object = [NSJSONSerialization JSONObjectWithData:data
                                                    options:NSUTF8StringEncoding
                                                      error:nil];
        return leanplum_cStringFromObject([Leanplum objectForKeyPathComponents:object]);
    }

    void _registerVariableCallback(const char *name)
    {
        NSString *varName = leanplum_createNSString(name);
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

        NSString *actionName = leanplum_createNSString(name);
        LeanplumActionKind actionKind = (LeanplumActionKind) kind;
        
        NSData *argsData = [leanplum_createNSString(args) dataUsingEncoding:NSUTF8StringEncoding];
        NSArray *argsArray = [NSJSONSerialization JSONObjectWithData:argsData
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];

        NSData *optionsData = [leanplum_createNSString(options) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *optionsDictionary = [NSJSONSerialization JSONObjectWithData:optionsData
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        
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
        LPVar *var;
        NSString *nameString = leanplum_createNSString(name);
        NSData *data = [leanplum_createNSString(jsonValue) dataUsingEncoding:NSUTF8StringEncoding];
        NSObject *object = [NSJSONSerialization JSONObjectWithData:data
                                                           options:NSUTF8StringEncoding error:nil];

        if (strcmp(kind, "integer") == 0) {
            if (![object.class isSubclassOfClass:NSNumber.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:leanplum_createNSString(name) withInteger:[(NSNumber *)object integerValue]];
        } else if (strcmp(kind, "float") == 0) {
            if (![object.class isSubclassOfClass:NSNumber.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:leanplum_createNSString(name) withFloat:[(NSNumber *)object floatValue]];
        } else if (strcmp(kind, "bool") == 0) {
            if (![object.class isSubclassOfClass:NSNumber.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:leanplum_createNSString(name) withBool:[(NSNumber *)object boolValue]];
        } else if (strcmp(kind, "file") == 0) {
            if (![object.class isSubclassOfClass:NSString.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:leanplum_createNSString(name) withFile:(NSString *) object];
        } else if (strcmp(kind, "group") == 0) {
            if (![object.class isSubclassOfClass:NSDictionary.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:leanplum_createNSString(name) withDictionary:(NSDictionary *)object];
        } else if (strcmp(kind, "list") == 0) {
            if (![object.class isSubclassOfClass:NSArray.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:leanplum_createNSString(name) withArray:(NSArray *)object];
        } else if (strcmp(kind, "string") == 0) {
            if (![object.class isSubclassOfClass:NSString.class]) {
                NSLog(@"Leanplum: %@", [NSString stringWithFormat:
                                        @"Unsupported value %@ for variable %@", object, nameString]);
                object = nil;
            }
            var = [LPVar define:leanplum_createNSString(name) withString:(NSString *) object];
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
        LPVar *var = [LPVar define:leanplum_createNSString(name)];

        if (var == nil) {
            return NULL;
        }
        return leanplum_cStringFromObject([var objectForKeyPath:nil]);

    }
    
} // extern "C"
