using ChilliSource.Mobile.UI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class SubscriptionTermsPage : SimpleBasePage<SubscriptionTermsPageViewModel>
    {
        public SubscriptionTermsPage()
        {
            BuildContent();
        }
        
        void BuildContent()
        {
            var barHeight = (int) DeviceInfo.StatusbarHeight;
            var navHeight = (int) DeviceInfo.NavBarHeight;
            var totalNavBarHeight = barHeight + navHeight;
            var bottomOffset = (int) DeviceInfo.GetSafeAreaInsets().Bottom;

            var navView = new NavigationView
            {
                BarTintColor = Color.Transparent,
                ShowLeftButton = true,
                
                ShowRightButton = false,
                Title = "",
                
                Padding = Dimensions.NavPadding(barHeight)
            };
            navView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(ViewModelType.BackCommand));
            
            var headerLabel = new ExtendedLabel
            {           
                Text = "Subscription Details",
                CustomFont = Fonts.HeroTitleFont,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(Dimensions.DefaultHorizontalMargin,10,
                    Dimensions.DefaultHorizontalMargin,0)
            };


            var textLabel = new ExtendedLabel()
            {
                CustomFont = Fonts.Header1WhiteRegularFont,
                HorizontalOptions = LayoutOptions.Center,
                //HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(Dimensions.DefaultHorizontalMargin,50,
                    Dimensions.DefaultHorizontalMargin,20)
            };
            textLabel.SetBinding(Label.TextProperty, nameof(ViewModelType.Text));
            
            var scrollView = new ScrollView()
            {
                Content = new StackLayout()
                {
                    Children = { headerLabel, textLabel }
                }
            };
            
            var button = new Button
            {                
                Text = "Continue",
                Style = Styles.PrimaryButtonStyle,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(Dimensions.DefaultHorizontalMargin,Dimensions.DefaultSpacing
                    ,Dimensions.DefaultHorizontalMargin,Dimensions.DefaultSpacing + bottomOffset),
            };
            button.SetBinding(Button.CommandProperty, nameof(ViewModelType.ContinueCommand));
            button.SetBinding(Button.IsVisibleProperty, nameof(ViewModelType.ShowContinueButton));
            
            var grid = new Grid()
            {
                RowSpacing = 0,
            };
            grid.RowDefinitions.Add(new RowDefinition() { Height = totalNavBarHeight });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            
            var gradient = Styles.BuildMainGradient();
            grid.Children.Add(gradient);
            Grid.SetRowSpan(gradient, 3);

            grid.Children.Add(navView);
            grid.Children.Add(scrollView,0,1);
            grid.Children.Add(button,0,2);
            
            Content = grid;
            
        }    
    }
    
    
}