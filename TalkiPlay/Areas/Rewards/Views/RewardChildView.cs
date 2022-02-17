using ChilliSource.Mobile.UI;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class RewardChildView : ContentView
    {
        public RewardChildView()
        {
            BuildContent();
        }

        void BuildContent()
        {
            
            var image = new SvgCachedImage()
            {
                Aspect = Aspect.AspectFit,
                HeightRequest = 200,
                WidthRequest = 200,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Source = Images.TreasureChestImage
            };

            var lockImage = new SvgCachedImage()
            {
                Aspect = Aspect.AspectFit,
                HeightRequest = 150,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Source = Images.LockImage
            };
            lockImage.SetBinding(CachedImage.IsVisibleProperty, nameof(RewardChildViewModel.IsLocked));

            var label = new ExtendedLabel
            {
                CustomFont = Fonts.HeaderFont,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(0,0,0,40),
            };
            label.SetBinding(Label.TextProperty, nameof(RewardChildViewModel.Name));
            
            var grid = new Grid()
            {
                Margin = 20
            };
            grid.Children.Add(image);
            grid.Children.Add(lockImage);
            grid.Children.Add(label);

            var gesture = new TapGestureRecognizer();
            gesture.SetBinding(TapGestureRecognizer.CommandProperty, nameof(RewardChildViewModel.Command));
            grid.GestureRecognizers.Add(gesture);
            
            Content = grid;
        }
    }
    
    
}