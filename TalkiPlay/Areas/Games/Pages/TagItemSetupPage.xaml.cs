using System;
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
using ChilliSource.Mobile.UI.ReactiveUI;

namespace TalkiPlay
{

    public partial class TagItemSetupPage : BasePage<ITagItemSetupPageViewModel>, IAnimationPage
    {
     
        public TagItemSetupPage()
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

          //  Image.Transformations.Add(new RoundedTransformation(4));

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(m => m.ViewModel.LoadCommand)
                    .Select(m => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.LoadCommand)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, v => v.Heading, view => view.Heading.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.SubHeading, view => view.SubHeading.Text).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.GoCommand, view => view.GoButton).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.SkipCommand, view => view.SkipButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.ShowGoButton, view => view.GoButtonView.IsVisible).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.ShowSkipButton, view => view.SkipButtonView.IsVisible).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Message, view => view.Message.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.SkipButtonText, view => view.SkipButton.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.GoButtonText, view => view.GoButton.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.NavigationTitle, view => view.NavigationView.Title).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);


                // this.WhenAnyValue(m => m.ViewModel.ShowTabsHelp)
                //     .ObserveOn(RxApp.MainThreadScheduler)
                //     .Where(m => m)
                //     .SubscribeSafe(async m =>
                //     {
                //         await Navigation.PushModalAsync(TagItemCollectionPageHelper.GetTabbedPage(this.ViewModel));
                //     })
                //     .DisposeWith(d);
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
