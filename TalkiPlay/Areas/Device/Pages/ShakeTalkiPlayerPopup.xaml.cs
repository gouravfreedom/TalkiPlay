using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class ShakeTalkiPlayerPopup : BasePopupPage<ShakeTalkiPlayerPopUpPageViewModel>
    {
        public ShakeTalkiPlayerPopup()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, v => v.Instructions, view => view.CarouselView.ItemsSource).DisposeWith(d);
            });
        }

        protected override bool OnBackgroundClicked()
        {
            return true;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
