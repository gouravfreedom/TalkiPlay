using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ChilliSource.Mobile.UI;
using ChilliSource.Mobile.UI.ReactiveUI;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class OnboardingChildNamePageViewModel : SimpleBasePageModel
    {        
        readonly QRCodeOnboardingStep _currentStep;
        ValidatableObjects _validations;
        QRCodeOnboardingState _state;

        public OnboardingChildNamePageViewModel(QRCodeOnboardingStep currentStep, QRCodeOnboardingState state)
        {
            _currentStep = currentStep;
            _state = state;
            HeaderText = QRCodeOnboardingHelper.GetHeaderTextForStep(_currentStep);

            SetupCommands();
            SetupValidation();
        }

        public string Title => "";

        public bool ShowNavBar => true;

        public string HeaderText { get; }

        public ICommand NextCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public ValidatableObject<string> FirstName { get; private set; }

        public string FirstNameText
        {
            get => FirstName.Value;
            set => FirstName.Value = value;
        }

        public bool FirstNameIsValid => FirstName.IsValid;
                
        public ObservableCollection<string> FirstNameErrors => FirstName.Errors;
        
        void SetupValidation()
        {
            FirstName = new ValidatableObject<string>()
            {
                Value = _state.Child?.Name ?? ""
            };

            FirstName.Validations.Add(new IsNotNullOrEmptyRule<string>(name => !String.IsNullOrWhiteSpace(name),
                ValidationMessages.RequiredValidationMessage("First name")));
            
            _validations = new ValidatableObjects { { "FirstName", FirstName } };

        }

        void SetupCommands()
        {
            NextCommand = new Command(() =>
            {
                if (!_validations.Validate())
                {
                    RaisePropertyChanged(nameof(FirstNameIsValid));
                    RaisePropertyChanged(nameof(FirstNameErrors));
                    return;
                }

                if (_state.Child == null) _state.Child = new ChildDto();

                _state.Child.Name = FirstName.Value;

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
