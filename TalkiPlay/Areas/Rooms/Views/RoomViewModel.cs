using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class RoomViewModel : ReactiveObject
    {       

        public IRoom Room { get; set; }

        [Reactive]
        public string Title { get; set; }

        [Reactive]
        public string HeroTitle { get; set; }

        [Reactive]
        public string BackgroundImage { get; set; }
        

        public ICommand EditCommand { get; set; }
    }
}