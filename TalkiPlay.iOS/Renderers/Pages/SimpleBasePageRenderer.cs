using TalkiPlay;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SimpleBasePage), typeof(SimpleBasePageRenderer))]

namespace TalkiPlay
{
    public class SimpleBasePageRenderer : PageRenderer
    {
        SimpleBasePage Page => Element as SimpleBasePage;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (Page != null)
            {
                Page.RaiseViewDidLoadEvent();
            }
        }
    }
}
