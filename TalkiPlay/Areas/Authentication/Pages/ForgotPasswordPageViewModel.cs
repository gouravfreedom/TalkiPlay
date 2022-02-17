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
    public class ForgotPasswordPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        readonly IUserDialogs _userDialogs;
        readonly ValidatableObjects _validations;        
        readonly IUserRepository _userService;
        
        
        public ForgotPasswordPageViewModel(
            IUserRepository userService = null,
            IUserDialogs userDialogs = null)
        {
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();            
            _userService = userService ?? Locator.Current.GetService<IUserRepository>();
            //Navigator = navigator;
            Activator = new ViewModelActivator();
            Email = new ReactiveValidatableObject<string>();
            AddValidations();
            SetupCommands();
            SetupRx();
            _validations = new ValidatableObjects {{"Email", Email}};
        }
        
        
        public override string Title => "";

        public ViewModelActivator Activator { get; }

        public ReactiveValidatableObject<string> Email { get; }

        public ReactiveCommand<Unit, Unit> RequestForgotPasswordTokenCommand { get; private set; }

        public string EmailData => Email.Value;

        public extern bool IsBusy { [ObservableAsProperty] get; }

        [Reactive]
        public bool IsRequestSent { get; set; }

        [Reactive]
        public string SuccessMessage { get; set; }

        void SetupRx()
        {
            this.WhenActivated(d =>
            {

                this.WhenAnyObservable(m => m.RequestForgotPasswordTokenCommand.IsExecuting)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(m => m)
                    .ToPropertyEx(this, v => v.IsBusy)
                    .DisposeWith(d);
            });
        }

        void SetupCommands()
        {

            RequestForgotPasswordTokenCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var isValid = _validations.Validate();
                if (!isValid)
                {
                    return;
                }
                _userDialogs.ShowLoading("Requesting ...");
                await _userService.RequestForgotPasswordToken(new ForgotPasswordTokenRequest
                {
                    Email = EmailData,
                });
                _userDialogs.HideLoading();
                IsRequestSent = true;
                SuccessMessage = $"An email has been sent to {EmailData} with instructions to set a new password.";

            });

            //RequestForgotPasswordTokenCommand = ReactiveCommand.CreateFromObservable(() => 
            //    Observable.Return(_validations.Validate())
            //        .Where(isValid => isValid)
            //        .ShowLoading("Requesting ...")
            //        .ObserveOn(RxApp.TaskpoolScheduler)
            //        .SelectMany(m =>  _userService.RequestForgotPasswordToken(new ForgotPasswordTokenRequest
            //        {
            //            Email = EmailData,
            //        }))
            //        .ObserveOn(RxApp.MainThreadScheduler)
            //        .HideLoading()
            //        .Do(m =>
            //        {
            //            IsRequestSent = true;
            //            SuccessMessage = $"An email has been sent to {EmailData} with instructions to set a new password.";
            //        })
            //        .Select(m => Unit.Default)
            //);

            RequestForgotPasswordTokenCommand
                .ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                //.HideLoading()
                .ShowExceptionDialog()
                .SubscribeSafe();

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
        }

        
    }
}