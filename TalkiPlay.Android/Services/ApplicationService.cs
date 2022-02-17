using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Plugin.CurrentActivity;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Size = Xamarin.Forms.Size;


namespace TalkiPlay.Droid
{
    public class ApplicationService : IApplicationService
    {
        private readonly Context _context;

        public ApplicationService(Context context)
        {
            _context = context;
        }

        private Context Context => _context;


        public double StatusbarHeight
        {
            get
            {
                var resourceId = Context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
                var displayMetrics = Context.Resources.DisplayMetrics;
                return resourceId > 0 ? Context.Resources.GetDimensionPixelSize(resourceId) / displayMetrics.Density: 0;
            }
        }

        public double NavBarHeight
        {
            get
            {
                var resourceId = Context.Resources.GetIdentifier("navigation_bar_height", "dimen", "android");
                var displayMetrics = Context.Resources.DisplayMetrics;
                return resourceId > 0 ? Context.Resources.GetDimensionPixelSize(resourceId) / displayMetrics.Density : 0;
            }
        }

        public Size ScreenSize
        {
            get
            {
                var wm = Context.GetSystemService(Context.WindowService);
                var manager = wm.JavaCast<IWindowManager>();
                var displayMetrics = Context.Resources.DisplayMetrics;
                var display = manager.DefaultDisplay;
                var metrics = new DisplayMetrics();
                display.GetMetrics(metrics);
                return new Size(metrics.WidthPixels / displayMetrics.Density, metrics.HeightPixels / displayMetrics.Density);
            }
        }

        public string Timezone => Java.Util.TimeZone.Default.ID;
        
        public void EnableSleepMode(bool isEnabled)
        {
            if (isEnabled)
            {
                CrossCurrentActivity.Current.Activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            }
        }

        public void OnTerminated()
        {
            Terminated?.Invoke(CrossCurrentActivity.Current.Activity, new EventArgs());
        }

        public void OpenSettings()
        {
           
        }

        public Thickness GetSafeAreaInsets(bool includeStatusBar = false)
        {
            return new Thickness() { Top = includeStatusBar ? StatusbarHeight : 0 };
        }

        public event EventHandler Terminated;
    }
}