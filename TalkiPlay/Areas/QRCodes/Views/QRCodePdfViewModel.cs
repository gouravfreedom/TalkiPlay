﻿using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Windows.Input;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class QRCodePdfViewModel
    {
        Action<IAsset> _downloadCallback;
        public QRCodePdfViewModel(IAsset asset, Action<IAsset> downloadCallback)
        {
            _downloadCallback = downloadCallback;
            Asset = asset;
            Title = asset.Name;
            DownloadCommand = new Command(() => _downloadCallback?.Invoke(Asset));
        }

        public IAsset Asset { get; }

        [Reactive]
        public string Title { get; }

        public ICommand DownloadCommand { get; private set; }
    }
}
