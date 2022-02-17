using System.Collections.Generic;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{

    public enum SettingsType
    {
        UpdatePassword,
        UpdateDetails,
        DeviceItems,
        DownloadPrintable,
        Legal,
        Help,
        SwitchMode,
        RestorePurchases,
        
        LegalPrivacy,
        LegalTerms,
        LegalSubscription
    }
    
    public class SettingsItemViewModel : ReactiveObject
    {
        [Reactive]
        public string Label { get; set; }
        
        [Reactive]
        public SettingsType Type { get; set; }
    }

    public class SettingsItemGroup : List<object>
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
    }
}