using ReactiveUI;
using ReactiveUI.XamForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TalkiPlay
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnboardingAnimView : ReactiveContentView<OnboardingItemViewModel>
    {
        public OnboardingAnimView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, v => v.Heading, view => view.Heading.Text).DisposeWith(d);
            });
        }
    }
}