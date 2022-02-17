using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using Splat;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class RewardsChildListPageViewModel: SimpleBasePageModel// BasePageViewModel, IActivatableViewModel
    {
        readonly IChildrenRepository _childrenService;
        readonly IConnectivityNotifier _connectivityNotifier;
        private bool _hasLoadedFirstTime;
        
        public RewardsChildListPageViewModel()
        {
            _childrenService = Locator.Current.GetService<IChildrenRepository>();
            _connectivityNotifier = Locator.Current.GetService<IConnectivityNotifier>();
            Children = new ObservableDynamicDataRangeCollection<RewardChildViewModel>();

            SetupRx();
            SetupCommands();

            MessageBus.Current.Listen<SubscriptionChangedMessage>()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe((x) => LoadDataCommand.Execute(Unit.Default));
            
        }
        
        public string Title => "Rewards";

        public int NumberOfColumns => Device.Idiom == TargetIdiom.Tablet ? 3 : 1;
        
        public ObservableDynamicDataRangeCollection<RewardChildViewModel> Children { get; private set; }
        
        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }
        
        
        void SetupRx()
        {
            _connectivityNotifier.Notifier.RegisterHandler(async context =>
            {
                await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                context.SetOutput(true);
            });//.DisposeWith(d);
            
            // this.WhenActivated(d =>
            // {
            //     _connectivityNotifier.Notifier.RegisterHandler(async context =>
            //     {
            //         await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
            //         context.SetOutput(true);
            //     }).DisposeWith(d);
            // });
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
                var hasSubscription = await SubscriptionService.GetUserHasSubscription();
                using (Children.SuspendNotifications())
                {
                    Children.AddRange(children.Select(c => new RewardChildViewModel(c, !hasSubscription,(child) => HandleSelection(child).Forget())));
                }

                _hasLoadedFirstTime = true;
            });
            
            LoadDataCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .ShowExceptionDialog()
                .SubscribeAndLogException();
        }

        async Task HandleSelection(IChild child)
        {
           
            if (await SubscriptionService.GetUserHasSubscription())
            {
                await SimpleNavigationService.PushAsync(new RewardListPageViewModel( child));
            }
            else
            {
                await SimpleNavigationService.PushModalAsync(new SubscriptionListPageViewModel());
            }
        }
    }
}