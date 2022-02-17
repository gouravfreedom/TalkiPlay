using System;
using System.Reactive;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class TalkiPlayNavigationHelper : ITalkiPlayNavigator {

        public void NavigateToLoginPage()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var tabPage = App.Current.MainPage;
                    App.Current.MainPage = Bootstrapper.GetLoginPage();

                    if (tabPage is TabbedPage page)
                    {
                        page.Children?.Clear();
                        page.CurrentPage = null;
                    }
                }
                else
                {

                    SimpleNavigationService.PushModalAsync(new LoginPageViewModel(LoginNavigationSource.Default)).Forget();
                    //var nav = Locator.Current.GetService<INavigationService>(Constants.MainNavigation);
                    //nav.PushPage(new LoginPageViewModel(nav, LoginNavigationSource.Default)).SubscribeSafe();
                }

            });
        }

        public void NavigateToTabbedPage(TabItemType defaultTab = TabItemType.Games)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                App.Current.MainPage = Bootstrapper.GetTabbedPage(defaultTab);
            });
        }
    }
}