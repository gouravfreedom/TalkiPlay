using ChilliSource.Mobile.UI.ReactiveUI;
using Splat;
using TalkiPlay.Managers;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public enum ModeSelectionPageSource
    {
        Startup,
        Settings
    }

    public class ModeSelectionPageViewModel : SimpleBasePageModel
    {

        readonly IUserSettings _userSettings;
        readonly ModeSelectionPageSource _source;

        public ModeSelectionPageViewModel(
            IUserSettings userSettings = null,
            ModeSelectionPageSource source = ModeSelectionPageSource.Settings)
        {
            _source = source;
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            SetupCommands();
        }

        public string Title => _source == ModeSelectionPageSource.Startup ? "" : "Mode Selection";

        public bool ShowNavBar => _source == ModeSelectionPageSource.Settings;

        public Command BuyerModeCommand { get; private set; }
        public Command ParentModeCommand { get; private set; }
        public Command BackCommand { get; private set; }
        public Command WaitModeCommand { get; private set; }

        void SetupCommands()
        {
            WaitModeCommand = new Command(() =>
            {
                WebpageHelper.OpenUrl(Config.WaitLink, "");
            });

            BuyerModeCommand = new Command(() =>
            {
                WebpageHelper.OpenUrl(Config.PurchaseLink, "");
            });

            ParentModeCommand = new Command(async () =>
            {
                _userSettings.HasTalkiPlayerDevice = false;
                await Bootstrapper.GotoOnboardingPage();
            });

            BackCommand = new Command(() =>
            {
                SimpleNavigationService.PopModalAsync().Forget();
            });
        }

    }
}


