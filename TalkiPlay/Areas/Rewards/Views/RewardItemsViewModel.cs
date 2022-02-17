﻿using System.Collections.Generic;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public class RewardItemsViewModel : ReactiveObject
    {
        public RewardItemsViewModel(List<RewardItemViewModel> items, int numberOfColumns)
        {
            RewardItems = items;
            NumberOfColumns = numberOfColumns;
        }

        //[Reactive]
        public List<RewardItemViewModel> RewardItems { get; }
        
        public int NumberOfColumns { get; }
    }
}