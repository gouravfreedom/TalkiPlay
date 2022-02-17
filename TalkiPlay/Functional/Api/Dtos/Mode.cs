using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class ModeDto : IMode
    {
        [JsonProperty("type")]
        public InstructionModeType Type { get; set; }

        [JsonProperty("imageAssetId")]
        public int? ImageAssetId { get; set; }

        [JsonProperty("audioAssetId")]
        public int? AudioAssetId { get; set; }
        
        [JsonProperty("animationAssetId")]
        public int? AnimationAssetId { get; set; }
        
        [JsonProperty("text")]
        public string Text { get; set; }
    }
    
    public enum InstructionModeType
    {
        Setup = 1,
        Receptive,
        Expressive,
        Reward
    }

}