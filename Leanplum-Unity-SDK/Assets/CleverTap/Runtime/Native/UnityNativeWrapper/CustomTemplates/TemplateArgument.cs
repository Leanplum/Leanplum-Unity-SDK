#if UNITY_EDITOR
namespace CleverTapSDK.Native
{
    internal static class TemplateArgumentType
    {
        internal static readonly string String = "string";
        internal static readonly string Number = "number";
        internal static readonly string Bool = "boolean";
        internal static readonly string File = "file";
        internal static readonly string Action = "action";
    }

    internal class TemplateArgument
    {
        internal string Name { get; }
        internal object Value { get; }
        internal string Type { get; }

        internal TemplateArgument(string name, string type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}
#endif