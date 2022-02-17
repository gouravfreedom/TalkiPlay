using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using ChilliSource.Mobile.Core;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using FormsControls.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Unit = System.Reactive.Unit;


namespace TalkiPlay.Shared
{
    public class RoomListPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        private readonly IBackgroundAudioPlayer _backgroundAudioPlayer;
        private readonly IUserSettings _userSettings;
        protected readonly IGameMediator _gameMediator;
        protected readonly IUserDialogs _userDialogs;
        protected readonly ITalkiPlayerManager _talkiPlayerManager;
        protected readonly IApplicationService _applicationService;
        private readonly IConnectivityNotifier _connectivityNotifier;
        protected readonly IGameService _gameService;
        protected readonly IAssetRepository _assetRepository;
        //protected readonly SourceList<RoomViewModel> _rooms = new SourceList<RoomViewModel>();
        //private readonly ObservableDynamicDataRangeCollection<RoomViewModel> _roomList = new ObservableDynamicDataRangeCollection<RoomViewModel>();

        public RoomListPageViewModel(INavigationService navigator,
            IGameService gameService = null,
            IConnectivityNotifier connectivityNotifier = null,
            IApplicationService applicationService = null,
            ITalkiPlayerManager talkiPlayerManager = null,
            IUserDialogs userDialogs = null,
            IGameMediator gameMediator = null,
            bool reset = true,
            IUserSettings userSettings = null,
            IBackgroundAudioPlayer backgroundAudioPlayer = null)
        {
            _backgroundAudioPlayer = backgroundAudioPlayer ?? Locator.Current.GetService<IBackgroundAudioPlayer>();
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            _gameService = gameService ?? Locator.Current.GetService<IGameService>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            Activator = new ViewModelActivator();
            Navigator = navigator;

            Rooms = new ObservableDynamicDataRangeCollection<RoomViewModel>();

            SetupRx();
            SetupCommands();

            if (reset)
            {
                _gameMediator.Reset();
            }
        }

        public override string Title => "Select a room";

        public ViewModelActivator Activator { get; }

        public ObservableDynamicDataRangeCollection<RoomViewModel> Rooms { get; private set; }

        public ICommand AddCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; protected set; }

        public ReactiveCommand<RoomViewModel, Unit> SelectCommand { get; protected set; }

        [Reactive]
        public bool ShowLeftMenuItem { get; set; } = false;


        public override IPageAnimation PageAnimation { get; set; } = new SlidePageAnimation()
        {
            BounceEffect = false,
            Subtype = AnimationSubtype.FromRight,
            Duration = AnimationDuration.Short
        };

        protected IList<PackDto> Packs { get; set; }

        private void SetupRx()
        {
            this.WhenActivated(d =>
            {
                Observable.FromAsync(() => _backgroundAudioPlayer.Play(Constants.ExploreMusic))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(_ => _backgroundAudioPlayer.ChangeVolume(Constants.SmallMusicVolume))
                    .SubscribeSafe()
                    .DisposeWith(d);

                //_rooms.Connect()
                //    .ObserveOn(RxApp.MainThreadScheduler)
                //    .Bind(_roomList)
                //    .DisposeMany()
                //    .Subscribe()
                //    .DisposeWith(d);


                _connectivityNotifier.Notifier.RegisterHandler(async context =>
                {
                    await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                    context.SetOutput(true);
                }).DisposeWith(d);

                this.WhenAnyObservable(m => m.LoadDataCommand.IsExecuting)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToPropertyEx(this, m => m.IsLoading)
                    .DisposeWith(d);

            });
        }

        private void SetupCommands()
        {
            AddCommand = ReactiveCommand.CreateFromTask(() =>
            {
                return SimpleNavigationService.PushModalAsync(new NavigationItemViewModel<AddEditRoomPageViewModel>());
            });

            LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _userDialogs.ShowLoading("Loading ...");
                var packs = await _assetRepository.GetPacks();
                var rooms = await _assetRepository.GetRooms();
                _userDialogs.HideLoading();
                //var size = _applicationService.ScreenSize;
                //var width = size.Width;

                Rooms.Clear();
                using (Rooms.SuspendNotifications())
                {
                    Rooms.AddRange(rooms.Select(room => new RoomViewModel
                    {
                        Room = room,
                        HeroTitle = room.Name,
                        //BackgroundImage = room.ImagePath.ToResizedImage(width - 40) ?? Images.PlaceHolder,
                        BackgroundImage = room.ImagePath ?? Images.PlaceHolder,
                        EditCommand = ReactiveCommand.CreateFromTask(() =>
                        {
                            return SimpleNavigationService.PushModalAsync(new NavigationItemViewModel<AddEditRoomPageViewModel>(room));
                        })
                    }));
                }


                //_rooms.Edit(items =>
                //{
                //    items.Clear();
                //    items.AddRange(rooms.Select(room => new RoomViewModel
                //    {
                //        Room = room,
                //        HeroTitle = room.Name,
                //        //BackgroundImage = room.ImagePath.ToResizedImage(width - 40) ?? Images.PlaceHolder,
                //        BackgroundImage = room.ImagePath ?? Images.PlaceHolder,
                //        EditCommand = ReactiveCommand.CreateFromObservable(() =>
                //        {
                //            return Navigator.PushModal(new NavigationItemViewModel<AddEditRoomPageViewModel>(room)); 
                //        })
                //    }).ToList());

                //});

                await ReloadGameIfActive();

                //if (!_userSettings.IsOnboarded)
                //{
                //    await Navigator.PushModal(new OnboardingPageViewModel(Navigator));
                //}

                return Unit.Default;
            });

            LoadDataCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();

            SelectCommand = ReactiveCommand.CreateFromTask<RoomViewModel, Unit>(async (vm) =>
              {
                  _userDialogs.ShowLoading("Loading ...");
                  var room = await _assetRepository.GetRoom(vm.Room.Id);
                  var items = await _assetRepository.GetItems(ItemType.Home);
                  _userDialogs.HideLoading();

                  _gameMediator.CurrentRoom = room;
                  _gameMediator.HomeTags = items;

                  await SimpleNavigationService.PushAsync(new GameListPageViewModel(Navigator));
                  return Unit.Default;
              });
            
            SelectCommand.ThrownExceptions
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();

        }

        async Task ReloadGameIfActive()
        {
            var reloadGameSession = _userSettings.ReloadGameSession;

            if (reloadGameSession)
            {
                _userSettings.ReloadGameSession = false;


                if (_userSettings.HasTalkiPlayerDevice)
                {

                    var gameSession = _userSettings.CurrentGameSession;

                    if (gameSession != null
                        && gameSession.TimeStamp < DateTime.Now.AddDays(1)
                        && gameSession.GameSessionRecords.Any(a => a.Status != GameSessionStatus.Completed))
                    {
                        var ok = await ConfirmReload();

                        if (!ok)
                        {
                            _userSettings.ClearGameSession();
                        }
                        else
                        {
                            SimpleNavigationService.PushAsync(new GameSessionPageViewModel(true, Navigator)).Forget();
                        }
                    }
                }
                else
                {
                    var gameSession = _userSettings.CurrentQRGameSession;

                    if (gameSession != null
                        && gameSession.TimeStamp < DateTime.Now.AddDays(1))
                    {
                        var ok = await ConfirmReload();

                        if (!ok)
                        {
                            _userSettings.ClearGameSession();
                        }
                        else
                        {
                            await SimpleNavigationService.PushModalAsync(new QRCodeGameSessionPageViewModel(true));
                        }
                    }
                }

            }
        }

        async Task<bool> ConfirmReload()
        {
            return await _userDialogs.ConfirmAsync(new ConfirmConfig()
            {
                Title = "Reload game?",
                Message = "You currently have active game session. Would you like to continue with game?",
                OkText = "Yes",
                CancelText = "No"
            });
        }

    }
}