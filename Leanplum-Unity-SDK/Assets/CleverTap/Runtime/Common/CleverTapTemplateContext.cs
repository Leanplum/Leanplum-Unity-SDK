using System.Collections.Generic;

namespace CleverTapSDK.Common
{
    /// <summary>
    /// Representation of the context around an invocation of a custom template. Use the <c>Get</c> methods to obtain the
    /// current values of the arguments. Use <c>SetPresented</c> and <c>SetDismissed</c> to notify the SDK of the current state of
    /// this invocation.
    /// </summary>
    public abstract class CleverTapTemplateContext
    {
        public readonly string TemplateName;
        public CleverTapTemplateContext(string templateName)
        {
            TemplateName = templateName;
        }

        /// <summary>
        /// Notify the SDK that the current template is presented.
        /// </summary>
        public abstract void SetPresented();

        /// <summary>
        /// Notify the SDK that the current template is dismissed. The current template is considered to be
        /// visible to the user until this method is called. Since the SDK can show only one InApp message at a time,
        /// all other messages will be queued until the current one is dismissed.
        /// </summary>
        public abstract void SetDismissed();

        /// <summary>
        /// Trigger an action argument by name.
        /// </summary>
        /// <param name="name">The action argument name</param>
        public abstract void TriggerAction(string name);

        /// <summary>
        /// Retrieve a <c>string</c> argument by name.
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <returns>The argument value or <c>null</c> if no such argument is defined for the template.</returns>
        public abstract string GetString(string name);

        /// <summary>
        /// Retrieve a <c>bool</c> argument by name.
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <returns>The argument value or <c>null</c> if no such argument is defined for the template.</returns>
        public abstract bool? GetBoolean(string name);

        /// <summary>
        /// Retrieve a <c>Dictionary</c> of all arguments under <paramref name="name"/>. Dictionary arguments will be combined
        /// with dot notation arguments. All values are converted to their defined type in the template. Action arguments
        /// are mapped to their name as a <c>string</c>
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <returns>The combined Dictionary or <c>null</c> if no arguments are found matching the provided <paramref name="name"/> for the template.</returns>
        public abstract Dictionary<string, object> GetDictionary(string name);

        /// <summary>
        /// Retrieve an absolute file path argument by name.
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <returns>The argument value or <c>null</c> if no such argument is defined for the template</returns>
        public abstract string GetFile(string name);

        /// <summary>
        /// Retrieve a <c>byte</c> argument by name.
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <returns>The argument value or <c>null</c> if no such argument is defined for the template.</returns>
        public abstract byte? GetByte(string name);

        /// <summary>
        /// Retrieve a <c>short</c> argument by name.
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <returns>The argument value or <c>null</c> if no such argument is defined for the template.</returns>
        public abstract short? GetShort(string name);

        /// <summary>
        /// Retrieve an <c>int</c> argument by name.
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <returns>The argument value or <c>null</c> if no such argument is defined for the template.</returns>
        public abstract int? GetInt(string name);

        /// <summary>
        /// Retrieve a <c>long</c> argument by name.
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <returns>The argument value or <c>null</c> if no such argument is defined for the template.</returns>
        public abstract long? GetLong(string name);

        /// <summary>
        /// Retrieve a <c>float</c> argument by name.
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <returns>The argument value or <c>null</c> if no such argument is defined for the template.</returns>
        public abstract float? GetFloat(string name);

        /// <summary>
        /// Retrieve a <c>double</c> argument by name.
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <returns>The argument value or <c>null</c> if no such argument is defined for the template.</returns>
        public abstract double? GetDouble(string name);

        internal abstract string GetTemplateString();

        /// <summary>
        /// Get a string representation of this template context with information about all arguments.
        /// </summary>
        public override string ToString()
        {
            return GetTemplateString();
        }
    }
}
