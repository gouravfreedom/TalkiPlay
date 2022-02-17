using System;
using System.Collections.Generic;
using System.ComponentModel;
using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;

namespace TalkiPlay.Shared
{
    public interface IFileData
    {
         string Version { get;}

         string Checksum { get; }

         string FileName { get; }

         long FileSize { get; }
    }

    public interface IDownloadFileResult : IDownloadFile
    {
        void OnPropertyChanged();
        void Cancel();
    }
    
    public class CompletedDownloadResult : IDownloadFileResult
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Url { get; set; }
        public string DestinationPathName { get;  set;}
        public IDictionary<string, string> Headers { get;  set;}
        public DownloadFileStatus Status { get; set; }
        public string StatusDetails { get; set;}
        public float TotalBytesExpected { get;  set;}
        public float TotalBytesWritten { get;  set;}
        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        }

        public void Cancel()
        {
            
        }
    }

    public class DownloadResult : IDownloadFileResult
    {
        private readonly IDownloadFile _file;

        public DownloadResult(IDownloadFile file)
        {
            _file = file;
            file.PropertyChanged += FileOnPropertyChanged;
        }

        private void FileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is IDownloadFile file)) return;

            Url = file.Url;
            Headers = file.Headers;
            Status = file.Status;
            StatusDetails = file.StatusDetails;
            TotalBytesExpected = file.TotalBytesExpected;
            TotalBytesWritten = file.TotalBytesWritten;

            if (!String.IsNullOrWhiteSpace(file.DestinationPathName))
            {
                DestinationPathName = CrossDownloadManager.Current.PathNameForDownloadedFile(file);
            }

            PropertyChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public string Url { get; private set; }
        public string DestinationPathName { get; private set;}
        public IDictionary<string, string> Headers { get; private set;}
        public DownloadFileStatus Status { get; private set;}
        public string StatusDetails { get; private set;}
        public float TotalBytesExpected { get; private set;}
        public float TotalBytesWritten { get; private set;}
        public void OnPropertyChanged()
        {
            
        }

        public void Cancel()
        {
            if (_file != null && _file.Status != DownloadFileStatus.COMPLETED)
            {
                CrossDownloadManager.Current.Abort(_file);
            }
        }
    }
    
}