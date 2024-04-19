using CleverTapSDK.Utilities;
using System;
using System.Collections.Generic;

namespace CleverTapSDK.Common {
    internal abstract class CleverTapPlatformInApps {
        protected readonly IDictionary<int, Action<bool>> inAppsFetchedCallbacks = new Dictionary<int, Action<bool>>();
        protected readonly CleverTapCounter inAppsFetchedIdCounter = new CleverTapCounter();

        internal virtual void FetchInApps(Action<bool> isSucessCallback) {
            var callbackId = inAppsFetchedIdCounter.GetNextAndIncreaseCounter();
            if (!inAppsFetchedCallbacks.ContainsKey(callbackId)) {
                inAppsFetchedCallbacks.Add(callbackId, isSucessCallback);
                FetchInApps(callbackId);
            }
        }

        internal virtual void InAppsFetched(int callbackId, bool isSuccess) {
            if (inAppsFetchedCallbacks.ContainsKey(callbackId)) {
                inAppsFetchedCallbacks[callbackId].Invoke(isSuccess);
                inAppsFetchedCallbacks.Remove(callbackId);
            }
        }

        internal abstract void FetchInApps(int callbackId);

        internal abstract void ClearInAppResources(bool expiredOnly);
    }
}
