using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using TalkiPlay;

[assembly: ExportRenderer(typeof(PressableView), typeof(PressableViewRenderer))]

namespace TalkiPlay
{
    public class PressableViewRenderer : ViewRenderer<PressableView, UIView>
    {
        public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            Element.OnPressed(true);
        }

        public override void TouchesCancelled(Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            Element.OnPressed(false);
        }

        public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            Element.OnPressed(false);
        }
    }
}