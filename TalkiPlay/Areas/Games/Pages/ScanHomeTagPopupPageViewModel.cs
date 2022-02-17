using System;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace TalkiPlay.Shared
{
    public class ScanHomeTagPopupPageViewModel : ReactiveObject, IPopModalViewModel
    {
        public string Title => "";

        [Reactive]
        public string Message { get; set; }
        
        [Reactive]
        public string Image { get; set; }

    }
}