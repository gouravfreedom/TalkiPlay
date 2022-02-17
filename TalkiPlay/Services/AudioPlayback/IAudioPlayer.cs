using System;
using System.Threading;
using System.Threading.Tasks;

namespace TalkiPlay.Shared
{
    public interface IAudioPlayer : IDisposable
    {
        Task<bool> Play(AudioPlayerSetting settings, CancellationToken cancelToken = default(CancellationToken));
        void Stop();
        void ChangeVolume(float volume);
        void ChangeVolume(float volume, double duration);
        double Duration { get; }
        AudioPlayerSetting Settings { get; }
        event EventHandler OnFinishPlayging;
    }
}