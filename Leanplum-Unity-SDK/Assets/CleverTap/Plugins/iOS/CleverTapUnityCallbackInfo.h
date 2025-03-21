#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM(NSInteger, CleverTapUnityCallback) {
    CleverTapUnityCallbackProfileInitialized = 0,
    CleverTapUnityCallbackProfileUpdates = 1,
    CleverTapUnityCallbackDeepLink = 2,
    CleverTapUnityCallbackPushNotificationTappedWithCustomExtras = 3,
    CleverTapUnityCallbackPushOpened = 4,
    CleverTapUnityCallbackInAppNotificationDismissed = 5,
    CleverTapUnityCallbackInAppNotificationButtonTapped = 6,
    CleverTapUnityCallbackInboxDidInitialize = 7,
    CleverTapUnityCallbackInboxMessagesDidUpdate = 8,
    CleverTapUnityCallbackInboxCustomExtrasButtonSelect = 9,
    CleverTapUnityCallbackInboxItemClicked = 10,
    CleverTapUnityCallbackNativeDisplayUnitsUpdated = 11,
    CleverTapUnityCallbackFeatureFlagsUpdated = 12,
    CleverTapUnityCallbackProductConfigInitialized = 13,
    CleverTapUnityCallbackProductConfigFetched = 14,
    CleverTapUnityCallbackProductConfigActivated = 15,
    CleverTapUnityCallbackVariablesChanged = 16,
    CleverTapUnityCallbackVariableValueChanged = 17,
    CleverTapUnityCallbackVariablesFetched = 18,
    CleverTapUnityCallbackInAppsFetched = 19,
    CleverTapUnityCallbackVariablesChangedAndNoDownloadsPending = 20,
    CleverTapUnityCallbackVariableFileIsReady = 21,
    CleverTapUnityCallbackCustomTemplatePresent = 22,
    CleverTapUnityCallbackCustomFunctionPresent = 23,
    CleverTapUnityCallbackCustomTemplateClose = 24,
    CleverTapUnityCallbackPushPermissionResponseReceived = 25,
    CleverTapUnityCallbackPushNotificationPermissionStatus = 26,
    CleverTapUnityCallbackInAppNotificationDidShow = 27,
    CleverTapUnityCallbackOneTimeVariablesChanged = 28,
    CleverTapUnityCallbackOneTimeVariablesChangedAndNoDownloadsPending = 29,
};

@interface CleverTapUnityCallbackInfo : NSObject <NSCopying>

@property (nonatomic, strong, readonly) NSString *callbackName;
@property (nonatomic, assign, readonly) BOOL isBufferable;

+ (nullable CleverTapUnityCallbackInfo *)infoForCallback:(CleverTapUnityCallback)callback;
+ (nullable CleverTapUnityCallbackInfo *)callbackFromName:(NSString *)callbackName;
+ (NSDictionary<NSNumber *, CleverTapUnityCallbackInfo *> *)callbackInfos;
+ (nullable NSNumber *)callbackEnumForName:(NSString *)callbackName;

@end

NS_ASSUME_NONNULL_END
