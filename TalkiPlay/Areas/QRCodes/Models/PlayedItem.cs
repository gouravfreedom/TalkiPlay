using System.Collections.Generic;

namespace TalkiPlay
{
    public class PlayedItem
    {
        
        const int MaxItemSegments = 4;
        private const int MaxAssetPlaybackCount = 2;
        
        public PlayedItem(int itemId)
        {
            ItemId = itemId;
            AssetCounts = new Dictionary<int, int>();
        }
        
        public int ItemId { get; }
        
        public int LastAssetId { get; private set; }
        
        public Dictionary<int, int> AssetCounts { get; }

        public bool IsCompleted => AssetCounts.Keys.Count >= MaxItemSegments;
        
        public void SetLastAssetId(int assetId)
        {
            LastAssetId = assetId;
        }

        public bool IsMaxAssetPlaybackCountExceeded(int assetId)
        {
            return AssetCounts.ContainsKey(assetId) &&
                   AssetCounts[assetId] >= MaxAssetPlaybackCount;
        }
    }
}