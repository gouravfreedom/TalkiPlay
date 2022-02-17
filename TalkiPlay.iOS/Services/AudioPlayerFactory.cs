using ChilliSource.Mobile.Core;
using TalkiPlay;
using TalkiPlay.Shared;

namespace TalkiPlay.iOS
{
    public class AudioPlayerFactory : IAudioPlayerFactory
    {
        public IAudioPlayer Create(ILogger logger)
        {
            return new AudioPlayer(logger);
        }
    }
}