using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class RewardListPage : SimpleBasePage<RewardListPageViewModel>
    {
        public RewardListPage()
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
            
            NavigationView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(RewardListPageViewModel.BackCommand));
            
        }

        // protected override void OnAppearing()
        // {
        //     base.OnAppearing();
        //     this.ViewModel.LoadDataCommand.Execute(System.Reactive.Unit.Default);
        // }
        
    }
}
