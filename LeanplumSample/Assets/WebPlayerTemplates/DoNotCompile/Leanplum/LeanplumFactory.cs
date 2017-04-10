// Copyright 2014, Leanplum, Inc.

using UnityEngine;
using System.Collections;

namespace LeanplumSDK 
{
	public class LeanplumFactory
	{
		private static LeanplumSDKObject _sdk = null;

		public static LeanplumSDKObject SDK
		{
			get
			{
				return _sdk;
			}
			set
			{
				_sdk = value;
			}
		}
	}
}
