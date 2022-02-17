using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using Humanizer;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public class GameSessionViewModel : ReactiveObject, IDisposable
    {
        private readonly INavigationService _navigator;
        private readonly ReactiveCommand<GameSessionViewModel, Unit> _selectCommand;
        private readonly IUserDialogs _userDialogs;
        private readonly IUserSettings _userSettings;
        private readonly IGameService _gameService;
        private readonly ILogger _logger;
        private readonly IGameMediator _gameMediator;
        private ITalkiPlayer _talkiPlayer;
        private readonly CompositeDisposable _disposable;

        public GameSessionViewModel(
            INavigationService navigator,
            ITalkiPlayer talkiPlayer,
            bool isReload,
            GameSessionStatus status,
            ReactiveCommand<GameSessionViewModel, Unit> selectCommand,
            IGameMediator gameMediator = null,
            IGameService gameService = null,
            ILogger logger = null,
            IUserSettings userSettings = null,
             CompositeDisposable disposables = null,
            IUserDialogs userDialogs = null)
        {
            _navigator = navigator;
            _talkiPlayer = talkiPlayer;
            _selectCommand = selectCommand;
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            _disposable = disposables ?? new CompositeDisposable();
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            _gameService = gameService ?? Locator.Current.GetService<IGameService>();
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
            GameStatus = status;
            //_disposable = disposables ?? new CompositeDisposable();
            
            Title = status.Humanize();

            SetupEventHandlers();
            SetupCommands();

            HandleStatus(status);

            var service = Locator.Current.GetService<IApplicationService>();
            EggHeight = service.ScreenSize.Height * 0.35;
        }

        [Reactive] public string DeviceName { get; set; }

        [Reactive] public string Title { get; set; }
        [Reactive] public string Description { get; set; }

        [Reactive] public GameSessionStatus GameStatus { get; set; } = GameSessionStatus.Pending;

        [Reactive] public bool HasError { get; set; }


        public Guid DeviceId { get; set; }

        public extern bool IsReady { [ObservableAsProperty] get; }
        
        public ReactiveCommand<Unit, Unit> StartCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ConnectCommand { get; set; }
        
        public bool HasTalkiPlayer => _talkiPlayer != null;


        public double EggHeight { get; set; }
        [Reactive] public string RewardEgg { get; set; }
        [Reactive] public bool StartPlayingAnim { get; set; } = false;
        public ReactiveCommand<Unit, Unit> CrackEggCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DoneCommand { get; private set; }

        void SetupEventHandlers()
        {
            _talkiPlayer?.WhenConnectionFailed()
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Subscribe(d =>
                   {
                       HasError = true;
                   })
                   .DisposeWith(_disposable);

            _talkiPlayer?.WhenAnyValue(v => v.ConnectionStatus)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(v =>
                {
                    _logger.Information($"Connection status: {v}");

                    switch (v)
                    {
                        case Plugin.BluetoothLE.ConnectionStatus.Disconnected:
                            if (GameStatus == GameSessionStatus.Pending)
                            {
                                HandleStatus(GameSessionStatus.WaitingForDevice);
                            }
                            break;
                    }
                })
                .DisposeWith(_disposable);

            _talkiPlayer?.OnDataResult()
                .ObserveOn(RxApp.TaskpoolScheduler)
                .SelectMany(result =>
                {
                    switch (result.Type)
                    {
                        case UploadDataType.GameResult:
                            return HandleGameResult(result);                        
                    }

                    return Observable.Return(result);
                })
                .Where(a => a != null)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(result =>
                {
                    if (result.Type == UploadDataType.GameResult)
                    {
                        HandleStatus(result.IsSuccess ? GameSessionStatus.RewardsCeremony : GameSessionStatus.Failed);
                    }
                    else if (result.Type == UploadDataType.DeviceInfo)
                    {
                        //this will be triggered if the user scans a home tag
                        //while the game is in progress, resulting in the game
                        //being automatically ended
                        if (GameStatus == GameSessionStatus.WaitingForDevice)
                        {                                                      
                            RequestGameResult();
                        }
                    }
                }, ex =>
                {
                    HandleStatus(GameSessionStatus.Failed);
                })
                .DisposeWith(_disposable);


            _talkiPlayer?.IsReady
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToPropertyEx(this, v => v.IsReady)
                .DisposeWith(_disposable);

            this.WhenAnyValue(m => m.GameStatus)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeSafe(sessionStatus =>
                {
                    var session = _userSettings.CurrentGameSession;
                    if (session == null)
                    {
                        return;
                    }
                    var sessionRecord = session.GameSessionRecords.FirstOrDefault(a => a.DeviceName == DeviceName);
                    if (sessionRecord != null)
                    {
                        sessionRecord.Status = sessionStatus;
                    }
                    _userSettings.CurrentGameSession = session;
                })
                .DisposeWith(_disposable);
        }

        void SetupCommands()
        {
            StartCommand = ReactiveCommand.Create(() =>
            {
                HandleStatus(GameSessionStatus.Pending);
            });

            StartCommand.ThrownExceptions.SubscribeAndLogException();

            ConnectCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                var observable = _talkiPlayer != null ? _talkiPlayer.Connect() : Observable.Return(false);
                return observable
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(m => Unit.Default);
            });

            ConnectCommand.ThrownExceptions.SubscribeAndLogException();

            CrackEggCommand = ReactiveCommand.Create(() =>
            {
                if (!StartPlayingAnim )
                {
                    StartPlayingAnim = true;
                }
            });

            DoneCommand = ReactiveCommand.CreateFromTask(() =>
            {
                HandleStatus(GameSessionStatus.Completed);
                return Task.CompletedTask;
            });
        }

        void RequestGameResult()
        {
            HandleStatus(GameSessionStatus.WaitingForResult);

            _talkiPlayer?.Upload(
                 new DataUploadData("GameResult", DataRequest.GameResultRequest(), DeviceName, UploadDataType.GameResult),
                 TimeSpan.FromMinutes(10));
        }

        async Task ShowRewardPopup()
        {
            IReward reward = null;
            IAsset rewardAsset = null; 
            if (_gameMediator.CurrentGame?.Chest?.Rewards != null)
            {
                reward = _gameMediator.CurrentGame.Chest.Rewards.Shuffle().FirstOrDefault();
                if (reward != null)
                {
                    var assetRepo =  Locator.Current.GetService<IAssetRepository>();
                    rewardAsset = await assetRepo.GetAssetById(reward.OpenImageAssetId);
                    RewardEgg = rewardAsset?.ImageContentPath;
                }
            }

            Observable.Timer(TimeSpan.FromSeconds(1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(_ =>
                {
                    Title = "Well done!";
                    Description = "Tap to crack the egg and\ncollect your reward";
                    StartPlayingAnim = false;
                    GameStatus = GameSessionStatus.RewardsCeremony;
                    return Unit.Default;
                })
                .SubscribeSafe();
        }


        void HandleStatus(GameSessionStatus status)
        {
            switch (status)
            {
                case GameSessionStatus.Pending:                    
                    Title = $"Tap your home tag to\n release {_gameMediator?.CurrentGame?.Name}\n to your TalkiPlayer";
                    Description = "";
                    break;
                case GameSessionStatus.WaitingForDevice:
                    Title = "To End\nTap your home tag";
                    Description = $"{_userSettings?.CurrentChild?.Name} is playing {_gameMediator?.CurrentGame?.Name}";
                    break;
                case GameSessionStatus.WaitingForResult:
                    Title = "";
                    Description = "";
                    break;
                case GameSessionStatus.RewardsCeremony:
                    ShowRewardPopup().Forget();
                    return;
                case GameSessionStatus.Completed:
                    Title = "";
                    break;
                case GameSessionStatus.Failed:
                    Title = "Failed to get game result. Please try again!";
                    Observable.Timer(TimeSpan.FromSeconds(5), RxApp.MainThreadScheduler)
                            .Do((m) => HandleStatus(GameSessionStatus.Completed)).Subscribe();
                    break;
            }

            GameStatus = status;
        }

        IObservable<IDataUploadResult> HandleGameResult(IDataUploadResult result)
        {
           if (result.IsSuccess)
           {
               var data = result.GetData<GameResult>();
               var gameSession = _userSettings.CurrentGameSession;
               var deviceRecord =
                    gameSession.GameSessionRecords.FirstOrDefault(a => a.DeviceName == this.DeviceName);

                if (deviceRecord != null)
                {
                    var gameTime = deviceRecord.GameStartOn;
                    var startTime = gameTime;
                    var endTime = gameTime.AddMilliseconds(data.ElapsedTime);
                    var itemCurrentTime = gameTime;

                    var itemResults = new List<GameSessionItemResult>();

                    foreach (var dataItemResult in data.ItemResults.OrderBy(m => m.ElapsedTime))
                    {
                        var itemEndTime = gameTime.AddMilliseconds(dataItemResult.ElapsedTime);

                        itemResults.Add(new GameSessionItemResult()
                        {
                            Status = dataItemResult.IsSuccess ? ItemResultStatus.Success : ItemResultStatus.Failure,
                            StartTime = itemCurrentTime,
                            EndTime = itemEndTime,
                            FailureCount = dataItemResult.NumberOfFailedAttempts,
                            ItemId = dataItemResult.ItemId
                        });

                        itemCurrentTime = itemEndTime;
                    }

                    var items = itemResults.Where(a => a.ItemId > 0).ToList();
                    
                    return _gameService.RecordGameResultOld(new RecordGameSessionResultRequest()
                    {
                        GameId = gameSession.GameId,
                        RoomId = gameSession.RoomId,
                        StartTime = startTime,
                        EndTime = endTime,
                        Children = deviceRecord.ChildId.ToList(),
                        Results = items,
                        DeviceName = deviceRecord.DeviceName
                    }, _gameMediator.CurrentPack.Id)
                        .RetryAfter(3, TimeSpan.FromMilliseconds(200), RxApp.TaskpoolScheduler)
                        .ToResult()
                        .Select(m =>
                            m.IsSuccessful ? result : DataUploadResult.Failed(result.Tag, result.Type, m.Exception));
                }
            }
           
           return Observable.Return(result);
        }
             
        public void Dispose()
        {
            _disposable.Clear();
        }

        public void Clean()
        {
            //IMPORTANT: This is disconnecting device
            //_talkiPlayer?.Disconnect();
            //_talkiPlayer?.Dispose();
        }

        public GameSessionViewModel CopyWith(ITalkiPlayer talkiPlayer, CompositeDisposable disposable)
        {
            return new GameSessionViewModel(_navigator, talkiPlayer, false, GameStatus, _selectCommand,
                _gameMediator, _gameService, _logger, _userSettings, disposable, _userDialogs)
            {
                DeviceId = this.DeviceId,
                DeviceName =  this.DeviceName
            };
        }
    }
}