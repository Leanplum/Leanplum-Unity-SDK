#import "CleverTapUnityManager.h"
#import <CleverTapSDK/CleverTap.h>
#import <CleverTapSDK/CleverTap+Inbox.h>
#import <CleverTapSDK/CleverTapSyncDelegate.h>
#import <CleverTapSDK/CleverTap+DisplayUnit.h>
#import <CleverTapSDK/CleverTap+FeatureFlags.h>
#import <CleverTapSDK/CleverTap+ProductConfig.h>
#import <CleverTapSDK/CleverTapInAppNotificationDelegate.h>
#import <CleverTapSDK/CleverTap+InAppNotifications.h>


static CleverTap *clevertap;

static NSString * kCleverTapGameObjectName = @"CleverTapUnity";
static NSString * kCleverTapGameObjectProfileInitializedCallback = @"CleverTapProfileInitializedCallback";
static NSString * kCleverTapGameObjectProfileUpdatesCallback = @"CleverTapProfileUpdatesCallback";
static NSString * kCleverTapDeepLinkCallback = @"CleverTapDeepLinkCallback";
static NSString * kCleverTapPushReceivedCallback = @"CleverTapPushReceivedCallback";
static NSString * kCleverTapPushOpenedCallback = @"CleverTapPushOpenedCallback";
static NSString * kCleverTapInAppNotificationDismissedCallback = @"CleverTapInAppNotificationDismissedCallback";
static NSString * kCleverTapInAppNotificationButtonTapped = @"CleverTapInAppNotificationButtonTapped";
static NSString * kCleverTapInboxDidInitializeCallback = @"CleverTapInboxDidInitializeCallback";
static NSString * kCleverTapInboxMessagesDidUpdateCallback = @"CleverTapInboxMessagesDidUpdateCallback";
static NSString * kCleverTapInboxCustomExtrasButtonSelect = @"CleverTapInboxCustomExtrasButtonSelect";
static NSString * kCleverTapNativeDisplayUnitsUpdated = @"CleverTapNativeDisplayUnitsUpdated";
static NSString * kCleverTapProductConfigFetched = @"CleverTapProductConfigFetched";
static NSString * kCleverTapProductConfigActivated = @"CleverTapProductConfigActivated";
static NSString * kCleverTapProductConfigInitialized = @"CleverTapProductConfigInitialized";
static NSString * kCleverTapFeatureFlagsUpdated = @"CleverTapFeatureFlagsUpdated";

@interface CleverTapUnityManager () < CleverTapInAppNotificationDelegate, CleverTapDisplayUnitDelegate, CleverTapInboxViewControllerDelegate, CleverTapProductConfigDelegate, CleverTapFeatureFlagsDelegate >

@end

@implementation CleverTapUnityManager

+ (CleverTapUnityManager*)sharedInstance {
    static CleverTapUnityManager *sharedInstance = nil;
    
    if(!sharedInstance) {
        sharedInstance = [[CleverTapUnityManager alloc] init];
        [sharedInstance registerListeners];
        
        clevertap = [CleverTap sharedInstance];
        [clevertap setLibrary:@"Unity"];
        
        [clevertap setInAppNotificationDelegate:sharedInstance];
        [clevertap setDisplayUnitDelegate:sharedInstance];
        [[clevertap productConfig] setDelegate:sharedInstance];
        [[clevertap featureFlags] setDelegate:sharedInstance];
    }
    
    return sharedInstance;
}


#pragma mark - Admin

+ (void)launchWithAccountID:(NSString*)accountID andToken:(NSString *)token {
    [self launchWithAccountID:accountID token:token region:nil];
}

+ (void)launchWithAccountID:(NSString*)accountID token:(NSString *)token region:(NSString *)region {
    [CleverTap setCredentialsWithAccountID:accountID token:token region:region];
    [[CleverTap sharedInstance] notifyApplicationLaunchedWithOptions:nil];
}

+ (void)setApplicationIconBadgeNumber:(int)num {
    [UIApplication sharedApplication].applicationIconBadgeNumber = num;
}

+ (void)setDebugLevel:(int)level {
    [CleverTap setDebugLevel:level];
}

- (void)setSyncDelegate:(id <CleverTapSyncDelegate>)delegate {
    [clevertap setSyncDelegate:delegate];
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
        [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapGameObjectProfileInitializedCallback withMessage:jsonString];
    }
}


- (void)didReceiveCleverTapProfileDidChangeNotification:(NSNotification*)notification {
    NSString *jsonString = [self dictToJson:notification.userInfo];
    if (jsonString != nil) {
        [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapGameObjectProfileUpdatesCallback withMessage:jsonString];
    }
}


#pragma mark - Offline API

- (void)setOffline:(BOOL)enabled{
    [clevertap setOffline:enabled];
}

#pragma mark - Opt-out API

- (void)setOptOut:(BOOL)enabled{
    [clevertap setOptOut:enabled];
}
- (void)enableDeviceNetworkInfoReporting:(BOOL)enabled{
    [clevertap enableDeviceNetworkInfoReporting:enabled];
}


#pragma mark - User Profile

- (void)onUserLogin:(NSDictionary *)properties {
    [clevertap onUserLogin:properties];
}

- (void)profilePush:(NSDictionary *)properties {
    [clevertap profilePush:properties];
}

- (id)profileGet:(NSString *)propertyName {
    return [clevertap profileGet:propertyName];
}

- (void)profileRemoveValueForKey:(NSString *)key {
    [clevertap profileRemoveValueForKey:key];
}

- (void)profileSetMultiValues:(NSArray<NSString *> *)values forKey:(NSString *)key {
    [clevertap profileSetMultiValues:values forKey:key];
}

- (void)profileAddMultiValue:(NSString *)value forKey:(NSString *)key {
    [clevertap profileAddMultiValue:value forKey:key];
}

- (void)profileAddMultiValues:(NSArray<NSString *> *)values forKey:(NSString *)key {
    [clevertap profileAddMultiValues:values forKey:key];
}

- (void)profileRemoveMultiValue:(NSString *)value forKey:(NSString *)key {
    [clevertap profileRemoveMultiValue:value forKey:key];
}

- (void)profileRemoveMultiValues:(NSArray<NSString *> *)values forKey:(NSString *)key {
    [clevertap profileRemoveMultiValues:values forKey:key];
}

- (void)profileIncrementValueBy:(NSNumber* _Nonnull)value forKey:(NSString *_Nonnull)key {
    [clevertap profileIncrementValueBy:value forKey:key];
}

- (void)profileDecrementValueBy:(NSNumber* _Nonnull)value forKey:(NSString *_Nonnull)key {
    [clevertap profileDecrementValueBy:value forKey:key];
}

- (NSString *)profileGetCleverTapID {
    return [clevertap profileGetCleverTapID];
}

- (NSString*)profileGetCleverTapAttributionIdentifier {
    return [clevertap profileGetCleverTapAttributionIdentifier];
}

#pragma mark - User Action Events

- (void)recordScreenView:(NSString *)screenName {
    if (!screenName) {
        return;
    }
    [clevertap recordScreenView:screenName];
}

- (void)recordEvent:(NSString *)event {
    [clevertap recordEvent:event];
}

- (void)recordEvent:(NSString *)event withProps:(NSDictionary *)properties {
    [clevertap recordEvent:event withProps:properties];
}

- (void)recordChargedEventWithDetails:(NSDictionary *)chargeDetails andItems:(NSArray *)items {
    [clevertap recordChargedEventWithDetails:chargeDetails andItems:items];
}

- (void)recordErrorWithMessage:(NSString *)message andErrorCode:(int)code {
    [clevertap recordErrorWithMessage:message andErrorCode:code];
}

- (NSTimeInterval)eventGetFirstTime:(NSString *)event {
    return [clevertap eventGetFirstTime:event];
}

- (NSTimeInterval)eventGetLastTime:(NSString *)event {
    return [clevertap eventGetLastTime:event];
}

- (int)eventGetOccurrences:(NSString *)event {
    return [clevertap eventGetOccurrences:event];
}

- (NSDictionary *)userGetEventHistory {
    return [clevertap userGetEventHistory];
}

- (CleverTapEventDetail *)eventGetDetail:(NSString *)event {
    return [clevertap eventGetDetail:event];
}


#pragma mark - Session API

- (NSTimeInterval)sessionGetTimeElapsed {
    return [clevertap sessionGetTimeElapsed];
}

- (CleverTapUTMDetail *)sessionGetUTMDetails {
    return [clevertap sessionGetUTMDetails];
}

- (int)userGetTotalVisits {
    return [clevertap userGetTotalVisits];
}

- (int)userGetScreenCount {
    return [clevertap userGetScreenCount];
}

- (NSTimeInterval)userGetPreviousVisitTime {
    return [clevertap userGetPreviousVisitTime];
}

#pragma mark - Push Notifications

+ (void)registerPush {
    UIApplication *application = [UIApplication sharedApplication];
    if ([application respondsToSelector:@selector(isRegisteredForRemoteNotifications)]) {
        UIUserNotificationSettings *settings = [UIUserNotificationSettings
                                                settingsForTypes:UIUserNotificationTypeAlert | UIUserNotificationTypeBadge | UIUserNotificationTypeSound
                                                categories:nil];
        
        
        [application registerUserNotificationSettings:settings];
        [application registerForRemoteNotifications];
    }
    else {
#if __IPHONE_OS_VERSION_MAX_ALLOWED < 80000
        [application registerForRemoteNotificationTypes:
         (UIRemoteNotificationTypeBadge | UIRemoteNotificationTypeAlert | UIRemoteNotificationTypeSound)];
#endif
    }
}

- (void)setPushToken:(NSData *)pushToken {
    [clevertap setPushToken:pushToken];
}

- (void)setPushTokenAsString:(NSString *)pushTokenString {
    [clevertap setPushTokenAsString:pushTokenString];
}

- (void)handleNotificationWithData:(id)data {
    [clevertap handleNotificationWithData:data];
}

- (void)showInAppNotificationIfAny {
    [clevertap showInAppNotificationIfAny];
}

- (void)registerApplication:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)notification {
    [self handleNotificationWithData:notification];
    
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
            NSString *dataString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
            
            NSString *methodName = (application.applicationState == UIApplicationStateActive) ? kCleverTapPushReceivedCallback : kCleverTapPushOpenedCallback;
            
            [self callUnityObject:kCleverTapGameObjectName forMethod:methodName withMessage:dataString];
        }
    }
}


#pragma mark - DeepLink Handling

- (void)handleOpenURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication {
    
    [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapDeepLinkCallback withMessage:[url absoluteString]];
}


#pragma mark - Referrer Tracking

- (void)pushInstallReferrerSource:(NSString *)source
                           medium:(NSString *)medium
                         campaign:(NSString *)campaign {
    
    [clevertap pushInstallReferrerSource:source medium:medium campaign:campaign];
}


#pragma mark - InApp Notification Delegates

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
        [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapInAppNotificationDismissedCallback withMessage:jsonString];
    }
}

- (void)inAppNotificationButtonTappedWithCustomExtras:(NSDictionary *)customExtras {
    
    NSMutableDictionary *jsonDict = [NSMutableDictionary new];
    
    if (customExtras != nil) {
        jsonDict[@"customExtras"] = customExtras;
    }
    
    NSString *jsonString = [self dictToJson:jsonDict];
    
    if (jsonString != nil) {
        [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapInAppNotificationButtonTapped withMessage:jsonString];
    }
}


#pragma mark - App Inbox

- (void)initializeInbox {
    [clevertap initializeInboxWithCallback:^(BOOL success) {
        NSLog(@"Inbox initialized %d", success);
        [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapInboxDidInitializeCallback withMessage:[NSString stringWithFormat:@"%@", success? @"YES": @"NO"]];
        [self inboxMessagesDidUpdate];
    }];
}

- (void)inboxMessagesDidUpdate {
    [clevertap registerInboxUpdatedBlock:^{
        NSLog(@"Inbox Messages updated");
        [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapInboxMessagesDidUpdateCallback withMessage:@"inbox updated."];
    }];
}

- (void)showAppInbox:(NSDictionary *)styleConfig {
    CleverTapInboxViewController *inboxController = [clevertap newInboxViewControllerWithConfig:[self _dictToInboxStyleConfig:styleConfig? styleConfig : nil] andDelegate:self];
    if (inboxController) {
        UINavigationController *navigationController = [[UINavigationController alloc] initWithRootViewController:inboxController];
        [UnityGetGLViewController() presentViewController:navigationController animated:YES completion:nil];
    }
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
    return (int)[clevertap getInboxMessageUnreadCount];
}

- (int)getInboxMessageCount {
    return (int)[clevertap getInboxMessageCount];
}

- (NSArray *)getAllInboxMessages {
    
    NSArray *inboxMessages = [clevertap getAllInboxMessages];
    
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
    
    NSArray *inboxMessages = [clevertap getUnreadInboxMessages];
    
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
    
    CleverTapInboxMessage *message = [clevertap getInboxMessageForId:messageId];
    
    return message.json;
}

- (void)deleteInboxMessageForID:(NSString *)messageId {
    [clevertap deleteInboxMessageForID:messageId];
}

- (void)markReadInboxMessageForID:(NSString *)messageId {
    [clevertap markReadInboxMessageForID:messageId];
}

- (void)recordInboxNotificationViewedEventForID:(NSString *)messageId {
    
    [clevertap recordInboxNotificationViewedEventForID:messageId];
}

- (void)recordInboxNotificationClickedEventForID:(NSString *)messageId {
    [clevertap recordInboxNotificationClickedEventForID:messageId];
}

- (void)messageButtonTappedWithCustomExtras:(NSDictionary *)customExtras {
    
    NSMutableDictionary *jsonDict = [NSMutableDictionary new];
    
    if (customExtras != nil) {
        jsonDict[@"customExtras"] = customExtras;
    }
    
    NSString *jsonString = [self dictToJson:jsonDict];
    
    if (jsonString != nil) {
        [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapInboxCustomExtrasButtonSelect withMessage:jsonString];
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
        [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapNativeDisplayUnitsUpdated withMessage:jsonString];
    }
}

- (NSArray *)getAllDisplayUnits {
    
    NSArray *displayUnits = [clevertap getAllDisplayUnits];
    
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
    
    CleverTapDisplayUnit *unit = [clevertap getDisplayUnitForID:unitID];
    
    return unit.json;
}

- (void)recordDisplayUnitViewedEventForID:(NSString *)unitID {
    [clevertap recordDisplayUnitViewedEventForID:unitID];
}

- (void)recordDisplayUnitClickedEventForID:(NSString *)unitID {
    [clevertap recordDisplayUnitClickedEventForID:unitID];
}


#pragma mark - Product Config

- (void)ctProductConfigFetched {
    [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapProductConfigFetched withMessage:@"Product Config Fetched"];
}

- (void)ctProductConfigActivated {
    [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapProductConfigActivated withMessage:@"Product Config Activated"];
}

- (void)ctProductConfigInitialized {
    [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapProductConfigInitialized withMessage:@"Product Config Initialized"];
}

- (void)fetchProductConfig {
    [[clevertap productConfig] fetch];
}

- (void)fetchProductConfigWithMinimumInterval:(NSTimeInterval)minimumInterval {
    [[clevertap productConfig] fetchWithMinimumInterval:minimumInterval];
}

- (void)setProductConfigMinimumFetchInterval:(NSTimeInterval)minimumFetchInterval {
    [[clevertap productConfig] setMinimumFetchInterval:minimumFetchInterval];
}

- (void)activateProductConfig {
    [[clevertap productConfig] activate];
}

- (void)fetchAndActivateProductConfig {
    [[clevertap productConfig] fetchAndActivate];
}

- (void)setProductConfigDefaults:(NSDictionary *)defaults {
    [[clevertap productConfig] setDefaults:defaults];
}

- (void)setProductConfigDefaultsFromPlistFileName:(NSString *)fileName {
    [[clevertap productConfig] setDefaultsFromPlistFileName:fileName];
}

- (NSDictionary *)getProductConfigValueFor:(NSString *)key {
    CleverTapConfigValue *value = [[clevertap productConfig] get:key];
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
    NSDate *date = [[clevertap productConfig] getLastFetchTimeStamp];
    double interval = date.timeIntervalSince1970;
    return interval;
}

- (void)resetProductConfig {
    [[clevertap productConfig] reset];
}


#pragma mark - Feature Flags

- (void)ctFeatureFlagsUpdated {
    [self callUnityObject:kCleverTapGameObjectName forMethod:kCleverTapFeatureFlagsUpdated withMessage:@"Feature Flags updated"];
}

- (BOOL)get:(NSString *)key withDefaultValue:(BOOL)defaultValue {
    return [[clevertap featureFlags] get:key withDefaultValue:defaultValue];
}

#pragma mark - Private Helpers

- (void)callUnityObject:(NSString *)objectName forMethod:(NSString *)method withMessage:(NSString *)message {
    UnitySendMessage([objectName UTF8String], [method UTF8String], [message UTF8String]);
}

- (NSString *)dictToJson:(NSDictionary *)dict {
    NSError *err;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&err];
    
    if(err != nil) {
        return nil;
    }
    
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

#pragma mark - In App Controls

- (void)suspendInAppNotifications {
    [clevertap suspendInAppNotifications];
}

- (void)discardInAppNotifications {
    [clevertap discardInAppNotifications];
}

- (void)resumeInAppNotifications {
    [clevertap resumeInAppNotifications];
}

@end
