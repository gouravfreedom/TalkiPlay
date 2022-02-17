using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using FormsControls.Base;
using ReactiveUI;
using ReactiveUI.XamForms;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class GameListPage : TabViewBase<GameListPageViewModel>
    {
     
        public GameListPage()
        {
            InitializeComponent();

            var service = Locator.Current.GetService<IApplicationService>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = Device.RuntimePlatform == Device.iOS ? (int)service.StatusbarHeight : 0;
                var navHeight = (int)service.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = totalHeight;
                var topInset = service.GetSafeAreaInsets().Top;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);
                GuideNavButton.HeightRequest = 40;
                GuideNavButton.Margin = new Thickness(0,topInset,10,0);

            });
            
            this.WhenActivated(d =>
            {
                this.WhenAnyValue(m => m.ViewModel.LoadDataCommand)
                    .Select(m => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.LoadDataCommand)
                    .DisposeWith(d);
                
                //this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
                
                //this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                // this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                // this.OneWayBind(ViewModel, v => v.ShowBackButton, view => view.NavigationView.ShowLeftButton).DisposeWith(d);
                 //this.OneWayBind(ViewModel, v => v.GuideCommand, view => view.NavigationView.RightButton.Command).DisposeWith(d);

                // this.OneWayBind(ViewModel, v => v.Games, view => view.CarouselView.ItemsSource).DisposeWith(d);
                // this.OneWayBind(ViewModel, v => v.CurrentIndex, view => view.CarouselView.Position).DisposeWith(d);
            });
        }

        // public void OnAnimationStarted(bool isPopAnimation)
        // {
        //    
        // }
        //
        // public void OnAnimationFinished(bool isPopAnimation)
        // {
        //    
        // }
        //
        // public IPageAnimation PageAnimation => new SlidePageAnimation()
        // {
        //     BounceEffect = false,
        //     Subtype = AnimationSubtype.FromRight,
        //     Duration = AnimationDuration.Short
        // };
    }
}
