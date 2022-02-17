using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using FormsControls.Base;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class TagItemStartPage  : BasePage<ITagItemStartPageViewModel>, IAnimationPage
    {
        public TagItemStartPage()
        {
            InitializeComponent();
            
            this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

            var service = Locator.Current.GetService<IApplicationService>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS ? (int) service.StatusbarHeight : 0;
                var navHeight = (int) service.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = totalHeight;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);

            });
            
            
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);
            
            //Image.Transformations.Add(new RoundedTransformation(4));

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(m => m.ViewModel.LoadDataCommand)
                    .Select(m => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.LoadDataCommand)
                    .DisposeWith(d);
                
                this.BindCommand(ViewModel, v => v.BackCommand, view => view.CancelButton.Button).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Heading, view => view.Heading.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.SubHeading, view => view.SubHeading.Text).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.BeginCommand, view => view.GoButton.Button).DisposeWith(d);
                
                this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);

        
            });
        }
        
        public void OnAnimationStarted(bool isPopAnimation)
        {
           
        }

        public void OnAnimationFinished(bool isPopAnimation)
        {
            
        }

        public IPageAnimation PageAnimation => this.ViewModel?.PageAnimation ?? new SlidePageAnimation()
        {
            BounceEffect = false,
            Subtype = AnimationSubtype.FromRight,
            Duration = AnimationDuration.Short
        };
    }
}
