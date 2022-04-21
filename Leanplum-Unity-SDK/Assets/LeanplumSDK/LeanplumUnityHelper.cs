//
// Copyright 2022, Leanplum, Inc.
//
//  Licensed to the Apache Software Foundation (ASF) under one
//  or more contributor license agreements.  See the NOTICE file
//  distributed with this work for additional information
//  regarding copyright ownership.  The ASF licenses this file
//  to you under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing,
//  software distributed under the License is distributed on an
//  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//  KIND, either express or implied.  See the License for the
//  specific language governing permissions and limitations
//  under the License.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#if !LP_UNITY_LEGACY_WWW
#if UNITY_5_5_OR_NEWER
using UnityNetworkingRequest = UnityEngine.Networking.UnityWebRequest;
using DownloadHandlerAssetBundle = UnityEngine.Networking.DownloadHandlerAssetBundle;
#else
using UnityNetworkingRequest = UnityEngine.Experimental.Networking.UnityWebRequest;
using DownloadHandlerAssetBundle = UnityEngine.Experimental.Networking.DownloadHandlerAssetBundle;
#endif
#endif

namespace LeanplumSDK
{
    /// <summary>
    ///     Provides a class that is implemented in a MonoBehaviour so that Unity functions can be
    ///     called through the GameObject.
    /// </summary>
    public class LeanplumUnityHelper : MonoBehaviour
    {
        private static LeanplumUnityHelper instance;

        internal static List<Action> delayed = new List<Action>();

        private bool developerModeEnabled;

        public static LeanplumUnityHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    var existing = FindObjectsOfType<LeanplumUnityHelper>();
                    foreach (var obj in existing)
                    {
                        Destroy(obj);
                    }

                    // Create LeanplumUnityHelper 
                    GameObject container = new GameObject("LeanplumUnityHelper");
                    instance = container.AddComponent<LeanplumUnityHelper>();

                    // In case instance is left, never save it to a scene file 
                    instance.hideFlags = HideFlags.DontSaveInEditor;
                }
                return instance;
            }
        }

        private readonly LinkedList<IEnumerator> coroutineQueue = new LinkedList<IEnumerator>();
        private IEnumerator runningCoroutine;

        IEnumerator CoroutineCoordinator()
        {
            // Executed every frame
            while (true)
            {
                while (coroutineQueue.Count > 0)
                {
                    if (runningCoroutine == null)
                    {
                        runningCoroutine = coroutineQueue.First.Value;
                        coroutineQueue.RemoveFirst();
                        yield return StartCoroutine(runningCoroutine);
                        runningCoroutine = null;
                    }
                }
                yield return null;
            }
        }

        /// <summary>
        /// Enqueue tasks to be executed synchronously
        /// </summary>
        /// <param name="task">The action to be executed</param>
        public void Enqueue(Action task)
        {
            coroutineQueue.AddLast(DoTask(task));
        }

        public void Enqueue(Func<IEnumerator> func)
        {
            coroutineQueue.AddLast(func());
        }

        /// <summary>
        /// Wraps Action to be executed from a Coroutine
        /// </summary>
        /// <param name="task">The action to be executed</param>
        /// <returns>Iterator for the Coroutine</returns>
        internal IEnumerator DoTask(Action task)
        {
            yield return null;
            task();
        }

        public void NativeCallback(string message)
        {
            LeanplumFactory.SDK.NativeCallback(message);
        }

        private void Start()
        {
            developerModeEnabled = Leanplum.IsDeveloperModeEnabled;

            // Prevent Unity from destroying this GameObject when a new scene is loaded.
            DontDestroyOnLoad(this.gameObject);

            StartCoroutine(CoroutineCoordinator());
        }

        private void OnApplicationQuit()
        {
            LeanplumNative.CompatibilityLayer.FlushSavedSettings();
            if (LeanplumNative.calledStart)
            {
                LeanplumNative.Stop();
            }
            LeanplumNative.isStopped = true;
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (!LeanplumNative.calledStart)
            {
                return;
            }

            if (isPaused)
            {
                LeanplumNative.Pause();
            }
            else
            {
                LeanplumNative.Resume();
            }
        }

        private void Update()
        {
            // Workaround so that CheckVarsUpdate() is invoked on Unity's main thread.
            // This is called by Unity on every frame.
            if (VarCache.VarsNeedUpdate && developerModeEnabled && Leanplum.HasStarted)
            {
                Leanplum.ForceContentUpdate();
            }

            // Run deferred actions.
            List<Action> actions = null;
            lock (delayed)
            {
                if (delayed.Count > 0)
                {
                    actions = new List<Action>(delayed);
                    delayed.Clear();
                }
            }
            if (actions != null)
            {
                foreach (Action action in actions)
                {
                    action();
                }
            }
        }

        internal void StartRequest(string url, WWWForm wwwForm, Action<WebResponse> responseHandler,
                                   int timeout, bool isAsset = false)
        {
            if (!isAsset)
            {
                // API requests are executed synchronously one after another
                // Insert at front so the request is run immediately after the task that creates and starts it
                // Current flow: Send -> SaveRequest -> SendRequests
                // Wait for the request to execute and then continue with next queued requests
                coroutineQueue.AddFirst(RunRequest(url, wwwForm, responseHandler, timeout, isAsset));
            }
            else
            {
                // Issue asset requests immediately
                StartCoroutine(RunRequest(url, wwwForm, responseHandler, timeout, isAsset));
            }
        }

#if !LP_UNITY_LEGACY_WWW
        private static UnityNetworkingRequest CreateWebRequest(string url, WWWForm wwwForm, bool isAsset)
        {
            UnityNetworkingRequest result;
            if (wwwForm == null && !isAsset)
            {
                result = UnityNetworkingRequest.Get(url);
            }
            else if (isAsset)
            {
                // CRC not available yet
                result = UnityWebRequestAssetBundle.GetAssetBundle(url);
            }
            else
            {
                result = UnityNetworkingRequest.Post(url, wwwForm);
            }
            return result;
        }
#else
        private static WWW CreateWww(string url, WWWForm wwwForm, bool isAsset)
        {
            WWW result = null;
            if (isAsset)
            {
                // Set an arbitrary version number - we identify different versions of assetbundles with
                // different filenames in the url.
                result = WWW.LoadFromCacheOrDownload(url, 1);
            }
            else
            {
                result = wwwForm == null ? new WWW(url) : new WWW(url, wwwForm);
            }
            return result;
        }
#endif

        private static IEnumerator RunRequest(string url, WWWForm wwwForm, Action<WebResponse> responseHandler, int timeout, bool isAsset)
        {
#if !LP_UNITY_LEGACY_WWW
            using (var request = CreateWebRequest(url, wwwForm, isAsset))
            {
                request.timeout = timeout;

                yield return request.SendWebRequest();

                while (!request.isDone)
                {
                    yield return null;
                }

                if (request.result == UnityNetworkingRequest.Result.ConnectionError
                    || request.result == UnityNetworkingRequest.Result.ProtocolError)
                {
                    responseHandler(new LeanplumUnityWebResponse(request.responseCode, request.error, null, null));
                }
                else
                {
                    DownloadHandler download = request.downloadHandler;
                    DownloadHandlerAssetBundle downloadAsset = download as DownloadHandlerAssetBundle;
                    responseHandler(new LeanplumUnityWebResponse(request.responseCode,
                        request.error,
                        !isAsset ? download.text : null,
                        isAsset ? downloadAsset?.assetBundle : null));
                }

            }
#else
            using (WWW www = CreateWww(url, wwwForm, isAsset))
            {
                float elapsed = 0.0f;
                while (!www.isDone && elapsed < timeout)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                if (www.isDone)
                {
                    responseHandler(new UnityWebResponse(200, www.error,
                        (String.IsNullOrEmpty(www.error) && !isAsset) ? www.text : null,
                        (String.IsNullOrEmpty(www.error) && isAsset) ? www.assetBundle : null));
                }
                else
                {
                    responseHandler(new UnityWebResponse(408, Constants.NETWORK_TIMEOUT_MESSAGE, String.Empty, null));
                }
            }
#endif
        }

        internal static void QueueOnMainThread(Action method)
        {
            lock (delayed)
            {
                delayed.Add(method);
            }
        }
    }
}
