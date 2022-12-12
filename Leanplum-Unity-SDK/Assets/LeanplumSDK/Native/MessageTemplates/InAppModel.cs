using System;

namespace LeanplumSDK
{
    public class InAppModel
    {
        public ActionContext Context { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        internal Action OnAccept { get; set; }
        internal Action OnCancel { get; set; }

        public bool HasCancelButton { get; set; } = true;

        public void Show()
        {
            InAppPanel.Create(this);
        }
    }
}