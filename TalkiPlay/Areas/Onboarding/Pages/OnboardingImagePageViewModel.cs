using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Splat;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class OnboardingImagePageViewModel : SimpleBasePageModel
    {                
        readonly QRCodeOnboardingStep _currentStep;
        private object _nextVM;
        private QRCodeOnboardingState _state;
        public OnboardingImagePageViewModel(QRCodeOnboardingStep currentStep, QRCodeOnboardingState state)
        {
            _state = state;
            _currentStep = currentStep;           
            SetupCommands();

            HeaderText = QRCodeOnboardingHelper.GetHeaderTextForStep(_currentStep);
            ImageSource = QRCodeOnboardingHelper.GetImageForStep(_currentStep);
        }

        public string Title => "";
        public bool ShowNavBar => !QRCodeOnboardingHelper.IsFirstStep(_currentStep) && !QRCodeOnboardingHelper.IsLastStep(_currentStep);

        public string HeaderText { get; }
        public string ButtonText => "Next";
        public string ImageSource { get; }

        public ICommand NextCommand { get; private set; }        
        public ICommand BackCommand { get; private set; }

        void SetupCommands()
        {
            NextCommand = new Command(async () =>
            {
                var userSettings = Locator.Current.GetService<IUserSettings>();
                var navigatorHelper = Locator.Current.GetService<ITalkiPlayNavigator>();
                var childService = Locator.Current.GetService<IChildrenRepository>();
                var children = await childService.GetChildren();
                var firstChild = children.FirstOrDefault();

                if (_nextVM == null)
                {
                    _nextVM = QRCodeOnboardingHelper.GetNextOnboardingViewModel(_currentStep, _state);    
                }

                if(_currentStep == QRCodeOnboardingStep.HuntGame && firstChild != null)
                {
                    if (userSettings.CurrentChild == null)
                    {
                        userSettings.CurrentChild = firstChild;
                    }
                    userSettings.IsQrOnboarded = true;
                    navigatorHelper.NavigateToTabbedPage(TabItemType.Items);
                }
                else
                {
                    await SimpleNavigationService.PushAsync(_nextVM);
                }
            });
            
            BackCommand = new Command(() =>
            {
                SimpleNavigationService.PopAsync().Forget();
            });
        }

    }
}
