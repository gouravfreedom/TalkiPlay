// using System;
// using System.Linq;
// using System.Reactive.Linq;
// using ChilliSource.Mobile.Core;
// using ChilliSource.Mobile.UI;
// using Humanizer;
// using ReactiveUI;
// using Splat;
// using TalkiPlay.Shared;
// using Unit = System.Reactive.Unit;
//
// namespace TalkiPlay
// {

// public interface ITabPageModel : IPageViewModel, IModalViewModel
// {
//         
// }

//     public class TabItemsModel : ITabItemsModel, ITabPageModel, ITabService
//     {
//         readonly ITabView _view;
//
//         public TabItemsModel(ITabView view)
//         {
//             _view = view;
//             Tabs = new ObservableRangeCollection<ITabItem>();
//             SetupTabs();
//         }
//
//         public ObservableRangeCollection<ITabItem> Tabs { get; }
//
//         public string Title => "";
//         public IObservable<Unit> ChangeTab(TabItemType tabItemType) => _view.ChangeTab(tabItemType);
//
//         void SetupTabs()
//         {
//             var userSettings = Locator.Current.GetService<IUserSettings>();
//
//             if (userSettings.HasTalkiPlayerDevice)
//             {
//                 Tabs.Add(new TabItem
//                 {
//                     Icon = Images.GameTabIcon,
//                     IsEnabled = true,
//                     PageFactory = () => Bootstrapper.GetTabItemPage(() => new RoomListPage(),
//                         navigator => new RoomListPageViewModel(navigator),
//                         TabItemType.Games.Humanize(),
//                         Images.GameTabIcon,
//                         TabItemType.Games.ToString()),
//                     SelectedIcon = Images.GameTabSelectedIcon,
//                     Type = TabItemType.Games,
//                     Title = TabItemType.Games.Humanize()
//                 });
//             }
//             else
//             {
//                 Tabs.Add(new TabItem
//                 {
//                     Icon = Images.GameTabIcon,
//                     IsEnabled = true,
//                     PageFactory = () => Bootstrapper.GetTabItemPage(() => new GameListPage(),
//                         navigator => new GameListPageViewModel(navigator, false),
//                         TabItemType.Games.Humanize(),
//                         Images.GameTabIcon,
//                         TabItemType.Games.ToString()),
//                     SelectedIcon = Images.GameTabSelectedIcon,
//                     Type = TabItemType.Games,
//                     Title = TabItemType.Games.Humanize()
//                 });
//             }
//
//             Tabs.Add(new TabItem
//             {
//                 Icon = Images.KidsTabIcon,
//                 IsEnabled = true,
//                 PageFactory = () => Bootstrapper.GetTabItemPage(() => new ChildListPage(),
//                 navigator => new ChildrenListPageViewModel(navigator),
//                     TabItemType.Children.Humanize(), 
//                     Images.KidsTabIcon,
//                     TabItemType.Children.ToString()),
//                 SelectedIcon = Images.KidsTabSelectedIcon,
//                 Title = TabItemType.Children.Humanize(),
//                 Type = TabItemType.Children
//             });
//
//             if (!userSettings.HasTalkiPlayerDevice)
//             {
//                 Tabs.Add(new TabItem
//                 {
//                     Icon = Images.RewardsTabIcon,
//                     IsEnabled = true,
//                     PageFactory = () => Bootstrapper.GetTabItemPage(() => new RewardsChildListPage(),
//                         navigator => new RewardsChildListPageViewModel(navigator),
//                         TabItemType.Rewards.Humanize(),
//                         Images.ItemsTabIcon,
//                         TabItemType.Rewards.ToString()),
//                     SelectedIcon = Images.RewardsTabSelectedIcon,
//                     Title = TabItemType.Rewards.Humanize(),
//                     Type = TabItemType.Rewards
//                 });
//             }
//
//             if (userSettings.HasTalkiPlayerDevice)
//             {
//                 Tabs.Add(new TabItem
//                 {
//                     Icon = Images.ItemsTabIcon,
//                     IsEnabled = true,
//                     PageFactory = () => Bootstrapper.GetTabItemPage(() => new RoomListPage(),
//                     navigator => new ItemsRoomListPageViewModel(navigator),
//                         TabItemType.Items.Humanize(),
//                         Images.ItemsTabIcon,
//                         TabItemType.Items.ToString()),
//                     SelectedIcon = Images.ItemsTabSelectedIcon,
//                     Title = TabItemType.Items.Humanize(),
//                     Type = TabItemType.Items
//                 });
//             }
//             else
//             {
//                 Tabs.Add(new TabItem
//                 {
//                     Icon = Images.ItemsTabIcon,
//                     IsEnabled = true,
//                     PageFactory = () => Bootstrapper.GetTabItemPage(() => new QRCodePdfListPage(),
//                         navigator => new QRCodePdfListPageViewModel(navigator),
//                         TabItemType.Items.Humanize(),
//                         Images.ItemsTabIcon,
//                         TabItemType.Items.ToString()),
//                     SelectedIcon = Images.ItemsTabSelectedIcon,
//                     Title = TabItemType.Items.Humanize(),
//                     Type = TabItemType.Items
//                 });
//                 
//               
//             }
//             
//             Tabs.Add(new TabItem
//             {
//                 Icon = Images.SettingsTabIcon,
//                 IsEnabled = true,
//                 PageFactory = () => Bootstrapper.GetTabItemPage(() => new SettingsPage(),
//                 navigator => new SettingsPageViewModel(navigator), 
//                     TabItemType.Settings.Humanize(),
//                     Images.SettingsTabIcon,
//                     TabItemType.Settings.ToString()),
//                 SelectedIcon = Images.SettingsTabSelectedIcon,
//                 Title = TabItemType.Settings.Humanize(),
//                 Type = TabItemType.Settings
//             });
//            
//         }
//
//        public IObservable<Unit> ShowBadge(string badge, TabItemType tabItemType)
//         {
//             return Observable.Start(() =>
//             {
//                 if (String.IsNullOrWhiteSpace(badge)) return;
//                 var t = Tabs.FirstOrDefault(m => (m as TabItem).Type == tabItemType);
//                 if (t != null)
//                 {
//                     t.BadgeCount = badge;
//                 }
//             }, RxApp.MainThreadScheduler);
//         }
//     }
//
//   
// }