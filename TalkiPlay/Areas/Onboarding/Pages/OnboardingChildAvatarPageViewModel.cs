using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using Splat;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class OnboardingChildAvatarPageViewModel : SimpleBasePageModel
    {
        readonly QRCodeOnboardingState _state;
        readonly IUserDialogs _userDialogs;
        readonly IAssetRepository _assetRepository;
        readonly IConnectivityNotifier _connectivityNotifier;
        private AvatarItemViewModel _selectedItem;
        
        private IChild _child;
        private readonly bool _isEdit;
        
        public OnboardingChildAvatarPageViewModel(IChild child)
        {
            _child = child;
            _isEdit = _child?.Id != null && _child.Id > 0;
            Title = _isEdit ? "Update avatar" : "Choose an avatar";
            NextButtonText = _isEdit ? "Save" : "Add child";
        }
        public OnboardingChildAvatarPageViewModel(QRCodeOnboardingStep currentStep, QRCodeOnboardingState state)
        {
            IsOnboardingMode = true;
            NextButtonText = "Let's Play";
            
            _state = state;
            HeaderText = string.Format(QRCodeOnboardingHelper.GetHeaderTextForStep(currentStep), _state.Child.Name);

            _userDialogs = Locator.Current.GetService<IUserDialogs>();

            _assetRepository = Locator.Current.GetService<IAssetRepository>();


            Avatars = new ObservableDynamicDataRangeCollection<AvatarItemViewModel>();

            _connectivityNotifier = Locator.Current.GetService<IConnectivityNotifier>();
            _connectivityNotifier.Notifier.RegisterHandler(async context =>
            {
                await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                context.SetOutput(true);
            });

            SetupCommands();

            LoadData().Forget();
        }

        public bool IsOnboardingMode { get; set; }
        

        public string Title { get; set; }
    
        public bool ShowNavBar => true;

        public string HeaderText { get; }

        public ICommand NextCommand { get; private set; }
        
        public string NextButtonText { get; set; }

        public ICommand BackCommand { get; private set; }


        public ObservableDynamicDataRangeCollection<AvatarItemViewModel> Avatars { get; private set; }

        public AvatarItemViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;

                foreach (var avatar in Avatars)
                {
                    avatar.IsSelected = false;
                }

                _selectedItem.IsSelected = true;
            }
        }

        void SetupCommands()
        {
            NextCommand = new Command(async() =>
            {
                _userDialogs.ShowLoading("Adding child ...");

                _state.Child.AssetId = SelectedItem.Asset.Id;

                var childrenRepo = Locator.Current.GetService<IChildrenRepository>();
                await childrenRepo.AddChild(_state.Child);

                _userDialogs.HideLoading();

                Locator.Current.GetService<IUserSettings>().IsQrOnboarded = true;
                var navigatorHelper = Locator.Current.GetService<ITalkiPlayNavigator>();
                navigatorHelper.NavigateToTabbedPage(TabItemType.Items);
            });


            BackCommand = new Command(() =>
            {
                SimpleNavigationService.PopAsync().Forget();
            });

        }

        async Task LoadData()
        {
            Dialogs.ShowLoading("Loading ...");

            try
            {
                var assets = await _assetRepository.GetAssets(AssetType.Image, Category.Avatar);
            
                Dialogs.HideLoading();

                Avatars.Clear();
                using (Avatars.SuspendNotifications())
                {
                    Avatars.AddRange(assets.Select(a => new AvatarItemViewModel(a, SelectionChanged)));
                }

                if (Avatars.Count > 0)
                {
                    SelectedItem = Avatars.First();
                    SelectedItem.IsSelected = true;
                    
                    //workaround for CollectionView Android bug where last item is larger than the rest
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        Avatars.Add(new AvatarItemViewModel(null, null));
                    }
                }
            }
            catch (Exception e)
            {
                Dialogs.HideLoading();
                e.ShowExceptionDialog();
            }
        }

        void SelectionChanged(int assetId)
        {
            SelectedItem = Avatars.FirstOrDefault(a => a.Asset.Id == assetId);
        }
    }
}