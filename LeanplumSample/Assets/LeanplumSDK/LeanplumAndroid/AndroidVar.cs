// Copyright 2014, Leanplum, Inc.

#if UNITY_ANDROID

using UnityEngine;
using LeanplumSDK.MiniJSON;
using System.Collections;
using System.Runtime.CompilerServices;
using System;
using System.Linq;

namespace LeanplumSDK
{
	/// <summary>
	///     Leanplum variable.
	/// </summary>
	/// <T>
	///     Type of the variable.
	///     Can be Boolean, Byte, Short, Integer, Long, Float, Double, Character, String,
	///     List, Dictionary, or Unity's AssetBundle. You may nest lists and dictionaries arbitrarily. </param>
	internal class AndroidVar<T> : Var<T>
	{
		private bool registeredCallbackInAndroid = false;
		protected bool valueHasChanged = false;
		private VariableCallback valueChanged;
		private T currentValue;
		private T defaultValue;
		
		internal AndroidVar(string name, string kind, T DefaultValue,  string filename = "")
		{
			Name = name;
			Kind = kind;
			FileName = filename;
			currentValue = DefaultValue;
			defaultValue = DefaultValue;
		}
		
		public override event VariableCallback ValueChanged
		{
			add
			{
				valueChanged += value;
				if (!registeredCallbackInAndroid)
				{
					registeredCallbackInAndroid = true;
					LeanplumAndroid.NativeSDK.CallStatic("registerVarCallback", Name);
				}
				if (valueHasChanged)
				{
					value();
				}
			}
			remove
			{
				valueChanged -= value;
			}
		}
		
		public override void OnValueChanged()
		{
			if (valueChanged != null)
			{
				valueChanged();
			}
			valueHasChanged = true;
		}
		
		public override string[] NameComponents
		{
			get
			{
				string jsonRepresentation = LeanplumAndroid.NativeSDK.CallStatic<string>("varNameComponents", Name);
				string[] result = new string[jsonRepresentation.Count(x => x == ',') + 1];
				SharedUtil.FillInValues(Json.Deserialize(jsonRepresentation), result);
				return result;
			}
		}

		public override T Value
		{ 
			get
			{
				if (Kind == Constants.Kinds.FILE)
				{
					string file = Json.Deserialize(LeanplumAndroid.NativeSDK.CallStatic<string>("fileValue", Name)) as string;
					if (file != FileName)
					{
						FileName = file;
					}
					return (T) Convert.ChangeType(AssetBundle.LoadFromFile(FileName), typeof(T));
				}
				else
				{
					string jsonRepresentation = LeanplumAndroid.NativeSDK.CallStatic<string>("varValue", Name);
					if (jsonRepresentation == Json.Serialize(currentValue))
					{
						return currentValue;
					}

					object newValue = Json.Deserialize(jsonRepresentation);
					if (newValue is IDictionary || newValue is IList)
					{
						SharedUtil.FillInValues(newValue, currentValue);
					}
					else if (newValue == null)
					{
						currentValue = defaultValue;
					}
					else
					{
						currentValue = (T) Convert.ChangeType(newValue, typeof(T));
					}
					return currentValue;
				}
			}
		}

		public override object GetDefaultValue()
		{
			return defaultValue;
		}
	}
}

#endif
