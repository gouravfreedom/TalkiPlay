using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;

namespace TalkiPlay
{
    public partial class OnboardingView : ReactiveContentView<OnboardingItemViewModel>
    {
        public OnboardingView()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, v => v.Heading, view => view.Heading.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.SubHeading, view => view.SubHeading.Text).DisposeWith(d);
            });
        }
    }
}
