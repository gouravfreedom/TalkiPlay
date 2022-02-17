using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData.Alias;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public class TalkiPlayerPairingPageViewModel  : BasePageViewModel, IActivatableViewModel, IModalViewModel
    {
        //private readonly IGameService _gameService;
        //private readonly IConnectivityNotifier _connectivityNotifier;
        private readonly IUserDialogs _userDialogs;
        private readonly ILogger _logger;
        //private readonly IAssetService _assetService;
        private readonly IAssetRepository _assetRepository;
        private readonly ITalkiPlayerManager _talkiPlayerManager;
        private readonly IGameMediator _gameMediator;
        private const string TAG = "GAME";
        bool _isExecuted;
        bool _audioFilesResultHandled;

        GameData _gameData;

        public TalkiPlayerPairingPageViewModel(INavigationService navigator,
            //IGameService gameService = null,
            //IConnectivityNotifier connectivityNotifier = null,
            IGameMediator gameMediator = null,
            //IAssetService assetService = null,
            ITalkiPlayerManager talkiPlayerManager = null,
            IUserDialogs userDialogs = null,
            ILogger logger = null
            )
           
        {
           // _gameService = gameService;
            //_connectivityNotifier = connectivityNotifier;
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            //_assetService = assetService ?? Locator.Current.GetService<IAssetService>();
            _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
            //_connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            //_gameService = gameService ?? Locator.Current.GetService<IGameService>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            Activator = new ViewModelActivator();
            Navigator = navigator;
            SetupRx();
            SetupCommand();
        }

        public override string Title => $"Pair {Constants.DeviceName}";

        public ViewModelActivator Activator { get; }

        [Reactive] public string Message { get; set; }

        public ReactiveCommand<Unit, Unit> TryAgainCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> LoadCommand { get; private set; }

        [Reactive] public bool HasError { get; set; }

                

        void HandleTalkiPlayerGameDataResult(IDataUploadResult result)
        {
            if (result.IsSuccess)
            {
                _gameMediator.Devices.Edit(items =>
                {
                    var device = items.Where(a => !(a is EmptyTalkiPlayerData)).FirstOrDefault(a =>
                        a.Name == _talkiPlayerManager.Current.Device.Name);
                    if (device == null)
                    {
                        items.Add(new TalkiPlayerData()
                        {
                            Name = _talkiPlayerManager.Current?.Name,
                            DeviceId = _talkiPlayerManager.Current.Device.Uuid,
                            Time = _gameData.GameTime,
                            Device = _talkiPlayerManager.Current
                        });
                    }
                });

                var name = _talkiPlayerManager.Current?.Name;
                //_talkiPlayerManager.Clear();
                _talkiPlayerManager.CancelUpload();

                var userDialog = Locator.Current.GetService<IUserDialogs>();
                userDialog.Toast(Dialogs.BuildSuccessToast($"{name} has been paired successfully."));

                SimpleNavigationService.PopModalAsync().Forget();
            }
            else
            {
                HasError = true;
                Message = $"Could not pair with {_talkiPlayerManager.Current?.Name}. Please try again!";
            }
        }

        async Task HandleTalkiPlayerAudioFilesResult(IDataUploadResult result)
        {
            if (result.IsSuccess)
            {
                var data = result.GetData<AvailableAudioFiles>();

                if (data != null)
                {

                    //var currentPack = _gameMediator.CurrentPack;


                    //var instructionsAudioFiles = _gameData.Instructions
                    //    .Where(m => m.Instruction != null)
                    //    .Select(m => m.Instruction?.AudioAsset?.ToLower());

                    //var itemsAudioFiles = _gameData.Instructions
                    //    .Where(a => a.Item != null)
                    //    .Select(m => m.Item?.AudioAssets?.Select(a => a?.ToLower()));

                    //var audioFiles = data.AudioFiles
                    //    .Select(a => a?.ToLower());

                    //var hasInstructionAudioFiles = false;
                    //var hasItemsAudioFiles = false;

                    //if (instructionsAudioFiles.Any())
                    //{
                    //    hasInstructionAudioFiles = instructionsAudioFiles.All(m => audioFiles.Contains(m));
                    //}
                    //else
                    //{
                    //    hasInstructionAudioFiles = true;
                    //}

                    //if (itemsAudioFiles.Any())
                    //{
                    //    var audios = new List<string>();
                    //    foreach (var itemsAudioFile in itemsAudioFiles)
                    //    {
                    //        foreach (var a in itemsAudioFile)
                    //        {
                    //            audios.Add(a);
                    //        }
                    //    }
                    //    hasItemsAudioFiles = audios.All(m => audioFiles.Contains(m));
                    //}
                    //else
                    //{
                    //    hasItemsAudioFiles = true;
                    //}

                    var audioFiles = data.AudioFiles
                      .Select(a => a?.ToLower())
                      .ToList();

                    var currentPack = _gameMediator.CurrentPack;// await _gameService.GetPack(_gameMediator.CurrentPack.Id);
                    bool hasAllPackAudioFiles = currentPack.AudioAssets.All(a => audioFiles.Contains(a.Filename.ToLower()));


                    //if (hasInstructionAudioFiles && hasItemsAudioFiles)
                    if (hasAllPackAudioFiles)
                    {
                        _talkiPlayerManager.Current.Upload(new DataUploadData("GameData", _gameData, TAG,
                            UploadDataType.GameData));
                    }
                    else
                    {                        
                        var confirmConfig = new ConfirmConfig()
                        {
                            Title = "Audio files are missing",
                            Message = $"{Constants.DeviceName} does not have all the audio files necessary for the selected {currentPack.Name} pack. " +
                            $"Would you like to upload these audio files now?",
                            OkText = "Ok",
                            CancelText = "Cancel"
                        };

                        var ok = await _userDialogs.ConfirmAsync(confirmConfig);

                        if (ok)
                        {
                            
                            await SimpleNavigationService.PushModalAsync(new AudioPackListPageViewModel(Navigator,
                                AudioPackNavigationSource.TagItemStartPage, currentPack.Id, null, (pack) =>
                                {
                                    _talkiPlayerManager.Current.Upload(new DataUploadData("GameData", _gameData, TAG,
                                    UploadDataType.GameData));
                                }));
                            return;
                        }
                        else
                        {
                            await SimpleNavigationService.PopModalAsync();
                        }                       
                    }
                }

            }
        }
        
        void SetupRx()
        {
            this.WhenActivated(d =>
            {
               _talkiPlayerManager.Current?.OnDataResult()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(async result =>
                    {
                        if (result.Type == UploadDataType.GameData)
                        {
                            HandleTalkiPlayerGameDataResult(result);                            
                        }
                        else if (result.Type == UploadDataType.AvailableAudioFiles && !_audioFilesResultHandled)
                        {
                            _audioFilesResultHandled = true;
                            await HandleTalkiPlayerAudioFilesResult(result);
                        }                        
                        
                    })
                    .HideLoading()
                    .SubscribeSafe()
                    .DisposeWith(d);
                               
                 _talkiPlayerManager.Current?.IsReady
                     .ObserveOn(RxApp.MainThreadScheduler)
                     .HideLoading()
                     .Where(isReady => isReady && !_isExecuted)
                     .Select(a => Unit.Default)
                     .InvokeCommand(this, v => v.LoadCommand)
                     .DisposeWith(d);

                 //_talkiPlayerManager.Current?.WhenDisconnectedUponInactivity()
                 //    .SubscribeSafe()
                 //    .DisposeWith(d);

                this.WhenAnyValue(m => m._talkiPlayerManager.Current.ConnectionStatus)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(m => _logger.Information($"Status : {m}"))
                .SubscribeSafe()
                .DisposeWith(d);
                

            });
        }

        void SetupCommand()
        {
            TryAgainCommand = ReactiveCommand.CreateFromObservable(() => LoadCommand.Execute());
            TryAgainCommand.ThrownExceptions.SubscribeAndLogException();

            LoadCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _isExecuted = true;
                _userDialogs.ShowLoading("Pairing ...");
                _talkiPlayerManager.Current?.CancelUpload();

                var assets = await _assetRepository.GetAssets(AssetType.Audio);
                assets = assets.Where(a => a.Type == AssetType.Audio).ToList();
                var tags = await _assetRepository.GetTags();

                var pack = _gameMediator.CurrentPack;// await _gameService.GetPack(_gameMediator.CurrentPack.Id);                
                var items = pack.Items.Distinct().ToList();
                
                
                var room = _gameMediator.CurrentRoom;
                
                _gameData = new GameData(_gameMediator.GameSessionId.ToString(),
                    _gameMediator.CurrentGame, tags, assets, items, room.TagItems);

                _userDialogs.HideLoading();

                _talkiPlayerManager.Current.Upload(new DataUploadData("AudioList",
                           DataRequest.GetAudioFileListRequest(), "AudioList",
                           UploadDataType.AvailableAudioFiles));


                //_talkiPlayerManager.Current.Upload(new DataUploadData("AudioData",
                //    DataRequest.GetAudioFileListRequest(), TAG,
                //    UploadDataType.AvailableAudioFiles));
                
            });
                       
            LoadCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .Do(ex =>
                {
                    HasError = true;
                    Message = ex.Message;
                })
                .SubscribeAndLogException();

            BackCommand = ReactiveCommand.CreateFromTask(() => SimpleNavigationService.PopModalAsync());
            
            BackCommand.ThrownExceptions.SubscribeAndLogException();
        }                  
    }
}