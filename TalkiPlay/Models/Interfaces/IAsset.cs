
namespace TalkiPlay.Shared
{
    public interface IAsset
    {
        int Id { get;  }

        string Name { get;  }

        AssetType Type { get; }

        string Filename { get; }

        int Filesize { get; }

        string FilePath { get; }
        
        string ImageContentPath { get; }
        
        Category? Category { get; }
        
        AssetPurpose? Purpose { get; set; }
    }

    public enum Category
    {
        Item,
        Room,
        Avatar,
        System
    }
}