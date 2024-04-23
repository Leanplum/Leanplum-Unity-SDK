#if UNITY_IOS
using CleverTapSDK.Common;
using CleverTapSDK.Utilities;
using System;
using System.Collections;

namespace CleverTapSDK.IOS {
    internal class IOSVar<T> : Var<T> {
        internal IOSVar(string name, string kind, T defaultValue) : base(name, kind, defaultValue) {}

        public override T Value {
            get {
                string jsonRepresentation = IOSDllImport.CleverTap_getVariableValue(name, kind);

                if (jsonRepresentation == null) {
                    return defaultValue;
                }

                if (jsonRepresentation == Json.Serialize(value)) {
                    return value;
                }

                object newValue = Json.Deserialize(jsonRepresentation);

                if (newValue is IDictionary) {
                    Util.FillInValues(newValue, value);
                } else {
                    value = (T)Convert.ChangeType(newValue, typeof(T));
                }

                return value;
            }
        }
    }
}
#endif