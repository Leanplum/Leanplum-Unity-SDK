using UnityEngine;
using System;
using System.Collections.Generic;
using CleverTapSDK.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CleverTapSDK {
    [Obsolete]
    public class CleverTapUnity : MonoBehaviour {

        public String CLEVERTAP_ACCOUNT_ID = "YOUR_CLEVERTAP_ACCOUNT_ID";
        public String CLEVERTAP_ACCOUNT_TOKEN = "YOUR_CLEVERTAP_ACCOUNT_TOKEN";
        public String CLEVERTAP_ACCOUNT_REGION = "";
        public int CLEVERTAP_DEBUG_LEVEL = 0;
        public bool CLEVERTAP_ENABLE_PERSONALIZATION = true;
        public bool CLEVERTAP_DISABLE_IDFV;

        void Awake() {
            DontDestroyOnLoad(gameObject);

            //Disable CleverTap unity logger
            //CleverTap.SetLogLevel(LogLevel.None);

            CleverTap.SetDebugLevel(CLEVERTAP_DEBUG_LEVEL);

            CleverTap.OnCleverTapDeepLinkCallback += CleverTapDeepLinkCallback;
            CleverTap.OnCleverTapProfileInitializedCallback += CleverTapProfileInitializedCallback;
            CleverTap.OnCleverTapProfileUpdatesCallback += CleverTapProfileUpdatesCallback;
            CleverTap.OnCleverTapPushOpenedCallback += CleverTapPushOpenedCallback;
            CleverTap.OnCleverTapInitCleverTapIdCallback += CleverTapInitCleverTapIdCallback;
            CleverTap.OnCleverTapInAppNotificationDismissedCallback += CleverTapInAppNotificationDismissedCallback;
            CleverTap.OnCleverTapInAppNotificationShowCallback += CleverTapInAppNotificationShowCallback;
            CleverTap.OnCleverTapOnPushPermissionResponseCallback += CleverTapOnPushPermissionResponseCallback;
            CleverTap.OnCleverTapInAppNotificationButtonTapped += CleverTapInAppNotificationButtonTapped;
            CleverTap.OnCleverTapInboxDidInitializeCallback += CleverTapInboxDidInitializeCallback;
            CleverTap.OnCleverTapInboxMessagesDidUpdateCallback += CleverTapInboxMessagesDidUpdateCallback;
            CleverTap.OnCleverTapInboxCustomExtrasButtonSelect += CleverTapInboxCustomExtrasButtonSelect;
            CleverTap.OnCleverTapInboxItemClicked += CleverTapInboxItemClicked;
            CleverTap.OnCleverTapNativeDisplayUnitsUpdated += CleverTapNativeDisplayUnitsUpdated;
            CleverTap.OnCleverTapProductConfigFetched += CleverTapProductConfigFetched;
            CleverTap.OnCleverTapProductConfigActivated += CleverTapProductConfigActivated;
            CleverTap.OnCleverTapProductConfigInitialized += CleverTapProductConfigInitialized;
            CleverTap.OnCleverTapFeatureFlagsUpdated += CleverTapFeatureFlagsUpdated;

            CleverTap.LaunchWithCredentialsForRegion(CLEVERTAP_ACCOUNT_ID, CLEVERTAP_ACCOUNT_TOKEN, CLEVERTAP_ACCOUNT_REGION);

            if (CLEVERTAP_ENABLE_PERSONALIZATION) {
                CleverTap.EnablePersonalization();
            }

            //==========[Testing Newly added Clevertap APIs]============================================
            //CleverTap.GetCleverTapID();

            //CleverTap.ProfileIncrementValueForKey("add_int", 2);
            //CleverTap.ProfileIncrementValueForKey("add_double", 3.5);
            //CleverTap.ProfileDecrementValueForKey("minus_int", 2);
            //CleverTap.ProfileDecrementValueForKey("minus_double", 3.5);
            //CleverTap.SuspendInAppNotifications();
            //CleverTap.DiscardInAppNotifications();
            //CleverTap.ResumeInAppNotifications();

            //record special Charged event
            //Dictionary<string, object> chargeDetails = new Dictionary<string, object>();
            //chargeDetails.Add("Amount", 500);
            //chargeDetails.Add("Currency", "USD");
            //chargeDetails.Add("Payment Mode", "Credit card");

            //Dictionary<string, object> item = new Dictionary<string, object>();
            //item.Add("price", 50);
            //item.Add("Product category", "books");
            //item.Add("Quantity", 1);

            //Dictionary<string, object> item2 = new Dictionary<string, object>();
            //item2.Add("price", 100);
            //item2.Add("Product category", "plants");
            //item2.Add("Quantity", 10);

            //List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
            //items.Add(item);
            //items.Add(item2);

            //CleverTap.RecordChargedEventWithDetailsAndItems(chargeDetails, items);

            //CleverTap.RecordEvent("testEventPushAmp");
            //Push Primer APIs usages

            //bool isPushPermissionGranted = CleverTap.IsPushPermissionGranted();
            //Debug.Log("isPushPermissionGranted" + isPushPermissionGranted);

            //Dictionary<string, object> item = new Dictionary<string, object>();
            //item.Add("inAppType", "half-interstitial");
            //item.Add("titleText", "Get Notified");
            //item.Add("messageText", "Please enable notifications on your device to use Push Notifications.");
            //item.Add("followDeviceOrientation", true);
            //item.Add("positiveBtnText", "Allow");
            //item.Add("negativeBtnText", "Cancel");
            //item.Add("backgroundColor", "#FFFFFF");
            //item.Add("btnBorderColor", "#0000FF");
            //item.Add("titleTextColor", "#0000FF");
            //item.Add("messageTextColor", "#000000");
            //item.Add("btnTextColor", "#FFFFFF");
            //item.Add("btnBackgroundColor", "#0000FF");
            //item.Add("imageUrl", "https://icons.iconarchive.com/icons/treetog/junior/64/camera-icon.png");
            //item.Add("btnBorderRadius", "2");
            //item.Add("fallbackToSettings", true);
            //CleverTap.PromptPushPrimer(item);

            //CleverTap.PromptForPushPermission(false);

            //Push Primer APIs usages

            //bool isPushPermissionGranted = CleverTap.IsPushPermissionGranted();
            //Debug.Log("isPushPermissionGranted" + isPushPermissionGranted);

            //Dictionary<string, object> item = new Dictionary<string, object>();
            //item.Add("inAppType", "half-interstitial");
            //item.Add("titleText", "Get Notified");
            //item.Add("messageText", "Please enable notifications on your device to use Push Notifications.");
            //item.Add("followDeviceOrientation", true);
            //item.Add("positiveBtnText", "Allow");
            //item.Add("negativeBtnText", "Cancel");
            //item.Add("backgroundColor", "#FFFFFF");
            //item.Add("btnBorderColor", "#0000FF");
            //item.Add("titleTextColor", "#0000FF");
            //item.Add("messageTextColor", "#000000");
            //item.Add("btnTextColor", "#FFFFFF");
            //item.Add("btnBackgroundColor", "#0000FF");
            //item.Add("imageUrl", "https://icons.iconarchive.com/icons/treetog/junior/64/camera-icon.png");
            //item.Add("btnBorderRadius", "2");
            //item.Add("fallbackToSettings", true);
            //CleverTap.PromptPushPrimer(item);

            //CleverTap.PromptForPushPermission(false);

            //Push Templates APIs usages
            //CleverTap.RecordEvent("Send Basic Push");
            //CleverTap.RecordEvent("Send Carousel Push");
            //CleverTap.RecordEvent("Send Manual Carousel Push");
            //CleverTap.RecordEvent("Send Filmstrip Carousel Push");
            //CleverTap.RecordEvent("Send Rating Push");
            //CleverTap.RecordEvent("Send Product Display Notification");
            //CleverTap.RecordEvent("Send Linear Product Display Push");
            //CleverTap.RecordEvent("Send CTA Notification");
            //CleverTap.RecordEvent("Send Zero Bezel Notification");
            //CleverTap.RecordEvent("Send Zero Bezel Text Only Notification");
            //CleverTap.RecordEvent("Send Timer Notification");
            //CleverTap.RecordEvent("Send Input Box Notification");
            //CleverTap.RecordEvent("Send Input Box Reply with Event Notification");
            //CleverTap.RecordEvent("Send Input Box Reply with Auto Open Notification");
            //CleverTap.RecordEvent("Send Input Box Remind Notification DOC FALSE");
            //CleverTap.RecordEvent("Send Input Box CTA DOC true");
            //CleverTap.RecordEvent("Send Input Box CTA DOC false");
            //CleverTap.RecordEvent("Send Input Box Reminder DOC true");
            //CleverTap.RecordEvent("Send Input Box Reminder DOC false");
        }

        void Start() { }

        // handle deep link url
        void CleverTapDeepLinkCallback(string url) {
        }

        // called when then the CleverTap user profile is initialized
        // returns {"CleverTapID":<CleverTap unique user id>}
        void CleverTapProfileInitializedCallback(string message) {
        }

        // called when the user profile is updated as a result of a server sync
        /**
            returns dict in the form:
            {
                "profile":{"<property1>":{"oldValue":<value>, "newValue":<value>}, ...},
                "events:{"<eventName>":
                            {"count":
                                {"oldValue":(int)<old count>, "newValue":<new count>},
                            "firstTime":
                                {"oldValue":(double)<old first time event occurred>, "newValue":<new first time event occurred>},
                            "lastTime":
                                {"oldValue":(double)<old last time event occurred>, "newValue":<new last time event occurred>},
                        }, ...
                    }
            }
        */
        void CleverTapProfileUpdatesCallback(string message) {
        }

        // returns the data associated with the push notification
        void CleverTapPushOpenedCallback(string message) {
        }

        // returns a unique CleverTap identifier suitable for use with install attribution providers.
        void CleverTapInitCleverTapIdCallback(string message) {
        }

        // returns the custom data associated with an in-app notification click
        void CleverTapInAppNotificationDismissedCallback(string message) {
        }

        // returns the custom data associated with an in-app notification click
        void CleverTapInAppNotificationShowCallback(string message) {
        }

        // returns the status of push permission response after it's granted/denied
        void CleverTapOnPushPermissionResponseCallback(string message) {
        }

        // returns when an in-app notification is dismissed by a call to action with custom extras
        void CleverTapInAppNotificationButtonTapped(string message) {
        }

        // returns callback for InitializeInbox
        void CleverTapInboxDidInitializeCallback() {
        }

        void CleverTapInboxMessagesDidUpdateCallback() {
        }

        // returns on the click of app inbox message with a map of custom Key-Value pairs
        void CleverTapInboxCustomExtrasButtonSelect(string message) {
        }

        // returns on the click of app inbox message with a string of the inbox payload along with page index and button index
        void CleverTapInboxItemClicked(string message) {
        }

        // returns native display units data
        void CleverTapNativeDisplayUnitsUpdated(string message) {
        }

        // invoked when Product Experiences - Product Config are fetched
        void CleverTapProductConfigFetched(string message) {
        }

        // invoked when Product Experiences - Product Config are activated
        void CleverTapProductConfigActivated(string message) {
        }

        // invoked when Product Experiences - Product Config are initialized
        void CleverTapProductConfigInitialized(string message) {
        }

        // invoked when Product Experiences - Feature Flags are updated
        void CleverTapFeatureFlagsUpdated(string message) {
        }

#if UNITY_EDITOR
        private void OnValidate() {
            EditorPrefs.SetBool("CLEVERTAP_DISABLE_IDFV", CLEVERTAP_DISABLE_IDFV);
        }
#endif
    }
}
