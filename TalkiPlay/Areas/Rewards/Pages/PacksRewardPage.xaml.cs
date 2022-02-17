using System;
using System.Collections.Generic;
using System.Text;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TalkiPlay
{
    public partial class PacksRewardPage : TabViewBase<PacksRewardPageViewModel>
    {
        public PacksRewardPage()
        {
            InitializeComponent();

            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = DeviceInfo.StatusbarHeight;
                var navHeight = (int)DeviceInfo.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = barHeight;
            });
        }

        public override void AboutToAppear()
        {
            base.AboutToAppear();
            var vm = BindingContext as PacksRewardPageViewModel;
            vm?.LoadDataOnAppear(true);
        }

        private void OnPackSelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            if (packList.SelectedItem == null)
            {
                return;
            }
            
            var packVm = packList.SelectedItem as PackRewardViewModel;
            var vm = BindingContext as PacksRewardPageViewModel;
            vm.SelectedPack = packVm;
            packList.SelectedItem = null;

            SimpleNavigationService.PushAsync(new PackItemsRewardPage() { BindingContext = vm }).Forget();
        }

        private void OnFavGame1Tapped(System.Object sender, System.EventArgs e)
        {
            var vm = BindingContext as PacksRewardPageViewModel;
            if (vm?.FavoriteGame1 != null)
            {
                SimpleNavigationService.PushAsync(new GameConfigurationPageViewModel(vm.Navigator, vm.FavoriteGame1)).Forget();
            }
        }

        private void OnFavGame2Tapped(System.Object sender, System.EventArgs e)
        {
            var vm = BindingContext as PacksRewardPageViewModel;
            if (vm?.FavoriteGame2 != null)
            {
                SimpleNavigationService.PushAsync(new GameConfigurationPageViewModel(vm.Navigator, vm.FavoriteGame2)).Forget();
            }
        }
    }
        
}