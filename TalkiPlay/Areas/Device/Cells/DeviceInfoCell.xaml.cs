using System.Reactive.Disposables;
using ChilliSource.Mobile.UI;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;

namespace TalkiPlay
{
    public partial class DeviceInfoItemView :  ReactiveContentView<DeviceInfoViewModel>
    {
        public DeviceInfoItemView()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Label, v => v.Label.Text).DisposeWith(d); 
                this.OneWayBind(ViewModel, vm => vm.Value, v => v.Value.Text).DisposeWith(d); 
                
            });
        }     
    }

    public class DeviceInfoItemViewCell : BaseCell
    {
        public DeviceInfoItemViewCell()
        {
            View = new DeviceInfoItemView();
        }
    }
}
