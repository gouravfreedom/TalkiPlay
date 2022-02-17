using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using MediaManager;
using ReactiveUI;
using Splat;
using TalkiPlay.Managers;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace TalkiPlay.Shared
{   
    public class QRCodeGameSessionPageViewModel : SimpleBasePageModel
    {
        #region Fields
        
        //const int MaxItemSegments = 4;
        //static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        
        readonly IUserSettings _userSettings;
        readonly IGameMediator _gameMediator;
        readonly IConnectivityNotifier _connectivityNotifier;
        readonly IGameService _gameService;        
        readonly IAssetRepository _assetRepository;
        
        bool _gameEnded;
        private bool _isInitialized;
        private List<AssetDto> _instructionAssets;

        IList<ItemDto> _gameItems;
        IList<IInstruction> _huntGameInstructions;
        QRGameSession _gameSession;
        readonly AudioPlaybackManager _playbackManager;
        TaskCompletionSource<bool> _itemAnimationCompletedSource;
        private readonly List<PlayedItem> _playedItems = new List<PlayedItem>();
        
        // private readonly Dictionary<int, PlayedItem> _playedItems =
        //     new Dictionary<int, PlayedItem>();
        //readonly Dictionary<int, int> _lastPlayedItemAssetIds = new Dictionary<int, int>();
        //readonly Dictionary<int, List<int>> _playedItemAssetIds = new Dictionary<int, List<int>>();
        private bool _windowAnimationIsPlaying;
        private bool _fullScreenAnimationIsPlaying;
        private bool _fullScreenAnimationIsVisible;
        private float _fullScreenAnimationProgress;
        private string _fullScreenAnimationSource;
        private bool _itemAnimationIsPlaying;
        private bool _itemAnimationIsVisible;
        private float _itemAnimationProgress;
        private string _itemAnimationSource;
        private string _itemAnimationJsonSource;
        private bool _showLargeStar = true;
        private bool _showStarList;
        private string _gameTitle;
        private Geometry _currentItemImageClipGeometry;
        private string _currentItemImageSource;
        private ObservableCollection<StarItem> _starItems;
        private bool _currentItemImageIsVisible;

        #endregion
        
        #region Constructor
        
        public QRCodeGameSessionPageViewModel(bool reloadGameSession,
            IGameService gameService = null,
            IConnectivityNotifier connectivityNotifier = null,
            IGameMediator gameMediator = null,            
            IConfig config = null,
            IUserSettings userSettings = null)
        {
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
                   
            _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            _gameService = gameService ?? Locator.Current.GetService<IGameService>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();

            _playbackManager = new AudioPlaybackManager(_gameMediator);
            
            CrossMediaManager.Current.Notification.ShowNavigationControls = false;
            CrossMediaManager.Current.Notification.ShowPlayPauseControls = false;
            CrossMediaManager.Current.Notification.Enabled = false;
            CrossMediaManager.Current.MediaPlayer.ShowPlaybackControls = false;
            CrossMediaManager.Current.ClearQueueOnPlay = true;
            
            SetupCommands();
            
            if (!reloadGameSession)
            {
                LoadNewGame();
            }
            else
            {
                LoadCurrentGame();
            }
            
            _connectivityNotifier.Notifier.RegisterHandler(async context =>
            {
                await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                context.SetOutput(true);
            });
                        
            CrossMediaManager.Current.MediaItemFinished += (sender, args) =>
            {
                _playbackManager.SetAudioPlaybackCompletion();
            };
            
            LoadDataCommand.Execute();
        }
        
        void SetItemAnimationCompletion()
        {
            if (!_itemAnimationCompletedSource?.Task.IsCompleted ?? false)
            {
                _itemAnimationCompletedSource.SetResult(true);
                _itemAnimationCompletedSource = null;
            } 
        }
        
        #endregion
        
        #region Public Methods
        
        public async Task StartAnimations()
        {
            WindowAnimationIsPlaying = true;
            ShowLargeStar = true;

            await Task.Delay(4000);
            
            GameTitle = "What can you see?";
            ShowLargeStar = false;

            CurrentItemImageIsVisible = true;
            ShowStarList = true;
        }
        
        #endregion
        
        #region Properties

        public bool WindowAnimationIsPlaying
        {
            get => _windowAnimationIsPlaying;
            set { _windowAnimationIsPlaying = value; RaisePropertyChanged(); }
        }

        public bool GameEnded
        {
            get => _gameEnded;
            set { _gameEnded = value; RaisePropertyChanged();  }
        }

        public bool FullScreenAnimationIsPlaying
        {
            get => _fullScreenAnimationIsPlaying;
            set { _fullScreenAnimationIsPlaying = value; RaisePropertyChanged();  }
        }

        public bool FullScreenAnimationIsVisible
        {
            get => _fullScreenAnimationIsVisible;
            set { _fullScreenAnimationIsVisible = value; RaisePropertyChanged();  }
        }

        public float FullScreenAnimationProgress
        {
            get => _fullScreenAnimationProgress;
            set { _fullScreenAnimationProgress = value; RaisePropertyChanged();  }
        }

        public string FullScreenAnimationSource
        {
            get => _fullScreenAnimationSource;
            set { _fullScreenAnimationSource = value; RaisePropertyChanged();  }
        }
        
        public bool ItemAnimationIsPlaying
        {
            get => _itemAnimationIsPlaying;
            set { _itemAnimationIsPlaying = value; RaisePropertyChanged();  }
        }

        public bool ItemAnimationIsVisible
        {
            get => _itemAnimationIsVisible;
            set { _itemAnimationIsVisible = value; RaisePropertyChanged();  }
        }

        public float ItemAnimationProgress
        {
            get => _itemAnimationProgress;
            set { _itemAnimationProgress = value; RaisePropertyChanged();  }
        }

        public string ItemAnimationSource
        {
            get => _itemAnimationSource;
            set { _itemAnimationSource = value; RaisePropertyChanged();  }
        }
        
        public string ItemAnimationJsonSource
        {
            get => _itemAnimationJsonSource;
            set { _itemAnimationJsonSource = value; RaisePropertyChanged();  }
        }
        
        public bool ShowLargeStar
        {
            get => _showLargeStar;
            set { _showLargeStar = value; RaisePropertyChanged();  }
        }

        public bool ShowStarList
        {
            get => _showStarList;
            set { _showStarList = value; RaisePropertyChanged();  }
        }

        public ObservableCollection<StarItem> StarItems
        {
            get => _starItems;
            set { _starItems = value; RaisePropertyChanged();  }
        }

        public string CurrentItemImageSource
        {
            get => _currentItemImageSource;
            set { _currentItemImageSource = value; RaisePropertyChanged();  }
        }

        public bool CurrentItemImageIsVisible
        {
            get => _currentItemImageIsVisible;
            set { _currentItemImageIsVisible = value; RaisePropertyChanged();  }
        }
        
        //TODO: add converter
        public Geometry CurrentItemImageClipGeometry
        {
            get => _currentItemImageClipGeometry;
            set { _currentItemImageClipGeometry = value; RaisePropertyChanged();  }
        }

        public string GameTitle
        {
            get => _gameTitle;
            set { _gameTitle = value; RaisePropertyChanged(); }
        }
        
        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; set; }
        
        public ICommand NextItemDebugCommand { get; private set; }
        
        public ICommand EndGameDebugCommand { get; private set; }
        
        public ICommand BackCommand { get; private set;}
        
        public ICommand CollectRewardCommand { get;private set; }
        public ICommand FullScreenAnimationCompletedCommand { get;private set; }
        
        public ICommand ItemAnimationCompletedCommand { get;private set; }
        
        #endregion

        #region Game Management

        void HandleEndGameDebug()
        {
#if DEV
            EndGame().Forget();
#endif
        }
        
        void HandleNextItemDebug()
        {
            #if DEV
            
            if (_gameMediator.CurrentGame.Type == GameType.Explore)
            {               
                var collectedItems = _gameSession.Results.Where(i => i.IsCollected).ToList();
                var item = _gameItems.Where(i => collectedItems.All(r => r.ItemId != i.Id)).Shuffle().First();
                             
                ProcessQRCode(item.QRCode).Forget();
            }
            else
            {
                if (_gameSession.HuntInstructionIndex < _huntGameInstructions.Count)
                {
                    var item = _huntGameInstructions[_gameSession.HuntInstructionIndex];
                    ProcessQRCode(item.Item.QRCode).Forget();
                }
            }
            #endif
        }

        private bool _isQRCodeProcessingLocked;

        public async Task ProcessQRCode(string qrCodeText)
        {
            if (_gameMediator.CurrentGame == null || _gameEnded || _isQRCodeProcessingLocked)
            {
                return;
            }
            
            Debug.WriteLine($"QRCode: {qrCodeText} - Waiting to start");
            _isQRCodeProcessingLocked = true;
            
            //await _semaphoreSlim.WaitAsync(2000);
            //_semaphoreSlim.Release();
            
            try
            {
                Debug.WriteLine($"QRCode: {qrCodeText} - Processing");
                if (_gameMediator.CurrentGame.Type == GameType.Hunt)
                {
                    Debug.WriteLine($"QRCode: {qrCodeText} - ProcessHuntInstruction");
                    await ProcessHuntInstruction(qrCodeText);

                    await Task.Delay(100);
                }
                else if (_gameMediator.CurrentGame.Type == GameType.Explore)
                {
                    await ProcessExploreItem(qrCodeText);
                }
            
            }
            catch (Exception ex)
            {
                Debug.Write("ProcessQRCode: " + ex.Message);
            }
            finally
            {
                _isQRCodeProcessingLocked = false;
                Debug.WriteLine($"QRCode: {qrCodeText} - Completed");
            }
        }

        async Task ProcessExploreItem(string qrCodeText)
        {
            var item = _gameItems.FirstOrDefault(i =>
                        i.QRCode?.Equals(qrCodeText,
                            StringComparison.InvariantCultureIgnoreCase) ?? false);
            
            Debug.WriteLine($"QRCode: {qrCodeText} - ProcessItem");


            if (item != null)
            {
                Debug.WriteLine($"QRCode: ProcessItem - {item.Name}");
                CurrentItemImageSource = null;
                var assetId = await ProcessItem(item);
                if (assetId.HasValue)
                {
                    ShowItemImage(item);
                    RefreshStarItemsDisplay(false);
                    
                    await _playbackManager.PlayAudio(GameEnded, assetId.Value);
                    
                    var playedItem = _playedItems.FirstOrDefault(i => i.ItemId == item.Id);
                    
                    await ShowItemCompletion(item, playedItem, false);
                    await WaitForItemAnimationToFinish();

                    var result =
                        _gameSession.Results.FirstOrDefault(r => r.ItemId == item.Id);
                    if (result == null)
                    {
                        result = new GameSessionItemResult()
                        {
                            Status = ItemResultStatus.Success,
                            StartTime = DateTime.Now,
                            EndTime = DateTime.Now,
                            FailureCount = 0,
                            ItemId = item.Id
                        };
                        _gameSession.Results.Add(result);
                    }
                    else
                    {
                        result.EndTime = DateTime.Now;

                        
                        if (playedItem != null)
                        {
                            result.IsCollected = playedItem.IsCompleted;
                        }
                        
                        // if (_playedItemAssetIds.ContainsKey(item.Id))
                        // {
                        //     result.IsCollected = _playedItemAssetIds[item.Id].Count >=
                        //                          MaxItemSegments;
                        // }
                    }
                    SaveSession();
                }
            }

            await Task.Delay(100);
            if (_gameSession.Results.Count(r => r.IsCollected) == _gameItems.Count)
            {
                await EndGame();
            }
        }
        
        void ShowItemImage(IItem item)
        {
            var assetCount = 0;
            var playedItem = _playedItems.FirstOrDefault(i => i.ItemId == item.Id);
            if (playedItem != null)
            {
                assetCount = playedItem.AssetCounts.Keys.Count;
            }
            
            // List<int> playedAssetIds = null;
            // if (_playedItemAssetIds.ContainsKey(item.Id))
            // {
            //     playedAssetIds = _playedItemAssetIds[item.Id];
            // }
            
            CurrentItemImageSource = item.ImagePath;

            //switch (playedAssetIds?.Count)
            switch (assetCount)
            {
                case 1:
                {
                    CurrentItemImageClipGeometry = Geometries.OneQuarterClipGeometry;
                    break;
                }
                case 2:
                {
                    CurrentItemImageClipGeometry = Geometries.HalfClipGeometry;
                    break;
                }
                case 3:
                {
                    CurrentItemImageClipGeometry = Geometries.ThreeQuartersClipGeometry;
                    break;
                }
                default:
                {
                    CurrentItemImageClipGeometry = null;
                    
                    var completionAsset =
                        item.Assets.FirstOrDefault(a => a.Purpose == AssetPurpose.PuzzleCompletion);
                    if (completionAsset != null)
                    {
                        CurrentItemImageSource = completionAsset.ImageContentPath;
                    }

                    ShowItemAnimation(item);
                    break;
                }
            }
            // #if DEV
            // ShowItemAnimation(item);
            // #endif
        }

        void ShowItemAnimation(IItem item)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var animationAsset =
                    item.Assets.FirstOrDefault(a => a.Type == AssetType.Animation);

                if (animationAsset == null)
                {
                    return;
                }

                await ShowAnimation(animationAsset.FilePath);
            });
        }

        async Task ShowAnimation(string animationFilePath)
        {
            if (string.IsNullOrEmpty(animationFilePath) || !File.Exists(animationFilePath))
            {
                return;
            }

            SetItemAnimationCompletion();

            InitAnimationCompletionSource();
            
            CurrentItemImageIsVisible = false;

            if (Device.RuntimePlatform == Device.Android)
            {
                var json = await File.ReadAllTextAsync(animationFilePath);

                ItemAnimationJsonSource = json;
                //ItemAnimationProgress = 0; 
                ItemAnimationIsVisible = true;
                //ItemAnimationIsPlaying = true;
            }
            else
            {
                ItemAnimationSource = animationFilePath;
                ItemAnimationProgress = 0;
                ItemAnimationIsVisible = true;
                ItemAnimationIsPlaying = true;
            }
        }

        async Task WaitForItemAnimationToFinish()
        {
            if (_itemAnimationCompletedSource != null &&
                !_itemAnimationCompletedSource.Task.IsCompleted)
            {
                await _itemAnimationCompletedSource.Task;
            }
        }

        void InitAnimationCompletionSource()
        {
            if (_itemAnimationCompletedSource != null &&
                !_itemAnimationCompletedSource.Task.IsCompleted)
            {
                _itemAnimationCompletedSource.TrySetCanceled();
            }

            _itemAnimationCompletedSource = new TaskCompletionSource<bool>();
        }
        
        async Task ShowItemCompletion(IItem item, PlayedItem playedItem, bool isHunt)
        {
            bool showStarAnimation = false;

            if (isHunt)
            {
                showStarAnimation = _gameSession.Results.Any(r => r.ItemId == item.Id);
            }
            else if (playedItem != null)
            {
                showStarAnimation = playedItem.IsCompleted;
            }
            // else if (_playedItemAssetIds.ContainsKey(item.Id))
            // {
            //     var playedAssetIds = _playedItemAssetIds[item.Id];
            //     showStarAnimation = playedAssetIds.Count >= MaxItemSegments;
            // }
            
            FullScreenAnimationSource = showStarAnimation ?  Images.ConfettiWithStarAnimation : Images.ConfettiAnimation;
            FullScreenAnimationProgress = 0;
            FullScreenAnimationIsVisible = true;
            FullScreenAnimationIsPlaying = true;

            #if !DEV
            await _playbackManager.PlayAudio(showStarAnimation ?  Audio.ConfettiSound : Audio.PuzzlePieceSound);
            #endif
            
        }
        
        void RefreshStarItemsDisplay(bool isHunt)
        {
            var starItems = new List<StarItem>();
            
            foreach (var item in _gameItems)
            {
                bool isCollected = false;
                if (isHunt)
                {
                    isCollected = _gameSession.Results.Any(r => r.ItemId == item.Id);
                }
                else
                {
                    var playedItem = _playedItems.FirstOrDefault(i => i.ItemId == item.Id);
                    if (playedItem != null)
                    {
                        isCollected = playedItem.IsCompleted;
                    }
                    
                    // if (_playedItemAssetIds.ContainsKey(item.Id))
                    // {
                    //     var playedAssetIds = _playedItemAssetIds[item.Id];
                    //     isCollected = playedAssetIds.Count >= MaxItemSegments;
                    // }
                }
                starItems.Add(new StarItem(item.Id, isCollected));
            }
            
            StarItems = new ObservableCollection<StarItem>(starItems.OrderByDescending(i => i.IsCollected));
        }
        
        void SaveSession()
        {
            _userSettings.CurrentQRGameSession = _gameSession;
        }

        async Task PlayNextHuntInstruction()
        {
            if (_gameSession.HuntInstructionIndex >= _huntGameInstructions.Count)
            {
                return;
            }
            
            var instruction = _huntGameInstructions[_gameSession.HuntInstructionIndex];
            int assetId;

            var mode = instruction.Modes.FirstOrDefault(a => a.Type == InstructionModeType.Receptive);
            if (mode != null && mode.AudioAssetId != null)
            {
                assetId = mode.AudioAssetId.Value;                
            }
            else
            {
                mode = instruction.Modes.FirstOrDefault(a => a.Type == InstructionModeType.Expressive);
                if (mode != null && mode.AudioAssetId != null)
                {
                    assetId = mode.AudioAssetId.Value;                    
                }
                else
                {
                    return;
                }
            }
            
            await _playbackManager.PlayAudio(GameEnded, assetId);
            
            _gameSession.HuntInstructionStartTime = DateTime.Now;
            _gameSession.HuntInstructionFailureCount = 0;            

        }

        async Task ProcessHuntInstruction(string qrCodeText)
        {            
            var instruction = _huntGameInstructions.FirstOrDefault(i =>
                   i.Item.QRCode?.Equals(qrCodeText, StringComparison.InvariantCultureIgnoreCase) ?? false);

            if ((instruction?.Item?.AudioAssetIds?.Count ?? 0) == 0)
            {
                await _playbackManager.PlaySystemSound(GameEnded,SoundType.WrongScan);
                return;
            }
            
            var index = _huntGameInstructions.IndexOf(instruction);
            if (index != _gameSession.HuntInstructionIndex)
            {
                await _playbackManager.PlaySystemSound(GameEnded,SoundType.WrongScan);
                _gameSession.HuntInstructionFailureCount++;
                SaveSession();
                return;
            }
           
            var item = instruction.Item;

            if (!_gameSession.IsHuntGameCompleted)
            {
                _gameSession.Results.Add(new GameSessionItemResult()
                {
                    Status = ItemResultStatus.Success,
                    StartTime = _gameSession.HuntInstructionStartTime,
                    EndTime = DateTime.Now,
                    FailureCount = _gameSession.HuntInstructionFailureCount,
                    ItemId = item.Id,
                    IsCollected = true
                });
            }
            
            
            CurrentItemImageSource = null;
            int? assetId;
            var mode = instruction.Modes.FirstOrDefault(a => a.Type == InstructionModeType.Reward);
            if (mode?.AudioAssetId != null)
            {
                await _playbackManager.PlaySystemSound(GameEnded,SoundType.CorrectScan);
                assetId = mode.AudioAssetId;
            }
            else
            {
                assetId = await ProcessItem(item);
            }


            if (mode?.AnimationAssetId != null)
            {
                var asset =
                    _instructionAssets.FirstOrDefault(i => i.Id == mode.AnimationAssetId.Value);
                
                if (asset != null)
                {
                    await ShowAnimation(asset.FilePath);    
                }
            }
            else
            {
                CurrentItemImageSource = item.ImagePath;
                ShowItemAnimation(item);
            }
            
            RefreshStarItemsDisplay(true);
            if (assetId.HasValue)
            {
                await _playbackManager.PlayAudio(GameEnded, assetId.Value);
            }
            
            var playedItem = _playedItems.FirstOrDefault(i => i.ItemId == item.Id);
            
            await ShowItemCompletion(item, playedItem,true);
        
            await WaitForItemAnimationToFinish();
            
            if (!_gameSession.IsHuntGameCompleted)
            {
                if (_gameSession.HuntInstructionIndex == _huntGameInstructions.Count - 1)
                {
                    await _playbackManager.PlaySystemSound(GameEnded,SoundType.HuntFinish);
                    _gameSession.IsHuntGameCompleted = true;
                    EndGame().Forget();
                }
                else if (!GameEnded)
                {
                    _gameSession.HuntInstructionIndex++;
                    CurrentItemImageSource = null;
                    await Task.Delay(500);
                    await PlayNextHuntInstruction();
                }

                SaveSession();
            }
        }

        async Task<int?> ProcessItem(IItem item)
        {
            if (item?.AudioAssetIds == null)
            {
                Debug.WriteLine("ProcessItem: WrongScan");
                await _playbackManager.PlaySystemSound(GameEnded,SoundType.WrongScan);
                return null;
            }
            
            int assetId = item.AudioAssetIds.FirstOrDefault();

            if (assetId == 0)
            {
                Debug.WriteLine("ProcessItem: Error");
                await _playbackManager.PlaySystemSound(GameEnded,SoundType.Error);
                return null;
            }

            var playedItem = _playedItems.FirstOrDefault(i => i.ItemId == item.Id);
            
            if (item.AudioAssetIds.Count > 1)
            {
                if (playedItem != null)
                {
                    var playedItemCopy = playedItem;
                    assetId = item.AudioAssetIds
                        .Where(id => id != playedItemCopy.LastAssetId && !playedItemCopy.IsMaxAssetPlaybackCountExceeded(id))
                        .Shuffle()
                        .FirstOrDefault();

                    if (assetId == 0)
                    {
                        Dialogs.Toast(Dialogs.BuildErrorToast("No audio asset found"));
                        return null;
                    }
                }
            }

            Debug.WriteLine("ProcessItem: CorrectScan");
            await _playbackManager.PlaySystemSound(GameEnded,SoundType.CorrectScan);

            if (playedItem == null)
            {
                playedItem = new PlayedItem(item.Id);
                _playedItems.Add(playedItem);
            }
            else
            {
                playedItem.SetLastAssetId(assetId);
            }

            if (!playedItem.AssetCounts.ContainsKey(assetId))
            {
                playedItem.AssetCounts[assetId] = 1;
            }
            else
            {
                playedItem.AssetCounts[assetId]++;
            }
            
            Debug.WriteLine("ProcessItem: AudioAsset " + assetId);
            return assetId;
        }
        
        async Task StartGame()
        {
            _isQRCodeProcessingLocked = true;
            
            _gameEnded = false;

            await _playbackManager.PlaySystemSound(GameEnded,SoundType.StartGame);
            
            if (_gameMediator.CurrentGame?.Type == GameType.Hunt)
            {
                await Task.Delay(1000);
                
                await PlayNextHuntInstruction();
            }
            
            _isQRCodeProcessingLocked = false;
        }

        async Task CancelGame()
        {
            var confirmConfig = new ConfirmConfig()
            {
                Title = _gameMediator?.CurrentGame?.Name ?? "",
                Message = $"Are you sure you wish to cancel this game?",
                OkText = "Yes",
                CancelText = "No"
            };
            var ok = await Dialogs.ConfirmAsync(confirmConfig);

            if (ok)
            {
                EndGameSession();
            }
        }

        async Task EndGame()
        {
            if (GameEnded)
            {
                return;
            }
            GameEnded = true;
            
            CurrentItemImageIsVisible = false;
            ItemAnimationIsVisible = false;
            
            FullScreenAnimationSource = Images.TrophyAnimation;
            FullScreenAnimationProgress = 0;
            FullScreenAnimationIsVisible = true;
            FullScreenAnimationIsPlaying = true;
                
            await _playbackManager.PlayAudio(Audio.CupSound);
        }
        
        async Task CollectReward()
        {
            if (!GameEnded)
            {
                return;
            }
            
            var gameRecord = new RecordGameSessionResultRequest()
            {
                GameId = _gameSession.GameId,                
                StartTime = _gameSession.GameStartTime,
                EndTime = DateTime.Now,
                Children = _gameSession.ChildrenIds,
                DeviceName = "QRCode Scanner",
                Results = _gameSession.Results
            };            

            Dialogs.ShowLoading("Saving ...");

            try
            {
                var reward = await _gameService.RecordGameResult(gameRecord, _gameSession.PackId);
                Dialogs.HideLoading();
                
                if (_gameMediator.CurrentGame.Chest != null && reward != null)
                {
                    var model = new QRCodeGameRewardPageViewModel(_gameMediator.CurrentGame.Chest, reward,EndGameSession);

                    await SimpleNavigationService.PushPopupAsync(model);
                }
                else
                {
                    EndGameSession();
                }
            }
            catch (Exception e)
            {
                Dialogs.HideLoading();
                e.ShowExceptionDialog();
                EndGameSession();
            }
        }
        
        void EndGameSession()
        {
            try
            {
                _userSettings.ClearGameSession();
                _gameMediator.Reset();
            }
            catch(Exception ex)
            {
                ex.LogException();
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                SimpleNavigationService.PopModalAsync().Forget();
            });
        }
        
        void LoadGameItems(IPack pack, IGame game, bool isExistingGame)
        {
            var itemsSettings = _userSettings.ItemsSettings;

            if (game.Type == GameType.Explore)
            {

                _gameItems = pack.Items
                    .Where(i => i.Id > 0 && !string.IsNullOrEmpty(i.Name))
                    .Where(i => i.Type == ItemType.Default)
                    .Where(i =>
                        !itemsSettings.Any(setting => setting.ItemId == i.Id && !setting.IsActive))
                    .Distinct()
                    .ToList();
            }
            else
            {
                var instructions = game.Instructions
                    .Where(i => !itemsSettings.Any(setting => setting.ItemId == i.Item.Id && !setting.IsActive))
                    .ToList();

                _gameItems = instructions.Select(i => (ItemDto) i.Item).ToList();
                
                if (isExistingGame)
                {
                    _huntGameInstructions = new List<IInstruction>();
                    foreach (var itemId in _gameSession.HuntInstructionsOrder)
                    {
                        var instruction = instructions.FirstOrDefault(i => i.Item.Id == itemId);
                        if (instruction != null)
                        {
                            _huntGameInstructions.Add(instruction);
                        }
                    }
                }
                else
                {
                    if (game.InstructionsAreRandom)
                    {
                        _huntGameInstructions = instructions.Shuffle().ToList();
                    }
                    else
                    {
                        _huntGameInstructions = instructions;
                    }

                    _gameSession.HuntInstructionsOrder = _huntGameInstructions.Select(i => i.Item.Id).ToList();
                }
            }
            RefreshStarItemsDisplay(game.Type == GameType.Hunt);

            var animationAssets =
                _gameItems.SelectMany(i => i.Assets.Where(a => a.Type == AssetType.Animation)).ToList();
            if (animationAssets.Count > 0)
            {
                Task.Run(() =>
                {
                    AssetDownloadManager.DownloadAnimations(animationAssets).Forget();
                });
            }

            if (game.Type == GameType.Hunt)
            {
                _instructionAssets = new List<AssetDto>();
                var assetIds =
                    _huntGameInstructions.SelectMany(i => i.Modes.Select(m => m.AnimationAssetId));
                foreach (var assetId in assetIds)
                {
                    if (assetId != null)
                    {
                        _instructionAssets.Add(new AssetDto()
                        {
                            Id = assetId.Value
                        });
                    }
                }

                Task.Run(() =>
                {
                    AssetDownloadManager.DownloadAnimations(_instructionAssets).Forget();
                });
            }
        }

        void SetTitleText()
        {
            GameTitle = $"It's time to {_gameMediator.CurrentPack.Name.ToLower()} & collect stars";
        }
        
        #endregion
        
        #region New Game

        void LoadNewGame()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            var children = _gameMediator.Children.Items.Where(i => i.Id > 0);
            
            _gameSession = new QRGameSession
            {
                TimeStamp = DateTime.Now,
                GameId = _gameMediator.CurrentGame.Id,                
                PackId = _gameMediator.CurrentPack.Id,
                ChildrenIds = children.Select(c => c.Id).ToList(),
                GameStartTime = DateTime.Now,
                Results = new List<GameSessionItemResult>(),
            };            

            var game = _gameMediator.CurrentGame;
            SetTitleText();

            //SetChildrenInfo(children.ToList());

            LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                Dialogs.ShowLoading();
                
                LoadGameItems(_gameMediator.CurrentPack, game, false);

                //_assetsSettings = _userSettings.AssetsSettings;
               
                SaveSession();

                Dialogs.HideLoading();

                await StartGame();

            });

            LoadDataCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)                
                .ShowExceptionDialog()
                .SubscribeAndLogException();

            //SetupCommands();
            //SetupRx();
        }
        
        #endregion

        #region Existing Game

        void LoadCurrentGame()
        {
            LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (_isInitialized)
                {
                    return;
                }
                
                _isInitialized = true;                    

                Dialogs.ShowLoading();

                _gameSession = _userSettings.CurrentQRGameSession;
                
                var game = await _gameService.GetGame(_gameSession.GameId);
                
                _gameMediator.CurrentGame = game;

                var pack = await _assetRepository.GetPack(_gameSession.PackId);

                LoadGameItems(pack, game, true);

                SetTitleText();

                // var childIds = _gameSession.ChildrenIds;
                // var children = await _childrenService.GetChildren();
                // children = children.Where(a => childIds.Contains(a.Id)).ToList();
                //SetChildrenInfo(children);

                //_assetsSettings = _userSettings.AssetsSettings;
                Dialogs.HideLoading();
                await StartGame();
            });

            LoadDataCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)                
                .ShowExceptionDialog()
                .SubscribeAndLogException();
        }
        
        #endregion
        
        #region Helpers
        
        
        void SetupCommands()
        {           
            NextItemDebugCommand = new Command(HandleNextItemDebug);

            EndGameDebugCommand = new Command(HandleEndGameDebug);
            
            BackCommand = new Command(() => CancelGame().Forget());
            
            CollectRewardCommand = new Command(() => CollectReward().Forget());
            
            FullScreenAnimationCompletedCommand =
                new Command(() =>
                {
                    FullScreenAnimationIsPlaying = false;
                    FullScreenAnimationProgress = 0;
                    if (!GameEnded)
                    {
                        FullScreenAnimationIsVisible = false;
                    }
                });
            
            ItemAnimationCompletedCommand = new Command(() =>
            {
                ItemAnimationIsPlaying = false;
                ItemAnimationProgress = 0;
                ItemAnimationIsVisible = false;
                
                if (!GameEnded)
                {
                    CurrentItemImageIsVisible = true;
                }

                SetItemAnimationCompletion();
            });      
        }

        #endregion
    }
}
