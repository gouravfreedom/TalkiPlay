using System.Reactive;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public abstract class PlayerViewModel
    {
        public abstract int Id { get;  }
    }

    public class ChildPlayerViewModel : PlayerViewModel
    {
        public ChildPlayerViewModel(IChild cihld,
            ReactiveCommand<IChild, Unit> removeCommand
            )
        {
            Child = cihld;
            Name = Child.Name;
            Avatar = Child.PhotoPath/*.ToResizedImage(width:80)*/ ?? Images.AvatarPlaceHolder;
            RemoveCommand = ReactiveCommand.CreateFromObservable(() => removeCommand.Execute(this.Child));
            RemoveCommand.ThrownExceptions.SubscribeAndLogException();
        }
        
         [Reactive] public string Name { get; set; }
         [Reactive] public string Avatar { get; set; }
         
         public IChild Child { get; }
         public override int Id => Child.Id;
         public ReactiveCommand<Unit, Unit> RemoveCommand { get; }
    }
    
    public class SelectPlayerViewModel : PlayerViewModel
    {
        public SelectPlayerViewModel(
            ReactiveCommand<Unit, Unit> selectCommand
        )
        {
            SelectCommand = ReactiveCommand.CreateFromObservable(() => selectCommand.Execute());
            SelectCommand.ThrownExceptions.SubscribeAndLogException();
        }
        
        public override int Id => 0;
        
        public ReactiveCommand<Unit, Unit> SelectCommand { get; }
    }
}