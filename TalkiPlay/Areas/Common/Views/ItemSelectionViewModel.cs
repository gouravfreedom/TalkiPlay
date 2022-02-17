﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public class ItemSelectionViewModel : ReactiveObject
    {
        public ItemSelectionViewModel()
        {
            this.WhenAnyValue(m => m.Selected)
                .Select(m => m ? Images.SelectedIcon : Images.UnSelectedIcon)
                .ToPropertyEx(this, x => x.Icon);
        }
        public object Source { get; set; }
     
        [Reactive]
        public string Label { get; set; }

        public T Get<T>() => (T) Source;
        
        public extern string Icon { [ObservableAsProperty]get;}
        
        [Reactive]
        public bool Selected { get; set; }
    }

    public class EmptyItemSelectionViewModel : ItemSelectionViewModel, IEmptyItemViewModel
    {
        [Reactive] public double Height { get; set; } = 220;
    }

    public interface IEmptyItemViewModel
    {
        double Height { get;  }
    }
}
        
