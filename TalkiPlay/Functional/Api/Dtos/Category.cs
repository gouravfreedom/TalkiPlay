using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TalkiPlay.Shared
{
    public class CategoryDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imagePath")]
        public string ImagePath { get; set; }


        [JsonIgnore]
        public List<GameDto> Games { get; } = new List<GameDto>();
    }
}
