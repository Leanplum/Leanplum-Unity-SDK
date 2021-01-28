using System.Collections.Generic;

namespace LeanplumSDK
{
    public abstract class ActionContext
    {
        public delegate void ActionResponder(ActionContext context);

        /// <summary>
        /// Name of the action
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Tracks an event in the context of the current message.
        /// </summary>
        /// <param name="eventName">event name</param>
        /// <param name="value">event value</param>
        /// <param name="param">event params</param>
        public abstract void Track(string eventName, double value, IDictionary<string, object> param);

        /// <summary>
        /// Tracks an event in the conext of the current message, with any parent actions prepended to the message event name.
        /// </summary>
        /// <param name="eventName">event name</param>
        /// <param name="value">event value</param>
        /// <param name="info">event info</param>
        /// <param name="param">event params</param>
        public abstract void TrackMessageEvent(string eventName, double value, string info, IDictionary<string, object> param);

        /// <summary>
        /// Runs the action given by the "name" key.
        /// </summary>
        /// <param name="name">action name</param>
        public abstract void RunActionNamed(string name);

        /// <summary>
        /// Runs and tracks an event for the action given by the "name" key.
        /// This will track an event if no action is set.
        /// </summary>
        /// <param name="name">action name</param>
        public abstract void RunTrackedActionNamed(string name);

        /// <summary>
        /// Get string for name
        /// </summary>
        /// <param name="name">name of the string</param>
        /// <returns>found string or null</returns>
        public abstract string GetStringNamed(string name);

        /// <summary>
        /// Get bool for name 
        /// </summary>
        /// <param name="name">name of the bool</param>
        /// <returns>found bool or null</returns>
        public abstract bool? GetBooleanNamed(string name);

        /// <summary>
        /// Get object for name
        /// </summary>
        /// <param name="name">name of the object</param>
        /// <typeparam name="T">Dictionary or List</typeparam>
        /// <returns>found object or default</returns>
        public abstract T GetObjectNamed<T>(string name);

        /// <summary>
        /// Get UnityEngine Color for name
        /// </summary>
        /// <param name="name">name of the color</param>
        /// <returns>found Color or default Color</returns>
        public abstract UnityEngine.Color GetColorNamed(string name);

        /// <summary>
        /// Get number for name
        /// </summary>
        /// <param name="name">name of the number</param>
        /// <typeparam name="T">int, double, float, byte, char</typeparam>
        /// <returns>found object or default</returns>
        public abstract T GetNumberNamed<T>(string name);

        /// <summary>
        /// Prevents the currently active message from appearing again in the future.
        /// </summary>
        public abstract void MuteForFutureMessagesOfSameKind();
    }
}