using System;
using System.Windows.Input;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.UI;
using Humanizer;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class ButtonViewModel : BaseViewModel
    {
        public ButtonViewModel(string id, string text, Action<string> callback)
        {
            Text = text;
            Command = new Command(() => callback?.Invoke(id));
        }
        public ButtonViewModel(Enum item, Action<Enum> callback)
        {
            Text = item.Humanize();
            Command = new Command(() => callback?.Invoke(item));
        }
        
        public string Text { get; }
        public ICommand Command { get; }
    }
}