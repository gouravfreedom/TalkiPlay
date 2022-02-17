using System.Threading.Tasks;
using ReactiveUI;
using Rg.Plugins.Popup.Interfaces.Animations;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class BasePopupPage : PopupPage
    {
        protected BasePopupPage(bool useDefaultAnimation = true)
        {
            if (useDefaultAnimation)
            {
                Animation = new BaseModalPopoverPageAnimation();
            }
        }
    }
    
    public class BasePopupPage<T> : BasePopupPage, IViewFor<T> where T : class
    {
        public BasePopupPage(bool useDefaultAnimation = true) : base(useDefaultAnimation)
        {
          
        }
    
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (T) value;
        }

        public T ViewModel { get; set; }
    }


    public class BaseModalPopoverPageAnimation : IPopupAnimation
    {
        public async Task Appearing(View content, PopupPage page)
        {
            content.Opacity = 0;
            await content.FadeTo(1);
        }

        public async Task Disappearing(View content, PopupPage page)
        {
            await content.FadeTo(0);
        }

        public void Disposing(View content, PopupPage page)
        {
            //
        }

        public void Preparing(View content, PopupPage page)
        {
            //
        }
    }

    public class FromTopRevealAnimation : IPopupAnimation
    {
        public async Task Appearing(View content, PopupPage page)
        {
            content.TranslationY = content.Height;
            await content.TranslateTo(0, 0, length: 400, easing: Easing.SinOut);
        }

        public async Task Disappearing(View content, PopupPage page)
        {
            await content.TranslateTo(0, content.Height, length: 400, easing: Easing.SinIn);
        }

        public void Disposing(View content, PopupPage page)
        {
            //
        }

        public void Preparing(View content, PopupPage page)
        {
            //
        }
    }

}



