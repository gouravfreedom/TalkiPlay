using System;
using System.ComponentModel;
using Com.Airbnb.Lottie;
using Lottie.Forms.Droid;
using TalkiPlay;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(ExtendedAnimationView), typeof(ExtendedAnimationViewRenderer))]

namespace TalkiPlay
{
    public class ExtendedAnimationViewRenderer : AnimationViewRenderer
    {
        public ExtendedAnimationViewRenderer()
        {
        }
        
        public ExtendedAnimationView ExtendedAnimationView => Element as ExtendedAnimationView;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ExtendedAnimationView.JsonSourceProperty.PropertyName)
            {
                SetJsonSource();
            }
        }

        void SetJsonSource()
        {
            try
            {
                var source = ExtendedAnimationView.JsonSource;

                if (!string.IsNullOrEmpty(source))
                {
                    LottieComposition.Factory.FromJsonString(source, (composition) =>
                    {
                        Control.Composition = composition;
                        Control.Progress = 0;
                        Control.PlayAnimation();
                    });
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }
    }
}