using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using FFImageLoading.Transformations;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class ChildCell : ReactiveContentView<ChildViewModel>
    {
        public ChildCell()
        {
            InitializeComponent();
            this.Photo.Transformations.Add(new CircleTransformation(10.0, Colors.WhiteColor.ToHex()));
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.Name.Text).DisposeWith(d); 
                
            });
        }
    }
}
