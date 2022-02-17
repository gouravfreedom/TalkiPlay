﻿using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using Plugin.DownloadManager.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class QRCodePdfListPageViewModel : SimpleBasePageModel
    {
        readonly IAssetRepository _assetRepository;
        IDownloadFileResult _downloadResult;
        IAsset _currentAsset;
        private bool _hasLoadedFirstTime;

        public QRCodePdfListPageViewModel()
        {
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            Items = new ObservableDynamicDataRangeCollection<QRCodePdfViewModel>();

            SetupCommands();
        }

        public ObservableDynamicDataRangeCollection<QRCodePdfViewModel> Items { get; }
        public string Title => "QR Codes";

        public ICommand BackCommand { get; private set; }

        void SetupCommands()
        {
            BackCommand = new Command(() =>
            {
                SimpleNavigationService.PopModalAsync().Forget();
            });
        }

        public async Task LoadData()
        {
            if (!_hasLoadedFirstTime)
            {
                Dialogs.ShowLoading();
            }

            var assets = await _assetRepository.GetAllPdfAssets();
            
            Dialogs.HideLoading();

            Items.Clear();
            using (Items.SuspendNotifications())
            {
                Items.AddRange(assets.Select(a => new QRCodePdfViewModel(a, DownloadTapped)));
            }

            _hasLoadedFirstTime = true;
        }
        
        void DownloadTapped(IAsset asset)
        {
            _currentAsset = asset;
            Dialogs.ShowLoading("Downloading ...");
            _downloadResult = AssetDownloadManager.BuildAssetDownloadResult(asset);
            _downloadResult.PropertyChanged += DownloadResultReceived;
        }

        void DownloadResultReceived(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is IDownloadFile file))
            {
                return;
            }

            if (e.PropertyName == nameof(IDownloadFile.Status))
            {                
                if (file.Status == DownloadFileStatus.FAILED || file.Status == DownloadFileStatus.COMPLETED)
                {
                    _downloadResult.PropertyChanged -= DownloadResultReceived;
                    Dialogs.HideLoading();
                }

                if (file.Status == DownloadFileStatus.COMPLETED)
                {
                    var filePath = file.DestinationPathName;

                    var newFilePath = filePath.Replace(_currentAsset.Filename, $"{_currentAsset.Name}.pdf");
                    if (File.Exists(newFilePath))
                    {
                        File.Delete(newFilePath);
                    }
                    
                    File.Move(filePath, newFilePath);
                    

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Launcher.OpenAsync(new OpenFileRequest
                        {
                            File = new ReadOnlyFile(newFilePath, "application/pdf"),
                            Title = _currentAsset.Name
                        }).Forget();
                    });
                                        
                }                                
            }
        }
    }
}
