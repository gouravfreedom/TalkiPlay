// using System;
// using System.Collections.Generic;
// using System.Reactive;
// using ChilliSource.Mobile.Core;
// using ChilliSource.Mobile.UI;
// using TalkiPlay.Shared;
// using Xamarin.Forms;
// using Unit = System.Reactive.Unit;
//
// namespace TalkiPlay
// {
//     public interface ITab : ITabItem
//     {
//         Func<Page> PageFactory { get; }
//         TabItemType Type { get; }
//     }
//
//     public interface ITabView
//     {
//         IObservable<Unit> ChangeTab(TabItemType tabItemType);
//     }
//
//     public interface ITabItemsModel
//     {
//         ObservableRangeCollection<ITabItem> Tabs { get; }
//     }
//
//     public interface ITabController
//     {
//         void OnWillAppear();
//         void OnWillDisappear();
//         bool HideTabItemText { get; }
//         IList<ITabItem> TabItems { get; }
//     }
// }
//
