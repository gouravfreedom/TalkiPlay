using System.Collections.Generic;

namespace TalkiPlay.Shared
{
    public interface IPack
    {
        int Id { get; }
        
        string Name { get; }
        
        string ImagePath { get; }
        
        IList<ItemDto> Items { get; set; }
        
        //IList<IItem> Items { get; }
        
        IList<AssetDto> AudioAssets { get; set; }
        
        //IList<IAsset> Assets { get; }
    }
}