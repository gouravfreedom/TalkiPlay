using System.Reactive.Disposables;
using System.Windows.Input;
using ChilliSource.Mobile.UI;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class SettingsMenuItemView : ReactiveContentView<SettingsItemViewModel>
    {
        public SettingsMenuItemView()
        {
            InitializeComponent();
        }

        public ICommand TapCommand { get; set; }

        void OnViewTapped(System.Object sender, System.EventArgs e)
        {
            TapCommand?.Execute(BindingContext);
        }
    }


    public partial class SettingsMenuItemViewCell : BaseCell
    {
        public SettingsMenuItemViewCell()
        {
            View = new SettingsMenuItemView();
        }
    }


    public class GroupHeaderCell : BaseCell
    {

    }
}
