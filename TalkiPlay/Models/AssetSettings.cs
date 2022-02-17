using System;
namespace TalkiPlay.Shared
{
    public class AssetSettings
    {
        public AssetSettings()
        {
        }

        public AssetSettings(int assetId, string filePath)
        {
            AssetId = assetId;
            FilePath = filePath;
        }

        public int AssetId { get; set; }
        public string FilePath { get; set; }
    }
}
