using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BluetoothLE;

namespace TalkiPlay.Shared
{
    public interface ITalkiPlayer : IDisposable
    {
        string SystemId { get;  }
        string HardwareVersion { get; }
        string FirmwareVersion { get; }
        string ModelVersion { get; }
        string SerialNumber { get;}
        
        int BatteryLevel { get; }
        BatteryPowerStatus BatteryStatus { get; }

        bool IsConnected { get; }
        IDevice Device { get; }
        IObservable<bool> Connect(ConnectionConfig config = null);
        IObservable<bool> Disconnect();

        ConnectionStatus ConnectionStatus { get; }
        
        string Name { get; }

        void Upload(IUploadData data, TimeSpan? timeout = null);
        bool Uploading { get; }
        
        IObservable<IDataUploadResult> OnDataResult();
        
        IObservable<bool> IsReady { get; }

        IObservable<Exception> WhenConnectionFailed();

        IObservable<bool> WhenDisconnected();
        void CancelUpload();

        //IObservable<Unit> WhenDisconnectedUponInactivity();

        void UploadTagData(ItemDto item);
    }
}