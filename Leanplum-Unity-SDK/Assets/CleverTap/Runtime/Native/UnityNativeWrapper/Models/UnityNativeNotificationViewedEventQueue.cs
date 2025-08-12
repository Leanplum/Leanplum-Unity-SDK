#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleverTapSDK.Native
{
    internal class UnityNativeNotificationViewedEventQueue : UnityNativeBaseEventQueue
    {
        protected override string QueueName => nameof(UnityNativeNotificationViewedEventQueue);

        internal UnityNativeNotificationViewedEventQueue(UnityNativeCoreState coreState,
            UnityNativeNetworkEngine networkEngine) :
            base(coreState, networkEngine)
        {
  
        }

        protected override string GetRequestBaseURL()
        {
            return networkEngine.SpikyURI;
        }

        internal override async Task<List<UnityNativeEvent>> FlushEvents()
        {
            return await FlushEventsCore(request => networkEngine.ExecuteRequest(request));
        }
    }
}
#endif