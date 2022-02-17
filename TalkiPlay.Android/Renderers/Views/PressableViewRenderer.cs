using System;
//using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ChilliSource.Mobile.UI;
using Android.Content;
using Android.Views;
using TalkiPlay;

[assembly: Xamarin.Forms.ExportRenderer(typeof(PressableView), typeof(PressableViewRenderer))]

namespace TalkiPlay
{
    public class PressableViewRenderer : ViewRenderer<PressableView, View>, View.IOnTouchListener
    {
        public PressableViewRenderer(Context context) : base(context)
        {
            SetOnTouchListener(this);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    Element.OnPressed(true);
                    break;
                case MotionEventActions.Cancel:
                    Element.OnPressed(false);
                    break;
                case MotionEventActions.Up:
                    Element.OnPressed(false);
                    break;
            }
            return true;
        }
    }
}