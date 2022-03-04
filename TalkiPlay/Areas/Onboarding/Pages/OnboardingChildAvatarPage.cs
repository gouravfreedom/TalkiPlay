using ChilliSource.Mobile.UI;
using MagicGradients;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using GradientStop = Xamarin.Forms.GradientStop;

namespace TalkiPlay
{
    public class OnboardingChildAvatarPage : SimpleBasePage<OnboardingChildAvatarPageViewModel>
    {
        public OnboardingChildAvatarPage()
        {            
            BuildContent();            
        }

        void BuildContent()
        {
            var barHeight = DeviceInfo.StatusbarHeight;
            var navHeight = DeviceInfo.NavBarHeight;
            var totalHeight = barHeight + navHeight;
            var bottomOffset = DeviceInfo.GetSafeAreaInsets().Bottom;


            var navView = new NavigationView
                {
                    BarTintColor = Color.Transparent,
                    ShowLeftButton = true,
                    
                    ShowRightButton = false,
                    //Title = "",
                    
                    Padding = Dimensions.NavPadding(barHeight)
                }
                .Bind(NavigationView.TitleProperty,
                    nameof(OnboardingChildAvatarPageViewModel.Title));

            navView.SetBinding(NavigationView.IsVisibleProperty, nameof(OnboardingChildAvatarPageViewModel.ShowNavBar));
            navView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(OnboardingChildAvatarPageViewModel.BackCommand));


            var label = new ExtendedLabel
            {
                CustomFont = Fonts.Header1Font,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            label.SetBinding(Label.TextProperty, nameof(OnboardingChildAvatarPageViewModel.HeaderText));


            var collectionView = new CollectionView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,                
                SelectionMode = SelectionMode.None,                
                ItemsLayout = new GridItemsLayout(ItemsLayoutOrientation.Vertical)
                {                    
                    Span = 4,
                    HorizontalItemSpacing = 10,
                    VerticalItemSpacing = 10
                },
                ItemTemplate = new DataTemplate(() => new AvatarItem())
            };
            collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(OnboardingChildAvatarPageViewModel.Avatars));
            collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(OnboardingChildAvatarPageViewModel.SelectedItem), BindingMode.TwoWay );

            var stops = new GradientStopCollection();
            stops.Add(new GradientStop() { Color = Color.FromHex("#23bcba"), Offset = 0.1f });
            stops.Add(new GradientStop() { Color = Color.FromHex("#45e994"), Offset = 1.0f });

            var nextButtonFrame = new Frame()
            {
                HasShadow = false,
                BackgroundColor = Color.Transparent,
                Padding = new Thickness(20, 0),
                Margin = new Thickness(20, 0),
                CornerRadius = 25,
                Background = new LinearGradientBrush(stops)
            };

            var nextButton = new Button
            {
                Style = Styles.NewPrimaryButtonStyle,
                Text = "Next",
                HorizontalOptions = LayoutOptions.Fill
            }.Bind(Button.TextProperty,nameof(OnboardingChildAvatarPageViewModel.NextButtonText));
            nextButton.SetBinding(Button.CommandProperty, nameof(OnboardingChildAvatarPageViewModel.NextCommand));
            nextButtonFrame.Content = nextButton;
            var buttonLayout = new StackLayout
            {
                Children = { nextButtonFrame },
                Margin = new Thickness(0, 0, 0, 20 + bottomOffset)
            };

            var layout = new StackLayout
            {
                Spacing = 40,
                Margin = 20,                
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children = { label, collectionView },                
            };
            
            var grid = new Grid()
            {
                RowSpacing = 0,
            };
            grid.RowDefinitions.Add(new RowDefinition() { Height = totalHeight });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto});

            var gradient = Styles.BuildMainGradient();
            gradient.Bind(GradientView.IsVisibleProperty,
                nameof(OnboardingChildAvatarPageViewModel.IsOnboardingMode));
            grid.Children.Add(gradient);
            Grid.SetRowSpan(gradient, 3);

            grid.Children.Add(navView);
            grid.Children.Add(layout, 0, 1);
            grid.Children.Add(buttonLayout, 0, 2);
            
            Content = grid;
        }
    }
}
