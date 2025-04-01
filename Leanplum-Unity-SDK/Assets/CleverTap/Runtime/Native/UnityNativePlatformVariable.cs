#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;
using CleverTapSDK.Common;
using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;
using UnityEngine;

namespace CleverTapSDK.Native
{
    internal interface IVariablesResponseHandler
    {
        void HandleVariablesResponse(IDictionary<string, object> vars);
        void HandleVariablesResponseError();
    }

    internal class UnityNativePlatformVariable : CleverTapPlatformVariable, IVariablesResponseHandler
    {
        private readonly UnityNativeVarCache nativeVarCache;
        private UnityNativeEventManager unityNativeEventManager;
        private CleverTapCallbackHandler callbackHandler;
        private bool hasLoaded;
        private int fetchVariablesCallbackId = -1;

        internal UnityNativePlatformVariable() : this(new UnityNativeVarCache())
        {
        }

        internal UnityNativePlatformVariable(UnityNativeVarCache unityNativeVarCache) : base()
        {
            nativeVarCache = unityNativeVarCache;
        }

        /// <summary>
        /// Sets the dependencies. Loads variables diffs.
        /// Call this method as soon as the SDK initializes.
        /// </summary>
        /// <param name="unityNativeEventManager">The event manager instance.</param>
        /// <param name="callbackHandler">The <see cref="CleverTapCallbackHandler"/> to trigger Variables Callbacks.</param>
        /// <param name="coreState">
        /// The CoreState instance used accross the SDK instance.
        /// Requires reference so DeviceInfo updates are reflected.
        /// </param>
        internal void Load(UnityNativeEventManager unityNativeEventManager,
            CleverTapCallbackHandler callbackHandler,
            UnityNativeCoreState coreState)
        {
            if (unityNativeEventManager == null || callbackHandler == null || coreState == null)
            {
                CleverTapLogger.LogError("Cannot load UnityNativePlatformVariable with null parameters.");
                return;
            }

            hasLoaded = true;

            this.unityNativeEventManager = unityNativeEventManager;
            this.callbackHandler = callbackHandler;

            nativeVarCache.SetCoreState(coreState);
            // Load diffs on platform load
            nativeVarCache.LoadDiffs();
        }

        internal bool HasVarsRequestCompleted => nativeVarCache.HasVarsRequestCompleted;

        /// <summary>
        /// Resets the <see cref="UnityNativeVarCache"/> var cache.
        /// Loads the diffs.
        /// </summary>
        internal void ReloadCache()
        {
            if (nativeVarCache == null)
            {
                CleverTapLogger.LogError("Cannot reload cache - the VarCache is null.");
                return;
            }

            nativeVarCache.Reset();
            nativeVarCache.LoadDiffs();
        }

        void IVariablesResponseHandler.HandleVariablesResponse(IDictionary<string, object> vars)
        {
            if (hasLoaded)
            {
                HandleVariablesResponseSuccess(vars);
            }
            else
            {
                CleverTapLogger.LogError("CleverTap Error: Cannot handle variables success response. The platform variable is not loaded.");
            }
        }

        void IVariablesResponseHandler.HandleVariablesResponseError()
        {
            if (hasLoaded)
            {
                HandleVariablesResponseError();
            }
            else
            {
                CleverTapLogger.LogError("CleverTap Error: Cannot handle variables error response. The platform variable is not loaded.");
            }
        }

        /// <summary>
        /// Handles variables response success.
        /// Applies the diffs from the response and saves them.
        /// Triggers variables callbacks.
        /// Triggers Variables Fetched with success.
        /// </summary>
        /// <param name="varsJson">The variables from the response.</param>
        internal void HandleVariablesResponseSuccess(IDictionary<string, object> varsJson)
        {
            CleverTapLogger.Log("Variables Response Success");
            nativeVarCache.SetHasVarsRequestCompleted(true);

            var diffs = UnityNativeVariableUtils.ConvertDictionaryToNestedDictionaries(varsJson);
            nativeVarCache.ApplyVariableDiffs(diffs);
            nativeVarCache.SaveDiffs();

            TriggerVariablesChanged();
            TriggerVariablesFetched(true);
        }

        /// <summary>
        /// Handles variables response error.
        /// Marks the vars request as completed <see cref="HasVarsRequestCompleted"/>.
        /// Triggers Variables Changed callbacks.
        /// Triggers individual variables Update callbacks.
        /// Triggers Variables Fetched with error.
        /// </summary>
        /// <remarks>
        /// The vars diffs are already loaded in <see cref="Load" /> or <see cref="ReloadCache" />
        /// </remarks>
        internal void HandleVariablesResponseError()
        {
            if (!nativeVarCache.HasVarsRequestCompleted)
            {
                CleverTapLogger.Log("Handling Variables Response Error");
                nativeVarCache.SetHasVarsRequestCompleted(true);
                nativeVarCache.TriggerVariablesUpdate();
                TriggerVariablesChanged();
            }

            TriggerVariablesFetched(false);
        }

        private void TriggerVariablesChanged()
        {
            callbackHandler.CleverTapVariablesChanged("VariablesChanged");
            callbackHandler.CleverTapVariablesChangedAndNoDownloadsPending("VariablesChangedAndNoDownloadsPending");
        }

        private void TriggerVariablesFetched(bool success)
        {
            var fetchedMessage = new Dictionary<string, object>
            {
                { "isSuccess", success },
                { "callbackId", fetchVariablesCallbackId }
            };
            callbackHandler.CleverTapVariablesFetched(Json.Serialize(fetchedMessage));
        }

        internal override void SyncVariables()
        {
            SyncVariables(false);
        }

        internal override void SyncVariables(bool isProduction)
        {
            if (isProduction)
            {
                if (Debug.isDebugBuild)
                {
                    CleverTapLogger.Log("Calling SyncVariables(true) from Debug build. Do not use (isProduction: true) in this case.");
                }
                else
                {
                    CleverTapLogger.Log("Calling SyncVariables(true) from Release build. Do not release this build and use with caution.");
                }
            }
            else
            {
                if (!Debug.isDebugBuild)
                {
                    CleverTapLogger.Log("SyncVariables() can only be called from Debug builds.");
                    return;
                }
            }

            if (unityNativeEventManager != null && nativeVarCache != null)
            {
                if (nativeVarCache.VariablesCount == 0)
                {
                    CleverTapLogger.Log("CleverTap: No Variables defined.");
                }
                unityNativeEventManager.SyncVariables(nativeVarCache.GetDefineVarsPayload());
            }
            else
            {
                CleverTapLogger.LogError("CleverTap Error: Cannot sync variables. The platform variable is not loaded.");
            }
        }

        internal override void FetchVariables(int callbackId)
        {
            if (unityNativeEventManager != null && nativeVarCache != null)
            {
                fetchVariablesCallbackId = callbackId;
                unityNativeEventManager.FetchVariables();
            }
            else
            {
                CleverTapLogger.LogError("CleverTap Error: Cannot fetch variables. The platform variable is not loaded.");
            }
        }

        protected override Var<T> DefineVariable<T>(string name, string kind, T defaultValue)
        {
            UnityNativeVar<T> result = new UnityNativeVar<T>(name, kind, defaultValue, nativeVarCache);
            varCache.Add(name, result);
            return result;
        }

        protected override Var<T> GetOrDefineVariable<T>(string name, T defaultValue)
        {
            var variable = base.GetOrDefineVariable<T>(name, defaultValue);
            return variable;
        }

        /// <summary>
        /// Defines a file variable with <paramref name="name"/> or gets the variable if already defined.
        /// Defining and syncing File variables is supported.
        /// Downloads and callbacks for file variables are not supported.
        /// Values are URL strings, not the actual files.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The file variable with type string.</returns>
        protected override Var<string> GetOrDefineFileVariable(string name)
        {
            return DefineVariable<string>(name, CleverTapVariableKind.FILE, null);
        }
    }
}
#endif
