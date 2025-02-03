#import <Foundation/Foundation.h>
#import "CleverTapUnityCallbackInfo.h"

NS_ASSUME_NONNULL_BEGIN

typedef void (*InAppNotificationButtonTapped) (const char *);

@interface CleverTapMessageSender : NSObject

+ (instancetype)sharedInstance;

- (void)send:(CleverTapUnityCallback)callback withMessage:(NSString *)message;
- (void)flushBuffer:(CleverTapUnityCallbackInfo *)callbackInfo;
- (void)disableBuffer:(CleverTapUnityCallbackInfo *)callbackInfo;
- (void)resetAllBuffers;

- (void)setInAppNotificationButtonTappedCallback:(InAppNotificationButtonTapped)callback;

@end

NS_ASSUME_NONNULL_END
