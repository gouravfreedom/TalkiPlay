using System;
using System.Windows.Input;
using ChilliSource.Mobile.UI;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class GuideGameViewModel : BaseViewModel
    {
        public GuideGameViewModel(IGame game, bool userHasSubscription)
        {
             Text = game.Name;
             ImageSource = game.ImagePath.ToResizedImage(80);
             IsLocked = game.AccessLevel != GameAccessLevel.Free && !userHasSubscription;
        }
        
        public string Text { get; }
        public string ImageSource { get; }
        
        public bool IsLocked { get; }
    }
}