using ChilliSource.Mobile.UI;
using FormsControls.Base;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class GuidePackSelectionPage : SimpleBasePage<GuidePackSelectionPageViewModel>
    {
        public GuidePackSelectionPage()
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

            var titleLabel = new ExtendedLabel
            {                
                CustomFont = Fonts.HeroTitleFont,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(Dimensions.DefaultHorizontalMargin,0,
                    Dimensions.DefaultHorizontalMargin,0)
            };
            titleLabel.SetBinding(Label.TextProperty, nameof(ViewModelType.HeaderText));
            
            var emptyView = new StackLayout
            {
                Margin = 40,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.Center
            };
            emptyView.Children.Add(new ExtendedLabel
            {
                Margin = 40,
                Text = "No game packs found.",
                CustomFont = Fonts.BodyWhiteFont,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            });
            
            var collectionView = new CollectionView()
            {
                Margin = new Thickness(Dimensions.DefaultHorizontalMargin,10,
                    Dimensions.DefaultHorizontalMargin,Dimensions.DefaultSpacing + bottomOffset),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                SelectionMode = SelectionMode.None,
                ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
                {
                    ItemSpacing = Dimensions.DefaultSpacing
                },
                Header = new StackLayout()
                {
                    Padding = new Thickness(0,0,0,20),
                    Children = { titleLabel}
                },
                ItemTemplate = new DataTemplate(() =>
                {
                    var layout = new StackLayout()
                    { 
                        Padding = 0,
                        Margin = 0,
                        Orientation = StackOrientation.Horizontal,
                        Spacing = Dimensions.DefaultSpacing,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    
                    layout.SetBinding(BindableLayout.ItemsSourceProperty, new Binding("."));
                    BindableLayout.SetItemTemplate(layout, new DataTemplate(typeof(GuidePackView)));
                    return layout;
                }),
                EmptyView = emptyView
                
            };
            collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(ViewModelType.Items));
            
            //TODO: re-enable once https://github.com/xamarin/Xamarin.Forms/issues/9125 is fixed
            
            // var collectionView = new CollectionView()
            // {
            //     Margin = new Thickness(Dimensions.DefaultHorizontalMargin,Dimensions.DefaultSpacing,
            //         Dimensions.DefaultHorizontalMargin,Dimensions.DefaultSpacing + bottomOffset),
            //     VerticalOptions = LayoutOptions.FillAndExpand,
            //     HorizontalOptions = LayoutOptions.FillAndExpand,
            //     SelectionMode = SelectionMode.None,
            //     ItemsLayout = new GridItemsLayout(ItemsLayoutOrientation.Vertical)
            //     {
            //         Span = Device.Idiom == TargetIdiom.Tablet ? 4 : 2, 
            //         HorizontalItemSpacing = Dimensions.DefaultSpacing,
            //         VerticalItemSpacing = Dimensions.DefaultSpacing
            //     },
            //     ItemTemplate = new DataTemplate(typeof(GuidePackView)),
            //     EmptyView = emptyView
            //     
            // };
            // collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(ViewModelType.Items));
            
            var gradient = Styles.BuildMainGradient();

            var grid = new Grid()
            {
                RowSpacing = 0
            };
            
            grid.RowDefinitions.Add(new RowDefinition(){Height = totalNavBarHeight});
            //grid.RowDefinitions.Add(new RowDefinition(){Height = GridLength.Auto});
            grid.RowDefinitions.Add(new RowDefinition(){Height = GridLength.Star});

            grid.Children.Add(gradient);
            Grid.SetRowSpan(gradient, 2);
            
            grid.Children.Add(navView);
            //grid.Children.Add(titleLabel,0,1);
            grid.Children.Add(collectionView,0,1);

            Content = grid;
            
        }
    }
}