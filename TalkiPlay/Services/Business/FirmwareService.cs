using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using ChilliSource.Mobile.Api;
using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public interface IFirmwareService
    {
        IObservable<IFileData> GetLatestFirmware();
        IObservable<IDownloadFileResult> DownloadLatestFirmware(IFileData fileData);

        bool IsCheckSumMatch(string path, string checksum, long size);
    }

    public class FirmwareService : IFirmwareService
    {
        private readonly ILogger _logger;
        private readonly IStorage _storage;
        //private readonly IDownloadManager _downloadManager;
        private readonly IConfig _config;
        private readonly IApi<ITalkiPlayApi> _api;


        public FirmwareService(
            //IDownloadManager downloadManager = null,
            IConfig config = null,
            IApi<ITalkiPlayApi> api = null,
            IStorage storage = null,
            ILogger logger = null
            )
        {
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            _storage = storage ?? Locator.Current.GetService<IStorage>();
            //_downloadManager = downloadManager ?? Locator.Current.GetService<IDownloadManager>();;
            _config = config ?? Locator.Current.GetService<IConfig>();
            _api = api ?? Locator.Current.GetService<IApi<ITalkiPlayApi>>();

        }
        
        public IObservable<IFileData> GetLatestFirmware()
        {
            return  _api.Client.GetLatestFirmware()
                .ToResult()
                .Do(m =>
                {
                    if (!m.IsSuccessful )
                    {
                        throw m.Exception;
                    }
                })
                .Select(r => r.Result);
        }

        public IObservable<IDownloadFileResult> DownloadLatestFirmware(IFileData fileData)
        {
            var fileName = $"firmware_{fileData.Version}.bin";
            var firmwarePath = Path.Combine(_storage.GetRootPath(), fileName);
            
            if (IsCheckSumMatch(firmwarePath, fileData.Checksum, fileData.FileSize))
            {
                return Observable.Return(new CompletedDownloadResult()
                {
                    Status = DownloadFileStatus.COMPLETED,
                    DestinationPathName = firmwarePath,
                    TotalBytesExpected = fileData.FileSize,
                    TotalBytesWritten = fileData.FileSize
                });
            }

            if (File.Exists(firmwarePath))
            {
                try
                {
                    File.Delete(firmwarePath);
                }
                catch (Exception)
                {
                    
                }
            }

            var downloadManager = CrossDownloadManager.Current;
            var downloadFile = downloadManager.CreateDownloadFile(Config.FirmwareDownloadUrl,
                new Dictionary<string, string>()
                {
                    {"apiKey", _config.ApiKey},
                    {"fileName", fileName}
                });
  
            downloadManager.Start(downloadFile, true);

            return Observable.Return(new DownloadResult(downloadFile));
        }
        
        public bool IsCheckSumMatch(string path, string checksum, long size)
        {
            return FileHelper.IsCheckSumMatch(path, checksum, size);
        }

    }
    
}