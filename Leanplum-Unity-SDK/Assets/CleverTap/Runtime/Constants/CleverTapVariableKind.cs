namespace CleverTapSDK.Constants
{
    /// <summary>
    /// CleverTap Variable Kind based on the Variable type and default value.
    /// Used when creating the Variable.
    /// Used in Unity Native and Mobile Native Plugins.
    /// </summary>
    internal static class CleverTapVariableKind
    {
        internal const string INT = "integer";
        internal const string FLOAT = "float";
        internal const string STRING = "string";
        internal const string BOOLEAN = "bool";
        internal const string DICTIONARY = "group";
        internal const string FILE = "file";
    }
}
