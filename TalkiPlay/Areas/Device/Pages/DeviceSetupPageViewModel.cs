using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using Plugin.BluetoothLE;
using Plugin.DownloadManager.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;
using Unit = System.Reactive.Unit;

namespace TalkiPlay.Shared
{
    public enum DeviceSetupSource
    {
        Onboard,
        Connect,
        Settings
    }

    public enum DeviceSetupStep
    {
        None,
        PairMe,
        TapMe,
        HomeTag,
        TagMe,
        Success,
        Failed,
    }

    public class DeviceSetupPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        private readonly IConfig _config;
        private readonly ILogger _logger;
        private readonly ITalkiPlayerManager _talkiPlayerManager;
        private readonly IAssetRepository _repoAsset;
        private readonly IUserDialogs _userDialogs;
        private CompositeDisposable _connectDisposable = new CompositeDisposable();
        private IDisposable _connectionTimeoutDisposable;
        private IDisposable _scanDisposable;
        private CompositeDisposable _tagReadDisposable;
        private DeviceSetupSource _sourceType = DeviceSetupSource.Onboard;

        public override string Title => "Device Setup";
        public ViewModelActivator Activator { get; }

        public DeviceSetupPageViewModel(
            DeviceSetupSource source,
            INavigationService navigator = null,
            IUserDialogs userDialogs = null,
            ITalkiPlayerManager talkiPlayerManager = null,
            IAssetRepository repoAsset = null,
            ILogger logger = null,
            IConfig config = null
            )
        {
            _sourceType = source;
            _config = config ?? Locator.Current.GetService<IConfig>();
            Activator = new ViewModelActivator();
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _repoAsset = repoAsset ?? Locator.Current.GetService<IAssetRepository>();
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            Activator = new ViewModelActivator();
            Navigator = navigator ?? Locator.Current.GetService<INavigationService>(TabItemType.Settings.ToString());            
            SetupRx();
            SetupCommands();


            if (_talkiPlayerManager.Current?.IsConnected == true)
            {
                _pickedDevice = _talkiPlayerManager.Current.Device;
                GoToHomeTagScanMode();
            }
            else
            {
                CurrentStep = _sourceType == DeviceSetupSource.Onboard ? DeviceSetupStep.PairMe : DeviceSetupStep.TapMe;
            }
        }

        public ReactiveCommand<Unit, Unit> SwitchPlugInCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PairMeCommand { get; private set; }
        public ReactiveCommand<IDevice, Unit> ConnectCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ConnectedCommand { get; private set; }
        ReactiveCommand<IDataUploadResult, Unit> DataHandleCommmand { get; set; }
        [Reactive] public String StepTitle { get; private set; }
        [Reactive] public bool ShowAnim { get; private set; } = true;
        [Reactive] public String StepAnim { get; private set; }
        [Reactive] public String StepImage { get; private set; }
        [Reactive] public double AnimScale { get; private set; }
        [Reactive] public bool IsPluggedIn { get; private set; } = false;


        private AudioPackListPageViewModel _packsVm;
        private IDevice _pickedDevice;
        private System.Collections.Generic.IList<IItem> _homeItems;
        private System.Collections.Generic.IList<ITag> _homeTags;

        private DeviceSetupStep _currentStep = DeviceSetupStep.None;
        public DeviceSetupStep CurrentStep
        {
            get => _currentStep;
            set
            {
                if (value != _currentStep)
                {
                    switch(value)
                    {
                        case DeviceSetupStep.PairMe:
                            StepTitle = "Hi, I'm your\nTalkiPlayer";
                            StepAnim = Images.ObTalkiGuyWelcome;
                            AnimScale = 1.3;
                            break;
                        case DeviceSetupStep.TapMe:
                            StepTitle = IsPluggedIn ? "Pluged In" : "Shake me!\nTap me!";
                            StepAnim = IsPluggedIn ? Images.ObPluggedIn : Images.ObShakeMeTapMe;
                            AnimScale = 1.2;
                            StartScan();
                            break;
                        case DeviceSetupStep.HomeTag:
                            StepTitle = "Please wait!";
                            StepAnim = Images.TpSearch;
                            AnimScale = 1.4;
                            CheckAndAddHomeTags().Forget();
                            break;
                        case DeviceSetupStep.TagMe:
                            StepTitle = _sourceType == DeviceSetupSource.Onboard ?
                                "Tap the home\nagain to\ncomplete setup" : "Tap a tag\nto register as\nhome tag";
                            StepAnim = Images.ObTagMe;
                            AnimScale = 1.2;
                            break;
                        case DeviceSetupStep.Success:
                            _tagReadDisposable?.Dispose();
                            StepTitle = "Success!";
                            StepAnim = Images.ObSuccess;
                            AnimScale = 1.6;
                            ExitSetup();
                            break;
                        case DeviceSetupStep.Failed:
                            _tagReadDisposable?.Dispose();
                            StepTitle = "Failed!!";
                            StepAnim = null;
                            StepImage = TalkiPlay.Shared.Images.TpSad;
                            ShowAnim = false;
                            ExitSetup();
                            break;
                    }

                    this.RaiseAndSetIfChanged(ref _currentStep, value);
                    this.RaisePropertyChanged(nameof(StepTitle));
                    this.RaisePropertyChanged(nameof(StepAnim));
                    this.RaisePropertyChanged(nameof(AnimScale));
                }
            }
        }        

        private void SetupRx()
        {
        }
        

        private void SetupCommands()
        {
            SwitchPlugInCommand = ReactiveCommand.CreateFromTask(() =>
            {
                IsPluggedIn = !IsPluggedIn;
                StepTitle = IsPluggedIn ? "Charge Me!" : "Shake me!\nTap me!";
                StepAnim = IsPluggedIn ? Images.ObPluggedIn : Images.ObShakeMeTapMe;
                return Task.CompletedTask;
            });

            PairMeCommand = ReactiveCommand.CreateFromTask(() =>
            {
                CurrentStep = DeviceSetupStep.TapMe;                
                return Task.CompletedTask;
            });

            ConnectCommand = ReactiveCommand.CreateFromTask<IDevice, Unit>(async (device) =>
            {
                
                return Unit.Default;
            });

            ConnectCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowErrorToast()
                .SubscribeAndLogException();

            ConnectedCommand = ReactiveCommand.CreateFromTask(() =>
            {
                GoToHomeTagScanMode();
                return Task.CompletedTask;
            });

            DataHandleCommmand = ReactiveCommand.CreateFromTask<IDataUploadResult, Unit>(async data =>
            {
                if (!data.IsSuccess)
                {
                    var error = data.GetData<ErrorDataResult>();
                    throw error.Exception;
                }

                if (data.Type != UploadDataType.TagData || data.Data == null ||
                    !(data.Data is TagData tagData) || String.IsNullOrWhiteSpace(data.Tag))
                {
                    throw new ApplicationException("Data is invalid");
                }

                var itemId = int.Parse(data.Tag);
                if (_homeTags?.FirstOrDefault(i => i.SerialNumber == tagData.SerialNumber) == null)
                {
                    Dialogs.ShowLoading("Creating home tag");
                    await _repoAsset.AddTag(itemId, tagData.SerialNumber);
                    Dialogs.HideLoading();
                }
                else
                {
                    Dialogs.Toast("Home tag already exist");
                }
                CurrentStep = DeviceSetupStep.Success;
                return Unit.Default;
            });

            DataHandleCommmand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .Do(m => CurrentStep = DeviceSetupStep.Failed)
                .SubscribeAndLogException();

        }


        public void StartScan()
        {

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

        private void HandleDeviceFound(IScanResult scanResult)
        {
            if (_pickedDevice == null && !string.IsNullOrWhiteSpace(scanResult.Device.Name)
                    && scanResult.Device.Name.Contains(_config.DeviceNamePrefix))
            {
                _pickedDevice = scanResult.Device;
                ConnectTo(_pickedDevice).Forget();
            }
        }

        private async Task ConnectTo(IDevice device)
        {
            _talkiPlayerManager.SetTalkiPlayer(device);
            SetupConnectionStatusChange();
            await _talkiPlayerManager.Current.Connect(new ConnectionConfig() { AutoConnect = true });
        }

        private void SetupConnectionStatusChange()
        {
            _connectDisposable = new CompositeDisposable();

            _talkiPlayerManager.Current?.WhenAnyValue(a => a.IsConnected)
                .Where(a => a)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(_ =>
                {
                    StopScan();
                    GoToHomeTagScanMode();
                })
                .HideLoading()
                .ShowSuccessToast($"Connected to {Constants.DeviceName}")
                .SubscribeSafe()
                //.InvokeCommand(this, a => a.ConnectedCommand)
                .DisposeWith(_connectDisposable);


            _talkiPlayerManager.Current?.Device?.WhenConnectionFailed()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(_ =>
                {
                    _connectionTimeoutDisposable?.Dispose();
                })
                .HideLoading()
                .ShowErrorToast("Could not connect to the reader.")
                .SubscribeSafe()
                .DisposeWith(_connectDisposable);

            _userDialogs.ShowLoading("Connecting ...");
            StartConnectionTimer();
        }

        void StartConnectionTimer()
        {
            _connectionTimeoutDisposable = Observable.Timer(TimeSpan.FromMinutes(1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .Do(_ =>
                {
                    _pickedDevice = null;
                })
                .ShowErrorToast("Connection attempt timed out. Trying again...")
                .SubscribeSafe();
        }

        public void StopScan()
        {
            try
            {
                CrossBleAdapter.Current.StopScan();
                _scanDisposable?.Dispose();
                _pickedDevice = null;
                _connectionTimeoutDisposable?.Dispose();
                _connectDisposable?.Dispose();

            }
            catch (Exception ex)
            {
                ex.LogException("StopScan");
            }
        }

        private void GoToHomeTagScanMode()
        {
            CurrentStep = _sourceType == DeviceSetupSource.Connect ? DeviceSetupStep.Success : DeviceSetupStep.HomeTag;
        }

        private async Task CheckAndAddHomeTags()
        {
            _homeItems = await _repoAsset.GetItems(ItemType.Home);
            var allTags = await _repoAsset.GetTags();
            _homeTags = allTags.Where(t => t.ItemIds.Any(id => _homeItems.Any(item => item.Id == id))).ToList();

            if (_sourceType != DeviceSetupSource.Settings && _homeTags.Count >  0)
            {
                CurrentStep = DeviceSetupStep.Success;
            }
            else
            {
                var packDisposable = new CompositeDisposable();
                _packsVm = new AudioPackListPageViewModel(null, AudioPackNavigationSource.DeviceSetupPage, null, "Home Tag", (pack) =>
                {
                    packDisposable.Dispose();
                    CurrentStep = DeviceSetupStep.TagMe;

                    _tagReadDisposable = new CompositeDisposable();
                    _talkiPlayerManager?.Current?.OnDataResult()
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .InvokeCommand(this, m => m.DataHandleCommmand)
                            .DisposeWith(_tagReadDisposable);

                    var packItems = pack.Items.OrderBy(a => a.Type.GetData<int>("SortOrder")).ToList();
                    var homeItem = packItems.FirstOrDefault(i => i.Type == ItemType.Home);
                    _talkiPlayerManager.Current?.UploadTagData(homeItem);
                });

                _packsVm.BindRx(packDisposable);
                _packsVm.RefreshPacks();
            }
        }

        private void ExitSetup()
        {
            bool isOnboarded = Locator.Current.GetService<IUserSettings>().IsOnboarded;
            Locator.Current.GetService<IUserSettings>().IsDeviceOnboarded = true;
            _ = Observable.Timer(TimeSpan.FromSeconds(isOnboarded ? 2 : 5), RxApp.MainThreadScheduler)
                .Do((m) =>
                {
                    if (isOnboarded)
                    {
                        SimpleNavigationService.PopModalAsync().Forget();
                    }
                    else
                    {
                        var state = new GuideState() {ShouldRlaceMainPage = true, EnableBackAtStart = false };
                        SimpleNavigationService.PushAsync(new GuideImagePageViewModel(state.StartStep, state)).Forget();
                    }
                })
                .Subscribe();
        }
    }
}
