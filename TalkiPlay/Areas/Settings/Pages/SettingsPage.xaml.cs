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
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class SettingsPage  : TabViewBase<SettingsPageViewModel>
    {

        private SettingsViewTemplateSelector _templateSelector;
        public SettingsPage()
        {
            InitializeComponent();

            _templateSelector = new SettingsViewTemplateSelector();
            SettingsItemList.ItemTemplate = _templateSelector;

            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = DeviceInfo.StatusbarHeight;
                var navHeight = (int)DeviceInfo.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = barHeight;
                //NavigationView.Padding = Dimensions.NavPadding(barHeight);
            });

            this.WhenActivated(d => {
                this.OneWayBind(ViewModel, v => v.SettingItems, view => view.SettingsItemList.ItemsSource).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.LogoutCommand, view => view.LogoutButton.Button).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.IsBusy, view => view.LogoutButton.IsBusy).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.SelectCommand, view => view._templateSelector.SelectCommand).DisposeWith(d);
            });
        }
        
    }
}
