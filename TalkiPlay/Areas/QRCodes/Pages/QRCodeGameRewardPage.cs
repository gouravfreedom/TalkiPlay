using System;
using ChilliSource.Mobile.UI;
using FFImageLoading.Svg.Forms;
using Rg.Plugins.Popup.Pages;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class QRCodeGameRewardPage : PopupPage
    {
        public QRCodeGameRewardPage()
        {
            //BackgroundColor = Color.White;
            BuildContent();
        }

        void BuildContent()
        {
            var label = new ExtendedLabel
            {
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                CustomFont = Fonts.Header1BlackFont
            };
            label.SetBinding(Label.TextProperty, nameof(QRCodeGameRewardPageViewModel.RewardInstruction));

            var image = new SvgCachedImage
            {
                Aspect = Aspect.AspectFit
            };
            image.SetBinding(SvgCachedImage.SourceProperty, nameof(QRCodeGameRewardPageViewModel.ImageSource));

            var button = new Button
            {
                Style = Styles.PrimaryButtonStyle,
                Text = "Close",
                Margin = new Thickness(0, 15, 0, 0)
            };
            button.SetBinding(ExtendedButton.CommandProperty, nameof(QRCodeGameRewardPageViewModel.CloseCommand));
            button.SetBinding(ExtendedButton.IsVisibleProperty, nameof(QRCodeGameRewardPageViewModel.ShowCloseButton));


            var grid = new Grid
            {
                RowSpacing = 8,
                Padding = 15,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star)});
            grid.RowDefinitions.Add(new RowDefinition { Height = 80});
            grid.Children.Add(label);
            grid.Children.Add(image, 0, 1);
            grid.Children.Add(button, 0, 2);

            var frame = new Frame
            {
                HasShadow = false,
                BackgroundColor = Color.White,
                Margin = 20,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Content = grid
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.SetBinding(TapGestureRecognizer.CommandProperty, nameof(QRCodeGameRewardPageViewModel.TapCommand));
            frame.GestureRecognizers.Add(tapGesture);

            Content = frame;
        }

    }
}
