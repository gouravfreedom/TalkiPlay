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
    public class AcceptInvitePageViewModel : BasePageViewModel, IActivatableViewModel
    {
        readonly TokenRequest _request;
        readonly IUserSettings _userSettings;
        readonly IUserRepository _userService;
        readonly IUserDialogs _userDialogs;
        readonly IConfig _config;
        readonly ValidatableObjects _validations;

        public AcceptInvitePageViewModel(
            TokenRequest request,
            INavigationService navigator,
            IUserSettings userSettings = null,
            IUserRepository userService = null,
            IUserDialogs userDialogs = null,
            IConfig config = null
            )
        {
        
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            _config = config ?? Locator.Current.GetService<IConfig>();
            _userService = userService ?? Locator.Current.GetService<IUserRepository>();
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            Navigator = navigator;
            Activator = new ViewModelActivator();
            Email = new ReactiveValidatableObject<string>();
            Password = new ReactiveValidatableObject<string>();
            FirstName = new ReactiveValidatableObject<string>();
            LastName = new ReactiveValidatableObject<string>();
            ConfirmPassword = new ReactiveValidatableObject<string>();
       
            AddValidations();
            SetupCommands();
            SetupRx();
            Email.Value = _request.Email;
            _validations = new ValidatableObjects {{"Email", Email}, {"Password", Password}, {"FirstName", FirstName}, {"LastName", LastName}, {"ConfirmPassword", ConfirmPassword}};
        }
        
        public override string Title => "Registration";

        public ViewModelActivator Activator { get; }

        public ReactiveValidatableObject<string> Email { get; }

        public ReactiveValidatableObject<string> FirstName { get; }

        public ReactiveValidatableObject<string> LastName { get; }

        public ReactiveValidatableObject<string> Password { get; }

        public ReactiveValidatableObject<string> ConfirmPassword { get; }

        public ReactiveCommand<Unit, Unit> SignupCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }

        public string EmailData => Email.Value;

        public string PasswordData => Password.Value;

        public string FirstNameData => FirstName.Value;

        public string LastNameData => LastName.Value;

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
                _userDialogs.ShowLoading("Registering ...");
                await _userService.Update(new PatchByTokenUserRequest
                {
                    Email = EmailData,
                    Password = PasswordData,
                    Token = _request.Token,
                    FirstName = FirstNameData,
                    LastName = LastNameData,
                });
                await _userService.Login(new LoginRequest
                {
                    Email = EmailData,
                    Password = PasswordData,
                    DeviceId = _userSettings.UniqueId
                });
                _userDialogs.HideLoading();

                var navigatorHelper = Locator.Current.GetService<ITalkiPlayNavigator>();
                navigatorHelper.NavigateToTabbedPage();
            });

            //SignupCommand = ReactiveCommand.CreateFromObservable(() => 
            //    Observable.Return(_validations.Validate())
            //        .Where(isValid => isValid)
            //        .ShowLoading("Registering ...")
            //        .ObserveOn(RxApp.TaskpoolScheduler)
            //        .SelectMany(m =>  _userService.Update(new PatchByTokenUserRequest
            //        {
            //            Email = EmailData,
            //            Password = PasswordData,
            //            Token = _request.Token,
            //            FirstName = FirstNameData,
            //            LastName = LastNameData,
            //        }))
            //        .SelectMany(m => _userService.Login(new LoginRequest
            //        {
            //            Email = EmailData,
            //            Password = PasswordData,
            //            DeviceId = _userSettings.UniqueId
            //        }))
            //        .ObserveOn(RxApp.MainThreadScheduler)
            //        .HideLoading()
            //        .Do(m =>
            //        {
            //            var navigatorHelper = Locator.Current.GetService<ITalkiPlayNavigator>();
            //            navigatorHelper.NavigateToTabbedPage();

            //        })
            //        .Select(m => Unit.Default)
            //);

            SignupCommand
                .ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeSafe();


            LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
            {

                _userDialogs.ShowLoading("Loading ...");
                var user = await _userService.GetUser(_request);
                FirstName.Value = user.FirstName;
                LastName.Value = user.LastName;
                _userDialogs.HideLoading();

            });

            //LoadDataCommand = ReactiveCommand.CreateFromObservable(() => Observable.Return(_request)
            //    .ShowLoading("Loading ...")
            //    .ObserveOn(RxApp.TaskpoolScheduler)
            //    .SelectMany(m => _userService.GetUser(m))
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Do(m =>
            //    {
            //        FirstName.Value = m.FirstName;
            //        LastName.Value = m.LastName;
            //    })
            //    .HideLoading()
            //    .Select(_ => Unit.Default));
            
            LoadDataCommand
                .ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                //.HideLoading()
                .ShowExceptionDialog()
                .SubscribeSafe();
           
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
           ConfirmPassword.Validations.Add(new RepeatPasswordRule(() => PasswordData, "Password and confirm password does not match"));
           FirstName.Validations.Add(new IsNotNullOrEmptyRule<string>(password => !String.IsNullOrWhiteSpace(password),
               ValidationMessages.RequiredValidationMessage("First name")));
           LastName.Validations.Add(new IsNotNullOrEmptyRule<string>(password => !String.IsNullOrWhiteSpace(password),
               ValidationMessages.RequiredValidationMessage("Last name")));
        }

       
    }
}