using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Common
{
    internal abstract class CleverTapPlatformVariable
    {
        protected readonly IDictionary<string, IVar> varCache = new Dictionary<string, IVar>();
        protected readonly IDictionary<int, Action<bool>> variablesFetchedCallbacks = new Dictionary<int, Action<bool>>();

        protected readonly CleverTapCounter variablesFetchedIdCounter = new CleverTapCounter();

        #region Default - Variables

        internal virtual Var<T> GetVariable<T>(string name)
        {
            var kindName = GetKindNameFromGenericType<T>();
            if (!string.IsNullOrEmpty(kindName) && varCache.ContainsKey(name) && varCache[name].Kind == kindName)
            {
                return (Var<T>)varCache[name];
            }

            return null;
        }

        internal Var<int> Define(string name, int defaultValue) =>
            GetOrDefineVariable<int>(name, defaultValue);

        internal Var<long> Define(string name, long defaultValue) =>
            GetOrDefineVariable<long>(name, defaultValue);

        internal Var<short> Define(string name, short defaultValue) =>
            GetOrDefineVariable<short>(name, defaultValue);

        internal Var<byte> Define(string name, byte defaultValue) =>
            GetOrDefineVariable<byte>(name, defaultValue);

        internal Var<bool> Define(string name, bool defaultValue) =>
            GetOrDefineVariable<bool>(name, defaultValue);

        internal Var<float> Define(string name, float defaultValue) =>
            GetOrDefineVariable<float>(name, defaultValue);

        internal Var<double> Define(string name, double defaultValue) =>
            GetOrDefineVariable<double>(name, defaultValue);

        internal Var<string> Define(string name, string defaultValue) =>
            GetOrDefineVariable<string>(name, defaultValue);

        internal Var<Dictionary<string, object>> Define(string name, Dictionary<string, object> defaultValue) =>
            GetOrDefineVariable<Dictionary<string, object>>(name, defaultValue);

        internal Var<Dictionary<string, string>> Define(string name, Dictionary<string, string> defaultValue) =>
            GetOrDefineVariable<Dictionary<string, string>>(name, defaultValue);

        internal Var<string> DefineFileVariable(string name) =>
            GetOrDefineFileVariable(name);

        internal void FetchVariables(Action<bool> isSucessCallback)
        {
            var callbackId = variablesFetchedIdCounter.GetNextAndIncreaseCounter();
            if (!variablesFetchedCallbacks.ContainsKey(callbackId))
            {
                variablesFetchedCallbacks.Add(callbackId, isSucessCallback);
                FetchVariables(callbackId);
            }
        }

        internal void VariablesFetched(int callbackId, bool isSuccess)
        {
            if (variablesFetchedCallbacks.ContainsKey(callbackId))
            {
                variablesFetchedCallbacks[callbackId].Invoke(isSuccess);
                variablesFetchedCallbacks.Remove(callbackId);
            }
        }

        internal void VariableFileIsReady(string name)
        {
            if (varCache.ContainsKey(name))
            {
                varCache[name].FileIsReady();
            }
        }

        internal void VariableChanged(string name)
        {
            if (varCache.ContainsKey(name))
            {
                varCache[name].ValueChanged();
            }
        }

        protected virtual Var<T> GetOrDefineVariable<T>(string name, T defaultValue)
        {
            string kindName = GetKindNameFromGenericType<T>();
            return GetOrDefineVariable<T>(name, kindName, defaultValue);
        }

        protected virtual Var<string> GetOrDefineFileVariable(string name)
        {
            return GetOrDefineVariable<string>(name, CleverTapVariableKind.FILE, null);
        }

        protected virtual Var<T> GetOrDefineVariable<T>(string name, string kindName, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                CleverTapLogger.LogError("CleverTap Error: Variable name cannot be empty.");
                return null;
            }

            if (name.StartsWith(".") || name.EndsWith("."))
            {
                CleverTapLogger.LogError($"CleverTap Error: Variable name \"{name}\" starts or ends with a `.` which is not allowed");
                return null;
            }

            if (string.IsNullOrEmpty(kindName))
            {
                CleverTapLogger.LogError($"CleverTap Error: Default value for \"{name}\" not recognized or not supported.");
                return null;
            }

            if (varCache.ContainsKey(name))
            {
                if (varCache[name].Kind != kindName)
                {
                    CleverTapLogger.LogError($"CleverTap Error: Variable \"{name}\" was already defined with a different kind");
                    return null;
                }
                return (Var<T>)varCache[name];
            }

            return DefineVariable<T>(name, kindName, defaultValue);
        }

        protected virtual string GetKindNameFromGenericType<T>()
        {
            Type type = typeof(T);
            return GetKindNameFromType(type);
        }

        internal static string GetKindNameFromType(Type type)
        {
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(char) || type == typeof(sbyte) || type == typeof(byte))
            {
                return CleverTapVariableKind.INT;
            }
            else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            {
                return CleverTapVariableKind.FLOAT;
            }
            else if (type == typeof(string))
            {
                return CleverTapVariableKind.STRING;
            }
            else if (type.GetInterfaces().Contains(typeof(IDictionary)))
            {
                return CleverTapVariableKind.DICTIONARY;
            }
            else if (type == typeof(bool))
            {
                return CleverTapVariableKind.BOOLEAN;
            }

            return string.Empty;
        }

        #endregion

        internal abstract void SyncVariables();
        internal abstract void SyncVariables(bool isProduction);
        internal abstract void FetchVariables(int callbackId);
        protected abstract Var<T> DefineVariable<T>(string name, string kind, T defaultValue);
    }
}
