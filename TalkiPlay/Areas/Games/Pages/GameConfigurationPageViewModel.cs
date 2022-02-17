using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using FormsControls.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Essentials;
using Xamarin.Forms;
using Unit = System.Reactive.Unit;

namespace TalkiPlay.Shared
{
    public class GameConfigurationPageViewModel : DeviceConnectionViewModel
    {
        readonly IGame _game;
        readonly IBackgroundAudioPlayer _backgroundAudioPlayer;
        readonly IUserSettings _userSettings;
        readonly IGameMediator _gameMediator;
        readonly IConnectivityNotifier _connectivityNotifier;
        readonly IGameService _gameService;
        readonly IAssetRepository _assetRepository;
        AssetDownloadManager _downloadManager;

        public GameConfigurationPageViewModel(INavigationService navigator,
            IGame game,
            IGameService gameService = null,
            IConnectivityNotifier connectivityNotifier = null,
            IGameMediator gameMediator = null,
            IUserSettings userSettings = null,
            IBackgroundAudioPlayer backgroundAudioPlayer = null)
        {
            _game = game;
            _backgroundAudioPlayer = backgroundAudioPlayer ?? Locator.Current.GetService<IBackgroundAudioPlayer>();
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();

            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            _gameService = gameService ?? Locator.Current.GetService<IGameService>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            Navigator = navigator ?? Locator.Current.GetService<INavigationService>(Constants.MainNavigation);
            _gameMediator.SetGameSessionId(Guid.NewGuid());
            SetupCommand();
        }

        private bool _hasLoadedFirstTime = false;
        public override string Title => "Ready to learn";

        [Reactive]
        public string GameShortDescription { get; set; }

        [Reactive]
        public string GameTitle { get; set; }

        [Reactive]
        public string GameBackgroundImage { get; set; }

        [Reactive]
        public string ItemsBackgroundImage { get; set; }

        [Reactive]
        public int Level { get; set; } = 1;

        [Reactive]
        public bool ShowTalkiPlayers { get; set; }

        public ReadOnlyObservableCollection<PlayerViewModel> Players { get; private set; }
        public ReadOnlyObservableCollection<TalkiPlayerBaseViewModel> TalkiPlayers { get; private set; }


        public ReactiveCommand<Unit, Unit> SelectPlayerCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> SelectTalkiPlayerCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> SelectRoomCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> OnboardGameItemsCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> ConfigureActiveItemsCommand { get; private set; }


        public ReactiveCommand<Unit, Unit> RemoveAllChildrenCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> StartGameCommand { get; private set; }

        public extern int ChildrenCount { [ObservableAsProperty] get; }
        public extern int DeviceCount { [ObservableAsProperty] get; }

        readonly SourceList<ItemConfigurationViewModel> _items = new SourceList<ItemConfigurationViewModel>();
        public ReadOnlyObservableCollection<ItemConfigurationViewModel> Items { get; private set; }


        protected override void OnVmAcivated(CompositeDisposable d)
        {
            base.OnVmAcivated(d);
            ShowTalkiPlayers = _userSettings.HasTalkiPlayerDevice;

            if (_userSettings.PlayInGameMusic)
            {

                Observable.FromAsync(() => _backgroundAudioPlayer.Play(Constants.HuntMusic))
                    .Delay(TimeSpan.FromSeconds(1))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(_ => _backgroundAudioPlayer.ChangeVolume(Constants.SmallMusicVolume))
                    .SubscribeSafe()
                    .DisposeWith(d);
            }
            else
            {
                _backgroundAudioPlayer.Stop();
            }

            if (_userSettings.CurrentChild != null && !_gameMediator.Children.Items.Any(c => c.Id == _userSettings.CurrentChild.Id))
            {
                _gameMediator.Children.Insert(_gameMediator.Children.Count - 1, _userSettings.CurrentChild);
            }

            _gameMediator.Children.Connect()
                .Transform(m =>
                {
                    if (m is EmptyChild)
                    {
                        return new SelectPlayerViewModel(this.SelectPlayerCommand);
                    }

                    return new ChildPlayerViewModel(m,
                        ReactiveCommand.Create<IChild, Unit>(child =>
                        {
                            _gameMediator.Children.Edit(items =>
                            {
                                var c = items.NotEmpty().FirstOrDefault(a => a.Id == child.Id);
                                if (c != null)
                                {
                                    items.Remove(c);
                                }
                            });

                            return Unit.Default;
                        })
                    ) as PlayerViewModel;
                })
                .Bind(out var players)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(d);

            Players = players;


            if (Items == null || Items.Count == 0)
            {
                _items.Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out var items)
                    .DisposeMany()
                    .Subscribe()
                    .DisposeWith(d);

                Items = items;
            }

            _connectivityNotifier.Notifier.RegisterHandler(async context =>
            {
                await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                context.SetOutput(true);
            }).DisposeWith(d);

            if (_userSettings.HasTalkiPlayerDevice)
            {
                _gameMediator.Devices.Edit(items => {
                    items.RemoveMany(items.Where(a => !(a is EmptyTalkiPlayerData)));
                 });

                if (_talkiPlayerManager.Current?.IsConnected == true)
                {
                    _gameMediator.Devices.Edit(items =>
                    {
                        items.Add(new TalkiPlayerData()
                        {
                            Name = _talkiPlayerManager.Current?.Name,
                            DeviceId = _talkiPlayerManager.Current.Device.Uuid,
                            Time = DateTime.Now,
                            Device = _talkiPlayerManager.Current
                        });
                    });
                }

                _gameMediator.Devices.Connect()
                    .Transform(m =>
                    {
                        if (m is EmptyTalkiPlayerData)
                        {
                            var item = new SelectTalkiPlayerViewModel(this.SelectTalkiPlayerCommand);
                            item.ChildrenCount = _gameMediator.Children.Count;
                            return item;
                        }

                        return new TalkiPlayerViewModel(m,
                            ReactiveCommand.Create<ITalkiPlayerData, Unit>(device =>
                            {
                                _gameMediator.Devices.Edit(items =>
                                {
                                    var c = items.NotEmpty().FirstOrDefault(a => a.Name == device.Name);
                                    if (c != null)
                                    {
                                        c.Device?.Disconnect();
                                        items.Remove(c);
                                        c.Device?.Dispose();
                                    }
                                });

                                return Unit.Default;
                            })
                        ) as TalkiPlayerBaseViewModel;
                    })
                    .Bind(out var devices)
                    .Subscribe()
                    .DisposeWith(d);

                TalkiPlayers = devices;

                _gameMediator.Devices.CountChanged
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToPropertyEx(this, v => v.DeviceCount)
                    .DisposeWith(d);

            }

            _gameMediator.Children.CountChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToPropertyEx(this, v => v.ChildrenCount)
                .DisposeWith(d);

            this.WhenAnyValue(m => m.ChildrenCount)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeSafe(m =>
                {
                    if (_userSettings.HasTalkiPlayerDevice)
                    {
                        foreach (var item in TalkiPlayers)
                        {
                            if (item is SelectTalkiPlayerViewModel vm)
                            {
                                vm.ChildrenCount = m;
                            }
                        }
                    }
                })
                .DisposeWith(d);
        }


        void SetupCommand()
        {
            LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                Dialogs.ShowLoading();

                var rooms = await _assetRepository.GetRooms();
                var defRoom = rooms.FirstOrDefault(r => r.Name.ToLower() == "kindergarten");
                if (defRoom == null)
                {
                    var assets = await _assetRepository.GetAssets(AssetType.Image, Category.Room);
                    var firstAsset = assets.FirstOrDefault();
                    var room = new RoomDto() { Id = 0, Name = "Kindergarten", AssetId = firstAsset != null ? firstAsset.Id : 0 };
                    defRoom = await _assetRepository.AddOrUpdateRoom(room);
                }

                defRoom = await _assetRepository.GetRoom(defRoom.Id);
                _gameMediator.CurrentRoom = defRoom;
                _gameMediator.HomeTags = await _assetRepository.GetItems(ItemType.Home);
                _gameMediator.Tags = await _assetRepository.GetTags();

                var game = await _gameService.GetGame(_game.Id);                
                _gameMediator.CurrentGame = game;

                var pack = await _assetRepository.GetPack(game.PackId);
                _gameMediator.CurrentPack = pack;

                GameTitle = game.Name;
                GameShortDescription = game.ShortDescription;
                GameBackgroundImage = game.ImagePath;//.ToResizedImage(250);

                var packItems = _gameMediator.CurrentPack.Items
                    .Where(i => i.Id > 0 && !string.IsNullOrEmpty(i.Name))
                    .DistinctBy(i => i.Id)
                    .Where(i => i.Type != ItemType.Home)
                    .ToList();

                Dialogs.HideLoading();                

                if (_userSettings.HasTalkiPlayerDevice)
                {
                    var tagItemIds = _gameMediator.Tags.SelectMany(a => a.ItemIds).ToList();
                    var roomTagItemIds = _gameMediator.CurrentRoom?.TagItems.SelectMany(a => a.ItemIds).ToList();
                    var models = packItems
                        .Where(a => tagItemIds.Contains(a.Id))
                        .Select(a =>
                        {
                            var model = new ItemConfigurationViewModel(a, true);
                            model.InitDefaultState(roomTagItemIds.Contains(a.Id));
                            return model;
                        })
                        .ToList();

                    _items.Edit(items =>
                    {
                        items.Clear();
                        items.AddRange(models);
                    });
                }
                else
                {
                    var models = new List<ItemConfigurationViewModel>();
                    if (_gameMediator.CurrentGame.Type == GameType.Explore)
                    {
                        foreach (var item in packItems)
                        {
                            models.Add(new ItemConfigurationViewModel(item, false, _userSettings.ItemsSettings));
                        }
                    }
                    else
                    {
                        var instructions = _gameMediator.CurrentGame.Instructions;
                        foreach (var instruction in instructions)
                        {
                            models.Add(new ItemConfigurationViewModel(instruction.Item, false, _userSettings.ItemsSettings));
                        }
                    }

                    _items.Edit(items =>
                    {
                        items.Clear();
                        _items.AddRange(models);                        
                    });
                }

                _hasLoadedFirstTime = true;
            });

            LoadDataCommand.ThrownExceptions
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();

            BackCommand = ReactiveCommand.Create(() =>
            {
                SimpleNavigationService.PopToRootAsync().Forget();
            });

            BackCommand.ThrownExceptions.SubscribeAndLogException();

            SelectPlayerCommand = ReactiveCommand.Create(() =>
            {
                SimpleNavigationService.PushModalAsync(
                    new ChildListPageViewModel(true, _gameMediator)).Forget();
            });

            SelectPlayerCommand.ThrownExceptions.SubscribeAndLogException();

            RemoveAllChildrenCommand = ReactiveCommand.Create(() => _gameMediator.Children.Edit(items =>
            {
                if (items.Count > 1)
                {
                    items.RemoveRange(1, items.Count - 1);
                }
            }));

            RemoveAllChildrenCommand.ThrownExceptions.SubscribeAndLogException();

            SelectRoomCommand = ReactiveCommand.Create(() =>
           {
               SimpleNavigationService.PushAsync(new GameRoomListPageViewModel(Navigator)
               {
                   ShowLeftMenuItem = true,
                   PageAnimation = new SlidePageAnimation()
                   {
                       BounceEffect = false,
                       Subtype = AnimationSubtype.FromBottom,
                       Duration = AnimationDuration.Short
                   }
               }).Forget();
           });

            SelectRoomCommand.ThrownExceptions.SubscribeAndLogException();

            ConfigureActiveItemsCommand = ReactiveCommand.Create(() =>
           {
               SimpleNavigationService.PushAsync(new GameActiveItemsConfigurationPageViewModel(Navigator)
               {
                   ShowLeftMenuItem = true,
                   PageAnimation = new SlidePageAnimation()
                   {
                       BounceEffect = false,
                       Subtype = AnimationSubtype.FromBottom,
                       Duration = AnimationDuration.Short
                   }
               }).Forget();
           });


            ConfigureActiveItemsCommand.ThrownExceptions.SubscribeAndLogException();

            SelectTalkiPlayerCommand = ReactiveCommand.CreateFromTask(() =>
           {
               return SimpleNavigationService.PushModalAsync(new NavigationItemViewModel<TapToPairPageViewModel>());
           });
            SelectTalkiPlayerCommand.ThrownExceptions.SubscribeAndLogException();

            var canExecute = this.WhenAnyValue(m => m.ChildrenCount,
                 m => m.IsConnected, (children, connected) => children > 1 && (connected || !_userSettings.HasTalkiPlayerDevice))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(m => m);

            OnboardGameItemsCommand = ReactiveCommand.Create(() =>
            {
                SimpleNavigationService.PushModalAsync(new TagItemStartPageViewModel(Navigator)).Forget();
            });

            StartGameCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(async (unit) =>
             {
                 if (_userSettings.HasTalkiPlayerDevice)
                 {
                     Dialogs.ShowLoading();
                     bool requireTagging = await IsTagOnboardingNeeded();
                     Dialogs.HideLoading();

                     if (requireTagging)
                     {                         
                         await SimpleNavigationService.PushModalAsync(new TagItemStartPageViewModel(Navigator));
                     }
                     else
                     {
                         await SendGameStartRequest();
                         await SimpleNavigationService.PushAsync(new GameSessionPageViewModel(false, Navigator));
                     }
                 }
                 else
                 {

                     var status = await Permissions.RequestAsync<Permissions.Camera>();

                     if (Device.RuntimePlatform == Device.Android)
                     {
                         await Task.Delay(200);
                     }
                     
                     if (status != PermissionStatus.Granted)
                     {

                         var ok = await Dialogs.ConfirmAsync(new ConfirmConfig()
                         {
                             Title = "Camera permission",
                             Message = $"Please go to Settings to enable camera access for TalkiPlay to be able to scan QR codes.",
                             OkText = "Settings",
                             CancelText = "Cancel"
                         });

                         if (ok)
                         {
                             AppInfo.ShowSettingsUI();
                         }
                     }
                     else
                     {
                         if (!await ShouldDownloadAudioAssets())
                         {
                             //Navigator.PushModal(new QRCodeGameSessionPageViewModel(false, Navigator));
                             await SimpleNavigationService.PushModalAsync(new QRCodeGameSessionPageViewModel(false));
                         }
                     }
                 }
                 return Unit.Default;
             }, canExecute);


            StartGameCommand.ThrownExceptions.SubscribeAndLogException();
        }


        async Task<bool> IsTagOnboardingNeeded()
        {
            //refresh room because its tag items may have been updated if a tag onboarding has
            //been done after the previous call to this function
            var room = await _assetRepository.GetRoom(_gameMediator.CurrentRoom.Id);
            _gameMediator.CurrentRoom = room;
            _gameMediator.CurrentPack = await _assetRepository.GetPack(_gameMediator.CurrentGame.PackId);

            var homeTagItems = _gameMediator.HomeTags.Select(a => a.Id);
            var itemIds = _gameMediator.CurrentPack.Items
                .Where(i => i.Id > 0 && !string.IsNullOrEmpty(i.Name))
                .Where(a => a.Type == ItemType.Default)
                .Select(a => a.Id);

            var hasTags = room?.TagItems.Any(a => !homeTagItems.Any(hti => a.ItemIds.Contains(hti))) ?? false;
            var atLeastOneItemTag = room?.TagItems.Any(a => itemIds.Any(i => a.ItemIds.Contains(i))) ?? false;

            return !hasTags || !atLeastOneItemTag;
        }

        async Task SendGameStartRequest()
        {
            var assets = await _assetRepository.GetAssets(AssetType.Audio);
            assets = assets.Where(a => a.Type == AssetType.Audio).ToList();
            var tags = _gameMediator.Tags;
            var pack = _gameMediator.CurrentPack;              
            var items = pack.Items.Distinct().ToList();
            var room = _gameMediator.CurrentRoom;
            var gameData = new GameData(_gameMediator.GameSessionId.ToString(),
                _gameMediator.CurrentGame, tags, assets, items, room.TagItems);
            _talkiPlayerManager.Current.Upload(new DataUploadData("GameData", gameData, "GAME", UploadDataType.GameData));
        }


        async Task<bool> ShouldDownloadAudioAssets()
        {
           
            Dialogs.ShowLoading();

            _downloadManager = new AssetDownloadManager( _gameMediator);

            var assets = await _downloadManager.GetAssetsToDownload(_gameMediator.CurrentGame);
            
            if (assets.Count == 0)
            {
                Dialogs.HideLoading();
                return false;
            }

            //var ok = await _downloadManager.PromptToDownload();

            //if (!ok)
            //{
            //    return false;
            //}

            _downloadManager.DownloadCompleted += DownloadCompleted;
            _downloadManager.StartDownload(assets);


            return true;

        }

        void DownloadCompleted(object sender, EventArgs e)
        {
            Dialogs.HideLoading();
            _downloadManager.DownloadCompleted -= DownloadCompleted;
            SimpleNavigationService.PushModalAsync(new QRCodeGameSessionPageViewModel(false)).Forget();
        }
    }

}