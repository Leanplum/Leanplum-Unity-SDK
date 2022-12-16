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
using System.Collections.Generic;

namespace LeanplumSDK
{
    public abstract class ActionContext
    {
        public delegate void ActionResponder(ActionContext context);
        public delegate void ActionDidDismiss(ActionContext context);

        internal delegate void ActionDidExecute(string actionName, ActionContext context);

        internal virtual event ActionDidDismiss Dismiss;
        internal virtual event ActionDidExecute ActionExecute;

        /// <summary>
        /// Id of the action
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// 
        /// </summary>
        internal virtual string Key { get; }

        /// <summary>
        /// Name of the action
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Tracks an event in the context of the current message.
        /// </summary>
        /// <param name="eventName">event name</param>
        /// <param name="value">event value</param>
        /// <param name="param">event params</param>
        public abstract void Track(string eventName, double value, IDictionary<string, object> param);

        /// <summary>
        /// Tracks an event in the conext of the current message, with any parent actions prepended to the message event name.
        /// </summary>
        /// <param name="eventName">event name</param>
        /// <param name="value">event value</param>
        /// <param name="info">event info</param>
        /// <param name="param">event params</param>
        public abstract void TrackMessageEvent(string eventName, double value, string info, IDictionary<string, object> param);

        /// <summary>
        /// Runs the action given by the "name" key.
        /// </summary>
        /// <param name="name">action name</param>
        public abstract void RunActionNamed(string name);

        /// <summary>
        /// Runs and tracks an event for the action given by the "name" key.
        /// This will track an event if no action is set.
        /// </summary>
        /// <param name="name">action name</param>
        public abstract void RunTrackedActionNamed(string name);

        /// <summary>
        /// Indicates the action has been dismissed.
        /// </summary>
        public abstract void Dismissed();

        /// <summary>
        /// Get string for name
        /// </summary>
        /// <param name="name">name of the string</param>
        /// <returns>found string or null</returns>
        public abstract string GetStringNamed(string name);

        /// <summary>
        /// Get bool for name 
        /// </summary>
        /// <param name="name">name of the bool</param>
        /// <returns>found bool or null</returns>
        public abstract bool? GetBooleanNamed(string name);

        /// <summary>
        /// Get object for name
        /// </summary>
        /// <param name="name">name of the object</param>
        /// <typeparam name="T">Dictionary or List</typeparam>
        /// <returns>found object or default</returns>
        public abstract T GetObjectNamed<T>(string name);

        /// <summary>
        /// Get UnityEngine Color for name
        /// </summary>
        /// <param name="name">name of the color</param>
        /// <returns>found Color or default Color</returns>
        public abstract UnityEngine.Color GetColorNamed(string name);

        /// <summary>
        /// Get file path for name.
        /// Returns URL on Unity. Returns file path on iOS and Android.
        /// </summary>
        /// <param name="name">name of the file argument</param>
        /// <returns></returns>
        public abstract string GetFile(string name);

        /// <summary>
        /// Get number for name
        /// </summary>
        /// <param name="name">name of the number</param>
        /// <typeparam name="T">int, double, float, byte, char</typeparam>
        /// <returns>found object or default</returns>
        public abstract T GetNumberNamed<T>(string name);
    }
}