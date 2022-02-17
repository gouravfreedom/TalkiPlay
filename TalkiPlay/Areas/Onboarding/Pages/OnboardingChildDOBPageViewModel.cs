using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ChilliSource.Mobile.UI;
using ChilliSource.Mobile.UI.ReactiveUI;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class OnboardingChildDOBPageViewModel : SimpleBasePageModel
    {
        readonly QRCodeOnboardingState _state;
        ValidatableObjects _validations;
        readonly QRCodeOnboardingStep _currentStep;

        public OnboardingChildDOBPageViewModel(QRCodeOnboardingStep currentStep, QRCodeOnboardingState state)
        {
            _state = state;
            _currentStep = currentStep;
            HeaderText = string.Format(QRCodeOnboardingHelper.GetHeaderTextForStep(currentStep), _state.Child.Name);

            SetupCommands();
            SetupValidation();
        }

        public string Title => "";

        public bool ShowNavBar => true;

        public string HeaderText { get; }        

        public ICommand NextCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public ValidatableObject<DateTime> DateOfBirth { get; private set; }

        public DateTime DateOfBirthValue
        {
            get => DateOfBirth.Value;
            set => DateOfBirth.Value = value;
        }

        public bool DateOfBirthIsValid => DateOfBirth.IsValid;
                
        public ObservableCollection<string> DateOfBirthErrors => DateOfBirth.Errors;
        
        void SetupValidation()
        {
            DateOfBirth = new ValidatableObject<DateTime>()
            {
                Value = _state.Child?.DateOfBirth ?? DateTime.Today
            };
            
            DateOfBirth.Validations.Add(new ActionValidationRule<DateTime>(birthDay => birthDay < DateTime.Today, "Please enter a valid date of birth"));
            
            _validations = new ValidatableObjects { { "DateOfBirth", DateOfBirth } };
        }

        void SetupCommands()
        {
            NextCommand = new Command(() =>
            {
                if (!_validations.Validate())
                {
                    RaisePropertyChanged(nameof(DateOfBirthIsValid));
                    RaisePropertyChanged(nameof(DateOfBirthErrors));
                    return;
                }

                _state.Child.DateOfBirth = DateOfBirth.Value;
              
                var vm = QRCodeOnboardingHelper.GetNextOnboardingViewModel(_currentStep, _state);
                SimpleNavigationService.PushAsync(vm).Forget();
            });


            BackCommand = new Command(() =>
            {
                SimpleNavigationService.PopAsync().Forget();
            });
        }
    }
}
