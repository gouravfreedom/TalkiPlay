﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Acr.UserDialogs;
using ChilliSource.Mobile.Core;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using ReactiveUI;
using Splat;
using Unit = System.Reactive.Unit;

namespace TalkiPlay.Shared
{
    public interface IPackPageViewModel : IBasePageViewModel, IActivatableViewModel
    {
        ReadOnlyObservableCollection<ItemSelectionViewModel> ItemGroups { get; }

        ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

        ReactiveCommand<ItemSelectionViewModel, Unit> SelectCommand { get; }
    }

    public class PackListPageViewModel : BasePageViewModel, IPackPageViewModel, IModalViewModel
    {
        protected readonly IGameMediator _gameMediator;
        protected readonly IUserDialogs _userDialogs;
        protected readonly IConnectivityNotifier _connectivityNotifier;
        protected readonly IGameService _gameService;
        protected readonly IAssetRepository _assetRepository;
        protected readonly SourceList<ItemSelectionViewModel> _itemGroups = new SourceList<ItemSelectionViewModel>();

        
         public PackListPageViewModel(INavigationService navigator,
            IGameService gameService = null,
            IConnectivityNotifier connectivityNotifier = null,
            IUserDialogs userDialogs = null,
            IGameMediator gameMediator = null)
         {
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

         private void SetupRx()
         {
             this.WhenActivated(d =>
             {
                 _itemGroups.Connect()
                     .ObserveOn(RxApp.MainThreadScheduler)
                     .Bind(out var itemGroupList)
                     .DisposeMany()
                     .Subscribe()
                     .DisposeWith(d);

                 ItemGroups = itemGroupList;
             
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
                     .SelectMany(_ => _assetRepository.GetPacks())
                     .ObserveOn(RxApp.MainThreadScheduler)
                     .HideLoading()
                     .Do(m =>
                     {
                        _itemGroups.Edit(items =>
                        {
                            items.Clear();
                            items.AddRange(m.Select(a => new ItemSelectionViewModel
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

            //Note: SelectCommand and BackCommand are now set and used only by ItemsPackListPageViewModel

             //SelectCommand = ReactiveCommand.CreateFromObservable<ItemSelectionViewModel, Unit>( m =>
             //    {
             //        var room = _gameMediator.CurrentRoom;
             //        var homeTagItems = _gameMediator.HomeTags.Select(a => a.Id);
             //        _gameMediator.CurrentPack = m.Source as IPack;

             //        return _gameService.GetPack(_gameMediator.CurrentPack.Id)
             //           .ObserveOn(RxApp.MainThreadScheduler)
             //          .SelectMany(pack =>
             //           {                           
             //              var items = pack.Items;
             //              var itemIds = items.Where(a => a.Type == ItemType.Default).Select(a => a.Id);
             //              var hasTags = room?.TagItems.Any(a => !homeTagItems.Contains(a.ItemId)) ?? false;
             //              var atLeastOneItemTag = room?.TagItems.Any(a => itemIds.Contains(a.ItemId)) ?? false;
                           
             //              if (hasTags && atLeastOneItemTag)
             //              {
             //                   //TODO: check potential bug here. navigation occasionally stops opening GameListPageViewModel and requires app restart
             //                   var navigator = Locator.Current.GetService<INavigationService>(TabItemType.Games.ToString());
             //                  navigator.PopToRootPage().SelectMany(_ => navigator.PushPage(new GameListPageViewModel(navigator))).SubscribeSafe();
             //                  return Navigator.PopModal();
             //              }

             //              return Navigator.PushPage(new TagItemStartPageViewModel(Navigator));

             //           });
             //    }
             //);

             //SelectCommand.ThrownExceptions.SubscribeAndLogException();
             
             //BackCommand = ReactiveCommand.CreateFromObservable(() =>
             //{
             //    var navigator = Locator.Current.GetService<INavigationService>(TabItemType.Games.ToString());
             //    navigator.PopToRootPage().SubscribeSafe();
             //    return Navigator.PopModal();
                 
             //});
             //BackCommand.ThrownExceptions.SubscribeAndLogException();
         }


         public override string Title => "Select a game pack";
        public ViewModelActivator Activator { get; }
        public ReadOnlyObservableCollection<ItemSelectionViewModel> ItemGroups { get; protected set; }
        
        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; protected set; }
        
        public ReactiveCommand<ItemSelectionViewModel, Unit> SelectCommand { get; protected set; }
   
    }
}
