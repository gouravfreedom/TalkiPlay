using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;

namespace TalkiPlay.Shared
{
    public class ItemsRoomListPageViewModel : RoomListPageViewModel
    {
        public ItemsRoomListPageViewModel(INavigationService navigator) : base(navigator)
        {
            this.SetupCommands();
        }
      
        void SetupCommands()
        {
            SelectCommand = ReactiveCommand.CreateFromTask<RoomViewModel, Unit>(async (roomVM) =>
            {
                _userDialogs.ShowLoading("Loading ...");
                var packs = await _assetRepository.GetPacks();
                var room = await _assetRepository.GetRoom(roomVM.Room.Id);
                var items = await _assetRepository.GetItems(ItemType.Home);
                _userDialogs.HideLoading();

                var homeTagItems = items.Select(a => a.Id);
                _gameMediator.CurrentRoom = room;
                _gameMediator.CurrentPack = packs.FirstOrDefault();
                var itemGroupIds = packs.Select(a => a.Id);

                bool roomHasNonHomeTagItems = room.TagItems.Any(a => !homeTagItems.Any(hti => a.ItemIds.Contains(hti)));

                if (_talkiPlayerManager.IsTalkiPlayerReady)
                {
                    // if all item group went through the onboarding 
                    if (room.Packs.Count == itemGroupIds.Count() && roomHasNonHomeTagItems)
                    {
                        await SimpleNavigationService.PushAsync(new ItemListPageViewModel(Navigator));
                    }

                    if (packs.Count > 1)
                    {
                        await SimpleNavigationService.PushModalAsync(new NavigationItemViewModel<ItemsPackListPageViewModel>());
                    }
                    else
                    {
                        await SimpleNavigationService.PushModalAsync(new NavigationItemViewModel<ItemsTagItemStartPageViewModel>());
                    }
                    return Unit.Default;
                }

                ReactiveCommand<INavigationService, Unit> navigationCommand;

                if (room.Packs.Count == itemGroupIds.Count() && roomHasNonHomeTagItems)
                {
                    navigationCommand =
                        ReactiveCommand.CreateFromObservable<INavigationService, Unit>(nav =>
                            nav.PushPage(new ItemListPageViewModel(Navigator)));
                }
                else
                {
                    navigationCommand = packs.Count > 1 ? ReactiveCommand.CreateFromObservable<INavigationService, Unit>(nav => nav.PushModal(new NavigationItemViewModel<ItemsPackListPageViewModel>()))
                        : ReactiveCommand.CreateFromObservable<INavigationService, Unit>(nav => nav.PushModal(new NavigationItemViewModel<ItemsTagItemStartPageViewModel>()));
                }

                await SimpleNavigationService.PushAsync(new DeviceListPageViewModel(Navigator, navigationCommand,
                    backCommand: ReactiveCommand.CreateFromTask<INavigationService, Unit>(async nav =>
                    {
                        await SimpleNavigationService.PopAsync();
                        return Unit.Default;
                    })));
                return Unit.Default;
            });

            //SelectCommand = ReactiveCommand.CreateFromObservable<RoomViewModel, Unit>(m =>
            //       ObservableOperatorExtensions.StartShowLoading("Loading ...")
            //        .ObserveOn(RxApp.TaskpoolScheduler)
            //        .SelectMany(_ => _gameService.GetRoom(m.Room.Id))
            //        .SelectMany(room => _gameService.GetItems(ItemType.Home).Select(items => (Items : items , Room: room)))
            //        .ObserveOn(RxApp.MainThreadScheduler)
            //        .HideLoading()
            //        .SelectMany(result =>
            //        {
            //            var room = result.Room;
            //            var homeTagItems = result.Items.Select(a => a.Id);
            //            _gameMediator.CurrentRoom = room;
            //            _gameMediator.CurrentPack = Packs.FirstOrDefault();
            //            var itemGroupIds = Packs.Select(a => a.Id);

            //            bool roomHasNonHomeTagItems = room.TagItems.Any(a => !homeTagItems.Any(hti => a.ItemIds.Contains(hti)));

            //            if (_talkiPlayerManager.IsTalkiPlayerReady)
            //            {
            //                // if all item group went through the onboarding 
            //                if (room.Packs.Count == itemGroupIds.Count() && roomHasNonHomeTagItems)
            //                {
            //                    return Navigator.PushPage(new ItemListPageViewModel(Navigator));
            //                }

            //                return Packs.Count > 1
            //                    ? Navigator.PushModal(new NavigationItemViewModel<ItemsPackListPageViewModel>())
            //                    : Navigator.PushModal(new NavigationItemViewModel<ItemsTagItemStartPageViewModel>());
            //            }

            //            ReactiveCommand<INavigationService, Unit> navigationCommand;

            //            if (room.Packs.Count == itemGroupIds.Count() && roomHasNonHomeTagItems)
            //            {
            //                navigationCommand =
            //                    ReactiveCommand.CreateFromObservable<INavigationService, Unit>(nav =>
            //                        nav.PushPage(new ItemListPageViewModel(Navigator)));
            //            }
            //            else
            //            {
            //                navigationCommand = Packs.Count > 1 ? ReactiveCommand.CreateFromObservable<INavigationService, Unit>(nav => nav.PushModal(new NavigationItemViewModel<ItemsPackListPageViewModel>())) 
            //                    : ReactiveCommand.CreateFromObservable<INavigationService, Unit>(nav => nav.PushModal(new NavigationItemViewModel<ItemsTagItemStartPageViewModel>()));
            //            }

            //            return Navigator.PushPage(new DeviceListPageViewModel(
            //                Navigator,
            //                navigationCommand,
            //                backCommand: ReactiveCommand.CreateFromObservable<INavigationService, Unit>(nav => nav.PopPage())
            //            ));

            //        })
            //);

            SelectCommand.ThrownExceptions
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();
        }
    }
}