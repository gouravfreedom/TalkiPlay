using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;

namespace TalkiPlay
{
    public partial class PlayerView : ReactiveContentView<ChildPlayerViewModel>
    {
        public PlayerView()
        {
            InitializeComponent();
            
            // this.WhenActivated(d =>
            // {
            //     this.OneWayBind(ViewModel, v => v.Name, view => view.Name.Text).DisposeWith(d);
            //     this.BindCommand(ViewModel, v => v.RemoveCommand, view => view.RemoveButton).DisposeWith(d);
            // });
        }
    }
}
