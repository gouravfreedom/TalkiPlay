using System;
using System.Collections.Generic;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System.Reactive.Disposables;
using ReactiveUI;
using System.Linq;
using RssFeedInfo = TalkiPlay.Services.Utility.RssImageFetcher.RssFeedInfo;
using FFImageLoading.Svg.Forms;
using TalkiPlay.Managers;

namespace TalkiPlay
{
    public partial class CategoryListPage : TabViewBase<CategoryListPageViewModel>
    {
        public CategoryListPage()
        {
            InitializeComponent();

            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = DeviceInfo.StatusbarHeight;
                var navHeight = (int)DeviceInfo.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = barHeight;
                //NavigationView.Padding = Dimensions.NavPadding(barHeight);
            });
        }

        private void OnRssImageTapped(object sender, EventArgs e)
        {
            var view = sender as View;
            var bc = view?.BindingContext as RssFeedInfo;
            if (bc != null)
            {
                WebpageHelper.OpenUrl(bc.Link, bc.Title);
            }
        }

        private void OnGameSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var vm = BindingContext as CategoryListPageViewModel;
            var view = sender as CollectionView;
            if (vm != null && view != null && view.SelectedItem != null)
            {
                vm.SelectGameCommand.Execute(view.SelectedItem as IGame);
                view.SelectedItem = null;
            }
        }


        public override void AboutToAppear()
        {
            base.AboutToAppear();
            var vm = BindingContext as CategoryListPageViewModel;
            vm?.LoadDataOnAppear();
        }
    }

}
