//
//  EmbeddedHTMLUrlCallback.h
//  LeanplumSDK-iOS
//
//  Created by Santiago Castañeda Muñoz on 9/24/19.
//  Copyright © 2019 Tilting Point - Leanplum. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Leanplum.h"

@interface EmbeddedHTMLUrlCallback : NSObject

- (id)initWithCallback:(LeanplumEmbeddedHTMLUrlCallbackBlock)callback;
- (BOOL)onEmbeddedUrl:(NSString *)url;

@end
