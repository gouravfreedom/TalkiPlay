using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public class DeviceInfoViewModel : DeviceInfoBaseViewModel
    {
        [Reactive] public string Value { get; set; }


    }
}