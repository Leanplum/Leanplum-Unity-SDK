#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;

namespace CleverTapSDK.Native {
    internal class UnityNativeProfileEventBuilder {
        private readonly UnityNativeEventValidator _eventValidator;

        internal UnityNativeProfileEventBuilder(UnityNativeEventValidator eventValidator) {
            _eventValidator = eventValidator;
        }

        internal UnityNativeEventBuilderResult<UnityNativePushEventResult> BuildPushEvent(Dictionary<string, object> properties) {
            var eventValidationResultsWithErrors = new List<UnityNativeValidationResult>();
            if (properties == null || properties.Count == 0) {
                return new UnityNativeEventBuilderResult<UnityNativePushEventResult>(eventValidationResultsWithErrors, new UnityNativePushEventResult(null, null));
            }

            var systemFields = new Dictionary<string, object>();
            var customFields = new Dictionary<string, object>();

            var cleanObjectDictonaryValidationResult = CleanObjectDictonary(properties);
            var allEventFields = cleanObjectDictonaryValidationResult.EventResult;
            if (cleanObjectDictonaryValidationResult.ValidationResults.Any(vr => !vr.IsSuccess)) {
                eventValidationResultsWithErrors.AddRange(cleanObjectDictonaryValidationResult.ValidationResults.Where(vr => !vr.IsSuccess));
            }


            var profile = new Dictionary<string, object>();
            foreach (var (key, value) in allEventFields) {
                if (UnityNativeConstants.Profile.IsKeyKnownProfileField(key)) {
                    profile.Add(UnityNativeConstants.Profile.GetKnownProfileFieldForKey(key), value);
                    continue;
                }                
            }

            customFields.Add("profile", profile);
            return new UnityNativeEventBuilderResult<UnityNativePushEventResult>(eventValidationResultsWithErrors, new UnityNativePushEventResult(systemFields, customFields));
        }

        private UnityNativeEventBuilderResult<Dictionary<string, object>> CleanObjectDictonary(Dictionary<string, object> objectDictonary) {
            var cleanObjectDictonary = new Dictionary<string, object>();
            var eventValidationResultsWithErrors = new List<UnityNativeValidationResult>();
            foreach (var (key, value) in objectDictonary) {
                var cleanObjectKeyValidationResult = _eventValidator.CleanObjectKey(key, out var cleanKey);
                if (!cleanObjectKeyValidationResult.IsSuccess) {
                    eventValidationResultsWithErrors.Add(cleanObjectKeyValidationResult);
                    continue;
                }

                var cleanObjectValue = _eventValidator.CleanObjectValue(value, out var cleanValue, true);
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