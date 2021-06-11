using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace LeanplumSDK
{
    public class EditorMessageTemplates
    {
        private EditorMessageTemplates()
        {
        }

        public static void DefineConfirm()
        {
            ActionArgs actionArgs = new ActionArgs();
            actionArgs.With<string>(Constants.Args.MESSAGE, "Confirm message");
            actionArgs.With<string>(Constants.Args.ACCEPT_TEXT, "Accept");
            actionArgs.With<string>(Constants.Args.CANCEL_TEXT, "Cancel");
            
            actionArgs.WithAction<object>(Constants.Args.ACCEPT_ACTION, null)
                .WithAction<object>(Constants.Args.CANCEL_ACTION, null);

            ActionContext.ActionResponder responder = new ActionContext.ActionResponder((context) =>
            {
                if (EditorUtility.DisplayDialog(Constants.Args.CONFIRM_NAME,
                context.GetStringNamed(Constants.Args.MESSAGE),
                context.GetStringNamed(Constants.Args.ACCEPT_TEXT),
                context.GetStringNamed(Constants.Args.CANCEL_TEXT)))
                {
                    context.RunTrackedActionNamed(Constants.Args.ACCEPT_ACTION);
                }
                else
                {
                    context.RunActionNamed(Constants.Args.CANCEL_ACTION);
                }
            });

            Leanplum.DefineAction(Constants.Args.CONFIRM_NAME, Constants.ActionKind.MESSAGE, actionArgs, null, responder);
        }

        public static void DefineOpenURL()
        {
            ActionArgs actionArgs = new ActionArgs();
            actionArgs.With<string>(Constants.Args.URL, "https://www.example.com");

            ActionContext.ActionResponder responder = new ActionContext.ActionResponder((context) =>
            {
                string url = context.GetStringNamed(Constants.Args.URL);
                if (!string.IsNullOrEmpty(url))
                {
                    Application.OpenURL(url);
                }
            });

            Leanplum.DefineAction(Constants.Args.OPEN_URL, Constants.ActionKind.ACTION, actionArgs, null, responder);
        }

        public static void DefineGenericDefinition()
        {
            string configVars = $"{Constants.Args.GENERIC_DEFINITION_CONFIG}.vars";
            ActionArgs args = new ActionArgs()
                .With<object>(Constants.Args.GENERIC_DEFINITION_CONFIG, null)
                .With<Dictionary<string, object>>(configVars, null);

            ActionContext.ActionResponder responder = new ActionContext.ActionResponder((context) =>
            {
                var messageConfig = context.GetObjectNamed<object>(Constants.Args.GENERIC_DEFINITION_CONFIG);
                var messageVars = context.GetObjectNamed<Dictionary<string, object>>(configVars);
                StringBuilder builder = new StringBuilder();
                NativeActionContext nativeContext = context as NativeActionContext;
                if (nativeContext != null && !string.IsNullOrEmpty(nativeContext.Id))
                {
                    builder.AppendLine($"Message Id: {nativeContext.Id}");
                }
                BuildString("message",
                    messageConfig, builder, 0);

                EditorUtility.DisplayDialog(context.Name, builder.ToString(), null);
            });

            Leanplum.DefineAction(Constants.Args.GENERIC_DEFINITION_NAME, Constants.ActionKind.MESSAGE, args, null, responder);
        }

        static void BuildString(string key, object var, StringBuilder builder, int level)
        {
            if (var == null)
            {
                return;
            }

            if (var is IDictionary)
            {
                builder.AppendLine($"{IndentString(level)}{key}:");
                var varDict = var as IDictionary<string, object>;
                foreach (string keyDict in varDict.Keys)
                {
                    BuildString(keyDict, varDict[keyDict], builder, ++level);
                    level--;
                }
            }
            else if (var is IList)
            {
                builder.AppendLine($"{IndentString(level)}{key}:");
                var varList = var as IList<object>;
                for (int i = 0; i < varList.Count; i++)
                {
                    BuildString($"[{i}]", varList[i], builder, ++level);
                    level--;
                }
            }
            else
            {
                if (var is string)
                {
                    var = $"\"{var}\"";
                }
                builder.AppendLine($"{IndentString(level)}{key}: {var}");
            }
        }

        static string IndentString(int tabs)
        {
            return new string(' ', tabs * 4);
        }
    }
}
#endif