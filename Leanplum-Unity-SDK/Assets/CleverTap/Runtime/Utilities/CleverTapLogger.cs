using UnityEngine;

namespace CleverTapSDK.Utilities {
    public enum LogLevel {
        None,
        Error,
        Debug,
    }

    internal static class CleverTapLogger {
        private static LogLevel Level = LogLevel.Debug;

        internal static void SetLogLevel(LogLevel level) {
            Level = level;
        }

        internal static void Log(string message) {
            if (Level >= LogLevel.Debug) {
                Debug.Log(message);
            }
        }

        internal static void LogError(string message) {
            if (Level >= LogLevel.Error) {
                Debug.LogError(message);
            }
        }
    }
}
