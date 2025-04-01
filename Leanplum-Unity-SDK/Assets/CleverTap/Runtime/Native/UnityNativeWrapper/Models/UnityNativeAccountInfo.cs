#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
namespace CleverTapSDK.Native
{
    internal class UnityNativeAccountInfo
    {
        private readonly string _accountId;
        private readonly string _accountToken;
        private readonly string _region;

        public UnityNativeAccountInfo(string accountId, string accountToken, string region = null)
        {
            _accountId = accountId;
            _accountToken = accountToken;
            _region = region;
        }

        public string AccountId => _accountId;
        public string AccountToken => _accountToken;
        public string Region => _region;
    }
}
#endif