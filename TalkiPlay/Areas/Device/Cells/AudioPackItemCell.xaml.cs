using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;
using ChilliSource.Mobile.UI.ReactiveUI;

namespace TalkiPlay
{
    public partial class AudioPackItemCell :  ReactiveBaseViewCell<AudioPackItemViewModel>
    {
        public AudioPackItemCell()
        {
            InitializeComponent();
        }
    }
}
