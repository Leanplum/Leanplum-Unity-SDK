using CleverTapSDK.Constants;
using CleverTapSDK.Utilities;

namespace CleverTapSDK.Common
{
    internal interface IVar
    {
        string Name { get; }
        string Kind { get; }
        void ValueChanged();
        void FileIsReady();
        bool IsFileReady { get; }

        object DefaultObjectValue { get; }
        string[] NameComponents { get; }
        void Update();
        void Reset();
    }

    public abstract class Var<T> : IVar
    {
        protected string name;
        protected string kind;
        protected T value;
        protected T defaultValue;
        protected bool isFileReady;

        public virtual event CleverTapCallbackDelegate OnValueChanged;
        public virtual event CleverTapCallbackDelegate OnFileReady;

        public Var(string name, string kind, T defaultValue)
        {
            this.name = name;
            this.kind = kind;
            this.defaultValue = value = defaultValue;
        }

        public virtual string Kind => kind;
        public virtual string Name => name;
        public virtual T Value => value;
        public virtual T DefaultValue => defaultValue;

        public virtual object DefaultObjectValue => DefaultValue;
        public virtual string[] NameComponents => new string[] { Name };

        public virtual string StringValue
        {
            get
            {
                if (Kind == CleverTapVariableKind.DICTIONARY)
                {
                    return Json.Serialize(Value);
                }
                else
                {
                    return Value?.ToString();
                }
            }
        }

        public virtual bool IsFileReady => Kind == CleverTapVariableKind.FILE && isFileReady;
        public string FileValue => Kind == CleverTapVariableKind.FILE ? StringValue : null;

        public virtual void ValueChanged()
        {
            OnValueChanged?.Invoke();
        }

        public virtual void FileIsReady()
        {
            if (!Kind.Equals(CleverTapVariableKind.FILE))
            {
                CleverTapLogger.Log($"Var \"{name}\": FileIsReady is only available for File Variables.");
                return;
            }

            isFileReady = true;
            OnFileReady?.Invoke();
        }

        public virtual void Update()
        {
        }

        public virtual void Reset()
        {
        }

        public static implicit operator T(Var<T> var)
        {
            return var.Value;
        }
    }
}
