using ChilliSource.Mobile.UI;
using Xamarin.Forms;
using TalkiPlay.Shared;

namespace TalkiPlay
{
    public class LegalSubscriptionInfoPage : SimpleBasePage<LegalSubscriptionInfoPageViewModel>
    {
        public LegalSubscriptionInfoPage()
        {
            BackgroundColor = Color.White;
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
                BarTintColor = Colors.NavColor1,
                ShowLeftButton = true,
                
                ShowRightButton = false,
                Title = "Subscription Details",
                
                Padding = Dimensions.NavPadding(barHeight)
            };
            navView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(ViewModelType.BackCommand));
            
            var textLabel = new ExtendedLabel()
            {
                CustomFont = Fonts.LabelBlackFont,
                HorizontalOptions = LayoutOptions.Center,
                //HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(Dimensions.DefaultHorizontalMargin,20,
                    Dimensions.DefaultHorizontalMargin,20)
            };
            textLabel.SetBinding(Label.TextProperty, nameof(ViewModelType.Text));
            
            var scrollView = new ScrollView()
            {
                Content = new StackLayout()
                {
                    Children = { textLabel }
                }
            };
            
            var grid = new Grid()
            {
                RowSpacing = 0,
            };
            grid.RowDefinitions.Add(new RowDefinition() { Height = totalNavBarHeight });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
            
            grid.Children.Add(navView);
            grid.Children.Add(scrollView,0,1);
            
            Content = grid;
            
        }  
        
    }
}