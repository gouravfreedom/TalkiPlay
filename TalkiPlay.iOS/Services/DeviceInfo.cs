using System;
using UIKit;
using Xamarin.Forms;

namespace TalkiPlay
{
    public static partial class DeviceInfo
    {
        public static double GetStatusbarHeight()
        {
            return UIApplication.SharedApplication.StatusBarFrame.Height;
        }

        public static double GetNavBarHeight()
        {
            return UIApplication.SharedApplication.KeyWindow?.RootViewController?.NavigationController?
            .NavigationBar?.Bounds.Height ?? 44.0;
        }


        public static Size GetScreenSize()
        {
            return new Size(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
        }
        
        
        public static Thickness GetPlatformSafeAreaInsets(bool includeStatusBar = false)
        {
            var result = new Thickness();

            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                var windows = UIApplication.SharedApplication?.Windows;

                if (windows != null && windows.Length > 0)
                {
                    var insets = windows[0].SafeAreaInsets;

                    result.Left = insets.Left;
                    result.Right = insets.Right;
                    result.Top = insets.Top;
                    result.Bottom = insets.Bottom;
                }
            }

            //this is needed because on iOS 12 the top inset for the status bar 
            //is automatically included but not for previous iOS versions
            if (result.Top <= 0 && includeStatusBar)
            {
                result.Top = 20;
            }

            return result;
        }
        
        public static bool GetIsInForeground()
        {
            return UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active;
        }
    }

}
