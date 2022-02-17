using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class TalkiPlayerView : ReactiveContentView<TalkiPlayerViewModel>
    {
        public TalkiPlayerView()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, v => v.Name, view => view.Name.Text).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.RemoveCommand, view => view.RemoveButton).DisposeWith(d);
            });
        }
    }
}
