namespace TalkiPlay.Shared
{
    public interface IMode
    {
        InstructionModeType Type { get;}
        int? ImageAssetId { get; }
        int? AudioAssetId { get;}
        int? AnimationAssetId { get; }
        string Text { get; }
    }
}