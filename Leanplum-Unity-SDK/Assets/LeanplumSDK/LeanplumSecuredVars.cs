using System.Collections.Generic;

namespace LeanplumSDK
{
    /// <summary>
    /// Represents Variables in JSON format, cryptographically signed from Leanplum server.
    /// </summary>
    public class LeanplumSecuredVars
    {
        private readonly string json;
        private readonly string signature;

        /// <summary>
        /// The JSON representation of the variables as received from Leanplum server.
        /// </summary>
        public string VarsJson
        {
            get
            {
                return json;
            }
        }

        /// <summary>
        /// Get the cryptographic signature of the variables.
        /// </summary>
        public string VarsSignature
        {
            get
            {
                return signature;
            }
        }

        internal LeanplumSecuredVars()
        {

        }

        public LeanplumSecuredVars(string json, string signature) : base()
        {
            this.json = json;
            this.signature = signature;
        }

        public static LeanplumSecuredVars FromDictionary(Dictionary<string, object> varsDict)
        {
            if (varsDict != null)
            {
                string json = Util.GetValueOrDefault(varsDict, Constants.Keys.SECURED_VARS_JSON_KEY)?.ToString();
                string signature = Util.GetValueOrDefault(varsDict, Constants.Keys.SECURED_VARS_SIGNATURE_KEY)?.ToString();
                if (!string.IsNullOrEmpty(json) && !string.IsNullOrEmpty(signature))
                {
                    LeanplumSecuredVars leanplumSecuredVars = new(json, signature);
                    return leanplumSecuredVars;
                }
            }
            return null;
        }
    }
}