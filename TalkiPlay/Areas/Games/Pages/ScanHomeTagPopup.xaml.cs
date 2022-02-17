using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class ScanHomeTagPopup : BasePopupPage<ScanHomeTagPopupPageViewModel>
    {
        public ScanHomeTagPopup()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, v => v.Message, view => view.Heading.Text).DisposeWith(d);
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
