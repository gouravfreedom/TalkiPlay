using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public class AudioPackItemViewModel   : ReactiveObject
    {
        public AudioPackItemViewModel(IPack pack)
        {
            Pack = pack;
            ModifiedPack = pack;
        }

        public IPack Pack { get; }
        public IPack ModifiedPack { get; set; }

        [Reactive]
        public string Name { get; set; }
    
     
        [Reactive] public string Icon { get; set; } = Images.DownloadIcon;

    }
    
}