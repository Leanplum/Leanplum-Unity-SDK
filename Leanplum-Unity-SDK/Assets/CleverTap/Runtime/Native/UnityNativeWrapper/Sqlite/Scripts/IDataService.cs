#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System.Collections.Generic;

namespace CleverTapSDK.Native
{
    public interface IDataService
    {
        void CreateTable<T>();
        int Insert<T>(T entry);
        void Delete<T>(int id);
        List<T> GetAllEntries<T>() where T : class, new();
    }

    public static class DataServiceFactory
    {
        public static IDataService CreateDataService(string databaseName)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return new WebGLDataService(databaseName);
#else
            return new SQLiteDataService(databaseName);
#endif
        }
    }
}
#endif



