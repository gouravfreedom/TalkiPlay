using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using TalkiPlay.Shared;

namespace TalkiPlay
{
    public class ConnectivityManager : IDisposable, IConnectivityNotifier
    {
        bool _isMonitoringConnectivity;
        bool _isConnectivityPageShowing;
        private INavigationService _navigator;
        static readonly Lazy<ConnectivityManager> LazyInstance = new Lazy<ConnectivityManager>(() => new ConnectivityManager());
        static readonly Lazy<NetworkConnectionNotifier> LazyNotifierInstance = new Lazy<NetworkConnectionNotifier>(() => new NetworkConnectionNotifier());
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private ConnectivityManager()
        {
            Notifier = new NetworkConnectionNotifier();
        }

        public static ConnectivityManager Instance => LazyInstance.Value;
        public NetworkConnectionNotifier Notifier { get; }
        
        public void InitializeConnectionMonitoring(INavigationService navigator)
        {
            _navigator = navigator;
            if (!_isMonitoringConnectivity)
            {
                Xamarin.Essentials.Connectivity.ConnectivityChanged += (sender, e) =>
                {
                    _isMonitoringConnectivity = true;
                    
                    if (e.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet)
                    {
                        HandlConnectivityRestored();
                      
                    }
                    else
                    {
                        HandleNoConnectivity();
                    }
                };
            }
        }

        private void HandlConnectivityRestored()
        {
            if (_isConnectivityPageShowing)
            {
				_navigator.PushPopup(new ConnectivityPageViewModel())
                    .StartWith(RxApp.MainThreadScheduler)
                    .SubscribeSafe(_ =>
                    {
                       // Messenger.Send(Notifications.ConnectivityRestored);
                        _isConnectivityPageShowing = false;
                    }).DisposeWith(_disposables);
            }
        }

        private void HandleNoConnectivity()
        {
            if (_isConnectivityPageShowing) return;
            
            _isConnectivityPageShowing = true;
            
            if (_navigator.HasModalInStack)
            {
                _navigator.PopModal()
                    .StartWith(RxApp.MainThreadScheduler)
                    .SubscribeSafe()
                    .DisposeWith(_disposables);
            }
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
