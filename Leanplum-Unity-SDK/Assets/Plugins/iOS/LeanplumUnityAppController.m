//
//  Copyright (c) 2025 Leanplum. All rights reserved.
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

#import "LeanplumUnityAppController.h"
#import "CleverTapUnityManager.h"
#import "CleverTapCustomTemplates.h"
#import <Leanplum/LeanplumInternal.h>
#import <Leanplum/Leanplum-Swift.h>

@implementation LeanplumUnityAppController

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    // Register Custom Templates and App Functions in CleverTap.
    // The CleverTap TemplateProducer is called when CleverTap instance is created.
    [CleverTapCustomTemplates registerCustomTemplates];
    
    [UNUserNotificationCenter currentNotificationCenter].delegate = (id <UNUserNotificationCenterDelegate>)self;
    
    return [super application:application didFinishLaunchingWithOptions:launchOptions];
}

- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options {
    // Call the CleverTapUnityManager to handle the URL if in Migration
    [self handleWithCleverTapInstance:^{
        [[CleverTapUnityManager sharedInstance] handleOpenURL:url];
    }];
    return [super application:app openURL:url options:options];
}

- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult))completionHandler {
    // Check if the push notification is sent from CleverTap.
    // Call CleverTapUnityManager if in Migration.
    if ([self isCleverTapPushNotification:userInfo]) {
        [self handleWithCleverTapInstance:^{
            BOOL isOpen = application.applicationState == UIApplicationStateActive ? NO : YES;
            [[CleverTapUnityManager sharedInstance] sendRemoteNotificationCallbackToUnity:userInfo isOpen:isOpen];
        }];
    }
    
    [super application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:completionHandler];
}

- (void)userNotificationCenter:(UNUserNotificationCenter *)center didReceiveNotificationResponse:(UNNotificationResponse *)response withCompletionHandler:(void (^)(void))completionHandler {
    NSDictionary *userInfo = response.notification.request.content.userInfo;
    // Check if the push notification is sent from CleverTap.
    // Call CleverTapUnityManager if in Migration.
    if ([self isCleverTapPushNotification:userInfo]) {
        [self handleWithCleverTapInstance:^{
            [[CleverTapUnityManager sharedInstance]
             sendRemoteNotificationCallbackToUnity:userInfo isOpen:YES];
        }];
    }
    completionHandler();
}

- (void)userNotificationCenter:(UNUserNotificationCenter *)center willPresentNotification:(UNNotification *)notification withCompletionHandler:(void (^)(UNNotificationPresentationOptions))completionHandler {
    NSDictionary *userInfo = notification.request.content.userInfo;
    // Check if the push notification is sent from CleverTap.
    // Call CleverTapUnityManager if in Migration.
    if ([self isCleverTapPushNotification:userInfo]) {
        [self handleWithCleverTapInstance:^{
            [[CleverTapUnityManager sharedInstance] didReceiveRemoteNotification:userInfo isOpen:YES openInForeground:YES];
        }];
    }
    completionHandler(UNNotificationPresentationOptionNone);
}

/**
 * Call a block if app is in Migration.
 * If CleverTap instance has launched, call the block,
 * otherwise wait for Leanplum start and check if in Migration.
 * @param block The block to execute.
 * @remark Copies LPCTNotificationManager.handleWithCleverTapInstance logic
 *  which requires casting the notifications manager to LPCTNotificationManager.
 */
- (void)handleWithCleverTapInstance:(void (^)(void))block {
    if ([[MigrationManager shared] hasLaunched]) {
        block();
    } else {
        [Leanplum onStartIssued:^{
            if ([[MigrationManager shared] useCleverTap]) {
                block();
            }
        }];
    }
}

/**
 * Checks if a push notification is sent from CleverTap.
 * @param notification The push notification user info.
 * @remark Copies CleverTap:isCTPushNotification: which requires a CleverTap instance.
 */
- (BOOL)isCleverTapPushNotification:(NSDictionary *)notification {
    for (NSString *key in [notification allKeys]) {
        if ([key hasPrefix:@"W$"] || [key hasPrefix:@"wzrk_"]) {
            return YES;
        }
    }
    return NO;
}

@end
