using ChilliSource.Mobile.UI;
using ChilliSource.Mobile.UI.ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class OnboardingPage : SimpleBasePage
    {
        public OnboardingPage()
        {
            InitializeComponent();
    
            this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

            carouselView.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == CarouselView.PositionProperty.PropertyName)
                {
                    var model = this.BindingContext as OnboardingPageViewModel;
                    if (indicatorView.Position == indicatorView.Count - 1)
                    {
                        model.GotoNextState().Forget();
                    }
                }                
            };

            var service = Locator.Current.GetService<IApplicationService>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = Device.RuntimePlatform == Device.iOS ? (int)service.StatusbarHeight : 0;
                var navHeight = (int)service.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = totalHeight;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);

                int bottomOffset = (int)service.GetSafeAreaInsets(false).Bottom;
                MainLayout.Padding = new Thickness(0, 0, 0, 15 + bottomOffset);
            });

            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);

            NavigationView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(OnboardingPageViewModel.BackCommand));
        }       
    }

    public class ObCarouselView : CarouselView
    {
        public ObCarouselView()
        {

        }

        public new void SendScrolled(ItemsViewScrolledEventArgs e)
        {
            base.SendScrolled(e);
        }

        public new void SendRemainingItemsThresholdReached()
        {
            base.SendRemainingItemsThresholdReached();
        }
        protected override void OnScrollToRequested(ScrollToRequestEventArgs e)
        {
            base.OnScrollToRequested(e);
        }
    }

    public class ObCarouselItemTemplateSelector : DataTemplateSelector
    {
        private DataTemplate _templateImgView;
        private DataTemplate _templateAnimView;

        public ObCarouselItemTemplateSelector()
        {
            _templateImgView = new DataTemplate(typeof(OnboardingImageView));
            _templateAnimView = new DataTemplate(typeof(OnboardingAnimView));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var model = (OnboardingItemViewModel)item;

            if (!string.IsNullOrEmpty(model.Resource) && model.Resource.EndsWith(".json"))
            {
                return _templateAnimView;                
            }
            else
            {
                return _templateImgView;
            }
        }
    }
}
