using Android.Views;
using TalkiPlay;
using TalkiPlay.Droid;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly:ExportEffect(typeof(NativeBorderlessEffect), nameof(BorderlessEffect))]

namespace TalkiPlay.Droid
{
    public class NativeBorderlessEffect : PlatformEffect
    {
        private View NativeView => Control ?? Container;
      
        protected override void OnAttached()
        {
           // var layoutParams = new ViewGroup.MarginLayoutParams(Control.LayoutParameters);
          //  layoutParams.SetMargins(0, 0, 0, 0);
          //  NativeView.LayoutParameters = layoutParams;
          //  Control.LayoutParameters = layoutParams;
            // Control.SetPadding(30, 50, 30, 0);
            // NativeView.Background = null;
            Control?.SetBackgroundColor(Color.Transparent.ToAndroid());
        }

        protected override void OnDetached()
        {
          
        }
    }
}