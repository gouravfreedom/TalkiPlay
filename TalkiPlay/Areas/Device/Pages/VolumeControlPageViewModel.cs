﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using Humanizer;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace TalkiPlay.Shared
{
    public class VolumeControlPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        private readonly IConfig _config;
        private readonly ILogger _logger;
        private readonly ITalkiPlayerManager _talkiPlayerManager;
        private readonly IUserDialogs _userDialogs;
        
        public VolumeControlPageViewModel(INavigationService navigator,
            IUserDialogs userDialogs = null,
            ITalkiPlayerManager talkiPlayerManager = null,
            ILogger logger = null,
            IConfig config = null)
        {
            _config = config ?? Locator.Current.GetService<IConfig>();
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            Activator = new ViewModelActivator();
            Navigator = navigator ?? Locator.Current.GetService<INavigationService>(TabItemType.Settings.ToString());

            SetupCommands();
            SetupRx();
        }

        public override string Title => "Volume control";
        public ViewModelActivator Activator { get; }

        private void SetupRx()
        {
            this.WhenActivated(d =>
            {
                   _talkiPlayerManager.Current.OnDataResult()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .HideLoading()
                    .Subscribe(data =>
                    {
                        
                        
                        switch (data.Type)
                        {
                            case UploadDataType.Volume:
                                if (data.IsSuccess)
                                {
                                    _userDialogs.Alert(new AlertConfig()
                                    {
                                        Title = "Volume update",
                                        Message = "Volume has been updated successfully.",
                                        OkText =  "Ok",
                                    });
                                }
                                else
                                {
                                    _userDialogs.Alert(new AlertConfig()
                                    {
                                        Title = "Volume update",
                                        Message = "Failed to update Volume. Please try again!",
                                        OkText =  "Ok",
                                    });
                                }
                                break;
                            case UploadDataType.VolumeInfo:
                                if (data.IsSuccess)
                                {
                                    var result = data.GetData<VolumeInfo>();

                                    if (result != null)
                                    {
                                        Volume = result.Volume;
                                    }
                                }
                                break;
                        }
                        
                     
                      
                    }, ex => { })
                    .DisposeWith(d);


            });
        }
        private void SetupCommands()
        {
            BackCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SimpleNavigationService.PopModalAsync();
            });
            BackCommand.ThrownExceptions.ObserveOn(RxApp.MainThreadScheduler).SubscribeAndLogException();

            LoadCommand = ReactiveCommand.Create(() =>
            {
               // _talkiPlayerManager.Current.Upload(new DataUploadData("VolumeInfo", DataRequest.VolumeInfoRequest(), "VolumeInfo", UploadDataType.VolumeInfo));
                
            });
            
            LoadCommand.ThrownExceptions.SubscribeAndLogException();
            
            UpdateCommand = ReactiveCommand.Create( () =>
            {
                _talkiPlayerManager.Current.Upload(new DataUploadData("VolumeUpdate", DataRequest.VolumeRequest((int) Volume), "VolumeUpdate", UploadDataType.Volume));
            });
            UpdateCommand.ThrownExceptions.SubscribeAndLogException();
        }

        public ReactiveCommand<Unit, Unit> UpdateCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LoadCommand { get; private set; }

        [Reactive] public double Volume { get; set; } = 50;
    }
}
