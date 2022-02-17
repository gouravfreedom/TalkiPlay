using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Acr.UserDialogs;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.Logging;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using Humanizer;
using Newtonsoft.Json;
using Plugin.BluetoothLE;
using Plugin.DownloadManager.Abstractions;
using Plugin.FilePicker;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Forms;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public class TestBleRequestPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        private readonly IStorage _fileStorage;
        private readonly ILogger _logger;
        private readonly ITalkiPlayerManager _talkiPlayerManager;
        private readonly IUserDialogs _userDialogs;
        private readonly IFirmwareService _firmwareService;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly SourceList<ItemSelectionViewModel> _dataTypes = new SourceList<ItemSelectionViewModel>(); 
        private readonly ObservableDynamicDataRangeCollection<ItemSelectionViewModel> _dataTypesList = new ObservableDynamicDataRangeCollection<ItemSelectionViewModel>();
        public override string Title => "TalkiPlayer Tests";
        public ViewModelActivator Activator { get; }

        public TestBleRequestPageViewModel(
            INavigationService navigator,
            IUserDialogs userDialogs = null,
            IFirmwareService firmwareService = null,
            ITalkiPlayerManager talkiPlayerManager = null,
            ILogger logger = null,
            IStorage fileStorage = null
            )
        {
            _fileStorage = fileStorage ?? Locator.Current.GetService<IStorage>();
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

        private void SetupCommands()
        {
            BackCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SimpleNavigationService.PopModalAsync();
            });
            BackCommand.ThrownExceptions.ObserveOn(RxApp.MainThreadScheduler).SubscribeAndLogException();

            SelectJsonFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var filter = Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android ? new[] {"application/json"} : null;
                var fileData = await CrossFilePicker.Current.PickFile(filter);
                if (fileData == null)
                    return; // user canceled file picking

                if (fileData.DataArray != null && fileData.DataArray.Length > 0)
                {
                    var contents = Encoding.UTF8.GetString(fileData.DataArray);
                    JsonFile = new DataUploadData(fileData.FileName, contents, "", UploadType);
                }

            });

            SelectJsonFileCommand
                .ThrownExceptions
                .ShowExceptionDialog()
                .SubscribeAndLogException();
            
            SelectFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var fileData = await CrossFilePicker.Current.PickFile();

                if (fileData == null)
                    return; // user canceled file picking

                var filePath = Path.Combine(_fileStorage.GetRootPath(), fileData.FileName);
                await _fileStorage.WriteAsync(fileData.GetStream(), filePath);
                JsonFileName = FileHelper.GetFileName(filePath);
                var size = FileHelper.GetFileSize(filePath);
                var checksum = FileHelper.GenerateCheckSum(filePath);
                DataFile = new FileUploadData(JsonFileName, size, checksum, filePath, "", UploadType);

            });

            SelectFileCommand.ThrownExceptions
                .ShowExceptionDialog()
                .SubscribeAndLogException();


            SelectCommand = ReactiveCommand.CreateFromObservable<ItemSelectionViewModel, Unit>( vm =>
            {
                foreach(var v in _dataTypes.Items)
                {
                    v.Selected = false;
                }

                vm.Selected = true;
                UploadType = (UploadDataType) vm.Source;
                   
                return Observable.Return(Unit.Default);
            });
            
            SelectCommand.ThrownExceptions.SubscribeAndLogException();

            var canExecute =(this).WhenAnyValue(a => a.JsonFile, a => a.DataFile, a => a.UploadType, a => a.IsReady, (Func<IDataUploadData, IFileUploadData, UploadDataType, bool, bool>)((json, file, type, isReady) =>
            {
                   if (!isReady) return false;
                    
                    if (type == UploadDataType.Unknown) return false;

                if (type == UploadDataType.Audio || type == UploadDataType.Firmware)
                    return file != null;

                if (type == UploadDataType.GameData || type == UploadDataType.Volume
                    || type == UploadDataType.AudioDelete)
                    return json != null;

                return isReady;

                }));

            UploadCommand = ReactiveCommand.CreateFromObservable( (Func<IObservable<Unit>>)(() =>
            {
                return ObservableOperatorExtensions.StartShowLoading("Requesting ...")
                    .Do((Action<Unit>)(_ =>
                    {
                        _talkiPlayerManager.Current?.CancelUpload();

                        Id = Guid.NewGuid().ToString();
                        switch (UploadType)
                        {
                            case UploadDataType.Firmware:
                            case UploadDataType.Audio:
                                _talkiPlayerManager.Current?.Upload(new FileUploadData(DataFile.Name, DataFile.Size,
                                    DataFile.Checksum,
                                    DataFile.Path, Id, UploadType), TimeSpan.FromMinutes(30));
                                break;
                            default:
                                _talkiPlayerManager.Current?.Upload(new DataUploadData(UploadType.Humanize(), JsonFile?.DataJson,
                                    Id, UploadType));
                                break;
                        }
                    }))
                    .Select(_ => Unit.Default);
            }), canExecute);
            
            UploadCommand.ThrownExceptions
                .ShowExceptionDialog()
                .SubscribeAndLogException();


            ConnectCommand = ReactiveCommand.Create(() =>
            {
                if (_talkiPlayerManager.Current == null) return;
                
                if (IsConnected)
                {
                    _talkiPlayerManager.Current.Disconnect().Subscribe();
                }
                else
                {
                    _talkiPlayerManager.Current.Connect().Subscribe();
                }
            });

            ConnectCommand.ThrownExceptions.SubscribeAndLogException();
            
        }

        private void SetupUploadTypes()
        {
            _dataTypes.Clear();
            _dataTypes.Edit(items =>
            {
                EnumExtensions.GetValues<UploadDataType>().ToList().ForEach(type =>
                {
                    if (type != UploadDataType.Unknown && type != UploadDataType.MetaData)
                    {
                        items.Add(new ItemSelectionViewModel()
                        {
                            Label = type.Humanize(),
                            Source = type
                        });
                    }
                });
                
                //items.Add(new EmptyItemSelectionViewModel());
            });
           
        }


        private void SetupRx()
        {
            
            this.WhenActivated(d =>
            {
                SetupUploadTypes();
                
                _dataTypes.Connect()
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Bind(_dataTypesList)
                   .Subscribe()
                   .DisposeWith(d);

          
                _talkiPlayerManager.Current?.OnDataResult()
                     .HideLoading()
                     .SelectMany(m =>
                     {
                         if (m.IsSuccess)
                         {
                             var model = m.Data?.ToJson();
                             return Observable.Start(() => _logger.Information($"Success - {model}"))
                                .ObserveOn(RxApp.MainThreadScheduler)
                                .Do(unit =>
                                {

                                    _userDialogs.Alert(new AlertConfig()
                                    {
                                        Title = "Successfully completed",
                                        Message = $"{model}",
                                        OkText = "OK"
                                    });

                                });
                         }
                        
                         var errorData = m.Data as ErrorDataResult;
                         return Observable.Start(() => _logger.Information("Failed"))
                            .Do(unit =>
                             {

                                 _userDialogs.Alert(new AlertConfig()
                                 {
                                     Title = "Failed",
                                     Message = $"{errorData?.Exception?.Message ?? "Request failed"}",
                                     OkText = "OK"
                                 });

                             });
                     })
                     .SubscribeSafe()
                     .DisposeWith(d);

                _talkiPlayerManager.Current?.Device?.WhenConnectionFailed()
                    .ShowErrorToast($"Failed to connect to the {Constants.DeviceName}. try again!")
                    .SubscribeSafe()
                    .DisposeWith(d);

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

                _talkiPlayerManager.Current?.IsReady
                    .Where(a => a)
                    .ToPropertyEx(this, v => v.IsReady)
                    .DisposeWith(d);

                //_talkiPlayerManager.Current?.WhenDisconnectedUponInactivity()
                //    .SubscribeSafe()
                //    .DisposeWith(d);
            });
        }

        public ReactiveCommand<Unit, Unit> UploadCommand { get; private set; }
        public ReactiveCommand<ItemSelectionViewModel, Unit> SelectCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SelectJsonFileCommand { get; private set; }
        
        public ReactiveCommand<Unit, Unit> SelectFileCommand { get; private set; }
        public ObservableCollection<ItemSelectionViewModel> Data => _dataTypesList;
        public extern bool IsConnected { [ObservableAsProperty] get; }
        public extern ConnectionStatus ConnectionStatus { [ObservableAsProperty] get; }
        public extern bool IsReady { [ObservableAsProperty] get; }

        [Reactive] public IDataUploadData JsonFile { get; set; }
        [Reactive] public IFileUploadData DataFile { get; set; }

        [Reactive] public string FileName { get; set; }
        [Reactive] public string JsonFileName { get; set; }
        [Reactive] public UploadDataType UploadType { get; set; } = UploadDataType.Unknown;
        
        public ReactiveCommand<Unit, Unit> ConnectCommand { get; private set; }
        
     
        public string Id { get; set; }
    }

}