#import "CleverTapUnityCallbackInfo.h"
#import <Foundation/Foundation.h>

@implementation CleverTapUnityCallbackInfo

- (instancetype)initWithName:(NSString *)callbackName bufferable:(BOOL)bufferable {
    self = [super init];
    if (self) {
        _callbackName = callbackName;
        _isBufferable = bufferable;
    }
    return self;
}

- (BOOL)isEqual:(id)object {
    if (self == object) return YES;
    if (![object isKindOfClass:[CleverTapUnityCallbackInfo class]]) return NO;
    
    CleverTapUnityCallbackInfo *other = (CleverTapUnityCallbackInfo *)object;
    return [self.callbackName isEqualToString:other.callbackName] && self.isBufferable == other.isBufferable;
}

- (NSUInteger)hash {
    return self.callbackName.hash ^ @(self.isBufferable).hash;
}

- (id)copyWithZone:(NSZone *)zone {
    return [[CleverTapUnityCallbackInfo allocWithZone:zone] initWithName:self.callbackName bufferable:self.isBufferable];
}

+ (NSDictionary<NSNumber *, CleverTapUnityCallbackInfo *> *)callbackInfos {
    static NSDictionary<NSNumber *, CleverTapUnityCallbackInfo *> *callbacks = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        callbacks = @{
            @(CleverTapUnityCallbackProfileInitialized):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapProfileInitializedCallback" bufferable:YES],
            @(CleverTapUnityCallbackProfileUpdates):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapProfileUpdatesCallback" bufferable:NO],
            @(CleverTapUnityCallbackDeepLink):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapDeepLinkCallback" bufferable:YES],
            @(CleverTapUnityCallbackPushNotificationTappedWithCustomExtras):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapPushNotificationTappedWithCustomExtrasCallback" bufferable:YES],
            @(CleverTapUnityCallbackPushOpened):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapPushOpenedCallback" bufferable:YES],
            @(CleverTapUnityCallbackInAppNotificationDismissed):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapInAppNotificationDismissedCallback" bufferable:YES],
            @(CleverTapUnityCallbackInAppNotificationButtonTapped):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapInAppNotificationButtonTapped" bufferable:YES],
            @(CleverTapUnityCallbackInboxDidInitialize):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapInboxDidInitializeCallback" bufferable:YES],
            @(CleverTapUnityCallbackInboxMessagesDidUpdate):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapInboxMessagesDidUpdateCallback" bufferable:NO],
            @(CleverTapUnityCallbackInboxCustomExtrasButtonSelect):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapInboxCustomExtrasButtonSelect" bufferable:NO],
            @(CleverTapUnityCallbackInboxItemClicked):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapInboxItemClicked" bufferable:NO],
            @(CleverTapUnityCallbackNativeDisplayUnitsUpdated):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapNativeDisplayUnitsUpdated" bufferable:YES],
            @(CleverTapUnityCallbackFeatureFlagsUpdated):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapFeatureFlagsUpdated" bufferable:YES],
            @(CleverTapUnityCallbackProductConfigInitialized):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapProductConfigInitialized" bufferable:YES],
            @(CleverTapUnityCallbackProductConfigFetched):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapProductConfigFetched" bufferable:NO],
            @(CleverTapUnityCallbackProductConfigActivated):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapProductConfigActivated" bufferable:NO],
            @(CleverTapUnityCallbackVariablesChanged):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapVariablesChanged" bufferable:NO],
            @(CleverTapUnityCallbackVariableValueChanged):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapVariableValueChanged" bufferable:NO],
            @(CleverTapUnityCallbackVariablesFetched):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapVariablesFetched" bufferable:NO],
            @(CleverTapUnityCallbackInAppsFetched):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapInAppsFetched" bufferable:NO],
            @(CleverTapUnityCallbackVariablesChangedAndNoDownloadsPending):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapVariablesChangedAndNoDownloadsPending" bufferable:NO],
            @(CleverTapUnityCallbackVariableFileIsReady):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapVariableFileIsReady" bufferable:NO],
            @(CleverTapUnityCallbackCustomTemplatePresent):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapCustomTemplatePresent" bufferable:YES],
            @(CleverTapUnityCallbackCustomFunctionPresent):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapCustomFunctionPresent" bufferable:YES],
            @(CleverTapUnityCallbackCustomTemplateClose):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapCustomTemplateClose" bufferable:NO],
            @(CleverTapUnityCallbackPushPermissionResponseReceived):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapOnPushPermissionResponseCallback" bufferable:YES],
            @(CleverTapUnityCallbackPushNotificationPermissionStatus):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapPushNotificationPermissionStatus" bufferable:NO],
            @(CleverTapUnityCallbackInAppNotificationDidShow):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"CleverTapInAppNotificationShowCallback" bufferable:YES],
            @(CleverTapUnityCallbackOneTimeVariablesChanged):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"OneTimeCleverTapVariablesChanged" bufferable:NO],
            @(CleverTapUnityCallbackOneTimeVariablesChangedAndNoDownloadsPending):
                [[CleverTapUnityCallbackInfo alloc] initWithName:@"OneTimeCleverTapVariablesChangedAndNoDownloadsPending" bufferable:NO]
        };
    });
    return callbacks;
}

+ (NSDictionary<NSString *, NSNumber *> *)namesCallbacks {
    NSDictionary *callbackInfos = [self callbackInfos];
    static NSDictionary<NSString *, NSNumber *> *namesCallbacks = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        NSMutableDictionary<NSString *, NSNumber *> *namesCallbacksMutable = [NSMutableDictionary dictionaryWithCapacity:callbackInfos.count];
        for (NSNumber *key in callbackInfos) {
            CleverTapUnityCallbackInfo *info = [callbackInfos objectForKey:key];
            [namesCallbacksMutable setObject:key forKey:info.callbackName];
        }
        namesCallbacks = namesCallbacksMutable;
    });
    return namesCallbacks;
}

+ (nullable CleverTapUnityCallbackInfo *)infoForCallback:(CleverTapUnityCallback)callback {
    return [self callbackInfos][@(callback)];
}

+ (nullable CleverTapUnityCallbackInfo *)callbackFromName:(NSString *)callbackName {
    NSNumber *index = [self callbackEnumForName:callbackName];
    if (index) {
        CleverTapUnityCallback callback = (CleverTapUnityCallback)[index integerValue];
        return [self infoForCallback:(CleverTapUnityCallback)callback];
    }
    return nil;
}

+ (nullable NSNumber *)callbackEnumForName:(NSString *)callbackName {
    return [self namesCallbacks][callbackName];
}

@end
