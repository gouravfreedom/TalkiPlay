using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class EditUserDetailsPageViewModel : BasePageViewModel, IActivatableViewModel
    {              
        //readonly IUserSettings _userSettings;
        readonly IUserRepository _userService;
        //readonly IUserDialogs _userDialogs;        
        static ValidatableObjects _validations;

        public EditUserDetailsPageViewModel(
            INavigationService navigator,
            IUserSettings userSettings = null,
            IUserRepository userService = null,
            IUserDialogs userDialogs = null            
            )
        {
            //_userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();            
            _userService = userService ?? Locator.Current.GetService<IUserRepository>();
            //_userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();
            //Navigator = navigator;
            Activator = new ViewModelActivator();
            Email = new ReactiveValidatableObject<string>();
            FirstName = new ReactiveValidatableObject<string>();
            LastName = new ReactiveValidatableObject<string>();
            AddValidations();
            SetupCommands();
            SetupRx();
            
            PopulateUserFields().Forget();
            // var user = _userSettings.User;
            // Email.Value = user.Email;
            // FirstName.Value = user.FirstName;
            // LastName.Value = user.LastName;
            // _validations = new ValidatableObjects {{"Email", Email}, {"FirstName", FirstName}, {"LastName", LastName}};
        }

        async Task PopulateUserFields()
        {
            var user = await SecureSettingsService.GetUser();
            Email.Value = user.Email;
            FirstName.Value = user.FirstName;
            LastName.Value = user.LastName;
            _validations = new ValidatableObjects {{"Email", Email}, {"FirstName", FirstName}, {"LastName", LastName}};
            
        }
        
        public override string Title => "Edit details";

        public ViewModelActivator Activator { get; }

        public ReactiveValidatableObject<string> Email { get; }

        public ReactiveValidatableObject<string> FirstName { get; }

        public ReactiveValidatableObject<string> LastName { get; }

        public ReactiveCommand<Unit, Unit> UpdateDetailsCommand { get; private set; }

        public string EmailData => Email.Value;

        public string FirstNameData => FirstName.Value;

        public string LastNameData => LastName.Value;

        public extern bool IsBusy { [ObservableAsProperty] get; }

        void SetupRx()
        {
            this.WhenActivated(d =>
            {
                this.WhenAnyObservable(m => m.UpdateDetailsCommand.IsExecuting)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(m => m)
                    .ToPropertyEx(this, v => v.IsBusy)
                    .DisposeWith(d);
            });
        }

        void SetupCommands()
        {
            UpdateDetailsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!_validations.Validate())
                {
                    return;
                }

                Dialogs.ShowLoading("Updating ...");

                await _userService.Update(new PatchUserRequest
                {
                    Email = EmailData,
                    FirstName = FirstNameData,
                    LastName = LastNameData,
                    IsEmailSpecified = true,
                    IsNameSpecified = true,
                });
                Dialogs.HideLoading();

                Dialogs.Toast(Dialogs.BuildSuccessToast("Your details have been updated successfully."));
                // Dialogs.Toast(new ToastConfig("Your details have been updated successfully.")
                // {
                //     Duration = TimeSpan.FromSeconds(5),
                //     Position = ToastPosition.Bottom,
                //     BackgroundColor = Colors.DarkTealColor,
                //     MessageTextColor = Color.White
                // });

                await SimpleNavigationService.PopModalAsync();
                //await Navigator.PopPage();
            });

            BackCommand =
                ReactiveCommand.Create(() => SimpleNavigationService.PopModalAsync().Forget());
            
            //UpdateDetailsCommand = ReactiveCommand.CreateFromObservable(() => 
            //    Observable.Return(_validations.Validate())
            //        .Where(isValid => isValid)
            //        .ShowLoading("Updating ...")
            //        .ObserveOn(RxApp.TaskpoolScheduler)
            //        .SelectMany(m =>  _userService.Update(new PatchUserRequest
            //        {
            //            Email = EmailData,
            //            FirstName = FirstNameData,
            //            LastName = LastNameData,
            //            IsEmailSpecified = true,
            //            IsNameSpecified = true,
            //        }))
            //        .ObserveOn(RxApp.MainThreadScheduler)
            //        .HideLoading()
            //        .Do(m =>
            //        {
            //            _userDialogs.Toast(new ToastConfig("Your details has been updated successfully.")
            //            {
            //                Duration = TimeSpan.FromSeconds(5),
            //                Position = ToastPosition.Bottom,
            //                BackgroundColor = Colors.DarkTealColor,
            //                MessageTextColor = Color.White
            //            });
            //        })
            //        .SelectMany(_ =>  Navigator.PopPage())
            //);

            UpdateDetailsCommand
                .ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeSafe();

        }

        void AddValidations()
        {
           Email.Validations.Add(new IsNotNullOrEmptyRule<string>(email => !String.IsNullOrWhiteSpace(email),
               ValidationMessages.RequiredValidationMessage("Email address")));
           Email.Validations.Add(new EmailRule<string>(ValidationMessages.EmailValidationMessage));
           FirstName.Validations.Add(new IsNotNullOrEmptyRule<string>(password => !String.IsNullOrWhiteSpace(password),
               ValidationMessages.RequiredValidationMessage("First name")));
           LastName.Validations.Add(new IsNotNullOrEmptyRule<string>(password => !String.IsNullOrWhiteSpace(password),
               ValidationMessages.RequiredValidationMessage("Last name")));
        }

      
    }
}