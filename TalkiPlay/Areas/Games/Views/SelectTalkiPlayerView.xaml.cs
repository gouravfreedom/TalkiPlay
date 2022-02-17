using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class SelectTalkiPlayerView : ReactiveContentView<SelectTalkiPlayerViewModel>
    {
        public SelectTalkiPlayerView()
        {
            InitializeComponent();

            
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, v => v.SelectCommand, view => view.AddButton).DisposeWith(d);
                
                this.WhenAnyObservable(v => v.ViewModel.SelectCommand.CanExecute)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .SubscribeSafe(m =>
                    {
                        this.Frame.BackgroundColor = m ? Color.Transparent :  Colors.WarmGrey;
                    })
                    .DisposeWith(d);
            });
        }
    }
}
