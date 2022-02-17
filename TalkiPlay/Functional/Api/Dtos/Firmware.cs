using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class FileDataDto : IFileData
    {

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("crc")]
        public string Checksum { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileSize")]
        public long FileSize { get; set; }
        
        public string FilePath { get; set; }
    }

}