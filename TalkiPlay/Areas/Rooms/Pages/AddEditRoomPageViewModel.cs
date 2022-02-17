﻿using System;
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
    public class AddEditRoomPageViewModel : BasePageViewModel, IActivatableViewModel, IModalViewModelWithParameters
    {
        protected IRoom _room;        
        private readonly IConnectivityNotifier _connectivityNotifier;
        protected bool _isEdit;
        readonly ValidatableObjects _validations;
        readonly IUserDialogs _userDialogs;

        public AddEditRoomPageViewModel(INavigationService navigator,
            IRoom room = null,
            IUserDialogs userDialogs = null,
            IConnectivityNotifier connectivityNotifier = null)
        {
            _room = room;

            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            Activator = new ViewModelActivator();
            Navigator = navigator;
            _isEdit = room != null;
            ButtonText = _isEdit ? "Update" : "Next";
            Name = new ReactiveValidatableObject<string>();
            
            AddValidations();

            _validations = new ValidatableObjects { { "Name", Name }};

            SetupRx();
            SetupCommands();
        }

        public override string Title => _isEdit ? "Update details" : "Add a room";
        public ViewModelActivator Activator { get; }
        public ReactiveCommand<Unit, Unit> NextCommand { get; protected set; }
        public ReactiveValidatableObject<string> Name { get; }        


        public string NameData => Name.Value;        

        public extern bool IsBusy { [ObservableAsProperty] get; }

        [Reactive]
        public string ButtonText { get; set; } = "Next";

        [Reactive]
        public object Parameters { get; set; }

        void SetupRx()
        {
            this.WhenActivated(d =>
            {
                _connectivityNotifier.Notifier.RegisterHandler(async context =>
                {
                    await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                    context.SetOutput(true);
                }).DisposeWith(d);

                this.WhenAnyObservable(m => m.NextCommand.IsExecuting)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(m => m)
                    .ToPropertyEx(this, v => v.IsBusy)
                    .DisposeWith(d);

                this.WhenAnyValue(m => m.Parameters)
                    .Select(m => m as IRoom)
                    .Where(m => m != null)
                    .SubscribeSafe(m =>
                    {
                        Parameters = null;
                        _room = m;
                        _isEdit = true;
                        this.Name.Value = m.Name;                        
                    })
                    .DisposeWith(d);
            });
        }

        void AddValidations()
        {
            Name.Validations.Add(new IsNotNullOrEmptyRule<string>(name => !String.IsNullOrWhiteSpace(name), ValidationMessages.RequiredValidationMessage("Name")));            
        }

        void SetupCommands()
        {
            NextCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                var room = new RoomDto()
                {
                    Id = _room?.Id ?? 0,                    
                    Name = NameData,                    
                    AssetId = _room?.AssetId ?? 0,
                };

                _room = room;

                return Observable.Return(_validations.Validate())
                    .Where(isValid => isValid)
                    .SelectMany(_ => Observable.FromAsync(() => SimpleNavigationService.PushAsync(new RoomImageSelectionPageViewModel(Navigator, room))));
            });

            NextCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();


            BackCommand = ReactiveCommand.CreateFromTask(() => SimpleNavigationService.PopModalAsync());
            BackCommand.ThrownExceptions.SubscribeAndLogException();
        }

    }


}
