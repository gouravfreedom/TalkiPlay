using Xamarin.Forms;

namespace TalkiPlay
{
    public static partial class Dimensions
    {
        public const int DefaultChildImageSize = 60;
        
        public const int DefaultCornerRadius = 5;
        public const int DefaultSpacing = 20;
        public const int DefaultHorizontalMargin = 15;
        public const int DefaultButtonHeight = 60;
        public const int DefaultNavBarHeight = 40;
        public static Thickness NavPadding(double top) => new Thickness(15, top + 8, 15, 8);

        public static Thickness GetFormTextFieldPadding()
        {
#if __IOS__
            return new Thickness(1);
#endif

#if __ANDROID__
            return  new Thickness(10, 10, 10, 8);
#endif
        }

        public static double GetFormTextFieldHeight()
        {
#if __IOS__
            return 60;
#endif

#if __ANDROID__
            return 40;
#endif
        }
    }
}