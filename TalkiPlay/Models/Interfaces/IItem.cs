using System;
using System.Collections.Generic;
using ChilliSource.Core.Extensions;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public enum ItemType
    {
        [Data("SortOrder", 1)]
        Default,
        [Data("SortOrder", 0)]
        Home
    }
    
    public interface IItem : IEquatable<IItem>
    {
        int Id { get; }
        string Name { get; }
        string QRCode { get; }
        ItemType Type { get; }
        int? GroupId { get; }
        
        string ImagePath { get; }
        
        IList<int> AudioAssetIds { get; }
        IList<AssetDto> Assets { get; set; }
        
        //IList<string> Tags { get; }
    }

    public class TagReadRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
 
        [JsonProperty("isHometag")]
        public bool IsHomeTag { get; set; }
    }
}