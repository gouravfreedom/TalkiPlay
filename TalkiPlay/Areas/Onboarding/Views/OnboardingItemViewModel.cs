using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public class OnboardingItemViewModel : ReactiveObject
    {
        [Reactive]
        public string Heading { get; set; }
        
        [Reactive]
        public string SubHeading { get; set; }
        
        [Reactive]
        public string Resource { get; set; }

        [Reactive]
        public double ResourceScale { get; set; } = 1d;

    }
}