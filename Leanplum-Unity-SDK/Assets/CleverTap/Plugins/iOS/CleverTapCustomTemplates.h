#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

static NSString * kCleverTapDirName = @"CleverTapSDK";
static NSString * kCustomTemplatesDirName = @"CustomTemplates";

/// Registers Custom Templates and App Functions using JSON templates definitions.
@interface CleverTapCustomTemplates : NSObject

/// Registers Custom Templates and App Functions using the JSON templates definition files.
/// Uses the files located in `CleverTapSDK/CustomTemplates` folder in the main bundle.
/// Ensure the folder is aded to the Copy Bundle Resources phase.
+ (void)registerCustomTemplates;

@end

NS_ASSUME_NONNULL_END
