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
    public partial class ChildListPage :  BasePage<ChildListPageViewModel>//, IAnimationPage
    {
    
        public ChildListPage()
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

                    
                    this.OneWayBind(ViewModel, v => v.Children, view => view.ChildrenList.ItemsSource).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.AddCommand, view => view.AddChildButton.Button).DisposeWith(d);

                    this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.ShowLeftMenuItem, view => view.NavigationView.ShowLeftButton).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.AddCommand, view => view.NavigationView.RightButton).DisposeWith(d);

                    this.ChildrenList.Events()
                        .SelectionChanged
                        .Select(m => m.CurrentSelection.FirstOrDefault())
                        .Where(m => m != null)
                        .Select(m => (ChildViewModel) m)
                        .Do(m => this.ChildrenList.SelectedItem = null)
                        .InvokeCommand(this, v => v.ViewModel.SelectCommand)
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
    }
}
