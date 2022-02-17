using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public class RoomImageItemViewModel : ReactiveObject
    {
        public RoomImageItemViewModel(IAsset asset)
        {
            Asset = asset;
            Image = asset.ImageContentPath;
        }
        
        
        [Reactive]
        public string Image { get; set; }

        public IAsset Asset { get; }
        
        [Reactive]
        public bool IsSelected { get; set; }
    }
}