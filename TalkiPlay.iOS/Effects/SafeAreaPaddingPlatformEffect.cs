using System;
using System.Linq;
using ChilliSource.Core.Extensions;
using TalkiPlay;
using TalkiPlay.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(SafeAreaPaddingPlatformEffect), nameof(SafeAreaPaddingEffect))]

namespace TalkiPlay
{
    public class SafeAreaPaddingPlatformEffect : PlatformEffect
    {
        Thickness _padding;
        protected override void OnAttached()
        {
            if (Element is Layout element)
            {
                var effect = (SafeAreaPaddingEffect)Element.Effects.FirstOrDefault(m => m is SafeAreaPaddingEffect);

                if (effect != null)
                {                    
                    var insets = new ApplicationService().GetSafeAreaInsets(true);                    
                    _padding = element.Padding;

                    var top = effect.SafeAreaPaddingType.Contains(SafeAreaPaddingType.Top) ? insets.Top : 0;
                    var bottom = effect.SafeAreaPaddingType.Contains(SafeAreaPaddingType.Bottom) ? insets.Bottom : 0;

                    element.Padding = new Thickness(_padding.Left, _padding.Top + top, _padding.Right, _padding.Bottom + bottom);
                }
            }
        }

        protected override void OnDetached()
        {
            if (Element is Layout element)
            {
                element.Padding = _padding;
            }
        }
    }
}
