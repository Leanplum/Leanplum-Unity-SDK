//
//  Copyright (c) 2022 Leanplum. All rights reserved.
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
#import <Leanplum/LPActionContext.h>
#import <Leanplum/LPInternalState.h>
#import "LeanplumUnityHelper.h"
#import "LeanplumActionContextBridge.h"
#import "LeanplumIOSBridge.h"
#import "LeanplumUnityConstants.h"
#import <Leanplum/Leanplum-Swift.h>

#define LEANPLUM_CLIENT @"unity-nativeios"

__attribute__ ((__constructor__)) static void initialize_bridge(void)
{
}

static char *__LPgameObject;
static NSMutableArray *__LPVariablesCache = [NSMutableArray array];
const char *__NativeCallbackMethod = "NativeCallback";

@interface Leanplum()
typedef void (^LeanplumHandledBlock)(BOOL success);
+ (void)setClient:(NSString *)client withVersion:(NSString *)version;
+ (LPActionContext *)createActionContextForMessageId:(NSString *)messageId;
@end

// Variable Delegate class
@interface LPUnityVarDelegate : NSObject <LPVarDelegate>
@end

@implementation LPUnityVarDelegate
/**
 * Called when the value of the variable changes.
 */
- (void)valueDidChange:(LPVar *)var
{
    UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                     [[NSString stringWithFormat:@"VariableValueChanged:%@", var.name] UTF8String]);
}

@end
// Variable Delegate class END

@interface LPSecuredVars (JsonKeys)
@end

@implementation LPSecuredVars (JsonKeys)
+ (NSString *)securedVarsKey
{
    return @"json";
}
+ (NSString *)securedVarsSignatureKey
{
    return @"signature";
}
@end

@implementation LeanplumIOSBridge
+ (void) sendMessageToUnity:(NSString *) messageName withKey: (NSString *)key
{
    [self sendMessageToUnity:[NSString stringWithFormat:@"%@:%@", messageName, key]];
}

+ (void) sendMessageToUnity:(NSString *) message
{
    UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                     [message UTF8String]);
}
@end

extern "C"
{
    /**
     * Leanplum bridge public methods implementation
     */
#pragma mark Setup
    void lp_setGameObject(const char *gameObject)
    {
        __LPgameObject = (char *)malloc(strlen(gameObject) + 1);
        strcpy(__LPgameObject, gameObject);
    }

    // Leanplum start actions
    void LeanplumSetupCallbackBlocks()
    {
        [Leanplum onVariablesChanged:^{
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod, "VariablesChanged:");
        }];
        
        [Leanplum onVariablesChangedAndNoDownloadsPending:^{
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                            "VariablesChangedAndNoDownloadsPending:");
        }];
        
        [[Leanplum inbox] onChanged:^{
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                            [@"InboxOnChanged" UTF8String]);
        }];
        
        [[Leanplum inbox] onForceContentUpdate:^(BOOL success) {
            int res = [@(success) intValue];
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                                [[NSString stringWithFormat:@"InboxForceContentUpdate:%d", res] UTF8String]);
        }];
    }

#pragma mark Leanplum Methods
    void lp_registerForNotifications()
    {
        [Leanplum enablePushNotifications];
    }

    void lp_setAppIdDeveloper(const char *appId, const char *accessKey)
    {
        
        NSString *NSSAppId = lp::to_nsstring(appId);
        NSString *NSSAccessKey = lp::to_nsstring(accessKey);

        [Leanplum setAppId:NSSAppId withDevelopmentKey:NSSAccessKey];
    }

    void lp_setAppIdProduction(const char *appId, const char *accessKey)
    {
        NSString *NSSAppId = lp::to_nsstring(appId);
        NSString *NSSAccessKey = lp::to_nsstring(accessKey);

        [Leanplum setAppId:NSSAppId withProductionKey:NSSAccessKey];
    }

    bool lp_hasStarted()
    {
        return [Leanplum hasStarted];
    }

    bool lp_hasStartedAndRegisteredAsDeveloper()
    {
        return [Leanplum hasStartedAndRegisteredAsDeveloper];
    }

    void lp_addOnceVariablesChangedAndNoDownloadsPendingHandler(const int key)
    {
        [Leanplum onceVariablesChangedAndNoDownloadsPending:^{
            const char *msg = [[NSString stringWithFormat:@"OnceVariablesChangedAndNoDownloadsPending:%d", key] UTF8String];
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod, msg);
        }];
    }

    void lp_setApiHostName(const char *hostName, const char *servletName, int useSSL)
    {
        [Leanplum setApiHostName:lp::to_nsstring(hostName)
                 withPath:lp::to_nsstring(servletName) usingSsl:[@(useSSL) boolValue]];
    }

    void lp_setNetworkTimeout(int seconds, int downloadSeconds)
    {
        [Leanplum setNetworkTimeoutSeconds:seconds forDownloads:downloadSeconds];
    }

    void lp_setEventsUploadInterval(int uploadInterval)
    {
        LPEventsUploadInterval interval = (LPEventsUploadInterval)uploadInterval;
        if (interval == AT_MOST_5_MINUTES || interval == AT_MOST_10_MINUTES || interval == AT_MOST_15_MINUTES) {
            [Leanplum setEventsUploadInterval:interval];
        }
    }

    void lp_setAppVersion(const char *version)
    {
        [Leanplum setAppVersion:lp::to_nsstring(version)];
    }

    void lp_setDeviceId(const char *deviceId)
    {
        [Leanplum setDeviceId:lp::to_nsstring(deviceId)];
    }

    const char *lp_getDeviceId()
    {
        return lp::to_string([Leanplum deviceId]);
    }

    const char *lp_getUserId()
    {
        return lp::to_string([Leanplum userId]);
    }

    void lp_setLogLevel(int logLevel)
    {
        [Leanplum setLogLevel:(LPLogLevel)logLevel];
    }

    void lp_setTestModeEnabled(bool isTestModeEnabled)
    {
        [Leanplum setTestModeEnabled:isTestModeEnabled];
    }

    void lp_setTrafficSourceInfo(const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON)
                        dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        [Leanplum setTrafficSourceInfo:dictionary];
    }

    void lp_advanceTo(const char *state, const char *info, const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        [Leanplum advanceTo:lp::to_nsstring(state)
                   withInfo:lp::to_nsstring(info) andParameters:dictionary];
    }

    void lp_setUserAttributes(const char *newUserId, const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];
        [Leanplum setUserId:lp::to_nsstring(newUserId) withUserAttributes:dictionary];
    }

    void lp_pauseState()
    {
        [Leanplum pauseState];
    }

    void lp_resumeState()
    {
        [Leanplum resumeState];
    }

    const char * lp_variants()
    {
        return lp::to_json_string([Leanplum variants]);
    }

    const char * lp_messageMetadata()
    {
        return lp::to_json_string([Leanplum messageMetadata]);
    }

    void lp_forceContentUpdate()
    {
        [Leanplum forceContentUpdate];
    }

    void lp_forceContentUpdateWithHandler(int key)
    {
        [Leanplum forceContentUpdateWithBlock:^(BOOL success) {
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                             [[NSString stringWithFormat:@"ForceContentUpdateWithHandler:%d:%d", key, [@(success) intValue]] UTF8String]);
        }];
    }
    
    void lp_setDeviceLocationWithLatitude(double latitude, double longitude)
    {
        [Leanplum setDeviceLocationWithLatitude: latitude
                                      longitude: longitude];
    }
    
    void lp_disableLocationCollection()
    {
        [Leanplum disableLocationCollection];
    }

    void lp_setPushDeliveryTrackingEnabled(bool enabled)
    {
        [Leanplum setPushDeliveryTrackingEnabled:enabled];
    }

    void lp_start(const char *sdkVersion, const char *userId, const char *dictStringJSON)
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
                      UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                                       [[NSString stringWithFormat:@"Started:%d", res] UTF8String]);
                  }];
        LeanplumSetupCallbackBlocks();
    }

    void lp_trackIOSInAppPurchases()
    {
        [Leanplum trackInAppPurchases];
    }

    void lp_trackPurchase(const char *event, double value, const char *currencyCode, const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];

        [Leanplum trackPurchase:lp::to_nsstring(event) withValue:value andCurrencyCode:lp::to_nsstring(currencyCode)
          andParameters:dictionary];
    }

    void lp_track(const char *event, double value, const char *info, const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                   options:NSUTF8StringEncoding
                                                                     error:nil];

        [Leanplum track:lp::to_nsstring(event) withValue:value andInfo:lp::to_nsstring(info)
          andParameters:dictionary];
    }

#pragma mark Actions
    typedef int (*ShouldDisplayCallback) (const char *);
    void lp_onShouldDisplayMessage(ShouldDisplayCallback callback)
    {
        [[LPActionManager shared] shouldDisplayMessage:^MessageDisplayChoice * _Nonnull(LPActionContext * _Nonnull context) {
            NSString *key = [LeanplumActionContextBridge addActionContext:context];
            // Call Unity
            int result = callback(lp::to_string(key));
            [[LeanplumActionContextBridge sharedActionContexts] removeObjectForKey:key];
            switch (result) {
                case 0:
                    return [MessageDisplayChoice show];
                    break;
                case 1:
                    return [MessageDisplayChoice discard];
                    break;
                case -1:
                    return [MessageDisplayChoice delayWithSeconds:-1];
                    break;
                default:
                    int delay = result - 2;
                    return [MessageDisplayChoice delayWithSeconds:delay];
                    break;
            }
        }];
    }
    
    typedef const char * (*PrioritizeMessagesCallback) (const char *, const char *);
    void lp_onPrioritizeMessages(PrioritizeMessagesCallback callback)
    {
        [[LPActionManager shared] prioritizeMessages:^NSArray<LPActionContext *> * _Nonnull(NSArray<LPActionContext *> * _Nonnull contexts, ActionsTrigger * _Nullable  actionsTrigger) {
    
            NSMutableArray<NSString *> *arr = [NSMutableArray arrayWithCapacity:contexts.count];
            [contexts enumerateObjectsUsingBlock:^(LPActionContext * _Nonnull context, NSUInteger idx, BOOL * _Nonnull stop) {
                [arr addObject:[LeanplumActionContextBridge addActionContext:context]];
            }];
            
            NSMutableDictionary *contextualValues = [[NSMutableDictionary alloc] init];
            if ([actionsTrigger contextualValues]) {
                contextualValues[@"parameters"] = actionsTrigger.contextualValues.parameters;
                contextualValues[@"arguments"] = actionsTrigger.contextualValues.arguments;
                contextualValues[@"previousAttributeValue"] = actionsTrigger.contextualValues.previousAttributeValue;
                contextualValues[@"attributeValue"] = actionsTrigger.contextualValues.attributeValue;
            }
            
            NSDictionary *trigger = @{
                @"eventName": actionsTrigger.eventName ?: @"",
                @"condition": actionsTrigger.condition ?: @[],
                @"contextualValues": contextualValues
            };
    
            NSString *csv = [arr componentsJoinedByString:@","];
    
            // Call Unity
            const char *result = callback(lp::to_string(csv), lp::to_json_string(trigger));
    
            NSArray<NSString *> *keys = [lp::to_nsstring(result) componentsSeparatedByString:@","];
            NSMutableArray<LPActionContext *> *eligibleContexts = [NSMutableArray arrayWithCapacity:keys.count];
            [keys enumerateObjectsUsingBlock:^(NSString * _Nonnull key, NSUInteger idx, BOOL * _Nonnull stop) {
                [eligibleContexts addObject:[LeanplumActionContextBridge sharedActionContexts][key]];
            }];
    
            // remove non-eligible contexts from the shared ones
            [arr removeObjectsInArray:keys];
            [arr enumerateObjectsUsingBlock:^(NSString * _Nonnull key, NSUInteger idx, BOOL * _Nonnull stop) {
                [[LeanplumActionContextBridge sharedActionContexts] removeObjectForKey:key];
            }];
    
            return eligibleContexts;
        }];
    }

    void sendMessageActionContext(NSString *messageName, LPActionContext *context)
    {
        if (context != nil) {
            NSString *key = [LeanplumActionContextBridge addActionContext:context];
            NSString *unityMessage = [NSString stringWithFormat:@"%@:%@", messageName, key];
            [LeanplumIOSBridge sendMessageToUnity:unityMessage];
        }
    }

    void lp_defineAction(const char* name, int kind, const char *args, const char *options)
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
        
        NSMutableArray *arguments = [NSMutableArray new];
        
        static NSString *lp_KIND_INT = @"integer";
        static NSString *lp_KIND_FLOAT = @"float";
        static NSString *lp_KIND_STRING = @"string";
        static NSString *lp_KIND_BOOLEAN = @"bool";
        static NSString *lp_KIND_DICTIONARY = @"group";
        static NSString *lp_KIND_ARRAY = @"list";
        static NSString *lp_KIND_ACTION = @"action";
        static NSString *lp_KIND_COLOR = @"color";
        static NSString *lp_KIND_FILE = @"file";
        
        for (NSDictionary* arg in argsArray)
        {
            NSString* argName = arg[@"name"];
            NSString* argKind = arg[@"kind"];
            id defaultValue = arg[@"defaultValue"];
            
            if (argName == nil || argKind == nil || (defaultValue == nil && ![argKind isEqualToString:lp_KIND_ACTION]))
            {
                continue;
            }
            
            if ([argKind isEqualToString:lp_KIND_ACTION])
            {
                // Allow registering an Action with null default value
                // as it is done in the iOS SDK
                NSString* actionValue = (NSString*) defaultValue;
                if ([actionValue isKindOfClass:[NSNull class]])
                {
                    actionValue = nil;
                }
                [arguments addObject:[LPActionArg argNamed:argName withAction:actionValue]];
            }
            else if ([argKind isEqualToString:lp_KIND_INT] && [defaultValue isKindOfClass:[NSNumber class]])
            {
                NSNumber* intValue = (NSNumber*) defaultValue;
                [arguments addObject:[LPActionArg argNamed:argName withNumber:intValue]];
            }
            else if ([argKind isEqualToString:lp_KIND_FLOAT] && [defaultValue isKindOfClass:[NSNumber class]])
            {
                NSNumber* floatValue = (NSNumber*) defaultValue;
                [arguments addObject:[LPActionArg argNamed:argName withNumber:floatValue]];
            }
            else if ([argKind isEqualToString:lp_KIND_STRING] && [defaultValue isKindOfClass:[NSString class]])
            {
                NSString* stringValue = (NSString*) defaultValue;
                [arguments addObject:[LPActionArg argNamed:argName withString:stringValue]];
            }
            else if ([argKind isEqualToString:lp_KIND_BOOLEAN])
            {
                BOOL boolValue = [defaultValue boolValue];
                [arguments addObject:[LPActionArg argNamed:argName withBool:boolValue]];
            }
            else if ([argKind isEqualToString:lp_KIND_DICTIONARY])
            {
                [arguments addObject:[LPActionArg argNamed:argName withDict:defaultValue]];
            }
            else if ([argKind isEqualToString:lp_KIND_ARRAY])
            {
                [arguments addObject:[LPActionArg argNamed:argName withArray:defaultValue]];
            }
            else if ([argKind isEqualToString:lp_KIND_COLOR])
            {
                long long longVal = [defaultValue longLongValue];
                [arguments addObject:[LPActionArg argNamed:argName withColor:lp::leanplum_intToColor(longVal)]];
            }
            else if ([argKind isEqualToString:lp_KIND_FILE])
            {
                [arguments addObject:[LPActionArg argNamed:argName withFile:defaultValue]];
            }
        }
        
        [Leanplum defineAction:actionName
                        ofKind:actionKind
                 withArguments:arguments
                   withOptions:optionsDictionary
                presentHandler:^BOOL(LPActionContext * _Nonnull context) {
            sendMessageActionContext(ACTION_RESPONDER, context);
            return YES;
        } dismissHandler:^BOOL(LPActionContext * _Nonnull context) {
            sendMessageActionContext(ACTION_DISMISS, context);
            return YES;
        }];
    }

    const void lp_triggerDelayedMessages()
    {
        [[LPActionManager shared] triggerDelayedMessages];
    }

    const void lp_onMessageDisplayed()
    {
        [[LPActionManager shared] onMessageDisplayed:^(LPActionContext * _Nonnull context) {
            sendMessageActionContext(ON_MESSAGE_DISPLAYED, context);
        }];
    }

    const void lp_onMessageDismissed()
    {
        [[LPActionManager shared] onMessageDismissed:^(LPActionContext * _Nonnull context) {
            sendMessageActionContext(ON_MESSAGE_DISMISSED, context);
        }];
    }

    const void lp_onMessageAction()
    {
        [[LPActionManager shared] onMessageAction:^(NSString * _Nonnull action, LPActionContext * _Nonnull context) {
            NSString *key = [LeanplumActionContextBridge addActionContext:context];
            NSString *message = [NSString stringWithFormat:@"%@:%@|%@", ON_MESSAGE_ACTION, action, key];
            [LeanplumIOSBridge sendMessageToUnity:message];
        }];
    }

    const void lp_setActionManagerPaused(bool paused)
    {
        [[LPActionManager shared] setIsPaused:paused];
    }

    const void lp_setActionManagerEnabled(bool enabled)
    {
        [[LPActionManager shared] setIsEnabled:enabled];
    }

    const char * lp_createActionContextForId(const char *actionId)
    {
        NSString *mId = lp::to_nsstring(actionId);
        LPActionContext *context = [Leanplum createActionContextForMessageId:mId];
    
        if (!context.actionName)
        {
            // Action not found
            return NULL;
        }
        NSString *key = [LeanplumActionContextBridge addActionContext:context];
        return lp::to_string(key);
    }

    bool lp_triggerAction(const char *actionId)
    {
        NSString *key = lp::to_nsstring(actionId);
        LPActionContext *context = [LeanplumActionContextBridge sharedActionContexts][key];

        if (!context)
        {
            NSPredicate *predicate = [NSPredicate predicateWithFormat:@"SELF CONTAINS[cd] %@", key];
            NSArray *keys = [[LeanplumActionContextBridge sharedActionContexts] allKeys];
            NSUInteger index = [keys indexOfObjectPassingTest:
                         ^(id obj, NSUInteger idx, BOOL *stop) {
                           return [predicate evaluateWithObject:obj];
                         }];

              if (index != NSNotFound) {
                  context = [LeanplumActionContextBridge sharedActionContexts][keys[index]];
              } else {
                  const char * newKey = lp_createActionContextForId(actionId);
                  if (newKey)
                  {
                      context = [LeanplumActionContextBridge sharedActionContexts][lp::to_nsstring(newKey)];
                  }
              }
        }

        if (context) {
            ActionsTrigger *trigger = [[ActionsTrigger alloc] initWithEventName:@"manual-trigger"
                                                                      condition:@[@"manual-trigger"]
                                                               contextualValues:nil];
            [[LPActionManager shared] triggerWithContexts:@[context] priority:PriorityDefault trigger:trigger];
            return YES;
        }
        return NO;
    }

#pragma mark Variables
    const char * lp_securedVars()
    {
        LPSecuredVars *securedVars = [Leanplum securedVars];
        if (securedVars) {
            NSDictionary *securedVarsDict = @{
                [LPSecuredVars securedVarsKey]: [securedVars varsJson],
                [LPSecuredVars securedVarsSignatureKey]: [securedVars varsSignature]
            };
            return lp::to_json_string(securedVarsDict);
        }
        return NULL;
    }
    
    const char * lp_vars()
    {
        return lp::to_json_string([[LPVarCache sharedCache] diffs]);
    }

    const char *lp_objectForKeyPath(const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data
                                                                options:NSUTF8StringEncoding
                                                                    error:nil];
        return lp::to_json_string([Leanplum objectForKeyPath:dictionary, nil]);
    }
    
    const char *lp_objectForKeyPathComponents(const char *dictStringJSON)
    {
        NSData *data = [lp::to_nsstring(dictStringJSON) dataUsingEncoding:NSUTF8StringEncoding];
        id object = [NSJSONSerialization JSONObjectWithData:data
                                                    options:NSUTF8StringEncoding
                                                    error:nil];
        return lp::to_json_string([Leanplum objectForKeyPathComponents:object]);
    }
    
    void lp_registerVariableCallback(const char *name)
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

    void lp_defineVariable(const char *name, const char *kind, const char *jsonValue)
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

    const char *lp_getVariableValue(const char *name, const char *kind)
    {
        LPVar *var = [LPVar define:lp::to_nsstring(name)];

        if (var == nil) {
            return NULL;
        }
        return lp::to_json_string([var objectForKeyPath:nil]);
    }

#pragma mark Inbox
    int lp_inbox_count()
    {
        return (int) [Leanplum inbox].count;
    }
    
    int lp_inbox_unreadCount()
    {
        return (int) [Leanplum inbox].unreadCount;
    }

    const char *lp_inbox_messageIds()
    {
        return lp::to_json_string([Leanplum inbox].messagesIds);
    }

    void lp_inbox_downloadMessages()
    {
        [[Leanplum inbox] downloadMessages];
    }

    void lp_inbox_downloadMessagesWithCallback()
    {
        [[Leanplum inbox] downloadMessages:^(BOOL success) {
            int res = [@(success) intValue];
            UnitySendMessage(__LPgameObject, __NativeCallbackMethod,
                                 [[NSString stringWithFormat:@"InboxDownloadMessages:%d", res] UTF8String]);
        }];
    }

    const char *lp_inbox_messages()
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
                @"data": message.data ?: [NSNull null]
            };
            [messageData addObject:data];
        }
        return lp::to_json_string(messageData);
    }

    void lp_inbox_read(const char *messageId)
    {
        NSString *msgId = lp::to_nsstring(messageId);
        LPInboxMessage *msg = [[Leanplum inbox] messageForId:msgId];
        if (msg) {
            [msg read];
        }
    }

    void lp_inbox_markAsRead(const char *messageId)
    {
        NSString *msgId = lp::to_nsstring(messageId);
        LPInboxMessage *msg = [[Leanplum inbox] messageForId:msgId];
        if (msg) {
            [msg markAsRead];
        }
    }

    void lp_inbox_remove(const char *messageId)
    {
        NSString *msgId = lp::to_nsstring(messageId);
        LPInboxMessage *msg = [[Leanplum inbox] messageForId:msgId];
        if (msg) {
            [msg remove];
        }
    }

    void lp_inbox_disableImagePrefetching()
    {
        [[Leanplum inbox] disableImagePrefetching];
    }
} // extern "C"
