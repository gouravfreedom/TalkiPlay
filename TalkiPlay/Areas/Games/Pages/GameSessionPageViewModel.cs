using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using DynamicData.Alias;
using Humanizer;
using Plugin.BluetoothLE;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Forms;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public class GameSessionPageViewModel  : BasePageViewModel, IActivatableViewModel
    {
        //private readonly bool _reloadGameSession;
        private readonly IChildrenRepository _childrenService;
        private readonly IUserSettings _userSettings;
        private readonly IUserDialogs _userDialogs;
        //private readonly ILogger _logger;
        private readonly IConfig _config;
        //private readonly ITalkiPlayerManager _talkiPlayerManager;
        private readonly IGameMediator _gameMediator;
        private readonly IConnectivityNotifier _connectivityNotifier;
        private readonly IGameService _gameService;
        private readonly IAssetRepository _assetRepository;
        private readonly SourceList<GameSessionViewModel> _gameSessions = new SourceList<GameSessionViewModel>();
        private IDisposable _scanDisposable;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly SourceList<ITalkiPlayer> _deviceList = new SourceList<ITalkiPlayer>();
        private readonly SourceList<GameSessionViewModel> _activeDeviceList = new SourceList<GameSessionViewModel>();
        //private IDisposable _timeoutDisposable;

        public GameSessionPageViewModel(
            bool reloadGameSession,
            INavigationService navigator,
            IGameService gameService = null,
            IChildrenRepository childrenService = null,
            IConnectivityNotifier connectivityNotifier = null,
            IGameMediator gameMediator = null,
            //ITalkiPlayerManager talkiPlayerManager = null,
            IConfig config = null,
            //ILogger logger = null,
            IUserDialogs userDialogs = null,
            IUserSettings userSettings = null)
        {
            //_reloadGameSession = reloadGameSession;
            _childrenService = childrenService ?? Locator.Current.GetService<IChildrenRepository>();
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            //_logger = logger ?? Locator.Current.GetService<ILogger>();
            _config = config ?? Locator.Current.GetService<IConfig>();
            //_talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            _gameService = gameService ?? Locator.Current.GetService<IGameService>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            Activator = new ViewModelActivator();
            Navigator = navigator;
            BackImageSource = SimpleBasePage.BackImageSource;

            if (!reloadGameSession)
            {
                LoadNewGame();
            }
            else
            {
                LoadCurrentGame();
            }                                  
        }

        public override string Title => "Game session";

        [Reactive] public ReadOnlyObservableCollection<GameSessionViewModel> GameSessions { get; set; }
        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; set; }
        //public ReactiveCommand<Unit, Unit> ReLoadDataCommand { get; set; }
        public ViewModelActivator Activator { get; }
        public ReactiveCommand<GameSessionViewModel, Unit> SelectCommand { get; protected set; }

        public ReactiveCommand<Unit, Unit> EndGameCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> SkipCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PlayNextGameCommand { get; protected set; }

        public extern int CompletedCount { [ObservableAsProperty] get; }

        [Reactive] public string GameTitle { get; set; }

        [Reactive] public bool IsInitialized { get; set; }
        [Reactive] public bool IsCurrentGameDone { get; set; }
        [Reactive] public IGame NextGame { get; set; }
        [Reactive] public ImageSource BackImageSource { get; set; }


        void LoadNewGame()
        {

            _userSettings.CurrentGameSession = GameSession.Create(_gameMediator.GameSessionId,
                _gameMediator.CurrentGame,
                _gameMediator.CurrentRoom,
                _gameMediator.DeviceWithPlayers
            );

            var game = _gameMediator.CurrentGame;
            var room = _gameMediator.CurrentRoom;
            GameTitle = $"You are playing {game.Name} game in {room.Name} room.";
            SetupCommandsForNewGame();
            SetupRxForNewGame();
        }

        private void LoadCurrentGame()
        {
            SetupCommandsForCurrentGame();
            SetupRxForCurrentGame();
        }

        private void SetupRxForNewGame()
        {
            _gameSessions.Edit(items =>
            {
                var devices = _gameMediator.DeviceWithPlayers.Select(a =>
                {
                    var device = _gameMediator.TalkiPlayers.FirstOrDefault(m => m.Name == a.DeviceName);
                    var child = a.Children.FirstOrDefault();
                    var name = a.Children.Count > 1 ? $"{a.Children.Count} children" : child.Name;
                    var avatar = a.Children.Count > 1
                        ? Images.AvatarPlaceHolder 
                        : child?.PhotoPath.ToResizedImage(Dimensions.DefaultChildImageSize) ?? Images.AvatarPlaceHolder;
                   
                    return new GameSessionViewModel(Navigator, device?.Device, false, GameSessionStatus.Pending,
                        SelectCommand, disposables: _compositeDisposable)
                    {
                        DeviceName = device?.Name,
                        DeviceId = device.DeviceId
                    };
                });

                items.AddRange(devices);
                
            });

            SetupRx();
        }

        private void SetupRxForCurrentGame()
        {
            this.WhenActivated(d =>
            {
                this.WhenAnyObservable(m => m._deviceList.CountChanged)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Where(a => a > 0)
                    .Do(m =>
                    {
                        if (m == _gameSessions.Count)
                        {                            
                            StopScan();
                        }
                    })
                    .SubscribeSafe(m =>
                    {
                        foreach (var talkiPlayer in _deviceList.Items)
                        {
                            var session = _gameSessions.Items.FirstOrDefault(a => a.DeviceName == talkiPlayer.Name && !a.HasTalkiPlayer);
                            if (session != null)
                            {
                                var index = _gameSessions.Items.IndexOf(session);
                                var newGameSession = session.CopyWith(talkiPlayer, _compositeDisposable);
                                _gameSessions.ReplaceAt(index, newGameSession);
                                newGameSession.ConnectCommand.Execute().SubscribeSafe();
                                session.Dispose();
                            }
                        }                    
                    })
                    .DisposeWith(d);
            });
                       
                
            SetupRx();
        }
        
        private void SetupRx()
        {
            this.WhenActivated(d =>
            {
                _connectivityNotifier.Notifier.RegisterHandler(async context =>
                {
                    await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                    context.SetOutput(true);
                }).DisposeWith(d);
                
                _gameSessions.Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out var gameSessions)
                    .SubscribeSafe()
                    .DisposeWith(d);

                _gameSessions.Connect().WhenPropertyChanged(m => m.GameStatus)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .SubscribeSafe(m =>
                    {
                        var item = _activeDeviceList.Items.FirstOrDefault(a => a == m.Sender);
                        BackImageSource = m.Value <= GameSessionStatus.WaitingForResult ? SimpleBasePage.BackImageSource : SimpleBasePage.GameBackImageSource;

                        if (m.Value == GameSessionStatus.RewardsCeremony)
                        {
                            FindNextRecommendation().Forget();
                        }

                        if (item == null && m.Value == GameSessionStatus.Completed)
                        {
                            _activeDeviceList.Edit(a =>
                            {
                                a.Add(m.Sender);
                            });

                            HandleCurrentGameDone();
                        }
                    })
                    .DisposeWith(d);                
                
                 this.WhenAnyObservable(m => m._activeDeviceList.CountChanged)
                     .ObserveOn(RxApp.MainThreadScheduler)
                     .Select(a => a)
                     .ToPropertyEx(this, v => v.CompletedCount)
                     .DisposeWith(d);

                GameSessions = gameSessions;
                d.Add(_compositeDisposable);
            });
        }


        private void SetupCommandsForCurrentGame()
        {
            LoadDataCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                if (!IsInitialized)
                {
                    var currentGameSession = _userSettings.CurrentGameSession;
                    IsInitialized = true;
                    return ObservableOperatorExtensions.StartShowLoading("Loading ...")
                        .ObserveOn(RxApp.TaskpoolScheduler)
                        .SelectMany(m => _assetRepository.GetRoom(currentGameSession.RoomId))
                        .Do(m => _gameMediator.CurrentRoom = m)
                        .SelectMany(m => _gameService.GetGameWithAssets(currentGameSession.GameId, m.TagItems.Select(a => a.Id).ToList()))
                        .Do(m => _gameMediator.CurrentGame = m)
                        .SelectMany(m => _childrenService.GetChildren())
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Do(m =>
                        {

                            var game = _gameMediator.CurrentGame;
                            var room = _gameMediator.CurrentRoom;
                            GameTitle = $"You are playing {game.Name} game in {room.Name} room.";
                            var childIds = currentGameSession.GameSessionRecords.SelectMany(a => a.ChildId);
                            var children = m.Where(a => childIds.Contains(a.Id)).ToList();
                            
                            _gameSessions.Edit(items =>
                            {
                                var devices = currentGameSession.GameSessionRecords.Select(a =>
                                {
                                     var child = children.FirstOrDefault();
                                    var name = children.Count > 1 ? $"{children.Count} children" : child.Name;
                                    var avatar = children.Count > 1
                                        ? Images.AvatarPlaceHolder 
                                        : child?.PhotoPath.ToResizedImage(Dimensions.DefaultChildImageSize) ?? Images.AvatarPlaceHolder;
                   
                                    return new GameSessionViewModel(Navigator, null , true, a.Status,  SelectCommand)
                                    {
                                        Title = a.Status.Humanize(),
                                        DeviceName = a.DeviceName,
                                        DeviceId = a.DeviceId
                                    };
                                });

                                items.AddRange(devices);
                
                            });

                            StartScan();

                        })
                        .HideLoading()
                        .Select(m => Unit.Default);
                }
              
                return Observable.Return(Unit.Default);
            });

            LoadDataCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();
            
            SetupCommand();
        }

        private void SetupCommandsForNewGame()
        {
            LoadDataCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                foreach (var session in _gameSessions.Items)
                {
                    session.StartCommand.Execute().SubscribeSafe();
                }

                return Observable.Return(Unit.Default);
            });

            LoadDataCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();

            SetupCommand();
        }

        private void SetupCommand()
        {
            SelectCommand = ReactiveCommand.CreateFromObservable<GameSessionViewModel, Unit>(m => Observable.Empty<Unit>());
            SelectCommand.ThrownExceptions.SubscribeAndLogException();

            SkipCommand = ReactiveCommand.CreateFromTask(() =>
            {
                SimpleNavigationService.PopToRootAsync().Forget();
                return Task.CompletedTask;
            });

            PlayNextGameCommand = ReactiveCommand.CreateFromTask(async() =>
            {
                await SimpleNavigationService.PushAsync(new GameConfigurationPageViewModel(Navigator, NextGame));
                return Unit.Default;
            });
            
            EndGameCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                StopScan();
                
                if (CompletedCount >= _gameSessions.Count)
                {
                    foreach (var session in _gameSessions.Items)
                    {
                        session.Clean();
                    }
                    
                    _userSettings.ClearGameSession();
                    _gameMediator.Reset();
                    //Navigator.PopToRootPage().SubscribeSafe();
                    SimpleNavigationService.PopToRootAsync().Forget();
                }
                else
                {
                    var ok = await _userDialogs.ConfirmAsync(new ConfirmConfig()
                    {
                        Title = "End game",
                        Message =  $"Not all game sessions are completed. This will discard in-complete game session. Are you sure you want to end game?",
                        OkText = "Yes, end game",
                        CancelText = "No, resume game"
                    });

                    if (ok)
                    {
                        foreach (var session in _gameSessions.Items)
                        {
                            session.Clean();
                        }
                        
                        _gameMediator.Reset();
                        _userSettings.ClearGameSession();
                        //Navigator.PopToRootPage().SubscribeSafe();
                        SimpleNavigationService.PopToRootAsync().Forget();
                    }
                }
                
            });
            
            EndGameCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeAndLogException();

            BackCommand = ReactiveCommand.CreateFromObservable(() => EndGameCommand.Execute());
            
            BackCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeAndLogException();
        }
        

       
        public void StartScan()
        {
            var currentGameSession = _userSettings.CurrentGameSession;
            CrossBleAdapter.Current.StopScan();
            CrossBleAdapter.Current.WhenStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(m => m == AdapterStatus.PoweredOn)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(status =>
                {
                    _scanDisposable = CrossBleAdapter.Current.Scan() 
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Subscribe(scanResult =>
                    {
                        var deviceNames = currentGameSession.GameSessionRecords.Select(a => a.DeviceName).ToList();
                        
                        if (!String.IsNullOrWhiteSpace(scanResult.Device.Name)
                               && scanResult.Device.Name.Contains(_config.DeviceNamePrefix) 
                               && deviceNames.Contains(scanResult.Device.Name))
                        {
        
                            var d = _deviceList.Items.FirstOrDefault(a => a.Name == scanResult.Device.Name);
        
                            if(d == null)
                            {
                                _deviceList.Add(TalkiPlayer.Create(scanResult.Device));
                            }
                          
                        }
                    }, ex =>
                    {
                        var logger = Locator.Current.GetService<ILogger>();
                        logger?.Error(ex);
        
                    });
                }, ex =>
                {
                    var logger = Locator.Current.GetService<ILogger>();
                    logger?.Error(ex);
        
                });
         }
         
         public void StopScan()
         {
             try {
        
                 CrossBleAdapter.Current.StopScan();
                 _scanDisposable?.Dispose();
             
             }
             catch(Exception ex) {
                 ex.LogException("StopScan");
             }
         }

        void HandleCurrentGameDone()
        {            
            IsCurrentGameDone = true;
        }



        public async Task FindNextRecommendation()
        {
            var gameState = new GuideState() {
                SelectedChild = Locator.Current.GetService<IChildrenRepository>().ActiveChild,
                SelectedPack = _gameMediator.CurrentPack
            };

            NextGame = (await gameState.GetGameRecommendations()).FirstOrDefault();
        }
    }
}