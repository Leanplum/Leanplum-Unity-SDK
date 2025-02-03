using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleverTapSDK.Common
{
    public class MonoHelper : MonoBehaviour
    {
        private static MonoHelper _instance;
        public static MonoHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("MonoHelper");
                    _instance = obj.AddComponent<MonoHelper>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }

        private SynchronizationContext _context;

        private void Awake()
        {
            _instance = this;
            _context = SynchronizationContext.Current;
        }

        public Task RunOnMainThread(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();
            _context.Post(_ =>
            {
                action();
                tcs.SetResult(true);
            }, null);
            return tcs.Task;
        }

        public Task<T> RunOnMainThread<T>(Func<T> function)
        {
            var tcs = new TaskCompletionSource<T>();
            _context.Post(_ =>
            {
                var result = function();
                tcs.SetResult(result);
            }, null);
            return tcs.Task;
        }
    }
}