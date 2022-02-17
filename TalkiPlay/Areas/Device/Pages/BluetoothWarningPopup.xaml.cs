using System.Reactive.Disposables;
using ReactiveUI;
using TalkiPlay.Shared;

namespace TalkiPlay
{
    public partial class BluetoothWarningPopup : BasePopupPage<BluetoothWarningPopupPageViewModel>
	{
		public BluetoothWarningPopup()
		{
			InitializeComponent();

			this.WhenActivated(d =>
			{
				this.BindCommand(ViewModel, v => v.GoToSettingsCommand, view => view.SettingsButton.Button)
					.DisposeWith(d);

				this.BindCommand(ViewModel, v => v.CancelCommand, view => view.CancelButton.Button).DisposeWith(d);
				
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
	}
}
