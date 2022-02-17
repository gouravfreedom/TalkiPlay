using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class DeviceItemCell : ReactiveBaseViewCell<DeviceItemViewModel>
    {
        public DeviceItemCell()
        {
            InitializeComponent();
           
            this.WhenActivated(d =>
            {
               // this.OneWayBind(ViewModel, vm => vm.Name, v => v.Name.Text).DisposeWith(d); 
               // this.OneWayBind(ViewModel, vm => vm.Icon, v => v.Icon.Source).DisposeWith(d); 
                
            });
        }
    }
}
