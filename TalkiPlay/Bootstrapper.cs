using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Akavache;
using ChilliSource.Mobile.Api;
using ChilliSource.Mobile.Core;
using ChilliSource.Mobile.Logging;
using ChilliSource.Mobile.UI.ReactiveUI;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.BluetoothLE;
using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;
using ReactiveUI;
using Serilog;
using Serilog.Events;
using Serilog.Sink.AppCenter;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Essentials;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay
{
    public static class Bootstrapper
    {

        public static bool IsProcessingSubscription { get; set; }
        

        public static void Init()
        {
            VersionTracking.Track();
            
            RegisterServices();
            RegisterPages();
            
            var appService = Locator.Current.GetService<IApplicationService>();
            appService.Terminated += OnTerminated;

            var userSettings = Locator.Current.GetService<IUserSettings>();
            userSettings.ReloadGameSession = true;

            
            if (VersionTracking.IsFirstLaunchEver)
            {
                //make sure no old user data is still in the keychain
                SecureSettingsService.ClearUser();
            }
            else
            {
                SyncUserDetails();
                SubscriptionHelper.VerifySubscriptionInBackground();
            }
        }

        private static void OnTerminated(object sender, EventArgs args)
        {
            var talkiPlayerManager = Locator.Current.GetService<ITalkiPlayerManager>();
            talkiPlayerManager.Dispose();
        }
        
        public static void CleanupSessionAndReset()
        {
            var userRepository = Locator.Current.GetService<IUserRepository>();
            userRepository.ClearSession();
            
            var navHelper = Locator.Current.GetService<ITalkiPlayNavigator>();
            navHelper.NavigateToLoginPage();
        }

        public static void SyncUserDetails()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                return;
            }
            
            Task.Run(async () =>
            {
                var user = await SecureSettingsService.GetUser();
                if (user == null)
                {
                    return;
                }
                
                var api = Locator.Current.GetService<IApi<ITalkiPlayApi>>();
                var key = await SecureSettingsService.GetUserKey();
                api.SetUserKey(key);
                
                var previousSubscriptionStatus = user.SubscriptionStatus;
                var userService = Locator.Current.GetService<IUserRepository>();

                try
                {
                    var refreshedUser = await userService.GetUser();
                    
                    if (refreshedUser.SubscriptionStatus != previousSubscriptionStatus)
                    {
                        MessageBus.Current.SendMessage(new SubscriptionChangedMessage());
                    }
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e, e.Message);
                }
            });
        }

        public static void PlayMusic()
        {
            var player = Locator.Current.GetService<IBackgroundAudioPlayer>();
            if (!player.IsPlaying)
            {
                player.Play(Constants.ExploreMusic);
                player.ChangeVolume(Constants.SmallMusicVolume);
            }
        }

        public static void StopMusic()
        {
            var backgroundAudioPlayer = Locator.Current.GetService<IBackgroundAudioPlayer>();
            backgroundAudioPlayer.Stop();
        }
        


        #region Pages

        public static async Task<Page> GetMainPage()
        {
            //var page = new TestGamePage();
            //var viewModel = new TestGamePageViewModel();
            //page.BindingContext = viewModel;
            //return new NavigationPage(page);


            //return GetTabbedPage();

            var settings = Locator.Current.GetService<IUserSettings>();
            var isUserLoggedIn = await SecureSettingsService.IsUserLoggedIn();

            
             if (isUserLoggedIn)
             {
                if (settings.IsOnboarded)
                {
                    return GetTabbedPage();
                }
                else
                {
                    if (settings.HasTalkiPlayerDevice)
                    {
                        return GetDeviceOnboardingPage();
                    }
                    else
                    {
                        return GetQRCodeOnboardingPage();
                    }
                }
             }
             else
            {
                 if (settings.IsOnboarded)
                 {
                     return GetLoginPage();
                 }
                 else
                 {
                    return GetModeSelectionPage();
                 }
             }
        }

        public static async Task GotoOnboardingPage()
        {
            var userSettings = Locator.Current.GetService<IUserSettings>();
            if (userSettings.HasTalkiPlayerDevice)
            {
                if (await SecureSettingsService.IsUserLoggedIn())
                {
                    SimpleNavigationService.PushAsync(new OnboardingPageViewModel(userSettings)).Forget();
                }
                else
                {
                    SimpleNavigationService.PushAsync(new SignupPageViewModel(SignupNavigationSource.ModeSelection)).Forget();
                }
            }
            else
            {
                if (await SecureSettingsService.IsUserLoggedIn())
                {
                    SimpleNavigationService.PushAsync(new OnboardingImagePageViewModel(QRCodeOnboardingStep.Welcome, new QRCodeOnboardingState())).Forget();
                    SimpleNavigationService.PopModalAsync().Forget();
                }
                else
                {
                    SimpleNavigationService.PushAsync(new SignupPageViewModel(SignupNavigationSource.ModeSelection)).Forget();
                }
            }
        }


        static Page GetModeSelectionPage()
        {
            var page = new NewModeSelectionPage();
            var viewModel = new NewModeSelectionPageViewModel(null, source: ModeSelectionPageSource.Startup);
            page.BindingContext = viewModel;
            return new NavigationPage(page);
        }

        static Page GetDeviceOnboardingPage()
        {
            var page = new OnboardingPage();
            var viewModel = new OnboardingPageViewModel();
            page.BindingContext = viewModel;
            //var page = new ContentPage();
            //page.Content = new WelcomeView();
            return new NavigationPage(page);
        }

        static Page GetQRCodeOnboardingPage()
        {            
            var page = new OnboardingImagePage();
            var viewModel = new OnboardingImagePageViewModel(QRCodeOnboardingStep.Welcome, new QRCodeOnboardingState());
            page.BindingContext = viewModel;
            return new NavigationPage(page);
        }

        public static Page GetLoginPage()
        {
            var startPage = new LoginPage();
            //var mainPage = new TransitionReactiveNavigationViewHost(startPage);
            var mainPage = new NavigationPage(startPage);
            //var navigator = SetupNavigationService(mainPage, Constants.MainNavigation);
            var viewModel = new LoginPageViewModel(LoginNavigationSource.Default);            
            ((IViewFor) startPage).ViewModel = viewModel;
            startPage.BindingContext = viewModel;
            startPage.Title = viewModel.Title;
            return mainPage;
        }
        
        public static Page GetChildPage<T>(Func<Page> pageFactory,
            Func<INavigationService,T> viewModelFactory) where T : class, IPageViewModel, IModalViewModel
        {
            var startPage = pageFactory();
            var navigationPage = new NavigationViewHost<NavigationItemViewModel<T>>(startPage);
            var navigator = new NavigationService(navigationPage, Locator.Current.GetService<ILogger>());
            var viewModel = viewModelFactory(navigator);
            ((IViewFor) startPage).ViewModel = viewModel;
            startPage.BindingContext = viewModel;
            startPage.Title = ((IModalViewModel) viewModel).Title;
            return navigationPage;
        }

        
        public static Page GetTabbedPage(TabItemType defaulTab = TabItemType.Games)
        {
             return new NavigationPage(new MainTabPage() { BindingContext = new MainTabPageViewModel() });
            
            // Page startPage;
            // ITabView tabView;
            //
            // if (Device.RuntimePlatform == Device.Android)
            // {
            //     startPage = new BottomTabsPage();                
            // }
            // else
            // {
            //     startPage = new TabsPage();                                
            // }
            //
            // tabView = (ITabView)startPage;
            //
            // var mainPage = new TransitionReactiveNavigationViewHost(startPage);
            // var navigator = SetupNavigationService(mainPage, Constants.MainNavigation);
            // var tabs = new TabItemsModel(tabView);
            // Locator.CurrentMutable.RegisterConstant(tabs, typeof(ITabService));
            // var viewModel = tabs;
            // ((IViewFor) startPage).ViewModel = viewModel;
            // startPage.BindingContext = viewModel;
            // startPage.Title = viewModel.Title;
            //
            // if (defaulTab != TabItemType.Children)
            // {
            //     tabView.ChangeTab(defaulTab);
            // }
            //
            // return mainPage;
        }
        
        
        public static Page GetTabItemPage<T>(Func<IViewFor<T>> pageCreator, 
            Func<INavigationService, T> viewModelCreator,
            string title,
            string icon,
            string contract )
            where T : class
        {
            var page = pageCreator();
            var mainPage = page as Page;
            var navigationPage = new TransitionReactiveNavigationViewHost(mainPage);
            var navigator = SetupNavigationService(navigationPage, contract);
            var viewModel = viewModelCreator(navigator);
            page.ViewModel = viewModel;
            mainPage.BindingContext = viewModel;
            switch (viewModel)
            {
                case IPageViewModel vm:
                    navigationPage.IconImageSource = ImageSource.FromFile(icon);
                    navigationPage.Title = title;
                    mainPage.Title = vm.Title;
                    break;
                case IModalViewModel mvm:
                    navigationPage.IconImageSource = ImageSource.FromFile(icon);
                    navigationPage.Title = title;
                    mainPage.Title = mvm.Title;
                    break;
                case IPopModalViewModel pvm:
                    navigationPage.IconImageSource = ImageSource.FromFile(icon);
                    navigationPage.Title = title;
                    mainPage.Title = pvm.Title;
                    break;
            }
            return navigationPage;
        }

       
        private static INavigationService SetupNavigationService(IView page, string contract)
        {
            var navigator = new NavigationService(page, Locator.Current.GetService<ILogger>());
            Locator.CurrentMutable.RegisterConstant(navigator, typeof(INavigationService), contract);
            return navigator;
        }
#endregion

        static EnvironmentInformation GetEnvironmentInformation()
        {
            //var timezoneProvider = Locator.Current.GetService<ITimezone>();
            var appService = Locator.Current.GetService<IApplicationService>();
            var settings = Locator.Current.GetService<IUserSettings>();
            
            if (String.IsNullOrWhiteSpace(settings.UniqueId))
            {
                settings.UniqueId = Guid.NewGuid().ToString();
            }
            
            var info = new EnvironmentInformation(Config.Environment,  settings.UniqueId,
                Xamarin.Essentials.DeviceInfo.Version.ToString(), appService.Timezone, 
                Xamarin.Essentials.DeviceInfo.Platform.ToString(), Config.AppName,
                Xamarin.Essentials.DeviceInfo.Name);

            return info;
        }

        private static ILogger BuildLogger()
        {
            AppCenter.Start($"ios={Config.AppCenterSecretiOS};" +
                            $"android={Config.AppCenterSecretAndroid}",
                  typeof(Analytics), typeof(Crashes));

            AppCenter.LogLevel = Microsoft.AppCenter.LogLevel.Warn;
          
            var logger = new LoggerConfiguration()
                .WithApplicationInformation(GetEnvironmentInformation())
                .MinimumLevel.Debug()
                .WriteTo.Diagnostics()
                .WriteTo.Debug()
                #if __IOS__
                .WriteTo.NSLog()
                #endif
                #if __ANDROID__
                .WriteTo.AndroidLog()
                #endif
                .WriteTo.AppCenterSink(null, LogEventLevel.Warning, AppCenterTarget.ExceptionsAsCrashes, Config.GetAppCenterSecret())
                .BuildLogger();
            
            return logger;
        }

        
        private static void RegisterServices()
        {
            BlobCache.ApplicationName = Config.AppName;
            BlobCache.ForcedDateTimeKind = DateTimeKind.Utc;            
            BlobCache.EnsureInitialized();
            ToastConfig.DefaultMessageTextColor = Color.White;
            ToastConfig.DefaultBackgroundColor = Colors.BlueColor;
            ToastConfig.DefaultActionTextColor = Color.White;
            ToastConfig.DefaultDuration = TimeSpan.FromSeconds(2);
			ToastConfig.DefaultPosition = ToastPosition.Bottom;
            Locator.CurrentMutable.RegisterLazySingleton(() => new Configuration(), typeof(IConfig));
            Locator.CurrentMutable.RegisterLazySingleton(() => BlobCache.LocalMachine, typeof(IBlobCache));
            Locator.CurrentMutable.RegisterLazySingleton(() => new UserSettings(), typeof(IUserSettings));
            Locator.CurrentMutable.RegisterLazySingleton(() => UserDialogs.Instance, typeof(IUserDialogs));
            Locator.CurrentMutable.RegisterConstant(BuildLogger(), typeof(ILogger));
            Locator.CurrentMutable.RegisterLazySingleton(() => new FileStorage(Config.AssetFolderName), typeof(IStorage));
            Locator.CurrentMutable.Register(() =>
            {
                var logger = Locator.Current.GetService<ILogger>();
                var factory = Locator.Current.GetService<IAudioPlayerFactory>();
                return factory.Create(logger);
            }, typeof(IAudioPlayer));

            //var messageHandler = Locator.Current.GetService<IMessageHandlerFactory>();
            Locator.CurrentMutable.RegisterLazySingleton(() => new ApiExceptionHandlerConfig(r =>
            {
                Device.BeginInvokeOnMainThread( CleanupSessionAndReset);
            }, r =>
            {
				var notifier = Locator.Current.GetService<IConnectivityNotifier>();
				notifier.Notifier.Handle(r)
                    .SubscribeSafe();
                
            }, Locator.Current.GetService<ILogger>()), typeof(ApiExceptionHandlerConfig));
            
            // Locator.CurrentMutable.RegisterLazySingleton(
            //     () => new ApiManager<ITalkiPlayApi>(
            //         new ApiConfiguration(Config.ApiUrl, 
            //             () => new NoNetworkHandler(new Connectivity(), new ApiAuthenticationHandler(GetToken, messageHandler.CreateHandler()))
            //         )
            //     ), 
            //     typeof(IApi<ITalkiPlayApi>));

            Locator.CurrentMutable.RegisterLazySingleton(
                () => new ApiManager<ITalkiPlayApi>(
                    Config.ApiUrl,
                    new ApiToken(Config.ApiKey, GetEnvironmentInformation(), GetUserKey), 
                    new Connectivity(), 
                    new MessageHandlerFactory().CreateHandler()),
                typeof(IApi<ITalkiPlayApi>));
            
            Locator.CurrentMutable.RegisterLazySingleton(() => new AssetRepository(), typeof(IAssetRepository));
            Locator.CurrentMutable.RegisterLazySingleton(() => new UserRepository(), typeof(IUserRepository));
            Locator.CurrentMutable.RegisterLazySingleton(() => new ChildrenRepository(), typeof(IChildrenRepository));
            Locator.CurrentMutable.RegisterLazySingleton(() => new RewardsRepository(), typeof(IRewardsRepository));
            Locator.CurrentMutable.RegisterLazySingleton(() => new GameService(), typeof(IGameService));
            
            Locator.CurrentMutable.Register(() =>
            {
                var logger = Locator.Current.GetService<ILogger>();
                var factory = Locator.Current.GetService<IAudioPlayerFactory>();
                return factory.Create(logger);
                
            }, typeof(IAudioPlayer));
            
            Locator.CurrentMutable.RegisterLazySingleton(() => new GameMediator(), typeof(IGameMediator));
            Locator.CurrentMutable.RegisterLazySingleton(() => ConnectivityManager.Instance, typeof(IConnectivityNotifier));
            Locator.CurrentMutable.RegisterLazySingleton(() => new BackgroundAudioPlayer(Locator.Current.GetService<IAudioPlayerFactory>()), typeof(IBackgroundAudioPlayer));
            Locator.CurrentMutable.RegisterLazySingleton(() =>  new FirmwareService(), typeof(IFirmwareService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new TalkiPlayerManager(), typeof(ITalkiPlayerManager));
            Locator.CurrentMutable.RegisterLazySingleton(() => new TalkiPlayNavigationHelper(),  typeof(ITalkiPlayNavigator));
            //TODO: re-enable once Bluetooth version needed again.
            Locator.CurrentMutable.RegisterLazySingleton(() => CrossBleAdapter.Current,  typeof(IAdapter));
            
            //Locator.CurrentMutable.RegisterLazySingleton(() => new RewardCollector(), typeof(IRewardCollector));
            Locator.CurrentMutable.RegisterLazySingleton(() => CrossDownloadManager.Current, typeof(IDownloadManager));
           }
        
        static async Task<string> GetUserKey()
        {
            return await SecureSettingsService.GetUserKey();
        }
        
        private static void RegisterPages()
        {
            Locator.CurrentMutable.Register(() => new CategoryListPage(), typeof(IViewFor<CategoryListPageViewModel>));

            Locator.CurrentMutable.Register(() => new GuideImagePage(), typeof(IViewFor<GuideImagePageViewModel>));
            Locator.CurrentMutable.Register(() => new GuideChildSelectionPage(), typeof(IViewFor<GuideChildSelectionPageViewModel>));
            Locator.CurrentMutable.Register(() => new GuideQuestionPage(), typeof(IViewFor<GuideQuestionPageViewModel>));
            Locator.CurrentMutable.Register(() => new GuideInfoPage(), typeof(IViewFor<GuideInfoPageViewModel>));
            Locator.CurrentMutable.Register(() => new GuidePackSelectionPage(), typeof(IViewFor<GuidePackSelectionPageViewModel>));
            Locator.CurrentMutable.Register(() => new GuideRecommendationPage(), typeof(IViewFor<GuideRecommendationPageViewModel>));

            Locator.CurrentMutable.Register(() => new DeviceSetupPage(), typeof(IViewFor<DeviceSetupPageViewModel>));
            Locator.CurrentMutable.Register(() => new LoadingPage(), typeof(IViewFor<LoadingPageViewModel>));
            Locator.CurrentMutable.Register(() => new ConnectivityPage(), typeof(IViewFor<ConnectivityPageViewModel>));
            Locator.CurrentMutable.Register(() => new GameListPage(), typeof(IViewFor<GameListPageViewModel>));
            Locator.CurrentMutable.Register(() => new DeviceListPage(), typeof(IViewFor<DeviceListPageViewModel>));
            //Locator.CurrentMutable.Register(() => new RewardListPage(), typeof(IViewFor<RewardListPageViewModel>));
            Locator.CurrentMutable.Register(() => new DevicePage(), typeof(IViewFor<DevicePageViewModel>));
            Locator.CurrentMutable.Register(() => new FirmwareUpdatePage(), typeof(IViewFor<FirmwareUpdatePageViewModel>));
            //Locator.CurrentMutable.Register(() => new LoginPage(), typeof(IViewFor<LoginPageViewModel>));
            


            Locator.CurrentMutable.Register(() => new AcceptInvitePage(),typeof(IViewFor<AcceptInvitePageViewModel>));
            Locator.CurrentMutable.Register(() => new ResetPasswordPage(),typeof(IViewFor<ResetPasswordPageViewModel>));
            Locator.CurrentMutable.Register(() => new ForgotPasswordPage(),typeof(IViewFor<ForgotPasswordPageViewModel>));
            
            //Locator.CurrentMutable.Register(() => new ChangePasswordPage(),typeof(IViewFor<ChangePasswordPageViewModel>));
            //Locator.CurrentMutable.Register(() => new EditUserDetailsPage(),typeof(IViewFor<EditUserDetailsPageViewModel>));
            
            Locator.CurrentMutable.Register(() => new PackListPage(),typeof(IViewFor<IPackPageViewModel>));
            Locator.CurrentMutable.Register(() => GetChildPage(() => new PackListPage(),
                navigator => new PackListPageViewModel(navigator)), typeof(IViewFor<NavigationItemViewModel<PackListPageViewModel>>));
            Locator.CurrentMutable.Register(() => GetChildPage(() => new PackListPage(),
                navigator => new ItemsPackListPageViewModel(navigator)), typeof(IViewFor<NavigationItemViewModel<ItemsPackListPageViewModel>>));

            Locator.CurrentMutable.Register(() => new TagItemStartPage(),typeof(IViewFor<TagItemStartPageViewModel>));
            Locator.CurrentMutable.Register(() => new TagItemStartPage(),typeof(IViewFor<ItemsTagItemStartPageViewModel>));
            Locator.CurrentMutable.Register(() => new TagItemSetupPage(),typeof(IViewFor<TagItemSetupPageViewModel>));
            Locator.CurrentMutable.Register(() => new TagItemSetupPage(),typeof(IViewFor<ItemsTagItemSetupPageViewModel>));

            Locator.CurrentMutable.Register(() => new ItemsListPage(), typeof(IViewFor<ItemListPageViewModel>));
            
            Locator.CurrentMutable.Register(() => GetChildPage(() => new TagItemStartPage(),
                navigator => new TagItemStartPageViewModel(navigator)), typeof(IViewFor<NavigationItemViewModel<TagItemStartPageViewModel>>));
            Locator.CurrentMutable.Register(() => GetChildPage(() => new TagItemStartPage(),
                navigator => new ItemsTagItemStartPageViewModel(navigator)), typeof(IViewFor<NavigationItemViewModel<ItemsTagItemStartPageViewModel>>));
            Locator.CurrentMutable.Register(() => GetChildPage(() => new DeviceListPage(),
                navigator => new DeviceListPageViewModel(navigator)), typeof(IViewFor<NavigationItemViewModel<DeviceListPageViewModel>>));

            //Locator.CurrentMutable.Register(() => new ChildListPage(), typeof(IViewFor<ChildListPageViewModel>));
            // Locator.CurrentMutable.Register(() => new AddEditChildPage(), typeof(IViewFor<AddEditChildPageViewModel>));
            // Locator.CurrentMutable.Register(() => new AvatarSelectionPage(), typeof(IViewFor<AvatarSelectionPageViewModel>));
            // Locator.CurrentMutable.Register(() => GetChildPage(() => new AddEditChildPage(),
            //     navigator => new AddEditChildPageViewModel(navigator)), typeof(IViewFor<NavigationItemViewModel<AddEditChildPageViewModel>>));
            // Locator.CurrentMutable.Register(() => new ChildDetailsPage(), typeof(IViewFor<ChildDetailsPageViewModel>));

            Locator.CurrentMutable.Register(() => new RoomListPage(), typeof(IViewFor<RoomListPageViewModel>));
            Locator.CurrentMutable.Register(() => new RoomListPage(), typeof(IViewFor<ItemsRoomListPageViewModel>));
            Locator.CurrentMutable.Register(() => new RoomListPage(), typeof(IViewFor<GameRoomListPageViewModel>));
            Locator.CurrentMutable.Register(() => new AddEditRoomPage(), typeof(IViewFor<AddEditRoomPageViewModel>));
            Locator.CurrentMutable.Register(() => new RoomImageSelectionPage(), typeof(IViewFor<RoomImageSelectionPageViewModel>));
            Locator.CurrentMutable.Register(() => GetChildPage(() => new AddEditRoomPage(),
                navigator => new AddEditRoomPageViewModel(navigator)), typeof(IViewFor<NavigationItemViewModel<AddEditRoomPageViewModel>>));
            
            //Locator.CurrentMutable.Register(() => new GameConfigurationPage(), typeof(IViewFor<GameConfigurationPageViewModel>));            
            
            // Locator.CurrentMutable.Register(() => GetChildPage(() => new ChildListPage(),
            //      navigator => new GameChildListPageViewModel(navigator)), typeof(IViewFor<NavigationItemViewModel<GameChildListPageViewModel>>));
            //
            Locator.CurrentMutable.Register(() => new GameActiveItemsConfigurationPage(),  typeof(IViewFor<GameActiveItemsConfigurationPageViewModel>));
            Locator.CurrentMutable.Register(() => GetChildPage(() => new TapToPairPage(),
                navigator => new TapToPairPageViewModel(navigator)), typeof(IViewFor<NavigationItemViewModel<TapToPairPageViewModel>>));
            Locator.CurrentMutable.Register(() => new TalkiPlayerPairingPage(), typeof(IViewFor<TalkiPlayerPairingPageViewModel>));
            Locator.CurrentMutable.Register(() => new BluetoothWarningPopup(), typeof(IViewFor<BluetoothWarningPopupPageViewModel>));
            Locator.CurrentMutable.Register(() => new GameSessionPage(), typeof(IViewFor<GameSessionPageViewModel>));
            Locator.CurrentMutable.Register(() => new TestBleRequestSelectionPage(), typeof(IViewFor<TestBleRequestPageViewModel>));
            Locator.CurrentMutable.Register(() => new ConfirmationDialogPage(), typeof(IViewFor<ConfirmationDialogPageViewModel>));
            Locator.CurrentMutable.Register(() => new AudioListPage(),  typeof(IViewFor<AudioListPageViewModel>));
            Locator.CurrentMutable.Register(() => new VolumeControlPage(),   typeof(IViewFor<VolumeControlPageViewModel>));
            Locator.CurrentMutable.Register(() => new ShakeTalkiPlayerPopup(),   typeof(IViewFor<ShakeTalkiPlayerPopUpPageViewModel>));
            Locator.CurrentMutable.Register(() => new ScanHomeTagPopup(),    typeof(IViewFor<ScanHomeTagPopupPageViewModel>));
            Locator.CurrentMutable.Register(() => new AudioPackListPage(),    typeof(IViewFor<AudioPackListPageViewModel>));
            Locator.CurrentMutable.Register(() => new ChestPage(),     typeof(IViewFor<ChestPageViewModel>));

            Locator.CurrentMutable.Register(() => new TestPage(), typeof(IViewFor<TestPageViewModel>));
            Locator.CurrentMutable.Register(() => new WebPage(), typeof(IViewFor<WebPageViewModel>));

            Locator.CurrentMutable.Register(() => new SettingsPage(), typeof(IViewFor<SettingsPageViewModel>));
            Locator.CurrentMutable.Register(() => new RewardsChildListPage(), typeof(IViewFor<RewardsChildListPageViewModel>));
            Locator.CurrentMutable.Register(() => new PacksRewardPage(), typeof(IViewFor<PacksRewardPageViewModel>));
            Locator.CurrentMutable.Register(() => new QRCodePdfListPage(), typeof(IViewFor<QRCodePdfListPageViewModel>));

            Locator.CurrentMutable.Register(() => new ModeSelectionPage(), typeof(IViewFor<ModeSelectionPageViewModel>));
            Locator.CurrentMutable.Register(() => new NewModeSelectionPage(), typeof(IViewFor<NewModeSelectionPageViewModel>));
            //Locator.CurrentMutable.Register(() => GetChildPage(() => new ModeSelectionPage(),
            //    navigator => new ModeSelectionPageViewModel(navigator)), typeof(IViewFor<NavigationItemViewModel<ModeSelectionPageViewModel>>));

            //Locator.CurrentMutable.Register(() => new GameQRCodeSessionPage(), typeof(IViewFor<GameQRCodeSessionPageViewModel>));
            Locator.CurrentMutable.Register(() => new OnboardingPage(), typeof(IViewFor<OnboardingPageViewModel>));

            Locator.CurrentMutable.Register(() => new LegalLinksPage(), typeof(IViewFor<LegalLinksPageViewModel>));
        }
    }
}