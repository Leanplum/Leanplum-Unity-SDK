#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>

#import <CleverTapSDK/CleverTap.h>
#import <CleverTapSDK/CleverTapUTMDetail.h>
#import <CleverTapSDK/CleverTapEventDetail.h>


@interface CleverTapUnityManager : NSObject

+ (CleverTapUnityManager *)sharedInstance;

+ (void)launchWithAccountID:(NSString*)accountID andToken:(NSString *)token;
+ (void)launchWithAccountID:(NSString*)accountID token:(NSString *)token region:(NSString *)region;
+ (void)setDebugLevel:(int)level;
+ (void)enablePersonalization;
+ (void)disablePersonalization;
+ (void)setLocation:(CLLocationCoordinate2D)location;
+ (void)registerPush;
+ (void)setApplicationIconBadgeNumber:(int)num;


#pragma mark - Offline API

- (void)setOffline:(BOOL)enabled;


#pragma mark - Opt-out API

- (void)setOptOut:(BOOL)enabled;
- (void)enableDeviceNetworkInfoReporting:(BOOL)enabled;


#pragma mark - User Profile

- (void)onUserLogin:(NSDictionary *)properties;
- (void)profilePush:(NSDictionary *)properties;
- (void)profileRemoveValueForKey:(NSString *)key;
- (void)profileSetMultiValues:(NSArray<NSString *> *)values forKey:(NSString*)key;
- (void)profileAddMultiValue:(NSString *)value forKey:(NSString *)key;
- (void)profileAddMultiValues:(NSArray<NSString *> *)values forKey:(NSString*)key;
- (void)profileRemoveMultiValue:(NSString *)value forKey:(NSString *)key;
- (void)profileRemoveMultiValues:(NSArray<NSString *> *)values forKey:(NSString*)key;
- (void)profileIncrementValueBy:(NSNumber* _Nonnull)value forKey:(NSString *_Nonnull)key;
- (void)profileDecrementValueBy:(NSNumber* _Nonnull)value forKey:(NSString *_Nonnull)key;
- (id)profileGet:(NSString *)propertyName;
- (NSString *)profileGetCleverTapID;
- (NSString *)profileGetCleverTapAttributionIdentifier;


#pragma mark - User Action Events

- (void)recordScreenView:(NSString *)screenName;
- (void)recordEvent:(NSString *)event;
- (void)recordEvent:(NSString *)event withProps:(NSDictionary *)properties;
- (void)recordChargedEventWithDetails:(NSDictionary *)chargeDetails andItems:(NSArray *)items;

- (NSTimeInterval)eventGetFirstTime:(NSString *)event;
- (NSTimeInterval)eventGetLastTime:(NSString *)event;
- (int)eventGetOccurrences:(NSString *)event;
- (NSDictionary *)userGetEventHistory;
- (CleverTapEventDetail *)eventGetDetail:(NSString *)event;


#pragma mark - User Session

- (NSTimeInterval)sessionGetTimeElapsed;
- (CleverTapUTMDetail *)sessionGetUTMDetails;
- (int)userGetTotalVisits;
- (int)userGetScreenCount;
- (NSTimeInterval)userGetPreviousVisitTime;


#pragma mark - Push Notifications

- (void)setPushToken:(NSData *)pushToken;
- (void)setPushTokenAsString:(NSString *)pushTokenString;
- (void)registerApplication:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)notification;

- (void)handleOpenURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication;
- (void)pushInstallReferrerSource:(NSString *)source
                           medium:(NSString *)medium
                         campaign:(NSString *)campaign;


#pragma mark - App Inbox

- (void)initializeInbox;
- (void)showAppInbox:(NSDictionary *)styleConfig;
- (void)dismissAppInbox;
- (int)getInboxMessageUnreadCount;
- (int)getInboxMessageCount;
- (NSArray *)getAllInboxMessages;
- (NSArray *)getUnreadInboxMessages;
- (NSDictionary *)getInboxMessageForId:(NSString *)messageId;
- (void)deleteInboxMessageForID:(NSString *)messageId;
- (void)deleteInboxMessagesForIDs:(NSArray *)messageIds;
- (void)markReadInboxMessageForID:(NSString *)messageId;
- (void)markReadInboxMessagesForIDs:(NSArray *)messageIds;
- (void)recordInboxNotificationViewedEventForID:(NSString *)messageId;
- (void)recordInboxNotificationClickedEventForID:(NSString *)messageId;


#pragma mark - Native Display

- (NSArray *)getAllDisplayUnits;
- (NSDictionary *)getDisplayUnitForID:(NSString *)unitID;
- (void)recordDisplayUnitViewedEventForID:(NSString *)unitID;
- (void)recordDisplayUnitClickedEventForID:(NSString *)unitID;


#pragma mark - Product Config

- (void)fetchProductConfig;
- (void)fetchProductConfigWithMinimumInterval:(NSTimeInterval)minimumInterval;
- (void)setProductConfigMinimumFetchInterval:(NSTimeInterval)minimumFetchInterval;
- (void)activateProductConfig;
- (void)fetchAndActivateProductConfig;
- (void)setProductConfigDefaults:(NSDictionary *)defaults;
- (void)setProductConfigDefaultsFromPlistFileName:(NSString *)fileName;
- (NSDictionary *)getProductConfigValueFor:(NSString *)key;
- (double)getProductConfigLastFetchTimeStamp;
- (void)resetProductConfig;


#pragma mark - Feature Flags

- (BOOL)get:(NSString *)key withDefaultValue:(BOOL)defaultValue;

#pragma mark - In-App Controls

- (void)suspendInAppNotifications;
- (void)discardInAppNotifications;
- (void)resumeInAppNotifications;

#pragma mark - Push Primer
- (void)promptForPushPermission:(BOOL)showFallbackSettings;
- (void)promptPushPrimer:(NSDictionary *)json;
- (void)isPushPermissionGranted;


#pragma mark - Variables
- (void)syncVariables;
- (void)syncVariables:(BOOL)isProduction;
- (void)fetchVariables:(int)callbackId;

- (void)defineVar:(NSString *)name kind:(NSString *)kind andDefaultValue:(NSString *)defaultValue;
- (NSString *)getVariableValue:(NSString *)name;

#pragma mark - Client-side In-Apps
- (void)fetchInApps:(int)callbackId;
- (void)clearInAppResources:(BOOL)expiredOnly;

@end
