#if (!UNITY_IOS && !UNITY_ANDROID && !UNITY_WEBGL) || UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite4Unity3d;
using UnityEngine;

namespace CleverTapSDK.Native
{
    public class SQLiteDataService : IDataService
    {
        private SQLiteConnection _connection;

        public SQLiteDataService(string databaseName)
        {
            string fileExt = ".db";
            string dbPath = Path.Combine(Application.persistentDataPath, databaseName + fileExt);
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }

        public void CreateTable<T>()
        {
            _connection.CreateTable<T>();
        }

        public int Insert<T>(T entry)
        {
            _connection.Insert(entry);
            return GetLastInsertedEntry();
        }

        public void Delete<T>(int id)
        {
            _connection.Delete<T>(id);
        }

        private int GetLastInsertedEntry()
        {
            return _connection.ExecuteScalar<int>("SELECT last_insert_rowid()");
        }

        public List<T> GetAllEntries<T>() where T : class, new()
        {
            return _connection.Table<T>().ToList();
        }
    }
}
#endif
