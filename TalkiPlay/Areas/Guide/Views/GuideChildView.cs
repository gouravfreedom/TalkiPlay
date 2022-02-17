using ChilliSource.Mobile.UI;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class GuideChildView : ContentView
    {
        public GuideChildView()
        {
            
            BuildContent();
            //this.SetBinding(ContentView.IsVisibleProperty, nameof(GuideChildViewModel.IsVisible));
        }
        
        void BuildContent()
        {
            var image = new SvgCachedImage()
            {
                Aspect = Aspect.AspectFit,
                HeightRequest = 80,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                LoadingPlaceholder = Images.LoadingIndicator,
                ErrorPlaceholder = Images.PlaceHolder,
            };
            image.SetBinding(CachedImage.SourceProperty, nameof(GuideChildViewModel.ImageSource));
          
            var label = new ExtendedLabel
            {
                CustomFont = Fonts.Header2Font,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 8, 0, 0),
                LineBreakMode = LineBreakMode.TailTruncation
            };
            label.SetBinding(Label.TextProperty, nameof(GuideChildViewModel.Name));

           
            
            var layout = new StackLayout()
            {
                Spacing = 0,
                Padding = 0,
                
               
                Children = { image, label}
            };

            var width = (DeviceInfo.ScreenSize.Width - 15 * 2 - 20) / 2 - 20 -4; 
            
            var frame = new Frame()
            {
                HasShadow = false,
                CornerRadius = 5,
                Margin = 2,
                Padding = 10,
                Content = layout,
                BackgroundColor = Colors.Blue3Color,
                IsClippedToBounds = true,
                BorderColor = Colors.WhiteColor,
                
                //TODO: remove once https://github.com/xamarin/Xamarin.Forms/issues/9125 is fixed
                WidthRequest = width,
                
               
            };
            
            
            var gesture = new TapGestureRecognizer();
            gesture.SetBinding(TapGestureRecognizer.CommandProperty, nameof(GuideChildViewModel.Command));
            frame.GestureRecognizers.Add(gesture);
            
            Content = frame;
        }
    }
}