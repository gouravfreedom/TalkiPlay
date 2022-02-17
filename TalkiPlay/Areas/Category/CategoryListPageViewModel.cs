using Acr.UserDialogs;
using ChilliSource.Mobile.Api;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TalkiPlay.Shared;
using RssFeedInfo = TalkiPlay.Services.Utility.RssImageFetcher.RssFeedInfo;

namespace TalkiPlay.Shared
{
    public class CategoryListPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        private readonly IApi<ITalkiPlayApi> _api;
        private readonly IUserSettings _userSettings;
        private readonly IGameMediator _gameMediator;
        private readonly IConnectivityNotifier _connectivityNotifier;
        private readonly IGameService _gameService;
        private readonly IAssetRepository _assetRepository;

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public CategoryListPageViewModel(INavigationService navigator = null,
            IGameService gameService = null,
            IConnectivityNotifier connectivityNotifier = null,
            IGameMediator gameMediator = null,
            IUserSettings userSettings = null)
        {
            Navigator = navigator;
            _api = Locator.Current.GetService<IApi<ITalkiPlayApi>>();

            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();

            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            _gameService = gameService ?? Locator.Current.GetService<IGameService>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();

            SetupRX();
            SetupCommands();
        }

        public ICommand SelectChildCommand { get; private set; }

        [Reactive] public bool IsRssLoading { get; private set; } = true;        
        public ObservableCollection<RssFeedInfo> RssImages { get; } = new ObservableCollection<RssFeedInfo>();

        [Reactive]
        public bool IsCategoriesLoading { get; private set; } = true;
        public ObservableCollection<CategoryDto> Categories { get; } = new ObservableCollection<CategoryDto>();

        public ReactiveCommand<Unit, Unit> LoadRssCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LoadCategoriesCommand { get; private set; }
        public ReactiveCommand<IGame, Unit> SelectGameCommand { get; private set; }

        public override string Title => "Game Categories";

        [Reactive]
        public IChild ActiveChild { get; private set; }

        private void SetupRX()
        {
            this.WhenActivated(d =>
            {
                _connectivityNotifier.Notifier.RegisterHandler(async context =>
                {
                    await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                    context.SetOutput(true);
                }).DisposeWith(d);
            });
        }

        private void SetupCommands()
        {

            SelectChildCommand = ReactiveCommand.CreateFromTask(() =>
            {
                SimpleNavigationService.PushAsync(new ChildListPageViewModel(false)).Forget();
                return Task.CompletedTask;
            });

            LoadRssCommand = ReactiveCommand.CreateFromTask(() =>
            {
                IsRssLoading = true;
                RssImages.Clear();
                Services.Utility.RssImageFetcher.FetchDeviceImages((feeds) =>
                {
                    feeds.ForEach((item) => RssImages.Add(item));
                }, () =>
                {
                    if (IsRssLoading)
                    {
                        IsRssLoading = false;
                        this.RaisePropertyChanged(nameof(IsRssLoading));
                    }
                });
                return Task.CompletedTask;
            });

            LoadRssCommand.ThrownExceptions
                 .ObserveOn(RxApp.MainThreadScheduler)
                 .ShowExceptionDialog()
                 .SubscribeAndLogException();

            LoadRssCommand.Execute(Unit.Default);

            LoadCategoriesCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var categories = await _api.Client.GetCategories();
                var games = await _api.Client.GetGames();
                Categories.Clear();

                categories.ToObservable().Do((m) => {                    
                    m.Games.AddRange(games.Where(g => g.Categories.Contains(m.Id)));
                    if (m.Games.Count > 0)
                    {
                        Categories.Add(m);
                    }
                }).Subscribe();
                IsCategoriesLoading = false;
                this.RaisePropertyChanged(nameof(IsCategoriesLoading));
            });

            LoadCategoriesCommand.ThrownExceptions
                 .ObserveOn(RxApp.MainThreadScheduler)
                 .ShowExceptionDialog()
                 .SubscribeAndLogException();

            LoadCategoriesCommand.Execute(Unit.Default);

            SelectGameCommand = ReactiveCommand.CreateFromTask<IGame, Unit>(async m => {
                //Dialogs.ShowLoading();                
                //Dialogs.HideLoading();
                await SimpleNavigationService.PushAsync(new GameConfigurationPageViewModel(Navigator, m));
                return Unit.Default;
            });
        }

        public void LoadDataOnAppear()
        {
            ActiveChild = Locator.Current.GetService<IChildrenRepository>().ActiveChild;
            this.RaisePropertyChanged(nameof(ActiveChild));
        }
    }
}
