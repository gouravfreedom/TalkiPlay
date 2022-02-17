using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acr.Logging;
using Acr.UserDialogs;
using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public class AssetDownloadManager
    {
        readonly IAssetRepository _assetRepository;
        readonly IGameMediator _gameMediator;
        readonly IUserSettings _userSettings;
        readonly ILogger _logger;
        readonly Queue<IAsset> _queue;
        readonly IConfig _config;
        readonly IStorage _storage;        
        
        public AssetDownloadManager(
            IGameMediator gameMediator = null,
            IUserSettings userSettings = null,
            IAssetRepository assetRepository = null,
            ILogger logger = null,
            IConfig config = null,
            IStorage storage = null)
        {
            _queue = new Queue<IAsset>();
            
            _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            _assetRepository = assetRepository ?? Locator.Current.GetService<IAssetRepository>();
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            _config = config ?? Locator.Current.GetService<IConfig>();
            _storage = storage ?? Locator.Current.GetService<IStorage>();
        }

        IProgressDialog ProgressDialog { get; set; }
        

        [Reactive]
        int NumberOfItemsInQueue { get; set; }

        [Reactive]
        int NumberOfItemsProcessed { get; set; }

        [Reactive]
        int TotalNumberOfItems { get; set; }
        

        [Reactive]
        IDownloadFileResult AudioDownloadResult { get; set; }

        [Reactive]
        IAsset CurrentItem { get; set; }

        [Reactive]
        bool WaitingForItemResult { get; set; }

        
        public event EventHandler DownloadCompleted;

        public async Task<List<IAsset>> GetAssetsToDownload(IGame game)
        {         

            var itemsSettings = _userSettings.ItemsSettings;
            var excludedItemIds = itemsSettings.Where(s => !s.IsActive).Select(s => s.ItemId).ToList();

            var items = _gameMediator.CurrentPack.Items.Where(i => !excludedItemIds.Contains(i.Id));
            var packAssetIds = items.SelectMany(i => i.AudioAssetIds).ToList();

            var packAssets = _gameMediator.CurrentPack.AudioAssets.Where(a => packAssetIds.Contains(a.Id));

            var systemAssets = await _assetRepository.GetAssets(AssetType.Audio, Category.System);
          

            var assetsToDownload = new List<IAsset>();

            foreach (var asset in systemAssets)
            {
                var localFilePath = BuildAssetDownloadPath(asset, _storage);
                
                if (!File.Exists(localFilePath))
                {
                    assetsToDownload.Add(asset);
                }
            }
            
            foreach (var asset in packAssets)
            {
                var localFilePath = BuildAssetDownloadPath(asset, _storage);
                
                if (!File.Exists(localFilePath))
                {
                    assetsToDownload.Add(asset);
                }
            }                      

            if (game.Type == GameType.Hunt)
            {
                var audioAssetIds =
                    game.Instructions.SelectMany(i => i.Modes.Select(m => m.AudioAssetId));
                foreach (var audioAssetId in audioAssetIds)
                {
                    if (audioAssetId == null)
                    {
                        continue;
                    }
                    
                    var asset = new AssetDto()
                    {
                        Id = audioAssetId.Value,
                        Filename = $"{audioAssetId.Value}.mp3" 
                    };
                    var localFilePath = BuildAssetDownloadPath(asset, _storage);
                
                    if (!File.Exists(localFilePath))
                    {
                        assetsToDownload.Add(asset);
                    }
                }
            }
            
            return assetsToDownload;
        }

        public async Task<bool> PromptToDownload()
        {                        
            var confirmConfig = new ConfirmConfig()
            {
                Title = "Audio files are missing",
                Message = $"TalkiPlay does not have all the audio files necessary for the selected {_gameMediator.CurrentPack.Name} pack. " +
                $"Would you like to download these audio files now?",
                OkText = "Ok",
                CancelText = "Cancel"
            };

            var ok = await Dialogs.ConfirmAsync(confirmConfig);
            return ok;
        }

        public void StartDownload(List<IAsset> assets, bool showProgress = false)
        {
            foreach (var asset in assets)
            {                
                _queue.Enqueue(asset);                
            }

            TotalNumberOfItems = assets.Count;
            NumberOfItemsInQueue = _queue.Count;

            if (showProgress)
            {
                ProgressDialog = Dialogs.Progress(new ProgressDialogConfig()
                {
                    Title = $"Downloading {NumberOfItemsProcessed} / {TotalNumberOfItems} assets ...",
                    IsDeterministic = true,
                    MaskType = MaskType.Black,
                    CancelText = "Cancel",
                    AutoShow = true,
                    OnCancel = () =>
                    {
                        AudioDownloadResult?.Cancel();

                    }
                });
            }

            DownloadNext();        
        }


        void DownloadNext()
        {
            if (NumberOfItemsInQueue > 0)                        
            {
                var next = _queue.Dequeue();
                CurrentItem = next;
                WaitingForItemResult = true;
                NumberOfItemsInQueue = _queue.Count;

                UpdateProgressStatus();

                AudioDownloadResult = BuildAssetDownloadResult(next, _config, _storage);
                SetupDownloadMonitor();
                NumberOfItemsProcessed++;
            }
            else
            {
                EndDownload();

                DownloadCompleted?.Invoke(null, new EventArgs());                
            }

        }

        void EndDownload()
        {
            ProgressDialog?.Hide();
            ProgressDialog?.Dispose();
            ProgressDialog = null;
            CurrentItem = null;
            NumberOfItemsProcessed = 0;
            NumberOfItemsInQueue = 0;
            AudioDownloadResult = null;
        }

        void UpdateProgressStatus()
        {

            var percent = (double)NumberOfItemsProcessed / (double)TotalNumberOfItems;
            NumberOfItemsInQueue = _queue.Count;

            if (ProgressDialog != null)
            {
                ProgressDialog.PercentComplete = (int)(percent * 100);

                ProgressDialog.Title = $"Downloading {TotalNumberOfItems - NumberOfItemsInQueue}  / {TotalNumberOfItems} assets ...";
            }
        }

        void SetupDownloadMonitor()
        {
            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => AudioDownloadResult.PropertyChanged += h,
                    h => AudioDownloadResult.PropertyChanged -= h)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(e =>
                {
                    if (!(e.Sender is IDownloadFile file))
                    {
                        return;
                    }
                    
                    if (e.EventArgs.PropertyName.Equals(nameof(IDownloadFile.Status)))
                    {
                        switch (file.Status)
                        {
                            case DownloadFileStatus.FAILED:
                                {
                                    EndDownload();                                    
                                    DownloadCompleted?.Invoke(null, new EventArgs());                                    
                                    break;
                                }
                            case DownloadFileStatus.CANCELED:
                                {
                                    EndDownload();                                    
                                    DownloadCompleted?.Invoke(null, new EventArgs());                                    

                                    break;
                                }
                            case DownloadFileStatus.COMPLETED:
                                {
                                    
                                    _logger?.Information(
                                        $"Total Downloaded: {file.TotalBytesWritten}, Status: {file.Status}, Path: {file.DestinationPathName}, Url: {file.Url}");
                                   
                                    DownloadNext();
                                    break;
                                }
                        }
                    }
                });
        }
     
        public static string BuildAssetDownloadPath(IAsset assetToDownload, IStorage storage)
        {
            var ext = Path.GetExtension(assetToDownload.Filename);
            var fileName = $"{assetToDownload.Id}{ext}";
            return Path.Combine(storage.GetRootPath(), fileName);
        }
        
        public static IDownloadFileResult BuildAssetDownloadResult(IAsset assetToDownload, IConfig config = null, IStorage storage = null)
        {
            config = config ?? Locator.Current.GetService<IConfig>();
            storage = storage ?? Locator.Current.GetService<IStorage>();
            
            var url = $"{config.GetAssetDownloadUrl(assetToDownload.Id)}";
            string fileName = "";
            //TODO: make sure the type field is populated correctly
            //if (assetToDownload.Type == AssetType.Audio)
            if (assetToDownload.Filename.ToLower().EndsWith("mp3"))
            {
                var ext = Path.GetExtension(assetToDownload.Filename);
                fileName = $"{assetToDownload.Id}{ext}";
            }
            else
            {
                fileName = $"{assetToDownload.Filename}";    
            }

            var assetFilePath = Path.Combine(storage.GetRootPath(), fileName);

             Debug.WriteLine($"BuildAssetDownloadResult for asset {assetToDownload.Name}: {assetFilePath}");
            //var audioFilePath = BuildAssetDownloadPath(assetToDownload, storage);
            
            if (!string.IsNullOrWhiteSpace(assetToDownload.FilePath))
            {                
                return new CompletedDownloadResult()
                {
                    Status = DownloadFileStatus.COMPLETED,
                    DestinationPathName = assetToDownload.FilePath,
                    TotalBytesExpected = assetToDownload.Filesize,
                    TotalBytesWritten = assetToDownload.Filesize
                } as IDownloadFileResult;
            }

            if (File.Exists(assetFilePath))
            {
                try
                {
                    File.Delete(assetFilePath);
                }
                catch (Exception)
                {

                }
            }

            var downloadManager = CrossDownloadManager.Current;
            //(CrossDownloadManager.Current as DownloadManagerImplementation).IsVisibleInDownloadsUi = false;
            
            var downloadFile = downloadManager.CreateDownloadFile(url,
                new Dictionary<string, string>()
                {
                    {"apiKey", config.ApiKey},
                    {"fileName", fileName}
                });

            downloadManager.Start(downloadFile, true);
            return new DownloadResult(downloadFile) as IDownloadFileResult;

        }

        public static async Task DownloadAnimations(List<AssetDto> animationAssets)
        {
            var config = Locator.Current.GetService<IConfig>();
            var storage = Locator.Current.GetService<IStorage>();
            
            var client = new WebClient();

            foreach (var asset in animationAssets)
            {
                try
                {
                    var url = 
                        $"{config.GetAssetDownloadUrl(asset.Id)}?ApiKey={config.ApiKey}";
                    var data = await client.DownloadDataTaskAsync(new Uri(url));

                    if (data != null && data.Length > 0)
                    {
                        var fileName = $"animation_{asset.Id}.json";
                        var localFilePath = Path.Combine(storage.GetRootPath(), fileName);

                        if (File.Exists(localFilePath))
                        {
                            File.Delete(localFilePath);
                        }
                        
                        await File.WriteAllBytesAsync(localFilePath, data);
                        Debug.WriteLine("Downloaded animation: " + localFilePath);
                        asset.FilePath = localFilePath;
                    }
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e.Message,e);
                    Console.WriteLine(e);
                }
            }
        }
        
    }
}
