using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using FormsControls.Base;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class FirmwareUpdatePage : BasePage<FirmwareUpdatePageViewModel>, IAnimationPage
    {
        public FirmwareUpdatePage()
        {
            InitializeComponent();
            var service = Locator.Current.GetService<IApplicationService>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = Device.RuntimePlatform == Device.iOS ? (int)service.StatusbarHeight : 0;
                var navHeight = (int)service.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = totalHeight;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);

            });


            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(m => m.ViewModel.CommandCheckUpdate)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.CommandCheckUpdate)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);

                this.OneWayBind(ViewModel, v => v.IsCheckingUpdate, view => view.gridCheckingUpdate.IsVisible).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.IsNoUpdateAvailable, view => view.gridNoUpdate.IsVisible).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.IsDeviceNotReady, view => view.gridDeviceNotReady.IsVisible).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.DeviceNotReadyTitle, view => view.lblDeviceNotReadyTitle.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.DeviceNotReadyMessage, view => view.lblDeviceNotReadyMessage.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.DeviceNotReadyAndConnected, view => view.svgImgDeviceConnected.IsVisible).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.DeviceNotReadyAndDisconnected, view => view.svgImgDeviceDisconnected.IsVisible).DisposeWith(d);

                this.OneWayBind(ViewModel, v => v.IsUpdateAvailable, view => view.gridUpdateAvailable.IsVisible).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.IsReadyToUpdate, view => view.gridReadyToUpdate.IsVisible).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.IsUpdating, view => view.gridUpdating.IsVisible).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.IsUpdateSuccess, view => view.gridUpdateSuccess.IsVisible).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.IsUpdateFailed, view => view.gridUpdateFailed.IsVisible).DisposeWith(d);

                this.BindCommand(ViewModel, v => v.CommandNoUpdateRequire, view => view.btnNoUpdateRequire.Button).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.CommandDeviceNotReady, view => view.btnDeviceNotReady.Button).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.CommandUpdateAvailable, view => view.btnUpdateAvailable.Button).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.CommandReadyToUpdate, view => view.btnReadyToUpdate.Button).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.CommandUpdateFailed, view => view.btnUpdateFailed.Button).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.CommandUpdateSuccess, view => view.btnUpdateSuccess.Button).DisposeWith(d);

                this.OneWayBind(ViewModel, v => v.CurrentVersion, view => view.lblVersionChecking.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.CurrentVersion, view => view.lblVersionNoUpdate.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.NewVersion, view => view.lblVersionUpdateAvailable.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.NewVersion, view => view.lblVersionReadyToUpdate.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.NewVersion, view => view.lblVersionUpdateSuccess.Text).DisposeWith(d);
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
