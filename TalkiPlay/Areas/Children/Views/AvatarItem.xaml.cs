using ReactiveUI.XamForms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    
    public partial class AvatarItem : ReactiveContentView<AvatarItemViewModel>
    {

        TapGestureRecognizer _tapGesture;

        public AvatarItem()
        {
            InitializeComponent();

            _tapGesture = new TapGestureRecognizer();
            _tapGesture.SetBinding(TapGestureRecognizer.CommandProperty, nameof(AvatarItemViewModel.SelectedCommand));
            this.GestureRecognizers.Add(_tapGesture);

            //AvatarFrame.SetBinding(Frame.BorderColorProperty,
            //    nameof(AvatarItemViewModel.IsSelected),
            //    converter: Converters.BooleanToColorConverter);
        }
    }
}
