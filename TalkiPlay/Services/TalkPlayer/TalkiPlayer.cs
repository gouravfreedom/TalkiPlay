using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Plugin.BluetoothLE;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;


namespace TalkiPlay.Shared
{
    class TalkiPlayer : ReactiveObject, ITalkiPlayer
    {
        private readonly INavigationService _navigationService;
        private readonly IUserDialogs _userDialogs;
        private readonly ILogger _logger;
        private readonly IConfig _config;
        private readonly IStorage _storage;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private static Guid BATTERY_SERVICE_UUID = new Guid("0000180F-0000-1000-8000-00805F9B34FB");
        private static Guid BATTERY_LEVEL_CHARACTERISTIC_UUID = new Guid("00002A19-0000-1000-8000-00805F9B34FB");
        private static Guid BATTERY_POWER_STATE_CHARACTERISTIC_UUID = new Guid("00002A1A-0000-1000-8000-00805F9B34FB");

        private static Guid DEVICE_INFORMATION_SERVICE_UUID = new Guid("0000180A-0000-1000-8000-00805F9B34FB");
        private static Guid FIRMWARE_REVISION_CHARACTERISTIC_UUID = new Guid("00002A26-0000-1000-8000-00805F9B34FB");
        private static Guid MODEL_NUMBER_CHARACTERISTIC_UUID = new Guid("00002A24-0000-1000-8000-00805F9B34FB");
        private static Guid SERIAL_NUMBER_CHARACTERISTIC_UUID = new Guid("00002A25-0000-1000-8000-00805F9B34FB");
        private static Guid HARDWARE_REVISION_CHARACTERISTIC_UUID = new Guid("00002A27-0000-1000-8000-00805F9B34FB");
        private static Guid SYSTEM_ID_CHARACTERISTIC_UUID = new Guid("00002A23-0000-1000-8000-00805F9B34FB");

        private static Guid DATA_TRANSFER_SERVICE_UUID = new Guid("0000abf0-0000-1000-8000-00805f9b34fb");
        private static Guid DATA_TRANSFER_INPUT_CHARACTERISTIC_UUID = new Guid("0000abf2-0000-1000-8000-00805f9b34fb");
        private static Guid DATA_TRANSFER_OUTPUT_CHARACTERISTIC_UUID = new Guid("0000abf1-0000-1000-8000-00805f9b34fb");
        private readonly Subject<IDataUploadResult> _dataUploadSubject = new Subject<IDataUploadResult>();
        private readonly BehaviorSubject<bool> _isReadySubject = new BehaviorSubject<bool>(false);
        private readonly Subject<Exception> _connectionFailedSubject = new Subject<Exception>();
        private readonly BehaviorSubject<bool> _isPipeLineReadySubject = new BehaviorSubject<bool>(false);

        private IDisposable _timeoutDisposable;
        private IDisposable _connectionTimeoutDisposable;
        private CompositeDisposable _characteristsicsDisposable = new CompositeDisposable();
        private CompositeDisposable _uploadDisposable = new CompositeDisposable();
        //private static int MaxByteSize = 20;
        private static int MtuRequestSize = 517;
        private static int MaxMtuSize = 512;
        private static int MaxiOSMtuSize = 512;
        //private CancellationTokenSource cancellationTokenSource;
        private List<CharacteristicGattResult> _buffers = new List<CharacteristicGattResult>();
        private Subject<List<CharacteristicGattResult>> _bufferSubject = new Subject<List<CharacteristicGattResult>>();
        private IDisposable _readBufferTimerDisposable;
        private int _bufferIterationCount = 0;
        private bool _hasDeviceInfo = false;
        private bool _isManualDisconnect = false;
        private readonly Subject<bool> _disconnectSubject = new Subject<bool>();
       
        public static ITalkiPlayer Create(IDevice device) => new TalkiPlayer(device);

        private TalkiPlayer(IDevice device, 
            IConfig config = null,
            IStorage storage = null,
            ILogger logger = null,
            IUserDialogs userDialogs = null,
            INavigationService navigationService = null)
        {
            _navigationService = navigationService ?? Locator.Current.GetService<INavigationService>(Constants.MainNavigation);
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            _logger = logger ?? Locator.Current.GetService<ChilliSource.Mobile.Core.ILogger>();
            _config = config ?? Locator.Current.GetService<IConfig>();
            _storage = storage ?? Locator.Current.GetService<IStorage>();
            Device = device;
            SetupRxForConnectionStatus();
            
//            SetupRxForDeviceInformation();
//            SetupRxForBatteryStatus();
            //SetupRxForDataTransfer();
        }
     
        public void Dispose()
        {
            _disposable?.Dispose();
            Device?.CancelConnection();
            Device = null;
            DataTransferOutput = null;
            _connectionTimeoutDisposable?.Dispose();
            _connectionTimeoutDisposable = null;
            _timeoutDisposable?.Dispose();
            _timeoutDisposable = null;
            _characteristsicsDisposable?.Clear();
            _uploadDisposable?.Clear();
            _hasDeviceInfo = false;
            _isManualDisconnect = false;
        }

        [Reactive]
        public string SystemId { get; set; }
        [Reactive]
        public string HardwareVersion { get; set; }
        [Reactive]
        public string FirmwareVersion { get; set; }
        [Reactive]
        public string ModelVersion { get; set; }
        [Reactive]
        public string SerialNumber { get; set; }

        [Reactive] public int BatteryLevel { get; set; } = 100;

        [Reactive] public BatteryPowerStatus BatteryStatus { get; set; } = BatteryPowerStatus.Charging;

        [Reactive]
        public bool IsConnected { get; set; }
        
        public IDevice Device { get; private set; }
        public ConnectionConfig DeviceConfig { get; private set; }
        public IObservable<bool> Connect(ConnectionConfig config = null)
        {
            try
            {
                var cfg = config ?? (DeviceConfig ?? new ConnectionConfig { AutoConnect = true });
                DeviceConfig = cfg;
                Device.Connect(cfg);

                if (!IsConnected)
                {
                    StartTimeoutTimerForConnection();
                }

                return Observable.Return(true);
            }
            catch (Exception)
            {
                return Observable.Return(false);
            }
        }

        public IObservable<bool> Disconnect()
        {
            try
            {
                Device.CancelConnection();
                _isManualDisconnect = true;
               
                return Observable.Return(true);
            }
            catch (Exception)
            {
                return Observable.Return(false);
            }
        }

        public extern ConnectionStatus ConnectionStatus { [ObservableAsProperty] get;}
        public string Name => Device.Name;

        public void Upload(IUploadData data, TimeSpan? timeout = null)
        {
            if (IsUploading())
            {
                return;
            }

            Uploading = true;
            this.UploadData = data;

      
            switch (data.Type)
            {
                case UploadDataType.Firmware:
                case UploadDataType.Audio:
                {
                    if (!(data is IFileUploadData uploadData)) return;
                
                    if(String.IsNullOrWhiteSpace(uploadData.Path))
                    {
                        throw new ArgumentNullException(nameof(uploadData.Path));
                    }

                    if(!File.Exists(uploadData.Path)) {
                    
                        var result = DataUploadResult.Failed(UploadData?.Tag, UploadData?.Type ?? UploadDataType.Unknown,
                            new ApplicationException( $"Could not find the file in the path {uploadData.Path}"));

                        _dataUploadSubject.OnNext(result);
                        CleanupUpload();
                        return;
                    }
                    
                    var item = new FileData()
                    {
                        Checksum = uploadData.Checksum,
                        FileName = uploadData.Name,
                        FileSize = uploadData.Size,
                    };

                    var jsonString = JsonConvert.SerializeObject(item, JsonSerializerSettings);
                    MetaData = new DataUploadData(name: uploadData.Name, jsonString, uploadData.Tag,
                        UploadDataType.MetaData);
                    
                    Observable.Start(() => MetaData, RxApp.TaskpoolScheduler)
                        .SelectMany( m => Observable.FromAsync(cancelToken =>  UploadFile(m, timeout, cancelToken)))
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(_ =>
                        {
                          // _logger.Warning($"Upload - {UploadData?.Type} sent");
                   
                        } , ex =>
                        {
                            _logger.Error(ex);
                            CleanupUpload();
                            var result = DataUploadResult.Failed(UploadData?.Tag, UploadData?.Type ?? UploadDataType.Unknown,
                                ex);
                            _dataUploadSubject.OnNext(result);
                        })
                        .DisposeWith(_uploadDisposable);
                    break;
                }
               default:
                    Observable.Start(() => UploadData, RxApp.TaskpoolScheduler)
                        .SelectMany( m => Observable.FromAsync(cancelToken =>  UploadFile(m, timeout, cancelToken)))
                        .ObserveOn(RxApp.MainThreadScheduler)
                       .Subscribe(_ =>
                        {
                          //  _logger.Warning($"Upload - {UploadData?.Type} sent");

                        }, ex =>
                        {
                            _logger.Error(ex);
                            CleanupUpload();
                            var result = DataUploadResult.Failed(UploadData?.Tag, UploadData?.Type ?? UploadDataType.Unknown,
                                ex);
                            _dataUploadSubject.OnNext(result);
                        })
                        .DisposeWith(_uploadDisposable);
                    break;
            }
        }

        private async Task<IDataUploadResult> GetResult(CharacteristicGattResult result,
            IGattCharacteristic characteristic, CancellationToken cancelToken = default(CancellationToken))
        {
           
            var data = result.Data;
            if (data == null || data.Length < 6)
            {
                return DataUploadResult.Failed(UploadData?.Tag,  UploadData?.Type ?? UploadDataType.Unknown, new ApplicationException("Data is invalid")); 
            }
            
            try
            {
                var type = (UploadDataType) data[0];
                var isSuccess = data[1] == 0;
                var sizeBytes = Slicer(data, 2, 4);
                var size = GetSize(sizeBytes.ToArray());
                _logger.Information($"size: {size}");
                
                if (!isSuccess)
                {
                    var errorDataBuffer = await ReadBufferInChunks(result, characteristic, size);

                    var errorData = new HardwareErrorDataResult()
                    { 
                        ErrorCode = "9999",
                        Message = "Something went wrong. Could not recover from fatal error."
                    };
                    
                    if (errorDataBuffer != null && errorDataBuffer.Length > 0)
                    {
                        errorData = Deserialize<HardwareErrorDataResult>(errorDataBuffer);
                    }
                    
                    return DataUploadResult.Failed(UploadData?.Tag, type, new ApplicationException(errorData.Message));     
                }
                
                switch (type)
                {
                    case UploadDataType.GameResult:
                       return await GetData<GameResult>(result, characteristic, size, type);
                    case UploadDataType.TagData:
                        return await GetData<TagData>(result,characteristic, size, type);
                    case UploadDataType.AvailableAudioFiles:
                        return await GetData<AvailableAudioFiles>(result, characteristic, size, type);
                    case UploadDataType.DeviceInfo:
                       return await GetData<TalkiPlayerDeviceInfo>(result, characteristic, size, type);
                    case UploadDataType.VolumeInfo:
                        return await GetData<VolumeInfo>(result, characteristic, size, type);
                    default:
                        return new DataUploadResult(type, UploadData?.Tag, true, new EmptyResult());
                }
            }
            catch (Exception ex)
            {
                return DataUploadResult.Failed(UploadData?.Tag, UploadData?.Type ?? UploadDataType.Unknown, ex);
            }
        }

        private async Task<IDataUploadResult> GetData<T>(CharacteristicGattResult gattResult, IGattCharacteristic characteristic, int size, UploadDataType type) where T : IDataResult
        {
            var buffer = await ReadBufferInChunks(gattResult, characteristic, size);
            if (buffer == null || buffer.Length <= 0)
                return DataUploadResult.Failed(UploadData?.Tag, type,
                    new ApplicationException("Data is invalid"));
            var result = Deserialize<T>(buffer);
            _logger.Information($"{result}");
            return new DataUploadResult(type, UploadData?.Tag, true, result);
        }

#pragma warning disable 1998
        private async Task<byte[]> ReadBufferInChunks(CharacteristicGattResult gattResult, IGattCharacteristic characteristic, int length)
#pragma warning restore 1998
        {
            if (length == 0) return null;
            var buffer = gattResult;
            var data = Slicer(buffer.Data, 6, buffer.Data.Length - 6);
       //     _logger.Information($"Data => {BitConverter.ToString(gattResult.Data)}");

            //Observable.Interval(TimeSpan.FromMilliseconds(5))
            //    .SelectMany(c => characteristic.Read())
            //     .Do(d =>
            //     {
            //         _logger.Information($"Data => {BitConverter.ToString(d.Data)}");
            //     })
            //     .Subscribe(d =>
            //    { 
            //    }, ex => _logger.Error(ex));


            //            
            //            if (data.Length == length)
            //            {
            //                buffer = gattResult;
            //            }
            //            else 
            //            {
            //               buffer = await characteristic.Read();
            //               data = Slicer(buffer.Data, 6, buffer.Data.Length - 6);
            //            }

            //   _logger.Warning($"dataRead: {BitConverter.ToString(data)} - size:{data.Length}");
            return data;
        }

        private int GetSize(byte[] sizeBytes)
        {
            if (sizeBytes == null || sizeBytes.Length <= 0) return 0;

            var sizes = new byte[sizeBytes.Length];
            sizeBytes.CopyTo(sizes, 0);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(sizes);
            }

            var size = BitConverter.ToUInt32(sizes, 0);
            return (int) size;
        }

        private T Deserialize<T>(byte[] data) where T : IDataResult
        {
            try
            {
                var json = Encoding.UTF8.GetString(data);
                _logger.Information($"json: {json}");
                var resultData = JsonConvert.DeserializeObject<T>(json);
                return resultData;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw ex;
            }
        }

        private byte[] Slicer(byte[] bytes, int index, int count)
        {
            return bytes.Mid(index, count);
        }

        private void SetupBatteryCharacteristics(IGattCharacteristic characteristic)
        {
            var characteristics = new List<Guid>()
            {
                BATTERY_LEVEL_CHARACTERISTIC_UUID,
                BATTERY_POWER_STATE_CHARACTERISTIC_UUID
            };

            if (!characteristics.Contains(characteristic.Uuid))
            {
                return;
            }
            
            if (characteristic.CanNotify())
            {
                characteristic.EnableNotifications()
                    .Subscribe()
                    .DisposeWith(_characteristsicsDisposable);
            }

            if (characteristic.CanRead())
            {
                if (characteristic.Uuid == BATTERY_LEVEL_CHARACTERISTIC_UUID)
                {
                    characteristic.Read()
                        .Subscribe(m => UpdateBatteryLevel(m.Data))
                        .DisposeWith(_characteristsicsDisposable);

                    characteristic.WhenNotificationReceived()
                        .Subscribe(r => UpdateBatteryLevel(r.Data))
                        .DisposeWith(_characteristsicsDisposable);
                }

                if (characteristic.Uuid == BATTERY_POWER_STATE_CHARACTERISTIC_UUID)
                {
                    characteristic.Read()
                        .Subscribe(m => UpdateBatteryStatus(m.Data))
                        .DisposeWith(_characteristsicsDisposable);

                    characteristic.WhenNotificationReceived()
                        .Subscribe(r => UpdateBatteryStatus(r.Data))
                        .DisposeWith(_characteristsicsDisposable);
                }
            }
        }


        private void SetupCharacteristics()
        {

            //Device.GetKnownCharacteristics(DEVICE_INFORMATION_SERVICE_UUID, 
            //        FIRMWARE_REVISION_CHARACTERISTIC_UUID,  HARDWARE_REVISION_CHARACTERISTIC_UUID
            //        )
            //    .Subscribe(SetupDeviceCharacteristics)
            //    .DisposeWith(_characteristsicsDisposable);

            //            Device.GetKnownCharacteristics(BATTERY_SERVICE_UUID, 
            //                    BATTERY_LEVEL_CHARACTERISTIC_UUID, 
            //                    BATTERY_POWER_STATE_CHARACTERISTIC_UUID)
            //                .Subscribe(SetupBatteryCharacteristics)
            //                .DisposeWith(_characteristsicsDisposable);

        
            Device.GetKnownCharacteristics(DATA_TRANSFER_SERVICE_UUID,
                    DATA_TRANSFER_INPUT_CHARACTERISTIC_UUID,
                    DATA_TRANSFER_OUTPUT_CHARACTERISTIC_UUID)
                .Subscribe(SetupDataTransferCharacteristics)
                .DisposeWith(_characteristsicsDisposable);

        }


        private void StartReadCharacteristicsBufferTimer()
        {
            if (_readBufferTimerDisposable != null) return;

            _readBufferTimerDisposable = Observable.Timer(TimeSpan.FromMinutes(1))
                .Subscribe(a =>
                {
                    NotifyReadBufferIsCompleted();
                });

        }

        private void NotifyReadBufferIsCompleted()
        {
            
            var buffers = new List<CharacteristicGattResult>(_buffers);
            _buffers.Clear();
            _bufferIterationCount = 0;
            _bufferSubject.OnNext(buffers);
        }

        private void SetupDataTransferCharacteristics(IGattCharacteristic characteristic)
        {
          
            var characteristics = new List<Guid>()
            {
                DATA_TRANSFER_INPUT_CHARACTERISTIC_UUID,
                DATA_TRANSFER_OUTPUT_CHARACTERISTIC_UUID
            };

            if (!characteristics.Contains(characteristic.Uuid))
            {
                return;
            }
            
            if (characteristic.Uuid == DATA_TRANSFER_INPUT_CHARACTERISTIC_UUID)
            {
                if (characteristic.CanNotify())
                {
                    characteristic.EnableNotifications()
                        .SubscribeSafe()
                        .DisposeWith(_characteristsicsDisposable);


                    characteristic.WhenNotificationReceived()
                        .Do(m => _logger.Information($"Receved: {BitConverter.ToString(m.Data)}"))
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Do(_ =>
                        {
                            _timeoutDisposable?.Dispose();
                            _timeoutDisposable = null;
                        })
                        .Do(m => StartReadCharacteristicsBufferTimer())
                       
                        .Subscribe(a =>
                        {
                            if(!_buffers.Any())
                            {
                                var data = a.Data;
                                var length = a.Data.Length;
                                var sizeBytes = Slicer(data, 2, 4);
                                var size = GetSize(sizeBytes.ToArray());
                                var totalSize = size + 6;
                               var bufferCount = totalSize > length ? (totalSize / length) : 0;
                                _bufferIterationCount = (int) Math.Ceiling((decimal) bufferCount) + 1;
                            }

                            _buffers.Add(a);

                            if(_buffers.Count >= _bufferIterationCount)
                            {
                                NotifyReadBufferIsCompleted();
                            }
                        })
                        .DisposeWith(_characteristsicsDisposable);

                    _bufferSubject
                     .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(a =>
                    {
                        _readBufferTimerDisposable?.Dispose();
                        _readBufferTimerDisposable = null;

                    })
                    .ObserveOn(RxApp.TaskpoolScheduler)
                        .Select(d =>
                     {
                         using (var stream = new MemoryStream())
                         {
                             d.Where(a => a.Data != null && a.Data.Length > 0).ToList().ForEach(r =>
                             {
                                 stream.Write(r.Data, 0, r.Data.Length);
                             });

                             stream.Position = 0;
                             return new CharacteristicGattResult(characteristic, stream.ToArray());    
                         }
                         
                     })
                        .Where(_ => IsUploading())
                      
                     //.Do(m => _logger.Warning($"Data - {BitConverter.ToString(m.Data)}"))
                        .SelectMany(m => Observable.FromAsync(cancelToken => GetResult(m, characteristic, cancelToken: cancelToken)))
                        .SelectMany(result =>
                        {
                            if (result.IsSuccess)
                            {
                                
                                switch (result.Type)
                                {
                                    case UploadDataType.MetaData:
                                         Observable.FromAsync(
                                                cancelToken => UploadFile(UploadData, timeout: TimeSpan.FromMinutes(30), cancelToken: cancelToken),
                                                RxApp.TaskpoolScheduler)
                                            .Select(_ => Unit.Default)
                                            .SubscribeSafe()
                                            .DisposeWith(_uploadDisposable);
                                        return Observable.Return(Unit.Default);
                                    default:
                                         CleanupUpload();
                                        _dataUploadSubject.OnNext(result);
                                        return Observable.Return(Unit.Default);
                                }
                            }

                            _uploadDisposable?.Clear();
                            CleanupUpload();
                            if (result.Data is ErrorDataResult errorData)
                            {
                                _logger.Error(errorData.Exception);
                            }
                            
                            _dataUploadSubject.OnNext(result);

                            return Observable.Return(Unit.Default);
                        })
                        .Subscribe(_ => {


                        }, exception =>
                        {
                            _timeoutDisposable?.Dispose();
                            _logger.Error(exception);
                            CleanupUpload();
                            var result = DataUploadResult.Failed(UploadData?.Tag, UploadData?.Type ?? UploadDataType.Unknown,
                                exception);
                            _dataUploadSubject.OnNext(result);
                        })
                        .DisposeWith(_characteristsicsDisposable);
                }
            }

            if (characteristic.Uuid == DATA_TRANSFER_OUTPUT_CHARACTERISTIC_UUID)
            {
                DataTransferOutput = characteristic;
                _isPipeLineReadySubject?.OnNext(true);
            }
        }

        private void SetupDeviceCharacteristics(IGattCharacteristic characteristic)
        {
            var characteristics = new List<Guid>()
            {
                FIRMWARE_REVISION_CHARACTERISTIC_UUID,
                HARDWARE_REVISION_CHARACTERISTIC_UUID,
              
            };

            if (!characteristics.Contains(characteristic.Uuid))
            {
                return;
            }
            
            if (characteristic.CanRead())
            {
                characteristic.Read()
                    .Subscribe(m =>
                    {
                        if (m.Data == null) return;

                        if (characteristic.Uuid == FIRMWARE_REVISION_CHARACTERISTIC_UUID)
                        {
                            FirmwareVersion = Encoding.UTF8.GetString(m.Data);
                        }

//                        if (characteristic.Uuid == MODEL_NUMBER_CHARACTERISTIC_UUID)
//                        {
//                            ModelVersion = Encoding.UTF8.GetString(m.Data);
//                        }

//                        if (characteristic.Uuid == SERIAL_NUMBER_CHARACTERISTIC_UUID)
//                        {
//                            SerialNumber = Encoding.UTF8.GetString(m.Data);
//                        }

                        if (characteristic.Uuid == HARDWARE_REVISION_CHARACTERISTIC_UUID)
                        {
                            HardwareVersion = Encoding.UTF8.GetString(m.Data);
                        }

//                        if (characteristic.Uuid == SYSTEM_ID_CHARACTERISTIC_UUID)
//                        {
//                            SystemId = Convert.ToBase64String(m.Data);
//                        }
                    })
                    .DisposeWith(_characteristsicsDisposable);
            }
        }


        private bool IsiOS { get => Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS; }

        private int GetMaxByteSize { get => MtuSize ?? Device?.MtuSize ?? (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS ? MaxiOSMtuSize : MaxMtuSize); }

        private void SetupRxForConnectionStatus()
        {
            Device.WhenConnected()
                .Do(_ => _connectionTimeoutDisposable?.Dispose())
                .SelectMany(_ =>
                {
                    if(IsiOS)
                    {
                        return Observable.Return(Device.MtuSize);
                    }
                    else
                    {
                        return Device.RequestMtu(MtuRequestSize);
                    }
                })
                .Do(size => MtuSize = size)
                .Do(size => _logger.Information($"Size: {size}"))
                .SelectMany(_ => Observable.StartAsync(() =>
                {
                    if(Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
                    {
                        return Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    else
                    {
                        return Task.Delay(0);
                    }

                }))
                .Do(_ => SetupCharacteristics())
                .SubscribeSafe()
                .DisposeWith(_disposable);

            Device.WhenDisconnected()
                .Do(_ => _connectionTimeoutDisposable?.Dispose())
                .Do(_ => OnDisconnectCleanup())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(_ =>
                {
                    _disconnectSubject.OnNext(_isManualDisconnect);
                    _isManualDisconnect = false;
                })
                .Subscribe(_ => IsConnected = false)
                .DisposeWith(_disposable);

            Device.WhenConnectionFailed()
                .Do(_ => _connectionTimeoutDisposable?.Dispose())
                .Do(ex => OnDisconnectCleanup())
                .Do(ex => _connectionFailedSubject?.OnNext(new ApplicationException(ex.Message)))
                .Subscribe(_ => IsConnected = false)
                .DisposeWith(_disposable);

            Device.WhenStatusChanged()
                .ToPropertyEx(this, m => m.ConnectionStatus)
                .DisposeWith(_disposable);

            this.IsReady
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(m => IsConnected = m)
                .SubscribeSafe()
                .DisposeWith(_disposable);
            
            this._isPipeLineReadySubject
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .Where(ready => ready)
                .Where(m => !_hasDeviceInfo)
                .Do(m =>
                {
                    _isManualDisconnect = false;
                    if (IsShakeTalkiPlayerPopupShown && _navigationService.HasPopModalInStack)
                    {
                        IsShakeTalkiPlayerPopupShown = false;
                        _navigationService.PopPopup().SubscribeSafe();
                    }
                })
                .Subscribe(ready =>
                {
                    this.OnDataResult()
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .SubscribeSafe(GetDeviceInfo);

                    Upload(new DataUploadData("DeviceInfo", DataRequest.DeviceInfoRequest(), Guid.NewGuid().ToString(), UploadDataType.DeviceInfo), TimeSpan.FromMinutes(1));
                })
            .DisposeWith(_disposable);
          
        }

        private void GetDeviceInfo(IDataUploadResult data)
        {
            if (data is DataUploadResult result && result.IsSuccess && result.Type == UploadDataType.DeviceInfo)
            {
                if (result.Data != null && result.Data is TalkiPlayerDeviceInfo info)
                {
                    FirmwareVersion = info.FirmwareVersion;
                    HardwareVersion = info.HardwareVersion;
                    _hasDeviceInfo = true;
                    if (!_isReadySubject.Value)
                    {
                        _isReadySubject?.OnNext(true);
                    }
                }
            }

        }


        private void OnDisconnectCleanup()
        {
            _timeoutDisposable?.Dispose();
            _uploadDisposable?.Clear();
            _characteristsicsDisposable?.Clear();
            CleanupUpload();
            DataTransferOutput = null;
            _isReadySubject?.OnNext(false);
            _isPipeLineReadySubject?.OnNext(false);
            _hasDeviceInfo = false;

        }

        private void UpdateBatteryLevel(byte[] data)
        {
            if (data == null) return;

            BatteryLevel = (int)data[0];
        }

        private void UpdateBatteryStatus(byte[] data)
        {
            if (data == null) return;

            var status1 = (int)data[0];
            var status2 = (int)data[1];
            var status3 = (int)data[2];
            var status4 = (int)data[3];
         
            BatteryStatus = (BatteryPowerStatus) status3;
        }

        private async Task UploadFile(IUploadData data,  TimeSpan? timeout = null, CancellationToken cancelToken  = default(CancellationToken))
        {
            if (DataTransferOutput == null)
            {
                _logger.Information("There is no output characteristic available");
                throw new ArgumentNullException(nameof(DataTransferOutput), "There are no output characteristic established");
            }

            cancelToken.ThrowIfCancellationRequested();

            var finalData = new List<byte> {(byte) data.Type};

            if (data is IFileUploadData uploadData)
            {
                using (var stream = File.OpenRead(uploadData.Path))
                {
                    _logger.Information($"Name: {uploadData.Name}");
                    _logger.Information($"Size : {stream.Length}");
                    _logger.Information($"ExpectedNumberOfIteration : {stream.Length / MaxiOSMtuSize}");

                    await WriteDataInChunks(stream, data.Type, true, cancelToken: cancelToken);
                    
                    StartTimeoutTimerForUploadResponse(timeout);
                }

            }
            else if(data is IDataUploadData dataUploadData)
            {
                var json = !String.IsNullOrWhiteSpace(dataUploadData.DataJson) ? dataUploadData.DataJson : dataUploadData.Data?.ToJson(JsonSerializerSettings);
                var uploadingData = Encoding.UTF8.GetBytes(json ?? "");
                var length = uploadingData.Length;
                var unsignedInt = Convert.ToUInt32(length);
                var lengthBytes = BitConverter.GetBytes(unsignedInt);

                if(BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBytes);
                }

                finalData.AddRange(lengthBytes);
                finalData.AddRange(uploadingData);

                if (dataUploadData.Name == "GameData")
                {
                    _logger.Information($"Game Data json: {json}");
                }
                else
                {
                    _logger.Information($"json: {json}");
                }
                _logger.Information($"Size : {finalData.Count}");

                using (var stream = new MemoryStream(finalData.ToArray()))
                {
                    await WriteDataInChunks(stream, data.Type, cancelToken: cancelToken);
                }

                StartTimeoutTimerForUploadResponse(timeout);
            }
            else
            {
                throw new NotSupportedException();
                
            }
        }

        private async Task WriteDataInChunks(Stream stream, UploadDataType dataType, bool addType = false, CancellationToken cancelToken  = default(CancellationToken))
        {
            var maxByteSize = GetMaxByteSize;

            int byteSize;

            if(IsiOS)
            {

               byteSize = maxByteSize > MaxiOSMtuSize ? MaxiOSMtuSize : maxByteSize;
            }
            else
            {
               byteSize = maxByteSize > MaxMtuSize ? MaxMtuSize : maxByteSize;

            }

            var buffer = new byte[byteSize];
            int bytesRead;

            if (addType)
            {
                var initialBuffer = new byte[byteSize];
                bytesRead = await stream.ReadAsync(initialBuffer, 0, buffer.Length - 1, cancelToken);
                buffer[0] = (byte) dataType;
                var bufferCopy = initialBuffer.Mid(0, bytesRead);
                Array.Copy(bufferCopy, 0, buffer, 1, bufferCopy.Length);
            }
            else
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancelToken);
            }

            var i = 0;

            while (bytesRead > 0 && !cancelToken.IsCancellationRequested)
            {
                await DataTransferOutput.WriteWithoutResponse(buffer)
                  .RetryAfter(5, IsiOS ? TimeSpan.FromMilliseconds(100) : TimeSpan.Zero, RxApp.TaskpoolScheduler);

                if (cancelToken.IsCancellationRequested)
                {
                    _logger.Information($"NumberOfInteration: {i}");
                    break;
                }

                buffer =  new byte[byteSize];
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancelToken);

                if(IsiOS)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(150));
                }

                //_logger.Verbose($"BytesRead: {bytesRead}");

                i++;
            }

            _logger.Information($"NumberOfInteration: {i}");


        }
        private void StartTimeoutTimerForUploadResponse(TimeSpan? timeout = null)
        {
            _timeoutDisposable?.Dispose();
            _timeoutDisposable = null;
            _timeoutDisposable = Observable.Timer(timeout ?? TimeSpan.FromMinutes(5))
                .SubscribeSafe(_ =>
                { 
                    CleanupUpload();
                    var result = DataUploadResult.Failed(UploadData?.Tag, UploadData?.Type ?? UploadDataType.Unknown,
                        new TimeoutException());
                    _dataUploadSubject.OnNext(result);
                });
        }

        private void StartTimeoutTimerForConnection()
        {
            _connectionTimeoutDisposable?.Dispose();
            _connectionTimeoutDisposable = null;
            _connectionTimeoutDisposable = Observable.Timer(TimeSpan.FromMinutes(1))
                .SubscribeSafe(ex =>
                {
                    this.Device?.CancelConnection();
                    IsConnected = false;
                    _connectionFailedSubject?.OnNext(new TimeoutException());
                });
        }

     
      
        [Reactive]
        public bool Uploading { get; set; }

        public IObservable<IDataUploadResult> OnDataResult() =>  _dataUploadSubject;
        public IObservable<bool> IsReady => _isReadySubject;

        public IObservable<Exception> WhenConnectionFailed() => _connectionFailedSubject;
        public IObservable<bool> WhenDisconnected() => _disconnectSubject;
        public void CancelUpload()
        {
            CleanupUpload();
        }

        //public IObservable<Unit> WhenDisconnectedUponInactivity()
        //{
        //    return this.WhenDisconnected()
        //        .ObserveOn(RxApp.MainThreadScheduler)
        //        .Where(m => !m)
        //        .Where(m => !IsShakeTalkiPlayerPopupShown)
        //        .SelectMany(_ =>
        //        {
        //            IsShakeTalkiPlayerPopupShown = true;
        //            return _navigationService.PushPopup(new ShakeTalkiPlayerPopUpPageViewModel());
        //        })
        //        .Select(_ => Unit.Default);
        //}

      
        private bool IsUploading()
        {            
            return Uploading;
        }
        private IUploadData UploadData { get; set; }
        private IUploadData MetaData { get; set; }
        
        [Reactive]
        public bool IsShakeTalkiPlayerPopupShown { get; set; }
        private static JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings()
        {
            Formatting = Formatting.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter> { new StringEnumConverter(new CamelCaseNamingStrategy()) },
            TypeNameHandling = TypeNameHandling.None,
        };
       
        private void CleanupUpload()
        {
            Uploading = false;
            UploadData = null;
            MetaData = null;
        }
        
        private IGattCharacteristic DataTransferOutput { get; set; }

        public int? MtuSize { get; set; }


        public void UploadTagData(ItemDto item)
        {
            var request = new TagReadRequest
            {
                Id = item != null ? item.Id : 0,
                Name = item?.Name,
                IsHomeTag = item?.Type == ItemType.Home
            };

            CancelUpload();
            var uploadData = new DataUploadData(request.Name, request, $"{request.Id}", UploadDataType.TagData);            
            Upload(uploadData, TimeSpan.FromMinutes(10));
        }
    }
}
