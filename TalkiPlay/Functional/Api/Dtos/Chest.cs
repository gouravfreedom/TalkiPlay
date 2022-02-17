using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class ChestDto : IChest
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("rewards")]
        public IList<RewardDto> RewardList { get; set; }

        [JsonIgnore] 
        public IList<IReward> Rewards => RewardList?.ToList<IReward>();
    }
}