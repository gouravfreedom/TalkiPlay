using Acr.UserDialogs;
using TalkiPlay.Functional.UI.FormsExtensions;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class WebPage : SimpleBasePage<WebPageViewModel>//, IAnimationPage
    {
        // private NavigationView _navView;
        // private WebView _webView;

        public WebPage()
        {
            BuildContent();
            
            // NavigationPage.SetHasNavigationBar(this, false);
            // NavigationPage.SetHasBackButton(this, false);
            //
            
            //Device.BeginInvokeOnMainThread(BuildContent);

            // this.WhenActivated(d =>
            // {               
            //     this.OneWayBind(ViewModel, v => v.Source, view => view._webView.Source).DisposeWith(d);
            //
            //     this.OneWayBind(ViewModel, v => v.Title, view => view._navView.Title).DisposeWith(d);
            //     this.OneWayBind(ViewModel, v => v.ShowBackButton, view => view._navView.ShowLeftButton).DisposeWith(d);
            //     this.BindCommand(ViewModel, v => v.BackCommand, view => view._navView.LeftButton).DisposeWith(d);
            //     this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);               
            // });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //var userDialog = Locator.Current.GetService<IUserDialogs>();
            Dialogs.ShowLoading("Loading ...", MaskType.Black);
        }

        void BuildContent()
        {            
            //var barHeight = Device.RuntimePlatform == Device.iOS ? (int)service.StatusbarHeight : 0;                    
            var barHeight = DeviceInfo.StatusbarHeight;
            var navHeight = DeviceInfo.NavBarHeight;
            var totalHeight = barHeight + navHeight;
            
            var navView = new NavigationView
            {
                BarTintColor = Colors.NavColor1,
                ShowLeftButton = true,
                
                ShowRightButton = false,
                
                Padding = Dimensions.NavPadding(barHeight)
            };
            navView.SetBinding(NavigationView.TitleProperty, nameof(ViewModel.Title));
            navView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(ViewModel.BackCommand));
            //navView.SetBinding(NavigationView.Left, nameof(ViewModel.BackCommand));
            
            var webView = new WebView()
            {             
                Margin = 0,
            };
            webView.SetBinding(WebView.SourceProperty, nameof(ViewModel.Source));
            webView.Navigating += WebView_Navigating;
            webView.Navigated += WebView_Navigated;

            var grid = new Grid()
            {                
            };
            grid.RowSpacing = 0;
            grid.RowDefinitions.Add(new RowDefinition() { Height = totalHeight });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });

            grid.Children.Add(navView, 0, 0);
            grid.Children.Add(webView, 0, 1);

            Content = grid;

        }

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            //var userDialog = Locator.Current.GetService<IUserDialogs>();
            Dialogs.HideLoading();
        }

        // public void OnAnimationStarted(bool isPopAnimation)
        // {
        // }
        //
        // public void OnAnimationFinished(bool isPopAnimation)
        // {
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
