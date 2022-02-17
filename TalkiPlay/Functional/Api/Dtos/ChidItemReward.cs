using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class ChildItemRewardDto : IChildItemReward
    {
        [JsonProperty("itemId")]
        public int ItemId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("failureCount")]
        public int FailureCount { get; set; }

        public bool IsSuccess()
        {
            return Status?.ToLower() == "success";
        }
    }

    public class ChildItemRewardsDto : IChildItemRewards
    {
        [JsonProperty("durationInSeconds")]
        public int DurationInSeconds { get; set; }

        [JsonProperty("items")]
        public IList<ChildItemRewardDto> Items { get; set; }
    }

    public class ChildItemProgressDto : IChildItemProgress
    {
        [JsonProperty("itemId")]
        public int Id { get; set; }


        [JsonProperty("star")]
        public double Star { get; set; }
    }

    public class ChildPackProgressDto : IChildPackProgress
    {
        [JsonProperty("packId")]
        public int Id { get; set; }

        [JsonProperty("packName")]
        public string Name { get; set; }

        [JsonProperty("progressPercentage")]
        public double Progress { get; set; }

        [JsonProperty("totalMinutesPlayed")]
        public double TotalMinutesPlayed { get; set; }
    }
}
