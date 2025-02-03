#if (!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace CleverTapSDK.Native {
    internal class UnityNativeSessionManager {
        private const long SESSION_LENGTH_SECONDS = 20 * 60;
        
        private UnityNativeSession _currentSession;

        private string _accountId;

        internal UnityNativeSessionManager(string accountId) {
            _accountId = accountId;
            _currentSession = new UnityNativeSession(_accountId);
        }

        public UnityNativeSession CurrentSession {
            get {
                if (IsSessionExpired()) {
                    ResetSession();
                }

                return _currentSession;
            }
        }

        internal void ResetSession() {
            _currentSession = new UnityNativeSession(_accountId);
        }

        internal bool IsFirstSession() {
            return _currentSession.IsFirstSession;
        }

        /// <summary>
        /// Used for Page events only.
        /// Increment when RecordScreenView is called.
        /// Not supported yet.
        /// </summary>
        /// <returns>The screens count.</returns>
        internal int GetScreenCount() {
            return 1;
        }

        /// <summary>
        /// The current screen name.
        /// Equivalent to the current Activity name on Android
        /// and the current ViewController on iOS.
        /// Not supported yet.
        /// </summary>
        /// <returns>The name of the current screen.</returns>
        internal string GetScreenName()
        {
            return string.Empty;
        }

        internal long GetLastSessionLength() {
            return _currentSession.LastSessionLength;
        }

        internal void UpdateSessionTimestamp() {
            if (IsSessionExpired()) {
                ResetSession();
            }

            _currentSession?.UpdateTimestamp();
        }

        private bool IsSessionExpired() {
            return _currentSession.GetNow() - _currentSession.LastUpdateTimestamp > SESSION_LENGTH_SECONDS;
        }
    }
}
#endif