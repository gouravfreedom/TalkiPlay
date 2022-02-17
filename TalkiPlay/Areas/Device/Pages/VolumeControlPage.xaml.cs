using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EasyLayout.Forms;
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
    public partial class VolumeControlPage : BasePage<VolumeControlPageViewModel>,
        IAnimationPage
    {

        public VolumeControlPage()
        {
            InitializeComponent();

           this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

            var service = Locator.Current.GetService<IApplicationService>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = Device.RuntimePlatform == Device.iOS ? (int)service.StatusbarHeight : 0;
                var navHeight = (int)service.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                //var size = service.ScreenSize;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);

                MainLayout.ConstrainLayout(() =>
                    NavigationView.Right() == MainLayout.Right() &&
                    NavigationView.Left() == MainLayout.Left() &&
                    NavigationView.Top() == MainLayout.Top() &&
                    NavigationView.Height() == totalHeight.ToConst()
                );

                var listTop = totalHeight;

                MainLayout.ConstrainLayout(() =>
                    SubLayout.Right() == MainLayout.Right() &&
                    SubLayout.Left() == MainLayout.Left() &&
                    SubLayout.Top() == MainLayout.Top() + listTop.ToConst() &&
                    SubLayout.Bottom() == MainLayout.Bottom()
                );

                int bottomOffset = (int)service.GetSafeAreaInsets(false).Bottom;
                int buttonHeight = 60+ bottomOffset;

                MainLayout.ConstrainLayout(() =>
                    ChangeButton.Right() == MainLayout.Right() &&
                    ChangeButton.Left() == MainLayout.Left() &&
                    ChangeButton.Bottom() == MainLayout.Bottom() &&
                    ChangeButton.Height() == buttonHeight.ToConst()
                );

                ChangeButton.ContentPadding = bottomOffset > 0 ? new Thickness(0, 0, 0, 15) : new Thickness();                
                ChangeButton.ButtonHeightRequest = buttonHeight;
            });


            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);
        
            this.WhenActivated(d =>
            {
                this.WhenAnyValue(m => m.ViewModel.LoadCommand)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.LoadCommand)
                    .DisposeWith(d);
                
                this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.UpdateCommand, view => view.ChangeButton.Button).DisposeWith(d);
                this.Bind(ViewModel, v => v.Volume, view => view.VolumeControl.Value);

                this.WhenAnyValue(m => m.ViewModel.Volume)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .SubscribeSafe(a => { VolumeValue.Text = $"{(int) a}";})
                    .DisposeWith(d);
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
            safeInsets.Top = 0;
            Padding = safeInsets;
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
            Subtype = AnimationSubtype.FromBottom,
            Duration = AnimationDuration.Short
        };
    }
}
