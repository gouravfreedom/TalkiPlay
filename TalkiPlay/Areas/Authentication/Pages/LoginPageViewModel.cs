using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace TalkiPlay.Shared
{
    public enum LoginNavigationSource
    {
        Default,
        Signup, 
        DeviceOnboarding,
        QRCodeOnboarding,
    }

    public class LoginPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        //private readonly IUserDialogs _userDialogs;
        private readonly ValidatableObjects _validations;        
        private readonly IUserRepository _userService;
        private readonly IUserSettings _userSettings;
        readonly LoginNavigationSource _navSource;

        public LoginPageViewModel(
            //INavigationService navigator,
            LoginNavigationSource navSource,
            IUserSettings userSettings = null,
            IUserRepository userService = null
            //IUserDialogs userDialogs = null            
            )
        {
            _navSource = navSource;
            //_userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();            
            _userService = userService ?? Locator.Current.GetService<IUserRepository>();
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            //Navigator = navigator;
            Activator = new ViewModelActivator();
            Email = new ReactiveValidatableObject<string>();
            Password = new ReactiveValidatableObject<string>();
            _validations = new ValidatableObjects {{"Email", Email}, {"Password", Password}};
            AddValidations();
            SetupCommands();
            SetupRx();
        }
                
        public override string Title => "";

        public bool ShowNavBar => _navSource == LoginNavigationSource.Signup;
        

        public ViewModelActivator Activator { get; }

        public ReactiveValidatableObject<string> Email { get; }

        public ReactiveValidatableObject<string> Password { get; }

        public ReactiveCommand<Unit, Unit> ForgotPasswordCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> SignupCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; private set; }

        public string EmailData => Email.Value;

        public string PasswordData => Password.Value;

        public extern bool IsBusy { [ObservableAsProperty] get; }

        void SetupRx()
        {
            this.WhenActivated(d =>
            {

                this.WhenAnyObservable(m => m.LoginCommand.IsExecuting)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(m => m)
                    .ToPropertyEx(this, v => v.IsBusy)
                    .DisposeWith(d);
            });
        }

        void SetupCommands()
        {
            LoginCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (_validations.Validate())
                {
                    Dialogs.ShowLoading("Signing in ...");
                    await _userService.Login(new LoginRequest
                    {
                        Email = EmailData,
                        Password = PasswordData,
                        DeviceId = _userSettings.UniqueId
                    });

                    await SubscriptionHelper.VerifySubscription();                    
                    var navigatorHelper = Locator.Current.GetService<ITalkiPlayNavigator>();
                    if (_userSettings.IsOnboarded)
                    {
                        navigatorHelper.NavigateToTabbedPage();
                    }
                    else
                    {
                        await Bootstrapper.GotoOnboardingPage();
                    }
                    Dialogs.HideLoading();
                }

            });

            LoginCommand
                .ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeSafe();
            
         
            ForgotPasswordCommand =  ReactiveCommand.Create(() =>
            {
                SimpleNavigationService.PushAsync(new ForgotPasswordPageViewModel()).Forget();                
            });
            ForgotPasswordCommand
                .ThrownExceptions.SubscribeAndLogException();

            SignupCommand = ReactiveCommand.Create(() =>
            {
                if (_navSource == LoginNavigationSource.Default)
                {
                    SimpleNavigationService.PushAsync(new SignupPageViewModel(SignupNavigationSource.Login)).Forget();
                }
                else
                {
                    SimpleNavigationService.PopAsync().Forget();                    
                }                
            });
            SignupCommand
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
        }

       

    }
}