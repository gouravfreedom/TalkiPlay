using System.Collections.Generic;

namespace TalkiPlay.Shared
{
    public interface IInstruction
    {
        string Name { get;}
        IItem Item { get; }
        IList<IMode> Modes { get; }
    }
    
}