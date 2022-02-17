using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.App;
using Size = Xamarin.Forms.Size;

namespace TalkiPlay
{
    public static partial class DeviceInfo
    {
        static Context Context => Application.Context;
        //Xamarin.Essentials.Platform.AppContext;

        public static double GetStatusbarHeight()
        {
            return 0;
            //var resourceId = Context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            //var displayMetrics = Context.Resources.DisplayMetrics;
            //return resourceId > 0 ? Context.Resources.GetDimensionPixelSize(resourceId) / displayMetrics.Density : 0;

        }

        public static double GetNavBarHeight()
        {
            var resourceId = Context.Resources.GetIdentifier("navigation_bar_height", "dimen", "android");
            var displayMetrics = Context.Resources.DisplayMetrics;
            return resourceId > 0 ? Context.Resources.GetDimensionPixelSize(resourceId) / displayMetrics.Density : 0;

        }


        public static Size GetScreenSize()
        {
            var wm = Context.GetSystemService(Context.WindowService);
            var manager = wm.JavaCast<IWindowManager>();
            var displayMetrics = Context.Resources.DisplayMetrics;
            var display = manager.DefaultDisplay;
            var metrics = new DisplayMetrics();
            display.GetMetrics(metrics);
            return new Size(metrics.WidthPixels / displayMetrics.Density, metrics.HeightPixels / displayMetrics.Density);

        }


        public static Xamarin.Forms.Thickness GetPlatformSafeAreaInsets(bool includeStatusBar = false)
        {
            return new Xamarin.Forms.Thickness() { Top = includeStatusBar ? GetStatusbarHeight() : 0 };
        }
        
        public static bool GetIsInForeground()
        {
            Context context = Context;
            ActivityManager activityManager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            IList<ActivityManager.RunningAppProcessInfo> appProcesses = activityManager.RunningAppProcesses;
            if (appProcesses == null)
            {
                return false;
            }
            string packageName = context.PackageName;
            foreach (ActivityManager.RunningAppProcessInfo appProcess in appProcesses)
            {
                if (appProcess.Importance == Importance.Background && appProcess.ProcessName == packageName)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
