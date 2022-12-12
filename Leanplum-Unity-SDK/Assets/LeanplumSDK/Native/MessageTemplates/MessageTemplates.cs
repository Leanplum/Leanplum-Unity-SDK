using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace LeanplumSDK
{
    public class MessageTemplates
    {
        private MessageTemplates()
        {
        }

        public static void DefineAlert()
        {
            ActionArgs actionArgs = new ActionArgs();
            actionArgs.With(Constants.Args.TITLE, "App name");
            actionArgs.With(Constants.Args.MESSAGE, "Alert message goes here.");
            actionArgs.With(Constants.Args.DISMISS_TEXT, "OK");

            actionArgs.WithAction<object>(Constants.Args.DISMISS_ACTION, null);

            ActionContext.ActionResponder responder = new ActionContext.ActionResponder((actionContext) =>
            {
                var model = new InAppModel();
                model.Title = actionContext.GetStringNamed(Constants.Args.TITLE);
                model.Message = actionContext.GetStringNamed(Constants.Args.MESSAGE);
                model.OnAccept += () =>
                {
                    actionContext.RunActionNamed(Constants.Args.DISMISS_ACTION);
                };
                model.HasCancelButton = false;
                model.Context = actionContext;
                model.Show();
            });

            Leanplum.DefineAction(Constants.Args.ALERT_NAME, Constants.ActionKind.MESSAGE, actionArgs, null, responder);
        }

        public static void DefineConfirm()
        {
            ActionArgs actionArgs = new ActionArgs();
            actionArgs.With(Constants.Args.MESSAGE, "Confirm message");
            actionArgs.With(Constants.Args.ACCEPT_TEXT, "Accept");
            actionArgs.With(Constants.Args.CANCEL_TEXT, "Cancel");
            
            actionArgs.WithAction<object>(Constants.Args.ACCEPT_ACTION, null)
                .WithAction<object>(Constants.Args.CANCEL_ACTION, null);

            ActionContext.ActionResponder responder = new ActionContext.ActionResponder((actionContext) =>
            {
                var model = new InAppModel();
                model.Title = actionContext.GetStringNamed(Constants.Args.TITLE);
                model.Message = actionContext.GetStringNamed(Constants.Args.MESSAGE);
                model.Context = actionContext;
                model.OnAccept += () =>
                {
                    actionContext.RunTrackedActionNamed(Constants.Args.ACCEPT_ACTION);
                };
                model.OnCancel += () =>
                {
                    actionContext.RunActionNamed(Constants.Args.CANCEL_ACTION);
                };

                model.Show();
            });

            Leanplum.DefineAction(Constants.Args.CONFIRM_NAME, Constants.ActionKind.MESSAGE, actionArgs, null, responder);
        }

        public static void DefineCenterPopup()
        {
            // TODO: Add fields
            ActionArgs actionArgs = new ActionArgs();
            actionArgs.With(Constants.Args.TITLE_TEXT, "App name");
            actionArgs.With(Constants.Args.MESSAGE_TEXT, "Alert message goes here.");
            actionArgs.With(Constants.Args.ACCEPT_BUTTON_TEXT, "OK");

            actionArgs.WithAction<object>(Constants.Args.ACCEPT_ACTION, null);

            ActionContext.ActionResponder responder = new ActionContext.ActionResponder((actionContext) =>
            {
                var model = new InAppModel();
                model.Title = actionContext.GetStringNamed(Constants.Args.TITLE_TEXT);
                model.Message = actionContext.GetStringNamed(Constants.Args.MESSAGE_TEXT);
                model.OnAccept = () =>
                {
                    actionContext.RunTrackedActionNamed(Constants.Args.ACCEPT_ACTION);
                };
                model.HasCancelButton = false;
                model.Context = actionContext;
                model.Show();
            });

            Leanplum.DefineAction(Constants.Args.CENTER_POPUP, Constants.ActionKind.MESSAGE, actionArgs, null, responder);
        }

        public static void DefineOpenURL()
        {
            ActionArgs actionArgs = new ActionArgs();
            actionArgs.With(Constants.Args.URL, "https://www.example.com");

            ActionContext.ActionResponder responder = new ActionContext.ActionResponder((context) =>
            {
                string url = context.GetStringNamed(Constants.Args.URL);
                if (!string.IsNullOrEmpty(url))
                {
                    Application.OpenURL(url);
                }
                context.Dismissed();
            });

            Leanplum.DefineAction(Constants.Args.OPEN_URL, Constants.ActionKind.ACTION, actionArgs, null, responder);
        }

        public static void DefineGenericDefinition()
        {
            string configVars = $"{Constants.Args.GENERIC_DEFINITION_CONFIG}.vars";
            ActionArgs args = new ActionArgs()
                .With<object>(Constants.Args.GENERIC_DEFINITION_CONFIG, null)
                .With<object>(configVars, null);

            ActionContext.ActionResponder responder = new ActionContext.ActionResponder((actionContext) =>
            {
                var messageConfig = actionContext.GetObjectNamed<object>(Constants.Args.GENERIC_DEFINITION_CONFIG);
                var messageVars = actionContext.GetObjectNamed<object>(configVars);
                var actionName = actionContext.GetObjectNamed<string>($"{Constants.Args.GENERIC_DEFINITION_CONFIG}.action");

                StringBuilder builder = new StringBuilder();
                builder.AppendLine($"Message Id: {actionContext.Id}");
                BuildString("message", messageConfig, builder, 0);

                var model = new InAppModel();
                model.Title = actionName;
                model.Message = builder.ToString();
                model.HasCancelButton = false;
                model.Context = actionContext;
                model.Show();
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
                if (varDict != null)
                {
                    foreach (string keyDict in varDict.Keys)
                    {
                        BuildString(keyDict, varDict[keyDict], builder, ++level);
                        level--;
                    }
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