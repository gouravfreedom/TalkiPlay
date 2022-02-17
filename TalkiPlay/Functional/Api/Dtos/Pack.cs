using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class PackDto : IPack
    {
        public PackDto()
        {
        }
        
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imagePath")]
        public string ImagePath { get; set; }
        
        [JsonProperty("items")]
        public IList<ItemDto> Items { get; set; }
        
        [JsonProperty("audioAssets")]
        public IList<AssetDto> AudioAssets { get; set; }
        
    }

    public class PackOnboardedDto
    {
        [JsonProperty("itemGroupId")]
        public int Id { get; set; }
        
        [JsonProperty("isOnboarded")]
        public bool IsOnboarded { get; set; }
    }
}