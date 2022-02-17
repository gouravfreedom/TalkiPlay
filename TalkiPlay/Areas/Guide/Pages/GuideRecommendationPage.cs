using ChilliSource.Mobile.UI;
using FormsControls.Base;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class GuideRecommendationPage : SimpleBasePage<GuideRecommendationPageViewModel>
    {
        public GuideRecommendationPage()
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
                //Margin = new Thickness(Dimensions.DefaultHorizontalMargin,0,Dimensions.DefaultHorizontalMargin,0)
            };
            titleLabel.SetBinding(Label.TextProperty, nameof(ViewModelType.HeaderText));
            titleLabel.SetBinding(Label.FontSizeProperty, nameof(ViewModelType.HeaderTextFontSize));

            
            var emptyView = new StackLayout
            {
                Margin = 40,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.Center
            };
            emptyView.Children.Add(new ExtendedLabel
            {
                Margin = 40,
                Text = "No game recommendations found.",
                CustomFont = Fonts.BodyWhiteFont,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            });
            
            var collectionView = new CollectionView()
            {
                Margin = new Thickness(Dimensions.DefaultHorizontalMargin,
                    10,
                    Dimensions.DefaultHorizontalMargin,
                    0),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                SelectionMode = SelectionMode.None,
                ItemsLayout = new GridItemsLayout(ItemsLayoutOrientation.Vertical)
                {
                    Span = Device.Idiom == TargetIdiom.Tablet ? 2 : 1, 
                    HorizontalItemSpacing = Dimensions.DefaultSpacing,
                    VerticalItemSpacing = Dimensions.DefaultSpacing
                },
                Header = new StackLayout()
                {
                    Padding = new Thickness(0,0,0,20),
                    Children =
                    {
                        titleLabel                        
                    }
                },
                ItemTemplate = new DataTemplate(typeof(GuideGameView)),
                EmptyView = emptyView
                
            };
            collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(ViewModelType.Items));
            
            var button = new Button
            {                
                Style = Styles.PrimaryButtonStyle,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(Dimensions.DefaultHorizontalMargin,Dimensions.DefaultSpacing
                    ,Dimensions.DefaultHorizontalMargin,Dimensions.DefaultSpacing + bottomOffset),
            };
            button.SetBinding(Button.CommandProperty, nameof(ViewModelType.NextCommand));
            button.SetBinding(Button.TextProperty, nameof(ViewModelType.NextButtonText));

            var gradient = Styles.BuildMainGradient();

            var grid = new Grid()
            {
                RowSpacing = 0
            };
            
            grid.RowDefinitions.Add(new RowDefinition(){Height = totalNavBarHeight});
            //grid.RowDefinitions.Add(new RowDefinition(){Height = GridLength.Auto});
            grid.RowDefinitions.Add(new RowDefinition(){Height = GridLength.Star});
            grid.RowDefinitions.Add(new RowDefinition(){Height = GridLength.Auto});

            grid.Children.Add(gradient);
            Grid.SetRowSpan(gradient, 3);
            
            grid.Children.Add(navView);
            //grid.Children.Add(titleLabel,0,1);
            grid.Children.Add(collectionView,0,1);
            grid.Children.Add(button,0,2);

            Content = grid;
        }
        
    }
}