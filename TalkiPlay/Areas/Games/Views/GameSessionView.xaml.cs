using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class GameSessionView : ReactiveContentView<GameSessionViewModel>
    {
        public GameSessionView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.RewardImage.IsVisible = false;
                this.DoneButton.IsVisible = false;

                this.BindCommand(ViewModel, v => v.DoneCommand, v => v.DoneButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.StartPlayingAnim, v => v.StartPlayingEggWiggle).DisposeWith(d);
            });
        }

        void OnWiggleAnimFinish(System.Object sender, System.EventArgs e)
        {
            (BindingContext as GameSessionViewModel).Title = "Awesome!!";
            (BindingContext as GameSessionViewModel).Description = "";
            this.RewardAnim.IsVisible = false;
            this.RewardImage.IsVisible = true;
            this.DoneButton.IsVisible = true;            
        }

        public readonly static BindableProperty StartPlayingEggWiggleProperty = BindableProperty.Create(nameof(StartPlayingEggWiggle), typeof(bool),
            typeof(GameSessionView), false, propertyChanged:
            (bindable, value, newValue) =>
            {
                if (bindable is GameSessionView view)
                {
                    if ((bool)newValue)
                    {
                        view.RewardAnim.Play();
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
