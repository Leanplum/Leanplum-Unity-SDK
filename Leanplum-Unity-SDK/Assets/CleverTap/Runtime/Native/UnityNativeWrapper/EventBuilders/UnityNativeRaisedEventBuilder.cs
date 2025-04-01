#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native {
    internal class UnityNativeRaisedEventBuilder {
         private readonly UnityNativeEventValidator _eventValidator;

        internal UnityNativeRaisedEventBuilder(UnityNativeEventValidator eventValidator) {
            _eventValidator = eventValidator;
        }

        internal UnityNativeEventBuilderResult<Dictionary<string, object>> Build(string eventName, Dictionary<string, object> properties = null) {
            var eventValidationResultsWithErrors = new List<UnityNativeValidationResult>();
            if (string.IsNullOrWhiteSpace(eventName)) {
                return new UnityNativeEventBuilderResult<Dictionary<string, object>>(eventValidationResultsWithErrors, null);
            }

            var isRestrictedNameValidationResult = _eventValidator.IsRestrictedName(eventName);
            if (!isRestrictedNameValidationResult.IsSuccess) {
                eventValidationResultsWithErrors.Add(isRestrictedNameValidationResult);
                return new UnityNativeEventBuilderResult<Dictionary<string, object>>(eventValidationResultsWithErrors, null);
            }
            
            var isDiscardedValidationResult = _eventValidator.IsEventDiscarded(eventName);
            if (!isDiscardedValidationResult.IsSuccess) {
                eventValidationResultsWithErrors.Add(isDiscardedValidationResult);
                return new UnityNativeEventBuilderResult<Dictionary<string, object>>(eventValidationResultsWithErrors, null);
            }

            var cleanEventNameValidationResult = _eventValidator.CleanEventName(eventName, out var cleanEventName);
            if (!cleanEventNameValidationResult.IsSuccess) {
                eventValidationResultsWithErrors.Add(cleanEventNameValidationResult);
                return new UnityNativeEventBuilderResult<Dictionary<string, object>>(eventValidationResultsWithErrors, null);
            }

            var eventDetails = new Dictionary<string, object>();
            eventDetails.Add(UnityNativeConstants.Event.EVENT_NAME, cleanEventName);

            if (properties == null || properties.Count == 0) {
                return new UnityNativeEventBuilderResult<Dictionary<string, object>>(eventValidationResultsWithErrors, eventDetails);
            }

            var cleanObjectDictionaryValidationResult = CleanObjectDictonary(properties);
            if (cleanObjectDictionaryValidationResult.ValidationResults.Any(vr => !vr.IsSuccess)) {
                eventValidationResultsWithErrors.AddRange(cleanObjectDictionaryValidationResult.ValidationResults.Where(vr => !vr.IsSuccess));
            }

            eventDetails.Add(UnityNativeConstants.Event.EVENT_DATA, cleanObjectDictionaryValidationResult.EventResult);

            return new UnityNativeEventBuilderResult<Dictionary<string, object>>(eventValidationResultsWithErrors, eventDetails);
        }

        internal UnityNativeEventBuilderResult<Dictionary<string, object>> BuildFetchEvent(int type) {
            // Validate that type is a supported fetch type
            // Currently only Variables fetch is supported
            if (type != UnityNativeConstants.Event.WZRK_FETCH_TYPE_VARIABLES) {
                var validationError = new UnityNativeValidationResult(512,
                $"Unsupported fetch type: {type}");
                return new UnityNativeEventBuilderResult<Dictionary<string, object>>(new List<UnityNativeValidationResult> { validationError }, null);
            }

            var eventDetails = new Dictionary<string, object> {
                { UnityNativeConstants.Event.EVENT_NAME, UnityNativeConstants.Event.EVENT_WZRK_FETCH }
            };

            var properties = new Dictionary<string, object> {
                { "t", type }
            };

            eventDetails.Add(UnityNativeConstants.Event.EVENT_DATA, properties);

            return new UnityNativeEventBuilderResult<Dictionary<string, object>>(new List<UnityNativeValidationResult>(), eventDetails);
        }

        internal UnityNativeEventBuilderResult<Dictionary<string, object>> BuildChargedEvent(Dictionary<string, object> details, List<Dictionary<string, object>> items) {
            var eventValidationResultsWithErrors = new List<UnityNativeValidationResult>();
            if (details == null || items == null) {
                return new UnityNativeEventBuilderResult<Dictionary<string, object>>(eventValidationResultsWithErrors, null);
            }

            if (items.Count > 50) {
                CleverTapLogger.Log("Charged event contained more than 50 items.");
                eventValidationResultsWithErrors.Add(new UnityNativeValidationResult(522, "Charged event contained more than 50 items."));
                return new UnityNativeEventBuilderResult<Dictionary<string, object>>(eventValidationResultsWithErrors, null);
            }

            var eventDetails = new Dictionary<string, object>();
            eventDetails.Add(UnityNativeConstants.Event.EVENT_NAME, UnityNativeConstants.Event.EVENT_CHARGED);

            var detailsCleanObjectDictonaryValidationResult = CleanObjectDictonary(details);
            var eventData = detailsCleanObjectDictonaryValidationResult.EventResult;
            if (detailsCleanObjectDictonaryValidationResult.ValidationResults.Any(vr => !vr.IsSuccess)) {
                eventValidationResultsWithErrors.AddRange(detailsCleanObjectDictonaryValidationResult.ValidationResults.Where(vr => !vr.IsSuccess));
            }

            eventDetails.Add(UnityNativeConstants.Event.EVENT_DATA, eventData);

            var itemsDetails = new List<Dictionary<string, object>>();
            foreach (var item in items) {
                var itemCleanObjectDictronaryValidationResult = CleanObjectDictonary(item);
                if (itemCleanObjectDictronaryValidationResult.ValidationResults.Any(vr => !vr.IsSuccess)) {
                    eventValidationResultsWithErrors.AddRange(itemCleanObjectDictronaryValidationResult.ValidationResults.Where(vr => !vr.IsSuccess));
                }

                itemsDetails.Add(itemCleanObjectDictronaryValidationResult.EventResult);
            }

            eventData.Add(UnityNativeConstants.Event.EVENT_CHARGED_ITEMS, itemsDetails);

            return new UnityNativeEventBuilderResult<Dictionary<string, object>>(eventValidationResultsWithErrors, eventDetails);
        }

        private UnityNativeEventBuilderResult<Dictionary<string, object>> CleanObjectDictonary(Dictionary<string, object> objectDictonary) {
            var cleanObjectDictonary = new Dictionary<string, object>();
            var eventValidationResultsWithErrors = new List<UnityNativeValidationResult>();
            foreach (var (key, value) in objectDictonary) {
                var cleanObjectKeyValdaitonReuslt = _eventValidator.CleanObjectKey(key, out var cleanKey);
                if (!cleanObjectKeyValdaitonReuslt.IsSuccess) {
                    eventValidationResultsWithErrors.Add(cleanObjectKeyValdaitonReuslt);
                    continue;
                }

                var cleanObjectValue = _eventValidator.CleanObjectValue(value, out var cleanValue, false);
                if (!cleanObjectValue.IsSuccess) {
                    eventValidationResultsWithErrors.Add(cleanObjectValue);
                    continue;
                }

                cleanObjectDictonary.Add(cleanKey, cleanValue);
            }

            return new UnityNativeEventBuilderResult<Dictionary<string, object>>(eventValidationResultsWithErrors, cleanObjectDictonary);
        }
    }
}
#endif