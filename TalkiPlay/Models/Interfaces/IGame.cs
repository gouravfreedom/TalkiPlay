using System.Collections.Generic;

namespace TalkiPlay.Shared
{
    public interface IGame
    {
        int Id { get; }

        string Name { get; }
        string ShortDescription { get;  }
        
        string ImagePath { get; }
        GameType Type { get;}

        GameLevel Level { get; }
        
        GameAccessLevel AccessLevel { get; set; }

        string Description { get; }

        int PackId { get; }

        IList<IInstruction> Instructions { get;  }

        //IList<IItem> Items { get; }
        
        IList<ISound> Sounds { get;  }
       
        IChest Chest { get; }
        
        bool InstructionsAreRandom { get; }
        
        bool IsRecommended { get; set; }

        IList<int> Categories { get; set; }


    }
}