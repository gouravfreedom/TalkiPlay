using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace TalkiPlay.Shared
{
    public class AddEditChildPageViewModel : BasePageViewModel, IActivatableViewModel//, IModalViewModelWithParameters
    {
        protected IChild _child;        
        private readonly IConnectivityNotifier _connectivityNotifier;
        protected bool _isEdit;
        private readonly ValidatableObjects _validations;
        private readonly bool _isAsModal = false;

        public AddEditChildPageViewModel(
            bool isAsModal,
            IChild child = null,
            IConnectivityNotifier connectivityNotifier = null)
        {
            _isAsModal = isAsModal;
            _child = child;
            
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            Activator = new ViewModelActivator();
            _isEdit = child != null;
            FirstName = new ReactiveValidatableObject<string>() {Value = _child?.Name};
            DateOfBirth = new ReactiveValidatableObject<DateTime> { Value = _child?.DateOfBirth ?? DateTime.Today };

            AddValidations();

            _validations = new ValidatableObjects { { "FirstName", FirstName }, { "DateOfBirth", DateOfBirth } };

            SetupRx();
            SetupCommands();
        }

        public override string Title => _isEdit ? "Update details" : "Add child";
        public ViewModelActivator Activator { get; }
        public ReactiveCommand<Unit, Unit> NextCommand { get; protected set; }
        
        //public new ReactiveCommand<Unit, Unit> BackCommand { get; protected set; }
        public ReactiveValidatableObject<string> FirstName { get; }
        public ReactiveValidatableObject<DateTime> DateOfBirth { get; }


        public string FirstNameData => FirstName.Value;

        public DateTime DateOfBirthData => DateOfBirth.Value;

        public extern bool IsBusy { [ObservableAsProperty] get; }

        [Reactive]
        public string ButtonText { get; set; } = "Next";
        
        void SetupRx()
        {
            this.WhenActivated(d =>
            {
                _connectivityNotifier.Notifier.RegisterHandler(async context =>
                {
                    await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                    context.SetOutput(true);
                }).DisposeWith(d);
                
            });
        }


        void AddValidations()
        {
            FirstName.Validations.Add(new IsNotNullOrEmptyRule<string>(name => !String.IsNullOrWhiteSpace(name), 
                ValidationMessages.RequiredValidationMessage("First name")));
            DateOfBirth.Validations.Add(new ActionValidationRule<DateTime>(birthDay => birthDay < DateTime.Today, 
                "Please enter a valid date of birth"));
        }

        void SetupCommands()
        {
            NextCommand = ReactiveCommand.Create(() =>
                {
                    var child = new ChildDto()
                    {
                        Id = _child?.Id ?? 0,
                        DateOfBirth = DateOfBirthData,
                        Name = FirstNameData,
                        AssetId = _child?.AssetId ?? 0,
                    };

                    _child = child;

                    if (_validations.Validate())
                    {
                        SimpleNavigationService.PushAsync(new AvatarSelectionPageViewModel(child)).Forget();
                    }
                }
            );
            
            NextCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();
            
            BackCommand = ReactiveCommand.Create(() =>
            {
                if (_isAsModal)
                {
                    SimpleNavigationService.PopModalAsync().Forget();
                }
                else
                {
                    SimpleNavigationService.PopAsync().Forget();
                }
            });
            BackCommand.ThrownExceptions.SubscribeAndLogException();
        }
    }
}
