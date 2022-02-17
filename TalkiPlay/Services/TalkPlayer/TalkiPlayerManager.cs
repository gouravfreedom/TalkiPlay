using System;
using System.Reactive.Disposables;
using Plugin.BluetoothLE;

namespace TalkiPlay.Shared
{
    public interface ITalkiPlayerManager : IDisposable
    {
        bool IsTalkiPlayerReady { get; }
        ITalkiPlayer Current { get; }
        void SetTalkiPlayer(IDevice device);
        void Clear();
        void CancelUpload();
    }

    public class TalkiPlayerManager : ITalkiPlayerManager
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public void Dispose()
        {
            _disposable?.Dispose();
            Current = null;
        }

        public bool IsTalkiPlayerReady => Current != null && Current.IsConnected;
        public ITalkiPlayer Current { get; private set; }
        public void SetTalkiPlayer(IDevice device)
        {
            if (Current != null)
            {
                Current.Disconnect().Subscribe();
                _disposable.Remove(Current);
            }

            if (device != null)
            {
                Current = TalkiPlayer.Create(device);
                _disposable.Add(Current);
            }
        }

        public void Clear()
        {
            CancelUpload();
            Current = null;
        }

        public void CancelUpload()
        {
            Current?.CancelUpload();
        }
    }
}