using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class RewardChildViewModel
    {
        public RewardChildViewModel(IChild child, bool isLocked, Action<IChild> callback)
        {
            Child = child;
            Name = child.Name;
            IsLocked = isLocked;
            Command = new Command(() => callback?.Invoke(child));
        }
        public IChild Child { get; }
        
        public bool IsLocked { get; set; }
        
        public string Name { get; set; }
        
        public ICommand Command { get; set; }
    }
}