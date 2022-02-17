using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;
using System.Reactive;
using ChilliSource.Core.Extensions;
using ReactiveUI.Fody.Helpers;
using System.Threading.Tasks;
using Plugin.BluetoothLE;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public interface ITagItemStartPageViewModel : IBasePageViewModel, IActivatableViewModel
    {
        ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

        ReactiveCommand<Unit, Unit> BeginCommand { get; }

        string Heading { get; }

        string InstructionImage { get; }

        string SubHeading { get; }
    }

    public class TagItemStartPageViewModel : BasePageViewModel, ITagItemStartPageViewModel, IModalViewModel
    {
        private readonly IGameMediator _gameMediator;
        protected readonly IRoom _room;
        //protected readonly IPack Pack;
        private readonly ILogger _logger;
        protected readonly IUserDialogs _userDialogs;
        private readonly ITalkiPlayerManager _talkiPlayerManager;
        //private readonly IApplicationService _applicationService;
        private readonly IConnectivityNotifier _connectivityNotifier;
        //private readonly IGameService _gameService;
        private readonly IAssetRepository _assetRepository;

        public TagItemStartPageViewModel(INavigationService navigator,
            //IGameService gameService = null,
            IAssetRepository assetRepository = null,
            IConnectivityNotifier connectivityNotifier = null,
            //IApplicationService applicationService = null,
            ITalkiPlayerManager talkiPlayerManager = null,
            IUserDialogs userDialogs = null,
            ILogger logger = null,
           IGameMediator gameMediator = null)
        {
             _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
             _logger = logger ?? Locator.Current.GetService<ILogger>();
             _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
             _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
             //_applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            _assetRepository = assetRepository ?? Locator.Current.GetService<IAssetRepository>();
            //_gameService = gameService ?? Locator.Current.GetService<IGameService>();
             Activator = new ViewModelActivator();
             Navigator = navigator;

            _room = _gameMediator.CurrentRoom;
            CurrentPack = _gameMediator.CurrentPack;
            Heading = $"Welcome to your {CurrentPack.Name.ToLower()} game pack";
            SubHeading = $"I want to setup my game pack now";
            InstructionImage = CurrentPack.ImagePath.ToResizedImage(height: 250) ?? Images.PlaceHolder;
            SetupRx();
            SetupCommands();
        }

        public override string Title => "Welcome!";
        public ViewModelActivator Activator { get; }
        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; protected set; }

        public ReactiveCommand<Unit, Unit> BeginCommand { get; protected set; }

        protected IList<ItemDto> PackItems { get; private set; }

        [Reactive] public string Heading { get; set; }

        [Reactive] public string InstructionImage { get; set; }

        [Reactive] public string SubHeading { get; set; }

        [Reactive] public object Parameters { get; set; }

        public IPack CurrentPack { get; set; }

        void SetupRx()
         {
             this.WhenActivated(d =>
             {
                 _connectivityNotifier.Notifier.RegisterHandler(async context =>
                 {
                     await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());                     
                     context.SetOutput(true);
                 }).DisposeWith(d);
                
                //_talkiPlayerManager.Current?.WhenDisconnectedUponInactivity()
                //                                .SubscribeSafe()
                //                                .DisposeWith(d);
             });
         }

        IDisposable _audioFilesResultSubscription;

        void SetupCommands()
         {
            LoadDataCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(m =>
            {
                //_userDialogs.ShowLoading("Loading ...");
                //CurrentPack = await _gameService.GetPack(Pack.Id);
                //_userDialogs.HideLoading();
                PackItems = CurrentPack?.Items.OrderBy(a => a.Type.GetData<int>("SortOrder")).ToList();
                return Task.FromResult(Unit.Default);
            });
          
            LoadDataCommand.ThrownExceptions
                 .ObserveOn(RxApp.MainThreadScheduler)
                 .HideLoading()
                 .ShowExceptionDialog()
                 .SubscribeAndLogException();

            BeginCommand = ReactiveCommand.CreateFromTask( () =>
            {                
                if (_talkiPlayerManager.IsTalkiPlayerReady)
                {
                    StartTagItemSetup();
                }
                else
                {
                    Dialogs.Toast(Dialogs.BuildErrorToast($"{Constants.DeviceName} not connected!"));
                    BackCommand.Subscribe();                    
                }

                return Task.CompletedTask;
               
            });
           
             BeginCommand.ThrownExceptions.SubscribeAndLogException();


             BackCommand = ReactiveCommand.CreateFromTask(() =>
             {
                 return SimpleNavigationService.PopModalAsync();
             });
             BackCommand.ThrownExceptions.SubscribeAndLogException();

         }

        void StartTagItemSetup()
        {            
            _userDialogs.ShowLoading("Loading ...");

            try
            {
                _audioFilesResultSubscription = _talkiPlayerManager.Current?.OnDataResult()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(async data =>
                {
                    _audioFilesResultSubscription.Dispose();

                    if (data.Type == UploadDataType.AvailableAudioFiles && data.IsSuccess && data.Data is AvailableAudioFiles files)
                    {
                        var audioAssetsInTalkPlayer = files.AudioFiles?.Select(a => a.ToLower()).ToList() ?? new List<string>();

                        var systemAssets = await _assetRepository
                            .GetAssets(AssetType.Audio, Category.System);

                        var audioAssetsToBeUploaded = new List<string>();
                        audioAssetsToBeUploaded.AddRange(CurrentPack.AudioAssets.Select(a => a.Filename.ToLower()));
                        audioAssetsToBeUploaded.AddRange(systemAssets.Select(a => a.Filename.ToLower()));
                        var hasAll = audioAssetsToBeUploaded.All(a => audioAssetsInTalkPlayer.Contains(a));

                        _userDialogs.HideLoading();

                        if (!hasAll)
                        {
                            var confirmConfig = new ConfirmConfig()
                            {
                                Title = "Audio files are missing",
                                Message = $"{Constants.DeviceName} does not have all the audio files necessary for the selected {CurrentPack.Name} pack. " +
                                $"Would you like to upload these audio files now?",
                                OkText = "Ok",
                                CancelText = "Cancel"
                            };

                            var ok = await _userDialogs.ConfirmAsync(confirmConfig);

                            if (ok)
                            {
                                await SimpleNavigationService.PushChildModalPage(new AudioPackListPageViewModel(Navigator,
                                    AudioPackNavigationSource.TagItemStartPage, CurrentPack.Id));
                                return;
                            }
                        }

                    }
                    else
                    {
                        _userDialogs.HideLoading();
                        _userDialogs.Toast(Dialogs.BuildErrorToast("Could not determine device audio content"));
                    }

                    var mediator = new TagItemsSelector(_room, PackItems, CurrentPack);
                    var current = mediator.GetNextItem();
                    if (current != null)
                    {
                        await SimpleNavigationService.PushAsync(new TagItemSetupPageViewModel(Navigator, mediator, current));
                        return;
                    }

                    _userDialogs.Toast("There are no items to be tagged.");
                })
                .SubscribeSafe();
            }
            catch(Exception exc)
            {
                throw exc;
            }

            _talkiPlayerManager.Current.Upload(new DataUploadData("AudioList",
                DataRequest.GetAudioFileListRequest(), "AudioList", UploadDataType.AvailableAudioFiles));
        }
        
    }
}