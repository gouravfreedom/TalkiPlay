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
    public class ChangePasswordPageViewModel  : BasePageViewModel, IActivatableViewModel
    {
        readonly IUserRepository _userService;
        //readonly IUserDialogs _userDialogs;
        readonly ValidatableObjects _validations;

        public ChangePasswordPageViewModel(
            INavigationService navigator,
            IUserRepository userService = null
            //IUserDialogs userDialogs = null
        )
        {
            //_userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            _userService = userService ?? Locator.Current.GetService<IUserRepository>();
            //Navigator = navigator;
            Activator = new ViewModelActivator();
            CurrentPassword = new ReactiveValidatableObject<string>();
            Password = new ReactiveValidatableObject<string>();
            ConfirmPassword = new ReactiveValidatableObject<string>();
            AddValidations();
            SetupCommands();
            SetupRx();
            _validations = new ValidatableObjects {{"CurrentPassword", CurrentPassword}, {"Password", Password}, {"ConfirmPassword", ConfirmPassword}};
        }

        public override string Title => "Change password";

        public ViewModelActivator Activator { get; }

        public ReactiveValidatableObject<string> CurrentPassword { get; }

        public ReactiveValidatableObject<string> Password { get; }

        public ReactiveValidatableObject<string> ConfirmPassword { get; }

        public ReactiveCommand<Unit, Unit> ChangePasswordCommand { get; private set; }

        public string PasswordData => Password.Value;

        public string CurrentPasswordData => CurrentPassword.Value;

        public extern bool IsBusy { [ObservableAsProperty] get; }


        void SetupRx()
        {
            this.WhenActivated(d =>
            {
                this.WhenAnyObservable(m => m.ChangePasswordCommand.IsExecuting)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(m => m)
                    .ToPropertyEx(this, v => v.IsBusy)
                    .DisposeWith(d);
            });
        }

        void SetupCommands()
        {

            ChangePasswordCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!_validations.Validate())
                {
                    return;
                }

                Dialogs.ShowLoading("Updating password ...");
                await _userService.Update(new PatchUserRequest()
                {
                    CurrentPassword = CurrentPasswordData,
                    NewPassword = PasswordData,
                    IsPasswordSpecified = true
                });

                Dialogs.Toast(Dialogs.BuildSuccessToast("Password has been updated successfully."));
                // Dialogs.Toast(new ToastConfig("Password has been updated successfully.")
                // {
                //     Duration = TimeSpan.FromSeconds(5),
                //     Position = ToastPosition.Bottom,
                //     BackgroundColor = Colors.DarkTealColor,
                //     MessageTextColor = Color.White
                // });
                Dialogs.HideLoading();

                await SimpleNavigationService.PopModalAsync();
                //await Navigator.PopPage();

            });

            //ChangePasswordCommand = ReactiveCommand.CreateFromObservable(() => 
            //    Observable.Return(_validations.Validate())
            //        .Where(isValid => isValid)
            //        .ShowLoading("Updating password ...")
            //        .ObserveOn(RxApp.TaskpoolScheduler)
            //        .SelectMany(m =>  _userService.Update(new PatchUserRequest()
            //        {
            //            CurrentPassword = CurrentPasswordData,
            //            NewPassword = PasswordData,
            //            IsPasswordSpecified = true
            //        }))
            //        .ObserveOn(RxApp.MainThreadScheduler)
            //        .HideLoading()
            //        .Do(m =>
            //        {
            //            _userDialogs.Toast(new ToastConfig("Password has been updated successfully.")
            //            {
            //                Duration = TimeSpan.FromSeconds(5),
            //                Position = ToastPosition.Bottom,
            //                BackgroundColor = Colors.DarkTealColor,
            //                MessageTextColor = Color.White
            //            });
            //        })
            //        .SelectMany(_ =>  Navigator.PopPage())
            //);

            ChangePasswordCommand
                .ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeSafe();
            
            BackCommand =
                ReactiveCommand.Create(() => SimpleNavigationService.PopModalAsync().Forget());

        }

        void AddValidations()
        {
           CurrentPassword.Validations.Add(new IsNotNullOrEmptyRule<string>(password => !String.IsNullOrWhiteSpace(password),
               ValidationMessages.RequiredValidationMessage("Current password")));
           Password.Validations.Add(new IsNotNullOrEmptyRule<string>(password => !String.IsNullOrWhiteSpace(password),
               ValidationMessages.RequiredValidationMessage("Password")));
           ConfirmPassword.Validations.Add(new IsNotNullOrEmptyRule<string>(password => !String.IsNullOrWhiteSpace(password),
               ValidationMessages.RequiredValidationMessage("Confirm password")));
           ConfirmPassword.Validations.Add(new RepeatPasswordRule(() => PasswordData,
               "Password and confirm password field does not match"));
        }
      
    }
}