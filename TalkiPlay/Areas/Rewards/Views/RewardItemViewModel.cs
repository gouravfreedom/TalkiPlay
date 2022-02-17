﻿using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public class RewardItemViewModel : ReactiveObject
    {

        public RewardItemViewModel(ChildRewardDto reward)
        {
            var count = 0;
            
            if (reward != null)
            {
                count = reward.Count;

                if (reward.Asset != null)
                {
                    RewardImage = reward.Asset.ImageContentPath.ToResizedImage(44, 44);
                }
                else
                {
                    RewardImage = Images.RewardUnknownIcon.ToResizedImage(44, 44);;
                }
            }
            else
            {
                RewardImage = Images.RewardUnknownIcon.ToResizedImage(44, 44);
            }

            Opacity = count > 0 ? 1 : 0;
            BadgeCount = count > 0 ? $"{count}" : " ";  
        }
        
        // public RewardItemViewModel(RewardItem rewardItem)
        // {
        //     var count = 0;
        //     if (rewardItem != null)
        //     {
        //         RewardImage = rewardItem.Asset.ImageContentPath.ToResizedImage(44, 44);
        //         count = rewardItem.NumberOfRewardCollected;
        //     }
        //     else
        //     {
        //         RewardImage = Images.RewardUnknownIcon.ToResizedImage(44, 44);;
        //     }
        //
        //     Opacity = count > 0 ? 1 : 0;
        //     BadgeCount = count > 0 ? $"{count}" : " ";
        //
        //     // this.WhenAnyValue(m => m.Count)
        //     //     .Select(m => m > 0 ? 1 : 0)
        //     //     .ToPropertyEx(this, m => m.Opacity);
        //     //
        //     // this.WhenAnyValue(m => m.Count)
        //     //     .Select(m => m > 0 ? $"{m}" : " ")
        //     //     .ToPropertyEx(this, m => m.BadgeCount);
        // }
        
        //[Reactive]
        public string RewardImage { get; set; }

        public int Opacity { get; }
        
        // [Reactive] 
        // private int Count { get; }
        
        public string BadgeCount { get; }
    }
}