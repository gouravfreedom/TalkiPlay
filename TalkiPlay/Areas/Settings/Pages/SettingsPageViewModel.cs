﻿using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Essentials;
using System;
 using TalkiPlay.Managers;

 namespace TalkiPlay.Shared
{
    public class SettingsPageViewModel  : BasePageViewModel, IActivatableViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private readonly ITalkiPlayNavigator _navigatorHelper;
        private readonly IUserRepository _userService;
        private readonly IConfig _config;
        private readonly IUserSettings _userSettings;        
        
        public SettingsPageViewModel(INavigationService navigator, 
            IUserSettings userSettings = null,
            IUserRepository userService = null,
            ITalkiPlayNavigator navigatorHelper = null,
            IUserDialogs userDialogs = null,
            IConfig config = null)
        {
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            _navigatorHelper = navigatorHelper ?? Locator.Current.GetService<ITalkiPlayNavigator>();
            _userService = userService ?? Locator.Current.GetService<IUserRepository>();
            Activator = new ViewModelActivator();
            _config = config ?? Locator.Current.GetService<IConfig>();
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            Navigator = navigator;
            SetupCommands();
            SetupRx();     
        }

        public override string Title => "Settings";

        public ViewModelActivator Activator { get; }

        public List<object> SettingItems { get; } = new List<object>();

        public ReactiveCommand<SettingsItemViewModel, Unit> SelectCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> LogoutCommand { get; private set; }
        
        public extern bool IsBusy { [ObservableAsProperty] get; }

        void SetupRx()
        {            
            
            MessageBus.Current.Listen<SubscriptionChangedMessage>()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe((x) => LoadData().Forget());
            
            this.WhenActivated(d =>
            {
                LoadData().Forget();

                this.WhenAnyObservable(m => m.LogoutCommand.IsExecuting)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(m => m)
                    .ToPropertyEx(this, v => v.IsBusy)
                    .DisposeWith(d);               
            });
        }

        void ToggleGameMusic(bool isOn)
        {            
            _userSettings.PlayInGameMusic = isOn;
        }
        
        void SetupCommands()
        {
            SelectCommand = ReactiveCommand.CreateFromObservable<SettingsItemViewModel, Unit>(m =>
            {
                switch (m.Type)
                {
                    case SettingsType.DeviceItems:
                        var deviceMgr = Locator.Current.GetService<ITalkiPlayerManager>();
                        if (deviceMgr.Current == null || deviceMgr.Current.IsConnected == false)
                        {
                            SimpleNavigationService.PushModalAsync(new DeviceSetupPageViewModel(DeviceSetupSource.Connect)).Forget();
                        }
                        else
                        {
                            SimpleNavigationService.PushChildModalPage(new DevicePageViewModel(null)).Forget();
                        }
                        break;
                    case SettingsType.DownloadPrintable:
                        SimpleNavigationService.PushModalWithNavigationAsync(new QRCodePdfListPageViewModel()).Forget();
                        break;
                        //return Navigator.PushModal(new NavigationItemViewModel<DeviceListPageViewModel>());
                    case SettingsType.UpdateDetails:
                        SimpleNavigationService.PushModalWithNavigationAsync(new EditUserDetailsPageViewModel(null)).Forget();
                        break;
                        //return Navigator.PushPage(new EditUserDetailsPageViewModel(Navigator));
                    case SettingsType.UpdatePassword:
                        SimpleNavigationService.PushModalWithNavigationAsync(new ChangePasswordPageViewModel(null)).Forget();
                        break;
                        //return Navigator.PushPage(new ChangePasswordPageViewModel(Navigator));
                    case SettingsType.RestorePurchases:
                    {
                        RestorePurchases().Forget();
                        break;
                    }
                    case SettingsType.Legal:
                    {
                        SimpleNavigationService.PushAsync(new LegalLinksPageViewModel()).Forget();
                        
                        break;
                    }
                    case SettingsType.Help:
                    {
                        WebpageHelper.OpenUrl(Config.HelpUrl, "FAQ & Training");
                        
                        break;
                    }
                    case SettingsType.SwitchMode:
                        {
                            SimpleNavigationService.PushModalWithNavigationAsync(new NewModeSelectionPageViewModel()).Forget();
                            break;        
                        }

                }
                return Observable.Return(Unit.Default);
            });

            SelectCommand.ThrownExceptions.SubscribeAndLogException();

            LogoutCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _userDialogs.ShowLoading("Signing out ...");
                await _userService.Logout();
                _userDialogs.HideLoading();
                _navigatorHelper.NavigateToLoginPage();
            });
         
            LogoutCommand.ThrownExceptions                
                .ShowExceptionDialog()
                .SubscribeAndLogException();           
        }

        async Task RestorePurchases()
        {
            Dialogs.ShowLoading();

            try
            {
                var result =
                    await SubscriptionService.RestoreSubscriptions(_userService, AppInfo.PackageName);

                if (result.IsSuccessful)
                {
                    await SecureSettingsService.UpdateUserSubscriptionStatus(UserSubscriptionStatus.AppStore);
                    await _userService.UpdateCompany(new CompanyPatchRequest(true, result.Result));

                    Dialogs.HideLoading();
                    Dialogs.Alert("Subscription successfully restored!","Subscriptions");
                    MessageBus.Current.SendMessage(new SubscriptionChangedMessage());
                }
                else
                {
                    Dialogs.HideLoading();
                    Dialogs.Alert("No active subscriptions found. Please sign up for a new subscription.", "Subscriptions");
                }
            
            }
            catch (Exception e)
            {
                Dialogs.HideLoading();
                Dialogs.Alert("An error has occured. Please try again later.","Subscriptions");
                Serilog.Log.Error(e, e.Message);
            }
        }
        
        async Task LoadData()
        {

            SettingItems.Clear();

            var profileItems = new SettingsItemGroup() { Title = "Profile" };
            SettingItems.Add(profileItems);

            profileItems.Add(new SettingsItemViewModel()
            {
                Label = "Update my details",
                Type = SettingsType.UpdateDetails
            });

            profileItems.Add(new SettingsItemViewModel()
            {
                Label = "Change my password",
                Type = SettingsType.UpdatePassword
            });

            var user = await SecureSettingsService.GetUser();
            if (user != null && user.SubscriptionStatus != UserSubscriptionStatus.Stripe)
            {
                profileItems.Add(new SettingsItemViewModel()
                {
                    Label = "Restore purchases",
                    Type = SettingsType.RestorePurchases
                });
            }

            profileItems.Add(new SettingsItemViewModel()
            {
                Label = $"FAQ & Training",
                Type = SettingsType.Help
            });



            var modeItems = new SettingsItemGroup() { Title = _userSettings.HasTalkiPlayerDevice ? "My TalkiPlayer" : "My Print Outs" };
            SettingItems.Add(modeItems);

            if (_userSettings.HasTalkiPlayerDevice)
            {
                modeItems.Add(new SettingsItemViewModel()
                {
                    Label = $"Manage my {Constants.DeviceName}",
                    Type = SettingsType.DeviceItems
                });
            }
            else
            {
                modeItems.Add(new SettingsItemViewModel()
                {
                    Label = "Download printables",
                    Type = SettingsType.DownloadPrintable
                });
            }


            var settingItems = new SettingsItemGroup() { Title = "Settings" };
            SettingItems.Add(settingItems);


            settingItems.Add(new SettingsItemViewModel()
            {
                Label = $"Terms & Conditions",
                Type = SettingsType.Legal
            });

            settingItems.Add(new SettingsItemViewModel()
            {
                Label = $"Switch scanning mode",
                Type = SettingsType.SwitchMode
            });

            if (_userSettings.HasTalkiPlayerDevice)
            {
                settingItems.Add(new ToggleItemViewModel(ToggleGameMusic)
                {
                    Title = "Game play music",
                    IsOn = _userSettings.PlayInGameMusic,
                });
            }

            settingItems.Add(new DeviceInfoViewModel()
            {
                Label = $"App version",
                Value = $"{VersionTracking.CurrentVersion} ({VersionTracking.CurrentBuild})"
            });           
        }
    }
}
