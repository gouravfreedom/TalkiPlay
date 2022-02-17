using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;

namespace TalkiPlay
{
    public partial class RoomCell : ReactiveContentView<RoomViewModel>
    {
        public RoomCell()
        {
            InitializeComponent();
            
           //this.WhenActivated(d =>
           // {                
           //     //this.OneWayBind(ViewModel, vm => vm.HeroTitle, v => v.HeroTitle.Text).DisposeWith(d);
           //  });
        }       
    }
}
