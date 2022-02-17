using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class TalkiPlayerInstructionItemView  : ReactiveContentView<TalkiPlayerInstructionItemViewModel>
    {
        public TalkiPlayerInstructionItemView()
        {
            InitializeComponent();
            
            
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, v => v.Header, view => view.Heading.Text).DisposeWith(d);
            });
        }
    }
}
