#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CleverTapSDK.Common;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native
{
    internal class UnityNativeVarCache
	{
        internal int VariablesCount { get => vars.Count; }
        internal const string DIFFS_KEY = "DIFFS_{0}";

        private readonly IDictionary<string, IVar> vars = new Dictionary<string, IVar>();
        private IDictionary<string, object> diffs = new Dictionary<string, object>();
        private object merged = null;
        private IDictionary<string, object> valuesFromClient = new Dictionary<string, object>();
        private bool hasVarsRequestCompleted = false;

        private UnityNativePreferenceManager preferenceManager;
        private UnityNativeCoreState coreState;

        private readonly object DiffsLock = new();

        internal void SetCoreState(UnityNativeCoreState coreState)
        {
            if (coreState == null)
            {
                CleverTapLogger.LogError("CoreState cannot be null.");
                return;
            }

            this.coreState = coreState;
            preferenceManager = UnityNativePreferenceManager.GetPreferenceManager(coreState.AccountInfo.AccountId);
        }

        internal bool HasVarsRequestCompleted => hasVarsRequestCompleted;
        internal void SetHasVarsRequestCompleted(bool completed)
        {
            hasVarsRequestCompleted = completed;
        }

        internal string GetDiffsKey(string deviceId)
        {
            return string.Format(DIFFS_KEY, deviceId);
        }

        internal virtual void RegisterVariable(IVar variable)
		{
            if (variable == null)
            {
                CleverTapLogger.LogError("RegisterVariable called with null variable.");
                return;
            }

            vars[variable.Name] = variable;

            object defaultValue = variable.DefaultObjectValue;
            if (defaultValue is IDictionary valueDictionary)
            {
                defaultValue = UnityNativeVariableUtils.CopyDictionary(valueDictionary);
            }
            UnityNativeVariableUtils.UpdateValues(variable.Name, variable.NameComponents, defaultValue, (IDictionary)valuesFromClient);

            MergeVariable(variable);
		}

        internal Var<T> GetVariable<T>(string name)
        {
            if (vars.ContainsKey(name))
            {
                if (vars[name] is Var<T> typedVar)
                {
                    return typedVar;
                }
                CleverTapLogger.Log($"GetVariable: Variable '{name}' exists but is not of type {typeof(T).Name}");
            }
            return null;
        }

        internal object GetMergedValue(string name)
        {
            string[] components = UnityNativeVariableUtils.GetNameComponents(name);
            object mergedValue = GetMergedValueFromComponentArray(components);
            if (mergedValue is IDictionary valueDictionary)
            {
                return UnityNativeVariableUtils.CopyDictionary(valueDictionary);
            }
            return mergedValue;
        }

        internal virtual object GetMergedValueFromComponentArray(object[] components)
        {
            return GetMergedValueFromComponentArray(components, merged ?? valuesFromClient);
        }

        internal object GetMergedValueFromComponentArray(object[] components, object values)
        {
            object mergedPtr = values;
            foreach (var component in components)
            {
                mergedPtr = UnityNativeVariableUtils.Traverse(mergedPtr, component, false);
            }
            return mergedPtr;
        }

        /// <summary>
        /// Merge default variable value with VarCache.merged value.
        /// If invoked with a.b.c.d, updates a, a.b, a.b.c, but a.b.c.d is left for the Var.Define.
        /// This is neccessary if variable was registered after VarCache.applyVariableDiffs.
        /// </summary>
        /// <param name="variable"></param>
        private void MergeVariable(IVar variable)
        {
            if (merged == null)
            {
                CleverTapLogger.Log("MergeVariable called, but `merged` member is null.");
                return;
            }
            if (merged is not IDictionary mergedDictionary)
            {
                CleverTapLogger.Log("MergeVariable called, but `merged` member is not of Dictionary type.");
                return;
            }

            string firstComponent = variable.NameComponents[0];
            valuesFromClient.TryGetValue(firstComponent, out object defaultValue);
            mergedDictionary.TryGetValue(firstComponent, out object mergedValue);

            bool shouldMerge = defaultValue != null && !defaultValue.Equals(mergedValue);
            if (shouldMerge)
            {
                object newValue = UnityNativeVariableUtils.MergeHelper(defaultValue, mergedValue);

                mergedDictionary[firstComponent] = newValue;

                // Build the name progressively to find and update all affected parent variables
                StringBuilder name = new StringBuilder(firstComponent);
                for (int i = 1; i < variable.NameComponents.Length; i++)
                {
                    // Try to update any existing variable with the current name path
                    vars.TryGetValue(name.ToString(), out IVar existing);
                    existing?.Update();
                    // Add the next component to the path
                    name.Append(UnityNativeVariableUtils.DOT)
                        .Append(variable.NameComponents[i]);
                }
            }
        }

        internal void ApplyVariableDiffs(IDictionary<string, object> diffs)
        {
            lock (DiffsLock)
            {
                if (diffs == null)
                    return;

                this.diffs = diffs;
                merged = UnityNativeVariableUtils.MergeHelper(valuesFromClient, this.diffs);

                TriggerVariablesUpdate();
            }
        }

        internal void TriggerVariablesUpdate()
        {
            foreach (var kv in vars)
            {
                var variable = kv.Value;
                variable?.Update();
            }
        }

        internal void LoadDiffs()
        {
            if (coreState == null || coreState.AccountInfo == null || coreState.DeviceInfo == null)
            {
                CleverTapLogger.LogError("Cannot load diffs before setting the coreState.");
                return;
            }

            CleverTapLogger.Log($"Loading Variable Diffs for deviceId: {coreState.DeviceInfo.DeviceId}");
            string diffsString = preferenceManager.GetString(GetDiffsKey(coreState.DeviceInfo.DeviceId), "{}");
            if (Json.Deserialize(diffsString) is IDictionary<string, object> diffsDict)
            {
                ApplyVariableDiffs(diffsDict);
            }
        }

        internal void SaveDiffs()
        {
            if (coreState == null || coreState.AccountInfo == null || coreState.DeviceInfo == null)
            {
                CleverTapLogger.LogError("Cannot save diffs before setting the coreState.");
                return;
            }

            lock (DiffsLock)
            {
                string serializedData = Json.Serialize(diffs);
                preferenceManager.SetString(GetDiffsKey(coreState.DeviceInfo.DeviceId), serializedData);
            }
        }

        internal Dictionary<string, object> GetDefineVarsPayload()
        {
            return UnityNativeVariableUtils.GetFlatVarsPayload(vars);
        }

        /// <summary>
        /// Resets the state.
        /// Sets hasVarsRequestCompleted to false, expecting new request. 
        /// Resets the registered variables.
        /// </summary>
        internal void Reset()
        {
            hasVarsRequestCompleted = false;
            foreach (var variable in vars)
            {
                variable.Value?.Reset();
            }
        }
    }
}
#endif