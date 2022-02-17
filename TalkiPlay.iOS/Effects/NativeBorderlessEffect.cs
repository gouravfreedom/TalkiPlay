using CoreGraphics;
using TalkiPlay;
using TalkiPlay.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportEffect(typeof(NativeBorderlessEffect), nameof(BorderlessEffect))]
namespace TalkiPlay.iOS
{
    public class NativeBorderlessEffect : PlatformEffect
    {
        private UIView NativeView => Control ?? Container;
        
        protected override void OnAttached()
        {
            NativeView.Layer.BorderWidth = 0;
        }

        protected override void OnDetached()
        {
           
        }
    }
}