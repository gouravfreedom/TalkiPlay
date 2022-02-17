using ChilliSource.Mobile.UI.ReactiveUI;

namespace TalkiPlay
{
    public class ReactiveBaseViewCell<T> : ReactiveBaseCell<T> where T : class
    {
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            this.ViewModel = (T) this.BindingContext;
        }
    }
}