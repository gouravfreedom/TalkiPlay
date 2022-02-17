using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using FormsControls.Base;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class RoomListPage : BasePage<RoomListPageViewModel>, IAnimationPage
    {
       
        public RoomListPage()
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
                var height = service.ScreenSize.Height / 2;

                EmptyViewLayout.Margin = new Thickness(0, 0, 0, height);

            });
            
            
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(m => m.ViewModel.LoadDataCommand)
                    .Select(m => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.LoadDataCommand)
                    .DisposeWith(d);
                
                this.OneWayBind(ViewModel, v => v.Rooms, view => view.RoomList.ItemsSource).DisposeWith(d);
             
                this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.ShowLeftMenuItem, view => view.NavigationView.ShowLeftButton).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.AddCommand, view => view.NavigationView.RightButton).DisposeWith(d);


                this.RoomList.Events()
                    .SelectionChanged
                    .Select(m => m.CurrentSelection.FirstOrDefault())
                    .Where(m => m != null)
                    .Select(m => (RoomViewModel)m)
                    .Do(m => this.RoomList.SelectedItem = null)
                    .InvokeCommand(this, v => v.ViewModel.SelectCommand)
                    .DisposeWith(d);

                this.WhenAnyValue(m => m.ViewModel.IsLoading)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(m => m)
                    .Subscribe(async m => { await this.RoomList.FadeTo(!m ? 1 : 0, 250, Easing.Linear); })
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
    }
}
