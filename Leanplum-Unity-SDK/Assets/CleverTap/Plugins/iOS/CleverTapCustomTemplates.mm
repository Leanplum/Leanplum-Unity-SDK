#import "CleverTapCustomTemplates.h"
#import "CleverTapTemplatePresenter.h"
#import "CleverTapAppFunctionPresenter.h"
#import "CTJsonTemplateProducer.h"
#import "CTCustomTemplatesManager.h"

@implementation CleverTapCustomTemplates

+ (void)registerCustomTemplates {
    NSString *resourcePath = [[NSBundle mainBundle] resourcePath];
    NSString *cleverTapPath = [resourcePath stringByAppendingPathComponent:kCleverTapDirName];
    NSString *customTemplatesPath = [cleverTapPath stringByAppendingPathComponent:kCustomTemplatesDirName];
    NSError *contentsOfDirectoryError;
    NSArray *directoryContents = [[NSFileManager defaultManager] contentsOfDirectoryAtPath:customTemplatesPath error:&contentsOfDirectoryError];
    if (contentsOfDirectoryError) {
        NSLog(@"Error getting contents of directory at path: %@. Error: %@.", customTemplatesPath, contentsOfDirectoryError);
        return;
    }
    if (directoryContents.count == 0) {
        NSLog(@"%@ directory is empty.", kCustomTemplatesDirName);
        return;
    }

    NSString *jsonExtension = @".json";
    CleverTapTemplatePresenter *templatePresenter = [[CleverTapTemplatePresenter alloc] init];
    CleverTapAppFunctionPresenter *functionPresenter = [[CleverTapAppFunctionPresenter alloc] init];
    for (NSString *file in directoryContents) {
        if ([file hasSuffix:jsonExtension]) {
            NSString *filePath = [customTemplatesPath stringByAppendingPathComponent:file];
            NSError *error;
            NSString *definitionsJson = [NSString stringWithContentsOfFile:filePath encoding:NSUTF8StringEncoding error:&error];
            if (error) {
                NSLog(@"Error getting contents of file: %@. Error: %@.", filePath, error);
                continue;
            }
            CTJsonTemplateProducer *producer = [[CTJsonTemplateProducer alloc] initWithJson:definitionsJson templatePresenter:templatePresenter functionPresenter:functionPresenter];
            [CleverTap registerCustomInAppTemplates:producer];
        }
    }
}

@end
