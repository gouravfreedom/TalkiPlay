using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using Humanizer;
using Plugin.BluetoothLE;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public class DevicePageViewModel : BasePageViewModel, IActivatableViewModel
    {
        private readonly IConfig _config;
        private readonly IDownloadManagerExtended _downloadManagerExtended;
        private readonly ILogger _logger;
        private readonly IStorage _storage;
        private readonly ITalkiPlayerManager _talkiPlayerManager;
        private readonly IUserDialogs _userDialogs;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly SourceList<DeviceInfoBaseViewModel> _deviceinfoList = new SourceList<DeviceInfoBaseViewModel>();
        
        public DevicePageViewModel(INavigationService navigator,
            IUserDialogs userDialogs = null,
            ITalkiPlayerManager talkiPlayerManager = null,
            ILogger logger = null,
            IStorage storage = null,
            IDownloadManagerExtended downloadManagerExtended = null,
            IConfig config = null)
        {
            _config = config ?? Locator.Current.GetService<IConfig>();
            _downloadManagerExtended =
                downloadManagerExtended ?? Locator.Current.GetService<IDownloadManagerExtended>();
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            _storage = storage ?? Locator.Current.GetService<IStorage>();
             _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            Activator = new ViewModelActivator();
            Navigator = navigator ?? Locator.Current.GetService<INavigationService>(TabItemType.Settings.ToString());

            SetupCommands();
            SetupRx();
        }

        public override string Title => "My TalkiPlayer";
        public ViewModelActivator Activator { get; }

        private void SetupRx()
        {
            SetupDeviceInfo();
            
            this.WhenActivated(d =>
            {
                var myComparer = SortExpressionComparer<DeviceInfoBaseViewModel>.Ascending(p => p.Order);
                _deviceinfoList.Connect()
                    .Sort(myComparer, SortOptions.UseBinarySearch)
                    .Bind(out var deviceInfos)
                    .DisposeMany()
                    .Subscribe()
                    .DisposeWith(d);

                DeviceInfoList = deviceInfos;
                
                _talkiPlayerManager.Current?.WhenAnyValue(m => m.IsConnected)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(m => _logger.Information($"{Constants.DeviceName} is connected: {m}"))
                    .ToPropertyEx(this, v => v.IsConnected)
                    .DisposeWith(d);

                _talkiPlayerManager.Current?.WhenAnyValue(m => m.ConnectionStatus)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(m => _logger.Information($"{Constants.DeviceName} connection status: {m}"))
                    .ToPropertyEx(this, v => v.ConnectionStatus)
                    .DisposeWith(d);

                //_talkiPlayerManager.Current?.WhenDisconnectedUponInactivity()
                //    .SubscribeSafe()
                //    .DisposeWith(d);
                
                this.WhenAnyValue(m => m.IsConnected)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(m =>
                    {
                        if (m)
                        {
                            //_userDialogs.Toast($"Connected to the {Constants.DeviceName}");
                            SetupDeviceInfo();
                        }
                        else
                        {
                            //_userDialogs.Toast($"Disconnected from the {Constants.DeviceName}");
                        }
                    })
                    .DisposeWith(d);
                
                
                SetupDeviceInfoEvents(d);

                d.Add(_compositeDisposable);
            });
        }

        private void SetupDeviceInfoEvents(CompositeDisposable d)
        {
            _talkiPlayerManager.Current?.WhenAnyValue(m => m.FirmwareVersion)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(m =>
                {
                    var item = (DeviceInfoViewModel) _deviceinfoList.Items.FirstOrDefault(a =>
                        a.FieldName == nameof(ITalkiPlayer.FirmwareVersion));

                    if (item != null)
                    {
                        item.Value = $"{m}";
                    }
                })
                .DisposeWith(d);

            _talkiPlayerManager.Current?.WhenAnyValue(m => m.HardwareVersion)
                .Subscribe(m =>
                {
                    var item = (DeviceInfoViewModel)_deviceinfoList.Items.FirstOrDefault(a =>
                       a.FieldName == nameof(ITalkiPlayer.HardwareVersion));

                    if (item != null)
                    {
                        item.Value = $"{m}";
                    }
                })
                .DisposeWith(d);

            _talkiPlayerManager.Current?.WhenAnyValue(m => m.BatteryLevel)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(m =>
                {
                    var item = (DeviceInfoViewModel) _deviceinfoList.Items.FirstOrDefault(a =>
                        a.FieldName == nameof(ITalkiPlayer.BatteryLevel));

                    if (item != null)
                    {
                        item.Value = $"{m}";
                    }
                })
                .DisposeWith(d);

            _talkiPlayerManager.Current?.WhenAnyValue(m => m.BatteryStatus)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(m =>
                {  
                    var item = (DeviceInfoViewModel) _deviceinfoList.Items.FirstOrDefault(a =>
                        a.FieldName == nameof(ITalkiPlayer.BatteryStatus));

                    if (item != null)
                    {
                        item.Value = $"{m}";
                    }
                })
                .DisposeWith(d);

            _talkiPlayerManager.Current?.OnDataResult()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(a => a.Type == UploadDataType.EraseFlash)
                .SelectMany(m => Observable.FromAsync(() => Task.Delay(TimeSpan.FromSeconds(1))).Select(_ => m))
                .HideLoading()
                .SubscribeSafe(m =>
                {
                   //Bug in the talkiplayer that even though erases the content
                   //still returns error so assume that it's successful regardless of reporting for now
                   //if (m.IsSuccess)
                   //{
                        _userDialogs.Alert(new AlertConfig()
                        {
                            Title = "Contents reset",
                            Message = $"Successfully erased all {Constants.DeviceName} contents"
                        });
                    //}
                    //else
                    // {
                    //      _userDialogs.Alert(new AlertConfig()
                    //      {
                    //          Title = "Reset all contents",
                    //          Message = "Failed to reset contents of TalkiPlayer. Please try again!",
                    //          OkText =  "Ok",
                    //      });
                    // }
                })
                .DisposeWith(d);

            _talkiPlayerManager.Current?.OnDataResult()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(a => a.Type == UploadDataType.DeepSleep)
                .SelectMany(m => Observable.FromAsync(() => Task.Delay(TimeSpan.FromSeconds(1))).Select(_ => m))
                .HideLoading()
                .SubscribeSafe((m) =>
                {
                    var config = new AlertConfig()
                    {
                        Title = "Travel Mode",
                        Message = $"Successfully put {Constants.DeviceName} into deep sleep."
                    };
                    config.SetAction(async () =>
                    {
                        await SimpleNavigationService.PopModalRootAsync(true);
                    });
                    _userDialogs.Alert(config);
                    
                })
                .DisposeWith(d);
        }

        private void SetupCommands()
        {
            AskStoragePermissionCommand = ReactiveCommand.CreateFromTask (async () =>
            {
                var granted = await _storage.HasStoragePermission();
                if (!granted)
                {
                    granted = await _storage.RequestStoragePermission();
                }

                IsStoragePermissionGranted = granted;

          
                if (granted && !_downloadManagerExtended.IsDownloadManagerEnabled())
                {
                    // show dialog to enable download manager
                    
                    _downloadManagerExtended.OpenSettingsToEnableDownloadManager();
                }
                
            });

            AskStoragePermissionCommand.ThrownExceptions.SubscribeAndLogException();


            var canUpload = this.WhenAnyValue( m => m.IsStoragePermissionGranted, m => m.IsConnected,
                (connected, storage) => connected && storage && _downloadManagerExtended.IsDownloadManagerEnabled());

            UpdateCommand = ReactiveCommand.CreateFromObservable<CheckForUpdateViewModel, Unit>( vm => 
                vm.UpdateCommand.Execute(), canUpload);

            UpdateCommand.ThrownExceptions.SubscribeAndLogException();

            ConnectCommand = ReactiveCommand.Create(() =>
            {
                if (_talkiPlayerManager.Current != null)
                {
                    if (IsConnected)
                    {
                        _talkiPlayerManager.Current?.Disconnect().Subscribe();
                    }
                    else
                    {
                        _talkiPlayerManager.Current?.Connect().Subscribe();
                    }
                }
            });

            ConnectCommand.ThrownExceptions.SubscribeAndLogException();
      
            
            BackCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SimpleNavigationService.PopModalAsync(true, SimpleNavigationService.TopModalPage.Navigation.ModalStack.Count);
            });
            
            BackCommand.ThrownExceptions.SubscribeAndLogException();
        }

        private void SetupDeviceInfo()
        {
            var device = _talkiPlayerManager.Current;
            if (!IsConnected || device == null) return;
            
            _deviceinfoList.Clear();

            _deviceinfoList.Edit(list =>
            {
                list.Add(new CheckForUpdateViewModel()
                {
                    Label = "Change my volume",
                    Order = 3,
                    Type = UpdateType.Volume,
                    UpdateCommand = ReactiveCommand.CreateFromObservable(() =>
                    SimpleNavigationService.PushChildModalPage(new VolumeControlPageViewModel(null)).ToObservable(), outputScheduler: RxApp.MainThreadScheduler)
                });

                list.Add(new CheckForUpdateViewModel()
                {
                    Label = "Update my TalkiPlayer",
                    Order = 1,
                    Type = UpdateType.Firmware,
                    UpdateCommand = ReactiveCommand.CreateFromObservable(() =>
                        SimpleNavigationService.PushChildModalPage(new FirmwareUpdatePageViewModel(null)).ToObservable(), outputScheduler: RxApp.MainThreadScheduler)
                });

                list.Add(new CheckForUpdateViewModel()
                {
                    Label = "Audio pack update",
                    Order = 2,
                    Type = UpdateType.Audio,
                    UpdateCommand = ReactiveCommand.CreateFromObservable(() =>
                        SimpleNavigationService.PushChildModalPage(new AudioPackListPageViewModel(null)).ToObservable(), outputScheduler: RxApp.MainThreadScheduler)
                });

                var canExecute = this.WhenAnyValue(m => m.IsConnected).ObserveOn(RxApp.MainThreadScheduler).Select(m => m);

                list.Add(new CheckForUpdateViewModel()
                {
                    Label = "Erase all contents",
                    Order = 4,
                    Type = UpdateType.EraseFlash,
                    UpdateCommand = ReactiveCommand.CreateFromTask(async () =>
                    {
                        var ok = await _userDialogs.ConfirmAsync(new ConfirmConfig()
                        {
                            Title = "Are you sure you want to proceed?",
                            Message = $"This will erase all contents in the {Constants.DeviceName}",
                            OkText = "Erase Now",
                            CancelText = "Cancel"
                        });

                        if (ok)
                        {
                            device.Upload(new DataUploadData("EraseFlash", DataRequest.EraseFlashRequest(), "Erase", UploadDataType.EraseFlash));
                            _userDialogs.ShowLoading("Erasing ...");
                        }

                    }, canExecute)
                });

                list.Add(new CheckForUpdateViewModel()
                {
                    Label = "Travel mode",
                    Order = 4,
                    Type = UpdateType.DeepSleep,
                    UpdateCommand = ReactiveCommand.CreateFromTask(async () =>
                    {
                        var ok = await _userDialogs.ConfirmAsync(new ConfirmConfig()
                        {
                            Title = "Are you sure you want to proceed?",
                            Message = $"This will put {Constants.DeviceName} into deep sleep and will not wake up until plugged in",
                            OkText = "Yes",
                            CancelText = "Cancel"
                        });

                        if (ok)
                        {
                            device.Upload(new DataUploadData("DeepSleep", DataRequest.DeepSleepRequest(), "DeepSleep", UploadDataType.DeepSleep));
                            _userDialogs.ShowLoading($"Putting {Constants.DeviceName} into deep sleep ...");
                        }

                    }, canExecute)
                });

                list.Add(new DeviceInfoViewModel()
                {
                    Label = "Device name",
                    Value = device?.Name,
                    Order = 4,
                    FieldName = nameof(ITalkiPlayer.Name)
                });

//                list.Add(new DeviceInfoViewModel()
//                {
//                    Label = "System Id",
//                    Value = _talkiPlayerManager.Current.SystemId,
//                    Order = 2,
//                    FieldName = nameof( _talkiPlayerManager.Current.SystemId)
//
//                });

                list.Add(new DeviceInfoViewModel()
                {
                        Label = "Firmware version",
                        Value =   device?.FirmwareVersion,
                        Order = 5,
                        FieldName = nameof(ITalkiPlayer.FirmwareVersion)
                });

                list.Add(new DeviceInfoViewModel()
                {
                    Label = "Hardware version",
                    Value = device?.HardwareVersion,
                    Order = 6,
                    FieldName = nameof(ITalkiPlayer.HardwareVersion)
                });

                //                list.Add(new DeviceInfoViewModel()
                //                {
                //                    Label = "Model version",
                //                    Value =   _talkiPlayerManager.Current.ModelVersion,
                //                    Order = 6,
                //                    FieldName = nameof( _talkiPlayerManager.Current.ModelVersion)
                //                });
                //                    
                //                list.Add(new DeviceInfoViewModel()
                //                {
                //                    Label = "Serial #",
                //                    Value =   _talkiPlayerManager.Current.SerialNumber,
                //                    Order = 7,
                //                    FieldName = nameof( _talkiPlayerManager.Current.SerialNumber)
                //                });

                //TODO: Show Battery status when TalkiPlayer supported
                // list.Add(new DeviceInfoViewModel()
                // {
                //     Label = "Battery level",
                //     Value = $"{_talkiPlayerManager.Current.BatteryLevel}%",
                //     Order = 7,
                //     FieldName = nameof( _talkiPlayerManager.Current.BatteryLevel)
                // });
                //     
                // list.Add(new DeviceInfoViewModel()
                // {
                //     Label = "Battery status",
                //     Value = $"{_talkiPlayerManager.Current.BatteryStatus.Humanize()}",
                //     Order = 7,
                //     FieldName = nameof( _talkiPlayerManager.Current.BatteryStatus)
                // });
                //
                
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (Config.IsInTestMode)
#pragma warning disable 162
                {
                    list.Add(new CheckForUpdateViewModel()
                    {
                        Label =$"{Constants.DeviceName} Tests",
                        Order = 0,
                        Type = UpdateType.Debug,
                        UpdateCommand = ReactiveCommand.CreateFromObservable(() =>
                            SimpleNavigationService.PushChildModalPage(new TestBleRequestPageViewModel(null)).ToObservable(), outputScheduler: RxApp.MainThreadScheduler)
                    });
                }
#pragma warning restore 162

                list.Add(new CheckForUpdateViewModel()
                {
                    Label = "Audio assets update",
                    Order = 0,
                    Type = UpdateType.Debug,
                    UpdateCommand = ReactiveCommand.CreateFromObservable(() =>
                        SimpleNavigationService.PushChildModalPage(new AudioListPageViewModel(null)).ToObservable(), outputScheduler: RxApp.MainThreadScheduler)
                });

                list.Add(new CheckForUpdateViewModel()
                {
                    Label = "Add home tag",
                    Order = 0,
                    Type = UpdateType.Audio,
                    UpdateCommand = ReactiveCommand.CreateFromObservable(() =>
                        SimpleNavigationService.PushChildModalPage(new DeviceSetupPageViewModel(DeviceSetupSource.Settings)).ToObservable(), outputScheduler: RxApp.MainThreadScheduler)
                });
            });
        }

        public ReactiveCommand<CheckForUpdateViewModel, Unit> UpdateCommand { get; private set; }
        
        public ReactiveCommand<Unit, Unit> ConnectCommand { get; private set; }
        
        public ReactiveCommand<Unit, Unit> AskStoragePermissionCommand { get; private set; }
        
        public extern bool IsConnected { [ObservableAsProperty] get; }
        public extern ConnectionStatus ConnectionStatus { [ObservableAsProperty] get; }

        public ReadOnlyObservableCollection<DeviceInfoBaseViewModel> DeviceInfoList { get; private set; }
        
        [Reactive] public bool IsStoragePermissionGranted { get; set; }
 
     
    }
}