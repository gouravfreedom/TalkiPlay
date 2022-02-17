using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using FormsControls.Base;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class RewardsChildListPage : TabViewBase<RewardsChildListPageViewModel>// BasePage<RewardsChildListPageViewModel>//, IAnimationPage
    {
        public RewardsChildListPage()
        {
            InitializeComponent();
            
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = DeviceInfo.StatusbarHeight;
                var navHeight = DeviceInfo.NavBarHeight;
                var totalHeight = barHeight + navHeight;

                NavigationView.Padding = Dimensions.NavPadding(barHeight);
                NavRow.Height = totalHeight;
            });
        }

        public override void AboutToAppear()
        {            
            if (!IsAppearedOnce)
            {
                ViewModel.LoadDataCommand.Execute(Unit.Default);
            }

            base.AboutToAppear();
        }
    }
}