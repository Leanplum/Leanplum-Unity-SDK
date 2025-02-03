namespace CleverTapSDK.Common {
    public delegate void CleverTapCallbackDelegate();

    public delegate void CleverTapCallbackWithMessageDelegate(string message);

    public delegate void CleverTapCallbackWithTemplateContext(CleverTapTemplateContext context);

    public delegate void CleverTapCallback<T>(T value);
}
