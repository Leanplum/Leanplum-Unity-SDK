#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using static CleverTapSDK.Native.UnityNativeConstants;

namespace CleverTapSDK.Native
{
    internal static class CustomTemplateUtils
    {
        internal static Dictionary<string, object> GetSyncTemplatesPayload(HashSet<CustomTemplate> templates)
        {
            var payload = new Dictionary<string, object>();
            payload["type"] = "templatePayload";

            var definitions = new Dictionary<string, object>();

            foreach (var template in templates)
            {
                var templateData = new Dictionary<string, object>();
                templateData["type"] = template.TemplateType;

                var groupedMap = new Dictionary<string, List<TemplateArgument>>();
                foreach (var arg in template.Arguments)
                {
                    var components = arg.Name.Split(CustomTemplates.DOT);
                    var firstComponent = components[0];
                    if (!groupedMap.ContainsKey(firstComponent))
                    {
                        groupedMap[firstComponent] = new List<TemplateArgument>();
                    }
                    groupedMap[firstComponent].Add(arg);
                }

                var ordered = new HashSet<string>();
                int order = 0;
                var arguments = new Dictionary<string, object>();

                foreach (var arg in template.Arguments)
                {
                    if (!arg.Name.Contains(CustomTemplates.DOT))
                    {
                        var argument = ArgumentPayload(arg, order);
                        arguments[arg.Name] = argument;
                        order++;
                    }
                    else
                    {
                        order = AddGroupArguments(groupedMap, ordered, order, arguments, arg);
                    }
                }

                templateData["vars"] = arguments;
                definitions[template.Name] = templateData;
            }

            payload["definitions"] = definitions;
            return payload;
        }

        private static int AddGroupArguments(Dictionary<string, List<TemplateArgument>> groupedMap, HashSet<string> ordered, int order, Dictionary<string, object> arguments, TemplateArgument arg)
        {
            var prefix = arg.Name.Split(CustomTemplates.DOT)[0];
            if (!ordered.Contains(prefix))
            {
                ordered.Add(prefix);
                var groupedArguments = groupedMap[prefix];
                groupedArguments.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

                foreach (var nestedArg in groupedArguments)
                {
                    var argument = ArgumentPayload(nestedArg, order);
                    arguments[nestedArg.Name] = argument;
                    order++;
                }
            }

            return order;
        }

        private static IDictionary<string, object> ArgumentPayload(TemplateArgument arg, int order)
        {
            var argument = new Dictionary<string, object>();
            if (arg.Value != null)
            {
                argument["defaultValue"] = arg.Value;
            }

            argument["type"] = arg.Type;
            argument["order"] = order;
            return argument;
        }
    }
}
#endif