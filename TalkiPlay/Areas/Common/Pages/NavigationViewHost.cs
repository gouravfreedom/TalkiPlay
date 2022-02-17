using System.Reactive.Concurrency;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class NavigationViewHost<T> : TransitionReactiveNavigationViewHost, IViewFor<T> where T : class, IModalViewModel
    {
        public NavigationViewHost(
            IScheduler backgroundScheduler = null,
            IScheduler mainScheduler = null,
            IViewLocator viewLocator = null
        ) : base(backgroundScheduler, mainScheduler, viewLocator)
        {
            
        }

        public NavigationViewHost(
            Page root,
            IScheduler backgroundScheduler = null,
            IScheduler mainScheduler = null,
            IViewLocator viewLocator = null
        ) : base(root, backgroundScheduler, mainScheduler, viewLocator)
        {
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (T) value;
        }

        public T ViewModel { get; set; }
    }

}