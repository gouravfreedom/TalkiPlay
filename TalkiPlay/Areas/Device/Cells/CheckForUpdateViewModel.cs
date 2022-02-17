using System.Reactive;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public class DeviceInfoBaseViewModel : ReactiveObject
    {
        public int Order { get; set; }
        
        [Reactive]
        public string Label { get; set; }
        
        public string FieldName { get; set; }
        
    }

    public enum UpdateType
    {
        Firmware,
        Audio,
        Debug,
        Volume,
        EraseFlash,
        DeepSleep
    }
    
    public class CheckForUpdateViewModel : DeviceInfoBaseViewModel
    {
        public CheckForUpdateViewModel()
        {
            UpdateCommand?.ThrownExceptions.SubscribeAndLogException();
        }
        
        [Reactive]
        public bool HasUpdate { get; set; }
        
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; set; }
        
        public UpdateType Type { get; set; }
    }
    
}