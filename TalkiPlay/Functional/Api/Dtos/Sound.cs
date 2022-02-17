using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class SoundDto : ISound
    {
        [JsonProperty("type")]
        public SoundType Type { get; set; }
 
        [JsonProperty("sound")]
        public AssetDto AssetActual { get; set; }

        [JsonIgnore]
        public IAsset Asset => AssetActual;
    }
}