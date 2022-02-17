using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.OS;
using Android.Support.V7.Widget;
using ChilliSource.Mobile.UI;
using FFImageLoading.Svg.Forms;
//using Naxam.Controls.Platform.Droid;
using Plugin.DownloadManager;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Provider;
using ChilliSource.Mobile.Core;
using Lottie.Forms.Droid;
using Plugin.DownloadManager.Abstractions;
//using Plugin.Permissions;
using Environment = Android.OS.Environment;
using Permission = Android.Content.PM.Permission;
using Uri = Android.Net.Uri;
using Intent = Android.Content.Intent;
using MediaManager;
using Plugin.InAppBilling;
using ZXing.Mobile;
using System.Threading.Tasks;
using Android.Runtime;

namespace TalkiPlay.Droid
{

    [Activity(Icon = "@drawable/icon", Theme = "@style/splashscreen", MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter (new[] { Intent.ActionView },
        Categories = new[] {
            Intent.ActionView,
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "https",
        DataHost = Config.Domain,
        AutoVerify = true)
    ]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.Window.RequestFeature(WindowFeatures.ActionBar);
            TabLayoutResource = Resource.Layout.Tabbar;
            //TabLayoutResourceId = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.SetTheme(Resource.Style.MyTheme);

            base.OnCreate(bundle);
            AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidEnvironmentUnhandledExceptionRaiser;
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainException;
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerException;

            this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Forms.SetFlags("IndicatorView_Experimental","Markup_Experimental","Shapes_Experimental", "Brush_Experimental");
            global::Xamarin.Forms.Forms.Init(this, bundle);

            InitPlugins(bundle);

            RegisterDependencies();

            //SetupBottomTabbedRenderer();
            
            LoadApplication(new App());
            

            //if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
            //    this.RequestPermissions(new[]
            //    {
            //        Manifest.Permission.AccessCoarseLocation,
            //        Manifest.Permission.Bluetooth,
            //        Manifest.Permission.BluetoothAdmin
            //    }, 0);
            //}
        }

        private void OnAndroidEnvironmentUnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            var exc = e.Exception;
            if (exc != null)
            {
                System.Diagnostics.Debug.WriteLine("Exc Msg: " + exc.Message);
                System.Diagnostics.Debug.WriteLine("Exc Stack: " + exc.StackTrace);
                var logger = Locator.Current.GetService<ChilliSource.Mobile.Core.ILogger>();
                logger.Error(exc, "Droid Env unhandled exception");
            }
        }
        
        private void OnCurrentDomainException(object sender, UnhandledExceptionEventArgs e)
        {
            var exc = e.ExceptionObject as Exception;
            if (exc != null)
            {
                System.Diagnostics.Debug.WriteLine("Exc Msg: " + exc.Message);
                System.Diagnostics.Debug.WriteLine("Exc Stack: " + exc.StackTrace);
                var logger = Locator.Current.GetService<ChilliSource.Mobile.Core.ILogger>();
                logger.Error(exc, "Droid current domain unhandled exception");
            }
        }

        private void OnTaskSchedulerException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var exc = e.Exception;
            if (exc != null)
            {
                System.Diagnostics.Debug.WriteLine("Exc Msg: " + exc.Message);
                System.Diagnostics.Debug.WriteLine("Exc Stack: " + exc.StackTrace);
                var logger = Locator.Current.GetService<ChilliSource.Mobile.Core.ILogger>();
                logger.Error(exc, "Droid task unhandled exception");
            }
        }

        void InitPlugins(Bundle bundle)
        {
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            UserDialogs.Init(() => this);
            var ignore = typeof(SvgCachedImage);
            AnimationViewRenderer.Init();
            FormsControls.Droid.Main.Init(this);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            Xamarin.Essentials.Platform.Init(this, bundle);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            CrossMediaManager.Current.Init(this);
        }
        private void RegisterDependencies()
        {
            RxApp.SuspensionHost.SetupDefaultSuspendResume();
            //Locator.CurrentMutable.RegisterConstant(new ExtendedAndroidCommandBinder(), typeof(ICreatesCommandBinding));
            Locator.CurrentMutable.RegisterLazySingleton(() => new ApplicationService(this), typeof(IApplicationService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new DownloadManagerExtended(this), typeof(IDownloadManagerExtended));
            SetupDownloadManager();
        }

        #region Overrides
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            
            //PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }
        
        // protected override void OnPostCreate(Bundle savedInstanceState)
        // {
        //     base.OnPostCreate(savedInstanceState);
        //     var toolbar = this.FindViewById<Toolbar>(Resource.Id.toolbar);
        //     SetSupportActionBar(toolbar);
        // }
        
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == 16908332)
            {
                var main = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
                if (main is IBasePageController navigator)
                {
                    navigator.OnBackButtonTapped();
                    return false;
                }
            }
            
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            var main = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
            if (main is IBasePageController navigator)
            {
                navigator.OnBackButtonTapped();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        #endregion
       
        
        #region Downloads
        
        private void SetupDownloadManager()
        {
            var folder = FileSystemManager.CreateDocumentsSubfolder(Config.AssetFolderName);
            
            if (CrossDownloadManager.Current is DownloadManagerImplementation mgr)
            {
                mgr.IsVisibleInDownloadsUi = false;
            }
            
            CrossDownloadManager.Current.PathNameForDownloadedFile = file =>
            {
                var fileUrl = Uri.Parse(file.Url);
                var fileName = file.Headers.ContainsKey("fileName") ? file.Headers["fileName"] : fileUrl.Path.Split('/').Last();

                if (file.Status == DownloadFileStatus.COMPLETED)
                {
                    return  System.IO.Path.Combine (folder, fileName);
                }

                var path = ApplicationContext.GetExternalFilesDir(Environment.DirectoryDownloads).AbsolutePath;
                return System.IO.Path.Combine (path, fileName);

            };

            var downloadManager = CrossDownloadManager.Current;
           
            downloadManager.CollectionChanged += (sender, e) => {
                if (e.NewItems != null) {
                    foreach (var item in e.NewItems) {
                        ((IDownloadFile)item).PropertyChanged += OnDownloadFilePropertyChanged;
                    }
                }
            };

            foreach (var item in downloadManager.Queue)
            {
                item.PropertyChanged += OnDownloadFilePropertyChanged;
            }
        }

        private void OnDownloadFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HandleDownloadStatusChanged((IDownloadFile) sender, e.PropertyName);
        }

        private void HandleDownloadStatusChanged(IDownloadFile file, string methodName)
        {
            if (methodName.Equals("Status") && file.Status == DownloadFileStatus.COMPLETED)
            {
                try
                {
                    file.PropertyChanged -= OnDownloadFilePropertyChanged;
                    var nativeDownloadManager = (DownloadManager)ApplicationContext.GetSystemService (Context.DownloadService);
                    
                    var source = nativeDownloadManager.GetUriForDownloadedFile (((DownloadFileImplementation)file).Id);
                    var rootPath = Locator.Current.GetService<IStorage>().GetRootPath();
                    var fileName = file.Headers.ContainsKey("fileName") ? file.Headers["fileName"] : GetFileName(source);
                    var destination = System.IO.Path.Combine(rootPath, fileName);
                    
                    using (var stream = ApplicationContext.ContentResolver.OpenInputStream (source)) 
                    {
                        using (var dest = File.OpenWrite(destination)) 
                        {
                            stream.CopyTo (dest);
                        }
                    }
                    nativeDownloadManager.Remove (((DownloadFileImplementation)file).Id);
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e.Message, e);
                }
            }
        }
        
        private string GetFileName(Uri uri) 
        {
            string result = null;
            if (uri.Scheme.Equals("content"))
            {
                var cursor = ContentResolver.Query(uri, null, null, null, null);
                try {
                    if (cursor != null && cursor.MoveToFirst()) {
                        result = cursor.GetString(cursor.GetColumnIndex(OpenableColumns.DisplayName));
                    }
                } finally {
                    cursor?.Close();
                }
            }

            if (result != null) return result;

            result = uri.Path;
            var cut = result.LastIndexOf('/');
            if (cut != -1) {
                result = result.Substring(cut + 1);
            }
            
            return result;
        }
        
        #endregion
        
       
  
        
       
        
        
        //void SetupBottomTabbedRenderer()
        //{
        //    BottomTabbedRenderer.BackgroundColor = Colors.WhiteColor.ToAndroid();
        //    BottomTabbedRenderer.FontSize = 10;
        //    BottomTabbedRenderer.IconSize = 20;
        //    BottomTabbedRenderer.MenuItemIconSetter = (menuItem, icon, selected) =>
        //    {
        //        var file = icon.File.Replace("images/", "");
        //        var image = selected ? file.Replace("_off", "_on") : file;
        //        var imageSource = new FileImageSource { File = image };
        //        BottomTabbedRenderer.DefaultMenuItemIconSetter.Invoke(menuItem, imageSource, selected);
        //    };
        //    var stateList = new ColorStateList(
        //        new[]
        //        {
        //            new[] {global::Android.Resource.Attribute.StateChecked
        //            },
        //            new[] { global::Android.Resource.Attribute.StateEnabled
        //            }
        //        },
        //        new int[] {
        //            Colors.Yellow.ToAndroid(),
        //            Colors.GreenyBlue.ToAndroid()
        //        });

        //    BottomTabbedRenderer.ItemTextColor = stateList;
        //    BottomTabbedRenderer.ItemIconTintList = stateList;
        //    BottomTabbedRenderer.Typeface = Typeface.CreateFromAsset(this.Assets, "fonts/Roboto-Bold.ttf");
        //    // BottomTabbedRenderer.ItemBackgroundResource = Resource.Drawable.tab_background;
        //    BottomTabbedRenderer.ItemSpacing = 8;
        //    BottomTabbedRenderer.ItemPadding = new Xamarin.Forms.Thickness(8);
        //    BottomTabbedRenderer.BottomBarHeight = 60;
        //    BottomTabbedRenderer.ItemAlign = ItemAlignFlags.Center;
        //}
    }
}
