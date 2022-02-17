using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace TalkiPlay.Shared
{
    public class RoomImageSelectionPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        private IRoom _room;
        //private readonly IAssetService _assetService;
        private readonly IAssetRepository _assetRepository;
        //private readonly IGameService _gameService;
        private readonly IConnectivityNotifier _connectivityNotifier;
        private readonly bool _isEdit;
        readonly IUserDialogs _userDialogs;
        private readonly SourceList<RoomImageItemViewModel> _images = new SourceList<RoomImageItemViewModel>();
        readonly ObservableDynamicDataRangeCollection<RoomImageItemViewModel> _imageList = new ObservableDynamicDataRangeCollection<RoomImageItemViewModel>();

        public RoomImageSelectionPageViewModel(
            INavigationService navigator,
            IRoom room = null,
            IUserDialogs userDialogs = null,
            IConnectivityNotifier connectivityNotifier = null)
            //IGameService gameService = null,
            //IAssetService assetService = null
            //)
        {
            _room = room;
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();

            //_assetService = assetService ?? Locator.Current.GetService<IAssetService>();
            //_gameService = gameService ?? Locator.Current.GetService<IGameService>();
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            
            Activator = new ViewModelActivator();
            Navigator = navigator;
            _isEdit = _room?.Id != null && _room.Id > 0;
            ButtonText = _isEdit ? "Save" : "Add a room";
            SetupRx();
            SetupCommands();
        }

        public override string Title => _isEdit ? "Update image" : "Choose an image";

        public ViewModelActivator Activator { get; }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; protected set; }

        public ReactiveCommand<RoomImageItemViewModel, Unit> SelectCommand { get; protected set; }

        public extern bool IsBusy { [ObservableAsProperty] get; }

        [Reactive]
        public string ButtonText { get; set; }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }

        public IObservableCollection<RoomImageItemViewModel> Images => _imageList;

        [Reactive]
        public IAsset SelectedImage { get; set; }

        [Reactive]
        public RoomImageItemViewModel SelectedItem { get; set; }

        void SetupRx()
        {
            this.WhenActivated(d =>
            {
                _images.Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(_imageList)
                    .DisposeMany()
                    .Subscribe()
                    .DisposeWith(d);
                
                _connectivityNotifier.Notifier.RegisterHandler(async context =>
                {
                    await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                    context.SetOutput(true);
                }).DisposeWith(d);
                
                this.WhenAnyObservable(m => m.SaveCommand.IsExecuting)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(m => m)
                    .ToPropertyEx(this, v => v.IsBusy)
                    .DisposeWith(d);
            });
        }
                
        void SetupCommands()
        {
            LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _userDialogs.ShowLoading("Loading ...");
                var assets = await _assetRepository.GetAssets(AssetType.Image, Category.Room);

                //reload room with all properties, including assetId
                if (_room.Id > 0)
                {
                    _room = await _assetRepository.GetRoom(_room.Id);
                }

                _userDialogs.HideLoading();

                _images.Edit(list =>
                {
                    list.Clear();

                    foreach (var asset in assets)
                    {
                        var vm = new RoomImageItemViewModel(asset)
                        {
                            IsSelected = asset.Id == _room.AssetId
                        };
                        list.Add(vm);
                    }

                    //list.AddRange(assets.Select(a => new RoomImageItemViewModel(a)
                    //{
                    //    IsSelected = a.Id == _room.AssetId
                    //}));

                    SelectedItem = list.FirstOrDefault(a => a.IsSelected);
                });

                return Unit.Default;

            });

            //LoadDataCommand = ReactiveCommand.CreateFromObservable(() =>
            //{
            //    return ObservableOperatorExtensions.StartShowLoading("Loading ...")
            //        .ObserveOn(RxApp.TaskpoolScheduler)
            //        .SelectMany(_ => _assetService.GetAssets(AssetType.Image, Category.Room))
            //            .ObserveOn(RxApp.MainThreadScheduler)
            //            .HideLoading()
            //            .Do(m =>
            //            {
            //                _images.Edit(list =>
            //                {
            //                    list.Clear();
            //                    list.AddRange(m.Select(a => new RoomImageItemViewModel(a)
            //                    {
            //                        IsSelected =  a.Id == _room.AssetId
            //                    }));

            //                    SelectedItem = list.FirstOrDefault(a => a.IsSelected);
            //                });
            //            })
            //            .Select(_ => Unit.Default);
            //}); 
            
            LoadDataCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();

            
            SelectCommand = ReactiveCommand.CreateFromObservable<RoomImageItemViewModel, Unit>(item =>
                {
                    _images.Edit(list =>
                    {
                        foreach (var avatarItem in list)
                        {
                            avatarItem.IsSelected = false;
                        }
                    });

                    this.SelectedItem = null;
                    
                    item.IsSelected = true;
                    SelectedImage = item.Asset;
                    return Observable.Return(Unit.Default);
                }
            );

            SelectCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeAndLogException();

            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _userDialogs.ShowLoading("Saving ...");

                var room = new RoomDto()
                {
                    Id = _room.Id,
                    Name = _room.Name,
                    AssetId = SelectedImage?.Id ?? 0,
                };

                await _assetRepository.AddOrUpdateRoom(room);

                _userDialogs.HideLoading();
                _userDialogs.Toast(_isEdit ? "Room details have been successfully updated." : "Room has been added successfully.");

                await SimpleNavigationService.PopModalAsync();
            });

            //SaveCommand = ReactiveCommand.CreateFromObservable<Unit, Unit>(
            //    item =>
            //    {
            //        return ObservableOperatorExtensions.StartShowLoading("Saving ...")
            //            .ObserveOn(RxApp.TaskpoolScheduler)
            //            .SelectMany(_ =>
            //            {
            //                var child = new RoomDto()
            //                {
            //                    Id = _room.Id,
            //                    Name = _room.Name,                                
            //                    AssetId = SelectedImage?.Id ?? 0,                                
            //                };
            //                return _gameService.AddOrUpdateRoom(child);
            //            })
            //            .ObserveOn(RxApp.MainThreadScheduler)
            //            .HideLoading()
            //            .ShowSuccessToast(_isEdit ? "Room details have been successfully updated." : "Room has been added successfully.")
            //            .SelectMany(m => Navigator.PopModal())
            //            .Select(_ => Unit.Default);
            //    });

            SaveCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();
         
            BackCommand = ReactiveCommand.CreateFromTask(() =>  SimpleNavigationService.PopAsync());
            BackCommand.ThrownExceptions.SubscribeAndLogException();
            
        }

       
        
    }
}