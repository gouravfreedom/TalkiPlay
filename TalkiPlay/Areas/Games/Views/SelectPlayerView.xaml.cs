using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class SelectPlayerView  : ReactiveContentView<SelectPlayerViewModel>
    {
        public SelectPlayerView()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                 this.BindCommand(ViewModel, v => v.SelectCommand, view => view.AddButton).DisposeWith(d);
            });
        }
    }
}
