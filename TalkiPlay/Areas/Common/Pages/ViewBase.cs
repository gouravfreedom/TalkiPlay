using System;
using ReactiveUI.XamForms;

namespace TalkiPlay
{
    public class ViewBase<T> : ReactiveContentView<T> where T: class
    {
        public ViewBase()
        {
        }
    }

    public class TabViewBase<T> : ViewBase<T> where T : class
    {
        public TabViewBase()
        {
        }

        protected bool IsAppearedOnce { get; set; } = false;
        public virtual void AboutToAppear() { IsAppearedOnce = true; }
        public virtual void AboutToDisappear() { }
    }
}
