namespace TalkiPlay.Shared
{

    public enum SoundType
    {
        Error, 
        StartGame, 
        WrongScan, 
        HuntFinish, 
        Victory, 
        CorrectScan,
    }
    
    public interface ISound
    {
        SoundType Type { get; }   
        IAsset Asset { get; }
    }
}