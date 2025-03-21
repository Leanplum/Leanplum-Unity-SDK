#import <Foundation/Foundation.h>
#import "UnityAppController.h"
#import <UserNotifications/UserNotifications.h>

NS_ASSUME_NONNULL_BEGIN

@interface CleverTapUnityAppController : UnityAppController <UNUserNotificationCenterDelegate>

@property (nonatomic) BOOL presentNotificationInForeground;

@end

#if !CT_NO_APP_CONTROLLER_SUBCLASS
IMPL_APP_CONTROLLER_SUBCLASS(CleverTapUnityAppController)
#endif

NS_ASSUME_NONNULL_END
