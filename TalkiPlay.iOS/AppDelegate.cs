using System;
using System.IO;
using System.Reactive.Linq;
using Acr.UserDialogs;
using Akavache;
using AVFoundation;
using ChilliSource.Mobile.Core;
using FFImageLoading.Svg.Forms;
using Foundation;
using Lottie.Forms.iOS.Renderers;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using ChilliSource.Mobile.UI.ReactiveUI;
using FFImageLoading;
//using Plugin.BluetoothLE;
using Plugin.DownloadManager;
//using TalkiPlay.iOS.Helpers;
using Unit = System.Reactive.Unit;
using MediaManager;
using ProgressRingControl.Forms.Plugin.iOS;

namespace TalkiPlay.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.SetFlags("IndicatorView_Experimental","Markup_Experimental","Shapes_Experimental", "CarouselView_Experimental");
            global::Xamarin.Forms.Forms.Init();
            Sharpnado.Tabs.iOS.Preserver.Preserve();

            InitPlugins();

            Locator.CurrentMutable.RegisterLazySingleton(() => new AudioPlayerFactory(), typeof(IAudioPlayerFactory));
            RegisterDependencies();
            SetGlobalAppearance();
            LoadApplication(new App());
         
            SetupAudioSession();

            return base.FinishedLaunching(app, options);
        }

        void InitPlugins()
        {
            SvgCachedImage.Init();
            Rg.Plugins.Popup.Popup.Init();
            CrossMediaManager.Current.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            ProgressRingRenderer.Init();
            var config = new FFImageLoading.Config.Configuration()
            {
                VerboseLogging = true,
                VerbosePerformanceLogging = false,
                VerboseMemoryCacheLogging = false,
                VerboseLoadingCancelledLogging = false,
                Logger = new CustomLogger(),
            };

            ImageService.Instance.Initialize(config);
            var ignore = typeof(SvgCachedImage);
            AnimationViewRenderer.Init();
            FormsControls.Touch.Main.Init();

            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
        }

        static void SetupAudioSession()
        {
            try
            {
                AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback, AVAudioSessionCategoryOptions.MixWithOthers);
            }
            catch(Exception)
            {
               
            }
        }
        
        void RegisterDependencies()
        {
            RxApp.SuspensionHost.SetupDefaultSuspendResume();
            Locator.CurrentMutable.RegisterLazySingleton(() => new MessageHandlerFactory(), typeof(IMessageHandlerFactory));
            Locator.CurrentMutable.RegisterLazySingleton(() => new AudioPlayerFactory(), typeof(IAudioPlayerFactory));
            //Locator.CurrentMutable.RegisterLazySingleton(() => new TimezoneProvider(), typeof(ITimezone));
            //Locator.CurrentMutable.RegisterConstant(new ExtendedUIKitCommandBinder(), typeof(ICreatesCommandBinding));
            Locator.CurrentMutable.RegisterLazySingleton(() => new ApplicationService(), typeof(IApplicationService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new DownloadManagerExtended(), typeof(IDownloadManagerExtended));
            SetupDownloadManager();
        }

        void SetGlobalAppearance()
        {
            //UIWindow.Appearance.TintColor = Fonts.MainAppColor.ToUIColor();

//            var navigationBarTitleAttributes = new UITextAttributes()
//            {
//                Font = UIFont.FromName(Fonts.NavigationTitleFont.Family, size: Fonts.NavigationTitleFont.Size),
//                TextColor = Fonts.NavigationTitleFont.Color.ToUIColor()
//            };

			//UINavigationBar.Appearance.TintColor = Color.White.ToUIColor();
          //  UINavigationBar.Appearance.SetTitleTextAttributes(navigationBarTitleAttributes);
       
//            var barButtonAttributes = new UITextAttributes()
//            {
//                Font = UIFont.FromName(Fonts.NavigationBarActionFont.Family, Fonts.NavigationBarActionFont.Size),
//                TextColor = Fonts.NavigationBarActionFont.Color.ToUIColor()
//            };

           // UIBarButtonItem.AppearanceWhenContainedIn(typeof(UINavigationBar)).SetTitleTextAttributes(barButtonAttributes, UIControlState.Normal);
           UITabBar.Appearance.BarTintColor = Color.White.ToUIColor();
            

        }

        public override void WillTerminate(UIApplication uiApplication)
        {
            base.WillTerminate(uiApplication);
            var service = Locator.Current.GetService<IApplicationService>();
            service?.OnTerminated();
            Shutdown();
        }

        void Shutdown()
        {
            Observable.FromAsync(async () => await BlobCache.Shutdown(), RxApp.TaskpoolScheduler)
                .OnErrorResumeNext(Observable.Return(Unit.Default))
                .SubscribeSafe();
        }
        
        public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, Action completionHandler)
        {
            CrossDownloadManager.BackgroundSessionCompletionHandler = completionHandler;
        }

        void SetupDownloadManager()
        {
            var folder = FileSystemManager.CreateDocumentsSubfolder(Config.AssetFolderName);
            
            CrossDownloadManager.Current.PathNameForDownloadedFile = file =>
            {
                var fileUrl = new NSUrl(file.Url, false);
                var fileName = file.Headers.ContainsKey("fileName") ? file.Headers["fileName"] : fileUrl.LastPathComponent;
                return Path.Combine(folder, fileName);
            };
        }
        
    
    }
    
    public class CustomLogger : FFImageLoading.Helpers.IMiniLogger
    {
        public void Debug(string message)
        {
            //Console.WriteLine(message);
        }

        public void Error(string errorMessage)
        {
            Console.WriteLine(errorMessage);
        }

        public void Error(string errorMessage, Exception ex)
        {
            Error(errorMessage + System.Environment.NewLine + ex.ToString());
        }
    }
}
