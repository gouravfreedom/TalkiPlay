using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Unit = System.Reactive.Unit;

namespace TalkiPlay.Shared
{
    public class ChildListPageViewModel : SimpleBasePageModel, IActivatableViewModel
    {        
        readonly IChildrenRepository _childrenService;
        readonly IConnectivityNotifier _connectivityNotifier;
        private bool _hasLoadedFirstTime;
        private readonly IGameMediator _gameMediator;
        private readonly bool _isSelectionMode;
        
        public ChildListPageViewModel(bool isSelectionMode, IGameMediator gameMediator = null)
        {
            _isSelectionMode = isSelectionMode;
            _childrenService = Locator.Current.GetService<IChildrenRepository>();
            _connectivityNotifier = Locator.Current.GetService<IConnectivityNotifier>();
            Activator = new ViewModelActivator();

            ShowLeftMenuItem = true;
            Children = new ObservableDynamicDataRangeCollection<ChildViewModel>();

            if (isSelectionMode)
            {
                _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
            }
            
            SetupRx();
            SetupCommands();
        }

        public string Title => "Children";

        public bool ShowEmptyState { get; set; }
        public ViewModelActivator Activator { get; }

        public ObservableDynamicDataRangeCollection<ChildViewModel> Children { get;  }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }

        public ReactiveCommand<ChildViewModel, Unit> SelectCommand { get; protected set; }

        public ReactiveCommand<Unit, Unit> AddCommand { get; protected set; }
        
        public ReactiveCommand<Unit, Unit> BackCommand { get; protected set; }

        [Reactive]
        public bool ShowLeftMenuItem { get; set; } = false;

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
                if (!_hasLoadedFirstTime)
                {
                    Dialogs.ShowLoading();
                }

                var children = await _childrenService.GetChildren();

                Dialogs.HideLoading();

                Children.Clear();
                using (Children.SuspendNotifications())
                {
                    Children.AddRange(children.Select(c => new ChildViewModel(c)));
                }

                ShowEmptyState = Children.Count == 0;
                _hasLoadedFirstTime = true;
            });
            
            LoadDataCommand.ThrownExceptions
                 .ObserveOn(RxApp.MainThreadScheduler)
                 //.HideLoading()
                 .ShowExceptionDialog()
                 .SubscribeAndLogException();

            SelectCommand = ReactiveCommand.Create<ChildViewModel, Unit>(m =>
            {
                if (_isSelectionMode)
                {
                    _gameMediator.Children.Edit(item =>
                    {
                        var c = item.FirstOrDefault(a => a.Id == m.Child.Id);
                        if (c == null)
                        {
                            item.Insert(item.Count - 1, m.Child);
                        }
                    });
                    Locator.Current.GetService<IUserSettings>().CurrentChild = m.Child;
                    SimpleNavigationService.PopModalAsync().Forget();
                }
                else
                {
                    SimpleNavigationService
                        .PushAsync(new ChildDetailsPageViewModel(m.Child)).Forget();    
                }
                return Unit.Default;
            });
            
             SelectCommand.ThrownExceptions.SubscribeAndLogException();

             BackCommand = ReactiveCommand.Create(() =>
             {
                 if (_isSelectionMode)
                 {
                     SimpleNavigationService.PopModalAsync().Forget();
                 }
                 else
                 {
                     SimpleNavigationService.PopAsync().Forget();    
                 }
             });
             
             BackCommand.ThrownExceptions.SubscribeAndLogException();
             
             AddCommand = ReactiveCommand.Create(()=> SimpleNavigationService.PushModalAsync(new AddEditChildPageViewModel(true)).Forget());
             AddCommand.ThrownExceptions.SubscribeAndLogException();

         }       
    }    
}