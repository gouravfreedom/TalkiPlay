using ChilliSource.Mobile.UI;
using FormsControls.Base;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class GuideQuestionPage : SimpleBasePage<GuideQuestionPageViewModel>
    {
        public GuideQuestionPage()
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

            navView.SetBinding(NavigationView.IsVisibleProperty, nameof(ViewModelType.ShowNavBar));
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
            
            var buttonLayout = new StackLayout()
            {
                Margin = new Thickness(15,15,15,30),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            buttonLayout.SetBinding(BindableLayout.ItemsSourceProperty, new Binding(nameof(ViewModelType.Items)));
            BindableLayout.SetItemTemplate(buttonLayout, new DataTemplate(typeof(ButtonView)));
            
            var scrollView = new ScrollView()
            {
                Content = new StackLayout()
                {
                    Children = { label, buttonLayout}
                }
            };
            
            var grid = new Grid()
            {
                RowSpacing = 0,
            };
            grid.RowDefinitions.Add(new RowDefinition() { Height = totalNavBarHeight });
            //grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
           

            var gradient = Styles.BuildMainGradient();
            grid.Children.Add(gradient);
            Grid.SetRowSpan(gradient, 2);

            grid.Children.Add(navView);
            //grid.Children.Add(label,0,1);
            grid.Children.Add(scrollView, 0, 1);

            Content = grid;
            
        }
    }
}