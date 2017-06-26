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
        // Otherwise use boilerplate code from docs.
        id notificationCenterClass = NSClassFromString(@"UNUserNotificationCenter");
        if (notificationCenterClass) {
            // iOS 10.
            SEL selector = NSSelectorFromString(@"currentNotificationCenter");
            id notificationCenter =
            ((id (*)(id, SEL)) [notificationCenterClass methodForSelector:selector])
            (notificationCenterClass, selector);
            if (notificationCenter) {
                selector =
                NSSelectorFromString(@"requestAuthorizationWithOptions:completionHandler:");
                IMP method = [notificationCenter methodForSelector:selector];
                LeanplumRequestAuthorization func =
                (LeanplumRequestAuthorization) method;
                func(notificationCenter, selector,
                     0b111, /* badges, sounds, alerts */
                     ^(BOOL granted, NSError *__nullable error) {
                         if (error) {
                             NSLog(@"Leanplum: Failed to request authorization for user "
                                   "notifications: %@", error);
                         }
                     });
            }
            [[UIApplication sharedApplication] registerForRemoteNotifications];
        } else if ([[UIApplication sharedApplication] respondsToSelector:
                    @selector(registerUserNotificationSettings:)]) {
            // iOS 8-9.
            UIUserNotificationSettings *settings = [UIUserNotificationSettings
                                                    settingsForTypes:UIUserNotificationTypeAlert |
                                                    UIUserNotificationTypeBadge |
                                                    UIUserNotificationTypeSound categories:nil];
            [[UIApplication sharedApplication] registerUserNotificationSettings:settings];
            [[UIApplication sharedApplication] registerForRemoteNotifications];
        } else {
            // iOS 7 and below.
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
            [[UIApplication sharedApplication] registerForRemoteNotificationTypes:
#pragma clang diagnostic pop
             UIUserNotificationTypeSound | UIUserNotificationTypeAlert |
             UIUserNotificationTypeBadge];
        }
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
        [Leanplum startWithUserId:leanplum_createNSString(userId) userAttributes:dictionary
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
