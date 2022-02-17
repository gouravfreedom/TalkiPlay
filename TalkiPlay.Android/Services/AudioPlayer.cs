using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.Res;
using Android.Media;
using Android.Support.V4.Content;
using System.IO;
using ChilliSource.Mobile.Core;
using Plugin.CurrentActivity;
using TalkiPlay.Shared;
using ILogger = ChilliSource.Mobile.Core.ILogger;
using AUri = Android.Net.Uri;

namespace TalkiPlay.Droid
{
    public class AudioPlayer : IAudioPlayer
    {
        private readonly Context _context;
        private readonly ILogger _logger;
        private AudioPlayerSetting _settings;
        private TaskCompletionSource<bool> _tcs;
        private MediaPlayer _player;

        public AudioPlayer(Context context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public Task<bool> Play(AudioPlayerSetting settings, CancellationToken cancelToken = default(CancellationToken))
        {
            _tcs = new TaskCompletionSource<bool>();
            _settings = settings;
            var audioFile = settings.FilePath.Replace(Audio.AudioFolder, "").Replace(".mp3", "");
            var resourceId =  _context.Resources.GetIdentifier(audioFile, "raw", CrossCurrentActivity.Current.Activity.PackageName);
            if (resourceId > 0)
            {
                _player = MediaPlayer.Create(CrossCurrentActivity.Current.Activity, resourceId);
            }
            else
            {
                var uri = AUri.Parse(settings.FilePath);
                _player = MediaPlayer.Create(CrossCurrentActivity.Current.Activity, uri);
            }
            
            _player.Completion += PlayerOnCompletion;
            _player.SetVolume(settings.Volume, settings.Volume);
            Duration = _player.Duration;
            _player.Start();
            return _tcs.Task;
        }

        public void Stop()
        {
            try {

                _tcs?.TrySetResult(false);
                _player?.Pause();
                _player?.Release();
                _player?.Dispose();
                _player = null;
            }
            catch(Exception ex) {
                Serilog.Log.Debug(ex.Message);
            }
        }

        public void ChangeVolume(float volume)
        {
            _player.SetVolume(volume,volume);
        }

        public void ChangeVolume(float volume, double duration)
        {
            _player.SetVolume(volume,volume);
        }

        public double Duration { get; private set; }
        public AudioPlayerSetting Settings => _settings;
        public event EventHandler OnFinishPlayging;

        private void CleanUpHandlers(MediaPlayer player)
        {
            if (player == null) return;
            player.Completion -= PlayerOnCompletion;
            player.Release();
        }

        private void PlayerOnCompletion(object sender, EventArgs eventArgs)
        {
            var player = (MediaPlayer)sender;
            OnFinishPlayging?.Invoke(this, new EventArgs());
            CleanUpHandlers(player);
            _logger?.Information("Finish playing sounds");
            _tcs.TrySetResult(true);
        }

        
        public void Dispose()
        {
           Stop();
        }
    }
}