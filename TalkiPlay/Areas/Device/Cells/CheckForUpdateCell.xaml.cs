using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class CheckForUpdateCell :  ReactiveBaseViewCell<CheckForUpdateViewModel>
    {
        public CheckForUpdateCell()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
               this.OneWayBind(ViewModel, vm => vm.Label, v => v.Label.Text).DisposeWith(d); 
                
            });
        }
    }
}
