﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using Plugin.BluetoothLE;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using DynamicData;
using ILogger = ChilliSource.Mobile.Core.ILogger;
using Unit = System.Reactive.Unit;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;

namespace TalkiPlay.Shared
{
    public class DeviceListPageViewModel : BasePageViewModel, IActivatableViewModel, IModalViewModel
    {
        private readonly ReactiveCommand<INavigationService, Unit> _backCommand;
        private readonly ConnectionConfig _connectionConfig;
        private readonly ITalkiPlayerManager _talkiPlayerManager;
        private readonly IConfig _config;
        private readonly IUserDialogs _userDialogs;
        private IDisposable _scanDisposable;
        private IDisposable _connectionTimeoutDisposable;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly SourceList<DeviceItemViewModel> _deviceSourceList = new SourceList<DeviceItemViewModel>();
        //private IDisposable _talkiplayerSearchTimeoutDisposable;
        public DeviceListPageViewModel(INavigationService navigator, 
            ReactiveCommand<INavigationService, Unit> navigateCommand = null,
            ReactiveCommand<INavigationService, Unit> backCommand = null,
            IUserDialogs userDialogs = null,
            IConfig config = null,
            ITalkiPlayerManager talkiPlayerManager = null,
            ConnectionConfig connectionConfig = null)
        {
            if (SimpleNavigationService.IsGoingBack)
            {
                return;
            }
            _backCommand = backCommand;
            _connectionConfig = connectionConfig;
            NavigateCommand = navigateCommand;
            _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _config = config ?? Locator.Current.GetService<IConfig>();
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            Activator = new ViewModelActivator();
            Navigator = navigator;
            SetupCommands();
            SetupRx();
            SetupInstructions();
        }

        public List<TalkiPlayerInstructionItemViewModel> Instructions { get; private set; }
        
        public ViewModelActivator Activator { get; }

        public ReactiveCommand<IDevice, Unit> ConnectCommand { get; private set; }

        public ReactiveCommand<DeviceItemViewModel, Unit> SelectCommand { get; private set; }

        private ReactiveCommand<INavigationService, Unit> NavigateCommand { get; set; }
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; set; }

        public ReactiveCommand<Unit, Unit> OpenAccountSettings { get; set; }

        [Reactive]
        public bool ShowEmptyState { get; set; } = true;

        [Reactive]
        public bool IsRefreshing { get; set; }

        [Reactive]
        public bool LeftMenuEnabled { get; set; } = true;

        public bool CanOpenDeviceDetailsWhenAlreadyConnected { get; set; } = true;

        public override string Title => "Connect device";

        public ReadOnlyObservableCollection<DeviceItemViewModel> Devices { get; private set; }

        void SetupInstructions()
        {
            Instructions = new List<TalkiPlayerInstructionItemViewModel>();
            Instructions.Add(new TalkiPlayerInstructionItemViewModel()
            {
                Header = $"Shake {Constants.DeviceName} until you hear the sound",
                Image = Images.ShakeTalkiPlayerImage
            });

            Instructions.Add(new TalkiPlayerInstructionItemViewModel()
            {
                Header = $"Tap any tag to activate",
                Image = Images.TapTagTalkiPlayerImage
            });
        }

        void SetupRx()
        {
            this.WhenActivated(d =>
            {
                _deviceSourceList.Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out var devices)
                    .DisposeMany()
                    .Subscribe()
                    .DisposeWith(d);

                Devices = devices;

                _deviceSourceList.Connect().WhenPropertyChanged(m => m.Selected)
                    .Where(m => m.Sender != null)
                    .Where(m => m.Sender.Selected)
                    .Select(m => m.Sender)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(selected =>
                    {
                        var items = Devices.Where(m => m.Selected && m.Name != selected.Name);
                        foreach (var i in items)
                        {
                            i.Selected = false;
                        }
                    })
                    .Select(m => m.BleDevice)
                    .Do(m =>
                    {
                        _talkiPlayerManager.SetTalkiPlayer(m);
                        
                        _talkiPlayerManager.Current?.WhenAnyValue(a => a.IsConnected)
                            .Where(a => a)
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Do(_ =>
                            {
                                _connectionTimeoutDisposable?.Dispose();
                            })
                            .HideLoading()
                            .Select(_ => Navigator)
                            .InvokeCommand(this, a => a.NavigateCommand)
                            .DisposeWith(d);
                        
                            
                        _talkiPlayerManager.Current?.Device?.WhenConnectionFailed()
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Do(_ =>
                            {
                                _connectionTimeoutDisposable?.Dispose();
                            })
                            .HideLoading()
                            .ShowErrorToast("Could not connect to the reader.")
                            .SubscribeSafe()
                            .DisposeWith(d);
                    })
                    .InvokeCommand(this, v => v.ConnectCommand)
                    .DisposeWith(d);
                
                d.Add(_compositeDisposable);
            });
        }

        void StartConnectionTimer()
        {
            _connectionTimeoutDisposable = Observable.Timer(TimeSpan.FromMinutes(1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .Do(_ =>
                {
                    foreach(var item in _deviceSourceList.Items)
                    {
                        item.Selected = false;
                    }

                })
                .ShowErrorToast("Connection attempt timed out. Please try again!")
                .SubscribeSafe();
        }
             
        void SetupCommands()
        {
            ConnectCommand = ReactiveCommand.CreateFromTask<IDevice, Unit>(async (device) =>
            {
                _userDialogs.ShowLoading("Connecting ...");
                StartConnectionTimer();
                var success = await _talkiPlayerManager.Current.Connect(_connectionConfig);                
                return Unit.Default;
            });

            //ConnectCommand = ReactiveCommand.CreateFromObservable<IDevice, Unit>( device =>
            //{
            //    return ObservableOperatorExtensions.StartShowLoading("Connecting ...")
            //        .SelectMany(_ => _talkiPlayerManager.Current.Connect(_connectionConfig))
            //        .Do(_ => StartConnectionTimer())
            //        .Select(_ => Unit.Default);
            //});

            ConnectCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .Do(e =>
                {
                    foreach (var i in this.Devices)
                    {
                        i.Selected = false;
                    }
                })
                .ShowErrorToast()
                .SubscribeAndLogException();

         
            SelectCommand = ReactiveCommand.CreateFromObservable<DeviceItemViewModel, Unit>(model => model.SelectCommand.Execute());
            SelectCommand.ThrownExceptions.SubscribeAndLogException();

            if (NavigateCommand == null)
            {
                NavigateCommand = ReactiveCommand.CreateFromTask<INavigationService, Unit>(async navigator =>
                {
                    SimpleNavigationService.PushChildModalPage(new DevicePageViewModel(null)).Forget();
                    return Unit.Default;
                });
            }

            NavigateCommand.ThrownExceptions
                .HideLoading()
                .SubscribeAndLogException();

            var canExecute = this.WhenAnyValue(m => m.IsRefreshing).Select(m => !m);
            RefreshCommand = ReactiveCommand.Create(() =>
            {
                IsRefreshing = true;
                StopScan();                
                StartScan();                
            }, canExecute);
            
            RefreshCommand.ThrownExceptions.SubscribeAndLogException();

            OpenAccountSettings = ReactiveCommand.CreateFromObservable(() => SimpleNavigationService.PushModalWithNavigationAsyncEx(new DeviceListPageViewModel(null)), outputScheduler: RxApp.MainThreadScheduler);
            
            OpenAccountSettings.ThrownExceptions.SubscribeAndLogException();


            BackCommand = ReactiveCommand.CreateFromObservable(() => SimpleNavigationService.PopModalAsync().ToObservable());
            BackCommand.ThrownExceptions.SubscribeAndLogException();
        }
       
        public void StartScan()
        {
            if (SimpleNavigationService.IsGoingBack)
            {
                return;
            }

            _deviceSourceList.Clear();            
            ShowEmptyState = true;

            OpenDeviceDetailsIfConnected();
            
            try
            {
                CrossBleAdapter.Current.StopScan();
            }
            catch (Exception) { }

            CrossBleAdapter.Current.WhenStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(ProcessAdapterStatus)
                .Where(m => m == AdapterStatus.PoweredOn)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(status =>
                {
                    //StopScan();
                    //CrossBleAdapter.Current.StopScan();
                    //_deviceSourceList.Clear();
                    //StartTimerUntilShowTalkiPlayerActivationInstructions();
                    _scanDisposable = CrossBleAdapter.Current.Scan()
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Subscribe(HandleDeviceFound, ex => Locator.Current.GetService<ILogger>()?.Error(ex));
                },
                ex => Locator.Current.GetService<ILogger>()?.Error(ex));          
         }

        IObservable<AdapterStatus> ProcessAdapterStatus(AdapterStatus status)
        {
            if (status == AdapterStatus.PoweredOff)
            {
                return Observable.Start(() =>
                {
                    if (CrossBleAdapter.Current.CanOpenSettings())
                    {
                        CrossBleAdapter.Current.OpenSettings();
                    }
                }).Select(_ => status);

            }
            else if (status == AdapterStatus.Unauthorized)
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

                }).Select(_ => status);
            }
            else
            {
                return Observable.Return(status);
            }
        }

        void HandleDeviceFound(IScanResult scanResult)
        {
            if (!string.IsNullOrWhiteSpace(scanResult.Device.Name)
                                && scanResult.Device.Name.Contains(_config.DeviceNamePrefix))
            {
                //if (_talkiplayerSearchTimeoutDisposable != null)
                //{
                //    if (Navigator.HasPopModalInStack)
                //    {
                //        Navigator.PopPopup().SubscribeSafe();
                //    }

                //    _talkiplayerSearchTimeoutDisposable?.Dispose();
                //    _talkiplayerSearchTimeoutDisposable = null;

                //}

                var device = _deviceSourceList.Items.FirstOrDefault(d => d.BleDevice.Name.Equals(scanResult.Device.Name));
                                
                if (device == null)
                {
                    IsRefreshing = false;
                    ShowEmptyState = false;                    
                    _deviceSourceList.Add(new DeviceItemViewModel(scanResult.Device, Navigator));
                }
            }
        }


        public void StopScan()
        {
            try
            {
                CrossBleAdapter.Current.StopScan();
                _scanDisposable?.Dispose();
                _deviceSourceList.Clear();
                _connectionTimeoutDisposable?.Dispose();
                //_talkiplayerSearchTimeoutDisposable?.Dispose();

            }
            catch (Exception ex)
            {
                ex.LogException("StopScan");
            }
        }

        //void StartTimerUntilShowTalkiPlayerActivationInstructions()
        //{
        //    _talkiplayerSearchTimeoutDisposable?.Dispose();
        //    _talkiplayerSearchTimeoutDisposable = null;
        //    _talkiplayerSearchTimeoutDisposable = Observable.Timer(Constants.TimeoutUntilShowTalkiPlayActiviationInstruciton)
        //        .ObserveOn(RxApp.MainThreadScheduler)
        //        .SelectMany(_ => Navigator.PushPopup(new ShakeTalkiPlayerPopUpPageViewModel()))
        //        .SubscribeSafe();
        //}

        private void OpenDeviceDetailsIfConnected()
        {
            if (CanOpenDeviceDetailsWhenAlreadyConnected)
            {
                if (SimpleNavigationService.IsInProgress)
                {
                    Observable.Timer(TimeSpan.FromMilliseconds(20), RxApp.MainThreadScheduler)
                        .Do((m) => OpenDeviceDetailsIfConnected())
                        .Subscribe();
                }
                else if (_talkiPlayerManager.Current?.IsConnected == true)
                {
                    SimpleNavigationService.PushChildModalPage(new DevicePageViewModel(null)).Forget();
                }
            }          
        } 
    }
}   