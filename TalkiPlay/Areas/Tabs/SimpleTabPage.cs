using System;
using ChilliSource.Mobile.UI.ReactiveUI;
using Humanizer;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace TalkiPlay
{
    public enum TabType
    {
        Games = 0,
        Children,
        Rewards,
        Collection,
        Settings
    }

    public class SimpleTabPage : Xamarin.Forms.TabbedPage
    {
        private TabType _previousTab;
        
        public SimpleTabPage()
        {

            On<Xamarin.Forms.PlatformConfiguration.Android>()
                .SetToolbarPlacement(ToolbarPlacement.Bottom);
                      //.SetBarItemColor(ThemeManager.TabBarColor)
                      //.SetBarSelectedItemColor(ThemeManager.TabBarColor);

                      
            this.UnselectedTabColor = Colors.GreenyBlue;
            this.SelectedTabColor = Colors.TabBarSelectedColor;
            
            // SelectedTabColor = ThemeManager.TabBarColor;
            // UnselectedTabColor = ThemeManager.TabBarColor;
            //
            // BarBackgroundColor = ThemeManager.TabBarBackgroundColor;
            // BarTextColor = ThemeManager.TabBarColor;
            
            CurrentPageChanged += OnTabChanged;

           

            //MessageBus.Subscribe(Notifications.SwitchToTabRequested, this);
            
            BuildTabs();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Device.RuntimePlatform == Device.Android && CurrentPage == null)
            {
                CurrentTabChanged?.Invoke(TabType.Games);
            }
        }
        
        // public void ReceiveMessage(Enum messageType, object parameter)
        // {
        //     switch(messageType)
        //     {
        //         case (Notifications.SwitchToTabRequested):
        //             {
        //                 var tabType = (TabType)parameter;
        //                 SetTab(tabType);
        //                 break;
        //             }                
        //     }
        // }
                             
        public Action<TabType> CurrentTabChanged;

        public void SetTab(TabType tab)
        {
            Device.BeginInvokeOnMainThread(() =>
            {               
                CurrentPage = Children[(int)tab];
            });
        }

        
        void OnTabChanged(object sender, EventArgs e)
        {
            var index = Children.IndexOf(CurrentPage);
            if (index >= 0)
            {
                var newTab = (TabType)index;
                CurrentTabChanged?.Invoke(newTab);
                              
                _previousTab = newTab;
            }
        }
        
        // void BuildTabs()
        // {       
        //     var navigator = Locator.Current.GetService<INavigationService>(Constants.MainNavigation);
        //     //var navigator = SetupNavigationService(mainPage, Constants.MainNavigation);
        //     
        //     var userSettings = Locator.Current.GetService<IUserSettings>();
        //
        //     if (userSettings.HasTalkiPlayerDevice)
        //     {
        //         var gamesPage = new NavigationPage(new RoomListPage())
        //         {
        //             Title = TabType.Games.Humanize(),
        //             IconImageSource = Images.GameTabIcon,
        //             BindingContext = new RoomListPageViewModel(navigator)
        //         };
        //         Children.Add(gamesPage);
        //     }
        //     else
        //     {
        //         var gamesPage = new NavigationPage(new GameListPage())
        //         {
        //             Title = TabType.Games.Humanize(),
        //             IconImageSource = Images.GameTabIcon,
        //             BindingContext = new GameListPageViewModel(navigator, false)
        //         };
        //         Children.Add(gamesPage);
        //     }
        //     
        //     var childrenPage = new NavigationPage(new ChildListPage()
        //     {
        //         Title = TabType.Children.Humanize(),
        //         IconImageSource = Images.KidsTabIcon,
        //         BindingContext = new ChildrenListPageViewModel(navigator)
        //     });
        //
        //     Children.Add(childrenPage);
        //
        //     var rewardsPage = new NavigationPage(new RewardsChildListPage())
        //     {
        //         Title = TabType.Rewards.Humanize(),
        //         IconImageSource = Images.RewardsTabIcon,
        //         BindingContext =  new RewardsChildListPageViewModel(navigator)
        //     };
        //     Children.Add(rewardsPage);
        //
        //     if (userSettings.HasTalkiPlayerDevice)
        //     {
        //         var collectionPage = new NavigationPage(new RoomListPage())
        //         {
        //             Title = TabType.Collection.Humanize(),
        //             IconImageSource = Images.ItemsTabIcon,
        //             BindingContext = new ItemsRoomListPageViewModel(navigator)
        //         };
        //         Children.Add(collectionPage);    
        //     }
        //     else
        //     {
        //         var collectionPage = new NavigationPage(new QRCodePdfListPage())
        //         {
        //             Title = TabType.Collection.Humanize(),
        //             IconImageSource = Images.ItemsTabIcon,
        //             BindingContext =  new QRCodePdfListPageViewModel(navigator)
        //         };
        //         Children.Add(collectionPage);
        //     }
        //
        //     var settingsPage = new NavigationPage(new SettingsPage())
        //     {
        //         Title = TabType.Settings.Humanize(),
        //         IconImageSource = Images.SettingsTabIcon,
        //         BindingContext =  new SettingsPageViewModel(navigator)
        //     };
        //     Children.Add(settingsPage);
        //     
        // }

        void BuildTabs()
        {       
            var navigator = Locator.Current.GetService<INavigationService>(Constants.MainNavigation);
            //var navigator = SetupNavigationService(mainPage, Constants.MainNavigation);
            
            var userSettings = Locator.Current.GetService<IUserSettings>();

            if (userSettings.HasTalkiPlayerDevice)
            {
                //var gamesPage = Bootstrapper.GetTabItemPage(() => new CategoryListPage(),
                //    navigator => new CategoryListPageViewModel(navigator),
                //    TabItemType.Games.Humanize(),
                //    Images.GameTabIcon,
                //    TabItemType.Games.ToString());
                //Children.Add(gamesPage);
            }
            else
            {
                var gamesPage = Bootstrapper.GetTabItemPage(() => new GameListPage(),
                    navigator => new GameListPageViewModel(navigator, false),
                    TabItemType.Games.Humanize(),
                    Images.GameTabIcon,
                    TabItemType.Games.ToString());
                Children.Add(gamesPage);
            }
            
            var childrenVM = new ChildListPageViewModel(false);
            var childrenPage = new ChildListPage()
            {
                BindingContext = childrenVM,
            };
            var childrenNavPage = new NavigationPage(childrenPage)
            {
                Title = TabType.Children.Humanize(),
                IconImageSource = Images.KidsTabIcon,
            };
            Children.Add(childrenNavPage);
            
            // var childrenPage = Bootstrapper.GetTabItemPage(() => new ChildListPage(),
            //     navigator => new ChildListPageViewModel(false),
            //     TabItemType.Children.Humanize(),
            //     Images.KidsTabIcon,
            //     TabItemType.Children.ToString());
            //
            // Children.Add(childrenPage);

            //var rewardsVM = new RewardsChildListPageViewModel();
            //var rewardsPage = new RewardsChildListPage()
            //{
            //    BindingContext = rewardsVM,
            //    //ViewModel = rewardsVM
            //};
            //var rewardsNavPage = new NavigationPage(rewardsPage)
            //{
            //    Title = TabType.Rewards.Humanize(),
            //    IconImageSource = Images.RewardsTabIcon,
            //};
            //Children.Add(rewardsNavPage);
            
            // var rewardsPage = Bootstrapper.GetTabItemPage(() => new RewardsChildListPage(),
            //     navigator => new RewardsChildListPageViewModel(navigator),
            //     TabItemType.Rewards.Humanize(),
            //     Images.RewardsTabIcon,
            //     TabItemType.Rewards.ToString());
            // Children.Add(rewardsPage);

            if (userSettings.HasTalkiPlayerDevice)
            {
                var collectionPage = Bootstrapper.GetTabItemPage(() => new RoomListPage(),
                    navigator => new ItemsRoomListPageViewModel(navigator),
                    TabItemType.Items.Humanize(),
                    Images.ItemsTabIcon,
                    TabItemType.Items.ToString());
                Children.Add(collectionPage);    
            }
            else
            {
                //var vm = new QRCodePdfListPageViewModel();
                //var page = new QRCodePdfListPage()
                //{
                //    BindingContext = vm,
                //    //ViewModel = vm
                //};
                //var collectionPage = new NavigationPage(page)
                //{
                //    Title = TabType.Collection.Humanize(),
                //    IconImageSource = Images.ItemsTabIcon,
                //};
                //Children.Add(collectionPage);
            }

            //var settingsPage = new NavigationPage(new SettingsPage())
            //{
            //    Title = TabType.Settings.Humanize(),
            //    IconImageSource = Images.SettingsTabIcon,
            //    BindingContext =  new SettingsPageViewModel(null)
            //};
            //Children.Add(settingsPage);
            
            // var settingsPage = Bootstrapper.GetTabItemPage(() => new SettingsPage(),
            //     navigator => new SettingsPageViewModel(navigator), 
            //     TabItemType.Settings.Humanize(),
            //     Images.SettingsTabIcon,
            //     TabItemType.Settings.ToString());
            // Children.Add(settingsPage);
            
        }
        
    }
}
