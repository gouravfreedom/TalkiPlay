using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class ItemDto : IItem
    {
        public ItemDto()
        {
            AudioAssetIds = new List<int>();
            Assets = new List<AssetDto>();
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("qrCode")]
        public string QRCode { get; set; }        

        [JsonProperty("type")]
        public ItemType Type { get; set; }

        [JsonProperty("groupId")]
        public int? GroupId { get; set; }

        [JsonProperty("imagePath")]
        public string ImagePath { get; set; }

        [JsonProperty("audioAssetIds")]
        public IList<int> AudioAssetIds { get; }
        
        [JsonProperty("itemAssets")]
        public IList<AssetDto> Assets { get; set; }

        public bool Equals(IItem other)
        {
            if (other == null) return false;
            return Id == other.Id;
        }
    }

}