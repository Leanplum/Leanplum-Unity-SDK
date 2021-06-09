namespace LeanplumSDK
{
    public class LeanplumSecuredVars
    {
        private string json;
        private string signature;
        
        public string varsJson
        {
            get
            {
                return json;
            }
        }
        
        public string varsSignature
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
    }
}