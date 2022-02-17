using Android.Content;
using TalkiPlay.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ContentPage), typeof(BasePageRenderer))]
namespace TalkiPlay.Android
{
    public class BasePageRenderer : PageRenderer
    {
        private IBasePageController BasePageController => Element as IBasePageController;
        private IPageController Controller => Element as IPageController;
        private ContentPage Page => Element as ContentPage;

        public BasePageRenderer(Context context) : base(context)
        {
      
        }
        
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (!BasePageController?.IsAppearing ?? false)
            {
                Controller.SendAppearing();
            }

            BasePageController?.OnAppeared();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();

            if (BasePageController?.IsAppearing ?? false)
            {
                Controller.SendDisappearing();
            }

        }
    }
}