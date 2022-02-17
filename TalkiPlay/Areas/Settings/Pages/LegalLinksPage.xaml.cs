using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class LegalLinksPage : BasePage<LegalLinksPageViewModel>
    {
        private SettingsViewTemplateSelector _templateSelector;

        public LegalLinksPage()
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

            _templateSelector = new SettingsViewTemplateSelector();
            SettingsItemList.ItemTemplate = _templateSelector;

            this.WhenActivated(d => {
                this.OneWayBind(ViewModel, v => v.Items, view => view.SettingsItemList.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.SelectCommand, view => view._templateSelector.SelectCommand).DisposeWith(d);
            });
        }
    }
}
