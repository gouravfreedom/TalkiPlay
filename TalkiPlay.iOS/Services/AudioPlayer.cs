using System;
using System.Threading;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using TalkiPlay;
using TalkiPlay.Shared;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.iOS
{
    public class AudioPlayer : IAudioPlayer
    {
        private readonly ILogger _logger;
        private TaskCompletionSource<bool> _tcs;
        private AVAudioPlayer _player;
        private AudioPlayerSetting _settings;
        public AudioPlayer(ILogger logger)
        {
            _logger = logger;
        }
   
        public Task<bool> Play(AudioPlayerSetting settings, CancellationToken cancelToken = default(CancellationToken))
        {
            _settings = settings;
            _tcs = new TaskCompletionSource<bool>();
            _player = AVAudioPlayer.FromUrl(CreateUrlFromFile(settings), out NSError err);
            if (err != null)
            {
                OnFinishPlayging?.Invoke(this, new EventArgs());
                _tcs.TrySetResult(false);
            }
            else
            {
                _player.Volume = settings.Volume;
                _player.NumberOfLoops = settings.NumberOfLoops;
                _player.FinishedPlaying += Player_FinishedPlaying;
                _player.Play();
                Duration = _player.Duration;
                cancelToken.Register(() =>
                {
                    CleanUpHandlers(_player);
                    _player?.Stop();
                    _player?.Dispose();
                    OnFinishPlayging?.Invoke(this, new EventArgs());
                    _tcs?.TrySetResult(false);
                });
            }

            return _tcs.Task;
        }

        public void Stop()
        {
            _tcs?.TrySetResult(false);
            _player?.Stop();
            _player?.Dispose();
            _player = null;
        }

        public void ChangeVolume(float volume)
        {
            if (_player != null)
            {
                _player.Volume = volume;
            }
        }

        public void ChangeVolume(float volume, double duration)
        {
            _player?.SetVolume(volume, duration);
        }

        public double Duration { get; private set; }
        public AudioPlayerSetting Settings => _settings;
        public event EventHandler OnFinishPlayging;

        private static NSUrl CreateUrlFromFile(AudioPlayerSetting settings)
        {
            return NSUrl.FromString(settings.FilePath);
        }

        private void CleanUpHandlers(AVAudioPlayer player) 
        {
            if(player != null) {
                player.FinishedPlaying -= Player_FinishedPlaying;
            }
        }
        
        void Player_FinishedPlaying(object sender, AVStatusEventArgs e)
        {
            var player = (AVAudioPlayer)sender;
            OnFinishPlayging?.Invoke(this, new EventArgs());
            CleanUpHandlers(player);
            _logger?.Information("Finish playing sounds");
            _tcs?.TrySetResult(true);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}