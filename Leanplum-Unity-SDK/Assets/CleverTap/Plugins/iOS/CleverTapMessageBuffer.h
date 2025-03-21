#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface CleverTapMessageBuffer : NSObject

@property (nonatomic, assign) BOOL isEnabled;
@property (nonatomic, strong) NSMutableArray *items;

- (instancetype)initWithEnabled:(BOOL)isEnabled;

- (void)addItem:(NSString *)item;
- (nullable NSString *)popItem;
- (NSUInteger)count;

@end

NS_ASSUME_NONNULL_END
