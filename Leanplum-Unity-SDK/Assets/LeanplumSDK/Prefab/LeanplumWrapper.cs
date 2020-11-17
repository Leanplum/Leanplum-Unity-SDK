//
// Copyright 2014, Leanplum, Inc.
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
using LeanplumSDK;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LeanplumWrapper : MonoBehaviour
{
    public string AppID;
    public string ProductionKey;
    public string DevelopmentKey;
    public string AppVersion;


    // TODO: Define basic defitions for Alert and Confirm using DisplayDialog
    // TODO: See if definition for HTML is needed
    // TODO: Enable includeDefaults: true in Dev Mode
    // TODO: Add maybePerformActions for Start and Track
    // TODO: Add models for Message Config and WhenTriggers
    // TODO: Add Open URL +
    // TODO: For others add open button to message +


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

        DefineCustomConfirm();
        DefineCustomOpenURL();
        DefineGenericDefinition();

        Leanplum.Start();

        Leanplum.Started += (succes) =>
        {
            Leanplum.Track("priority");
        };
    }

    void inboxChanged()
    {
        Debug.Log("Inbox changed delegate called");
    }

    void forceContentUpdate(bool success)
    {
        Debug.Log("Inbox forceContentUpdate delegate called: " + success);
    }

    void ShowMessage()
    {
        Leanplum.ShowMessage("6359747062202368");
    }

    void DefineCustomConfirm()
    {
        ActionArgs actionArgs = new ActionArgs();
        actionArgs.With<string>("Message", "Confirm message");
        actionArgs.With<string>("Accept text", "Accept");
        actionArgs.With<string>("Cancel text", "Cancel");


        actionArgs.With<int>("Test", 200);

        actionArgs.WithAction<object>("Accept action", null)
            .WithAction<object>("Cancel action", null);

        ActionContext.ActionResponder responder = new ActionContext.ActionResponder((context) =>
        {
            Debug.Log(context);
            Debug.Log(context.GetStringNamed("Message"));
            Debug.Log(context.GetNumberNamed<int>("Test"));

            if (EditorUtility.DisplayDialog("Confirm",
            context.GetStringNamed("Message"), context.GetStringNamed("Accept text"), context.GetStringNamed("Cancel text")))
            {
                Debug.Log("YES");
                context.RunTrackedActionNamed("Accept action");
            }
            else
            {
                Debug.Log("NO");
                context.RunActionNamed("Cancel action");
            }
        });

        Leanplum.DefineAction("Confirm", Constants.ActionKind.MESSAGE, actionArgs, null, responder);
    }

    void DefineCustomOpenURL()
    {
        ActionArgs actionArgs = new ActionArgs();
        actionArgs.With<string>("URL", "https://www.example.com");

        ActionContext.ActionResponder responder = new ActionContext.ActionResponder((context) =>
        {
            string url = context.GetStringNamed("URL");

            Application.OpenURL(url);
        });

        Leanplum.DefineAction("Open URL", Constants.ActionKind.ACTION, actionArgs, null, responder);
    }

    void DefineGenericDefinition()
    {
        ActionArgs args = new ActionArgs()
            .With<IDictionary<string, object>>("messageConfig", null)
            .With<IDictionary<string, object>>("messageConfig.vars", null);

        ActionContext.ActionResponder responder = new ActionContext.ActionResponder((context) =>
        {
            var messageConfig = context.GetObjectNamed<Dictionary<string, object>>("messageConfig");
            var messageVars = context.GetObjectNamed<Dictionary<string, object>>("messageConfig.vars");
            StringBuilder builder = new StringBuilder();
            LeanplumNative.BuildString("message", messageConfig, builder, 0);

            EditorUtility.DisplayDialog(context.Name, builder.ToString(), null);
        });

        Leanplum.DefineAction("Generic", Constants.ActionKind.MESSAGE, args, null, responder);
    }
}
