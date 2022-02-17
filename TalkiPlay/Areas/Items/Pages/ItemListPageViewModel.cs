using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Acr.UserDialogs;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.Core;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using FormsControls.Base;
using ReactiveUI;
using Splat;
using Unit = System.Reactive.Unit;

namespace TalkiPlay.Shared
{
    public class ItemListPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        private readonly ITalkiPlayerManager _talkiPlayerManager;

        private readonly IGameMediator _gameMediator;
        private readonly IUserDialogs _userDialogs;
        private readonly IConnectivityNotifier _connectivityNotifier;
        private readonly IGameService _gameService;
        protected readonly IAssetRepository _assetRepository;
        private readonly SourceList<ItemSelectionViewModel> _items = new SourceList<ItemSelectionViewModel>();

        public ItemListPageViewModel(
            INavigationService navigator, 
            IGameService gameService = null,
            IConnectivityNotifier connectivityNotifier = null,
            IUserDialogs userDialogs = null,
            IGameMediator gameMediator = null,
            ITalkiPlayerManager talkiPlayerManager = null)
        {
            _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            _gameService = gameService ?? Locator.Current.GetService<IGameService>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            Activator = new ViewModelActivator();
            Navigator = navigator;
            SetupRx();
            SetupCommands();
        }

        public override string Title => "Collection";
        public ViewModelActivator Activator { get; }

        public ReadOnlyObservableCollection<ItemSelectionViewModel> Items { get; private set; }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }

        public ReactiveCommand<ItemSelectionViewModel, Unit> SelectCommand { get; private set; }

        private void SetupRx()
          {
              
             this.WhenActivated(d =>
             {
                 //_talkiPlayerManager.Current?.WhenDisconnectedUponInactivity()
                 //    .SubscribeSafe()
                 //    .DisposeWith(d);

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

         private void SetupCommands()
         {
             LoadDataCommand = ReactiveCommand.CreateFromObservable(() =>
                 ObservableOperatorExtensions.StartShowLoading("Loading ...")
                     .ObserveOn(RxApp.TaskpoolScheduler)
                     .SelectMany(m => 
                        {
                            if (_gameMediator.CurrentRoom?.Packs == null)
                            {
                                return null;
                            }
                            return _assetRepository.GetPacks(_gameMediator.CurrentRoom?.Packs?.ToArray());
                        })
                     .ObserveOn(RxApp.MainThreadScheduler)
                     .HideLoading()
                     .Do(packs =>
                     {
                         var itemList = packs.SelectMany(a => a.Items).DistinctBy(a => a.Id)
                             .OrderBy(a => a.Type.GetData<int>("SortOrder")).ToList();
                        _items.Edit(items =>
                        {
                            items.Clear();
                            items.AddRange(itemList.Select(a => new ItemSelectionViewModel
                            {    
                                Label = a.Name,
                                Source = a
                                
                            }).ToList());
                        });
                     })
                     .Select(_ => Unit.Default)
             );

             LoadDataCommand.ThrownExceptions
                 .ObserveOn(RxApp.MainThreadScheduler)
                 .HideLoading()
                 .ShowExceptionDialog()
                 .SubscribeAndLogException();

             SelectCommand = ReactiveCommand.CreateFromTask<ItemSelectionViewModel, Unit>( async (m) =>
                 {
                     var item = m.Source as ItemDto;
                     var items = new List<ItemDto>() {item};
                     var pack = new PackDto()
                     {
                         Id = item?.GroupId ?? 0
                     };
                     var selector = new TagItemsSelector(_gameMediator.CurrentRoom, items, pack);
                     await SimpleNavigationService.PushAsync(new ItemsTagItemSetupPageViewModel(Navigator, selector, item, true));
                     return Unit.Default;
                 }
             );

             SelectCommand.ThrownExceptions.SubscribeAndLogException();

             BackCommand = ReactiveCommand.CreateFromTask(() =>  SimpleNavigationService.PushAsync(new ItemsRoomListPageViewModel(Navigator)));
             BackCommand.ThrownExceptions.SubscribeAndLogException();
         }


        
        
        
    }
}