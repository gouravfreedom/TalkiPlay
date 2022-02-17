using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using MediaManager;
using MediaManager.Media;
using MediaManager.Playback;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public class AudioListPageViewModel  : BasePageViewModel, IActivatableViewModel, IModalViewModel
    {
        private readonly ILogger _logger;
        private readonly IAssetRepository _assetRepository;
        private readonly ITalkiPlayerManager _talkiPlayerManager;
        private readonly IConfig _config;
        private readonly IUserDialogs _userDialogs;
        private readonly SourceList<AudioItemViewModel> _audioItemsList = new SourceList<AudioItemViewModel>();
        private readonly Queue<AudioItemViewModel> _queue = new Queue<AudioItemViewModel>();
        public AudioListPageViewModel(INavigationService navigator, 
            IUserDialogs userDialogs = null,
            IConfig config = null,
            ITalkiPlayerManager talkiPlayerManager = null,
            IAssetRepository assetRepository = null,
            ILogger logger  = null
            )
        {
           _logger = logger ?? Locator.Current.GetService<ILogger>();
           _assetRepository = assetRepository ?? Locator.Current.GetService<IAssetRepository>();
            _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _config = config ?? Locator.Current.GetService<IConfig>();
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            Activator = new ViewModelActivator();
            Navigator = navigator;
            SetupCommands();
            SetupRx();
        }

        private void SetupRx()
        {
            var comparer = OrderedComparer<AudioItemViewModel>
                .OrderBy(m => m.Name);

            this.WhenActivated(d =>
            {
           
                Observable.FromEventPattern<MediaItemFinishedEventHandler, MediaItemEventArgs>(h => CrossMediaManager.Current.MediaItemFinished += h,
                        h =>  CrossMediaManager.Current.MediaItemFinished -= h)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .SubscribeSafe(_ =>
                    {
                        if (CurrentPlaying != null)
                        {
                            CurrentPlaying.IsPlaying = false;
                        }
                    })
                    .DisposeWith(d);
                
                _audioItemsList.Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Sort(comparer)
                    .Bind(out var audioItems)
                    .DisposeMany()
                    .Subscribe()
                    .DisposeWith(d);

                AudioItems = audioItems;

                _talkiPlayerManager.Current.OnDataResult()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(data =>
                    {
                        var id = 0;
                      
                        switch (data.Type)
                        {
                            case UploadDataType.AudioDelete:
                               
                                if (int.TryParse(data.Tag , out id))
                                {
                                    var vm = _audioItemsList.Items.FirstOrDefault(a => a.Asset.Id == id);
                                    if (vm != null)
                                    {
                                        vm.DownloadStatus = data.IsSuccess ? AudioUpdateStatus.None : AudioUpdateStatus.Uploaded;
                                    }
                                }
                                break;
                            
                            case UploadDataType.AvailableAudioFiles:
                                if (data.IsSuccess && data.Data is AvailableAudioFiles files)
                                {
                                    foreach (var v in _audioItemsList.Items)
                                    {
                                        _logger.Information($"asset - { v.Asset.Name} - {v.Asset.Filename}");
                                    }

                                    
                                    foreach (var file in files.AudioFiles)
                                    {
                                        var audio = _audioItemsList.Items.FirstOrDefault(a => a.Asset.Filename.ToLower() == file.ToLower());

                                      
                                        if (audio != null)
                                        {
                                            _logger.Information($"Audio -> {audio.Asset.Name}");
                                            audio.IsExists = true;
                                            audio.IsDownloaded = true;
                                            audio.DownloadStatus = AudioUpdateStatus.Uploaded;
                                        }
                                        else
                                        {
                                            _logger.Information($"Not Found Audio -> {file}");
 
                                        }
                                    }
                                }

                                break;
                            case UploadDataType.Audio:
                                if (int.TryParse(data.Tag , out id))
                                {
                                    var vm = _audioItemsList.Items.FirstOrDefault(a => a.Asset.Id == id);
                                    if (vm != null)
                                    {
                                        vm.DownloadStatus = data.IsSuccess ? AudioUpdateStatus.Uploaded : AudioUpdateStatus.None;
                                        WaitingForItemResult = false;
                                        CurrentItem = null;
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
                    .SelectMany(m =>
                    {
                        var next = _queue.Dequeue();

                        if (next != null)
                        {
                            CurrentItem = next;
                            WaitingForItemResult = true;
                            NumberOfItemsInQueue = _queue.Count;
                            return next.DownloadCommand.Execute();
                        }

                        return Observable.Return(Unit.Default);
                    })
                    .SubscribeSafe()
                    .DisposeWith(d);

                this.WhenAnyValue(m => m.NumberOfItemsInQueue)
                    .Where(m => m == 0)
                    .HideLoading()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(m => ShowUpdateAllButton = true)
                    .SubscribeSafe()
                    .DisposeWith(d);
            });
        }
        
        private void SetupCommands()
        {
            LoadCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _userDialogs.ShowLoading("Loading ...");
                var assets = await _assetRepository.GetAssets(AssetType.Audio);
                _userDialogs.HideLoading();

                IsRefreshing = false;
                _audioItemsList.Edit(items =>
                {
                    items.Clear();
                    foreach (var asset in assets.Where(a => a.Type == AssetType.Audio && a.Name.StartsWith("AASystem",StringComparison.InvariantCultureIgnoreCase)))
                    {
                        items.Add(new AudioItemViewModel(asset, AudioPlayCommand)
                        {
                            Name = asset.Name,
                        });
                    }                   
                });

                _talkiPlayerManager.Current.Upload(new DataUploadData("AudioList",
                     DataRequest.GetAudioFileListRequest(), "AudioList", UploadDataType.AvailableAudioFiles));

            });

            //LoadCommand = ReactiveCommand.CreateFromObservable(() =>
            //{
            //    return ObservableOperatorExtensions.StartShowLoading("Loading ...")
            //        .ObserveOn(RxApp.TaskpoolScheduler)
            //        .SelectMany(_ => _assetService.GetAssets(AssetType.Audio))
            //        .Select(m => m.Where(a => a.Type == AssetType.Audio))
            //        .ObserveOn(RxApp.MainThreadScheduler)
            //        .HideLoading()
            //        .Do(m =>
            //        {
            //            IsRefreshing = false;
            //            _audioItemsList.Edit(items =>
            //            {
            //                items.Clear();
            //                items.AddRange(m.Select(a => new AudioItemViewModel(a, AudioPlayCommand)
            //                {
            //                    Name = a.Name,

            //                }).ToList());
            //            });
                        
            //           _talkiPlayerManager.Current.Upload(new DataUploadData("AudioList",
            //                DataRequest.GetAudioFileListRequest(), "AudioList", UploadDataType.AvailableAudioFiles));
            //        })
            //        .Select(_ => Unit.Default);
            //});

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
            
            SelectCommand = ReactiveCommand.CreateFromObservable<AudioItemViewModel, Unit>(_ => { return Observable.Return(Unit.Default); });
            SelectCommand.ThrownExceptions.SubscribeAndLogException();

            AudioPlayCommand = ReactiveCommand.CreateFromTask<AudioItemViewModel, Unit>(async item =>
            {
                if (CurrentPlaying != null)
                {
                    if (CurrentPlaying != item || CurrentPlaying == item)
                    {
                        await CrossMediaManager.Current.Stop();
                        CurrentPlaying.IsPlaying = false;
                        CurrentPlaying = null;
                    }
                }

                var filePath = $"{_config.GetAssetDownloadUrl(item.Asset.Id)}?ApiKey={_config.ApiKey}";
                await CrossMediaManager.Current.Play(filePath);
                item.IsPlaying = true;
                CurrentPlaying = item;

                return Unit.Default;
            });

            AudioPlayCommand.ThrownExceptions.SubscribeAndLogException();
            
            UpdateAllCommand = ReactiveCommand.Create(() =>
            {
                _logger.Information($"Queing {_audioItemsList.Items.Count()} for upload");
                foreach (var audioItemViewModel in _audioItemsList.Items.OrderBy(a => a.Name))
                {
                    _queue.Enqueue(audioItemViewModel);
                }

                NumberOfItemsInQueue = _queue.Count;
                ShowUpdateAllButton = false;
                _userDialogs.ShowLoading("Updating ...", MaskType.Clear);
            });

            UpdateAllCommand.ThrownExceptions.SubscribeAndLogException();
        }

        public override string Title => "Audio assets update";
      
        public ReadOnlyCollection<AudioItemViewModel> AudioItems { get; private set; }

        public ViewModelActivator Activator { get; }
        
        public ReactiveCommand<Unit, Unit> LoadCommand { get; private set;}
        public ReactiveCommand<AudioItemViewModel, Unit> SelectCommand { get; private set; }
        
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; set; }

        [Reactive] public bool IsRefreshing { get; set; }
        
        public extern bool IsReady { [ObservableAsProperty] get; }

        public ReactiveCommand<AudioItemViewModel, Unit> AudioPlayCommand { get; private set; }
        
        private AudioItemViewModel CurrentPlaying { get; set; }
        
        public ReactiveCommand<Unit, Unit> UpdateAllCommand { get; private set; }
        
        [Reactive]
        public AudioItemViewModel CurrentItem { get; set; }
        
        [Reactive]
        public bool WaitingForItemResult { get; set; }
        
        [Reactive]
        public int NumberOfItemsInQueue { get; set; }

        [Reactive] public bool ShowUpdateAllButton { get; set; } = true;
        
       

    }
}