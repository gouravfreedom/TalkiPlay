using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using Splat;

namespace TalkiPlay.Shared
{
    public class GameRoomListPageViewModel : RoomListPageViewModel
    {
        //private readonly IAssetRepository _assetRepository;
        public GameRoomListPageViewModel(INavigationService navigator) : base(navigator, reset: false)
        {
            //_assetRepository = Locator.Current.GetService<IAssetRepository>();
            this.SetupCommands();
        }
        
        private void SetupCommands()
        {

            LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _userDialogs.ShowLoading("Loading ...");
                Packs = await _assetRepository.GetPacks();
                var rooms = await _assetRepository.GetRooms();
                var size = _applicationService.ScreenSize;
                _userDialogs.HideLoading();
                var width = size.Width;

                Rooms.Clear();
                using (Rooms.SuspendNotifications())
                {
                    Rooms.AddRange(rooms.Where(a => a.TagsCount > 0).Select(a => new RoomViewModel
                    {
                        Room = a,
                        HeroTitle = a.Name,
                        BackgroundImage = a.ImagePath.ToResizedImage(width - 40)
                    }));
                }

                //_rooms.Edit(items =>
                //{
                //    items.Clear();
                //    items.AddRange(rooms.Where(a => a.TagsCount > 0).Select(a => new RoomViewModel
                //    {
                //        Room = a,
                //        HeroTitle = a.Name,
                //        BackgroundImage = a.ImagePath.ToResizedImage(width - 40)
                //    }).ToList());
                //});

            });

            //LoadDataCommand = ReactiveCommand.CreateFromObservable(() =>
            //    ObservableOperatorExtensions.StartShowLoading("Loading ...")
            //        .ObserveOn(RxApp.TaskpoolScheduler)
            //        .SelectMany(_ => _gameService.GetPacks())
            //        .Do(m => Packs = m)
            //        .SelectMany(_ => _gameService.GetRooms())
            //        .ObserveOn(RxApp.MainThreadScheduler)
            //        .HideLoading()
            //        .Select(m =>
            //        {
            //            var size = _applicationService.ScreenSize;

            //            return (Width: size.Width, Rooms: m);
            //        })
            //        .ObserveOn(RxApp.TaskpoolScheduler)
            //        .Do(m =>
            //        {
            //            var width = m.Width;
            //            var rooms = m.Rooms;
            //            _rooms.Edit(items =>
            //            {
            //                items.Clear();
            //                items.AddRange(rooms.Where(a => a.TagsCount > 0).Select(a => new RoomViewModel
            //                {    
            //                    Room = a,
            //                    HeroTitle = a.Name,
            //                    BackgroundImage =  a.ImagePath.ToResizedImage(width - 40)
            //                }).ToList());
            //            }); 
            //        })
            //        .Select(_ => Unit.Default)
            //);

            LoadDataCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();
            
            
            SelectCommand = ReactiveCommand.CreateFromObservable<RoomViewModel, Unit>(m =>
                ObservableOperatorExtensions.StartShowLoading("Loading ...")
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .SelectMany(_ => _assetRepository.GetRoom(m.Room.Id))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .HideLoading()
                    .SelectMany(room =>
                    {
                        _gameMediator.CurrentRoom = room;
                        return Observable.FromAsync(() => SimpleNavigationService.PopAsync());
                    })
                    .Select(_ => Unit.Default)
            );

            SelectCommand.ThrownExceptions
                .HideLoading()
                .SubscribeAndLogException();
        }
    }
}