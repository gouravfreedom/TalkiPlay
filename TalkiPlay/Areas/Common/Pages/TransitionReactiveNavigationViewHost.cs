using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ChilliSource.Mobile.UI.ReactiveUI;
using FormsControls;
using FormsControls.Base;
using ReactiveUI;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Ensure = ChilliSource.Mobile.UI.ReactiveUI.Ensure;

namespace TalkiPlay
{
    public class TransitionReactiveNavigationViewHost : AnimationNavigationPage, IView
    {
        private readonly IScheduler _backgroundScheduler;
        private readonly IScheduler _mainScheduler;
        private readonly IViewLocator _viewLocator;
        private readonly IObservable<IPageViewModel> _pagePopped;
     
        public TransitionReactiveNavigationViewHost(
            IScheduler backgroundScheduler = null,
            IScheduler mainScheduler = null,
            IViewLocator viewLocator = null
        )
        {

            this.EnableInteractivePopGesture = false;
            this._backgroundScheduler = backgroundScheduler ?? RxApp.TaskpoolScheduler;
            this._mainScheduler = mainScheduler ?? RxApp.MainThreadScheduler;
            this._viewLocator = viewLocator ?? Locator.Current.GetService<IViewLocator>();


            this._pagePopped = Observable
                .FromEventPattern<NavigationEventArgs>(x => this.Popped += x, x => this.Popped -= x)
                .Select(ep => ep.EventArgs.Page.BindingContext as IPageViewModel)
                .Where(v => v != null);
        }


        public TransitionReactiveNavigationViewHost(
            Page root,
            IScheduler backgroundScheduler = null,
            IScheduler mainScheduler = null,
            IViewLocator viewLocator = null
        ) : base(root)
        {
            this.EnableInteractivePopGesture = false;
            this._backgroundScheduler = backgroundScheduler ?? RxApp.TaskpoolScheduler;
            this._mainScheduler = mainScheduler ?? RxApp.MainThreadScheduler;
            this._viewLocator = viewLocator ?? Locator.Current.GetService<IViewLocator>();
           this._pagePopped = Observable
                .FromEventPattern<NavigationEventArgs>(x => this.Popped += x, x => this.Popped -= x)
                .Select(ep => ep.EventArgs.Page.BindingContext as IPageViewModel)
                .Where(v => v != null);
        }
        
       
        public IObservable<IPageViewModel> PagePopped => this._pagePopped;

        public IObservable<Unit> PushModal(IModalViewModel modalViewModel, string contract,
           bool animate)
        {
            Ensure.ArgumentNotNull(modalViewModel, nameof(modalViewModel));

            return Observable
                .Start(
                    () =>
                    {
                        var page = this.LocatePageFor(modalViewModel, contract);

                        if (modalViewModel is IModalViewModelWithParameters vm)
                        {
                            if (page is NavigationPage navigationPage && navigationPage.RootPage is IViewFor rootPage)
                            {
                                if (rootPage.ViewModel is IModalViewModelWithParameters pvm)
                                {
                                    pvm.Parameters = vm.Parameters;
                                }
                            }
                        }
                        this.SetPageTitle(page, modalViewModel.Title);
                        return page;
                    },
                    this._backgroundScheduler)
                .ObserveOn(this._mainScheduler)
                .SelectMany(
                    page =>
                             this
                            .Navigation
                            .PushModalAsync(page, animate)
                            .ToObservable());
        }

        private static bool IsPopupPage(Page page)
        {
            return (page is PopupPage);
        }

        public IObservable<Unit> PopModal(bool animate) =>
            this
                .Navigation
                .PopModalAsync(animate)
                .ToObservable()
                .Select(_ => Unit.Default)
                    // XF completes the pop operation on a background thread :/
                .ObserveOn(this._mainScheduler);

        public IObservable<Unit> PushPopup(IPopModalViewModel viewModel, string contract, bool animate)
        {
            Ensure.ArgumentNotNull(viewModel, nameof(viewModel));

            return Observable
                    .Start(
                            () =>
                            {
                                var page = this.LocatePageFor(viewModel, contract);
                                this.SetPageTitle(page, viewModel.Title);
                                return page;
                            },
                             this._backgroundScheduler)
                    .Where(IsPopupPage)
                    .ObserveOn(this._mainScheduler)
                    .SelectMany(
                    page =>
                        this.Navigation
                            .PushPopupAsync((PopupPage) page, animate)
                            .ToObservable());

        }

        public IObservable<Unit> PopPopup(
            bool animate, bool resetStack)
        {
            IObservable<Unit> observable;
            if (resetStack)
            {
                observable = Observable.FromAsync(async() => await Navigation
                    .PopAllPopupAsync(animate));
            }
            else
            {
                observable = Observable.FromAsync(async () => await Navigation
                    .PopPopupAsync(animate));
            }
                
            return observable
                .OnErrorResumeNext(Observable.Return(Unit.Default))
                .Select(_ => Unit.Default)
                // XF completes the pop operation on a background thread :/
                .ObserveOn(this._mainScheduler);
        }

        public IObservable<Unit> PushPage(IPageViewModel pageViewModel, string contract, bool resetStack, 
            bool animate)
        {
            Ensure.ArgumentNotNull(pageViewModel, nameof(pageViewModel));

            // If we don't have a root page yet, be sure we create one and assign one immediately because otherwise we'll get an exception.
            // Otherwise, create it off the main thread to improve responsiveness and perceived performance.
            var hasRoot = this.Navigation.NavigationStack.Count > 0;
            var mainScheduler = hasRoot ? this._mainScheduler : CurrentThreadScheduler.Instance;
            var backgroundScheduler = hasRoot ? this._backgroundScheduler : CurrentThreadScheduler.Instance;

            return Observable
                .Start(
                    () =>
                    {
                        var page = this.LocatePageFor(pageViewModel, contract);
                        this.SetPageTitle(page, pageViewModel.Title);
                        return page;
                    },
                    backgroundScheduler)
                .ObserveOn(mainScheduler)
                .SelectMany(
                    page =>
                    {
                        if (resetStack)
                        {
                            if (this.Navigation.NavigationStack.Count == 0)
                            {
                                return this
                                    .Navigation
                                    .PushAsync(page, animated: animate)
                                    .ToObservable();
                            }
                            else
                            {
                            // XF does not allow us to pop to a new root page. Instead, we need to inject the new root page and then pop to it.
                            this
                                    .Navigation
                                    .InsertPageBefore(page, this.Navigation.NavigationStack[0]);

                                return this
                                    .Navigation
                                    .PopToRootAsync(animated: false)
                                    .ToObservable();
                            }
                        }
                        else
                        {
                           
                            return this
                                .Navigation
                                .PushAsync(page, animate)
                                .ToObservable();
                        }
                    });
        }

        public IObservable<Unit> PopToRootPage(bool animate) =>
            this
                .Navigation
                .PopToRootAsync(animated: animate)
                .ToObservable();

        public IObservable<Unit> PopPage(bool animate) =>
            this
                .Navigation
                .PopAsync(animate)
                .ToObservable()
                .Select(_ => Unit.Default)
                // XF completes the pop operation on a background thread :/
                .ObserveOn(this._mainScheduler);

        private Page LocatePageFor(object viewModel, string contract)
        {
            Ensure.ArgumentNotNull(viewModel, nameof(viewModel));

            var view = _viewLocator.ResolveView(viewModel, contract);
            var viewFor = view;
            var page = view as Page;

            if (view == null)
            {
                throw new InvalidOperationException($"No view could be located for type '{viewModel.GetType().FullName}', contract '{contract}'. Be sure Splat has an appropriate registration.");
            }

            if (viewFor == null)
            {
                throw new InvalidOperationException($"Resolved view '{view.GetType().FullName}' for type '{viewModel.GetType().FullName}', contract '{contract}' does not implement IViewFor.");
            }

            if (page == null)
            {
                throw new InvalidOperationException($"Resolved view '{view.GetType().FullName}' for type '{viewModel.GetType().FullName}', contract '{contract}' is not a Page.");
            }

            page.BindingContext = viewModel;
            viewFor.ViewModel = viewModel;
        
            return page;
        }

        private void SetPageTitle(Page page, string resourceKey)
        {
            var title = resourceKey;
            page.Title = title;
        }

    }
}
