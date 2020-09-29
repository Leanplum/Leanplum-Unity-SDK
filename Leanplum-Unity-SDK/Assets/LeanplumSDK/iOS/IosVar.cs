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
#if UNITY_IPHONE

using UnityEngine;
using LeanplumSDK.MiniJSON;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;

namespace LeanplumSDK
{
	/// <summary>
	///     Leanplum variable.
	/// </summary>
	/// <T>
	///     Type of the variable.
	///     Can be Boolean, Byte, Short, Integer, Long, Float, Double, Character, String,
	///     List, Dictionary, or Unity's AssetBundle. You may nest lists and dictionaries arbitrarily. </param>
	public class IOSVar<T> : Var<T>
	{
		private VariableCallback valueChanged;
		private T _defaultValue;
		private T _val;
		
		public IOSVar(string name, string kind, T defaultValue,  string filename = "")
		{
			Name = name;
			Kind = kind;
			FileName = filename;
			_val = _defaultValue = defaultValue;
			LeanplumIOS.IOSVarCache[name] = this;
		}

		public override event VariableCallback ValueChanged
		{
			add
			{
				valueChanged += value;
			}
			remove
			{
				valueChanged -= value;
			}
		}

		public override void OnValueChanged()
		{
			if (valueChanged != null)
				valueChanged();
		}

		public override object GetDefaultValue()
		{
			return _defaultValue;
		}

		public override T Value
		{ 
			get
			{
				string jsonRepresentation = LeanplumIOS._getVariableValue(Name, Kind);

				if (jsonRepresentation == null)
				{
					return _defaultValue;
				}

				if (jsonRepresentation == Json.Serialize(_val))
				{
					return _val;
				}

				if (Kind == Constants.Kinds.FILE)
				{
					if (jsonRepresentation != FileName)
					{
						FileName = jsonRepresentation;
					}
					
					return (T) Convert.ChangeType(AssetBundle.LoadFromFile(FileName), typeof(T));
				}

				object newValue = Json.Deserialize(jsonRepresentation);

				if (newValue is IDictionary || newValue is IList)
				{
					SharedUtil.FillInValues(newValue, _val);
				}
				else
				{
					_val = (T) Convert.ChangeType(newValue, typeof(T));
				}
				
				return _val;
			}
		}
	}
}

#endif
