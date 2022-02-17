using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.Core;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Unit = System.Reactive.Unit;


namespace TalkiPlay.Shared
{
    public class GameActiveItemsConfigurationPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        
         readonly IGameMediator _gameMediator;
        readonly IUserDialogs _userDialogs;
        readonly IUserSettings _userSettings;
        readonly IConnectivityNotifier _connectivityNotifier;
        //readonly IGameService _gameService;
        readonly IAssetRepository _assetRepository;
        readonly SourceList<ItemConfigurationViewModel> _items = new SourceList<ItemConfigurationViewModel>();

        public GameActiveItemsConfigurationPageViewModel(
            INavigationService navigator,
            IAssetRepository assetRepository = null,
            //IGameService gameService = null,
            IConnectivityNotifier connectivityNotifier = null,
            IUserDialogs userDialogs = null,
            IUserSettings userSettings = null,
            IGameMediator gameMediator = null)
        {
            _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            _assetRepository = assetRepository ?? Locator.Current.GetService<IAssetRepository>();
            //_gameService = gameService ?? Locator.Current.GetService<IGameService>();
            Activator = new ViewModelActivator();
            Navigator = navigator;
            SetupRx();
            SetupCommands();
        }

        public override string Title => "Collection";
        public ViewModelActivator Activator { get; }

        public ReadOnlyObservableCollection<ItemConfigurationViewModel> Items { get; private set; }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }

        [Reactive] public bool ShowLeftMenuItem { get; set; } = false;

        void SetupRx()
        {
            this.WhenActivated(d =>
            {
                _items.Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out var items)
                    .DisposeMany()
                    .Subscribe()
                    .DisposeWith(d);

                Items = items;
             
                _connectivityNotifier.Notifier.RegisterHandler(async context =>
                {
                    await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                    context.SetOutput(true);
                }).DisposeWith(d);
            });
        }

         void SetupCommands()
         {

            LoadDataCommand = ReactiveCommand.Create(() =>
            {
                _userDialogs.ShowLoading("Loading ...");
                var pack = _gameMediator.CurrentPack;// await _gameService.GetPack(_gameMediator.CurrentPack.Id);
                _userDialogs.HideLoading();
                var packItems = pack.Items
                    .Where(i => i.Id > 0 && !string.IsNullOrEmpty(i.Name))
                    .DistinctBy(i => i.Id)
                    .Where(i => i.Type != ItemType.Home)
                    .ToList();

                if (_userSettings.HasTalkiPlayerDevice)
                {

                    var tagItemIds = _gameMediator.Tags.SelectMany(a => a.ItemIds).ToList();
                    var roomTagItemIds = _gameMediator.CurrentRoom.TagItems.SelectMany(a => a.ItemIds).ToList();
                    _items.Edit(items =>
                    {
                        items.Clear();
                        items.AddRange(packItems.Where(a => tagItemIds.Contains(a.Id))
                            .Select(a => new ItemConfigurationViewModel(a, _userSettings.HasTalkiPlayerDevice)
                        {
                            IsEnabled = roomTagItemIds.Contains(a.Id)
                        }).ToList());
                    });
                }
                else
                {
                    var itemsSettings = _userSettings.ItemsSettings;
                    //var itemsSettings = await _assetRepository.GetItemsSettings();

                    _items.Edit(items =>
                    {
                        items.Clear();

                        if (_gameMediator.CurrentGame.Type == GameType.Explore)
                        {
                            foreach (var item in packItems)
                            {
                                items.Add(new ItemConfigurationViewModel(item,
                                        _userSettings.HasTalkiPlayerDevice, itemsSettings));
                            }
                        }
                        else
                        {
                            var instructions = _gameMediator.CurrentGame.Instructions;
                            foreach (var instruction in instructions)
                            {
                                items.Add(new ItemConfigurationViewModel(instruction.Item,
                                    _userSettings.HasTalkiPlayerDevice, itemsSettings));
                            }
                        }
                    });
                }

                return Unit.Default;
                //return Task.FromResult(Unit.Default);
            });
            
             LoadDataCommand.ThrownExceptions
                 .ObserveOn(RxApp.MainThreadScheduler)
                 .HideLoading()
                 .ShowExceptionDialog()
                 .SubscribeAndLogException();

           
             BackCommand = ReactiveCommand.CreateFromTask(async() => await SimpleNavigationService.PopAsync());
             BackCommand.ThrownExceptions.SubscribeAndLogException();
         }
    }
}