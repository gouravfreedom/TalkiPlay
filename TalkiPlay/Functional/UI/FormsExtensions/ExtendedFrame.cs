using System;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class ExtendedFrame : Frame
    {
        public ExtendedFrame()
        {
        }
        
        public static readonly BindableProperty BorderWidthProperty =
            BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(ExtendedFrame), default(double));

        public double BorderWidth
        {
            get { return (double)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }

        public static readonly BindableProperty ShadowOpacityProperty =
            BindableProperty.Create(nameof(ShadowOpacity), typeof(double), typeof(ExtendedFrame), 0.0);

        public double ShadowOpacity
        {
            get { return (double)GetValue(ShadowOpacityProperty); }
            set { SetValue(ShadowOpacityProperty, value); }
        }


        public static readonly BindableProperty ShadowRadiusProperty =
            BindableProperty.Create(nameof(ShadowRadius), typeof(double), typeof(ExtendedFrame), 0.0);

        public double ShadowRadius
        {
            get { return (double)GetValue(ShadowRadiusProperty); }
            set { SetValue(ShadowRadiusProperty, value); }
        }

        public static readonly BindableProperty ShadowOffsetProperty =
            BindableProperty.Create(nameof(ShadowOffset), typeof(Size), typeof(ExtendedFrame), default(Size));

        public Size ShadowOffset
        {
            get { return (Size)GetValue(ShadowOffsetProperty); }
            set { SetValue(ShadowOffsetProperty, value); }
        }

    }
}
