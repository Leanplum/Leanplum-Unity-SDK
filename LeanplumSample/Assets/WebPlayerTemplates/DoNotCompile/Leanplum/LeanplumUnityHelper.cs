//
// Copyright 2013, Leanplum, Inc.
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
using System.Timers;
using UnityEngine;

namespace LeanplumSDK
{
    /// <summary>
    ///     Provides a class that is implemented in a MonoBehaviour so that Unity functions can be
    ///     called through the GameObject.
    ///
    /// </summary>
    public class LeanplumUnityHelper : MonoBehaviour
    {
        private static LeanplumUnityHelper instance;

        internal static List<Action> delayed = new  List<Action>();

        private bool developerModeEnabled;

        public static LeanplumUnityHelper Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                // Object not found - create and store a new one.
                instance = FindObjectOfType(typeof(LeanplumUnityHelper)) as LeanplumUnityHelper;

                GameObject container = new GameObject("LeanplumUnityHelper", typeof(LeanplumUnityHelper));
                instance = container.GetComponent<LeanplumUnityHelper>();
                if (instance == null)
                {
                    LeanplumNative.CompatibilityLayer.LogError("Problem during the creation of LeanplumUnityHelper.");
                }
                return instance;
            }

            private set
            {
                instance = value;
            }
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
                VarCache.CheckVarsUpdate();
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
            StartCoroutine(RunRequest(url, wwwForm, responseHandler, timeout, isAsset));
        }

        private static IEnumerator RunRequest(string url, WWWForm wwwForm, Action<WebResponse> responseHandler,
                                              int timeout, bool isAsset)
        {
            WWW www;

            // If this is an assetbundle download request, try loading from cache first.
            if (isAsset)
            {
                // Set an arbitrary version number - we identify different versions of assetbundles with
                // different filenames in the url.
                www = WWW.LoadFromCacheOrDownload(url, 1);
            }
            else
            {
                www = wwwForm == null ? new WWW(url) : new WWW(url, wwwForm);
            }

            // Create a timer to check for timeouts.
            var timeoutTimer = new Timer(timeout * 1000);
            timeoutTimer.Elapsed += delegate {
                timeoutTimer.Stop();
                www.Dispose();
                QueueOnMainThread(() =>
                {
                    responseHandler(new UnityWebResponse(Constants.NETWORK_TIMEOUT_MESSAGE, String.Empty, null));
                });
            };
            timeoutTimer.Start();

            yield return www;

            // If the timer is still enabled, the request didn't time out.
            if (timeoutTimer.Enabled)
            {
                timeoutTimer.Stop();
                responseHandler(new UnityWebResponse(www.error,
                                                     String.IsNullOrEmpty(www.error) && !isAsset ? www.text : null,
                                                     String.IsNullOrEmpty(www.error) ? www.assetBundle : null));
                www.Dispose();
            }
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
