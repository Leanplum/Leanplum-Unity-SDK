//
//  LeanplumActionContext.h
//  Unity-iPhone
//
//  Created by Nikola Zagorchev on 21.01.21.
//

#ifndef LeanplumActionContextBridge_h
#define LeanplumActionContextBridge_h

#pragma once

@interface LeanplumActionContextBridge : NSObject
+ (NSMutableDictionary<NSString *, LPActionContext *> *) sharedActionContexts;
@end
#endif /* LeanplumActionContextBridge_h */
