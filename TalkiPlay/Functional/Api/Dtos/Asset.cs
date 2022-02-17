using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class AssetDto : IAsset
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public AssetType Type { get; set; }
        
        [JsonProperty("purpose")]
        public AssetPurpose? Purpose { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("filesize")]
        public int Filesize { get; set; }
        
        public string FilePath { get; set; }

        [JsonProperty("imageContentPath")]
        public string ImageContentPath { get; set; }

        [JsonProperty("category")]
        public Category? Category { get; }
    }

    public enum AssetType
    {
        Image = 1,
        Audio,
        Pdf,
        Animation
    }

    public enum AssetPurpose
    {
        None,
        PuzzleCompletion
    }
}