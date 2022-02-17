using System;
using System.Collections.Generic;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class TestGamePage : BasePage<TestGamePageViewModel>
    {
        public TestGamePage()
        {
            InitializeComponent();

            //var service = Locator.Current.GetService<IApplicationService>();
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    var barHeight = Device.RuntimePlatform == Device.iOS ? (int)service.StatusbarHeight : 0;
            //    var navHeight = (int)service.NavBarHeight;
            //    var totalHeight = barHeight + navHeight;
            //    //NavRow.Height = totalHeight;
            //    NavigationView.Padding = Dimensions.NavPadding(barHeight);

            //});
        }
    }
}
