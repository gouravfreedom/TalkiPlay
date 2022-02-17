using ChilliSource.Mobile.UI.ReactiveUI;
using Splat;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class NewModeSelectionPageViewModel : SimpleBasePageModel
    {

        readonly IUserSettings _userSettings;
        readonly ModeSelectionPageSource _source;

        public NewModeSelectionPageViewModel(
            IUserSettings userSettings = null,
            ModeSelectionPageSource source = ModeSelectionPageSource.Settings)
        {
            _source = source;
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            SetupCommands();
        }

        public string Title => _source == ModeSelectionPageSource.Startup ? "" : "Mode Selection";

        public bool ShowNavBar => _source == ModeSelectionPageSource.Settings;

        public Command EducatorModeCommand { get; private set; }
        public Command NewUserModeCommand { get; private set; }
        public Command BackCommand { get; private set; }

        void SetupCommands()
        {
            EducatorModeCommand = new Command(async () =>
            {
                _userSettings.HasTalkiPlayerDevice = true;
                await Bootstrapper.GotoOnboardingPage();
            });

            NewUserModeCommand = new Command(async () =>
            {
                //_userSettings.HasTalkiPlayerDevice = false;
                //await Bootstrapper.GotoOnboardingPage();
                SimpleNavigationService.PushAsync(new ModeSelectionPageViewModel()).Forget();
            });

            BackCommand = new Command(() =>
            {
                SimpleNavigationService.PopModalAsync().Forget();
            });
        }

    }
}


