﻿using System.Reactive;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public class ConfirmationDialogPageViewModel : ReactiveObject, IPopModalViewModel
    {
        public string Title => "";

        [Reactive]
        public string MainTitle { get; set; }
        
        [Reactive]
        public string SubTitle { get; set; }
      
        [Reactive]
        public string NoButtonText { get; set; }
        
        [Reactive]
        public string YesButtonText { get; set; }
             
        public Interaction<Unit, bool> ConfirmationInteraction = new Interaction<Unit, bool>();
    }
}