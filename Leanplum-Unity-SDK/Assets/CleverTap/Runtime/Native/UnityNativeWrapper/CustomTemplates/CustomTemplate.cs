#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace CleverTapSDK.Native
{
    internal class CustomTemplate
    {
        internal string Name { get; }
        internal string TemplateType { get; }
        internal bool IsVisual { get; }
        internal List<TemplateArgument> Arguments { get; }

        internal CustomTemplate(string name, string templateType, bool isVisual, List<TemplateArgument> arguments)
        {
            Name = name;
            TemplateType = templateType;
            IsVisual = isVisual;
            Arguments = arguments;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(CustomTemplate))
                return false;

            var other = (CustomTemplate)obj;
            return string.Equals(Name, other.Name, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
    }
}
#endif