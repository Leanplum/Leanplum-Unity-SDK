#import "CleverTapUnityAppController.h"
#import "CleverTapUnityManager.h"
#import "CleverTapCustomTemplates.h"
#import <CleverTapSDK/CleverTap.h>

#pragma mark - Constants
#define CT_ACCOUNT_ID_LABEL @"CleverTapAccountID"
#define CT_TOKEN_LABEL @"CleverTapToken"
#define CT_REGION_LABEL @"CleverTapRegion"
#define CT_PROXY_DOMAIN_LABEL @"CleverTapProxyDomain"
#define CT_SPIKY_PROXY_DOMAIN_LABEL @"CleverTapSpikyProxyDomain"
#define CT_PRESENT_NOTIF_FOREGROUND @"CleverTapPresentNotificationForeground"

#pragma mark - CleverTapUnityAppController
@implementation CleverTapUnityAppController

#pragma mark - application:didFinishLaunchingWithOptions:
- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    // Register Custom Templates
    [CleverTapCustomTemplates registerCustomTemplates];
    
#if !NO_UNUSERNOTIFICATIONCENTER
    // Set UNUserNotificationCenter delegate to self based on settings
    [UNUserNotificationCenter currentNotificationCenter].delegate = (id <UNUserNotificationCenterDelegate>)self;
    
    NSString *presentNotificationInForegroundMeta = [self metaDataForAttribute:CT_PRESENT_NOTIF_FOREGROUND];
    self.presentNotificationInForeground = [presentNotificationInForegroundMeta boolValue];
#endif
    
#ifdef NO_AUTOINTEGRATE
    NSString *_accountId = [self getMetaDataForAttribute:CT_ACCOUNT_ID_LABEL];
    NSString *_accountToken = [self getMetaDataForAttribute:CT_TOKEN_LABEL];
    NSString *_accountRegion = [self getMetaDataForAttribute:CT_REGION_LABEL];
    NSString *_proxyDomain = [self getMetaDataForAttribute:CT_PROXY_DOMAIN_LABEL];
    NSString *_spikyProxyDomain = [self getMetaDataForAttribute:CT_SPIKY_PROXY_DOMAIN_LABEL];
    if (_accountRegion) {
        [CleverTap setCredentialsWithAccountID:_accountId token:_accountToken region:_accountRegion];
    } else {
        [CleverTap setCredentialsWithAccountID:_accountId token:_accountToken proxyDomain:_proxyDomain spikyProxyDomain:_spikyProxyDomain];
    }
    [[CleverTap sharedInstance] notifyApplicationLaunchedWithOptions:launchOptions];
#else
    // Use Auto Integrate
    [CleverTap autoIntegrate];
#endif
    // Initialize the CleverTapUnityManager
    [CleverTapUnityManager sharedInstance];
    
    // Return super application:didFinishLaunchingWithOptions:
    return [super application:application didFinishLaunchingWithOptions:launchOptions];
}

- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options {
    [[CleverTapUnityManager sharedInstance] handleOpenURL:url];
    return [super application:app openURL:url options:options];
}

- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult))completionHandler {
    BOOL isOpen = application.applicationState == UIApplicationStateActive ? NO : YES;
#ifdef NO_AUTOINTEGRATE
    // Handle the push notification
    [[CleverTapUnityManager sharedInstance] didReceiveRemoteNotification:userInfo isOpen:isOpen openInForeground:NO];
#else
    // Send callback to unity
    [[CleverTapUnityManager sharedInstance] sendRemoteNotificationCallbackToUnity:userInfo isOpen:isOpen];
#endif
    [super application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:completionHandler];
}

#pragma mark - UNUserNotificationCenter methods
#if !NO_UNUSERNOTIFICATIONCENTER
- (void)userNotificationCenter:(UNUserNotificationCenter *)center didReceiveNotificationResponse:(UNNotificationResponse *)response withCompletionHandler:(void (^)(void))completionHandler {
    NSDictionary *userInfo = response.notification.request.content.userInfo;
#ifdef NO_AUTOINTEGRATE
    [[CleverTapUnityManager sharedInstance] didReceiveRemoteNotification:userInfo
                                                                  isOpen:YES openInForeground:YES];
#else
    [[CleverTapUnityManager sharedInstance]
     sendRemoteNotificationCallbackToUnity:userInfo isOpen:YES];
#endif
    completionHandler();
}

- (void)userNotificationCenter:(UNUserNotificationCenter *)center willPresentNotification:(UNNotification *)notification withCompletionHandler:(void (^)(UNNotificationPresentationOptions))completionHandler {
    if (self.presentNotificationInForeground) {
        [[CleverTapUnityManager sharedInstance]
         sendRemoteNotificationCallbackToUnity:notification.request.content.userInfo isOpen:NO];
        if (@available(iOS 14.0, *)) {
            completionHandler(UNNotificationPresentationOptionBanner | UNNotificationPresentationOptionBadge | UNNotificationPresentationOptionSound);
        } else {
            completionHandler(UNNotificationPresentationOptionAlert | UNNotificationPresentationOptionBadge | UNNotificationPresentationOptionSound);
        }
    } else {
        [[CleverTapUnityManager sharedInstance] didReceiveRemoteNotification:notification.request.content.userInfo isOpen:YES openInForeground:YES];
        completionHandler(UNNotificationPresentationOptionNone);
    }
}
#endif

#pragma mark - No Auto Integrate Push Registration
#ifdef NO_AUTOINTEGRATE
- (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken {
    [[CleverTapUnityManager sharedInstance] setPushToken:deviceToken];
    [super application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
}

- (void)application:(UIApplication *)application didFailToRegisterForRemoteNotificationsWithError:(NSError *)error {
    NSLog(@"Failed to register for remote notifications: %@", error);
    [super application:application didFailToRegisterForRemoteNotificationsWithError:error];
}
#endif

#pragma mark - Utils
static NSDictionary *plistRootInfoDict;

- (id)infoDictionaryValueForKey:(NSString *)key {
    if (!plistRootInfoDict) {
        plistRootInfoDict = [[NSBundle mainBundle] infoDictionary];
    }
    return plistRootInfoDict[key];
}

- (NSString *)metaDataForAttribute:(NSString *)name {
    id _value = [self infoDictionaryValueForKey:name];
    
    if(_value && ![_value isKindOfClass:[NSString class]]) {
        _value = [NSString stringWithFormat:@"%@", _value];
    }
    
    NSString *value = (NSString *)_value;
    if (value == nil || [[value stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]] isEqualToString:@""]) {
        value = nil;
    } else {
        value = [value stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]];
    }
    return value;
}

@end
