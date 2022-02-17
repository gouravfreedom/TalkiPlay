using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public interface IAudioPlayerFactory
    {
        IAudioPlayer Create(ILogger logger);
    }
}