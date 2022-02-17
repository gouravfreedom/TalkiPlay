using System;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class AvatarItemViewModel : ReactiveObject
    {
        Action<int> _selectionCallback;
        public AvatarItemViewModel(IAsset asset, Action<int> selectionCallback)
        {
            IsVisible = asset != null;
            if (asset == null)
            {
                return;
            }
            
            _selectionCallback = selectionCallback;
            Asset = asset;
            AvatarImage = asset.ImageContentPath.ToResizedImage(60);

            SelectedCommand = new Command(() => _selectionCallback?.Invoke(Asset.Id));            
        }
                
        [Reactive]
        public string AvatarImage { get; set; }

        public IAsset Asset { get; }
        
        [Reactive]
        public bool IsSelected { get; set; }

        public ICommand SelectedCommand { get; }
        
        public bool IsVisible { get; }
    }
}