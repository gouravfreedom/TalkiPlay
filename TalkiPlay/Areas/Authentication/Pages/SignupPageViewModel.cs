using System;
//using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Forms;
namespace TalkiPlay.Shared
{
    public enum SignupNavigationSource
    {
        ModeSelection,
        Login
    }

    public class SignupPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        readonly IUserRepository _userService;
        readonly ValidatableObjects _validations;
        readonly SignupNavigationSource _navSource;
        readonly IUserSettings _userSettings;

        public SignupPageViewModel(                        
            SignupNavigationSource navSource,
            IUserRepository userService = null,
            IUserSettings userSettings = null)
        {
            _navSource = navSource;
            _userService = userService ?? Locator.Current.GetService<IUserRepository>();
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();

            Activator = new ViewModelActivator();
            Email = new ReactiveValidatableObject<string>();
            Password = new ReactiveValidatableObject<string>();            
            Household = new ReactiveValidatableObject<string>();
            ConfirmPassword = new ReactiveValidatableObject<string>();
            HasAgreedToTnc = new ReactiveValidatableObject<bool>();
            HouseholdColor = new ReactiveValidatableObject<Color>();
            CompanyColor = new ReactiveValidatableObject<Color>();
            HouseholdText = new ReactiveValidatableObject<string>();
            HouseholdText.Value = "Household name";
            HouseholdPlaceHolder = new ReactiveValidatableObject<string>();
            HouseholdPlaceHolder.Value = "Household name...";
            HouseholdColor.Value = Color.DarkGray;
            CompanyColor.Value = Color.White;
            AddValidations();
            SetupCommands();
            SetupRx();
            
            _validations = new ValidatableObjects {{"Email", Email}, {"Password", Password},
                {"ConfirmPassword", ConfirmPassword}, {"Household", Household}, {"HasAgreedToTnc", HasAgreedToTnc}};
        }
        
        public override string Title => "Sign up";
        public bool ShowNavBar => true;

        //public bool HasAgreedToTnc { get; set; }

        public ViewModelActivator Activator { get; }

        public ReactiveValidatableObject<string> Email { get; }
    
        public ReactiveValidatableObject<string> Password { get; }

        public ReactiveValidatableObject<string> ConfirmPassword { get; }

        public ReactiveValidatableObject<string> Household { get; }

        public ReactiveValidatableObject<string> HouseholdText { get; }

        public ReactiveValidatableObject<string> HouseholdPlaceHolder { get; }

        public ReactiveValidatableObject<bool> HasAgreedToTnc { get; }

        public ReactiveValidatableObject<Color> HouseholdColor { get; }
        public ReactiveValidatableObject<Color> CompanyColor { get; }

        public ReactiveCommand<Unit, Unit> SignupCommand { get; private set; }
        
        public ReactiveCommand<Unit, Unit> LoginCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> ParentCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> CompanyCommand { get; private set; }


        public extern bool IsBusy { [ObservableAsProperty] get; }

        void SetupRx()
        {
            this.WhenActivated(d =>
            {
                this.WhenAnyObservable(m => m.SignupCommand.IsExecuting)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(m => m)
                    .ToPropertyEx(this, v => v.IsBusy)
                    .DisposeWith(d);
            });
        }

        void SetupCommands()
        {

            SignupCommand = ReactiveCommand.CreateFromTask(async () =>
            {                
                var isValid = _validations.Validate();
                if (!isValid)
                {
                    return;
                }               

                Dialogs.ShowLoading("Registering ...");
                var appService = Locator.Current.GetService<IApplicationService>();
                await _userService.Register(new RegistrationRequest
                {
                    Email = Email.Value,
                    Password = Password.Value,
                    CompanyName = Household.Value,
                    CompanyType = _userSettings.HasTalkiPlayerDevice ? CompanyType.Company : CompanyType.HouseHold,
                    Timezone = appService.Timezone
                });
                Dialogs.HideLoading();

                if (_navSource == SignupNavigationSource.ModeSelection)
                {
                    await Bootstrapper.GotoOnboardingPage();
                }
                else
                {                    
                    Locator.Current.GetService<ITalkiPlayNavigator>().NavigateToTabbedPage();
                }
            });
          
            SignupCommand
                .ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeSafe();
            
            LoginCommand = ReactiveCommand.Create(() =>
            {
                if (_navSource == SignupNavigationSource.Login)
                {
                    SimpleNavigationService.PopAsync().Forget();
                }
                else
                {
                    SimpleNavigationService.PushAsync(new LoginPageViewModel(LoginNavigationSource.Signup)).Forget();
                }
            });

            ParentCommand = ReactiveCommand.Create(() =>
            {
                //_userSettings.HasTalkiPlayerDevice = false;
                HouseholdText.Value = "Household name";
                HouseholdPlaceHolder.Value = "Household name...";
                HouseholdColor.Value = Color.DarkGray;
                CompanyColor.Value = Color.White;
            });

            CompanyCommand = ReactiveCommand.Create(() =>
            {
                //_userSettings.HasTalkiPlayerDevice = true;
                HouseholdText.Value = "Company name";
                HouseholdPlaceHolder.Value = "Company name...";
                HouseholdColor.Value = Color.White;
                CompanyColor.Value = Color.DarkGray;
            });

            LoginCommand
                .ThrownExceptions.SubscribeAndLogException();

            BackCommand = ReactiveCommand.Create(() =>
            {
                SimpleNavigationService.PopAsync().Forget();
            });

        }

        void AddValidations()
        {
            Email.Validations.Add(new IsNotNullOrEmptyRule<string>(email => !String.IsNullOrWhiteSpace(email),
               ValidationMessages.RequiredValidationMessage("Email address")));

            Email.Validations.Add(new EmailRule<string>(ValidationMessages.EmailValidationMessage));

            Password.Validations.Add(new IsNotNullOrEmptyRule<string>(password => !String.IsNullOrWhiteSpace(password),
               ValidationMessages.RequiredValidationMessage("Password")));

            ConfirmPassword.Validations.Add(new IsNotNullOrEmptyRule<string>(password => !String.IsNullOrWhiteSpace(password),
               ValidationMessages.RequiredValidationMessage("Confirm password")));

            ConfirmPassword.Validations.Add(new RepeatPasswordRule(() => Password.Value, "Password and confirm password does not match"));

            Household.Validations.Add(new IsNotNullOrEmptyRule<string>(Household => !String.IsNullOrWhiteSpace(Household),
               ValidationMessages.RequiredValidationMessage("Household name")));

            HasAgreedToTnc.Validations.Add(new IsNotNullOrEmptyRule<bool>(hasAgreed => hasAgreed, "Please agree to the terms and conditions"));
        }

       
    }
}