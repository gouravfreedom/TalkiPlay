using ChilliSource.Mobile.UI;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class GuidePackView : ContentView
    {
        public GuidePackView()
        {
            BuildContent();
            //this.SetBinding(ContentView.IsVisibleProperty, nameof(GuidePackViewModel.IsVisible));
        }

        void BuildContent()
        {
            var image = new SvgCachedImage()
            {
                Aspect = Aspect.AspectFill,
                HeightRequest = 100,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                LoadingPlaceholder = Images.LoadingIndicator,
                ErrorPlaceholder = Images.PlaceHolder,
            };
            image.SetBinding(CachedImage.SourceProperty, nameof(GuidePackViewModel.ImageSource));
            
            var label = new ExtendedLabel
            {
                CustomFont = Fonts.Header2Font,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(10, 8, 10, 10),
                LineBreakMode = LineBreakMode.TailTruncation
            };
            label.SetBinding(Label.TextProperty, nameof(GuidePackViewModel.Text));

            var layout = new StackLayout()
            {
                Spacing = 0,
                Children = { image, label}
            };
            
            var width = (DeviceInfo.ScreenSize.Width - 15 * 2 - 20) / 2 - 4 -4; 
            
            var frame = new Frame()
            {
                HasShadow = false,
                CornerRadius = 5,
                Margin = 2,
                Padding = 2,
                Content = layout,
                BackgroundColor = Colors.Blue3Color,
                IsClippedToBounds = true,
                BorderColor = Colors.WhiteColor,
                
                //TODO: remove once https://github.com/xamarin/Xamarin.Forms/issues/9125 is fixed
                WidthRequest = width,
            };
            var gesture = new TapGestureRecognizer();
            gesture.SetBinding(TapGestureRecognizer.CommandProperty, nameof(GuidePackViewModel.Command));
            frame.GestureRecognizers.Add(gesture);
            
            Content = frame;
        }
    }
}