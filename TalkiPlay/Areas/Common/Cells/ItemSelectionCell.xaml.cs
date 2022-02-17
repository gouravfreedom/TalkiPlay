using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class ItemSelectionCell : ReactiveBaseViewCell<ItemSelectionViewModel>
    {
        public ItemSelectionCell()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Label, v => v.Label.Text).DisposeWith(d); 
            });
        }
    }
}
