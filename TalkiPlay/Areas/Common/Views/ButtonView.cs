using ChilliSource.Mobile.UI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class ButtonView : ContentView
    {
        public ButtonView()
        {
            BuildContent();
        }

        void BuildContent()
        {
            var button = new ExtendedButton
            {                
                Style = Styles.PrimaryButtonStyle,
                HeightRequest = 70,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(0,10),
                BorderColor = Colors.WhiteColor,
                BackgroundColor = Colors.Blue3Color,
                BorderWidth = 1,
                AllowTextWrapping = true,
                ContentPadding = new Thickness(10,0)
                
            };
            button.SetBinding(Button.TextProperty, nameof(ButtonViewModel.Text));
            button.SetBinding(Button.CommandProperty, nameof(ButtonViewModel.Command));

            Content = button;
        }
    }
}