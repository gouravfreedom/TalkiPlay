using System.Collections.Generic;

namespace TalkiPlay.Shared
{
    public interface IChest
    {
        int Id { get; }
        IList<IReward> Rewards { get; }
    }
}