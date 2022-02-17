using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using FormsControls.Base;
using Lottie.Forms;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class GameConfigurationPage : BasePage<GameConfigurationPageViewModel>, IAnimationPage
    {
        readonly TapGestureRecognizer _removeAllGesture;
     
        public GameConfigurationPage()
        {
            InitializeComponent();

            this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

            var service = Locator.Current.GetService<IApplicationService>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = Device.RuntimePlatform == Device.iOS ? (int) service.StatusbarHeight : 0;
                var navHeight = (int) service.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = totalHeight;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);

            });
                        
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);

            BtnConfigGame.IsVisible = Locator.Current.GetService<IUserSettings>().HasTalkiPlayerDevice;
            PlayerList.ItemTemplate = new PlayerViewTemplateSelector();

            this.WhenActivated(d =>
            {
                SyncItemsHeight();
                this.WhenAnyValue(m => m.ViewModel.LoadDataCommand)
                    .Select(m => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.LoadDataCommand)
                    .DisposeWith(d);
                
                this.OneWayBind(ViewModel, v => v.Players, view => view.PlayerList.ItemsSource).DisposeWith(d);
                //this.OneWayBind(ViewModel, v => v.Items, view => view.ItemsList.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.GameTitle, view => view.TitleLabel.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.GameShortDescription, view => view.ShortDescription.Text).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.RemoveAllChildrenCommand, view => view._removeAllGesture).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.OnboardGameItemsCommand, view => view.BtnConfigGame).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
               
                this.WhenAnyObservable(v => v.ViewModel.StartGameCommand.CanExecute)
                  .SubscribeSafe(isReady =>
                  {
                      this.StartGameButton.BackgroundColor = isReady ? Colors.Blue5Color : Colors.DisableColor;
                      this.BtnConfigGame.IsEnabled = isReady;
                  })
                  .DisposeWith(d);

                this.WhenAnyValue(m => m.ViewModel.Items.Count)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(m => Unit.Default)
                .SubscribeSafe((m) => SyncItemsHeight())
                .DisposeWith(d);
                 
            });
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
           
            var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
            safeInsets.Top = 0;
            Padding = safeInsets;
        }

        private void SyncItemsHeight()
        {
            var oldSource = ItemsList.ItemsSource as ReadOnlyObservableCollection<ItemConfigurationViewModel>;
            if (oldSource == null || ViewModel.Items.Count != oldSource.Count)
            {
                ItemsList.ItemsSource = null;
                ItemsList.ItemsSource = ViewModel.Items;
            }
            var count = Math.Ceiling((double)ViewModel.Items.Count / 2);
            this.ItemsList.HeightRequest = count * 50 + (count - 1) * 12 + 20;
        }

        public void OnAnimationStarted(bool isPopAnimation)
        {
           
        }

        public void OnAnimationFinished(bool isPopAnimation)
        {
            
        }

        public IPageAnimation PageAnimation => this.ViewModel?.PageAnimation ??  new SlidePageAnimation()
        {
            BounceEffect = false,
            Subtype = AnimationSubtype.FromRight,
            Duration = AnimationDuration.Short
        };

        private void OnWelcomeAnimClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as DeviceConnectionViewModel;
            vm?.TogglePairMePopup();
        }

        private void OnTappedToPair(object sender, EventArgs e)
        {
            SimpleNavigationService.PushModalAsync(new DeviceSetupPageViewModel(DeviceSetupSource.Connect)).Forget();
        }
    }
}
