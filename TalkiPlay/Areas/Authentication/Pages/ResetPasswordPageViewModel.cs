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
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class ResetPasswordPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        readonly TokenRequest _request;
        readonly IUserRepository _userService;
        readonly ValidatableObjects _validations;

        public ResetPasswordPageViewModel(
            TokenRequest request,
            INavigationService navigator,
            IUserRepository userService = null
        )
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _userService = userService ?? Locator.Current.GetService<IUserRepository>();
            Navigator = navigator;
            Activator = new ViewModelActivator();
            Password = new ReactiveValidatableObject<string>();
            ConfirmPassword = new ReactiveValidatableObject<string>();
       
            AddValidations();
            SetupCommands();
            SetupRx();
            _validations = new ValidatableObjects {{"Password", Password}, {"ConfirmPassword", ConfirmPassword}};
        }

        public override string Title => "Reset password";

        public ViewModelActivator Activator { get; }

        public ReactiveValidatableObject<string> Password { get; }

        public ReactiveValidatableObject<string> ConfirmPassword { get; }

        public ReactiveCommand<Unit, Unit> ResetPasswordCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }

        public string PasswordData => Password.Value;

        public extern bool IsBusy { [ObservableAsProperty] get; }

        void SetupRx()
        {
            this.WhenActivated(d =>
            {
                this.WhenAnyObservable(m => m.ResetPasswordCommand.IsExecuting)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(m => m)
                    .ToPropertyEx(this, v => v.IsBusy)
                    .DisposeWith(d);
            });
        }

        void SetupCommands()
        {
            ResetPasswordCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!_validations.Validate())
                {
                    return;
                }

                Dialogs.ShowLoading("Resetting ...");
                await _userService.Update(new PatchByTokenUserRequest()
                {
                    Email = _request.Email,
                    Password = PasswordData,
                    Token = _request.Token,
                });
                Dialogs.HideLoading();

                Dialogs.Toast(Dialogs.BuildSuccessToast("Password has been reset successfully. Please log in with your new password."));
                
                // _userDialogs.Toast(new ToastConfig("Password has been reset successfully. Please log in with your new password.")
                // {
                //     Duration = TimeSpan.FromSeconds(5),
                //     Position = ToastPosition.Bottom,
                //     BackgroundColor = Colors.DarkTealColor,
                //     MessageTextColor = Color.White
                // });

                await SimpleNavigationService.PushAsync(new LoginPageViewModel(LoginNavigationSource.Default));
                //await Navigator.PushPage(new LoginPageViewModel(LoginNavigationSource.Default));
            });

            //ResetPasswordCommand = ReactiveCommand.CreateFromObservable(() => 
            //    Observable.Return(_validations.Validate())
            //        .Where(isValid => isValid)
            //        .ShowLoading("Reseting ...")
            //        .ObserveOn(RxApp.TaskpoolScheduler)
            //        .SelectMany(m =>  _userService.Update(new PatchByTokenUserRequest()
            //        {
            //            Email = _request.Email,
            //            Password = PasswordData,
            //            Token = _request.Token,
            //        }))
            //        .ObserveOn(RxApp.MainThreadScheduler)
            //        .HideLoading()
            //        .Do(m =>
            //        {
            //            _userDialogs.Toast(new ToastConfig("Password has been reset successfully. Please login with your new password.")
            //            {
            //                Duration = TimeSpan.FromSeconds(5),
            //                Position = ToastPosition.Bottom,
            //                BackgroundColor = Colors.DarkTealColor,
            //                MessageTextColor = Color.White
            //            });
            //        })
            //        .SelectMany(_ =>  Navigator.PushPage(new LoginPageViewModel(Navigator)))
            //);

            ResetPasswordCommand
                .ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeSafe();

            BackCommand =
                ReactiveCommand.CreateFromTask(async () =>
                    {
                        await SimpleNavigationService.PushAsync(new LoginPageViewModel(LoginNavigationSource.Default));
                        //await Navigator.PushPage(new LoginPageViewModel(LoginNavigationSource.Default));
                    });
            
            BackCommand
                .ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeSafe();
        }

        void AddValidations()
        {
           Password.Validations.Add(new IsNotNullOrEmptyRule<string>(password => !String.IsNullOrWhiteSpace(password),
               ValidationMessages.RequiredValidationMessage("Password")));
           ConfirmPassword.Validations.Add(new IsNotNullOrEmptyRule<string>(password => !String.IsNullOrWhiteSpace(password),
               ValidationMessages.RequiredValidationMessage("Confirm password")));
           ConfirmPassword.Validations.Add(new RepeatPasswordRule(() => PasswordData,
               "Password and confirm password field does not match"));
        }

      
    }
}