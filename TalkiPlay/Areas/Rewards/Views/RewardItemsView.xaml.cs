using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class RewardItemsView : ReactiveContentView<RewardItemsViewModel>
    {
        public RewardItemsView()
        {
            InitializeComponent();
        }
    }
}
