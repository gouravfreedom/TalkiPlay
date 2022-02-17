using System;
using System.Collections.Generic;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class LoadingPage : BasePopupPage<LoadingPageViewModel>
    {
        public LoadingPage()
        {
            InitializeComponent();
        }
        
        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        protected override bool OnBackgroundClicked()
        {
            var isBackground = ViewModel?.OnBackgroundClicked ?? true;
            return isBackground;
        }
    }
}
