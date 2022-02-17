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

namespace TalkiPlay
{
    public partial class PackItemsRewardPage : BasePage<PacksRewardPageViewModel>
    {
        public PackItemsRewardPage()
        {
            InitializeComponent();

            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = DeviceInfo.StatusbarHeight;
                var navHeight = (int)DeviceInfo.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = totalHeight;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);
            });

            this.WhenActivated(d =>
            {
                packPicker.SelectedItem = ViewModel?.SelectedPack;
                packPicker.Events()
                .SelectedIndexChanged
                .Select(m => this.packPicker.SelectedItem as PackRewardViewModel)
                .InvokeCommand(this.ViewModel, v => v.LoadOnePackCommand)
                .DisposeWith(d);
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var vm = BindingContext as PacksRewardPageViewModel;
            vm?.LoadDataOnAppear(false);
        }
    }
}
