using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public class ChildViewModel : ReactiveObject
    {

        public ChildViewModel(IChild child)
        {
            Child = child;

            Name = child.Name;
            Photo = child.PhotoPath.ToResizedImage(Dimensions.DefaultChildImageSize) ?? Images.AvatarPlaceHolder;
            Age = child.Age;
        }

        public IChild Child { get; }
        
        [Reactive]
        public string Name { get; set; }
        
        [Reactive]
        public string Photo { get; set; }
        
        [Reactive]
        public int Age { get; set; }
    }
}