using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using CleverTap;
using CleverTap.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CleverTapUnity: MonoBehaviour {

    public String CLEVERTAP_ACCOUNT_ID = "YOUR_CLEVERTAP_ACCOUNT_ID";
    public String CLEVERTAP_ACCOUNT_TOKEN = "YOUR_CLEVERTAP_ACCOUNT_TOKEN";
    public String CLEVERTAP_ACCOUNT_REGION = "";
    public int CLEVERTAP_DEBUG_LEVEL = 0;
    public bool CLEVERTAP_ENABLE_PERSONALIZATION = true;
    public bool CLEVERTAP_DISABLE_IDFV;

    void Awake()
    {
        // DO NOT INITIALIZE CLEVERTAP HERE WHILE IN MIGRATION
        DontDestroyOnLoad(gameObject);
    }

    void Start() {}

    // handle deep link url
    void CleverTapDeepLinkCallback(string url) {
        Debug.Log("unity received deep link: " + (!String.IsNullOrEmpty(url) ? url : "NULL"));
    }

    // called when then the CleverTap user profile is initialized
    // returns {"CleverTapID":<CleverTap unique user id>}
    void CleverTapProfileInitializedCallback(string message) {
        Debug.Log("unity received profile initialized: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));

        if (String.IsNullOrEmpty(message)) {
            return;
        }

        try {
            JSONClass json = (JSONClass)JSON.Parse(message);
            Debug.Log(String.Format("unity parsed profile initialized {0}", json));
        } catch {
            Debug.LogError("unable to parse json");
        }
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
        Debug.Log("unity received profile updates: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));

        if (String.IsNullOrEmpty(message)) {
            return;
        }

        try {
            JSONClass json = (JSONClass)JSON.Parse(message);
            Debug.Log(String.Format("unity parsed profile updates {0}", json));
        } catch {
            Debug.LogError("unable to parse json");
        }
    }

    // returns the data associated with the push notification
    void CleverTapPushOpenedCallback(string message) {
        Debug.Log("unity received push opened: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));

        if (String.IsNullOrEmpty(message)) {
            return;
        }

        try {
            JSONClass json = (JSONClass)JSON.Parse(message);
            Debug.Log(String.Format("push notification data is {0}", json));
        } catch {
            Debug.LogError("unable to parse json");
        }
    }

    // returns a unique CleverTap identifier suitable for use with install attribution providers.
    void CleverTapInitCleverTapIdCallback(string message) {
        Debug.Log("unity received clevertap id: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
    }

    // returns the custom data associated with an in-app notification click
    void CleverTapInAppNotificationDismissedCallback(string message) {
        Debug.Log("unity received inapp notification dismissed: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
    }

    // returns the custom data associated with an in-app notification click
    void CleverTapInAppNotificationShowCallback(string message) {
        Debug.Log("unity received inapp notification onShow(): " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
    }

    // returns the status of push permission response after it's granted/denied
    void CleverTapOnPushPermissionResponseCallback(string message) {
         //Ensure to create call the `CreateNotificationChannel` once notification permission is granted to register for receiving push notifications for Android 13+ devices.
        Debug.Log("unity received push permission response: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
    }

    // returns when an in-app notification is dismissed by a call to action with custom extras
    void CleverTapInAppNotificationButtonTapped(string message) {
        Debug.Log("unity received inapp notification button tapped: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
    }

    // returns callback for InitializeInbox
    void CleverTapInboxDidInitializeCallback(){
        Debug.Log("unity received inbox initialized");
    }

    void CleverTapInboxMessagesDidUpdateCallback(){
        Debug.Log("unity received inbox messages updated");
    }

    // returns on the click of app inbox message with a map of custom Key-Value pairs
    void CleverTapInboxCustomExtrasButtonSelect(string message) {
        Debug.Log("unity received inbox message button with custom extras select: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
    }

    // returns on the click of app inbox message with a string of the inbox payload along with page index and button index
    void CleverTapInboxItemClicked(string message)
    {
        Debug.Log("unity received inbox message clicked callback: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
    }

    // returns native display units data
    void CleverTapNativeDisplayUnitsUpdated(string message) {
        Debug.Log("unity received native display units updated: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
    }

    // invoked when Product Experiences - Product Config are fetched 
    void CleverTapProductConfigFetched(string message) {
        Debug.Log("unity received product config fetched: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
    }

    // invoked when Product Experiences - Product Config are activated
    void CleverTapProductConfigActivated(string message) {
        Debug.Log("unity received product config activated: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
    }

    // invoked when Product Experiences - Product Config are initialized
    void CleverTapProductConfigInitialized(string message) {
        Debug.Log("unity received product config initialized: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
    }

    // invoked when Product Experiences - Feature Flags are updated 
    void CleverTapFeatureFlagsUpdated(string message) {
        Debug.Log("unity received feature flags updated: " + (!String.IsNullOrEmpty(message) ? message : "NULL"));
    }

    #if UNITY_EDITOR
    private void OnValidate() {
        EditorPrefs.SetBool("CLEVERTAP_DISABLE_IDFV", CLEVERTAP_DISABLE_IDFV);
    }
    #endif
}
