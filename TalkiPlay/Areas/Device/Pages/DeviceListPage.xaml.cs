using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using FormsControls.Base;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class DeviceListPage : BasePage<DeviceListPageViewModel>, IAnimationPage
    {
    
        public DeviceListPage()
        {
            InitializeComponent();

            BleList.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
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
        
            this.WhenActivated(d =>
                {
                    this.OneWayBind(ViewModel, v => v.Devices, view => view.BleList.ItemsSource).DisposeWith(d);
                   
                    this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.LeftMenuEnabled, view => view.NavigationView.ShowLeftButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.Instructions, view => view.EmptyStateCarouselView.ItemsSource).DisposeWith(d);                

                    this.BleList.Events()
                        .ItemSelected
                        .Where(m => m.SelectedItem != null)
                        .Select(m => (DeviceItemViewModel)m.SelectedItem)
                        .Do(m => this.BleList.SelectedItem = null)
                        .InvokeCommand(this, v => v.ViewModel.SelectCommand)
                        .DisposeWith(d);


                    BleList.Events()
                        .Refreshing
                        .Select(m => Unit.Default)
                        .InvokeCommand(this, v => v.ViewModel.RefreshCommand)
                        .DisposeWith(d);

                    ViewModel.WhenAnyValue(m => m.IsRefreshing)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(m => { BleList.IsRefreshing = m; })
                        .DisposeWith(d);

                    ViewModel.WhenAnyValue(m => m.ShowEmptyState)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(m => { EmptyStateView.IsVisible = m; })
                        .DisposeWith(d);


                });
        }


        public override void OnAppeared()
        {
            base.OnAppeared();
            this.ViewModel.StartScan();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                this.ViewModel.StartScan();
            }
            
            var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
            safeInsets.Top = 0;
            Padding = safeInsets;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.ViewModel.StopScan();
        }


        public void OnAnimationStarted(bool isPopAnimation)
        {
        }

        public void OnAnimationFinished(bool isPopAnimation)
        {
        }

        public IPageAnimation PageAnimation => this.ViewModel?.PageAnimation ?? new SlidePageAnimation()
        {
            BounceEffect = false,
            Subtype = AnimationSubtype.FromBottom,
            Duration = AnimationDuration.Short
        };
    }
}
