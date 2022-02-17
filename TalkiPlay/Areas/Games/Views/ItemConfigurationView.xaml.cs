using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;

namespace TalkiPlay
{
    public partial class ItemConfigurationView : ReactiveContentView<ItemConfigurationViewModel>
    {
        public ItemConfigurationView()
        {
            InitializeComponent();
            
            // this.WhenActivated(d =>
            // {
            //     this.OneWayBind(ViewModel, vm => vm.Name, v => v.Name.Text).DisposeWith(d); 
            //     this.Bind(ViewModel, vm => vm.IsEnabled, v => v.ItemOnOff.IsToggled).DisposeWith(d);
            // });
        }
    }
}
