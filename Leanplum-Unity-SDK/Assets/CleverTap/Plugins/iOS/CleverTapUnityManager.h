#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>

#import <CleverTapSDK/CleverTap.h>
#import <CleverTapSDK/CleverTapUTMDetail.h>
#import <CleverTapSDK/CleverTapEventDetail.h>

NS_ASSUME_NONNULL_BEGIN

typedef void (*UserEventLogCallback) (const char *, const char *);

@interface CleverTapUnityManager : NSObject

+ (CleverTapUnityManager *)sharedInstance;

- (void)onPlatformInit;
- (void)onCallbackAdded:(NSString *)callbackName;
- (void)onVariablesCallbackAdded:(NSString *)callbackName callbackId:(int)callbackId;
- (void)setCleverTapInstance:(CleverTap *)instance;

#pragma mark - Admin

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

- (NSTimeInterval)eventGetFirstTime:(NSString *)event __attribute__((deprecated("Deprecated, use getUserEventLog instead")));
- (NSTimeInterval)eventGetLastTime:(NSString *)event __attribute__((deprecated("Deprecated, use getUserEventLog instead")));
- (int)eventGetOccurrences:(NSString *)event __attribute__((deprecated("Deprecated, use getUserEventLogCount instead")));
- (NSDictionary *)userGetEventHistory __attribute__((deprecated("Deprecated, use getUserEventLogHistory instead")));
- (CleverTapEventDetail *)eventGetDetail:(NSString *)event __attribute__((deprecated("Deprecated, use getUserEventLog instead")));

- (void)getUserEventLog:(NSString *)eventName forKey:(NSString *)key withCallback:(UserEventLogCallback)callback;
- (void)getUserAppLaunchCount:(NSString *)key withCallback:(UserEventLogCallback)callback;
- (void)getUserEventLogCount:(NSString *)eventName forKey:(NSString *)key withCallback:(UserEventLogCallback)callback;
- (void)getUserEventLogHistory:(NSString *)key withCallback:(UserEventLogCallback)callback;
- (long)getUserLastVisitTs;

#pragma mark - User Session

- (NSTimeInterval)sessionGetTimeElapsed;
- (CleverTapUTMDetail *)sessionGetUTMDetails;
- (int)userGetTotalVisits __attribute__((deprecated("Deprecated, use getUserAppLaunchCount instead")));
- (int)userGetScreenCount;
- (NSTimeInterval)userGetPreviousVisitTime __attribute__((deprecated("Deprecated, use getUserLastVisitTs instead")));

#pragma mark - Push Notifications

- (void)setPushToken:(NSData *)pushToken;
- (void)setPushTokenAsString:(NSString *)pushTokenString;
- (void)didReceiveRemoteNotification:(NSDictionary *)notification
                              isOpen:(BOOL)isOpen
                    openInForeground:(BOOL)openInForeground;
- (void)sendRemoteNotificationCallbackToUnity:(NSDictionary *)notification isOpen:(BOOL)isOpen;

- (void)handleOpenURL:(NSURL *)url;
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
- (void)defineFileVar:(NSString *)name;
- (NSString *)getVariableValue:(NSString *)name;
- (NSString *)getFileVariableValue:(NSString *)name;

#pragma mark - Client-side In-Apps

- (void)fetchInApps:(int)callbackId;
- (void)clearInAppResources:(BOOL)expiredOnly;

#pragma mark - Custom Templates

- (void)customTemplateSetPresented:(NSString *)name;
- (void)customTemplateSetDismissed:(NSString *)name;
- (void)customTemplateTriggerAction:(NSString *)templateName named:(NSString *)argumentName;

- (NSString *)customTemplateContextToString:(NSString *)name;
- (NSString *)customTemplateGetStringArg:(NSString *)templateName named:(NSString *)argumentName;

- (BOOL)customTemplateGetBooleanArg:(NSString *)templateName named:(NSString *)argumentName;

- (NSString *)customTemplateGetFileArg:(NSString *)templateName named:(NSString *)argumentName;

- (NSDictionary *)customTemplateGetDictionaryArg:(NSString *)templateName named:(NSString *)argumentName;

- (int)customTemplateGetIntArg:(NSString *)templateName named:(NSString *)argumentName;

- (double)customTemplateGetDoubleArg:(NSString *)templateName named:(NSString *)argumentName;

- (float)customTemplateGetFloatArg:(NSString *)templateName named:(NSString *)argumentName;

- (int64_t)customTemplateGetLongArg:(NSString *)templateName named:(NSString *)argumentName;

- (int16_t)customTemplateGetShortArg:(NSString *)templateName named:(NSString *)argumentName;

- (int8_t)customTemplateGetByteArg:(NSString *)templateName named:(NSString *)argumentName;

- (void)syncCustomTemplates:(BOOL)isProduction;

@end

NS_ASSUME_NONNULL_END
