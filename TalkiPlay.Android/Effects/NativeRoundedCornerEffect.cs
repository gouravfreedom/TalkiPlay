using System.Collections.Generic;
using System.ComponentModel;
using Android.Graphics.Drawables;
using Android.OS;
using TalkiPlay;
using TalkiPlay.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views.View;
using AViewGroup = Android.Views.ViewGroup;

[assembly:ExportEffect(typeof(NativeRoundedCornerEffect), nameof(RoundedCornerEffect))]

namespace TalkiPlay.Android
{
    public class NativeRoundedCornerEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            UpdateLayer();
        }

        protected override void OnDetached()
        {

        }


        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
            UpdateLayer();
        }

        void UpdateLayer()
        {
            if (Control != null && Control is AView view)
            {
                SetBorder(view);
            }
            else if (Container != null && Container is AViewGroup viewGroup)
            {
                SetBorder(viewGroup);
            } 
        }

        void SetBorder(AView view1)
        {
            var originalBg = view1.Background;
            var background = view1.Background as GradientDrawable ?? new GradientDrawable();
            if (originalBg is ColorDrawable colorDrawable)
            {
                background.SetColor(colorDrawable.Color);
            }
            
            var position = RoundedCornerEffect.GetRoundedCornerPosition(this.Element);
            var radius = (float) RoundedCornerEffect.GetRadius(this.Element);

            switch (position)
            {
                case RoundedCornerPosition.AllCorners :
                    background.SetCornerRadius(radius);
                    break;
                default:
                    var radii = GetRadius(radius);
                    background.SetCornerRadii(radii);
                    break;
            }
            
            background.SetStroke((int) RoundedCornerEffect.GetBorderWidth(this.Element),
                RoundedCornerEffect.GetBorderColor(this.Element).ToAndroid());
            view1.Background = background;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                view1.ClipToOutline = true;
            }
        }

        float[] GetRadius(float radius)
        {
            var r = new List<float>();
            
            if (RoundedCornerEffect.HasTopLeft(this.Element))
            {
                r.Add(radius);
            }
            else
            {
                r.Add(0);
            }

            if (RoundedCornerEffect.HasTopRight(this.Element))
            {
                r.Add(radius);
            }
            else
            {
                r.Add(0);
            }

            if (RoundedCornerEffect.HasBottomLeft(this.Element))
            {
                r.Add(radius);
            }
            else
            {
                r.Add(0);
            }

            if (RoundedCornerEffect.HasBottomRight(this.Element))
            {
                r.Add(radius);
            }
            else
            {
                r.Add(0);
            }

            return r.ToArray();
        }        
    }
}