#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CleverTapSDK.Utilities;
using UnityEngine;

namespace CleverTapSDK.Native
{

    public class WebGLDataService : IDataService
    {
        [DllImport("__Internal")]
        private static extern void Init(string dbName);

        [DllImport("__Internal")]
        private static extern void SetCurrentTable(string tableName);

        [DllImport("__Internal")]
        private static extern string Insert(string jsonEntry);

        [DllImport("__Internal")]
        private static extern string Delete(int id);

        [DllImport("__Internal")]
        private static extern string GetAllEntries();

        public WebGLDataService(string databaseName)
        {
            Init(databaseName);
        }

        public void CreateTable<T>()
        {
            string tableName = typeof(T).Name;
            SetCurrentTable(tableName);
        }

        public int Insert<T>(T entry)
        {
            try
            {
                string json = JsonUtility.ToJson(entry);
                CleverTapLogger.Log("Serialized JSON: " + json);
                string result = Insert(json);
                CleverTapLogger.Log("Insert result: " + result);
                return int.TryParse(result, out int id) ? id : -1;
            }catch(Exception e)
            {
                CleverTapLogger.LogError("Error while inserting data in db. " + e.StackTrace);
                return -1;
            }
        }

        public void Delete<T>(int id)
        {
            Delete(id);
        }

        public List<T> GetAllEntries<T>() where T : class, new()
        {
            try
            {
                string result = GetAllEntries();
                CleverTapLogger.Log("DB Cache Data: " + result);

                if (string.IsNullOrEmpty(result))
                {
                    CleverTapLogger.Log("DB Cache is Empty");
                    return new List<T>();
                }

                // Deserialize the JSON string into a dictionary
                var rawDict = Json.Deserialize(result) as Dictionary<string, object>;
                if (rawDict == null)
                {
                    CleverTapLogger.Log("Failed to parse JSON into dictionary.");
                    return new List<T>();
                }

                List<T> entries = new List<T>();

                foreach (var kvp in rawDict)
                {
                    // Convert each dictionary entry to JSON string and then to object T
                    string jsonString = Json.Serialize(kvp.Value);
                    T entry = JsonUtility.FromJson<T>(jsonString);
                    entries.Add(entry);
                }

                CleverTapLogger.Log($"DB Cache has {entries.Count} Entries");
                return entries;
            }
            catch (Exception e)
            {
                CleverTapLogger.LogError("Error while fetching data from db. " + e.Message + "\n" + e.StackTrace);
                return new List<T>();
            }
        }

    }

}
#endif
