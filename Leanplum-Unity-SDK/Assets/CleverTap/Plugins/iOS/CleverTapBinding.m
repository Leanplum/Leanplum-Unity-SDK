#import "CleverTapUnityManager.h"
#import "CleverTapMessageSender.h"

#pragma mark - Utils

NSString* clevertap_stringToNSString(const char* str) {
    return str != NULL ? [NSString stringWithUTF8String:str] : [NSString stringWithUTF8String:""];
}

NSString* clevertap_toJsonString(id val) {
    NSString *jsonString;
    
    if (val == nil) {
        return nil;
    }
    
    if ([val isKindOfClass:[NSArray class]] || [val isKindOfClass:[NSDictionary class]]) {
        NSError *error;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:val options:NSJSONWritingPrettyPrinted error:&error];
        jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        
        if (error != nil) {
            jsonString = nil;
        }
    } else {
        jsonString = [NSString stringWithFormat:@"%@", val];
    }
    
    return jsonString;
}

NSMutableArray* clevertap_NSArrayFromArray(const char* array[], int size) {
    
    NSMutableArray *values = [NSMutableArray arrayWithCapacity:size];
    for (int i = 0; i < size; i ++) {
        NSString *value = clevertap_stringToNSString(array[i]);
        [values addObject:value];
    }
    
    return values;
}

NSMutableDictionary* clevertap_dictFromJsonString(const char* jsonString) {
    
    NSMutableDictionary *dict = [NSMutableDictionary dictionaryWithCapacity:1];
    
    if (jsonString != NULL && jsonString != nil) {
        NSError *jsonError;
        NSData *objectData = [clevertap_stringToNSString(jsonString) dataUsingEncoding:NSUTF8StringEncoding];
        dict = [NSJSONSerialization JSONObjectWithData:objectData
                                               options:NSJSONReadingMutableContainers
                                                 error:&jsonError];
    }
    
    return dict;
}

NSMutableArray* clevertap_NSArrayFromJsonString(const char* jsonString) {
    NSMutableArray *arr = [NSMutableArray arrayWithCapacity:1];
    
    if (jsonString != NULL && jsonString != nil) {
        NSError *jsonError;
        NSData *objectData = [clevertap_stringToNSString(jsonString) dataUsingEncoding:NSUTF8StringEncoding];
        arr = [NSJSONSerialization JSONObjectWithData:objectData
                                              options:NSJSONReadingMutableContainers
                                                error:&jsonError];
    }
    
    return arr;
}

__attribute__((deprecated("Used by deprecated methods only.")))
NSMutableDictionary* clevertap_eventDetailToDict(CleverTapEventDetail* detail) {
    
    NSMutableDictionary *_dict = [NSMutableDictionary new];
    
    if(detail) {
        if(detail.eventName) {
            [_dict setObject:detail.eventName forKey:@"eventName"];
        }
        
        if(detail.firstTime){
            [_dict setObject:@(detail.firstTime) forKey:@"firstTime"];
        }
        
        if(detail.lastTime){
            [_dict setObject:@(detail.lastTime) forKey:@"lastTime"];
        }
        
        if(detail.count){
            [_dict setObject:@(detail.count) forKey:@"count"];
        }
    }
    
    return _dict;
}

NSMutableDictionary* clevertap_utmDetailToDict(CleverTapUTMDetail* detail) {
    
    NSMutableDictionary *_dict = [NSMutableDictionary new];
    
    if(detail) {
        if(detail.source) {
            [_dict setObject:detail.source forKey:@"source"];
        }
        
        if(detail.medium) {
            [_dict setObject:detail.medium forKey:@"medium"];
        }
        
        if(detail.campaign) {
            [_dict setObject:detail.campaign forKey:@"campaign"];
        }
    }
    
    return _dict;
}

char* clevertap_cStringCopy(const char* string) {
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}


#pragma mark - Admin

void CleverTap_onPlatformInit(){
    [[CleverTapUnityManager sharedInstance] onPlatformInit];
}

void CleverTap_onCallbackAdded(const char* callbackName) {
    [[CleverTapUnityManager sharedInstance]
     onCallbackAdded:clevertap_stringToNSString(callbackName)];
}

void CleverTap_onVariablesCallbackAdded(const char* callbackName, int callbackId) {
    [[CleverTapUnityManager sharedInstance]
     onVariablesCallbackAdded:clevertap_stringToNSString(callbackName) callbackId:callbackId];
}

void CleverTap_setInAppNotificationButtonTappedCallback(InAppNotificationButtonTapped callback) {
    [[CleverTapMessageSender sharedInstance]
     setInAppNotificationButtonTappedCallback:callback];
}

void CleverTap_getUserEventLog(const char* eventName, const char* key, UserEventLogCallback callback) {
    [[CleverTapUnityManager sharedInstance]
     getUserEventLog:clevertap_stringToNSString(eventName) forKey:clevertap_stringToNSString(key) withCallback:callback];
}

void CleverTap_getUserAppLaunchCount(const char* key, UserEventLogCallback callback) {
    [[CleverTapUnityManager sharedInstance]
     getUserAppLaunchCount:clevertap_stringToNSString(key) withCallback:callback];
}

void CleverTap_getUserEventLogCount(const char* eventName, const char* key, UserEventLogCallback callback) {
    [[CleverTapUnityManager sharedInstance]
     getUserEventLogCount:clevertap_stringToNSString(eventName) forKey:clevertap_stringToNSString(key) withCallback:callback];
}

void CleverTap_getUserEventLogHistory(const char* key, UserEventLogCallback callback) {
    [[CleverTapUnityManager sharedInstance]
     getUserEventLogHistory:clevertap_stringToNSString(key) withCallback:callback];
}

int64_t CleverTap_getUserLastVisitTs() {
    return [[CleverTapUnityManager sharedInstance] getUserLastVisitTs];
}

void CleverTap_setDebugLevel(int level) {
    [CleverTapUnityManager setDebugLevel:level];
}

void CleverTap_enablePersonalization() {
    [CleverTapUnityManager enablePersonalization];
}

void CleverTap_disablePersonalization() {
    [CleverTapUnityManager disablePersonalization];
}

void CleverTap_setLocation(double lat, double lon) {
    CLLocationCoordinate2D coord = CLLocationCoordinate2DMake(lat, lon);
    [CleverTapUnityManager setLocation:coord];
}

void CleverTap_registerPush() {
    [CleverTapUnityManager registerPush];
}

void CleverTap_setApplicationIconBadgeNumber(int num) {
    [CleverTapUnityManager setApplicationIconBadgeNumber:num];
}


#pragma mark - Offline API

void CleverTap_setOffline(const BOOL enabled) {
    [[CleverTapUnityManager sharedInstance] setOffline:enabled];
}


#pragma mark - Opt-out API

void CleverTap_setOptOut(const BOOL enabled) {
    [[CleverTapUnityManager sharedInstance] setOptOut:enabled];
}

void CleverTap_enableDeviceNetworkInfoReporting(const BOOL enabled) {
    [[CleverTapUnityManager sharedInstance] enableDeviceNetworkInfoReporting:enabled];
}


#pragma mark - User Profile

void CleverTap_onUserLogin(const char* properties) {
    NSMutableDictionary *profileProperties = clevertap_dictFromJsonString(properties);
    [[CleverTapUnityManager sharedInstance] onUserLogin:profileProperties];
}

void CleverTap_profilePush(const char* properties) {
    NSMutableDictionary *profileProperties = clevertap_dictFromJsonString(properties);
    [[CleverTapUnityManager sharedInstance] profilePush:profileProperties];
}

char* CleverTap_profileGet(const char* key) {
    id ret = [[CleverTapUnityManager sharedInstance] profileGet:clevertap_stringToNSString(key)];
    
    NSString *jsonString = clevertap_toJsonString(ret);
    
    if (jsonString == nil) {
        return NULL;
    }
    
    return clevertap_cStringCopy([jsonString UTF8String]);
}

char* CleverTap_profileGetCleverTapID() {
    NSString *ret = [[CleverTapUnityManager sharedInstance] profileGetCleverTapID];
    
    if (ret == nil) {
        return NULL;
    }
    
    return clevertap_cStringCopy([ret UTF8String]);
}

char* CleverTap_getCleverTapID() {
    return CleverTap_profileGetCleverTapID();
}

char* CleverTap_profileGetCleverTapAttributionIdentifier() {
    NSString *ret = [[CleverTapUnityManager sharedInstance] profileGetCleverTapAttributionIdentifier];
    
    if (ret == nil) {
        return NULL;
    }
    
    return clevertap_cStringCopy([ret UTF8String]);
}

void CleverTap_profileRemoveValueForKey(const char* key) {
    [[CleverTapUnityManager sharedInstance] profileRemoveValueForKey:clevertap_stringToNSString(key)];
}

void CleverTap_profileSetMultiValuesForKey(const char* key, const char* array[], int size) {
    
    if (array == NULL || array == nil || size == 0) {
        return;
    }
    
    NSArray *values = clevertap_NSArrayFromArray(array, size);
    
    [[CleverTapUnityManager sharedInstance] profileSetMultiValues:values forKey:clevertap_stringToNSString(key)];
}

void CleverTap_profileAddMultiValuesForKey(const char* key, const char* array[], int size) {
    
    if (array == NULL || array == nil || size == 0) {
        return;
    }
    
    NSArray *values = clevertap_NSArrayFromArray(array, size);
    
    [[CleverTapUnityManager sharedInstance] profileAddMultiValues:values forKey:clevertap_stringToNSString(key)];
    
}

void CleverTap_profileRemoveMultiValuesForKey(const char* key, const char* array[], int size) {
    
    if (array == NULL || array == nil || size == 0) {
        return;
    }
    
    NSArray *values = clevertap_NSArrayFromArray(array, size);
    
    [[CleverTapUnityManager sharedInstance] profileRemoveMultiValues:values forKey:clevertap_stringToNSString(key)];
}

void CleverTap_profileAddMultiValueForKey(const char* key, const char* value) {
    [[CleverTapUnityManager sharedInstance] profileAddMultiValue:clevertap_stringToNSString(value) forKey:clevertap_stringToNSString(key)];
}

void CleverTap_profileRemoveMultiValueForKey(const char* key, const char* value) {
    [[CleverTapUnityManager sharedInstance] profileRemoveMultiValue:clevertap_stringToNSString(value) forKey:clevertap_stringToNSString(key)];
}

void CleverTap_profileIncrementDoubleValueForKey(const char* key, const double value) {
    [[CleverTapUnityManager sharedInstance] profileIncrementValueBy:[NSNumber numberWithDouble: value] forKey:clevertap_stringToNSString(key)];
}

void CleverTap_profileIncrementIntValueForKey(const char* key, const int value) {
    [[CleverTapUnityManager sharedInstance] profileIncrementValueBy:[NSNumber numberWithInt: value] forKey:clevertap_stringToNSString(key)];
}

void CleverTap_profileDecrementDoubleValueForKey(const char* key, const double value) {
    [[CleverTapUnityManager sharedInstance] profileDecrementValueBy:[NSNumber numberWithDouble: value] forKey:clevertap_stringToNSString(key)];
}

void CleverTap_profileDecrementIntValueForKey(const char* key, const int value) {
    [[CleverTapUnityManager sharedInstance] profileDecrementValueBy:[NSNumber numberWithInt: value] forKey:clevertap_stringToNSString(key)];
}


#pragma mark - User Action Events

void CleverTap_recordScreenView(const char* screenName) {
    [[CleverTapUnityManager sharedInstance] recordScreenView:clevertap_stringToNSString(screenName)];
}

void CleverTap_recordEvent(const char* eventName, const char* properties) {
    NSMutableDictionary *eventProperties = clevertap_dictFromJsonString(properties);
    if (eventProperties == nil || eventProperties == NULL) {
        [[CleverTapUnityManager sharedInstance] recordEvent:clevertap_stringToNSString(eventName)];
    }else {
        [[CleverTapUnityManager sharedInstance] recordEvent:clevertap_stringToNSString(eventName) withProps:eventProperties];
    }
}

void CleverTap_recordChargedEventWithDetailsAndItems(const char* chargeDetails, const char* items) {
    NSDictionary *details = clevertap_dictFromJsonString(chargeDetails);
    NSArray *_items = clevertap_NSArrayFromJsonString(items);
    [[CleverTapUnityManager sharedInstance] recordChargedEventWithDetails:details andItems:_items];
}

__attribute__((deprecated("Deprecated, use getUserEventLog instead")))
int CleverTap_eventGetFirstTime(const char* eventName) {
    return [[CleverTapUnityManager sharedInstance] eventGetFirstTime:clevertap_stringToNSString(eventName)];
}

__attribute__((deprecated("Deprecated, use getUserEventLog instead")))
int CleverTap_eventGetLastTime(const char* eventName) {
    return [[CleverTapUnityManager sharedInstance] eventGetLastTime:clevertap_stringToNSString(eventName)];
}

__attribute__((deprecated("Deprecated, use getUserEventLogCount instead")))
int CleverTap_eventGetOccurrences(const char* eventName) {
    return [[CleverTapUnityManager sharedInstance] eventGetOccurrences:clevertap_stringToNSString(eventName)];
}

__attribute__((deprecated("Deprecated, use getUserEventLogHistory instead")))
char* CleverTap_userGetEventHistory() {
    NSDictionary *history = [[CleverTapUnityManager sharedInstance] userGetEventHistory];
    
    NSMutableDictionary *_history = [NSMutableDictionary new];
    
    for (NSString *key in history.allKeys) {
        _history[key] = clevertap_eventDetailToDict(history[key]);
    }
    
    NSString *jsonString = clevertap_toJsonString(_history);
    
    if (jsonString == nil) {
        return NULL;
    }
    
    return clevertap_cStringCopy([jsonString UTF8String]);
}

#pragma mark - User Session

char* CleverTap_sessionGetUTMDetails() {
    CleverTapUTMDetail *detail = [[CleverTapUnityManager sharedInstance] sessionGetUTMDetails];
    
    NSMutableDictionary *_detail = clevertap_utmDetailToDict(detail);
    
    NSString *jsonString = clevertap_toJsonString(_detail);
    
    if (jsonString == nil) {
        return NULL;
    }
    
    return clevertap_cStringCopy([jsonString UTF8String]);
}

int CleverTap_sessionGetTimeElapsed() {
    return [[CleverTapUnityManager sharedInstance] sessionGetTimeElapsed];
}

__attribute__((deprecated("Deprecated, use getUserEventLog instead")))
char* CleverTap_eventGetDetail(const char* eventName) {
    CleverTapEventDetail *detail = [[CleverTapUnityManager sharedInstance] eventGetDetail:clevertap_stringToNSString(eventName)];
    
    NSMutableDictionary *_detail = clevertap_eventDetailToDict(detail);
    
    NSString *jsonString = clevertap_toJsonString(_detail);
    
    if (jsonString == nil) {
        return NULL;
    }
    
    return clevertap_cStringCopy([jsonString UTF8String]);
}

__attribute__((deprecated("Deprecated, use getUserAppLaunchCount instead")))
int CleverTap_userGetTotalVisits() {
    return [[CleverTapUnityManager sharedInstance] userGetTotalVisits];
}

int CleverTap_userGetScreenCount() {
    return [[CleverTapUnityManager sharedInstance] userGetScreenCount];
}

__attribute__((deprecated("Deprecated, use getUserLastVisitTs instead")))
int CleverTap_userGetPreviousVisitTime() {
    return [[CleverTapUnityManager sharedInstance] userGetPreviousVisitTime];
}

#pragma mark - Push Notifications

void CleverTap_pushInstallReferrerSource(const char* source, const char* medium, const char* campaign) {
    [[CleverTapUnityManager sharedInstance] pushInstallReferrerSource:clevertap_stringToNSString(source) medium:clevertap_stringToNSString(medium) campaign:clevertap_stringToNSString(campaign)];
}


#pragma mark - App Inbox

int CleverTap_getInboxMessageUnreadCount() {
    return [[CleverTapUnityManager sharedInstance] getInboxMessageUnreadCount];
}

int CleverTap_getInboxMessageCount() {
    return [[CleverTapUnityManager sharedInstance] getInboxMessageCount];
}

void CleverTap_initializeInbox() {
    [[CleverTapUnityManager sharedInstance] initializeInbox];
}

void CleverTap_showAppInbox(const char* styleConfig) {
    NSMutableDictionary *styleConfigDict = clevertap_dictFromJsonString(styleConfig);
    [[CleverTapUnityManager sharedInstance] showAppInbox: styleConfigDict];
}

void CleverTap_dismissAppInbox() {
    [[CleverTapUnityManager sharedInstance] dismissAppInbox];
}

char* CleverTap_getAllInboxMessages() {
    id ret = [[CleverTapUnityManager sharedInstance] getAllInboxMessages];
    NSString *jsonString = clevertap_toJsonString(ret);
    if (jsonString == nil) {
        return NULL;
    }
    return clevertap_cStringCopy([jsonString UTF8String]);
}

char* CleverTap_getUnreadInboxMessages() {
    id ret = [[CleverTapUnityManager sharedInstance] getUnreadInboxMessages];
    NSString *jsonString = clevertap_toJsonString(ret);
    if (jsonString == nil) {
        return NULL;
    }
    return clevertap_cStringCopy([jsonString UTF8String]);
}

char* CleverTap_getInboxMessageForId(const char* messageId) {
    id ret = [[CleverTapUnityManager sharedInstance] getInboxMessageForId:clevertap_stringToNSString(messageId)];
    NSString *jsonString = clevertap_toJsonString(ret);
    if (jsonString == nil) {
        return clevertap_cStringCopy([@"{}" UTF8String]);
    }
    return clevertap_cStringCopy([jsonString UTF8String]);
}

void CleverTap_deleteInboxMessageForID(const char* messageId) {
    [[CleverTapUnityManager sharedInstance] deleteInboxMessageForID:clevertap_stringToNSString(messageId)];
}

void CleverTap_deleteInboxMessagesForIDs(const char* messageIds[], int size) {
    [[CleverTapUnityManager sharedInstance] deleteInboxMessagesForIDs:clevertap_NSArrayFromArray(messageIds, size)];
}

void CleverTap_markReadInboxMessageForID(const char* messageId) {
    [[CleverTapUnityManager sharedInstance] markReadInboxMessageForID:clevertap_stringToNSString(messageId)];
}

void CleverTap_markReadInboxMessagesForIDs(const char* messageIds[], int size) {
    [[CleverTapUnityManager sharedInstance] markReadInboxMessagesForIDs:clevertap_NSArrayFromArray(messageIds, size)];
}

void CleverTap_recordInboxNotificationViewedEventForID(const char* messageId) {
    [[CleverTapUnityManager sharedInstance] recordInboxNotificationViewedEventForID:clevertap_stringToNSString(messageId)];
}

void CleverTap_recordInboxNotificationClickedEventForID(const char* messageId) {
    [[CleverTapUnityManager sharedInstance] recordInboxNotificationClickedEventForID:clevertap_stringToNSString(messageId)];
}


#pragma mark - Native Display

char* CleverTap_getAllDisplayUnits() {
 
    id ret = [[CleverTapUnityManager sharedInstance] getAllDisplayUnits];
    NSString *jsonString = clevertap_toJsonString(ret);
    if (jsonString == nil) {
        return NULL;
    }
    return clevertap_cStringCopy([jsonString UTF8String]);
}

char* CleverTap_getDisplayUnitForID(const char* unitID) {
    id ret = [[CleverTapUnityManager sharedInstance] getDisplayUnitForID:clevertap_stringToNSString(unitID)];
    NSString *jsonString = clevertap_toJsonString(ret);
    if (jsonString == nil) {
        return NULL;
    }
    return clevertap_cStringCopy([jsonString UTF8String]);
}

void CleverTap_recordDisplayUnitViewedEventForID(const char* unitID) {
    [[CleverTapUnityManager sharedInstance] recordDisplayUnitViewedEventForID:clevertap_stringToNSString(unitID)];
}

void CleverTap_recordDisplayUnitClickedEventForID(const char* unitID) {
    [[CleverTapUnityManager sharedInstance] recordDisplayUnitClickedEventForID:clevertap_stringToNSString(unitID)];
}


#pragma mark - Product Config

void CleverTap_fetchProductConfig() {
    [[CleverTapUnityManager sharedInstance] fetchProductConfig];
}

void CleverTap_fetchProductConfigWithMinimumInterval(const double minimumInterval) {
    [[CleverTapUnityManager sharedInstance] fetchProductConfigWithMinimumInterval:minimumInterval];
}

void CleverTap_setProductConfigMinimumFetchInterval(const double minimumFetchInterval) {
    [[CleverTapUnityManager sharedInstance] setProductConfigMinimumFetchInterval:minimumFetchInterval];
}

void CleverTap_activateProductConfig() {
    [[CleverTapUnityManager sharedInstance] activateProductConfig];
}

void CleverTap_fetchAndActivateProductConfig() {
    [[CleverTapUnityManager sharedInstance] fetchAndActivateProductConfig];
}

void CleverTap_setProductConfigDefaults(const char* defaults) {
    NSMutableDictionary *defaultsDict = clevertap_dictFromJsonString(defaults);
    [[CleverTapUnityManager sharedInstance] setProductConfigDefaults:defaultsDict];
}

void CleverTap_setProductConfigDefaultsFromPlistFileName(const char* fileName) {
    [[CleverTapUnityManager sharedInstance] setProductConfigDefaultsFromPlistFileName:clevertap_stringToNSString(fileName)];
}

char* CleverTap_getProductConfigValueFor(const char* key) {
    id ret = [[CleverTapUnityManager sharedInstance] getProductConfigValueFor:clevertap_stringToNSString(key)];
    NSString *jsonString = clevertap_toJsonString(ret);
    if (jsonString == nil) {
       return NULL;
   }
   return clevertap_cStringCopy([jsonString UTF8String]);
}

double CleverTap_getProductConfigLastFetchTimeStamp() {
    return [[CleverTapUnityManager sharedInstance] getProductConfigLastFetchTimeStamp];
}

void CleverTap_resetProductConfig() {
    [[CleverTapUnityManager sharedInstance] resetProductConfig];
}


#pragma mark - Feature Flags

BOOL CleverTap_getFeatureFlag(const char* key, const BOOL defaultValue) {
    return [[CleverTapUnityManager sharedInstance] get:clevertap_stringToNSString(key) withDefaultValue:defaultValue];
}


#pragma mark - In-App Controls

void CleverTap_suspendInAppNotifications() {
    [[CleverTapUnityManager sharedInstance] suspendInAppNotifications];
}

void CleverTap_discardInAppNotifications() {
    [[CleverTapUnityManager sharedInstance] discardInAppNotifications];
}

void CleverTap_resumeInAppNotifications() {
    [[CleverTapUnityManager sharedInstance] resumeInAppNotifications];
}

#pragma mark - Push Primer
void CleverTap_promptPushPrimer(const char* json) {
    NSMutableDictionary *jsonDict = clevertap_dictFromJsonString(json);
    [[CleverTapUnityManager sharedInstance] promptPushPrimer: jsonDict];
}

void CleverTap_promptForPushPermission(const BOOL showFallbackSettings) {
    [[CleverTapUnityManager sharedInstance] promptForPushPermission: showFallbackSettings];
}

void CleverTap_isPushPermissionGranted() {
    return [[CleverTapUnityManager sharedInstance] isPushPermissionGranted];
}

#pragma mark - Variables

void CleverTap_defineVar(const char* name, const char* kind, const char* value) {
    [[CleverTapUnityManager sharedInstance] defineVar:clevertap_stringToNSString(name)
                                                 kind:clevertap_stringToNSString(kind)
                                      andDefaultValue:clevertap_stringToNSString(value)];
}

void CleverTap_defineFileVar(const char* name) {
    [[CleverTapUnityManager sharedInstance] defineFileVar:clevertap_stringToNSString(name)];
}

char* CleverTap_getVariableValue(const char* name) {
    NSString* json = [[CleverTapUnityManager sharedInstance]
                      getVariableValue:clevertap_stringToNSString(name)];
    return clevertap_cStringCopy([json UTF8String]);
}

char* CleverTap_getFileVariableValue(const char* name) {
    NSString* json = [[CleverTapUnityManager sharedInstance]
                      getFileVariableValue:clevertap_stringToNSString(name)];
    return clevertap_cStringCopy([json UTF8String]);
}

void CleverTap_syncVariables() {
    [[CleverTapUnityManager sharedInstance] syncVariables];
}

void CleverTap_syncVariablesProduction(const BOOL isProduction) {
    [[CleverTapUnityManager sharedInstance] syncVariables: isProduction];
}

void CleverTap_fetchVariables(int callbackId) {
    [[CleverTapUnityManager sharedInstance] fetchVariables:callbackId];
}

#pragma mark - Client-side In-Apps

void CleverTap_fetchInApps(int callbackId) {
    [[CleverTapUnityManager sharedInstance] fetchInApps:callbackId];
}

void CleverTap_clearInAppResources(const BOOL expiredOnly) {
    [[CleverTapUnityManager sharedInstance] clearInAppResources:expiredOnly];
}

#pragma mark - Custom Templates

void CleverTap_customTemplateSetPresented(const char* name) {
    [[CleverTapUnityManager sharedInstance]
                      customTemplateSetPresented:clevertap_stringToNSString(name)];
}

void CleverTap_customTemplateSetDismissed(const char* name) {
    [[CleverTapUnityManager sharedInstance]
                      customTemplateSetDismissed:clevertap_stringToNSString(name)];
}

char* CleverTap_customTemplateContextToString(const char* name) {
    NSString* value = [[CleverTapUnityManager sharedInstance]
                       customTemplateContextToString:clevertap_stringToNSString(name)];
    return clevertap_cStringCopy([value UTF8String]);
}

void CleverTap_customTemplateTriggerAction(const char* templateName, const char* argumentName) {
    [[CleverTapUnityManager sharedInstance]
     customTemplateTriggerAction:clevertap_stringToNSString(templateName) named:clevertap_stringToNSString(argumentName)];
}

char* CleverTap_customTemplateGetStringArg(const char* templateName, const char* argumentName) {
    NSString* value = [[CleverTapUnityManager sharedInstance]
                      customTemplateGetStringArg:clevertap_stringToNSString(templateName) named:clevertap_stringToNSString(argumentName)];
    return clevertap_cStringCopy([value UTF8String]);
}

bool CleverTap_customTemplateGetBooleanArg(const char* templateName, const char* argumentName) {
    return [[CleverTapUnityManager sharedInstance]
                       customTemplateGetBooleanArg:clevertap_stringToNSString(templateName) named:clevertap_stringToNSString(argumentName)];
}

char* CleverTap_customTemplateGetFileArg(const char* templateName, const char* argumentName) {
    NSString* value = [[CleverTapUnityManager sharedInstance]
                       customTemplateGetFileArg:clevertap_stringToNSString(templateName) named:clevertap_stringToNSString(argumentName)];
    return clevertap_cStringCopy([value UTF8String]);
}

char* CleverTap_customTemplateGetDictionaryArg(const char* templateName, const char* argumentName) {
    NSDictionary* json = [[CleverTapUnityManager sharedInstance]
                       customTemplateGetDictionaryArg:clevertap_stringToNSString(templateName) named:clevertap_stringToNSString(argumentName)];
    NSString *jsonString = clevertap_toJsonString(json);
    if (jsonString == nil) {
        return NULL;
    }
    
    return clevertap_cStringCopy([jsonString UTF8String]);
}

int CleverTap_customTemplateGetIntArg(const char* templateName, const char* argumentName) {
    return [[CleverTapUnityManager sharedInstance]
            customTemplateGetIntArg:clevertap_stringToNSString(templateName) named:clevertap_stringToNSString(argumentName)];
}

double CleverTap_customTemplateGetDoubleArg(const char* templateName, const char* argumentName) {
    return [[CleverTapUnityManager sharedInstance]
            customTemplateGetDoubleArg:clevertap_stringToNSString(templateName) named:clevertap_stringToNSString(argumentName)];
}

float CleverTap_customTemplateGetFloatArg(const char* templateName, const char* argumentName) {
    return [[CleverTapUnityManager sharedInstance]
            customTemplateGetFloatArg:clevertap_stringToNSString(templateName) named:clevertap_stringToNSString(argumentName)];
}

int64_t CleverTap_customTemplateGetLongArg(const char* templateName, const char* argumentName) {
    return [[CleverTapUnityManager sharedInstance]
            customTemplateGetLongArg:clevertap_stringToNSString(templateName) named:clevertap_stringToNSString(argumentName)];
}

int16_t CleverTap_customTemplateGetShortArg(const char* templateName, const char* argumentName) {
    return [[CleverTapUnityManager sharedInstance]
            customTemplateGetShortArg:clevertap_stringToNSString(templateName) named:clevertap_stringToNSString(argumentName)];
}

int8_t CleverTap_customTemplateGetByteArg(const char* templateName, const char* argumentName) {
    return [[CleverTapUnityManager sharedInstance]
            customTemplateGetByteArg:clevertap_stringToNSString(templateName) named:clevertap_stringToNSString(argumentName)];
}

void CleverTap_syncCustomTemplates(bool isProduction) {
    return [[CleverTapUnityManager sharedInstance]
            syncCustomTemplates:isProduction];
}
