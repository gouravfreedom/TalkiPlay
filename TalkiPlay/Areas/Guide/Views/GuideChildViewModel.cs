using System;
using System.Windows.Input;
using ChilliSource.Mobile.UI;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class GuideChildViewModel : BaseViewModel
    {
        public GuideChildViewModel(IChild child, Action<IChild> callback)
        {
            IsVisible = child != null;
            if (child == null)
            {
                return;
            }
            
            Name = child.Name;
            ImageSource = child.PhotoPath.ToResizedImage(Dimensions.DefaultChildImageSize) ?? Images.AvatarPlaceHolder;
            Command = new Command(() => callback?.Invoke(child));
        }
        
        public string Name { get;  }
        public string ImageSource { get; }
        
        public ICommand Command { get; }
        
        public bool IsVisible { get; }
    }
}