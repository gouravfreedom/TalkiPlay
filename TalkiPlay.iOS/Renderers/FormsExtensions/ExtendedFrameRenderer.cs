using System.ComponentModel;
using CoreGraphics;
using TalkiPlay;
using TalkiPlay.Shared;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedFrame), typeof(ExtendedFrameRenderer))]

namespace TalkiPlay
{
    public class ExtendedFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (Element == null)
            {
                return;
            }

            SetBorder();
            UpdateShadow();
        }

        public ExtendedFrame ExtendedFrame => Element as ExtendedFrame;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (ExtendedFrame == null)
            {
                return;
            }

            if (e.PropertyName == ExtendedFrame.BorderWidthProperty.PropertyName ||
                e.PropertyName == ExtendedFrame.BorderColorProperty.PropertyName)
            {
                SetBorder();
            }
            else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName ||
                e.PropertyName == VisualElement.IsEnabledProperty.PropertyName ||
                e.PropertyName == ExtendedFrame.ShadowOffsetProperty.PropertyName ||
                e.PropertyName == ExtendedFrame.ShadowRadiusProperty.PropertyName ||
                e.PropertyName == ExtendedFrame.ShadowOpacityProperty.PropertyName)
            {
                UpdateShadow();
            }
        }

        private void UpdateShadow()
        {
            Layer.ShadowOpacity = (float)ExtendedFrame.ShadowOpacity;
            Layer.ShadowOffset = ExtendedFrame.ShadowOffset.ToSizeF();
            Layer.ShadowRadius = (float)ExtendedFrame.ShadowRadius;
        }

        void SetBorder()
        {
            Layer.BorderWidth = (System.nfloat)ExtendedFrame.BorderWidth;
            Layer.BorderColor = ExtendedFrame.BorderColor.ToCGColor();
            Layer.CornerRadius = ExtendedFrame.CornerRadius;
        }
    }
}