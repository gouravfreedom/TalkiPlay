using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Mobile.Logging;
using ChilliSource.Mobile.UI.ReactiveUI;
using Plugin.BluetoothLE;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public class ReaderWarningPopupPageViewModel : ReactiveObject, IPopModalViewModel
    {
        private readonly ILogger _logger;
        private readonly IUserDialogs _toaster;
        private readonly INavigationService _navigator;
        private readonly IUserSettings _userSettings;

        public ReaderWarningPopupPageViewModel(Exception ex,
            INavigationService navigator = null,
            IUserSettings userSettings = null,
            IUserDialogs userDialogs = null,
            ILogger logger = null)
        {
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            _toaster = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            _navigator = navigator ?? Locator.Current.GetService<INavigationService>(Constants.MainNavigation);
            
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            Message = !String.IsNullOrWhiteSpace(_userSettings.ReaderDeviceId) 
                ? "Reader doesn't seem to be connected. Please connect." 
                : "Reader doesn't seem to be properly setup. Please tap \"Parent tab\" to setup.";
            ButtonText = !String.IsNullOrWhiteSpace(_userSettings.ReaderDeviceId) ? "Connect reader" : "Setup reader";

            GoToParentTabPage = ReactiveCommand.CreateFromObservable(() =>
            {
                var tabs = Locator.Current.GetService<ITabService>();
                if (!String.IsNullOrWhiteSpace(_userSettings.ReaderDeviceId))
                {
                    return CrossBleAdapter.Current.GetKnownDevice(Guid.Parse(_userSettings.ReaderDeviceId))
                        .SelectMany(device =>
                        {
                            if (device == null)
                            {
                                return tabs.ChangeTab(TabItemType.Settings);
                            }

                            return _navigator.PushPopup(new LoadingPageViewModel
                                {
                                    Message = "Connecting ...."
                                })
                                .ObserveOn(RxApp.TaskpoolScheduler)
                                .SelectMany(_ => Connect(device))
                                .ObserveOn(RxApp.MainThreadScheduler)
                                .SelectMany(_ => _navigator.PopPopup(resetStack: true))
                                .Do(_ => _toaster.Toast("Successfully connected to reader"));
                        });
                }
                
                return tabs.ChangeTab(TabItemType.Settings);
            });

            GoToParentTabPage.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(error => _navigator.PopPopup(resetStack: true)
                    .Select(m => error))
                .SubscribeAndLogException();
        }

        public string Title => "";
        [Reactive]
        public string Message { get; set; }
        
        [Reactive]
        public string ButtonText { get; set; }
        
        public ReactiveCommand<Unit, Unit> GoToParentTabPage { get; }
        
        private IObservable<bool> Connect(IDevice device)
        {
            return Observable.Start(() => device.Connect())
                .Select(_ => true);
        }
    }
}
