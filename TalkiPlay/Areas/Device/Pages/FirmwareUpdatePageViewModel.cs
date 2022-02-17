using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using Plugin.BluetoothLE;
using Plugin.DownloadManager.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;
using Unit = System.Reactive.Unit;

namespace TalkiPlay.Shared
{
    public class FirmwareUpdatePageViewModel : BasePageViewModel, IActivatableViewModel
    {
        private readonly IConfig _config;
        private readonly ILogger _logger;
        private readonly ITalkiPlayerManager _talkiPlayerManager;
        private readonly IUserDialogs _userDialogs;
        private readonly IFirmwareService _firmwareService;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private IDisposable _scanDisposable;

        public override string Title => "Software Update";
        public ViewModelActivator Activator { get; }

        public enum UpdateStep
        {
            DeviceNotReady,
            CheckingUpdate,
            CheckingFailed,
            NoUpdateRequire,
            UpdateAvailable,
            ReadyToUpdate,
            Downloading,
            DownloadFailed,
            Uploading,
            UploadFailed,
            UpdateSuccess,
        }

        public ReactiveCommand<Unit, Unit> CommandNoUpdateRequire { get; private set; }
        public ReactiveCommand<Unit, Unit> CommandDeviceNotReady { get; private set; }
        public ReactiveCommand<Unit, Unit> CommandUpdateAvailable { get; private set; }
        public ReactiveCommand<Unit, Unit> CommandReadyToUpdate { get; private set; }
        public ReactiveCommand<Unit, Unit> CommandUpdateSuccess { get; private set; }
        public ReactiveCommand<Unit, Unit> CommandUpdateFailed { get; private set; }

        [Reactive] public String CurrentVersion { get; private set; } = "v1.0.0";
        [Reactive] public String NewVersion { get; private set; } = "v2.0.0";

        [Reactive] public String DeviceNotReadyTitle { get; private set; } = "Device not connected";
        [Reactive] public String DeviceNotReadyMessage { get; private set; } = "Please connect device to continue update";
        [Reactive] public String DeviceNotReadyImage { get; private set; } = Images.TpSad;
        [Reactive] public bool DeviceNotReadyAndConnected { get; private set; }
        [Reactive] public bool DeviceNotReadyAndDisconnected { get; private set; }

        private UpdateStep? _currentStep = null;
        public UpdateStep? CurrentStep
        {
            get => _currentStep;
            set
            {
                if (value != _currentStep)
                {
                    this.RaiseAndSetIfChanged(ref _currentStep, value);
                    this.RaisePropertyChanged(nameof(IsDeviceNotReady));
                    this.RaisePropertyChanged(nameof(IsCheckingUpdate));
                    this.RaisePropertyChanged(nameof(IsNoUpdateAvailable));
                    this.RaisePropertyChanged(nameof(IsUpdateAvailable));
                    this.RaisePropertyChanged(nameof(IsReadyToUpdate));
                    this.RaisePropertyChanged(nameof(IsUpdating));
                    this.RaisePropertyChanged(nameof(IsUpdateSuccess));
                    this.RaisePropertyChanged(nameof(IsUpdateFailed));
                }
            }
        }

        public bool IsDeviceNotReady => _currentStep == UpdateStep.DeviceNotReady;
        public bool IsCheckingUpdate => _currentStep == UpdateStep.CheckingUpdate;
        public bool IsNoUpdateAvailable => _currentStep == UpdateStep.NoUpdateRequire;
        public bool IsUpdateAvailable => _currentStep == UpdateStep.UpdateAvailable;
        public bool IsReadyToUpdate => _currentStep == UpdateStep.ReadyToUpdate;
        public bool IsUpdating => _currentStep == UpdateStep.Uploading || _currentStep == UpdateStep.Downloading;
        public bool IsUpdateSuccess => _currentStep == UpdateStep.UpdateSuccess;
        public bool IsUpdateFailed => _currentStep == UpdateStep.CheckingFailed
            || _currentStep == UpdateStep.DownloadFailed
            || _currentStep == UpdateStep.UploadFailed;

        public FirmwareUpdatePageViewModel(
            INavigationService navigator,
            IUserDialogs userDialogs = null,
            IFirmwareService firmwareService = null,
            ITalkiPlayerManager talkiPlayerManager = null,
            ILogger logger = null,
            IConfig config = null
            )
        {
            _config = config ?? Locator.Current.GetService<IConfig>();
            Activator = new ViewModelActivator();
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _firmwareService = firmwareService ?? Locator.Current.GetService<IFirmwareService>();
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            Activator = new ViewModelActivator();
            Navigator = navigator ?? Locator.Current.GetService<INavigationService>(TabItemType.Settings.ToString());

            SetupRx();
            SetupCommands();
        }

        private void MoveToDeviceNotReadyState(String title, String msg, bool isConnected = false)
        {
            DeviceNotReadyTitle = title;
            DeviceNotReadyMessage = msg;
            DeviceNotReadyAndConnected = isConnected;
            DeviceNotReadyAndDisconnected = !isConnected;
            CurrentStep = UpdateStep.DeviceNotReady;
        }
        private void SetupCommands()
        {
            BackCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var isConnected = _talkiPlayerManager.Current?.IsConnected ?? false;
                if (IsUpdateSuccess && !isConnected)
                {
                    await SimpleNavigationService.PopModalRootAsync();
                }
                else
                {
                    await SimpleNavigationService.PopModalAsync();
                    //await SimpleNavigationService.PopAsync();
                }
            });

            BackCommand.ThrownExceptions
                .HideLoading()
                .SubscribeAndLogException();

            CommandNoUpdateRequire = BackCommand;
            CommandUpdateSuccess = BackCommand;
            CommandDeviceNotReady = BackCommand;

            CommandCheckUpdate = ReactiveCommand.CreateFromTask(async () =>
            {
                if (CurrentStep == UpdateStep.CheckingUpdate) return;
                CurrentVersion = _talkiPlayerManager.Current.FirmwareVersion;
                await Xamarin.Forms.Device.InvokeOnMainThreadAsync(() => CurrentStep = UpdateStep.CheckingUpdate);
                var result = await _firmwareService.GetLatestFirmware();
                NewVersion = result.Version;
                FileDataInfo = result;
                var currentfirmwareVersion = _talkiPlayerManager.Current.FirmwareVersion.ToSoftwareVersion();
                var firmwareVersion = result.Version.ToSoftwareVersion();
                CurrentStep = currentfirmwareVersion.CompareTo(firmwareVersion) >= 0 ? UpdateStep.NoUpdateRequire : UpdateStep.UpdateAvailable;
            });

            CommandCheckUpdate.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .Do(_ => CurrentStep = UpdateStep.CheckingFailed)
                .SubscribeAndLogException();

            CommandUpdateAvailable = ReactiveCommand.CreateFromTask(() =>
            {
                CurrentStep = UpdateStep.ReadyToUpdate;
                return Task.CompletedTask;
            });

            var canInstall = this.WhenAnyValue(m => m.BatteryLevel, m => m.BatteryStatus,
                (level, status) => level >= 80 || status == BatteryPowerStatus.Charging);

            CommandReadyToUpdate = ReactiveCommand.CreateFromTask(async () =>
            {
                if (CurrentStep == UpdateStep.Downloading) return;
                CurrentStep = UpdateStep.Downloading;
                UpdateStatus = FirmwareUpdateStatus.Downloading;
                var result = FileDataInfo;
                var downloadResult = await _firmwareService.DownloadLatestFirmware(result);
                FirmwareDownloadResult = downloadResult;
                SetupDownloadMonitor();
                FirmwareDownloadResult.OnPropertyChanged();
            }, canInstall);

            CommandReadyToUpdate.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .Do(_ =>
                {
                    CurrentStep = UpdateStep.DownloadFailed;
                })
                .SubscribeAndLogException();

            var canUpload = this.WhenAnyValue(m => m.FirmwareDownloadResult)
                .Select(m => m != null && m.Status == DownloadFileStatus.COMPLETED);

            UploadCommand = ReactiveCommand.CreateFromTask(() =>
            {
                if (CurrentStep == UpdateStep.Uploading)
                {
                    return Task.CompletedTask;
                }
                CurrentStep = UpdateStep.Uploading;
                UpdateStatus = FirmwareUpdateStatus.Uploading;

                var isCheckSumOk = IsCheckSumMatch();

                if (!isCheckSumOk)
                {
                    CurrentStep = UpdateStep.DownloadFailed;
                    UpdateStatus = FirmwareUpdateStatus.Failed;
                    return Task.CompletedTask;
                }

                var uploadTask = new FileUploadData("firmware.bin", FileDataInfo.FileSize, FileDataInfo.Checksum,
                    FirmwareDownloadResult.DestinationPathName, Guid.NewGuid().ToString(), UploadDataType.Firmware);
                _talkiPlayerManager.Current?.CancelUpload();
                _talkiPlayerManager.Current?.Upload(uploadTask, TimeSpan.FromMinutes(30));
                return Task.CompletedTask;
            }, canUpload);

            UploadCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .Do(ex =>
                {
                    CurrentStep = UpdateStep.UploadFailed;
                })
                .SubscribeAndLogException();

            CommandUpdateFailed = ReactiveCommand.CreateFromObservable(() => {
                switch (_currentStep)
                {
                    case UpdateStep.DeviceNotReady:
                        Observable.Start(() => _talkiPlayerManager.Current?.Connect());
                        break;
                    case UpdateStep.CheckingFailed:
                        return CommandCheckUpdate.Execute();
                    case UpdateStep.DownloadFailed:
                        return CommandReadyToUpdate.Execute();
                    case UpdateStep.UploadFailed:
                        return UploadCommand.Execute();
                }
                return Observable.Return(Unit.Default);
            });

            CommandUpdateFailed.ThrownExceptions.SubscribeAndLogException();
        }

        private bool IsCheckSumMatch()
        {
            return _firmwareService.IsCheckSumMatch(FirmwareDownloadResult.DestinationPathName, FileDataInfo.Checksum,
                FileDataInfo.FileSize);

        }

        private void SetupRx()
        {
            this.WhenActivated(d =>
            {
                _talkiPlayerManager.Current?.OnDataResult()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .HideLoading()
                     .SubscribeSafe(m =>
                     {
                         if (m.Type == UploadDataType.Firmware)
                         {
                             if (m.IsSuccess)
                             {
                                 UpdateStatus = FirmwareUpdateStatus.Uploaded;
                                 CurrentStep = UpdateStep.UpdateSuccess;
                             }
                             else
                             {
                                 UpdateStatus = FirmwareUpdateStatus.Failed;
                                 CurrentStep = UpdateStep.UploadFailed;
                                 _logger.Error((m.GetData<ErrorDataResult>())?.Exception);
                             }
                         }
                     })
                     .DisposeWith(d);

                _talkiPlayerManager.Current?.Device?.WhenConnectionFailed()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ =>
                    {
                        MoveToDeviceNotReadyState($"{Constants.DeviceName} is disconnected", $"Please connect {Constants.DeviceName} to continue update.");
                    })
                    .DisposeWith(d);

                _talkiPlayerManager.Current?.WhenAnyValue(m => m.IsConnected)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(isConnected => {
                        if (!isConnected)
                        {
                            MoveToDeviceNotReadyState($"{Constants.DeviceName} is not connected", $"Please connect {Constants.DeviceName} to continue update.");
                        }
                        _logger.Information($"{Constants.DeviceName} is connected: {isConnected}");
                    }).ToPropertyEx(this, v => v.IsConnected)
                    .DisposeWith(d);

                _talkiPlayerManager.Current?.WhenAnyValue(m => m.ConnectionStatus)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(m => _logger.Information($"{Constants.DeviceName} connection status: {m}"))
                    .Do(m =>
                    {
                        if (m == ConnectionStatus.Disconnected && UpdateStatus == FirmwareUpdateStatus.Uploaded)
                        {
                            _logger.Warning("Device has been updated");
                            DeviceFound = null;
                            StartScan();
                            UpdateStatus = FirmwareUpdateStatus.Updating;
                        }
                    })
                    .ToPropertyEx(this, v => v.ConnectionStatus)
                    .DisposeWith(d);

                _talkiPlayerManager.Current?.WhenAnyValue(m => m.BatteryLevel)
                    .ObserveOn(RxApp.MainThreadScheduler)
                     .ToPropertyEx(this, v => v.BatteryLevel)
                    .DisposeWith(d);

                _talkiPlayerManager.Current?.WhenAnyValue(m => m.BatteryStatus)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToPropertyEx(this, v => v.BatteryStatus)
                    .DisposeWith(d);

                _talkiPlayerManager.Current?.WhenAnyValue(m => m.FirmwareVersion)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(version => {
                        if (ConnectionStatus == ConnectionStatus.Connected
                        && CurrentStep == UpdateStep.DeviceNotReady
                        && UpdateStatus >= FirmwareUpdateStatus.Uploaded)
                        {
                            UpdateStatus = FirmwareUpdateStatus.Verifying;
                            CommandCheckUpdate.Execute().Subscribe();
                        }
                    })
                    .ToPropertyEx(this, v => v.FirmwareVersion)
                    .DisposeWith(d);

                this.WhenAnyValue(m => m.BatteryLevel, m => m.BatteryStatus,
                        (level, status) =>
                        {
                            if (level >= 80 || status == BatteryPowerStatus.Charging)
                            {
                                return "";
                            }

                            return
                                $"{Constants.DeviceName} does not have enough power left to go through this software update. Please plugin in to the charger before proceed with software update.";
                        })
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(m => {
                        if (!string.IsNullOrEmpty(m))
                        {
                            DeviceNotReadyTitle = $"{Constants.DeviceName} requires charging";
                            DeviceNotReadyMessage = m;
                        }
                    }).DisposeWith(d);

                this.WhenAnyValue(m => m.ConnectionStatus)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(m =>
                    {
                        switch (m)
                        {
                            case ConnectionStatus.Connecting:
                                DeviceNotReadyTitle = $"Connecting to {Constants.DeviceName}";
                                DeviceNotReadyMessage = $"Please wait, connecting to {Constants.DeviceName}...";
                                break;
                            case ConnectionStatus.Connected:
                                DeviceNotReadyTitle = $"Connected to {Constants.DeviceName}";
                                DeviceNotReadyMessage = $"Please wait, app will look for update soon...";
                                if (CurrentStep == UpdateStep.DeviceNotReady && UpdateStatus >= FirmwareUpdateStatus.Uploaded)
                                {
                                    MoveToDeviceNotReadyState(DeviceNotReadyTitle, DeviceNotReadyMessage, true);
                                }
                                break;
                            case ConnectionStatus.Disconnecting:
                                DeviceNotReadyTitle = $"Disconnecting from {Constants.DeviceName}";
                                DeviceNotReadyMessage = $"Disconnecting from {Constants.DeviceName}, app will try to reconnect once in proximity.";
                                break;
                            case ConnectionStatus.Disconnected:
                                DeviceNotReadyTitle = $"Disconnected from {Constants.DeviceName}";
                                DeviceNotReadyMessage = $"Disconnecting from {Constants.DeviceName}, app will try to reconnect once in proximity.";
                                break;
                        }
                    })
                    .DisposeWith(d);

                this.WhenAnyValue(m => m.DeviceFound)
                    .Where(a => a != null)
                    .Do(device => StopScan())
                    .Do(device => _talkiPlayerManager.Current?.Connect())
                    .SubscribeSafe()
                    .DisposeWith(d);

                this.WhenAnyValue(a => a.IsConnected, a => a.DeviceFound, a => a.UpdateStatus,
                   (connected, deviceFound, status) => (connected, deviceFound, status))
                   .Where(m => m.deviceFound != null && m.deviceFound.Value && m.connected && m.status != FirmwareUpdateStatus.Verifying)
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Select(m => Unit.Default)
                   .Do(a => {
                       if (UpdateStatus == FirmwareUpdateStatus.Updating)
                       {
                           UpdateStatus = FirmwareUpdateStatus.Verifying;
                       }
                   }).InvokeCommand(this, m => m.CommandCheckUpdate)
                   .DisposeWith(d);
            });
        }

        [Reactive] public IDownloadFileResult FirmwareDownloadResult { get; set; }
        private IFileData FileDataInfo { get; set; }

        public ReactiveCommand<Unit, Unit> CommandCheckUpdate { get; private set; }
        public ReactiveCommand<Unit, Unit> UploadCommand { get; private set; }

        [Reactive] public string MiscMessage { get; set; }


        public extern int BatteryLevel { [ObservableAsProperty] get; }
        public extern BatteryPowerStatus BatteryStatus { [ObservableAsProperty] get; }

        public extern bool IsConnected { [ObservableAsProperty] get; }
        public extern ConnectionStatus ConnectionStatus { [ObservableAsProperty] get; }

        public extern String FirmwareVersion { [ObservableAsProperty] get; }

        private void SetupDownloadMonitor()
        {
            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => FirmwareDownloadResult.PropertyChanged += h, h => FirmwareDownloadResult.PropertyChanged -= h)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(e =>
                {
                    if (!(e.Sender is IDownloadFile file)) return;

                    _logger?.Information(
                        $"Total Downloaded: {file.TotalBytesWritten}, Status: {file.Status}, Path: {file.DestinationPathName}");

                    if (file.Status == DownloadFileStatus.RUNNING)
                    {
                        if (file.TotalBytesWritten > 0)
                        {
                            var progress = (double)(file.TotalBytesWritten /
                                                     FileDataInfo.FileSize) * 100;
                        }
                    }

                    if (e.EventArgs.PropertyName.Equals(nameof(IDownloadFile.Status)))
                    {
                        switch (file.Status)
                        {
                            case DownloadFileStatus.FAILED:
                                UpdateStatus = FirmwareUpdateStatus.Failed;
                                CurrentStep = UpdateStep.DownloadFailed;
                                break;
                            case DownloadFileStatus.CANCELED:
                                UpdateStatus = FirmwareUpdateStatus.Cancelled;
                                CurrentStep = UpdateStep.DownloadFailed;
                                break;
                            case DownloadFileStatus.COMPLETED:
                                UploadCommand.Execute().Subscribe();
                                break;
                        }
                    }
                })
                .DisposeWith(_compositeDisposable);
        }

        private void StartScan()
        {
            CrossBleAdapter.Current.StopScan();
            CrossBleAdapter.Current.WhenStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(m =>
                {
                    if (m == AdapterStatus.PoweredOff)
                    {
                        return Observable.Start(() =>
                        {
                            if (CrossBleAdapter.Current.CanOpenSettings())
                            {
                                CrossBleAdapter.Current.OpenSettings();
                            }
                        }).Select(_ => m);

                    }
                    else if (m == AdapterStatus.Unauthorized)
                    {
                        return Observable.StartAsync(async () =>
                        {
                            var vm = new BluetoothWarningPopupPageViewModel(Navigator)
                            {
                                ButtonText = "Open settings",
                                Message = "You have not grant the Bluetooth usage permission. Talki app requires bluetooth connection. Please go to settings and grant bluetooth usage",
                                OpenSettings = () =>
                                {
                                    var applicationService = Locator.Current.GetService<IApplicationService>();
                                    applicationService.OpenSettings();
                                }

                            };

                            SimpleNavigationService.PushPopupAsync(vm).Forget();
                        }).Select(_ => m);
                    }
                    else
                    {
                        return Observable.Return(m);
                    }
                })
                .Where(m => m == AdapterStatus.PoweredOn)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(status =>
                {
                    _scanDisposable = CrossBleAdapter.Current.Scan()
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Subscribe(scanResult =>
                    {
                        if (!String.IsNullOrWhiteSpace(scanResult.Device.Name)
                                && scanResult.Device.Name == _talkiPlayerManager.Current?.Name
                                 && scanResult.Device.Name.Contains(_config.DeviceNamePrefix)
                                )
                        {
                            DeviceFound = true;
                        }
                    }, ex =>
                    {
                        var logger = Locator.Current.GetService<ILogger>();
                        logger?.Error(ex);

                    });
                }, ex =>
                {
                    var logger = Locator.Current.GetService<ILogger>();
                    logger?.Error(ex);
                });
        }

        private void StopScan()
        {
            try
            {

                CrossBleAdapter.Current.StopScan();
                _scanDisposable?.Dispose();
                //_connectionTimeoutDisposable?.Dispose();

            }
            catch (Exception ex)
            {
                ex.LogException("StopScan");
            }
        }

        [Reactive] public bool? DeviceFound { get; set; }

        [Reactive] public FirmwareUpdateStatus UpdateStatus { get; set; }

        public enum FirmwareUpdateStatus
        {
            Pending,
            Downloading,
            Uploading,
            Uploaded,
            Updating,
            Cancelled,
            Verifying,
            Failed,
        }
    }
}