#import "CleverTapMessageBuffer.h"

@interface CleverTapMessageBuffer ()
@property (nonatomic, strong, readonly) NSObject *lock;
@end

@implementation CleverTapMessageBuffer

- (instancetype)initWithEnabled:(BOOL)isEnabled {
    if (self = [super init]) {
        _isEnabled = isEnabled;
        _items = [NSMutableArray array];
        _lock = [[NSObject alloc] init];
    }
    return self;
}

- (void)addItem:(NSString *)item {
    @synchronized(self.lock) {
        if (item) {
            [self.items addObject:item];
        }
    }
}

- (nullable NSString *)popItem {
    @synchronized(self.lock) {
        if (self.items.count > 0) {
            NSString *last = [self.items lastObject];
            [self.items removeLastObject];
            return last;
        }
        return nil;
    }
}

- (NSUInteger)count {
    @synchronized(self.lock) {
        return self.items.count;
    }
}

@end
