using System;
using ChilliSource.Mobile.UI;
using EasyLayout.Forms;
using FFImageLoading.Svg.Forms;
using TalkiPlay.Shared;
using Xamarin.Forms;
using DynamicData;
using FormsControls.Base;

namespace TalkiPlay
{
    public class GuideImagePage : SimpleBasePage<GuideImagePageViewModel>
    {
        const int TitleHeight = 120;
        public GuideImagePage()
        {
            BuildContent();
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
                
                Padding = Dimensions.NavPadding(barHeight)
            };

            navView.SetBinding(NavigationView.IsVisibleProperty, nameof(ViewModelType.ShowNavBar));
            navView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(ViewModelType.BackCommand));

            
            //lblLogin.Font = lblLogin.Font.WithSize(17f);
            
            var titleLabel = new Label()
            {
                FontFamily = Fonts.HeroTitleFont.Family,
                TextColor = Fonts.HeroTitleFont.Color,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                //AdjustFontSizeToFit = true,
            };
            titleLabel.SetBinding(Label.TextProperty, nameof(ViewModelType.HeaderText));
            titleLabel.SetBinding(Label.FontSizeProperty, nameof(ViewModelType.HeaderTextFontSize));
            
            var image = new SvgCachedImage()
            {
                Aspect = Aspect.AspectFit,
            };
            image.SetBinding(SvgCachedImage.SourceProperty, nameof(ViewModelType.ImageSource));

            var stops = new GradientStopCollection();
            stops.Add(new GradientStop() { Color = Color.FromHex("#23bcba"), Offset = 0.1f });
            stops.Add(new GradientStop() { Color = Color.FromHex("#45e994"), Offset = 1.0f });

            var nextButtonFrame = new Frame()
            {
                HasShadow = false,
                BackgroundColor = Color.Transparent,
                Padding = new Thickness(20, 0),
                Margin = new Thickness(20, 0),
                CornerRadius = 25,
                Background = new LinearGradientBrush(stops)
            };

            var button = new Button
            {
                Style = Styles.NewPrimaryButtonStyle,
                Text = "Next",
                HorizontalOptions = LayoutOptions.Fill
            };
            nextButtonFrame.Content = button;

            //var button = new Button
            //{         
            //    Style = Styles.PrimaryButtonStyle,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //};
            button.SetBinding(Button.TextProperty, nameof(ViewModelType.NextButtonText));
            button.SetBinding(Frame.IsVisibleProperty, nameof(ViewModelType.ShowNextButton));
            button.SetBinding(Button.CommandProperty, nameof(ViewModelType.NextCommand));

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
                 titleLabel.Right() == mainLayout.Right() - Dimensions.DefaultHorizontalMargin &&
                 titleLabel.Left() == mainLayout.Left() + Dimensions.DefaultHorizontalMargin &&
                 titleLabel.Top() == navView.Bottom() + 10 //&&
                 //titleLabel.Height() == TitleHeight
             );

            var buttonBottomOffset = bottomOffset + Dimensions.DefaultSpacing;

            var imageBottomOffset = buttonBottomOffset + Dimensions.DefaultButtonHeight + Dimensions.DefaultSpacing;
            var imageTopOffset = totalNavBarHeight + 10 + 20 + TitleHeight;

            var imageHeightOffset = imageBottomOffset + imageTopOffset;

            mainLayout.ConstrainLayout(() =>
                nextButtonFrame.Right() == mainLayout.Right() - Dimensions.DefaultHorizontalMargin &&
                nextButtonFrame.Left() == mainLayout.Left() + Dimensions.DefaultHorizontalMargin &&
                nextButtonFrame.Bottom() == mainLayout.Bottom() - buttonBottomOffset.ToConst() &&
                nextButtonFrame.Height() == Dimensions.DefaultButtonHeight
            );
            
            mainLayout.ConstrainLayout(() =>
                 image.Right() == mainLayout.Right() - Dimensions.DefaultHorizontalMargin &&
                 image.Left() == mainLayout.Left() + Dimensions.DefaultHorizontalMargin &&
                 image.Top() == titleLabel.Bottom() + 20 &&
                 //image.Bottom() == button.Top() - 20
                 image.Height() == mainLayout.Height() - imageHeightOffset.ToConst()                                 
             );
            
            Content = mainLayout;            
        }
    }
}