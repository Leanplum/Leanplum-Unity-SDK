#import "CleverTapAppFunctionPresenter.h"
#import "CleverTapUnityManager.h"
#import "CleverTapMessageSender.h"
#import "CleverTapUnityCallbackInfo.h"

@implementation CleverTapAppFunctionPresenter

- (void)onPresent:(nonnull CTTemplateContext *)context {
    [[CleverTapMessageSender sharedInstance] send:CleverTapUnityCallbackCustomFunctionPresent withMessage:context.templateName];
}

- (void)onCloseClicked:(nonnull CTTemplateContext *)context {
    // NOOP - App Functions cannot have Action arguments.
}

@end
