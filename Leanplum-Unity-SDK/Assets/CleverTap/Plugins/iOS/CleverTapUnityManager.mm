#import "CleverTapUnityManager.h"
#import "CleverTapUnityCallbackHandler.h"
#import "CleverTapUnityCallbackInfo.h"
#import "CleverTapMessageSender.h"
#import <CleverTapSDK/CleverTap.h>
#import <CleverTapSDK/CleverTap+Inbox.h>
#import <CleverTapSDK/CleverTapSyncDelegate.h>
#import <CleverTapSDK/CleverTap+DisplayUnit.h>
#import <CleverTapSDK/CleverTap+FeatureFlags.h>
#import <CleverTapSDK/CleverTap+ProductConfig.h>
#import <CleverTapSDK/CleverTapInAppNotificationDelegate.h>
#import <CleverTapSDK/CleverTap+InAppNotifications.h>
#import <CleverTapSDK/CTLocalInApp.h>
#import <CleverTapSDK/Clevertap+PushPermission.h>
#import <CleverTapSDK/CTVar.h>
#import <CleverTapSDK/CleverTap+CTVar.h>
#import <CleverTapSDK/CTTemplateContext.h>

@interface CleverTapUnityManager ()

@property (nonatomic, strong) CleverTap *cleverTap;
@property (nonatomic, strong) CleverTapUnityCallbackHandler *callbackHandler;

@end

@implementation CleverTapUnityManager

+ (CleverTapUnityManager*)sharedInstance {
    static CleverTapUnityManager *sharedInstance = nil;
    
    if(!sharedInstance) {
        sharedInstance = [[CleverTapUnityManager alloc] init];
    }
    
    return sharedInstance;
}

- (instancetype)init {
    if (self = [super init]) {
        [self setCleverTapInstance:[CleverTap sharedInstance]];
    }
    return self;
}

- (void)setCleverTapInstance:(CleverTap *)instance {
    if (!instance) {
        NSLog(@"Cannot set nil CleverTap instance.");
        return;
    }
    NSLog(@"Setting CleverTap instance: %@", instance);
    self.cleverTap = instance;
    [self.cleverTap setLibrary:@"Unity"];
    self.callbackHandler = [CleverTapUnityCallbackHandler sharedInstance];
    [self.callbackHandler attachInstance:self.cleverTap];
    if (platformDidInit && shouldDisableBuffers) {
        [self disableMessageBuffers];
    }
}

static BOOL platformDidInit = NO;

- (void)onPlatformInit {
    NSLog(@"CleverTap Platform Init");
    platformDidInit = YES;
    if (self.cleverTap && shouldDisableBuffers) {
        [self disableMessageBuffers];
    }
}

- (void)onCallbackAdded:(NSString *)callbackName {
    CleverTapUnityCallbackInfo *callback = [CleverTapUnityCallbackInfo callbackFromName:callbackName];
    if (!callback) {
        NSLog(@"Unsupported callback added: %@", callbackName);
        return;
    }
    
    [[CleverTapMessageSender sharedInstance] disableBuffer:callback];
    [[CleverTapMessageSender sharedInstance] flushBuffer:callback];
}

- (void)onVariablesCallbackAdded:(NSString *)callbackName callbackId:(int)callbackId {
    NSNumber *callbackEnum = [CleverTapUnityCallbackInfo callbackEnumForName:callbackName];
    if (!callbackEnum) {
        NSLog(@"Unsupported callback added: %@", callbackName);
        return;
    }
    
    CleverTapUnityCallback callback = (CleverTapUnityCallback)[callbackEnum integerValue];
    CleverTapUnityCallbackHandler *handler = [CleverTapUnityCallbackHandler sharedInstance];
    
    switch (callback) {
        case CleverTapUnityCallbackVariablesChanged:
            [self.cleverTap onVariablesChanged:[handler variablesCallback:CleverTapUnityCallbackVariablesChanged callbackId:callbackId]];
            break;
        case CleverTapUnityCallbackVariablesChangedAndNoDownloadsPending:
            [self.cleverTap onVariablesChangedAndNoDownloadsPending:[handler variablesCallback:CleverTapUnityCallbackVariablesChangedAndNoDownloadsPending callbackId:callbackId]];
            break;
        case CleverTapUnityCallbackOneTimeVariablesChanged:
            [self.cleverTap onceVariablesChanged:[handler variablesCallback:CleverTapUnityCallbackOneTimeVariablesChanged callbackId:callbackId]];
            break;
        case CleverTapUnityCallbackOneTimeVariablesChangedAndNoDownloadsPending:
            [self.cleverTap onceVariablesChangedAndNoDownloadsPending:[handler variablesCallback:CleverTapUnityCallbackOneTimeVariablesChangedAndNoDownloadsPending callbackId:callbackId]];
            break;
        default:
            NSLog(@"Callback is not a Variables Callback: %@", callbackName);
            break;
    }
}

const double PENDING_EVENTS_TIME_OUT = 10.0;
static BOOL shouldDisableBuffers = YES;

- (void)disableMessageBuffers {
    // disable buffers after a delay in order to give some time for callback delegates to attach
    // and receive initially buffered messages. After that all buffers will be cleared and disabled
    // and messages will continue to be sent immediately.
    shouldDisableBuffers = NO;
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(PENDING_EVENTS_TIME_OUT * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
        NSLog(@"CleverTap resetting all buffers.");
        [[CleverTapMessageSender sharedInstance] resetAllBuffers];
    });
}

#pragma mark - Admin

+ (void)setApplicationIconBadgeNumber:(int)num {
    [UIApplication sharedApplication].applicationIconBadgeNumber = num;
}

+ (void)setDebugLevel:(int)level {
    [CleverTap setDebugLevel:level];
}

- (void)setSyncDelegate:(id <CleverTapSyncDelegate>)delegate {
    [self.cleverTap setSyncDelegate:delegate];
}

+ (void)enablePersonalization {
    [CleverTap enablePersonalization];
}

+ (void)disablePersonalization {
    [CleverTap disablePersonalization];
}

+ (void)setLocation:(CLLocationCoordinate2D)location {
    [CleverTap setLocation:location];
}

#pragma mark - Offline API

- (void)setOffline:(BOOL)enabled{
    [self.cleverTap setOffline:enabled];
}

#pragma mark - Opt-out API

- (void)setOptOut:(BOOL)enabled{
    [self.cleverTap setOptOut:enabled];
}
- (void)enableDeviceNetworkInfoReporting:(BOOL)enabled{
    [self.cleverTap enableDeviceNetworkInfoReporting:enabled];
}


#pragma mark - User Profile

- (void)onUserLogin:(NSDictionary *)properties {
    NSDictionary *attributes = cleverTap_convertDateValues(properties);
    [self.cleverTap onUserLogin:attributes];
}

- (void)profilePush:(NSDictionary *)properties {
    NSDictionary *attributes = cleverTap_convertDateValues(properties);
    [self.cleverTap profilePush:attributes];
}

- (id)profileGet:(NSString *)propertyName {
    return [self.cleverTap profileGet:propertyName];
}

- (void)profileRemoveValueForKey:(NSString *)key {
    [self.cleverTap profileRemoveValueForKey:key];
}

- (void)profileSetMultiValues:(NSArray<NSString *> *)values forKey:(NSString *)key {
    [self.cleverTap profileSetMultiValues:values forKey:key];
}

- (void)profileAddMultiValue:(NSString *)value forKey:(NSString *)key {
    [self.cleverTap profileAddMultiValue:value forKey:key];
}

- (void)profileAddMultiValues:(NSArray<NSString *> *)values forKey:(NSString *)key {
    [self.cleverTap profileAddMultiValues:values forKey:key];
}

- (void)profileRemoveMultiValue:(NSString *)value forKey:(NSString *)key {
    [self.cleverTap profileRemoveMultiValue:value forKey:key];
}

- (void)profileRemoveMultiValues:(NSArray<NSString *> *)values forKey:(NSString *)key {
    [self.cleverTap profileRemoveMultiValues:values forKey:key];
}

- (void)profileIncrementValueBy:(NSNumber* _Nonnull)value forKey:(NSString *_Nonnull)key {
    [self.cleverTap profileIncrementValueBy:value forKey:key];
}

- (void)profileDecrementValueBy:(NSNumber* _Nonnull)value forKey:(NSString *_Nonnull)key {
    [self.cleverTap profileDecrementValueBy:value forKey:key];
}

- (NSString *)profileGetCleverTapID {
    return [self.cleverTap profileGetCleverTapID];
}

- (NSString*)profileGetCleverTapAttributionIdentifier {
    return [self.cleverTap profileGetCleverTapAttributionIdentifier];
}

#pragma mark - User Action Events

- (void)recordScreenView:(NSString *)screenName {
    if (!screenName) {
        return;
    }
    [self.cleverTap recordScreenView:screenName];
}

- (void)recordEvent:(NSString *)event {
    [self.cleverTap recordEvent:event];
}

- (void)recordEvent:(NSString *)event withProps:(NSDictionary *)properties {
    NSDictionary *attributes = cleverTap_convertDateValues(properties);
    [self.cleverTap recordEvent:event withProps:attributes];
}

- (void)recordChargedEventWithDetails:(NSDictionary *)chargeDetails andItems:(NSArray *)items {
    NSDictionary *details = cleverTap_convertDateValues(chargeDetails);
    [self.cleverTap recordChargedEventWithDetails:details andItems:items];
}

- (void)recordErrorWithMessage:(NSString *)message andErrorCode:(int)code {
    [self.cleverTap recordErrorWithMessage:message andErrorCode:code];
}

- (NSTimeInterval)eventGetFirstTime:(NSString *)event {
    return [self.cleverTap eventGetFirstTime:event];
}

- (NSTimeInterval)eventGetLastTime:(NSString *)event {
    return [self.cleverTap eventGetLastTime:event];
}

- (int)eventGetOccurrences:(NSString *)event {
    return [self.cleverTap eventGetOccurrences:event];
}

- (NSDictionary *)userGetEventHistory {
    return [self.cleverTap userGetEventHistory];
}

- (CleverTapEventDetail *)eventGetDetail:(NSString *)event {
    return [self.cleverTap eventGetDetail:event];
}

- (void)getUserEventLog:(NSString *)eventName forKey:(NSString *)key withCallback:(UserEventLogCallback)callback {
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        CleverTapEventDetail *event = [[CleverTap sharedInstance] getUserEventLog:eventName];
        dispatch_async(dispatch_get_main_queue(), ^{
            if (!event) {
                callback([key UTF8String], [@"{}" UTF8String]);
                return;
            }
            
            NSDictionary *json = [CleverTapUnityManager userEventLogToDictionary:event];
            callback([key UTF8String], [[CleverTapUnityManager idToJsonString:json] UTF8String]);
        });
    });
}

- (void)getUserEventLogCount:(nonnull NSString *)eventName forKey:(nonnull NSString *)key withCallback:(nonnull UserEventLogCallback)callback {
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        int count = [[CleverTap sharedInstance] getUserEventLogCount:eventName];
        dispatch_async(dispatch_get_main_queue(), ^{
            NSString *countString = [NSString stringWithFormat:@"%d", count];
            callback([key UTF8String], [countString UTF8String]);
        });
    });
}

- (void)getUserEventLogHistory:(nonnull NSString *)key withCallback:(nonnull UserEventLogCallback)callback {
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        NSDictionary *history = [[CleverTap sharedInstance] getUserEventLogHistory];
        dispatch_async(dispatch_get_main_queue(), ^{
            if (!history) {
                callback([key UTF8String], [@"{}" UTF8String]);
                return;
            }
            
            NSMutableDictionary *result = [NSMutableDictionary new];
            for (NSString *key in history) {
                CleverTapEventDetail *event = history[key];
                result[key] = [CleverTapUnityManager userEventLogToDictionary:event];
            }
            
            NSString *json = [CleverTapUnityManager idToJsonString:result];
            callback([key UTF8String], [json UTF8String]);
        });
    });
}

- (void)getUserAppLaunchCount:(nonnull NSString *)key withCallback:(nonnull UserEventLogCallback)callback {
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        int count = [[CleverTap sharedInstance] getUserAppLaunchCount];
        dispatch_async(dispatch_get_main_queue(), ^{
            NSString *countString = [NSString stringWithFormat:@"%d", count];
            callback([key UTF8String], [countString UTF8String]);
        });
    });
}

- (long)getUserLastVisitTs {
    return [self.cleverTap getUserLastVisitTs];
}

// Serializes object to JSON string.
// Uses the same logic as clevertap_toJsonString(val).
// consider keeping only one of the methods/functions in the future.
+ (NSString *)idToJsonString:(id)val {
    NSString *jsonString;
    
    if (val == nil) {
        return nil;
    }
    
    if ([val isKindOfClass:[NSArray class]] || [val isKindOfClass:[NSDictionary class]]) {
        NSError *error;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:val options:NSJSONWritingPrettyPrinted error:&error];
        jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        
        if (error != nil) {
            jsonString = nil;
        }
    } else {
        jsonString = [NSString stringWithFormat:@"%@", val];
    }
    
    return jsonString;
}

+ (NSDictionary *)userEventLogToDictionary:(CleverTapEventDetail *)event {
    NSDictionary *json = @{
        @"countOfEvents": @(event.count),
        @"eventName": event.eventName,
        @"normalizedEventName": event.normalizedEventName,
        @"firstTS": @(event.firstTime),
        @"lastTS": @(event.lastTime),
        @"deviceID": event.deviceID
    };
    return json;
}

#pragma mark - Session API

- (NSTimeInterval)sessionGetTimeElapsed {
    return [self.cleverTap sessionGetTimeElapsed];
}

- (CleverTapUTMDetail *)sessionGetUTMDetails {
    return [self.cleverTap sessionGetUTMDetails];
}

- (int)userGetTotalVisits {
    return [self.cleverTap userGetTotalVisits];
}

- (int)userGetScreenCount {
    return [self.cleverTap userGetScreenCount];
}

- (NSTimeInterval)userGetPreviousVisitTime {
    return [self.cleverTap userGetPreviousVisitTime];
}

#pragma mark - Push Notifications

+ (void)registerPush {
    UIApplication *application = [UIApplication sharedApplication];
    if ([application respondsToSelector:@selector(isRegisteredForRemoteNotifications)]) {
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
        UIUserNotificationSettings *settings = [UIUserNotificationSettings
                                                settingsForTypes:UIUserNotificationTypeAlert | UIUserNotificationTypeBadge | UIUserNotificationTypeSound
                                                categories:nil];
        
        
        [application registerUserNotificationSettings:settings];
        [application registerForRemoteNotifications];
#pragma clang diagnostic pop
    }
    else {
#if __IPHONE_OS_VERSION_MAX_ALLOWED < 80000
        [application registerForRemoteNotificationTypes:
         (UIRemoteNotificationTypeBadge | UIRemoteNotificationTypeAlert | UIRemoteNotificationTypeSound)];
#endif
    }
}

- (void)setPushToken:(NSData *)pushToken {
    [self.cleverTap setPushToken:pushToken];
}

- (void)setPushTokenAsString:(NSString *)pushTokenString {
    [self.cleverTap setPushTokenAsString:pushTokenString];
}

- (void)didReceiveRemoteNotification:(NSDictionary *)notification isOpen:(BOOL)isOpen {
    [self didReceiveRemoteNotification:notification isOpen:isOpen openInForeground:NO];
}

- (void)didReceiveRemoteNotification:(NSDictionary *)notification
                              isOpen:(BOOL)isOpen
                    openInForeground:(BOOL)openInForeground {
    if (openInForeground) {
        [self.cleverTap handleNotificationWithData:notification openDeepLinksInForeground:YES];
    } else {
        [self.cleverTap handleNotificationWithData:notification];
    }
    
    [self sendRemoteNotificationCallbackToUnity:notification isOpen:isOpen];
}

- (void)sendRemoteNotificationCallbackToUnity:(NSDictionary *)notification isOpen:(BOOL)isOpen {
    // generate a new dictionary that rearrange the notification elements
    NSMutableDictionary *aps = [NSMutableDictionary dictionaryWithDictionary:[notification objectForKey:@"aps"]];
    
    // check if the object for key alert is a string; if it is, then convert it to a dictionary
    id alert = [aps objectForKey:@"alert"];
    if ([alert isKindOfClass:[NSString class]]) {
        NSDictionary *alertDictionary = [NSDictionary dictionaryWithObject:alert forKey:@"body"];
        [aps setObject:alertDictionary forKey:@"alert"];
    }
    
    // move all other dictionarys other than aps in payload to key extra in aps dictionary
    NSMutableDictionary *extraDictionary = [NSMutableDictionary dictionaryWithDictionary:notification];
    [extraDictionary removeObjectForKey:@"aps"];
    if ([extraDictionary count] > 0) {
        [aps setObject:extraDictionary forKey:@"extra"];
    }
    
    if ([NSJSONSerialization isValidJSONObject:aps]) {
        NSError *pushParsingError = nil;
        NSData *data = [NSJSONSerialization dataWithJSONObject:aps options:0 error:&pushParsingError];
        
        if (pushParsingError == nil) {
            [[CleverTapUnityCallbackHandler sharedInstance]
             didReceiveRemoteNotification:data isOpen:isOpen];
        }
    }
}

#pragma mark - DeepLink Handling

- (void)handleOpenURL:(NSURL *)url {
    [[CleverTapUnityCallbackHandler sharedInstance] deepLinkCallback:[url absoluteString]];
}

#pragma mark - Referrer Tracking

- (void)pushInstallReferrerSource:(NSString *)source
                           medium:(NSString *)medium
                         campaign:(NSString *)campaign {
    
    [self.cleverTap pushInstallReferrerSource:source medium:medium campaign:campaign];
}

#pragma mark - App Inbox

- (void)initializeInbox {
    [self.cleverTap initializeInboxWithCallback:^(BOOL success) {
        void(^block)(BOOL success) = [[CleverTapUnityCallbackHandler sharedInstance] initializeInboxBlock];
        block(success);
        [self inboxMessagesDidUpdate];
    }];
}

- (void)inboxMessagesDidUpdate {
    [self.cleverTap registerInboxUpdatedBlock:[[CleverTapUnityCallbackHandler sharedInstance] inboxUpdatedBlock]];
}

- (void)showAppInbox:(NSDictionary *)styleConfig {
    CleverTapInboxViewController *inboxController = [self.cleverTap newInboxViewControllerWithConfig:[self _dictToInboxStyleConfig:styleConfig? styleConfig : nil] andDelegate:self.callbackHandler];
    if (inboxController) {
        UINavigationController *navigationController = [[UINavigationController alloc] initWithRootViewController:inboxController];
        [UnityGetGLViewController() presentViewController:navigationController animated:YES completion:nil];
    }
}

- (void)dismissAppInbox {
    [self.cleverTap dismissAppInbox];
}

- (CleverTapInboxStyleConfig *)_dictToInboxStyleConfig: (NSDictionary *)dict {
    CleverTapInboxStyleConfig *_config = [CleverTapInboxStyleConfig new];
    NSString *title = [dict valueForKey:@"navBarTitle"];
    if (title) {
        _config.title = title;
    }
    NSArray *messageTags = [dict valueForKey:@"tabs"];
    if (messageTags) {
        _config.messageTags = messageTags;
    }
    NSString *backgroundColor = [dict valueForKey:@"inboxBackgroundColor"];
    if (backgroundColor) {
        _config.backgroundColor = [self ct_colorWithHexString:backgroundColor alpha:1.0];
    }
    NSString *navigationBarTintColor = [dict valueForKey:@"navBarColor"];
    if (navigationBarTintColor) {
        _config.navigationBarTintColor = [self ct_colorWithHexString:navigationBarTintColor alpha:1.0];
    }
    NSString *navigationTintColor = [dict valueForKey:@"navBarTitleColor"];
    if (navigationTintColor) {
        _config.navigationTintColor = [self ct_colorWithHexString:navigationTintColor alpha:1.0];
    }
    NSString *tabBackgroundColor = [dict valueForKey:@"tabBackgroundColor"];
    if (tabBackgroundColor) {
        _config.navigationBarTintColor = [self ct_colorWithHexString:tabBackgroundColor alpha:1.0];
    }
    NSString *tabSelectedBgColor = [dict valueForKey:@"tabSelectedBgColor"];
    if (tabSelectedBgColor) {
        _config.tabSelectedBgColor = [self ct_colorWithHexString:tabSelectedBgColor alpha:1.0];
    }
    NSString *tabSelectedTextColor = [dict valueForKey:@"tabSelectedTextColor"];
    if (tabSelectedTextColor) {
        _config.tabSelectedTextColor = [self ct_colorWithHexString:tabSelectedTextColor alpha:1.0];
    }
    NSString *tabUnSelectedTextColor = [dict valueForKey:@"tabUnSelectedTextColor"];
    if (tabUnSelectedTextColor) {
        _config.tabUnSelectedTextColor = [self ct_colorWithHexString:tabUnSelectedTextColor alpha:1.0];
    }
    NSString *noMessageTextColor = [dict valueForKey:@"noMessageTextColor"];
    if (noMessageTextColor) {
        _config.noMessageViewTextColor = [self ct_colorWithHexString:noMessageTextColor alpha:1.0];
    }
    NSString *noMessageText = [dict valueForKey:@"noMessageText"];
    if (noMessageText) {
        _config.noMessageViewText = noMessageText;
    }
    return _config;
}

- (UIColor *)ct_colorWithHexString:(NSString *)string alpha:(CGFloat)alpha {
    if (![string isKindOfClass:[NSString class]] || [string length] == 0) {
        return [UIColor colorWithRed:0.0f green:0.0f blue:0.0f alpha:1.0f];
    }
    unsigned int hexint = 0;
    NSScanner *scanner = [NSScanner scannerWithString:string];
    [scanner setCharactersToBeSkipped:[NSCharacterSet
                                       characterSetWithCharactersInString:@"#"]];
    [scanner scanHexInt:&hexint];
    UIColor *color =
    [UIColor colorWithRed:((CGFloat) ((hexint & 0xFF0000) >> 16))/255
                    green:((CGFloat) ((hexint & 0xFF00) >> 8))/255
                     blue:((CGFloat) (hexint & 0xFF))/255
                    alpha:alpha];
    return color;
}

- (int)getInboxMessageUnreadCount {
    return (int)[self.cleverTap getInboxMessageUnreadCount];
}

- (int)getInboxMessageCount {
    return (int)[self.cleverTap getInboxMessageCount];
}

- (NSArray *)getAllInboxMessages {
    
    NSArray *inboxMessages = [self.cleverTap getAllInboxMessages];
    
    NSMutableArray <NSDictionary *> *jsonArray = [NSMutableArray new];
    
    for (id object in inboxMessages) {
        if ([object isKindOfClass:[CleverTapInboxMessage class]]) {
            CleverTapInboxMessage *message = object;
            [jsonArray addObject:message.json];
        }
    }
    
    return jsonArray;
}

- (NSArray *)getUnreadInboxMessages {
    
    NSArray *inboxMessages = [self.cleverTap getUnreadInboxMessages];
    
    NSMutableArray <NSDictionary *> *jsonArray = [NSMutableArray new];
    
    for (id object in inboxMessages) {
        if ([object isKindOfClass:[CleverTapInboxMessage class]]) {
            CleverTapInboxMessage *message = object;
            [jsonArray addObject:message.json];
        }
    }
    
    return jsonArray;
}

- (NSDictionary *)getInboxMessageForId:(NSString *)messageId {
    
    CleverTapInboxMessage *message = [self.cleverTap getInboxMessageForId:messageId];
    
    return message.json;
}

- (void)deleteInboxMessageForID:(NSString *)messageId {
    [self.cleverTap deleteInboxMessageForID:messageId];
}

- (void)deleteInboxMessagesForIDs:(NSArray *)messageIds {
    [self.cleverTap deleteInboxMessagesForIDs:messageIds];
}

- (void)markReadInboxMessageForID:(NSString *)messageId {
    [self.cleverTap markReadInboxMessageForID:messageId];
}

- (void)markReadInboxMessagesForIDs:(NSArray *)messageIds {
    [self.cleverTap markReadInboxMessagesForIDs:messageIds];
}

- (void)recordInboxNotificationViewedEventForID:(NSString *)messageId {
    
    [self.cleverTap recordInboxNotificationViewedEventForID:messageId];
}

- (void)recordInboxNotificationClickedEventForID:(NSString *)messageId {
    [self.cleverTap recordInboxNotificationClickedEventForID:messageId];
}


#pragma mark - Native Display

- (NSArray *)getAllDisplayUnits {
    
    NSArray *displayUnits = [self.cleverTap getAllDisplayUnits];
    
    NSMutableArray <NSDictionary *> *jsonArray = [NSMutableArray new];
    
    for (id object in displayUnits) {
        if ([object isKindOfClass:[CleverTapDisplayUnit class]]) {
            CleverTapDisplayUnit *unit = object;
            [jsonArray addObject:unit.json];
        }
    }
    
    return jsonArray;
}

- (NSDictionary *)getDisplayUnitForID:(NSString *)unitID {
    
    CleverTapDisplayUnit *unit = [self.cleverTap getDisplayUnitForID:unitID];
    
    return unit.json;
}

- (void)recordDisplayUnitViewedEventForID:(NSString *)unitID {
    [self.cleverTap recordDisplayUnitViewedEventForID:unitID];
}

- (void)recordDisplayUnitClickedEventForID:(NSString *)unitID {
    [self.cleverTap recordDisplayUnitClickedEventForID:unitID];
}


#pragma mark - Product Config

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
- (void)fetchProductConfig {
    [[self.cleverTap productConfig] fetch];
}

- (void)fetchProductConfigWithMinimumInterval:(NSTimeInterval)minimumInterval {
    [[self.cleverTap productConfig] fetchWithMinimumInterval:minimumInterval];
}

- (void)setProductConfigMinimumFetchInterval:(NSTimeInterval)minimumFetchInterval {
    [[self.cleverTap productConfig] setMinimumFetchInterval:minimumFetchInterval];
}

- (void)activateProductConfig {
    [[self.cleverTap productConfig] activate];
}

- (void)fetchAndActivateProductConfig {
    [[self.cleverTap productConfig] fetchAndActivate];
}

- (void)setProductConfigDefaults:(NSDictionary *)defaults {
    [[self.cleverTap productConfig] setDefaults:defaults];
}

- (void)setProductConfigDefaultsFromPlistFileName:(NSString *)fileName {
    [[self.cleverTap productConfig] setDefaultsFromPlistFileName:fileName];
}

- (NSDictionary *)getProductConfigValueFor:(NSString *)key {
    CleverTapConfigValue *value = [[self.cleverTap productConfig] get:key];
    NSDictionary *jsonDict;
    if ([value.jsonValue isKindOfClass:[NSDictionary class]]) {
        jsonDict = value.jsonValue;
    }
    else {
        jsonDict = @{ key: value.jsonValue };
    }
    
    return jsonDict;
}

- (double)getProductConfigLastFetchTimeStamp {
    NSDate *date = [[self.cleverTap productConfig] getLastFetchTimeStamp];
    double interval = date.timeIntervalSince1970;
    return interval;
}

- (void)resetProductConfig {
    [[self.cleverTap productConfig] reset];
}
#pragma clang diagnostic pop

#pragma mark - Feature Flags

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
- (BOOL)get:(NSString *)key withDefaultValue:(BOOL)defaultValue {
    return [[self.cleverTap featureFlags] get:key withDefaultValue:defaultValue];
}
#pragma clang diagnostic pop

#pragma mark - Push Primer

- (CTLocalInApp*)_localInAppConfigFromReadableMap: (NSDictionary *)json {
    CTLocalInApp *inAppBuilder;
    CTLocalInAppType inAppType = HALF_INTERSTITIAL;
    //Required parameters
    NSString *titleText = nil, *messageText = nil, *followDeviceOrientation = nil, *positiveBtnText = nil, *negativeBtnText = nil;
    //Additional parameters
    NSString *fallbackToSettings = nil, *backgroundColor = nil, *btnBorderColor = nil, *titleTextColor = nil, *messageTextColor = nil, *btnTextColor = nil, *imageUrl = nil, *btnBackgroundColor = nil, *btnBorderRadius = nil;
    
    if ([json[@"inAppType"]  isEqual: @"half-interstitial"]){
        inAppType = HALF_INTERSTITIAL;
    }
    else {
        inAppType = ALERT;
    }
    if (json[@"titleText"]) {
        titleText = [json valueForKey:@"titleText"];
    }
    if (json[@"messageText"]) {
        messageText = [json valueForKey:@"messageText"];
    }
    if (json[@"followDeviceOrientation"]) {
        followDeviceOrientation = [json valueForKey:@"followDeviceOrientation"];
    }
    if (json[@"positiveBtnText"]) {
        positiveBtnText = [json valueForKey:@"positiveBtnText"];
    }
    
    if (json[@"negativeBtnText"]) {
        negativeBtnText = [json valueForKey:@"negativeBtnText"];
    }
    
    //creates the builder instance with all the required parameters
    inAppBuilder = [[CTLocalInApp alloc] initWithInAppType:inAppType
                                                 titleText:titleText
                                               messageText:messageText
                                   followDeviceOrientation:followDeviceOrientation
                                           positiveBtnText:positiveBtnText
                                           negativeBtnText:negativeBtnText];
    
    //adds optional parameters to the builder instance
    if (json[@"fallbackToSettings"]) {
        fallbackToSettings = [json valueForKey:@"fallbackToSettings"];
        [inAppBuilder setFallbackToSettings:fallbackToSettings];
    }
    if (json[@"backgroundColor"]) {
        backgroundColor = [json valueForKey:@"backgroundColor"];
        [inAppBuilder setBackgroundColor:backgroundColor];
    }
    if (json[@"btnBorderColor"]) {
        btnBorderColor = [json valueForKey:@"btnBorderColor"];
        [inAppBuilder setBtnBorderColor:btnBorderColor];
    }
    if (json[@"titleTextColor"]) {
        titleTextColor = [json valueForKey:@"titleTextColor"];
        [inAppBuilder setTitleTextColor:titleTextColor];
    }
    if (json[@"messageTextColor"]) {
        messageTextColor = [json valueForKey:@"messageTextColor"];
        [inAppBuilder setMessageTextColor:messageTextColor];
    }
    if (json[@"btnTextColor"]) {
        btnTextColor = [json valueForKey:@"btnTextColor"];
        [inAppBuilder setBtnTextColor:btnTextColor];
    }
    if (json[@"imageUrl"]) {
        imageUrl = [json valueForKey:@"imageUrl"];
        [inAppBuilder setImageUrl:imageUrl];
    }
    if (json[@"btnBackgroundColor"]) {
        btnBackgroundColor = [json valueForKey:@"btnBackgroundColor"];
        [inAppBuilder setBtnBackgroundColor:btnBackgroundColor];
    }
    if (json[@"btnBorderRadius"]) {
        btnBorderRadius = [json valueForKey:@"btnBorderRadius"];
        [inAppBuilder setBtnBorderRadius:btnBorderRadius];
    }
    return inAppBuilder;
}

- (void)promptForPushPermission:(BOOL)showFallbackSettings {
    [self.cleverTap promptForPushPermission:showFallbackSettings];
}

- (void)promptPushPrimer:(NSDictionary *)json {
    CTLocalInApp *localInAppBuilder = [self _localInAppConfigFromReadableMap:json];
    [self.cleverTap promptPushPrimer:localInAppBuilder.getLocalInAppSettings];
}

- (void)isPushPermissionGranted {
    if (@available(iOS 10.0, *)) {
        [self.cleverTap getNotificationPermissionStatusWithCompletionHandler:^(UNAuthorizationStatus status) {
            BOOL isPushEnabled = YES;
            if (status == UNAuthorizationStatusNotDetermined || status == UNAuthorizationStatusDenied) {
                isPushEnabled = NO;
            }
            NSLog(@"[CleverTap isPushPermissionGranted: %d]", isPushEnabled);
            [[CleverTapUnityCallbackHandler sharedInstance] pushPermissionCallback:isPushEnabled];
        }];
    }
    else {
        // Fallback on earlier versions
        NSLog(@"Push Notification is available from iOS v10.0 or later");
    }
}

#pragma mark - Private Helpers

- (NSString *)dictToJson:(NSDictionary *)dict {
    NSError *err;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&err];
    
    if(err != nil) {
        return nil;
    }
    
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

#pragma mark - In-App Controls

- (void)suspendInAppNotifications {
    [self.cleverTap suspendInAppNotifications];
}

- (void)discardInAppNotifications {
    [self.cleverTap discardInAppNotifications];
}

- (void)resumeInAppNotifications {
    [self.cleverTap resumeInAppNotifications];
}

#pragma mark - Variables

- (void)syncVariables
{
    [self.cleverTap syncVariables];
}

- (void)syncVariables:(BOOL)isProduction {
    [self.cleverTap syncVariables:isProduction];
}

- (void)fetchVariables:(int) callbackId
{
    [self.cleverTap fetchVariables:[[CleverTapUnityCallbackHandler sharedInstance] fetchVariablesBlock:callbackId]];
}

- (void)defineVar:(NSString *)name kind:(NSString *)kind andDefaultValue:(NSString *)defaultValue
{
    NSData *data = [defaultValue dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    
    CTVar *var = nil;
    
    if ([kind isEqualToString:@"integer"])
    {
        var = [self.cleverTap defineVar:name withInt:[defaultValue intValue]];
    }
    else if ([kind isEqualToString:@"float"])
    {
        var = [self.cleverTap defineVar:name withDouble:[defaultValue doubleValue]];
    }
    else if ([kind isEqualToString:@"bool"])
    {
        var = [self.cleverTap defineVar:name withBool:[defaultValue boolValue]];
    }
    else if ([kind isEqualToString:@"group"])
    {
        NSDictionary *json = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
        if (error) {
            NSLog(@"CleverTap: Error parsing JSON: %@", error);
            return;
        }
        var = [self.cleverTap defineVar:name withDictionary:json];
    }
    else if ([kind isEqualToString:@"string"])
    {
        NSString *value = [defaultValue substringWithRange:NSMakeRange(1, [defaultValue length] - 2)];
        var = [self.cleverTap defineVar:name withString:value];
    }
    else
    {
        NSLog(@"CleverTap: Unsupported type %@", kind);
        return;
    }
    
    [var onValueChanged:[[CleverTapUnityCallbackHandler sharedInstance] variableValueChanged:var.name]];
}

- (void)defineFileVar:(NSString *)name
{
    CTVar *var = [self.cleverTap defineFileVar:name];
    
    [var onValueChanged:[[CleverTapUnityCallbackHandler sharedInstance] variableValueChanged:var.name]];
    
    [var onFileIsReady:[[CleverTapUnityCallbackHandler sharedInstance] variableFileIsReady:var.name]];
}

- (NSString *)getVariableValue:(NSString *)name
{
    id value = [self.cleverTap getVariableValue:name];
    
    // Check if value is not nil and is serializable to JSON
    if (value) {
        NSError *error;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:value options:NSUTF8StringEncoding error:&error];
        
        if (!jsonData) {
            NSLog(@"Error serializing JSON: %@", error);
            return nil;
        } else {
            NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            return jsonString;
        }
    } else {
        NSLog(@"Value is nil or not a valid JSON object");
        return nil;
    }
}

- (NSString *)getFileVariableValue:(NSString *)name
{
    CTVar *var = [self.cleverTap getVariable:name];
    if (!var) {
        NSLog(@"Variable %@ not found.", name);
        return nil;
    }
    
    if (![var.kind isEqualToString:@"file"]) {
        NSLog(@"Variable %@ is not a file variable.", name);
        return nil;
    }
    
    return [var fileValue];
}

NSDictionary *cleverTap_convertDateValues(NSDictionary *dictionary) {
    if (dictionary == nil) {
        return dictionary;
    }
    
    NSMutableDictionary *dict = [[NSMutableDictionary alloc] initWithDictionary:dictionary];
    for (id key in dictionary) {
        id value = [dict objectForKey:key];
        if ([value isKindOfClass:[NSString class]]) {
            NSString *strVal = value;
            NSRange range = [strVal rangeOfString:@"ct_date_"];
            if(range.location != NSNotFound)
            {
                NSString *dateStr = [strVal substringWithRange:NSMakeRange(range.length, strVal.length - range.length)];
                NSDate *date = [[NSDate alloc] initWithTimeIntervalSince1970:[dateStr longLongValue]/1000];
                dict[key] = date;
            }
        }
    }
    return dict;
}

#pragma mark - Client-side In-Apps

- (void)fetchInApps:(int) callbackId {
    [self.cleverTap fetchInApps:[[CleverTapUnityCallbackHandler sharedInstance] fetchInAppsBlock:callbackId]];
}

- (void)clearInAppResources:(BOOL)expiredOnly {
    [self.cleverTap clearInAppResources:expiredOnly];
}

- (NSString *)customTemplateContextToString:(NSString *)name {
    CTTemplateContext *context = [self contextNamed:name];
    if (context) {
        return [context debugDescription];
    }
    return nil;
}

- (void)customTemplateSetDismissed:(NSString *)name {
    CTTemplateContext *context = [self contextNamed:name];
    if (context) {
        [context dismissed];
    }
}

- (void)customTemplateSetPresented:(NSString *)name {
    CTTemplateContext *context = [self contextNamed:name];
    if (context) {
        [context presented];
    }
}

- (void)customTemplateTriggerAction:(NSString *)templateName named:(NSString *)argumentName {
    CTTemplateContext *context = [self contextNamed:templateName];
    if (context) {
        [context triggerActionNamed:argumentName];
    }
}

- (NSString *)customTemplateGetStringArg:(NSString *)templateName named:(NSString *)argumentName {
    CTTemplateContext *context = [self contextNamed:templateName];
    if (context) {
        return [context stringNamed:argumentName];
    }
    return nil;
}

- (BOOL)customTemplateGetBooleanArg:(NSString *)templateName named:(NSString *)argumentName {
    CTTemplateContext *context = [self contextNamed:templateName];
    if (context) {
        return [context boolNamed:argumentName];
    }
    
    return NO;
}

- (int8_t)customTemplateGetByteArg:(NSString *)templateName named:(NSString *)argumentName {
    CTTemplateContext *context = [self contextNamed:templateName];
    if (context) {
        return (int8_t)[[context numberNamed:argumentName] charValue];
    }
    
    return 0;
}

- (NSDictionary *)customTemplateGetDictionaryArg:(NSString *)templateName named:(NSString *)argumentName {
    CTTemplateContext *context = [self contextNamed:templateName];
    if (context) {
        return [context dictionaryNamed:argumentName];
    }
    return nil;
}

- (double)customTemplateGetDoubleArg:(NSString *)templateName named:(NSString *)argumentName {
    CTTemplateContext *context = [self contextNamed:templateName];
    if (context) {
        return [context doubleNamed:argumentName];
    }
    
    return 0;
}

- (NSString *)customTemplateGetFileArg:(NSString *)templateName named:(NSString *)argumentName {
    CTTemplateContext *context = [self contextNamed:templateName];
    if (context) {
        return [context fileNamed:argumentName];
    }
    return nil;
}

- (float)customTemplateGetFloatArg:(NSString *)templateName named:(NSString *)argumentName {
    CTTemplateContext *context = [self contextNamed:templateName];
    if (context) {
        return [context floatNamed:argumentName];
    }
    
    return 0;
}

- (int)customTemplateGetIntArg:(NSString *)templateName named:(NSString *)argumentName {
    CTTemplateContext *context = [self contextNamed:templateName];
    if (context) {
        return [context intNamed:argumentName];
    }
    
    return 0;
}

- (int64_t)customTemplateGetLongArg:(NSString *)templateName named:(NSString *)argumentName {
    CTTemplateContext *context = [self contextNamed:templateName];
    if (context) {
        return [context longLongNamed:argumentName];
    }
    
    return 0;
}

- (int16_t)customTemplateGetShortArg:(NSString *)templateName named:(NSString *)argumentName {
    CTTemplateContext *context = [self contextNamed:templateName];
    if (context) {
        return (int16_t)[[context numberNamed:argumentName] shortValue];
    }
    
    return 0;
}

- (CTTemplateContext *)contextNamed:(NSString *)templateName {
    if (!self.cleverTap) {
        NSLog(@"CleverTap is not initialized");
        return nil;
    }
    
    CTTemplateContext *context  = [self.cleverTap activeContextForTemplate:templateName];
    if (!context) {
        NSLog(@"Custom template: %@ is not currently being presented", templateName);
        return nil;
    }
    
    return context;
}

- (void)syncCustomTemplates:(BOOL)isProduction {
    [self.cleverTap syncCustomTemplates:isProduction];
}

@end
