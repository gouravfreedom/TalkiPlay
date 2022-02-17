using ChilliSource.Mobile.UI;
using TalkiPlay.Managers;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class SubscriptionListPage : SimpleBasePage<SubscriptionListPageViewModel>
    {
        public SubscriptionListPage()
        {
            BuildContent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            var vm = BindingContext as SubscriptionListPageViewModel;
            vm.LoadData().Forget();
        }

        void BuildContent()
        {
            var barHeight = (int) DeviceInfo.StatusbarHeight;
            var navHeight = (int) DeviceInfo.NavBarHeight;
            var totalNavBarHeight = barHeight + navHeight;
            //var bottomOffset = (int) DeviceInfo.GetSafeAreaInsets().Bottom;

            var navView = new NavigationView
            {
                BarTintColor = Color.Transparent,
                ShowLeftButton = true,
                
                ShowRightButton = false,
                Title = "",
                
                Padding = Dimensions.NavPadding(barHeight)
            };

            //navView.SetBinding(NavigationView.IsVisibleProperty, nameof(ViewModel.ShowNavBar));
            navView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(ViewModelType.BackCommand));
            
            var label = new ExtendedLabel
            {                
                CustomFont = Fonts.HeroTitleFont,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(Dimensions.DefaultHorizontalMargin,10,
                    Dimensions.DefaultHorizontalMargin,0)
            };
            label.SetBinding(Label.TextProperty, nameof(ViewModelType.HeaderText));

            var subHeading = new ExtendedLabel()
            {
                Text = "Unlock all games with either a monthly or yearly subscription.",
                CustomFont = Fonts.Header1Font,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(Dimensions.DefaultHorizontalMargin,50,
                    Dimensions.DefaultHorizontalMargin,0)
            };
            
            var emptyStateView = new EmptyStateView();
            emptyStateView.InputTransparent = true;
            emptyStateView.SetBinding(EmptyStateView.EmptyStateTextProperty, nameof(ViewModelType.EmptyStateText));
            emptyStateView.SetBinding(EmptyStateView.IsVisibleProperty, nameof(ViewModelType.ShowEmptyState));

            var buttonLayout = new StackLayout()
            {
                Margin = new Thickness(Dimensions.DefaultHorizontalMargin,50,
                    Dimensions.DefaultHorizontalMargin,20),
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            buttonLayout.SetBinding(BindableLayout.ItemsSourceProperty, new Binding(nameof(ViewModelType.Items)));
            BindableLayout.SetItemTemplate(buttonLayout, new DataTemplate(typeof(ButtonView)));


            var privacyPolicyLabel = new ExtendedLabel()
            {
                Text = "Privacy Policy",
                CustomFont = Fonts.LinkWhiteFont,
                HorizontalTextAlignment = TextAlignment.Center
            };
            privacyPolicyLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    WebpageHelper.OpenUrl(Config.PrivacyPolicyUrl, "Privacy Policy");
                })
            });
              
            var termsLabel = new ExtendedLabel()
            {
                Text = "Terms of Use",
                CustomFont = Fonts.LinkWhiteFont,
                HorizontalTextAlignment = TextAlignment.Center
            };
            termsLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    WebpageHelper.OpenUrl(Config.TermsUrl, "Terms of Use");
                })
            });

            var linksLayout = new StackLayout()
            {
                Spacing = 10,
                Margin = new Thickness(Dimensions.DefaultHorizontalMargin,30,
                    Dimensions.DefaultHorizontalMargin,20),
                HorizontalOptions = LayoutOptions.Center,
                Children = {privacyPolicyLabel, termsLabel}
            };
            
            var mainLayout = new StackLayout()
            {
                Spacing = 0,
                Children = {label, subHeading, emptyStateView, buttonLayout,linksLayout}
            };
            
            var grid = new Grid()
            {
                RowSpacing = 0,
            };
            grid.RowDefinitions.Add(new RowDefinition() { Height = totalNavBarHeight });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
           

            var gradient = Styles.BuildMainGradient();
            grid.Children.Add(gradient);
            Grid.SetRowSpan(gradient, 2);

            grid.Children.Add(navView);
            grid.Children.Add(mainLayout,0,1);
            

            Content = grid;
            
        }
    }
}