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

using LeanplumSDK.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LeanplumSDK
{
    /// <summary>
    ///     Leanplum variable.
    /// </summary>
    /// <T>
    ///     Type of the variable.
    ///     Can be Boolean, Byte, Short, Integer, Long, Float, Double, Character, String,
    ///     List, Dictionary, or Unity's AssetBundle. You may nest lists and dictionaries arbitrarily. </param>
    internal class NativeVar<T> : Var<T>
    {
        private static readonly Regex nameComponentPattern = new Regex("(?:[^\\.\\[.(\\\\]+|\\\\.)+");
        private VariableCallback valueChanged;
        public object defaultClonedContainer;
        internal T _defaultValue;
        private T _value;
        internal bool fileReady;
        internal bool realtimeAssetUpdating;
        internal string currentlyDownloadingFile;

        internal NativeVar(string name, string kind, T defaultValue, string filename = "")
        {
            Name = name;
            Kind = kind;
            FileName = filename;
            _value = _defaultValue = defaultValue;
        }

        public void SetFilename(string fileName)
        {
            FileName = fileName;
        }

        public override event VariableCallback ValueChanged
        {
            add
            {
                valueChanged += value;
                if (Leanplum.HasStarted && this.fileReady)
                {
                    value();
                }
            }

            remove { valueChanged -= value; }
        }

        public override void OnValueChanged()
        {
            VariableCallback handler = valueChanged;
            if (handler != null) handler();
        }

        internal virtual void ClearValueChangedCallbacks()
        {
            valueChanged = null;
        }

        public override void Update()
        {
            object newValue = VarCache.GetMergedValueFromComponentArray(NameComponents);
            if (newValue == null)
            {
                newValue = GetDefaultValue();
            }

            if (Kind == Constants.Kinds.FILE)
            {
                if (VarCache.IsSilent)
                {
                    return;
                }

                string newFile = newValue.ToString();
                if (string.IsNullOrEmpty(newFile))
                {
                    return;
                }

                string url = null;
                if (VarCache.FileAttributes != null && VarCache.FileAttributes.ContainsKey(newFile))
                {
                    IDictionary<string, object> currentFile =
                        (VarCache.FileAttributes[newFile] as IDictionary<string, object>)
                        [string.Empty] as IDictionary<string, object>;
                    if (currentFile.ContainsKey(Constants.Keys.URL))
                    {
                        url = GetResourceURL(newFile);
                    }
                }

                // Download new file if the file is different than:
                // - the current file for the varible
                // - the file currently being downloaded for the variable
                // - there is no file value and realtime updating is enabled
                // Do not download and update if realtime updating is disabled
                // Wait until the file is downloaded from the server
                if (currentlyDownloadingFile != newFile && !string.IsNullOrEmpty(url) &&
                    ((newFile != FileName && realtimeAssetUpdating && fileReady) ||
                     (Value == null && realtimeAssetUpdating)))
                {
                    currentlyDownloadingFile = newFile;
                    FileName = newFile;
                    fileReady = false;

                    void OnFileResponse(object file)
                    {
                        _value = (T)file;
                        if (newFile == FileName && !fileReady)
                        {
                            fileReady = true;
                            OnValueChanged();
                            currentlyDownloadingFile = null;
                        }
                    }

                    void OnFileError(Exception ex)
                    {
                        if (newFile == FileName && !fileReady)
                        {
                            string errorMessage = $"Error downloading AssetBundle \"{Name}\" with \"{FileName}\". {ex}";
                            LeanplumNative.CompatibilityLayer.LogError(errorMessage);
                            currentlyDownloadingFile = null;
                        }
                    }

                    Leanplum.FileTransferManager.DownloadAssetResource(url, OnFileResponse, OnFileError);
                }
            }
            else
            {
                if (Json.Serialize(newValue) != Json.Serialize(Value))
                {
                    try
                    {
                        if (newValue is IDictionary || newValue is IList)
                        {
                            // If the value is a container, copy all the values from the newValue container to Value. 
                            Util.FillInValues(newValue, Value);
                        }
                        else
                        {
                            _value = (T) Convert.ChangeType(newValue, typeof(T));
                        }

                        if (VarCache.IsSilent)
                        {
                            return;
                        }

                        OnValueChanged();
                    }
                    catch (Exception ex)
                    {
                        Util.MaybeThrow(new LeanplumException("Error parsing values from server. " + ex.ToString()));
                        return;
                    }
                }
            }
        }


        /* 
         * "example.jpg": {
         *      "": {
         *          "size": 41802,
         *          "hash": null,
         *          "servingUrl": "http://lh3.googleusercontent.com/PymI6X...",
         *          "url": "/resource/AMIfv94zoleE43w_3PLB02..."
         *      }
         *  }
         */
        private string GetResourceURL(string fileName)
        {
            if (VarCache.FileAttributes.ContainsKey(fileName))
            {
                var fileAttributes = VarCache.FileAttributes[fileName] as IDictionary<string, object>;
                if (fileAttributes != null)
                {
                    var fileData = Util.GetValueOrDefault(fileAttributes, string.Empty) as IDictionary<string, object>;
                    var url = Util.GetValueOrDefault(fileData, Constants.Keys.URL) as string;
                    if (!string.IsNullOrEmpty(url) && url.StartsWith("/"))
                    {
                        return url.Substring(1);
                    }
                    return url;
                }
            }

            return null;
        }

        public override object GetDefaultValue()
        {
            if (_defaultValue is IDictionary || _defaultValue is IList)
            {
                return defaultClonedContainer;
            }

            if (FileName != null && FileName != String.Empty)
                return FileName;

            return _defaultValue;
//			return FileName as System.Object ?? defaultValue;
        }

        public override string[] NameComponents
        {
            get
            {
                var matches = nameComponentPattern.Matches(Name);
                string[] components = new string[matches.Count];
                int i = 0;
                foreach (var match in matches)
                {
                    components[i] = match.ToString();
                    i++;
                }

                return components;
            }
        }

        public override T Value
        {
            get { return _value; }
        }

        public static implicit operator T(NativeVar<T> var)
        {
            return var.Value;
        }
    }
}