using System;
using Xamarin.Forms;

namespace TalkiPlay
{
    public static partial class DeviceInfo
    {
        public static double StatusbarHeight => GetStatusbarHeight();

        public static double NavBarHeight => GetNavBarHeight();

        public static Size ScreenSize => GetScreenSize();

        public static bool IsInForeground => GetIsInForeground();
        
        public static Thickness GetSafeAreaInsets(bool includeStatusBar = false)
        {
            return GetPlatformSafeAreaInsets(includeStatusBar);
        }
    }
}
