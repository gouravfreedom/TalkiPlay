using ChilliSource.Mobile.UI;
using FFImageLoading.Forms;
using TalkiPlay.Shared;
using Xamarin.Forms;
using FFImageLoading.Svg.Forms;

namespace TalkiPlay
{
    public class GuideInfoPage : SimpleBasePage<GuideInfoPageViewModel>
    {
        public GuideInfoPage()
        {
            BuildContent();
        }
        
        void BuildContent()
        {
            var barHeight = (int)DeviceInfo.StatusbarHeight;
            var navHeight = (int)DeviceInfo.NavBarHeight;
            var totalNavBarHeight = barHeight + navHeight;
            var bottomOffset = (int)DeviceInfo.GetSafeAreaInsets().Bottom;

            var navView = new NavigationView
            {
                BarTintColor = Color.Transparent,
                ShowLeftButton = true,
                
                ShowRightButton = false,
                Title = "",
                
                Padding = Dimensions.NavPadding(barHeight)
            };
            navView.SetBinding(NavigationView.IsVisibleProperty, nameof(ViewModelType.ShowNavBar));
            navView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(ViewModelType.BackCommand));
            
            var titleLabel = new Label
            {                
                FontFamily = Fonts.HeroTitleFont.Family,
                TextColor = Fonts.HeroTitleFont.Color,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                //MinimumHeightRequest = 50,
            };
            titleLabel.SetBinding(Label.TextProperty, nameof(ViewModelType.HeaderText));
            titleLabel.SetBinding(Label.FontSizeProperty, nameof(ViewModelType.HeaderTextFontSize));
            
            var image = new SvgCachedImage()
            {
                Aspect = Aspect.AspectFit,
                HeightRequest = 300,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                //VerticalOptions = LayoutOptions.FillAndExpand,
                MinimumHeightRequest = 100,
            };
            image.SetBinding(CachedImage.SourceProperty, nameof(ViewModelType.ImageSource));
            image.SetBinding(CachedImage.IsVisibleProperty, nameof(ViewModelType.HasImage));

            var bodyLabel = new Label
            {                
                FontFamily = Fonts.HeroTitleFont.Family,
                TextColor = Fonts.HeroTitleFont.Color,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20,0),
                //AdjustFontSizeToFit = true,
                //MinimumHeightRequest = 50,
            };
            bodyLabel.SetBinding(Label.TextProperty, nameof(ViewModelType.BodyText));
            bodyLabel.SetBinding(Label.FontSizeProperty, nameof(ViewModelType.BodyTextFontSize));

            var mainLayout = new StackLayout()
            {
                Margin = new Thickness(15,20),
                Spacing = 20,
                VerticalOptions = LayoutOptions.Center,
                Children = { titleLabel, image, bodyLabel}
            };
            
            var button = new Button
            {                
                Style = Styles.PrimaryButtonStyle,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(15,20,15,20 + bottomOffset)
            };
            button.SetBinding(Button.TextProperty, nameof(ViewModelType.NextButtonText));
            button.SetBinding(Button.IsVisibleProperty, nameof(ViewModelType.ShowNextButton));
            button.SetBinding(Button.CommandProperty, nameof(ViewModelType.NextCommand));

            var imageButton = new ImageButtonView()
            {
                WidthRequest = 300,
                DefaultSource = Images.LetsPlayButtonImage,
                Margin = new Thickness(15,20,15,20 + bottomOffset)
            };
            imageButton.SetBinding(ImageButtonView.CommandProperty, nameof(ViewModelType.NextCommand));
            imageButton.SetBinding(ImageButtonView.IsVisibleProperty, nameof(ViewModelType.ShowImageButton));
            
            var gradient = Styles.BuildMainGradient();
            
            var grid = new Grid();
            
            grid.RowDefinitions.Add(new RowDefinition(){Height = totalNavBarHeight});
            grid.RowDefinitions.Add(new RowDefinition(){Height = GridLength.Auto});
            grid.RowDefinitions.Add(new RowDefinition(){Height = GridLength.Star});
            grid.RowDefinitions.Add(new RowDefinition(){Height = GridLength.Auto});
            
            grid.Children.Add(gradient);
            Grid.SetRowSpan(gradient, 4);
            
            grid.Children.Add(navView);
            grid.Children.Add(titleLabel,0,1);
            grid.Children.Add(mainLayout,0,2);
            grid.Children.Add(button,0,3);
            grid.Children.Add(imageButton,0,3);

            Content = grid;
        }
        
    }
    
   
}