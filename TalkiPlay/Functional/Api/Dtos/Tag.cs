using System.Collections.Generic;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class TagDto : ITag
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("itemIds")]
        public IList<int> ItemIds { get; set; }

        //[JsonIgnore]
        //public IItem Item { get; set; }
    }

    public class AddTagRequest
    {
        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("itemId")]
        public int ItemId { get; set; }
    }
}