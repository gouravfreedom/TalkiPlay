﻿using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace TalkiPlay.Shared
{
    public interface IBackgroundAudioPlayer
    {
        Task<bool> Play(AudioPlayerSetting settings);
        void Stop();
        bool IsPlaying { get;  }
        void ChangeVolume(float volume);
    }

    public class BackgroundAudioPlayer : ReactiveObject, IBackgroundAudioPlayer
    {
        private readonly ChilliSource.Mobile.Core.ILogger _logger;
        private readonly IAudioPlayerFactory _factory;
        private AudioPlayerSetting _settings;
        private IAudioPlayer _audioPlayer;

        public BackgroundAudioPlayer(IAudioPlayerFactory factory,
            ChilliSource.Mobile.Core.ILogger logger = null)
        {
            _settings = AudioPlayerSetting.Empty;
            _logger = logger ?? Locator.Current.GetService<ChilliSource.Mobile.Core.ILogger>();
            _factory = factory ?? Locator.Current.GetService<IAudioPlayerFactory>();
        }
        
        public Task<bool> Play(AudioPlayerSetting settings)
        {

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!Config.PlayMusic)
            {
                return Task.FromResult(false);
            }

#pragma warning disable 162
            if (!_settings.IsEmpty && _settings.FilePath.Equals(settings.FilePath) && IsPlaying)
            {
                return Task.FromResult(true);
            }

            if(IsPlaying)
            {
                _audioPlayer.Stop();
                _audioPlayer = null;
            }

            IsPlaying = true;
            _settings = settings;
            _audioPlayer = _factory.Create(_logger);
			return _audioPlayer.Play(_settings);
#pragma warning restore 162
        }

        public void Stop()
        {
            if (_audioPlayer != null)
            {
                IsPlaying = false;
                _audioPlayer.Stop();
                _audioPlayer = null;
            }
        }

        [Reactive]
        public bool IsPlaying { get; set; }

        public void ChangeVolume(float volume)
        {
            _audioPlayer?.ChangeVolume(volume);
        }
    }
}