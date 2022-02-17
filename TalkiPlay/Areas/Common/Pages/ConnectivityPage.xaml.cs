using System;
using System.Collections.Generic;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class ConnectivityPage : BasePopupPage<ConnectivityPageViewModel>
    {
        public ConnectivityPage()
        {
            InitializeComponent();
        }


        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }
    }
}
