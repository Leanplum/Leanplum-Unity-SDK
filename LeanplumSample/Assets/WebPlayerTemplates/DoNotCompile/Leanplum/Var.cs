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
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LeanplumSDK
{
	public abstract class Var
	{
		private static readonly Regex nameComponentPattern = new Regex("(?:[^\\.\\[.(\\\\]+|\\\\.)+");

		// Spurious warning. This is used in the Var<T> subclasses.
		#pragma warning disable 0067
		public virtual event VariableCallback ValueChanged;
		#pragma warning restore 0067
        
		public delegate void VariableCallback();

		/// <summary>
		///     Defines a new variable with a default value. If a Leanplum variable with the same name
		///     and type exists, this will return the existing variable.
		/// </summary>
		/// <param name="name"> Name of the variable. </param>
		/// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
		public static Var<int> Define(string name, int defaultValue) 
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}
		
		/// <summary>
		///     Defines a new variable with a default value. If a Leanplum variable with the same name
		///     and type exists, this will return the existing variable.
		/// </summary>
		/// <param name="name"> Name of the variable. </param>
		/// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
		public static Var<long> Define(string name, long defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}
		
		/// <summary>
		///     Defines a new variable with a default value. If a Leanplum variable with the same name
		///     and type exists, this will return the existing variable.
		/// </summary>
		/// <param name="name"> Name of the variable. </param>
		/// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
		public static Var<short> Define(string name, short defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}
		
		/// <summary>
		///     Defines a new variable with a default value. If a Leanplum variable with the same name
		///     and type exists, this will return the existing variable.
		/// </summary>
		/// <param name="name"> Name of the variable. </param>
		/// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
		public static Var<byte> Define(string name, byte defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}
		
		/// <summary>
		///     Defines a new variable with a default value. If a Leanplum variable with the same name
		///     and type exists, this will return the existing variable.
		/// </summary>
		/// <param name="name"> Name of the variable. </param>
		/// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
		public static Var<bool> Define(string name, bool defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}
		
		/// <summary>
		///     Defines a new variable with a default value. If a Leanplum variable with the same name
		///     and type exists, this will return the existing variable.
		/// </summary>
		/// <param name="name"> Name of the variable. </param>
		/// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
		public static Var<float> Define(string name, float defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}
		
		/// <summary>
		///     Defines a new variable with a default value. If a Leanplum variable with the same name
		///     and type exists, this will return the existing variable.
		/// </summary>
		/// <param name="name"> Name of the variable. </param>
		/// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
		public static Var<double> Define(string name, double defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}
		
		/// <summary>
		///     Defines a new variable with a default value. If a Leanplum variable with the same name
		///     and type exists, this will return the existing variable.
		/// </summary>
		/// <param name="name"> Name of the variable. </param>
		/// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
		public static Var<string> Define(string name, string defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}
		
		/// <summary>
		///     Defines a new variable with a default value. If a Leanplum variable with the same name
		///     and type exists, this will return the existing variable.
		/// </summary>
		/// <param name="name"> Name of the variable. </param>
		/// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
		public static Var<List<object>> Define(string name, List<object> defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}
		
		/// <summary>
		///     Defines a new variable with a default value. If a Leanplum variable with the same name
		///     and type exists, this will return the existing variable.
		/// </summary>
		/// <param name="name"> Name of the variable. </param>
		/// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
		public static Var<List<string>> Define(string name, List<string> defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}
		
		/// <summary>
		///     Defines a new variable with a default value. If a Leanplum variable with the same name
		///     and type exists, this will return the existing variable.
		/// </summary>
		/// <param name="name"> Name of the variable. </param>
		/// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
		public static Var<Dictionary<string, object>> Define(string name, Dictionary<string, object> defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}
		
		/// <summary>
		///     Defines a new variable with a default value. If a Leanplum variable with the same name
		///     and type exists, this will return the existing variable.
		/// </summary>
		/// <param name="name"> Name of the variable. </param>
		/// <param name="defaultValue"> Default value of the variable. Can't be null. </param>
		public static Var<Dictionary<string, string>> Define(string name, Dictionary<string, string> defaultValue)
		{
			return LeanplumFactory.SDK.Define(name, defaultValue);
		}
		
		/// <summary>
		///     Defines an asset bundle. If a Leanplum variable with the same name and type exists, this will
		///     return the existing variable.
		/// </summary>
		/// <returns>Leanplum variable.</returns>
		/// <param name="name">Name of variable.</param>
        /// <param name="realtimeUpdating">Setting it to <c>false</c> will prevent Leanplum from reloading
        ///     assetbundles as they change in development mode.</param>
        /// <param name="iosBundleName">Filename of iOS assetbundle.</param>
        /// <param name="androidBundleName">Filename of Android assetbundle.</param>
        /// <param name="standaloneBundleName">Filename of Standalone assetbundle.</param>
        public static Var<AssetBundle> DefineAssetBundle(string name, bool realtimeUpdating = true,
                                                         string iosBundleName = "", string androidBundleName = "",
                                                         string standaloneBundleName = "")
        {
			return LeanplumFactory.SDK.DefineAssetBundle(name, realtimeUpdating, iosBundleName, androidBundleName, standaloneBundleName);
        }

		/// <summary>
		///     Gets the kind of the variable as a string.
		/// </summary>
		/// <value>The kind of the variable.</value>
		public string Kind { get; protected set; }

		/// <summary>
		///     Gets the name of the variable as a string.
		/// </summary>
		/// <value>The name of the variable.</value>
		public string Name { get; protected set; }

		/// <summary>
		///     Gets the current filename if the value is an assetbundle.
		/// </summary>
		/// <value>The name of the file.</value>
		public string FileName { get; protected set; }

		/// <summary>
		///     Gets the variable name components as an array of strings.
		/// </summary>
		/// <value>The name components.</value>
		public virtual string[] NameComponents 
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

		public abstract void OnValueChanged();

		public abstract object GetDefaultValue();

		/// <summary>
		///			Used internally. Do not call this.
		/// </summary>
		public virtual void Update() {}
	}

	/// <summary>
	///     Leanplum variable.
	/// </summary>
	/// <T>
	///     Type of the variable.
	///     Can be Boolean, Byte, Short, Integer, Long, Float, Double, Character, String,
	///     List, Dictionary, or Unity's AssetBundle. You may nest lists and dictionaries arbitrarily. </param>
	public abstract class Var<T> : Var
	{
		public abstract T Value { get; }

		public static implicit operator T(Var<T> var)
		{
			return var.Value;
		}
	}
}
