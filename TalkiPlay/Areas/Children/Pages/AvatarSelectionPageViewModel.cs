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
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class AvatarSelectionPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        private IChild _child;
        readonly IUserDialogs _userDialogs;
        private readonly IAssetRepository _assetRepository;
        private readonly IChildrenRepository _childrenService;
        private readonly IConnectivityNotifier _connectivityNotifier;
        private readonly bool _isEdit;
        private readonly SourceList<AvatarItemViewModel> _avatars = new SourceList<AvatarItemViewModel>();
        private readonly ObservableDynamicDataRangeCollection<AvatarItemViewModel> _avatarList = new ObservableDynamicDataRangeCollection<AvatarItemViewModel>();

        public AvatarSelectionPageViewModel(
            IChild child = null,
             IUserDialogs userDialogs = null,
            IConnectivityNotifier connectivityNotifier = null,
            IChildrenRepository childrenService = null,
            IAssetRepository assetRepository = null)
        {
            _child = child;
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();

            _assetRepository = assetRepository ?? Locator.Current.GetService<IAssetRepository>();
            _childrenService = childrenService ?? Locator.Current.GetService<IChildrenRepository>();
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            Activator = new ViewModelActivator();
            _isEdit = _child?.Id != null && _child.Id > 0;
            ButtonText = _isEdit ? "Save" : "Add child";
            SetupRx();
            SetupCommands();
        }

        public override string Title => _isEdit ? "Update avatar" : "Choose an avatar";

        public ViewModelActivator Activator { get; }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; protected set; }

        //public new ReactiveCommand<Unit, Unit> BackCommand { get; protected set; }
        
        public ReactiveCommand<AvatarItemViewModel, Unit> SelectCommand { get; protected set; }
        
        [Reactive]
        public string ButtonText { get; set; }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }

        public IObservableCollection<AvatarItemViewModel> Avatars => _avatarList;


        [Reactive]
        public IAsset SelectedAvatar { get; set; }

        [Reactive]
        public AvatarItemViewModel SelectedItem { get; set; }

        void SetupRx()
        {
            this.WhenActivated(d =>
            {
                _avatars.Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(_avatarList)
                    .DisposeMany()
                    .Subscribe()
                    .DisposeWith(d);

                _connectivityNotifier.Notifier.RegisterHandler(async context =>
                {
                    await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                    context.SetOutput(true);
                }).DisposeWith(d);
            });
        }

        void SetupCommands()
        {
            LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _userDialogs.ShowLoading("Loading ...");
                var assets = await _assetRepository.GetAssets(AssetType.Image, Category.Avatar);
                
                _userDialogs.HideLoading();

                _avatars.Edit(list =>
                {
                    list.Clear();
                    list.AddRange(assets.Select(a => new AvatarItemViewModel(a, SelectionChanged)
                    {
                        IsSelected = a.Id == _child.AssetId
                    }));

                    //workaround for CollectionView Android bug where last item is larger than the rest
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        list.Add(new AvatarItemViewModel(null, null));
                    }
                    
                    SelectedItem = list.FirstOrDefault(a => a.IsSelected);
                });

                return Unit.Default;

            });
           
            LoadDataCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();


            SelectCommand = ReactiveCommand.CreateFromObservable<AvatarItemViewModel, Unit>(item =>
                {
                    _avatars.Edit(list =>
                    {
                        foreach (var avatarItem in list)
                        {
                            avatarItem.IsSelected = false;
                        }
                    });

                    this.SelectedItem = null;

                    item.IsSelected = true;
                    SelectedAvatar = item.Asset;
                    return Observable.Return(Unit.Default);
                }
            );

            SelectCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeAndLogException();


            SaveCommand = ReactiveCommand.CreateFromTask(async
                item =>
                {
                    
                    var child = new ChildDto()
                    {
                        Id = _child.Id,
                        Name = _child.Name,
                        AssetId = SelectedAvatar?.Id ?? 0,
                        DateOfBirth = _child.DateOfBirth
                    };
                    
                    var userSettings = Locator.Current.GetService<IUserSettings>();
                    IChild savedChild = null;
                    if (!userSettings.IsOnboarded || _isEdit)
                    {
                        Dialogs.ShowLoading();
                        savedChild = await _childrenService.AddOrUpdateChild(child);
                        Dialogs.HideLoading();
                        if (_isEdit)
                        {
                            Dialogs.Toast(Dialogs.BuildSuccessToast("Child's details have been successfully updated."));
                        }
                    }

                    if (userSettings.IsOnboarded)
                    {
                        if(_isEdit)
                        {
                            await SimpleNavigationService.PopModalAsync();
                        }
                        else
                        {
                            var state = new GuideState(GuideStep.CommunicationQuestion)
                            {
                                SelectedChild = child,
                                EnableBackAtStart = false,
                                IsModal = true,
                            };
                            await SimpleNavigationService.PushAsync(new GuideQuestionPageViewModel(state.StartStep, state));
                        }
                    }
                    else
                    {
                        userSettings.CurrentChild = savedChild;
                        await SimpleNavigationService.PushAsync(new DeviceSetupPageViewModel(DeviceSetupSource.Onboard));                     
                    }
                });

            SaveCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();

            BackCommand = ReactiveCommand.Create(() => SimpleNavigationService.PopAsync().Forget());
            BackCommand.ThrownExceptions.SubscribeAndLogException();
        }

        void SelectionChanged(int assetId)
        {
            SelectedItem = Avatars.FirstOrDefault(a => a.Asset.Id == assetId);
        }

    }
}