#import "CleverTapTemplatePresenter.h"
#import "CleverTapUnityManager.h"
#import "CleverTapMessageSender.h"
#import "CleverTapUnityCallbackInfo.h"

@implementation CleverTapTemplatePresenter

- (void)onPresent:(nonnull CTTemplateContext *)context {
    [[CleverTapMessageSender sharedInstance] send:CleverTapUnityCallbackCustomTemplatePresent withMessage:context.templateName];
}

- (void)onCloseClicked:(nonnull CTTemplateContext *)context {
    [[CleverTapMessageSender sharedInstance] send:CleverTapUnityCallbackCustomTemplateClose withMessage:context.templateName];
}

@end
