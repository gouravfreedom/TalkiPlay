using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class FileData : IFileData
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        
        [JsonProperty("checksum")]
        public string Checksum { get; set; }
        
        [JsonProperty("filename")]
        public string FileName { get; set; }
        
        [JsonProperty("size")]
        public long FileSize { get; set; }
        
        [JsonProperty("isLast")]
        public bool IsLast { get; set; }
    }
}