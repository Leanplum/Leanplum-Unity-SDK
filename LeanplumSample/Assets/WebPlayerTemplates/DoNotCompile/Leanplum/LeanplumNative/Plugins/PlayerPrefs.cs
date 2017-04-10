/*
	PreviewLabs.PlayerPrefs. Modified for Leanplum.

	Public Domain

	To the extent possible under law, PreviewLabs has waived all copyright and related or neighboring rights to this document. This work is published from: Belgium.

	http://www.previewlabs.com

*/

using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;

namespace LeanplumSDK.Prefs
{
    internal static class PlayerPrefs
	{
		private static Hashtable playerPrefsHashtable = new Hashtable();

		private static bool hashTableChanged = false;
		private static string serializedOutput = "";
		private static string serializedInput = "";
        private static Timer flushTimer;
        private static int flushFrequencyInSeconds = 300;

		private const string PARAMETERS_SEPERATOR = ";";
		private const string KEY_VALUE_SEPERATOR = ":";

		private static readonly string fileName = Application.persistentDataPath + "/LeanplumPrefs.txt";

		static PlayerPrefs()
		{
			//load previous settings
			StreamReader fileReader = null;

			if (File.Exists(fileName))
			{
				fileReader = new StreamReader(fileName);

				serializedInput = fileReader.ReadLine();

				Deserialize();

				fileReader.Close();

                flushTimer = new Timer(flushFrequencyInSeconds * 1000);
                flushTimer.Elapsed += delegate { Flush(); };
                flushTimer.Start();
			}
		}

		public static bool HasKey(string key)
		{
			return playerPrefsHashtable.ContainsKey(key);
		}

		public static void SetString(string key, string value)
		{
			playerPrefsHashtable[key] = value;
			hashTableChanged = true;
		}

		public static void SetInt(string key, int value)
		{
			playerPrefsHashtable[key] = value;
			hashTableChanged = true;
		}

		public static void SetFloat(string key, float value)
		{
			playerPrefsHashtable[key] = value;
			hashTableChanged = true;
		}

		public static void SetBool(string key, bool value)
		{
			playerPrefsHashtable[key] = value;
			hashTableChanged = true;
		}

		public static string GetString(string key)
		{
			if (playerPrefsHashtable[key] != null)
			{
				return playerPrefsHashtable[key].ToString();
			}
			return null;
		}

		public static string GetString(string key, string defaultValue)
		{
			if (playerPrefsHashtable[key] != null)
			{
				return playerPrefsHashtable[key].ToString();
			}
			else
			{
				playerPrefsHashtable[key] = defaultValue;
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public static int GetInt(string key)
		{
			if (playerPrefsHashtable[key] != null)
			{
				return (int) playerPrefsHashtable[key];
			}
			return 0;
		}

		public static int GetInt(string key, int defaultValue)
		{
			if (playerPrefsHashtable[key] != null)
			{
				return (int) playerPrefsHashtable[key];
			}
			else
			{
				playerPrefsHashtable[key] = defaultValue;
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public static float GetFloat(string key)
		{
			if (playerPrefsHashtable[key] != null)
			{
				return (float) playerPrefsHashtable[key];
			}
			return 0.0f;
		}

		public static float GetFloat(string key, float defaultValue)
		{
			if (playerPrefsHashtable[key] != null)
			{
				return (float) playerPrefsHashtable[key];
			}
			else
			{
				playerPrefsHashtable[key] = defaultValue;
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public static bool GetBool(string key)
		{
			if (playerPrefsHashtable[key] != null)
			{
				return (bool) playerPrefsHashtable[key];
			}
			return false;
		}

		public static bool GetBool(string key, bool defaultValue)
		{
			if (playerPrefsHashtable[key] != null)
			{
				return (bool) playerPrefsHashtable[key];
			}
			else
			{
				playerPrefsHashtable[key] = defaultValue;
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public static void DeleteKey(string key)
		{
			playerPrefsHashtable.Remove(key);
		}

		public static void DeleteAll()
		{
			playerPrefsHashtable.Clear();
		}

		public static void Flush()
		{
			if (hashTableChanged)
			{
				try {
					hashTableChanged = false;
					Serialize();

					StreamWriter fileWriter = null;
					fileWriter = File.CreateText(fileName);

					if (fileWriter == null)
					{
						Debug.LogWarning("PlayerPrefs::Flush() opening file for writing failed: " + fileName);
					}

					fileWriter.WriteLine(serializedOutput);

					fileWriter.Close();

					serializedOutput = "";
				} catch (Exception e) {
					Debug.LogError("Error when trying to save preferences.");
					Debug.LogException(e);
				}
			}
		}

		private static void Serialize()
		{
			IDictionaryEnumerator myEnumerator = playerPrefsHashtable.GetEnumerator();

			while ( myEnumerator.MoveNext() )
			{
                if (myEnumerator != null && myEnumerator.Key != null && myEnumerator.Value != null)
                {
    				if(serializedOutput != "")
    				{
    					serializedOutput += " " + PARAMETERS_SEPERATOR + " ";
    				}
    				serializedOutput += EscapeNonSeperators(myEnumerator.Key.ToString()) + " " + KEY_VALUE_SEPERATOR + " " + EscapeNonSeperators(myEnumerator.Value.ToString()) + " " + KEY_VALUE_SEPERATOR + " " + myEnumerator.Value.GetType();
                }
			}
		}

		private static void Deserialize()
		{
            if (String.IsNullOrEmpty(serializedInput))
            {
                return;
            }

			string[] parameters = serializedInput.Split(new string[] {" " + PARAMETERS_SEPERATOR + " "}, StringSplitOptions.None);

			foreach(string parameter in parameters)
			{
				string[] parameterContent = parameter.Split(new string[]{" " + KEY_VALUE_SEPERATOR + " "}, StringSplitOptions.None);

				if (parameterContent.Length >= 3)
				{
					string key = DeEscapeNonSeperators(parameterContent[0]);
					playerPrefsHashtable[key] = GetTypeValue(parameterContent[2], DeEscapeNonSeperators(parameterContent[1]));
				}
				if (parameterContent.Length != 3)
				{
					Debug.LogWarning("PlayerPrefs::Deserialize() parameterContent has " + parameterContent.Length + " elements");
				}
			}
		}

		private static string EscapeNonSeperators(string inputToEscape)
		{
			inputToEscape = inputToEscape.Replace(KEY_VALUE_SEPERATOR,"\\" + KEY_VALUE_SEPERATOR);
			inputToEscape = inputToEscape.Replace(PARAMETERS_SEPERATOR,"\\" + PARAMETERS_SEPERATOR);
			return inputToEscape;
		}

		private static string DeEscapeNonSeperators(string inputToDeEscape)
		{
			inputToDeEscape = inputToDeEscape.Replace("\\" + KEY_VALUE_SEPERATOR, KEY_VALUE_SEPERATOR);
			inputToDeEscape = inputToDeEscape.Replace("\\" + PARAMETERS_SEPERATOR, PARAMETERS_SEPERATOR);
			return inputToDeEscape;
		}

		public static object GetTypeValue(string typeName, string value)
		{
			if (typeName == "System.String")
			{
				return (object)value.ToString();
			}
			if (typeName == "System.Int32")
			{
				return (object)System.Convert.ToInt32(value);
			}
			if (typeName == "System.Boolean")
			{
				return (object)System.Convert.ToBoolean(value);
			}
			if (typeName == "System.Single")// -> single = float
			{
				return (object)System.Convert.ToSingle(value);
			}
			else
			{
				Debug.LogError("Unsupported type: " + typeName);
			}

			return null;
		}
	}
}