
/*
 * Based on
 * Source: SimpleIoCApp (https://github.com/Clancey/SimpleIoCApp)
 * Author:  James Clancey (https://github.com/Clancey)
 * License: Apache 2.0 (https://github.com/Clancey/SimpleIoCApp/blob/master/LICENSE)
*/

using System;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using ReactiveUI;
using System.Globalization;
using System.Reactive.Threading.Tasks;

namespace TalkiPlay.Shared
{
    public static class SimpleNavigationService
    {
        private static Dictionary<Type, Page> _singletonCache = new Dictionary<Type, Page>();
        public static bool IsInProgress { get; private set; } = false;

        static Page CurrentPage
        {
            get
            {
                var mainPage = Application.Current.MainPage;
                if (mainPage is TabbedPage tabbedPage)
                {
                    mainPage = tabbedPage.CurrentPage;
                }

                return TopModalPage ?? TopPage ?? mainPage;
            }
        }

        public static Page TopPage => Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
        public static Page TopModalPage => Application.Current.MainPage.Navigation.ModalStack.LastOrDefault();

        public static bool IsInModalMode => TopModalPage != null;

        public static INavigation Navigation => CurrentPage.Navigation;

        //public static async Task PushAsync(BasePageViewModel viewModel, bool animated = true)
        //{
        //    if (_inProgress)
        //    {
        //        return;
        //    }
        //    _inProgress = true;

        //    var page = BuildPage(viewModel, wrapInNavigation: false);
        //    if (page != null)
        //    {
        //        await Navigation.PushAsync(page, animated);
        //    }

        //    _inProgress = false;
        //}

        public static async Task PushInBackgroundAsync(object viewModel, bool animated = true, bool useSingleton = false)
        {
            if (IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            await Task.Run(() =>
            {
                var page = BuildPage(viewModel, wrapInNavigation: false, useSingleton: useSingleton);
                if (page != null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PushAsync(page, animated);    
                    });
                }
            });
            
            IsInProgress = false;
        }
        
        public static async Task PushAsync(object viewModel, bool animated = true, bool useSingleton = false)
        {
            if (IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            var page = BuildPage(viewModel, wrapInNavigation: false, useSingleton: useSingleton);
            if (page != null)
            {
                await Navigation.PushAsync(page, animated);
            }

            IsInProgress = false;
        }

        public static async Task PushAsync(Page page, bool animated = true)
        {
            if (page == null || IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            await Navigation.PushAsync(page, animated);

            IsInProgress = false;
        }

        public static async Task PushModalAsync(object viewModel, bool animated = true, bool wrapInNavigation = true)
        {
            if (IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            //viewModel.IsModal = true;
            var page = BuildPage(viewModel, wrapInNavigation);
            if (page != null)
            {
                await Navigation.PushModalAsync(page, animated);
            }

            IsInProgress = false;
        }

        public static async Task PushModalAsync(Type pageType, object viewModel, bool animated = true, bool wrapInNavigation = true)
        {
            if (IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            //viewModel.IsModal = true;
            var viewModelType = viewModel.GetType();

            var constructor = pageType.GetConstructor(new[] { viewModelType });
            Page page = null;
            if (constructor == null)
            {
                page = (Page)Activator.CreateInstance(pageType);
                page.BindingContext = viewModel;

            }
            else
            {
                page = (Page)constructor.Invoke(new[] { viewModel });
            }

            if (page != null)
            {
                await Navigation.PushModalAsync(page, animated);
            }

            IsInProgress = false;
        }

        public static async Task PushModalWithNavigationAsync(object viewModel, bool animated = true)
        {
            if (IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            //viewModel.IsModal = true;
            var page = BuildPage(viewModel, true);
            if (page != null)
            {
                await Navigation.PushModalAsync(page, animated);
            }

            IsInProgress = false;
        }

        public static async Task PushChildModalPage(object viewModel, bool animated = true)
        {
            if (IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            //viewModel.IsModal = true;
            var page = BuildPage(viewModel, false);
            if (page != null)
            {
                await Navigation.PushModalAsync(page, animated);
            }

            IsInProgress = false;
        }

        public static IObservable<System.Reactive.Unit> PushModalWithNavigationAsyncEx(object viewModel, bool animated = true)
        {
            return PushModalWithNavigationAsync(viewModel, animated).ToObservable();
        }


        public static async Task PushPopupAsync(object viewModel, bool animated = true)
        {
            if (IsInProgress)
            {
                return;
            }

            IsInProgress = true;

            //viewModel.IsModal = true;
            var page = BuildPage(viewModel, false);
            if (page is PopupPage popupPage)
            {
                await Navigation.PushPopupAsync(popupPage, animated);
            }

            IsInProgress = false;
        }

        public static async Task PushPopupAsync(Page page, bool animated = true)
        {
            if (IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            if (page is PopupPage popupPage)
            {
                await Navigation.PushPopupAsync(popupPage, animated);
            }

            IsInProgress = false;
        }

        public static async Task PopAsync(bool animated = true)
        {
            if (IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            await Navigation.PopAsync(animated);

            IsInProgress = false;
        }



        public static async Task PopPopupAsync(bool animated = true)
        {
            if (IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            var popupPage = PopupNavigation.Instance.PopupStack.LastOrDefault();
            if (popupPage != null)
            {
                await PopupNavigation.Instance.PopAsync(animated);
            }

            IsInProgress = false;
        }


        public static bool IsGoingBack { get; private set; } = false;

        public static async Task PopModalAsync(bool animated = true, int count = 1)
        {
            if (IsInProgress)
            {
                return;
            }
            IsInProgress = true;
            IsGoingBack = true;

            /*var popupPage = PopupNavigation.Instance.PopupStack.LastOrDefault();
            if (popupPage != null)
            {
                await PopupNavigation.Instance.PopAsync(animated);
            }                      
            else
            {*/
            while (count > 0 && Navigation.ModalStack.Count > 0)
            {
                await Navigation.PopModalAsync(animated);
                count--;
            }

            IsGoingBack = false;
            IsInProgress = false;
        }

        public static async Task PopToRootAsync(bool animated = true)
        {
            if (IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            await Navigation.PopToRootAsync(animated);

            IsInProgress = false;
        }

        public static async Task PopModalRootAsync(bool animated = true)
        {
            await PopModalAsync(animated, TopModalPage.Navigation.ModalStack.Count);
        }

        public static async Task Dismiss(bool animated = true)
        {
            if (IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            var topModalPage = Application.Current.MainPage.Navigation.ModalStack.LastOrDefault();
            if (topModalPage != null)
            {
                await Navigation.PopModalAsync(animated);
            }
            else
            {
                await Navigation.PopAsync(animated);
            }

            IsInProgress = false;
        }

        public static void SetRoot(object viewModel, bool wrapInNavigation = true)
        {
            Application.Current.MainPage = BuildPage(viewModel, wrapInNavigation);
        }

        static Type GetPageTypeForViewModel(Type viewModelType)
        {
            var viewName = viewModelType.FullName.Replace("ViewModel", string.Empty).Replace(".Shared", "");
            var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName.Replace(".Shared", "");
            var viewAssemblyName = string.Format(
                        CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            var viewType = Type.GetType(viewAssemblyName);
            return viewType;
        }

        static Page BuildPage(object viewModel, bool wrapInNavigation = true, bool useSingleton = false, bool isReactive = false)
        {
            var viewModelType = viewModel?.GetType();
            if (viewModelType == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            Page page = null;

            if (useSingleton)
            {
                if (_singletonCache.ContainsKey(viewModelType))
                {
                    page = _singletonCache[viewModelType];
                }
            }

            if (page == null)
            {
                var pageType = GetPageTypeForViewModel(viewModelType);
                
                if (pageType == null)
                {
                    return null;
                }

                var constructor = pageType.GetConstructor(new[] { viewModelType });
                if (constructor == null)
                {
                    page = (Page)Activator.CreateInstance(pageType);
                    page.BindingContext = viewModel;
                    
                }
                else
                {
                    page = (Page)constructor.Invoke(new[] { viewModel });
                }

                if (page == null)
                {
                    return null;
                }
            }

            if (useSingleton)
            {
                if (!_singletonCache.ContainsKey(viewModelType))
                {
                    _singletonCache.Add(viewModelType, page);
                }
                page.BindingContext = viewModel;
            }

            return wrapInNavigation ? new NavigationPage(page) : page;
        }
    }
}

