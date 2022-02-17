﻿using System;
using System.Reactive.Linq;
using ChilliSource.Mobile.Core;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;
using Unit = System.Reactive.Unit;

namespace TalkiPlay.Shared
{
    public class BluetoothWarningPopupPageViewModel : ReactiveObject, IPopModalViewModel
    {
        private readonly ILogger _logger;
        private readonly INavigationService _navigator;

        public BluetoothWarningPopupPageViewModel(
            INavigationService navigator,
            ILogger logger = null)
        {
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            _navigator = navigator;

            GoToSettingsCommand = ReactiveCommand.CreateFromTask( async () =>
            {
                await _navigator.PopPopup();
                OpenSettings?.Invoke();
            });

            GoToSettingsCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(error => _navigator.PopPopup()
                    .Select(m => error))
                .SubscribeAndLogException();

            CancelCommand = ReactiveCommand.CreateFromObservable(() => _navigator.PopPopup());
            CancelCommand.ThrownExceptions.SubscribeAndLogException();
        }

       

        public string Title => "";

        [Reactive]
        public string Message { get; set; }

        [Reactive]
        public string ButtonText { get; set; }

        public ReactiveCommand<Unit,Unit> GoToSettingsCommand { get; private set; }
        public ReactiveCommand<Unit,Unit> CancelCommand { get; private set; }

        
        public Action OpenSettings { get; set; }

      
    }
}
