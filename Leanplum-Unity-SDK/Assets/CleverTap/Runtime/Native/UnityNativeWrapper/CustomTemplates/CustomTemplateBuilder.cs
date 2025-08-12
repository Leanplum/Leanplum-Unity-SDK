#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using static CleverTapSDK.Native.UnityNativeConstants;

namespace CleverTapSDK.Native
{
    internal abstract class CustomTemplateBuilder
    {
        protected string name;
        protected string templateType;
        protected bool isVisual;
        protected List<TemplateArgument> arguments = new List<TemplateArgument>();

        protected HashSet<string> argumentNames = new HashSet<string>();
        protected HashSet<string> parentArgumentNames = new HashSet<string>();

        internal CustomTemplateBuilder(string templateType, bool isVisual)
        {
            this.templateType = templateType;
            this.isVisual = isVisual;
        }

        internal void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new CleverTapTemplateException("CleverTap: Template Name cannot be null, empty or white space.");
            }

            if (!string.IsNullOrWhiteSpace(this.name))
            {
                throw new CleverTapTemplateException("CleverTap: Template Name already set.");
            }

            this.name = name;
        }

        internal void AddDictionaryArgument(string name, Dictionary<string, object> dictionaryValue)
        {
            if (dictionaryValue == null || dictionaryValue.Count == 0)
            {
                throw new CleverTapTemplateException($"CleverTap: Dictionary value for argument \"{name}\" cannot be null or empty.");
            }

            foreach (var kv in dictionaryValue)
            {
                string argName = $"{name}{CustomTemplates.DOT}{kv.Key}";
                var value = kv.Value;

                if (value is string)
                {
                    AddArgument(argName, TemplateArgumentType.String, value);
                }
                else if (value is bool)
                {
                    AddArgument(argName, TemplateArgumentType.Bool, value);
                }
                else if (value is float || value is double || value is int || value is long || value is short || value is byte)
                {
                    AddArgument(argName, TemplateArgumentType.Number, value);
                }
                else if (value is Dictionary<string, object> dict)
                {
                    AddDictionaryArgument(argName, dict);
                }
                else
                {
                    throw new CleverTapTemplateException($"CleverTap: Unsupported value type \"{value?.GetType()}\" for argument \"{argName}\".");
                }
            }
        }

        internal void AddArgument(string name, string type, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new CleverTapTemplateException("CleverTap: Argument name cannot be null, empty or white space.");
            }

            if (name.StartsWith(CustomTemplates.DOT) || name.EndsWith(CustomTemplates.DOT))
            {
                throw new CleverTapTemplateException($"CleverTap: Argument name cannot start or end with \"{CustomTemplates.DOT}\"." +
                    $" Argument name: \"{name}\".");
            }

            if (argumentNames.Contains(name))
            {
                throw new CleverTapTemplateException($"CleverTap: Argument with name \"{name}\" already added.");
            }

            ValidateParentNames(name);
            argumentNames.Add(name);
            arguments.Add(new TemplateArgument(name, type, value));
        }

        internal void AddFileArgument(string key)
        {
            AddArgument(key, TemplateArgumentType.File, null);
        }

        private void ValidateParentNames(string name)
        {
            var nameComponents = name.Split(CustomTemplates.DOT, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder parentNameBuilder = new StringBuilder();
            for (int i = 0; i < nameComponents.Length - 1; i++)
            {
                if (i > 0)
                {
                    parentNameBuilder.Append(CustomTemplates.DOT);
                }

                var component = nameComponents[i];
                parentNameBuilder.Append(component);

                string parentName = parentNameBuilder.ToString();
                if (argumentNames.Contains(parentName))
                {
                    throw new CleverTapTemplateException($"CleverTap: Argument with name \"{parentName}\" already added.");
                }

                parentArgumentNames.Add(parentName);
            }

            if (parentArgumentNames.Contains(name))
            {
                throw new CleverTapTemplateException($"CleverTap: Argument with name \"{name}\" already added.");
            }
        }

        internal virtual CustomTemplate Build()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new CleverTapTemplateException("CleverTap: CustomTemplate must have a name.");
            }

            return new CustomTemplate(name, templateType, isVisual, arguments);
        }
    }

    internal class InAppTemplateBuilder : CustomTemplateBuilder
    {
        internal InAppTemplateBuilder() : base(CustomTemplates.TEMPLATE_TYPE, true)
        {
        }

        internal void AddActionArgument(string name)
        {
            AddArgument(name, TemplateArgumentType.Action, null);
        }
    }

    internal class AppFunctionBuilder : CustomTemplateBuilder
    {
        internal AppFunctionBuilder(bool isVisual) : base(CustomTemplates.FUNCTION_TYPE, isVisual)
        {
        }
    }
}
#endif