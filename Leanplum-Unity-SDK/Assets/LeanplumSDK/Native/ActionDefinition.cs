using System.Collections.Generic;

namespace LeanplumSDK
{
    public class ActionDefinition
    {
        internal string Name { get; set; }
        internal Constants.ActionKind Kind;
        internal IDictionary<string, object> Options;
        internal ActionArgs Args { get; set; }
        internal IDictionary<string, object> Vars;
        internal ActionContext.ActionResponder Responder { get; set; }
    }
}