#if UNITY_EDITOR
using System.Collections.Generic;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Native
{
    internal class JsonTemplateProducer
    {
        private static readonly string TemplateArgumentTypeObject = "object";

        private readonly string jsonTemplatesDefinition;

        internal JsonTemplateProducer(string jsonTemplatesDefinition)
        {
            this.jsonTemplatesDefinition = jsonTemplatesDefinition;
        }

        internal HashSet<CustomTemplate> DefineTemplates()
        {
            if (string.IsNullOrEmpty(jsonTemplatesDefinition))
            {
                throw new CleverTapTemplateException("CleverTap: JSON template definitions cannot be null or empty.");
            }

            var root = JSON.Parse(jsonTemplatesDefinition);
            if (root == null || root.AsObject == null)
            {
                throw new CleverTapTemplateException("CleverTap: Error parsing JSON template definitions.");
            }

            var templates = new HashSet<CustomTemplate>();

            foreach (KeyValuePair<string, JSONNode> keyValue in root.AsObject)
            {
                string key = keyValue.Key;
                var item = keyValue.Value;

                string type = item["type"];
                var arguments = item["arguments"];

                CustomTemplateBuilder builder;

                if (type == UnityNativeConstants.CustomTemplates.TEMPLATE_TYPE)
                {
                    builder = new InAppTemplateBuilder();
                }
                else if (type == UnityNativeConstants.CustomTemplates.FUNCTION_TYPE)
                {
                    bool isVisual = item["isVisual"] != null && item["isVisual"].AsBool;
                    builder = new AppFunctionBuilder(isVisual);
                }
                else
                {
                    throw new CleverTapTemplateException($"Unknown template type: {type} for template: {key}.");
                }

                builder.SetName(key);
                AddJsonArguments(arguments.AsObject, builder);
                templates.Add(builder.Build());
            }

            return templates;
        }

        private void AddJsonArguments(JSONNode arguments, CustomTemplateBuilder builder)
        {
            foreach (KeyValuePair<string, JSONNode> argKV in arguments.AsObject)
            {
                string argKey = argKV.Key;
                var arg = argKV.Value;
                string argType = arg["type"];
                var value = arg["value"];

                if (argType == TemplateArgumentTypeObject)
                {
                    var dictValue = ObjectArgumentJsonToDictionary(value.AsObject);
                    builder.AddDictionaryArgument(argKey, dictValue);
                    continue;
                }

                if (argType == TemplateArgumentType.String)
                {
                    builder.AddArgument(argKey, TemplateArgumentType.String, value.Value);
                }
                else if (argType == TemplateArgumentType.Number)
                {
                    builder.AddArgument(argKey, TemplateArgumentType.Number, value.AsDouble);
                }
                else if (argType == TemplateArgumentType.Bool)
                {
                    builder.AddArgument(argKey, TemplateArgumentType.Bool, value.AsBool);
                }
                else if (argType == TemplateArgumentType.File)
                {
                    if (value != null && value.Value != "")
                        throw new CleverTapTemplateException($"CleverTap: File arguments should not specify a value: {argKey}");

                    builder.AddFileArgument(argKey);
                }
                else if (argType == TemplateArgumentType.Action)
                {
                    if (value != null && value.Value != "")
                        throw new CleverTapTemplateException($"CleverTap: Action arguments should not specify a value: {argKey}");

                    if (builder is not InAppTemplateBuilder inAppTemplateBuilder)
                        throw new CleverTapTemplateException($"Function templates cannot have action arguments. Argument: {argKey}");

                    inAppTemplateBuilder.AddActionArgument(argKey);
                }
                else
                {
                    throw new CleverTapTemplateException($"Unknown argument type: {argType} for argument: {argKey}");
                }
            }
        }

        private Dictionary<string, object> ObjectArgumentJsonToDictionary(JSONNode json)
        {
            var result = new Dictionary<string, object>();

            foreach (KeyValuePair<string, JSONNode> kv in json.AsObject)
            {
                string name = kv.Key;
                var arg = kv.Value;
                string type = arg["type"];
                var val = arg["value"];

                if (type == TemplateArgumentTypeObject)
                {
                    result[name] = ObjectArgumentJsonToDictionary(val.AsObject);
                }
                else if (type == TemplateArgumentType.String)
                {
                    result[name] = val.Value;
                }
                else if (type == TemplateArgumentType.Number)
                {
                    result[name] = val.AsDouble;
                }
                else if (type == TemplateArgumentType.Bool)
                {
                    result[name] = val.AsBool;
                }
                else
                {
                    throw new CleverTapTemplateException($"Unsupported nested argument type: {type} for argument: {name}");
                }
            }

            return result;
        }
    }
}
#endif