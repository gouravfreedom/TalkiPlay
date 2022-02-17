﻿using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using FormsControls.Base;
using Plugin.BluetoothLE;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace TalkiPlay.Shared
{
    public class TapToPairPageViewModel  : BasePageViewModel, IActivatableViewModel, IModalViewModel
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IConnectivityNotifier _connectivityNotifier;
        //private readonly IGameMediator _gameMediator;
        private readonly ITalkiPlayerManager _manager;

        public TapToPairPageViewModel(INavigationService navigator,
            IConnectivityNotifier connectivityNotifier = null,
            //IGameMediator gameMediator = null,
            ITalkiPlayerManager manager = null)
        {
            
            _connectivityNotifier = connectivityNotifier;
            //_gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
            _manager = manager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            Activator = new ViewModelActivator();
            Navigator = navigator;
            SetupCommand();
            SetupRx();
        }

        public override string Title => "Tap to pair";

        [Reactive] 
        public string NavigationTitle { get; set; }
        public ViewModelActivator Activator { get; }

        [Reactive] 
        public string InstructionImage { get; set; }

        public ReactiveCommand<Unit, Unit> GoCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> LoadCommand { get; protected set; }

        void SetupCommand()
        {
            GoCommand = ReactiveCommand.CreateFromTask(() =>
            {
                var navCommand = ReactiveCommand.CreateFromObservable<INavigationService, Unit>(nav =>
                    {
                        return nav.PushPage(new TalkiPlayerPairingPageViewModel(nav));
                    });

                var config = new ConnectionConfig()
                {
                    AutoConnect = true,
                };

                var vm = new DeviceListPageViewModel(Navigator, navigateCommand: navCommand, connectionConfig: config)
                {
                    CanOpenDeviceDetailsWhenAlreadyConnected =  false
                };

                return SimpleNavigationService.PushAsync(vm);
            });

            GoCommand.ThrownExceptions.SubscribeAndLogException();
            
        
            LoadCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                _manager.Current?.Disconnect();
                _manager.Current?.Dispose();

                return ObservableOperatorExtensions.StartShowLoading("Loading ...")
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .SelectMany(_ => _assetRepository.GetItems(ItemType.Home))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .HideLoading()
                    .Do(items =>
                    {
                        var item = items.FirstOrDefault();
                        InstructionImage = item?.ImagePath.ToResizedImage(height: 250) ?? Images.PlaceHolder;
                    })
                    .Select(_ => Unit.Default);
            });
            
            LoadCommand.ThrownExceptions
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();

            BackCommand = ReactiveCommand.CreateFromTask(() => SimpleNavigationService.PopModalAsync());
            
            BackCommand.ThrownExceptions.SubscribeAndLogException();
        }

        void SetupRx()
        {
           this.WhenActivated(d =>
           {
                _connectivityNotifier.Notifier.RegisterHandler(async context =>
               {
                   await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                   context.SetOutput(true);
               }).DisposeWith(d);
             
           });
        }       
    }
}
