using System;
using ChilliSource.Mobile.UI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class QRCodePdfView : ContentView
    {
        public QRCodePdfView()
        {
            BuildContent();
        }

        void BuildContent()
        {
            var label = new ExtendedLabel
            {
                CustomFont = Fonts.Header1Font,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            label.SetBinding(Label.TextProperty, nameof(QRCodePdfViewModel.Title));

            var button = new ExtendedButtonEx
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Download"
            };
            button.SetBinding(Button.CommandProperty, nameof(QRCodePdfViewModel.DownloadCommand));

            var layout = new StackLayout()
            {
                Margin = new Thickness(40,40,40,0),
                Spacing = 20,
                Children = {label, button}
            };

            Content = layout;
        }
    }
}
