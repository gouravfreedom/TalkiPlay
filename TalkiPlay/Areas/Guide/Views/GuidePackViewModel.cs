using System;
using System.Windows.Input;
using ChilliSource.Mobile.UI;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class GuidePackViewModel : BaseViewModel
    {
        public GuidePackViewModel(int id, string text, string imageSource, Action<int> callback)
        {
            IsVisible = id > 0;
            Text = text;
            ImageSource = imageSource?.ToResizedImage(height: 100) ?? Images.PlaceHolder;
            
            Command = new Command(() => callback?.Invoke(id));
        }
        
        public string Text { get; }
        
        public string ImageSource { get; }
        
        public ICommand Command { get; }
        
        public bool IsVisible { get; }
    }
}