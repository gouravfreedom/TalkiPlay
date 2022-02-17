﻿using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public class LoadingPageViewModel : IPopModalViewModel
    {
        public string Title => "";

        [Reactive] 
        public string Message { get; set; } = " ";
        
        [Reactive]
        public bool OnBackgroundClicked { get; set; }
    }
}