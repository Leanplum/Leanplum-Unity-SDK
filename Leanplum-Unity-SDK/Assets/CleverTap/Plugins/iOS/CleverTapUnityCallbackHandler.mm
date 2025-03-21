#import "CleverTapUnityCallbackHandler.h"
#import "CleverTapUnityCallbackInfo.h"
#import "CleverTapMessageSender.h"

@implementation CleverTapUnityCallbackHandler

+ (instancetype)sharedInstance {
    static dispatch_once_t once = 0;
    static id _sharedObject = nil;
    dispatch_once(&once, ^{
        _sharedObject = [[self alloc] init];
    });
    return _sharedObject;
}

- (instancetype)init {
    self = [super init];
    if (self) {
        [self registerListeners];
    }
    return self;
}

- (void)callUnityObject:(CleverTapUnityCallback)callback withMessage:(NSString *)message {
    [[CleverTapMessageSender sharedInstance] send:callback withMessage:message];
}

- (void)attachInstance:(CleverTap *)instance {
    if (!instance){
        return;
    }
    
    [instance setPushNotificationDelegate:self];
    [instance setInAppNotificationDelegate:self];
    [instance setDisplayUnitDelegate:self];
    [instance setPushPermissionDelegate:self];
     
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
    [[instance productConfig] setDelegate:self];
    [[instance featureFlags] setDelegate:self];
#pragma clang diagnostic pop
}

- (void)dealloc {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

#pragma mark - Utils

- (NSString *)dictToJson:(NSDictionary *)dict {
    NSError *err;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&err];
    
    if(err != nil) {
        return nil;
    }
    
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

#pragma mark - Remote Notification

- (void)didReceiveRemoteNotification:(NSData *)data isOpen:(BOOL)isOpen {
    NSString *dataString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    
    // CleverTapUnityCallbackPushReceived is not implemented and not supported
    if (!isOpen)
        return;
    
    [self callUnityObject:CleverTapUnityCallbackPushOpened withMessage:dataString];
}

- (void)pushNotificationTappedWithCustomExtras:(NSDictionary *)customExtras {
    NSString *json = [self dictToJson:customExtras];
    [self callUnityObject:CleverTapUnityCallbackPushNotificationTappedWithCustomExtras withMessage:json];
}

#pragma mark - Deeplink

- (void)deepLinkCallback:(NSString *)url {
    [self callUnityObject:CleverTapUnityCallbackDeepLink withMessage:url];
}

#pragma mark - Variables

- (CleverTapVariablesChangedBlock)variablesCallback:(CleverTapUnityCallback)callback callbackId:(int)callbackId {
    return ^{
        NSDictionary* response = @{
            @"callbackId": @(callbackId)
        };
        
        NSString* json = [self dictToJson:response];
        [self callUnityObject:callback withMessage:json];
    };
}

- (CleverTapFetchVariablesBlock)fetchVariablesBlock:(int)callbackId {
    return ^(BOOL success) {
        NSDictionary* response = @{
            @"callbackId": @(callbackId),
            @"isSuccess": @(success)
        };
        
        NSString* json = [self dictToJson:response];
        [self callUnityObject:CleverTapUnityCallbackVariablesFetched withMessage:json];
    };
}

- (CleverTapVariablesChangedBlock)variableValueChanged:(NSString *)varName {
    return ^{
        [self callUnityObject:CleverTapUnityCallbackVariableValueChanged withMessage:varName];
    };
}

- (CleverTapVariablesChangedBlock)variableFileIsReady:(NSString *)varName {
    return ^{
        [self callUnityObject:CleverTapUnityCallbackVariableFileIsReady withMessage:varName];
    };
}

#pragma mark - CleverTapSyncDelegate/Listener

- (void)registerListeners {
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didReceiveCleverTapProfileDidChangeNotification:)
                                                 name:CleverTapProfileDidChangeNotification object:nil];
    
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didReceiveCleverTapProfileDidInitializeNotification:)
                                                 name:CleverTapProfileDidInitializeNotification object:nil];
}

- (void)didReceiveCleverTapProfileDidInitializeNotification:(NSNotification*)notification {
    NSString *jsonString = [self dictToJson:notification.userInfo];
    if (jsonString != nil) {
        [self callUnityObject:CleverTapUnityCallbackProfileInitialized withMessage:jsonString];
    }
}


- (void)didReceiveCleverTapProfileDidChangeNotification:(NSNotification*)notification {
    NSString *jsonString = [self dictToJson:notification.userInfo];
    if (jsonString != nil) {
        [self callUnityObject:CleverTapUnityCallbackProfileUpdates withMessage:jsonString];
    }
}

#pragma mark - InApp Notifications

- (CleverTapFetchInAppsBlock)fetchInAppsBlock:(int)callbackId {
    return ^(BOOL success) {
        NSDictionary* response = @{
            @"callbackId": @(callbackId),
            @"isSuccess": @(success)
        };
        
        NSString* json = [self dictToJson:response];
        [self callUnityObject:CleverTapUnityCallbackInAppsFetched withMessage:json];
    };
}

#pragma mark - InApp Notification Delegate

- (void)inAppNotificationDismissedWithExtras:(NSDictionary *)extras andActionExtras:(NSDictionary *)actionExtras {
    
    NSMutableDictionary *jsonDict = [NSMutableDictionary new];
    
    if (extras != nil) {
        jsonDict[@"extras"] = extras;
    }
    
    if (actionExtras != nil) {
        jsonDict[@"actionExtras"] = actionExtras;
    }
    
    NSString *jsonString = [self dictToJson:jsonDict];
    
    if (jsonString != nil) {
        [self callUnityObject:CleverTapUnityCallbackInAppNotificationDismissed withMessage:jsonString];
    }
}

- (void)inAppNotificationButtonTappedWithCustomExtras:(NSDictionary *)customExtras {
    
    NSMutableDictionary *jsonDict = [NSMutableDictionary new];
    
    if (customExtras != nil) {
        jsonDict[@"customExtras"] = customExtras;
    }
    
    NSString *jsonString = [self dictToJson:jsonDict];
    
    if (jsonString != nil) {
        [self callUnityObject:CleverTapUnityCallbackInAppNotificationButtonTapped withMessage:jsonString];
    }
}

- (void)inAppNotificationDidShow:(NSDictionary *)notification {
    NSString *jsonString = [self dictToJson:notification];
    
    if (jsonString != nil) {
        [self callUnityObject:CleverTapUnityCallbackInAppNotificationDidShow withMessage:jsonString];
    }
}

#pragma mark - Native Display

- (void)displayUnitsUpdated:(NSArray<CleverTapDisplayUnit *>*)displayUnits {
    
    NSMutableArray *jsonArray = [NSMutableArray new];
    
    for (id object in displayUnits) {
        if ([object isKindOfClass:[CleverTapDisplayUnit class]]) {
            CleverTapDisplayUnit *unit = object;
            [jsonArray addObject:unit.json];
        }
    }
    
    NSMutableDictionary *jsonDict = [NSMutableDictionary new];
    
    if (jsonArray != nil) {
        jsonDict[@"displayUnits"] = jsonArray;
    }
    
    NSString *jsonString = [self dictToJson:jsonDict];
    
    if (jsonString != nil) {
        [self callUnityObject:CleverTapUnityCallbackNativeDisplayUnitsUpdated withMessage:jsonString];
    }
}

#pragma mark - App Inbox

- (CleverTapInboxSuccessBlock)initializeInboxBlock {
    return ^(BOOL success) {
        NSLog(@"Inbox initialized %d", success);
        [self callUnityObject:CleverTapUnityCallbackInboxDidInitialize withMessage:[NSString stringWithFormat:@"%@", success? @"YES": @"NO"]];
    };
}

- (CleverTapInboxUpdatedBlock)inboxUpdatedBlock {
    return ^{
        NSLog(@"Inbox Messages updated");
        [self callUnityObject:CleverTapUnityCallbackInboxMessagesDidUpdate withMessage:@"inbox updated."];
    };
}

- (void)messageButtonTappedWithCustomExtras:(NSDictionary *)customExtras {
    
    NSMutableDictionary *jsonDict = [NSMutableDictionary new];
    
    if (customExtras != nil) {
        jsonDict[@"customExtras"] = customExtras;
    }
    
    NSString *jsonString = [self dictToJson:jsonDict];
    
    if (jsonString != nil) {
        [self callUnityObject:CleverTapUnityCallbackInboxCustomExtrasButtonSelect withMessage:jsonString];
    }
}

- (void)messageDidSelect:(CleverTapInboxMessage *_Nonnull)message atIndex:(int)index withButtonIndex:(int)buttonIndex {
    NSMutableDictionary *body = [NSMutableDictionary new];
    if ([message json] != nil) {
        if ([message json] != nil) {
            body[@"CTInboxMessagePayload"] = [NSMutableDictionary dictionaryWithDictionary:[message json]];
        }
        body[@"ContentPageIndex"] = @(index);
        body[@"ButtonIndex"] = @(buttonIndex);
        NSString *jsonString = [self dictToJson:body];
        if (jsonString != nil) {
            [self callUnityObject:CleverTapUnityCallbackInboxItemClicked withMessage:jsonString];
        }
    }
}

#pragma mark - Push Permissions

- (void)pushPermissionCallback:(BOOL)isPushEnabled {
    [self callUnityObject:CleverTapUnityCallbackPushNotificationPermissionStatus withMessage:[NSString stringWithFormat:@"%@", isPushEnabled ? @"True": @"False"]];
}

#pragma mark - Push Permission Delegate

- (void)onPushPermissionResponse:(BOOL)accepted {
    [self callUnityObject:CleverTapUnityCallbackPushPermissionResponseReceived withMessage:[NSString stringWithFormat:@"%@", accepted ? @"True": @"False"]];
}

#pragma mark - Product Config

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-implementations"

- (void)ctProductConfigFetched {
    [self callUnityObject:CleverTapUnityCallbackProductConfigFetched withMessage:@"Product Config Fetched"];
}

- (void)ctProductConfigActivated {
    [self callUnityObject:CleverTapUnityCallbackProductConfigActivated withMessage:@"Product Config Activated"];
}

- (void)ctProductConfigInitialized {
    [self callUnityObject:CleverTapUnityCallbackProductConfigInitialized withMessage:@"Product Config Initialized"];
}
#pragma clang diagnostic pop

#pragma mark - Feature Flags

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-implementations"
- (void)ctFeatureFlagsUpdated {
    [self callUnityObject:CleverTapUnityCallbackFeatureFlagsUpdated withMessage:@"Feature Flags updated"];
}
#pragma clang diagnostic pop

@end
