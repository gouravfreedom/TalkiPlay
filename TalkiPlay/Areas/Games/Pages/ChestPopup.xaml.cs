using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class ChestPage : BasePopupPage<ChestPageViewModel>
    {
        public ChestPage()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.Image.IsVisible = false ;
                this.CloseButton.IsVisible = false;

                this.WhenAnyValue(m => m.ViewModel.LoadDataCommand)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.LoadDataCommand)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, v => v.RewardInstruction, v => v.RewardInstruction.Text).DisposeWith(d);

                this.BindCommand(ViewModel, v => v.BackCommand, v => v.CloseButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.StartPlayingAnim, v => v.StartPlayingEggWiggle).DisposeWith(d);
            });
        }        

        protected override bool OnBackgroundClicked()
        {
            return false;
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        void OnWiggleAnimFinish(System.Object sender, System.EventArgs e)
        {
            this.Anim.IsVisible = false;
            this.Image.IsVisible = true;
            this.CloseButton.IsVisible = true;
        }

        public readonly static BindableProperty StartPlayingEggWiggleProperty = BindableProperty.Create(nameof(StartPlayingEggWiggle), typeof(bool),
            typeof(ChestPage), false, propertyChanged:
            (bindable, value, newValue) =>
            {
                if (bindable is ChestPage view)
                {
                    if ((bool)newValue)
                    {
                        view.Anim.Play();
                    }
                }

            });

        public bool StartPlayingEggWiggle
        {
            get => (bool)GetValue(StartPlayingEggWiggleProperty);
            set => SetValue(StartPlayingEggWiggleProperty, value);
        }
    }
}
