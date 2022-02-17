namespace TalkiPlay.Shared
{
    public interface IReward
    {
         int Id { get;  }

         int OpenImageAssetId { get; }
         int CollectImageAssetId { get; }
         double Percentage { get; }
    }
}