using Android.Content;
using ChilliSource.Mobile.Core;
using TalkiPlay.Shared;

namespace TalkiPlay.Droid
{
    public class AudioPlayerFactory : IAudioPlayerFactory
    {
        private readonly Context _context;

        public AudioPlayerFactory(Context context)
        {
            _context = context;
        }
        
        public IAudioPlayer Create(ILogger logger)
        {
            return new AudioPlayer(_context, logger);
        }
    }
}