﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using Plugin.DownloadManager.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public enum AudioUpdateStatus
    {
        None,
        Downloading,
        Downloaded,
        Uploading,
        Uploaded,
        Failed
    }
    
    public class AudioItemViewModel  : ReactiveObject
    {
        //readonly IAssetService _assetService;
        private readonly IAssetRepository _assetRepository;
        readonly ILogger _logger;        
        readonly ITalkiPlayerManager _talkiPlayerManager;

        public AudioItemViewModel(
            AssetDto asset,
            ReactiveCommand<AudioItemViewModel, Unit> audioPlayCommand,
            ITalkiPlayerManager talkiPlayerManager = null,
            ILogger logger = null
           // IAssetService assetService = null
          )
        {
           // _assetService = assetService ?? Locator.Current.GetService<IAssetService>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            Asset = asset;
            _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();

            this.WhenAnyValue(a => a.IsPlaying)
                .Select(a => a ? Images.StopIcon : Images.PlayIcon)
                .ToPropertyEx(this, a => a.Icon);


            SetupCommands(asset, audioPlayCommand);
        }

        void SetupCommands(IAsset asset, ReactiveCommand<AudioItemViewModel, Unit> audioPlayCommand)
        {
            AudioPlayCommand = ReactiveCommand.CreateFromObservable( () => audioPlayCommand.Execute(this));
            AudioPlayCommand.ThrownExceptions.SubscribeAndLogException();
            
            DownloadCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    var assetToDownload = await _assetRepository.GetAssetById(asset.Id);
                    AudioDownloadResult = AssetDownloadManager.BuildAssetDownloadResult(assetToDownload);
                    //AudioDownloadResult = await _assetService.GetAndDownloadAsset(this.Asset);
                    SetupDownloadMonitor();
                    DownloadStatus = AudioUpdateStatus.Downloading;                                       
                });
            
            DownloadCommand.ThrownExceptions.SubscribeAndLogException();

            UploadCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await _assetRepository.SaveAsset(Asset);

                if (!String.IsNullOrWhiteSpace(Asset.FilePath))
                {
                    var size = FileHelper.GetFileSize(Asset.FilePath);
                    var checksum = FileHelper.GenerateCheckSum(Asset.FilePath);
                    var data = new FileUploadData(Asset.Filename, size, checksum, Asset.FilePath, $"{Asset.Id}", UploadDataType.Audio);
                    _logger.Information($"Uploading: {Asset.Filename} from {Asset.FilePath}");
                    _talkiPlayerManager.Current.Upload(data);
                    DownloadStatus = AudioUpdateStatus.Uploading;
                }
            });
            UploadCommand.ThrownExceptions.SubscribeAndLogException();
            

            DeleteCommand = ReactiveCommand.Create(() =>
            {
                var data = new DataUploadData("AudioDelete", DataRequest.DeleteAudioFileRequest(new List<string>() { Asset.Filename.ToUpper()}), $"{Asset.Id}", UploadDataType.AudioDelete);
                _talkiPlayerManager.Current.Upload(data);
                DownloadStatus = AudioUpdateStatus.Uploading;
                IsExists = false;
            });
            
            DeleteCommand.ThrownExceptions.SubscribeAndLogException();
        }
        

        public AssetDto Asset { get; }

        [Reactive]
        public string Name { get; set; }

        [Reactive]
        public bool IsExists { get; set; }

        [Reactive]
        public bool IsDownloaded { get; set; }

        [Reactive]
        public AudioUpdateStatus DownloadStatus { get; set; }

        [Reactive]
        public bool IsPlaying { get; set; }

        //public ReactiveCommand<Unit, Unit> SelectCommand { get; }

        [Reactive]
        public double DownloadProgress { get; set; }


        public ReactiveCommand<Unit, Unit> AudioPlayCommand { get; private set;}
        public extern string Icon { [ObservableAsProperty] get; }

        [Reactive] 
        IDownloadFileResult AudioDownloadResult { get; set; }

        public ReactiveCommand<Unit, Unit> DownloadCommand { get; private set;}

        ReactiveCommand<Unit, Unit> UploadCommand { get; set;}

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; private set; }


        void SetupDownloadMonitor()
        {
            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => AudioDownloadResult.PropertyChanged += h,
                    h => AudioDownloadResult.PropertyChanged -= h)
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
                            var progress = (double) (file.TotalBytesWritten /
                                                    Asset.Filesize);

                            this.DownloadProgress = progress;
                        }
                    }

                    if (e.EventArgs.PropertyName.Equals(nameof(IDownloadFile.Status)))
                    {
                        switch (file.Status)
                        {
                            case DownloadFileStatus.FAILED:
                                DownloadStatus = AudioUpdateStatus.Failed;
                                break;
                            case DownloadFileStatus.CANCELED:
                                DownloadStatus = AudioUpdateStatus.None;
                                break;
                            case DownloadFileStatus.COMPLETED:
                                DownloadStatus = AudioUpdateStatus.Downloaded;
                                if (Asset is AssetDto asset)
                                {
                                    asset.FilePath = file.DestinationPathName;
                                }
                                UploadCommand.Execute().Subscribe();
                                break;
                        }
                    }
                });
        }
        
       
    }
}
