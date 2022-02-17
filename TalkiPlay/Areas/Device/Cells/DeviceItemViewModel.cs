﻿using System.Reactive;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using Plugin.BluetoothLE;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace TalkiPlay.Shared
{
    public class DeviceItemViewModel : ReactiveObject
    {
        private readonly IDevice _bleDevice;
        private readonly INavigationService _navigator;

        public DeviceItemViewModel(IDevice bleDevice,
            INavigationService navigator = null
        )
        {
            _bleDevice = bleDevice;
            _navigator = navigator ?? Locator.Current.GetService<INavigationService>(TabItemType.Settings.ToString());

            this.WhenAnyValue(m => m.Selected)
                .Select(m =>
                {
                    return m ? Images.SelectedIcon : Images.UnSelectedIcon;
                })
                .ToPropertyEx(this, x => x.Icon);

            SelectCommand = ReactiveCommand.Create(() =>
            {
                this.Selected = !this.Selected;
            });
            SelectCommand.ThrownExceptions.SubscribeAndLogException();

            Name = _bleDevice.Name;
        }
        
        [Reactive]
        public bool Selected { get; set; }
        
        [Reactive]
        public string Name { get; set; }
        
        public ReactiveCommand<Unit, Unit> SelectCommand { get; }
        
        public extern string Icon { [ObservableAsProperty]get;}

        public IDevice BleDevice => _bleDevice;
    }
}