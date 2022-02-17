using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace TalkiPlay.Shared
{
    public class ChildDetailsPageViewModel : BasePageViewModel, IActivatableViewModel//, IModalViewModel
    {
        private IChild _child;
        
        private readonly IChildrenRepository _childrenService;

        private readonly IConnectivityNotifier _connectivityNotifier;
      
        public ChildDetailsPageViewModel(
            IChild child,
            IConnectivityNotifier connectivityNotifier = null,
            IChildrenRepository childrenService = null)
        {
            _childrenService = childrenService ?? Locator.Current.GetService<IChildrenRepository>();
            _connectivityNotifier = connectivityNotifier ?? Locator.Current.GetService<IConnectivityNotifier>();
            Activator = new ViewModelActivator();
            SetData(child);
            SetupRx();
            SetupCommands();
        }
        
        public override string Title => "Child's details";
        public ViewModelActivator Activator { get; }
         
        [Reactive] 
        public string Name { get; set; }
        [Reactive] 
        public string Age { get; set; }
        [Reactive] 
        public DateTime? Birthday { get; set; }
         
        [Reactive] 
        public string AvatarImage { get; set; }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }
        
        //public new ReactiveCommand<Unit, Unit> BackCommand { get; private set; }
        
        public ReactiveCommand<Unit, Unit> ChangeCommand { get; private set; }
       
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

         void SetupCommands()
         {
             LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
             {
                //Dialogs.ShowLoading();
                var child = await _childrenService.GetChild(_child.Id);
                SetData(child);
                //Dialogs.HideLoading();
             });
             
             LoadDataCommand.ThrownExceptions
                 .ObserveOn(RxApp.MainThreadScheduler)
                 .ShowExceptionDialog()
                 .SubscribeAndLogException();
             
             BackCommand = ReactiveCommand.Create(() =>  SimpleNavigationService.PopAsync().Forget());
             BackCommand.ThrownExceptions.SubscribeAndLogException();
             
             ChangeCommand = ReactiveCommand.Create(()=> SimpleNavigationService.PushModalAsync(new AddEditChildPageViewModel(true, _child)).Forget()); 
             ChangeCommand.ThrownExceptions.SubscribeAndLogException();
         }

         void SetData(IChild m)
         {
             _child = m;
             Name = _child.Name;
             Age = _child.Age > 0 ? $"{_child.Age}" : "Not available";
             AvatarImage = _child.PhotoPath.ToResizedImage(80) ?? Images.AvatarPlaceHolder;
         }
    }
}