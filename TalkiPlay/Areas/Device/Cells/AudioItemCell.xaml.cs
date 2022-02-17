using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;
using ChilliSource.Mobile.UI.ReactiveUI;

namespace TalkiPlay
{
    public partial class AudioItemCell :  ReactiveBaseViewCell<AudioItemViewModel>
    {
        public AudioItemCell()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
                {
                    this.BindCommand(ViewModel, v => v.AudioPlayCommand, view => view.AudioPlayButton).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.DownloadCommand, view => view.DownloadButton).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.DeleteCommand, view => view.RemoveButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.DownloadProgress, view => view.ProgressView.AnimatedProgress)
                        .DisposeWith(d);
              
                    
                    this.WhenAnyValue(a => a.ViewModel.DownloadStatus)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .SubscribeSafe( status =>
                        {
                             switch (status)
                            {
                                case AudioUpdateStatus.Downloading:
                                    this.RemoveButton.IsVisible = false;
                                    this.DownloadButton.IsVisible = false;
                                    this.ProgressViewLayout.IsVisible = true;
                                    this.RetryButton.IsVisible = false;
                                    this.LoadingView.IsVisible = false;
                                    break;
                                case AudioUpdateStatus.Downloaded:
                                case AudioUpdateStatus.Uploading:
                                    this.RemoveButton.IsVisible = false;
                                    this.DownloadButton.IsVisible = false;
                                    this.ProgressViewLayout.IsVisible = false;
                                    this.LoadingView.IsVisible = true;
                                    this.RetryButton.IsVisible = false;
                                    break;
                                case AudioUpdateStatus.Uploaded:
                                    this.RemoveButton.IsVisible = true;
                                    this.DownloadButton.IsVisible = false;
                                    this.ProgressViewLayout.IsVisible = false;
                                    this.LoadingView.IsVisible = false;
                                    this.RetryButton.IsVisible = false;
                                    break;
                                case AudioUpdateStatus.Failed:
                                    this.RetryButton.IsVisible = true;
                                    this.RemoveButton.IsVisible = false;
                                    this.DownloadButton.IsVisible = false;
                                    this.ProgressViewLayout.IsVisible = false;
                                    this.LoadingView.IsVisible = false;
                                    break;
                                default:
                                    this.RemoveButton.IsVisible = false;
                                    this.DownloadButton.IsVisible = true;
                                    this.ProgressViewLayout.IsVisible = false;
                                    this.LoadingView.IsVisible = false;
                                    this.RetryButton.IsVisible = false;
                                    break;
                            }
                            
                        })
                        .DisposeWith(d);
                });
        }
    }
}
