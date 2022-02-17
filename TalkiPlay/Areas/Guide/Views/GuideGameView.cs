using ChilliSource.Mobile.UI;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using FFImageLoading.Transformations;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class GuideGameView : ContentView
    {
        public GuideGameView()
        {
            BuildContent();
        }

        void BuildContent()
        {
            var titleLabel = new ExtendedLabel
            {
                CustomFont = Fonts.HeaderFont,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(10,0),
            };
            titleLabel.SetBinding(Label.TextProperty, nameof(GuideGameViewModel.Text));
            
            var sampleLabel = new ExtendedLabel
            {
                CustomFont = Fonts.Header1Font,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Text = "Sample"
            };
            sampleLabel.SetBinding(Label.IsVisibleProperty, nameof(GuideGameViewModel.IsLocked));

            var labelLayout = new StackLayout()
            {
                Children = { titleLabel, sampleLabel},
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
            
            var image = new SvgCachedImage()
            {
                Aspect = Aspect.AspectFill,
                HeightRequest = 200,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                LoadingPlaceholder = Images.LoadingIndicator,
                ErrorPlaceholder = Images.PlaceHolder,
            };
            image.SetBinding(CachedImage.SourceProperty, nameof(GuideGameViewModel.ImageSource));

            var lockImage = new SvgCachedImage()
            {
                Aspect = Aspect.AspectFit,
                HeightRequest = 150,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                //Margin = 5,
                // HeightRequest = 30,
                // WidthRequest = 30,
                //Margin = new Thickness(0,5,5,0),
                // HorizontalOptions = LayoutOptions.End,
                // VerticalOptions = LayoutOptions.Start,
                Source = Images.LockImage
            };
            lockImage.SetBinding(CachedImage.IsVisibleProperty, nameof(GuideGameViewModel.IsLocked));

            
            var grid = new Grid();
            grid.Children.Add(image);
            grid.Children.Add(lockImage);
            grid.Children.Add(labelLayout);
            
            var frame = new Frame()
            {
                HasShadow = false,
                Margin = new Thickness(2,0),
                Padding = 0,
                CornerRadius = 5,
                Content = grid,
                IsClippedToBounds = true,
                BorderColor = Colors.WhiteColor
            };
            // var gesture = new TapGestureRecognizer();
            // gesture.SetBinding(TapGestureRecognizer.CommandProperty, nameof(GuideGameViewModel.Command));
            // frame.GestureRecognizers.Add(gesture);
            
            Content = frame;
        }
    }
}