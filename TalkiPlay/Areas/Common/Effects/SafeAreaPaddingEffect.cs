using System;
using Xamarin.Forms;
namespace TalkiPlay
{
    [Flags]
    public enum SafeAreaPaddingType
    {
        None,
        Top = 1,
        Bottom = 2,
    }

    public class SafeAreaPaddingEffect : RoutingEffect
    {
        public SafeAreaPaddingEffect(SafeAreaPaddingType type = SafeAreaPaddingType.Top)
            : base(EffectHelper.GetLocalName<SafeAreaPaddingEffect>())
        {
            SafeAreaPaddingType = type;
        }

        public SafeAreaPaddingType SafeAreaPaddingType { get; private set; }
    }
}
