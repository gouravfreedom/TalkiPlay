using ChilliSource.Mobile.UI;
using EasyLayout.Forms;
using FFImageLoading.Svg.Forms;
using TalkiPlay.Shared;
using Xamarin.Forms;
using DynamicData;
using FormsControls.Base;

namespace TalkiPlay
{
    public class OnboardingImagePage : SimpleBasePage
    {                 
        public OnboardingImagePage()
        {            
            if (Device.RuntimePlatform == Device.iOS)
            {
                ViewDidLoad += BuildContent;
            }
            else
            {
                BuildContent();
            }
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
                //HeightRequest = totalHeight,
                
                Padding = Dimensions.NavPadding(barHeight)
            };            
            //navView.Effects.Add(new SafeAreaPaddingEffect());

            navView.SetBinding(NavigationView.IsVisibleProperty, nameof(OnboardingImagePageViewModel.ShowNavBar));
            navView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(OnboardingImagePageViewModel.BackCommand));

            var image = new SvgCachedImage()
            {
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                Aspect = Aspect.AspectFit,                
                //HeightRequest = 250,
                //WidthRequest = 250
            };
            image.SetBinding(SvgCachedImage.SourceProperty, nameof(OnboardingImagePageViewModel.ImageSource));

            var label = new ExtendedLabel
            {                
                CustomFont = Fonts.Header1Font,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            label.SetBinding(Label.TextProperty, nameof(OnboardingImagePageViewModel.HeaderText));

            
            var button = new Button
            {                
                Style = Styles.PrimaryButtonStyle,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            button.SetBinding(Button.TextProperty, nameof(OnboardingImagePageViewModel.ButtonText));
            button.SetBinding(Button.CommandProperty, nameof(OnboardingImagePageViewModel.NextCommand));

            var gradient = Styles.BuildMainGradient();
            

            var mainLayout = new RelativeLayout();

            mainLayout.ConstrainLayout(() =>
                  gradient.Right() == mainLayout.Right() &&
                  gradient.Left() == mainLayout.Left() &&
                  gradient.Top() == mainLayout.Top() &&
                  gradient.Height() == mainLayout.Bottom()
              );

            mainLayout.ConstrainLayout(() =>
                   navView.Right() == mainLayout.Right() &&
                   navView.Left() == mainLayout.Left() &&
                   navView.Top() == mainLayout.Top() &&
                   navView.Height() == totalNavBarHeight.ToConst()
               );

            mainLayout.ConstrainLayout(() =>
                 label.Right() == mainLayout.Right() - 20 &&
                 label.Left() == mainLayout.Left() + 20 &&
                 label.Top() == navView.Bottom() + 20
             );

            var buttonBottom = bottomOffset + 20;

            var imageBottomOffset = buttonBottom + 60 + 20;
            var imageTopOffet = totalNavBarHeight + 20;

            var imageHeightOffset = imageBottomOffset + imageTopOffet + 100;

            mainLayout.ConstrainLayout(() =>
                 image.Right() == mainLayout.Right() - 20 &&
                 image.Left() == mainLayout.Left() + 20 &&
                 image.Top() == label.Bottom() + 40 && 
                 image.Height() == mainLayout.Height() - imageHeightOffset.ToConst()                                 
             );

            
            mainLayout.ConstrainLayout(() =>
                  button.Right() == mainLayout.Right() - 20 &&
                  button.Left() == mainLayout.Left() + 20 &&
                  button.Bottom() == mainLayout.Bottom() - buttonBottom.ToConst() &&
                  button.Height() == 60
              );
                                 
            Content = mainLayout;            
        }

        // public void OnAnimationStarted(bool isPopAnimation)
        // {
        //
        // }
        //
        // public void OnAnimationFinished(bool isPopAnimation)
        // {
        //
        // }
        //
        // public IPageAnimation PageAnimation => new SlidePageAnimation()
        // {
        //     BounceEffect = false,
        //     Subtype = AnimationSubtype.FromRight,
        //     Duration = AnimationDuration.Short
        // };

    }
}

