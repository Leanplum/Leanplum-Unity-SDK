#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleverTapSDK.Native
{
    internal class UnityNativeRaisedEventQueue : UnityNativeBaseEventQueue
    {
        protected override string QueueName => "RAISED_EVENTS";

        internal UnityNativeRaisedEventQueue(UnityNativeCoreState coreState,
            UnityNativeNetworkEngine networkEngine, int queueLimit = 49, int defaultTimerInterval = 1) :
            base(coreState, networkEngine, queueLimit, defaultTimerInterval)
        { }

        internal override async Task<List<UnityNativeEvent>> FlushEvents()
        {
            return await FlushEventsCore(path => networkEngine.ExecuteRequest(path));
        }
    }
}
#endif