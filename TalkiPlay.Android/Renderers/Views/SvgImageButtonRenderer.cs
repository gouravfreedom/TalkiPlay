using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using TalkiPlay;
using TalkiPlay.Android;
using Xamarin.Forms;
using Color = Android.Graphics.Color;
using AView = Android.Views.View;

[assembly: ExportRenderer(typeof(SvgImageButton), typeof(SvgImageButtonRenderer))]
namespace TalkiPlay.Android
{
    public class SvgImageButtonRenderer : ViewRenderer<SvgImageButton, AView>, AView.IOnTouchListener
    {
        private ISvgImageButtonController Controller => Element;
        public SvgImageButtonRenderer(Context context) : base(context) 
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SvgImageButton> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                this.SetOnTouchListener(this);
                
                if (Control == null)
                {
                    SetNativeControl(CreateNativeControl());
                }
            }

            if (e.OldElement != null)
            {
                this.SetOnTouchListener(null);
            }
        }

        protected override AView CreateNativeControl()
        {
            return new AView(Context);
        }

        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                Controller?.SendEnabled(value);
            }
        }

        public bool OnTouch(AView v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    Controller?.SendPressed();
                    break;
                case MotionEventActions.Cancel:
                   Controller.SendReleased();
                    break;
                case MotionEventActions.Up:
                    Controller?.SendReleased();
                    Controller?.SendClicked();
                    break;
            }
            return true;
        }
    }

}