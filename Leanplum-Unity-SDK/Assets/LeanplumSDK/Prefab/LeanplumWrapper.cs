//
// Copyright 2020, Leanplum, Inc.
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
using System.Collections.Generic;
using LeanplumSDK;
using UnityEngine;
using LeanplumSDK.MiniJSON;
using System.Linq;
using System.Collections;

public class LeanplumWrapper : MonoBehaviour
{
    public string AppID;
    public string ProductionKey;
    public string DevelopmentKey;
    public string AppVersion;

    void Awake()
	{
		if (Application.isEditor)
		{
			LeanplumFactory.SDK = new LeanplumNative();
		}
		else
		{
			// NOTE: Currently, the native iOS and Android SDKs do not support Unity Asset Bundles.
			// If you require the use of asset bundles, use LeanplumNative on all platforms.
			#if UNITY_IPHONE
			LeanplumFactory.SDK = new LeanplumApple();
			#elif UNITY_ANDROID
			LeanplumFactory.SDK = new LeanplumAndroid();
			#else
			LeanplumFactory.SDK = new LeanplumNative();
            #endif
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

		SocketUtilsFactory.Utils = new SocketUtils();
        
        if (!string.IsNullOrEmpty(AppVersion))
        {
            Leanplum.SetAppVersion(AppVersion);
        }
        if (string.IsNullOrEmpty(AppID) || string.IsNullOrEmpty(ProductionKey) || string.IsNullOrEmpty(DevelopmentKey))
        {
            Debug.LogError("Please make sure to enter your AppID, Production Key, and " +
                           "Development Key in the Leanplum GameObject inspector before starting.");
        }   

        if (Debug.isDebugBuild)
        {
            Leanplum.SetAppIdForDevelopmentMode(AppID, DevelopmentKey);
        }
        else
        {
            Leanplum.SetAppIdForProductionMode(AppID, ProductionKey);
        }

        Leanplum.Inbox.InboxChanged += inboxChanged;
        Leanplum.Inbox.ForceContentUpdate += forceContentUpdate;

#if UNITY_EDITOR
        EditorMessageTemplates.DefineConfirm();
        EditorMessageTemplates.DefineOpenURL();
        EditorMessageTemplates.DefineGenericDefinition();
        LeanplumNative.ShouldPerformActions(true);
#endif

        // Sample Implementation
        // TODO: remove when done testing here
        Leanplum.ShouldDisplayMessage((context) =>
        {
            Debug.Log($"ShouldDisplayMessage: {context}");

            if (GetActionNameFromMessageKey(context.Name) == "Confirm")
            {
                return MessageDisplayChoice.Discard();
            }
            if (GetActionNameFromMessageKey(context.Name) == "Interstitial")
            {
                return MessageDisplayChoice.Delay(5);
            }

            return MessageDisplayChoice.Show();
        });

        Leanplum.PrioritizeMessages((contexts, trigger) =>
        {
            Debug.Log($"PrioritizeMessages: {trigger}");
            if (contexts.Length > 2)
            {
                return contexts.Take(2).ToArray();
            }

            return contexts;
        });

        Leanplum.OnMessageDisplayed((context) =>
        {
            Debug.Log($"OnMessageDisplayed: {context}");
        });

        Leanplum.OnMessageDismissed((context) =>
        {
            Debug.Log($"OnMessageDismissed: {context}");
        });

        Leanplum.OnMessageAction((action, context) =>
        {
            Debug.Log($"OnMessageAction: {action} for {context}");
        });

        StartCoroutine(Pause());
        StartCoroutine(Enable());

        Leanplum.Start();
    }

    IEnumerator Pause()
    {
        yield return new WaitForSeconds(5);
        Debug.Log($"SetActionManagerPaused  - pause");
        Leanplum.SetActionManagerPaused(true);
        yield return new WaitForSeconds(5);
        Debug.Log($"SetActionManagerPaused - unpause");
        Leanplum.SetActionManagerPaused(false);
    }

    IEnumerator Enable()
    {
        yield return new WaitForSeconds(20);
        Debug.Log($"SetActionManagerEnabled  - false");
        Leanplum.SetActionManagerEnabled(false);
        yield return new WaitForSeconds(10);
        Debug.Log($"SetActionManagerEnabled - true");
        Leanplum.SetActionManagerEnabled(true);
    }

    private string GetActionNameFromMessageKey(string key)
    {
        // {actionName:messageId}
        return key.Split(':')[0];
    }

    void inboxChanged()
    {
        Debug.Log("Inbox changed delegate called");
    }

    void forceContentUpdate(bool success)
    {
        Debug.Log("Inbox forceContentUpdate delegate called: " + success);
    }
}