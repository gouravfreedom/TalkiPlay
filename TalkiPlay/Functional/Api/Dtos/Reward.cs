using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class RewardDto : IReward
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("openImageAssetId")]
        public int OpenImageAssetId { get; set; }
        
        [JsonProperty("collectImageAssetId")]
        public int CollectImageAssetId { get; set; }

        [JsonProperty("percentage")]
        public double Percentage { get; set; }
    }

    public class ChildRewardDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("collectImageAssetId")]
        public int CollectImageAssetId { get; set; }
        
        [JsonProperty("count")]
        public int Count { get; set; }
        
        [JsonIgnore]
        public IAsset Asset { get; set; }
    }

}