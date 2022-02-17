// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reactive;
// using System.Reactive.Linq;
// using ChilliSource.Mobile.UI;
// using Naxam.Controls.Forms;
// using ReactiveUI;
// using TalkiPlay.Shared;
// using Xamarin.Forms;
//
// namespace TalkiPlay
// {
//
//     public class BottomTabsPage : BottomTabbedPage, IViewFor<ITabItemsModel>, ITabView
//     {
//         private Dictionary<TabItemType, Page> _pages = new Dictionary<TabItemType, Page>();
//         
//         public BottomTabsPage()
//         {
//             NavigationPage.SetHasNavigationBar(this, false);
//         }
//
//
//
//         object IViewFor.ViewModel
//         {
//             get => ViewModel;
//             set => ViewModel = (ITabItemsModel) value;
//         }
//
//         public ITabItemsModel ViewModel { get; set; }
//
//         protected override void OnBindingContextChanged()
//         {
//             base.OnBindingContextChanged();
//
//             foreach (var item in ViewModel.Tabs)
//             {
//                 if (item is ITab tab)
//                 {
//                     var page = tab.PageFactory();
//                     _pages.Add(tab.Type, page);
//                     Children.Add(page);
//                 }
//             }
//           
//         }
//
//         public IObservable<Unit> ChangeTab(TabItemType tabItemType)
//         {
//             return Observable.Start(() =>
//                 {
//                     var tab = ViewModel.Tabs.FirstOrDefault(m => ((ITab) m).Type == tabItemType);
//                     if (tab == null) return;
//                     var page = _pages[((ITab) tab).Type];
//                     CurrentPage = page;
//                 }, RxApp.MainThreadScheduler)
//                 .Select(m => Unit.Default);
//         }
//
//         protected override bool OnBackButtonPressed()
//         {
//    
//             return true;
//         }
//     }
// }