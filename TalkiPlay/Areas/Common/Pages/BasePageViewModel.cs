﻿using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using FormsControls.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace TalkiPlay.Shared
{
    public interface IBasePageViewModel : IPageViewModel
    {
        ReactiveCommand<Unit,Unit> BackCommand { get; }
        bool IsLoading { get;  }
        
        IPageAnimation PageAnimation { get; }
    }
    
    public class NavigationItemViewModel<T> : IModalViewModelWithParameters where T : class, IModalViewModel
    {
        public NavigationItemViewModel()
        {
            
        }


        public NavigationItemViewModel(object parameters)
        {
            Parameters = parameters;
        }
        
        public object Parameters { get; set; }

        public string Title { get; } = "";
    }
    
    public abstract class BasePageViewModel : ReactiveObject, IBasePageViewModel
    {
        
        protected BasePageViewModel()
        {
            BackCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (Navigator != null)
                {
                    await Navigator?.PopPage();
                }
                else
                {
                    await SimpleNavigationService.PopAsync();
                }
            });
            BackCommand.ThrownExceptions.SubscribeAndLogException();
        }
        
        public INavigationService Navigator { get; protected set; }
        public abstract string Title { get; }
        
        public ReactiveCommand<Unit,Unit> BackCommand { get; set; }
        
        public extern bool IsLoading { [ObservableAsProperty] get;}

        public virtual IPageAnimation PageAnimation { get; set; }

        public bool IsModal { get; set; }
    }

    public abstract class BasePageViewModelEx : BasePageViewModel, IActivatableViewModel
    {
        private readonly IConnectivityNotifier _connectivityNotifier;
        public ViewModelActivator Activator { get; }

        protected BasePageViewModelEx()
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