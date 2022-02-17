using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using Plugin.DownloadManager.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public enum AudioPackNavigationSource
    {
        DevicePage,
        TagItemStartPage,
        DeviceSetupPage
    }

    public class AudioPackListPageViewModel : BasePageViewModel, IActivatableViewModel, IModalViewModel
    {
        //private readonly IGameService _gameService;

        private readonly ILogger _logger;
        //private readonly IAssetService _assetService;
        private readonly IAssetRepository _assetRepository;
        private readonly ITalkiPlayerManager _talkiPlayerManager;
        private readonly IConfig _config;
        private readonly IUserDialogs _userDialogs;
        private readonly SourceList<AudioPackItemViewModel> _audioItemsList = new SourceList<AudioPackItemViewModel>();
        private readonly Queue<AssetDto> _queue = new Queue<AssetDto>();
        readonly AudioPackNavigationSource _navigationSource;
        private int? _defaultPackId;
        private string _defaultPackName;
        readonly Action<IPack> _packDownloadCompletionCallback;

        public AudioPackListPageViewModel(INavigationService navigator,
            AudioPackNavigationSource navigationSource = AudioPackNavigationSource.DevicePage,
            int? defaultPackId = null,
            string defaulltPackName = null,
            Action<IPack> packDownloadCompletionCallback = null,
            IUserDialogs userDialogs = null,
            IConfig config = null,
            ITalkiPlayerManager talkiPlayerManager = null,
            //IGameService gameService = null,
            //IAssetService assetService = null,
            ILogger logger = null
        )
        {
            _navigationSource = navigationSource;
            _defaultPackId = defaultPackId;
            _defaultPackName = defaulltPackName;
            _packDownloadCompletionCallback = packDownloadCompletionCallback;
            //_assetService = assetService ?? Locator.Current.GetService<IAssetService>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            //_gameService = gameService ?? Locator.Current.GetService<IGameService>();
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _config = config ?? Locator.Current.GetService<IConfig>();
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            Activator = new ViewModelActivator();
            Navigator = navigator;
            SetupCommands();
            SetupRx();
        }

        public override string Title => "Audio pack update";

        public ReadOnlyCollection<AudioPackItemViewModel> AudioPackItems { get; private set; }

        public ViewModelActivator Activator { get; }

        public ReactiveCommand<Unit, Unit> LoadCommand { get; private set; }
        public ReactiveCommand<AudioPackItemViewModel, Unit> SelectCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; set; }

        [Reactive] public bool IsRefreshing { get; set; }

        public extern bool IsReady { [ObservableAsProperty] get; }

        [Reactive] public AssetDto CurrentItem { get; set; }

        [Reactive] public AudioPackItemViewModel Selected { get; set; }

        [Reactive] public bool WaitingForItemResult { get; set; }

        [Reactive] public int NumberOfItemsInQueue { get; set; }

        [Reactive] public int NumberOfItemsProcessed { get; set; }

        [Reactive] public int TotalNumberOfItems { get; set; }

        public IList<AssetDto> SystemAudioAssets { get; set; } = new List<AssetDto>();

        public ReactiveCommand<AssetDto, Unit> DownloadCommand { get; private set; }

        public ReactiveCommand<AssetDto, Unit> UploadCommand { get; private set; }

        [Reactive]
        public IPack CurrentPack { get; set; }

        public IProgressDialog ProgressDialog { get; set; }

        [Reactive]
        public IDownloadFileResult AudioDownloadResult { get; set; }

        public IList<string> AudioAssetsInTalkPlayer { get; set; } = new List<string>();

        void SetupRx()
        {
            this.WhenActivated(d => BindRx(d));
        }

        public void BindRx(CompositeDisposable d, bool forceUpdate = false)
        {
            var comparer = OrderedComparer<AudioPackItemViewModel>
                .OrderBy(m => m.Name);

            _audioItemsList.Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Sort(comparer)
                    .Bind(out var audioItems)
                    .DisposeMany()
                    .Subscribe()
                    .DisposeWith(d);

            AudioPackItems = audioItems;

            _talkiPlayerManager.Current.OnDataResult()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(data =>
                {

                    switch (data.Type)
                    {
                        case UploadDataType.AvailableAudioFiles:
                            if (data.IsSuccess && data.Data is AvailableAudioFiles files)
                            {
                                Dialogs.HideLoading();
                                AudioAssetsInTalkPlayer = files.AudioFiles?.Select(a => a.ToLower()).ToList() ?? new List<string>();
                                RefreshPacksStatus();

                                if (_defaultPackId == null && !string.IsNullOrEmpty(_defaultPackName))
                                {
                                    var defaultPackVM = _audioItemsList.Items.FirstOrDefault(i => i.Pack.Name?.ToLower() == _defaultPackName.ToLower());
                                    _defaultPackId = defaultPackVM?.Pack.Id;
                                    _defaultPackName = null;
                                }

                                if (_defaultPackId != null)
                                {                                    
                                    var defaultPackVM = _audioItemsList.Items.FirstOrDefault(i => i.Pack.Id == _defaultPackId.Value);
                                    _defaultPackId = null;
                                    if (defaultPackVM != null)
                                    {
                                        SelectCommand.Execute(defaultPackVM);
                                    }
                                }
                            }

                            break;
                        case UploadDataType.Audio:
                            if (int.TryParse(data.Tag, out var id))
                            {
                                if (CurrentItem.Id == id)
                                {
                                    WaitingForItemResult = false;
                                    CurrentItem = null;

                                    NumberOfItemsProcessed++;
                                    var percent = (double)NumberOfItemsProcessed / (double)TotalNumberOfItems;
                                    ProgressDialog.PercentComplete = (int)(percent * 100);
                                }
                            }
                            break;
                    }

                    if (!data.IsSuccess && data.Data is ErrorDataResult errorDataResult)
                    {
                        var errorMessage = $"Failed to upload audio file to {Constants.DeviceName}";

                        _userDialogs.Toast(Dialogs.BuildErrorToast(errorMessage));
                    }

                }, ex => { })
                .DisposeWith(d);



            _talkiPlayerManager.Current?.WhenAnyValue(m => m.IsConnected)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToPropertyEx(this, v => v.IsReady)
                .DisposeWith(d);


            this.WhenAnyValue(m => m.WaitingForItemResult, m => m.CurrentItem, m => m.NumberOfItemsInQueue,
                    ((b, model, count) => !b && model == null && count > 0))
                .Where(a => a)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(m =>
                {

                    var next = _queue.Dequeue();
                    if (next != null)
                    {
                        CurrentItem = next;
                        WaitingForItemResult = true;
                        NumberOfItemsInQueue = _queue.Count;
                        ProgressDialog.Title =
                            $"{TotalNumberOfItems - NumberOfItemsInQueue} / {TotalNumberOfItems} assets uploading ...";

                        return next;
                    }

                    return null;
                })
                .Where(m => m != null)
                .InvokeCommand(this, v => v.DownloadCommand)
                .DisposeWith(d);

            this.WhenAnyValue(m => m.NumberOfItemsProcessed)
                .Where(a => a >= TotalNumberOfItems && a > 0)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeSafe(_ =>
                {
                    ProgressDialog?.Hide();
                    ProgressDialog?.Dispose();
                    ProgressDialog = null;
                    CurrentItem = null;
                    if (Selected != null)
                    {
                        Selected.Icon = Images.SelectedIcon;
                    }

                    NumberOfItemsProcessed = 0;
                    NumberOfItemsInQueue = 0;

                    if (_navigationSource == AudioPackNavigationSource.TagItemStartPage || _navigationSource == AudioPackNavigationSource.DeviceSetupPage)
                    {
                        BackCommand.Execute();
                        _packDownloadCompletionCallback?.Invoke(Selected.ModifiedPack);
                    }
                })
                .DisposeWith(d);
        }

        public void RefreshPacks()
        {
            RefreshCommand?.Execute().Subscribe();
        }

        void RefreshPacksStatus()
        {
            _audioItemsList.Edit(async items =>
            {
                foreach (var item in items)
                {
                    var pack = await _assetRepository.GetPack(item.Pack.Id);
                    item.ModifiedPack = pack;
                    bool hasAll = PackHasAllAudioFiles(pack);
                    item.Icon = hasAll ? Images.SelectedIcon : Images.DownloadIcon;
                }
            });
        }

        void SetupCommands()
        {
            LoadCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                return ObservableOperatorExtensions.StartShowLoading("Loading ...")
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .SelectMany(_ => _assetRepository.GetAssets(AssetType.Audio, Category.System).Select(a => a.Where(b => b.Type == AssetType.Audio)))
                    .SelectMany(assets => _assetRepository.GetPacks().Select(packs => (Packs: packs, Assets: assets)))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(m =>
                    {
                        var packs = m.Packs;
                        var assets = m.Assets;

                        SystemAudioAssets = assets.ToList();
                        
                        IsRefreshing = false;
                        _audioItemsList.Edit(items =>
                        {
                            items.Clear();
                            items.AddRange(packs.Select(a => new AudioPackItemViewModel(a)
                            {
                                Name = a.Name,

                            }).ToList());
                        });

                        _talkiPlayerManager.Current.Upload(new DataUploadData("AudioList",
                            DataRequest.GetAudioFileListRequest(), "AudioList", UploadDataType.AvailableAudioFiles));
                    })
                    .Select(_ => Unit.Default);
            });

            LoadCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .SubscribeAndLogException();

            var canExecute = this.WhenAnyValue(m => m.IsRefreshing).Select(m => !m);
            RefreshCommand = ReactiveCommand.Create(() =>
            {
                IsRefreshing = true;
                _audioItemsList.Clear();
                LoadCommand.Execute().Subscribe();
            }, canExecute);

            RefreshCommand.ThrownExceptions.SubscribeAndLogException();


            BackCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SimpleNavigationService.PopModalAsync();
            });

            BackCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .SubscribeAndLogException();

            SelectCommand = ReactiveCommand.CreateFromTask<AudioPackItemViewModel, Unit>(PerformSelect);
            
            SelectCommand.ThrownExceptions.SubscribeAndLogException();

        
            DownloadCommand = ReactiveCommand.CreateFromTask<AssetDto, Unit>(async asset =>
            {
                var assetToDownload = await _assetRepository.GetAssetById(asset.Id);
                AudioDownloadResult = AssetDownloadManager.BuildAssetDownloadResult(assetToDownload);
                 //AudioDownloadResult = await _assetService.GetAndDownloadAsset(asset);
                SetupDownloadMonitor();
                return Unit.Default;
            });
            
            DownloadCommand.ThrownExceptions.SubscribeAndLogException();

            UploadCommand = ReactiveCommand.CreateFromTask<AssetDto, Unit>(async asset =>
            {
                 await _assetRepository.SaveAsset(asset);
                if (!String.IsNullOrWhiteSpace(asset.FilePath))
                {
                    var size = FileHelper.GetFileSize(asset.FilePath);
                    var checksum = FileHelper.GenerateCheckSum(asset.FilePath);
                    var data = new FileUploadData(asset.Filename, size, checksum, asset.FilePath, $"{asset.Id}", UploadDataType.Audio);
                    _talkiPlayerManager.Current.Upload(data);
                }
                
                return Unit.Default;
            });

            UploadCommand.ThrownExceptions.SubscribeAndLogException();
        }

       

        bool PackHasAllAudioFiles(IPack pack)
        {                        
            var audioAssetsToBeUploaded = new List<string>();
            audioAssetsToBeUploaded.AddRange(pack.AudioAssets.Select(a => a.Filename.ToLower()));
            audioAssetsToBeUploaded.AddRange(SystemAudioAssets.Select(a => a.Filename.ToLower()));
            var hasAll = audioAssetsToBeUploaded.All(a => AudioAssetsInTalkPlayer.Contains(a));

            return hasAll;
        }

        async Task<Unit> PerformSelect(AudioPackItemViewModel vm)
        {
            Selected = vm;

            var pack = await _assetRepository.GetPack(vm.Pack.Id);
            vm.ModifiedPack = pack;

            var systemAudioThatAreNotInTalkiPlayer = SystemAudioAssets
                .Where(a => !AudioAssetsInTalkPlayer.Contains(a.Filename.ToLower()))
                .ToList();

            TotalNumberOfItems = pack.AudioAssets.Count + systemAudioThatAreNotInTalkiPlayer.Count;

            var hasAll = PackHasAllAudioFiles(pack);                       

            vm.Icon = hasAll ? Images.SelectedIcon : Images.DownloadIcon;

            if (_navigationSource == AudioPackNavigationSource.DeviceSetupPage && hasAll)
            {
                _packDownloadCompletionCallback?.Invoke(pack);
                return Unit.Default;
            }

            bool ok = _navigationSource == AudioPackNavigationSource.DeviceSetupPage ? true : false;

            if (_navigationSource != AudioPackNavigationSource.DeviceSetupPage)
            {
                var confirmConfig = new ConfirmConfig()
                {
                    Title = "Audio pack update",
                    Message =
                        $"There are {TotalNumberOfItems} audio assets in this pack to be uploaded to {Constants.DeviceName}.\n" +
                        $"This will take around 5 - 30 mins (approx) to transfer to {Constants.DeviceName}.\nPlease do not close app while uploading.",
                    OkText = "Ok",
                    CancelText = "Cancel"
                };

                if (hasAll)
                {
                    confirmConfig = new ConfirmConfig()
                    {
                        Title = "Audio pack update",
                        Message =
                            $"{Constants.DeviceName} has all audio assets in this pack.\n Would you like to re-upload these assets?\n" +
                            $"This will take around 5 - 30 mins (approx) to transfer to {Constants.DeviceName}.\nPlease do not close app while uploading.",
                        OkText = "Yes",
                        CancelText = "No"
                    };
                }

                ok = await _userDialogs.ConfirmAsync(confirmConfig);
            }

            if (ok)
            {
                foreach (var asset in systemAudioThatAreNotInTalkiPlayer)
                {
                    _queue.Enqueue(asset);
                }

                foreach (var asset in pack.AudioAssets)
                {
                    _queue.Enqueue(asset);
                }

                NumberOfItemsInQueue = _queue.Count;

                ProgressDialog = _userDialogs.Progress(new ProgressDialogConfig()
                {
                    Title = $"{NumberOfItemsProcessed} / {TotalNumberOfItems} assets uploading ...",
                    IsDeterministic = true,
                    MaskType = MaskType.Black,
                    CancelText = "Cancel",
                    AutoShow = true,
                    OnCancel = () =>
                    {
                        AudioDownloadResult?.Cancel();
                        vm.Icon = Images.DownloadIcon;
                    }
                });
            }

            return Unit.Default;
        }
        
        void SetupDownloadMonitor()
        {
              Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                      h => AudioDownloadResult.PropertyChanged += h,
                      h => AudioDownloadResult.PropertyChanged -= h)
                  .ObserveOn(RxApp.MainThreadScheduler)
                  .Subscribe(e =>
                  {
                      if (!(e.Sender is IDownloadFile file)) return;
              
                      _logger?.Information(
                          $"Total Downloaded: {file.TotalBytesWritten}, Status: {file.Status}, Path: {file.DestinationPathName}");
              
                      if (e.EventArgs.PropertyName.Equals(nameof(IDownloadFile.Status)))
                      {
                          switch (file.Status)
                          {
                              case DownloadFileStatus.FAILED:
                                  ProgressDialog?.Dispose();
                                  Selected.Icon = Images.RetryIcon;
                                  break;
                              case DownloadFileStatus.CANCELED:
                                  ProgressDialog?.Hide();
                                  ProgressDialog?.Dispose();
                                  Selected.Icon = Images.DownloadIcon;
                                  break;
                              case DownloadFileStatus.COMPLETED:
                                
                                  if (CurrentItem is AssetDto asset)
                                  {
                                      asset.FilePath = file.DestinationPathName;
                                  }

                                  UploadCommand.Execute(CurrentItem).Subscribe();
                                  break;
                          }
                      }
                  });
          }                
    }
}