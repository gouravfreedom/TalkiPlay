using System.Threading.Tasks;
using ChilliSource.Mobile.UI;
using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using Lottie.Forms;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Shapes;
using ZXing.Net.Mobile.Forms;

namespace TalkiPlay
{
    public class QRCodeGameSessionPage :  SimpleBasePage<QRCodeGameSessionPageViewModel>//, IAnimationPage
    {
        ZXingScannerView _scannerView;
        private bool _isFirstLoad = true;
        private readonly double _itemSize;
        private readonly double _windowSize;

        public QRCodeGameSessionPage()
        {
            BackgroundColor = Color.White;

            _itemSize = 200;
            _windowSize = Device.Idiom == TargetIdiom.Tablet ? 400 : DeviceInfo.ScreenSize.Height < 700 ? 240 : 300;
            
            BuildContent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _scannerView.IsScanning = true;

            if (_isFirstLoad)
            {
                ViewModel.StartAnimations().Forget();
                _isFirstLoad = false;
            }
        }
        
        protected override void OnDisappearing()
        {
            _scannerView.IsScanning = false;

            base.OnDisappearing();
        }

        void BuildContent()
        {
            var barHeight = DeviceInfo.StatusbarHeight;
            var navHeight = (int)DeviceInfo.NavBarHeight;
            var totalHeight = barHeight + navHeight;
            var bottomOffset = DeviceInfo.GetSafeAreaInsets(false).Bottom;

             var navView = new NavigationView
             {
                 BarTintColor = Color.Transparent,
                 ShowLeftButton = true,
                 ShowRightButton = false,
                 
                 
                 Padding = Dimensions.NavPadding(barHeight)
             };
            navView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(ViewModel.BackCommand));
            
            
            _scannerView = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center, 
                HeightRequest = _windowSize-40,
                WidthRequest = _windowSize-40,
                
            }.Bind(IsVisibleProperty, nameof(ViewModel.GameEnded), converter:new InvertedBooleanConverter());
            
            _scannerView.Options.UseNativeScanning = true;
            _scannerView.Options.DelayBetweenAnalyzingFrames = 5;
            _scannerView.Options.DelayBetweenContinuousScans = 5;
            _scannerView.OnScanResult += OnScanResult;

            var windowAnimationView = new AnimationView
            {
                Animation = Images.WindowOpenAnimation,
                Loop = false,
                Speed = 0.4f, 
                WidthRequest = _windowSize,
                HeightRequest = _windowSize
            }
                .Bind(AnimationView.IsPlayingProperty, nameof(ViewModel.WindowAnimationIsPlaying))
                .Bind(IsVisibleProperty, nameof(ViewModel.GameEnded), converter:new InvertedBooleanConverter());

            var itemImage = new Image()
                {
                    WidthRequest = _itemSize,
                    HeightRequest = _itemSize,
                    HorizontalOptions = LayoutOptions.Center,
                    Aspect = Aspect.AspectFit,
                }
                .Bind(Image.SourceProperty, nameof(ViewModel.CurrentItemImageSource))
                .Bind(ClipProperty, nameof(ViewModel.CurrentItemImageClipGeometry))
                .Bind(IsVisibleProperty, nameof(ViewModel.CurrentItemImageIsVisible));
            
            var itemAnimation = new ExtendedAnimationView
                {
                    Loop = false,
                    Speed = 1f,
                    WidthRequest = _itemSize,
                    HeightRequest = _itemSize
                }
                .Bind(AnimationView.AnimationProperty, nameof(ViewModel.ItemAnimationSource))
                .Bind(ExtendedAnimationView.JsonSourceProperty, nameof(ViewModel.ItemAnimationJsonSource))
                .Bind(AnimationView.IsPlayingProperty, nameof(ViewModel.ItemAnimationIsPlaying))
                .Bind(IsVisibleProperty, nameof(ViewModel.ItemAnimationIsVisible))
                .Bind(AnimationView.PlaybackFinishedCommandProperty, nameof(ViewModel.ItemAnimationCompletedCommand));

            var centerLayout = new StackLayout()
            {
                Spacing = 20,
                Margin = new Thickness(20,0),
                Children =
                {
                    new ExtendedLabel
                    {
                        CustomFont = Fonts.Header1Font,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Margin = new Thickness(20,20,20,0)
                    }
                        .Bind(Label.TextProperty, nameof(ViewModel.GameTitle))
                        .Bind(IsVisibleProperty, nameof(ViewModel.GameEnded), converter:new InvertedBooleanConverter())
                        .BindTapGesture(nameof(ViewModel.NextItemDebugCommand))
                    ,
                    new Image()
                    {
                        Source = Icons.LargeGoldStarIcon
                    }.Bind(IsVisibleProperty,nameof(ViewModel.ShowLargeStar)),
                    new Grid()
                    {
                        Margin = new Thickness(0,20,0,0),
                        Children =
                        {
                            itemImage,
                            itemAnimation,
                        }
                    },
                }
            };
            
            var starLayout = new FlexLayout()
                {
                    Margin = new Thickness(20,0, 20, 20 + bottomOffset),
                    HeightRequest = 70,
                    Wrap = FlexWrap.Wrap,
                    JustifyContent = FlexJustify.Center
                }
                .Bind(Button.IsVisibleProperty, nameof(ViewModel.GameEnded), converter:new InvertedBooleanConverter())
                .Bind(BindableLayout.ItemsSourceProperty, nameof(ViewModel.StarItems))
                .BindTapGesture(nameof(ViewModel.EndGameDebugCommand))
                .Invoke(l => BindableLayout.SetItemTemplate(l, new DataTemplate(() =>
                {
                    return new Image()
                        {
                            Margin = new Thickness(5)
                        }
                        .Bind(Image.SourceProperty, nameof(StarItem.IsCollected), converter:Converters.BooleanToStarIconConverter);
                })));

            var collectRewardButton = new Button()
            {
                Style = Styles.PrimaryButtonStyle,
                Text = "Collect Reward", 
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(20,0, 20, 20 + bottomOffset),
            }
                .Bind(Button.IsVisibleProperty, nameof(ViewModel.GameEnded))
                .BindCommand(nameof(ViewModel.CollectRewardCommand));
            
            var fullScreenAnimation = new AnimationView
                {
                    Loop = false,
                    Speed = 1f,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    // WidthRequest = DeviceInfo.ScreenSize.Width,
                    // HeightRequest = DeviceInfo.ScreenSize.Height,
                    InputTransparent = true,
                }
                .Bind(AnimationView.AnimationProperty, nameof(ViewModel.FullScreenAnimationSource))
                .Bind(AnimationView.ProgressProperty, nameof(ViewModel.FullScreenAnimationProgress))
                .Bind(AnimationView.IsPlayingProperty,
                    nameof(ViewModel.FullScreenAnimationIsPlaying))
                .Bind(IsVisibleProperty, nameof(ViewModel.FullScreenAnimationIsVisible))
                .Bind(AnimationView.PlaybackFinishedCommandProperty,
                    nameof(ViewModel.FullScreenAnimationCompletedCommand));
                
            var grid = new Grid()
            {
                RowSpacing = 10,
            };

            grid.RowDefinitions.Add(new RowDefinition() { Height = totalHeight });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });            
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            
            var backgroundImage = new CachedImage()
            {
                Source = Images.GameBackgroundImage,
                Aspect = Aspect.AspectFill
            };
            
            grid.Children.Add(backgroundImage);
            Grid.SetRowSpan(backgroundImage,4);
            
            grid.Children.Add(navView);
            
            //#if !DEV || __IOS__
            grid.Children.Add(_scannerView, 0, 1);
            //#endif
            
            grid.Children.Add(windowAnimationView, 0, 1);
            grid.Children.Add(centerLayout, 0, 2);           
            grid.Children.Add(starLayout, 0, 3);
            grid.Children.Add(collectRewardButton, 0, 3);

            grid.Children.Add(fullScreenAnimation);
            Grid.SetRowSpan(fullScreenAnimation,4);
            
            Content = grid;            
        }

        private void OnScanResult(ZXing.Result result)
        {       
            System.Diagnostics.Debug.WriteLine("OnScanResult: " + result.Text);
            
            Task.Run(async () =>
            {
                 Device.BeginInvokeOnMainThread(() =>
                 {
                     _scannerView.IsAnalyzing = false;
                 });
                
                 await ViewModel.ProcessQRCode(result.Text);
                
                 Device.BeginInvokeOnMainThread(() =>
                 {
                     _scannerView.IsAnalyzing = true;
                 });
            });                                 
        }
    }
}

