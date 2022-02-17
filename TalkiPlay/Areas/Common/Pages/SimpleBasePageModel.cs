using System;
using System.Reactive.Disposables;
using ChilliSource.Mobile.UI;
using ReactiveUI;
using Splat;

namespace TalkiPlay.Shared
{
    public class SimpleBasePageModel : BaseViewModel
    {
        public SimpleBasePageModel()
        {
        }

        public bool IsModal { get; set; }
    }

    public abstract class SimpleBasePageModelEx : SimpleBasePageModel, IActivatableViewModel
    {
        private readonly IConnectivityNotifier _connectivityNotifier;
        public ViewModelActivator Activator { get; }

        protected SimpleBasePageModelEx()
        {
            this.Activator = new ViewModelActivator();
            _connectivityNotifier = Locator.Current.GetService<IConnectivityNotifier>();
            this.WhenActivated(d =>
            {
                _connectivityNotifier.Notifier.RegisterHandler(async context =>
                {
                    await SimpleNavigationService.PushPopupAsync(new ConnectivityPageViewModel());
                    context.SetOutput(true);
                }).DisposeWith(d);

                OnVmAcivated(d);
            });
        }

        protected virtual void OnVmAcivated(CompositeDisposable d)
        {

        }
    }
}
