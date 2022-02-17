using System.Collections.Generic;

namespace TalkiPlay.Shared
{
    public interface IRoom
    {
        int Id { get; } 
        string Name { get;  }
        string ImagePath { get;  }
        IList<int> Packs { get;  }
        IList<ITag> TagItems { get; }
        
        int TagsCount { get; }

        int? AssetId { get; }
    }

}