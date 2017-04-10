// Copyright 2013, Leanplum, Inc.

using LeanplumSDK.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LeanplumSDK
{
	/// <summary>
	///     Leanplum utilities.
	/// </summary>
	public static class SharedUtil
	{
		/// <summary>
		///     Helper function to copy and cast values from containers of object types
		///     to containers of primitive types.
		/// </summary>
		/// <param name="source">Source container.</param>
		/// <param name="destination">Destination container.</param>
		public static void FillInValues(object source, object destination)
		{
			if (source == null || Json.Serialize(source) == Json.Serialize(destination))
			{
				return;
			}
			
			if (source is IDictionary)
			{
				if (destination is IDictionary)
				{
					bool cleared = false;
					foreach (object key in ((IDictionary) source).Keys)
					{
						object typedKey = Convert.ChangeType(key, destination.GetType().GetGenericArguments()[0]);
						if (((IDictionary) source)[key] is IDictionary ||
						    ((IDictionary) source)[key] is IList)
						{
							if (((IDictionary) destination).Contains(typedKey))
							{
								FillInValues(((IDictionary) source)[key],
								             ((IDictionary) destination)[typedKey]);
							}
						}
						else
						{
							if (!cleared)
							{
								cleared = true;
								((IDictionary) destination).Clear();
							}
							
							((IDictionary) destination)[typedKey] =
								Convert.ChangeType(((IDictionary) source)[key],
								                   destination.GetType().GetGenericArguments()[1]);
						}
					}
				}
				else if (destination is IList)
				{
					foreach (object varSubscript in ((IDictionary) source).Keys)
					{
						var strSubscript = (string) varSubscript;
						int subscript = Convert.ToInt32(strSubscript.Substring(1, strSubscript.Length - 1 - 1));
						FillInValues(((IDictionary) source)[varSubscript],
						             ((IList) destination)[subscript]);
					}
				}
			}
			else if (source is IList || source is Array)
			{
				int index = 0;
				var sourceList = (IList) source;
				//        foreach (object value in (IList) source)
				for (int sourceIndex = 0; sourceIndex < sourceList.Count; sourceIndex++)
				{
					object value = sourceList[sourceIndex];
					
					if (value is IDictionary || value is IList)
					{
						FillInValues(value, ((IList) destination)[index]);
					}
					else
					{
						((IList) destination)[index] =
							Convert.ChangeType(value,
							                   destination.GetType().IsArray ?
							                   destination.GetType().GetElementType() :
							                   destination.GetType().GetGenericArguments()[0]);
					}
					index++;
				}
			}
			else
			{
				destination = Convert.ChangeType(source, source.GetType());
			}
		}
	}
}
