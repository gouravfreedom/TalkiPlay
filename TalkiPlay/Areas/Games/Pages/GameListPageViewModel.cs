﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Acr.UserDialogs;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.Core;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Forms;
using Unit = System.Reactive.Unit;

namespace TalkiPlay.Shared
{
    public class GameListPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        //private readonly IBackgroundAudioPlayer _backgroundAudioPlayer;
        private readonly ITalkiPlayerManager _talkiPlayerManager;
        private readonly IGameMediator _gameMediator;
        private readonly IConnectivityNotifier _connectivityNotifier;
        private readonly IGameService _gameService;
        private readonly IAssetRepository _assetRepository;
        private readonly IUserSettings _userSettings;
        private bool _hasLoadedFirstTime;

        public GameListPageViewModel(INavigationService navigator,
            bool showBackButton = true,
            IGameService gameService = null,
            IConnectivityNotifier connectivityNotifier = null,
            IGameMediator gameMediator = null,
            ITalkiPlayerManager talkiPlayerManager = null)
            //IBackgroundAudioPlayer backgroundAudioPlayer = null)
            //IUserDialogs userDialogs = null)
        {
            ShowBackButton = showBackButton;
            //_backgroundAudioPlayer = backgroundAudioPlayer ?? Locator.Current.GetService<IBackgroundAudioPlayer>();
            _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            _gameService = gameService ?? Locator.Current.GetService<IGameService>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            _userSettings = Locator.Current.GetService<IUserSettings>();
            Activator = new ViewModelActivator();
            Navigator = navigator;
            
            SetupCommand();
            SetupRx();
        }

        public override string Title => "Select a game";

        public int IndicatorSize => Device.Idiom == TargetIdiom.Tablet ? 10 : 6;

        [Reactive]
        public List<GameViewModel> Games { get; set; }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; set; }
        
        public ReactiveCommand<Unit, Unit> GuideCommand { get; set; }

        public ViewModelActivator Activator { get; }

        ReactiveCommand<IGame, Unit> SelectCommand { get; set; }

        [Reactive]
        public int CurrentIndex { get; set; }

        [Reactive]
        public int ItemCounts { get; set; }

        [Reactive]
        public bool ShowBackButton { get; set; }

        private void SetupRx()
        {
            MessageBus.Current
                .Listen<SubscriptionChangedMessage>()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => LoadDataCommand.Execute(Unit.Default));
                //.DisposeWith(d);

                MessageBus.Current
                    .Listen<GameRecommendationChangedMessage>()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(x => LoadDataCommand.Execute(Unit.Default));
                //.DisposeWith(d);
            
            this.WhenActivated(d =>
            {
                Bootstrapper.PlayMusic();
                
                // Observable.FromAsync(() => _backgroundAudioPlayer.Play(Constants.ExploreMusic))
                //     .Delay(TimeSpan.FromSeconds(1))
                //     .ObserveOn(RxApp.MainThreadScheduler)
                //     .Do(_ => _backgroundAudioPlayer.ChangeVolume(Constants.SmallMusicVolume))
                //     .SubscribeSafe()
                //     .DisposeWith(d);
                
                _connectivityNotifier.Notifier.RegisterHandler(async context =>
                {
                    await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                    context.SetOutput(true);
                }).DisposeWith(d);

                this.WhenAnyObservable(m => m.LoadDataCommand.IsExecuting)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToPropertyEx(this, m => m.IsLoading)
                    .DisposeWith(d);

                //_talkiPlayerManager.Current?.WhenDisconnectedUponInactivity()
                //    .SubscribeSafe()
                //    .DisposeWith(d);
            });
        }

        
        private void SetupCommand()
        {
             // SelectCommand = ReactiveCommand.CreateFromObservable<IGame, Unit>(m =>
             //      Navigator.PushPage(new GameConfigurationPageViewModel(Navigator, m))
             // );

             SelectCommand = ReactiveCommand.CreateFromTask<IGame, Unit>(async m => {
                     await SimpleNavigationService.PushAsync(new GameConfigurationPageViewModel(Navigator, m));
                     return Unit.Default;
                 }
             );

             
            SelectCommand.ThrownExceptions.SubscribeAndLogException();

            GuideCommand = ReactiveCommand.Create(GuideHelper.StartGuide);
            
            LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!_hasLoadedFirstTime)
                {
                    Dialogs.ShowLoading();
                }
                
                var games = await _gameService.GetGames();//room.TagItems.Select(a => a.ItemId).ToList()

                if (_userSettings.HasTalkiPlayerDevice)
                {
                    _gameMediator.Tags = await _assetRepository.GetTags();
                }

                var recommendedGames = _userSettings.RecommendedGames.DistinctBy(g => g.GameId);

                if (recommendedGames != null)
                {
                    foreach (var game in games)
                    {
                        if (recommendedGames.Any(rg => rg.GameId == game.Id))
                        {
                            game.IsRecommended = true;
                        }
                    }
                }
                

                var repository = Locator.Current.GetService<IChildrenRepository>();
                var children = await repository.GetChildren();
                
                var userHasSubscription = await SubscriptionService.GetUserHasSubscription();
                
                Games = games
                    .OrderByDescending(g => g.IsRecommended)
                    .ThenByDescending(g => g.AccessLevel == GameAccessLevel.Free)
                    .ThenBy(g => g.Name)
                    .Select(game => new GameViewModel(game, SelectCommand, children, userHasSubscription, _userSettings))
                    .ToList();
                ItemCounts = Games.Count;
                
                Dialogs.HideLoading();
                _hasLoadedFirstTime = true;
            });
           
            LoadDataCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();
        }

       
    }
}
