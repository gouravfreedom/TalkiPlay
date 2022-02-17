using System;
using Foundation;
using TalkiPlay.Shared;
using UIKit;
using Xamarin.Forms;

namespace TalkiPlay.iOS
{
    public class ApplicationService : IApplicationService
    {
        public double StatusbarHeight => UIApplication.SharedApplication.StatusBarFrame.Height;

        public double NavBarHeight => UIApplication.SharedApplication.KeyWindow?.RootViewController?.NavigationController?
            .NavigationBar?.Bounds.Height ?? 44.0;

        public Size ScreenSize => new Size(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
        
        public string Timezone => NSTimeZone.LocalTimeZone.Name;
        
        public void OpenSettings()
        {
            UIApplication.SharedApplication.OpenUrl(url: new Foundation.NSUrl(UIApplication.OpenSettingsUrlString));
        }

        public Thickness GetSafeAreaInsets(bool includeStatusBar = false)
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


        public void OnTerminated()
        {
            Terminated?.Invoke(UIApplication.SharedApplication, new EventArgs());
        }

        public event EventHandler Terminated;


    }
}