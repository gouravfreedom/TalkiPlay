using ChilliSource.Mobile.UI;
using Xamarin.Forms;
using TalkiPlay.Shared;

namespace TalkiPlay
{
    public class ToggleItemView : ContentView
    {
        ExtendedLabel _label;
        Switch _switch;

        public ToggleItemView()
        {
            BuildContent();
        }

        void BuildContent()
        {
            _label = new ExtendedLabel
            {
                CustomFont = Fonts.TitleRegularFontBig,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
            };
            _label.SetBinding(Label.TextProperty, nameof(ToggleItemViewModel.Title));

            _switch = new Switch
            {
                HorizontalOptions = LayoutOptions.End,
                OnColor = Colors.TealColor,
                ThumbColor = Colors.Yellow
            };
            _switch.SetBinding(Switch.IsToggledProperty, nameof(ToggleItemViewModel.IsOn), BindingMode.TwoWay);

            var grid = new Grid()
            {
                Margin = new Thickness(20, 15)
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.Children.Add(_label);
            grid.Children.Add(_switch, 1, 0);

            Content = grid;
        }
    }

    public class ToggleItemViewCell : ViewCell
    {
        public ToggleItemViewCell()
        {
            View = new ToggleItemView();
        }
    }
}
