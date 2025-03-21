#import "CleverTapMessageSender.h"
#import "CleverTapUnityCallbackInfo.h"
#import "CleverTapMessageBuffer.h"

static NSString * kCleverTapGameObjectName = @"IOSCallbackHandler";

@interface CleverTapMessageSender()

@property NSDictionary<CleverTapUnityCallbackInfo *, CleverTapMessageBuffer *> *messageBuffers;
@property (nonatomic) InAppNotificationButtonTapped inAppNotificationButtonTappedCallback;

@end

@implementation CleverTapMessageSender

+ (instancetype)sharedInstance {
    static dispatch_once_t once = 0;
    static id _sharedObject = nil;
    dispatch_once(&once, ^{
        _sharedObject = [[self alloc] init];
    });
    return _sharedObject;
}

- (instancetype)init {
    if (self = [super init]) {
        self.messageBuffers = [self createBuffers:YES];
    }
    return self;
}

- (NSDictionary<CleverTapUnityCallbackInfo *, CleverTapMessageBuffer *> *)createBuffers:(BOOL)enabled {
    NSMutableDictionary<CleverTapUnityCallbackInfo *, CleverTapMessageBuffer *> *buffers = [NSMutableDictionary dictionary];
    NSArray *callbackInfos = [[CleverTapUnityCallbackInfo callbackInfos] allValues];
    for (CleverTapUnityCallbackInfo *info in callbackInfos) {
        if (info.isBufferable) {
            buffers[info] = [[CleverTapMessageBuffer alloc] initWithEnabled:enabled];
        }
    }
    return [NSDictionary dictionaryWithDictionary:buffers];
}

- (void)send:(CleverTapUnityCallback)callback withMessage:(NSString *)message {
    @synchronized (self) {
        CleverTapUnityCallbackInfo *callbackInfo = [CleverTapUnityCallbackInfo infoForCallback:callback];
        CleverTapMessageBuffer *buffer = self.messageBuffers[callbackInfo];
        if (callbackInfo.isBufferable && buffer.isEnabled) {
            [buffer addItem:message];
            return;
        }
        
        if (callback == CleverTapUnityCallbackInAppNotificationButtonTapped) {
            // if direct callback is set call it otherwise fallback to UnitySendMessage
            if (self.inAppNotificationButtonTappedCallback) {
                self.inAppNotificationButtonTappedCallback([message UTF8String]);
                return;
            }
        }
        
        [self sendToUnity:callbackInfo withMessage:message];
    }
}

- (void)sendToUnity:(CleverTapUnityCallbackInfo *)callbackInfo withMessage:(NSString *)message {
    if (!callbackInfo) {
        NSLog(@"Cannot send message for nil callback.");
        return;
    }
    if (!message) {
        NSLog(@"Cannot send nil message to Unity. Callback: %@.", callbackInfo.callbackName);
        return;
    }
    
    UnitySendMessage([kCleverTapGameObjectName UTF8String], [callbackInfo.callbackName UTF8String], [message UTF8String]);
}

- (void)flushBuffer:(CleverTapUnityCallbackInfo *)callbackInfo {
    @synchronized (self) {
        CleverTapMessageBuffer *buffer = self.messageBuffers[callbackInfo];
        if (!buffer) {
            return;
        }
        
        while (buffer.count > 0) {
            NSString *message = [buffer popItem];
            [self sendToUnity:callbackInfo withMessage:message];
        }
    }
}

- (void)disableBuffer:(CleverTapUnityCallbackInfo *)callbackInfo {
    @synchronized (self) {
        CleverTapMessageBuffer *buffer = self.messageBuffers[callbackInfo];
        if (buffer) {
            [buffer setIsEnabled:NO];
        }
    }
}

- (void)resetAllBuffers {
    @synchronized (self) {
        self.messageBuffers = [self createBuffers:NO];
    }
}

- (void)setInAppNotificationButtonTappedCallback:(InAppNotificationButtonTapped)callback {
    _inAppNotificationButtonTappedCallback = callback;
}

@end
