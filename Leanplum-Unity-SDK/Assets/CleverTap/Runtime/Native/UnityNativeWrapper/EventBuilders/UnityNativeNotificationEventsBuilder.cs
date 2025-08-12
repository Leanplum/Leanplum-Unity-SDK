#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CleverTapSDK.Native
{
    internal class UnityNativeNotificationEventsBuilder
    {
        internal UnityNativeNotificationEventsBuilder(UnityNativeEventValidator eventValidator) { }

        internal UnityNativeEventBuilderResult<Dictionary<string, object>> BuildNotificationClickedEvent(Dictionary<string, object> properties)
        {
            return BuildEvent(UnityNativeConstants.Event.CLTAP_NOTIFICATION_CLICKED_EVENT_NAME, properties);
        }

        internal UnityNativeEventBuilderResult<Dictionary<string, object>> BuildNotificationViewedEvent(Dictionary<string, object> properties)
        {
            return BuildEvent(UnityNativeConstants.Event.CLTAP_NOTIFICATION_VIEWED_EVENT_NAME, properties);
        }

        private UnityNativeEventBuilderResult<Dictionary<string, object>> BuildEvent(string eventName, Dictionary<string, object> properties)
        {
            List<UnityNativeValidationResult> eventValidationResultsWithErrors = new List<UnityNativeValidationResult>();

            Dictionary<string, object> eventDetails = new Dictionary<string, object>();
            Dictionary<string, object> notif = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> item in properties)
            {
                if (!item.Key.StartsWith("wzrk_"))
                {
                    continue;
                }

                notif[item.Key] = item.Value;
            }

            if (notif.Count == 0)
            {
                eventValidationResultsWithErrors.Add(new UnityNativeValidationResult(512, "Notification data does not have any wzrk_* fields."));
                return new UnityNativeEventBuilderResult<Dictionary<string, object>>(eventValidationResultsWithErrors, null);
            }

            eventDetails[UnityNativeConstants.Event.EVENT_NAME] = eventName;
            eventDetails[UnityNativeConstants.Event.EVENT_DATA] = notif;

            return new UnityNativeEventBuilderResult<Dictionary<string, object>>(eventValidationResultsWithErrors, eventDetails);
        }
    }
}
#endif