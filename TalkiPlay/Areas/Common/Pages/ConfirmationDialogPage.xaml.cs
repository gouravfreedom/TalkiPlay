using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class ConfirmationDialogPage : BasePopupPage<ConfirmationDialogPageViewModel>
	{
    

        public ConfirmationDialogPage()
        {
	        InitializeComponent();

	       var completionSource = new TaskCompletionSource<bool>();

	        this.OkButton.Button.Command = new Command(() =>
	        {
		        completionSource.TrySetResult(true);
	        });

	        this.CancelButton.Button.Command = new Command(() =>
	        {
		        completionSource.TrySetResult(false);
	        });

			this.WhenActivated(d =>
			{

				this.ViewModel.ConfirmationInteraction.RegisterHandler(async handler =>
				{
					var result = await completionSource.Task;

					handler.SetOutput(result);

				}).DisposeWith(d);

				this.OneWayBind(ViewModel, v => v.MainTitle, view => view.TitleLabel.Text).DisposeWith(d);
				this.OneWayBind(ViewModel, v => v.YesButtonText, view => view.OkButton.Text).DisposeWith(d);
				this.OneWayBind(ViewModel, v => v.NoButtonText, view => view.CancelButton.Text).DisposeWith(d);
			});
		}

		protected override bool OnBackButtonPressed()
		{
			return false;
		}

		protected override bool OnBackgroundClicked()
		{
			return false;
		}
	}
}
