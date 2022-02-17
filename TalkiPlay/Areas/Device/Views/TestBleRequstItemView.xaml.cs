using System.Reactive.Disposables;
using ReactiveUI;
using TalkiPlay.Shared;


namespace TalkiPlay
{
    public partial class TestBleRequstItemView : ReactiveBaseViewCell<ItemSelectionViewModel>
    {
        public TestBleRequstItemView()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Label, v => v.Label.Text).DisposeWith(d); 
                 
            });
        }
    }
}
