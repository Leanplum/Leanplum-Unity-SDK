#if UNITY_IOS
using CleverTapSDK.Common;
using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;
using System;
using System.Collections;

namespace CleverTapSDK.IOS {
    internal class IOSVar<T> : Var<T> {
        internal IOSVar(string name, string kind, T defaultValue) : base(name, kind, defaultValue) {}

        public override T Value {
            get {
                if (kind.Equals(CleverTapVariableKind.FILE)) {
                    string fileValue = IOSDllImport.CleverTap_getFileVariableValue(name);
                    if (typeof(T) == typeof(string))
                    {
                        return (T)Convert.ChangeType(fileValue, typeof(T));
                    }
                    else
                    {
                        CleverTapLogger.LogError("File variables must be of string type");
                        return value;
                    }
                }

                string jsonRepresentation = IOSDllImport.CleverTap_getVariableValue(name);

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