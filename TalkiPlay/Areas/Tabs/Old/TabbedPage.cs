// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reactive;
// using System.Reactive.Linq;
// using ChilliSource.Mobile.UI;
// using ReactiveUI;
// using TalkiPlay.Shared;
// using Xamarin.Forms;
//
// namespace TalkiPlay
// {    
//     public class BaseTabbedPage : ExtendedTabbedPage, ITabController, ICanActivate
//     {
//         public event EventHandler ViewWillAppear;
//         public event EventHandler ViewWillDisappear;
//
//         public BaseTabbedPage()
//         {
//         }
//
//         public virtual void OnWillAppear()
//         {
//             ViewWillAppear?.Invoke(this, new EventArgs());
//         }
//
//         public virtual void OnWillDisappear()
//         {
//             ViewWillDisappear?.Invoke(this, new EventArgs());
//         }
//
//         public bool HideTabItemText { get; set; }
//
//         public IObservable<Unit> Activated => Observable
//             .FromEventPattern<EventHandler, EventArgs>(x => ViewWillAppear += x, x => ViewWillAppear -= x)
//             .Select(_ => Unit.Default);
//
//         public IObservable<Unit> Deactivated => Observable
//             .FromEventPattern<EventHandler, EventArgs>(x => ViewWillDisappear += x, x => ViewWillDisappear -= x)
//             .Select(_ => Unit.Default);
//
//         
//     }
//     
//     public class ReactiveExtendedTabbedPage<TViewModel> : BaseTabbedPage,
//         IViewFor<TViewModel> where TViewModel : class
//     {
//        /// <summary>
//         /// The ViewModel to display
//         /// </summary>
//         public TViewModel ViewModel
//         {
//             get => (TViewModel)GetValue(ViewModelProperty);
//             set => SetValue(ViewModelProperty, value);
//         }
//         
//         public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
//             nameof(ViewModel),
//             typeof(TViewModel),
//             typeof(ReactiveExtendedTabbedPage<TViewModel>),
//             default(TViewModel),
//             BindingMode.OneWay,
//             propertyChanged: OnViewModelChanged);
//
//         object IViewFor.ViewModel {
//             get => ViewModel;
//             set => ViewModel = (TViewModel)value;
//         }
//
//         protected override void OnBindingContextChanged()
//         {
//             base.OnBindingContextChanged();
//             ViewModel = BindingContext as TViewModel;
//         }
//
//         private static void OnViewModelChanged(BindableObject bindableObject, object oldValue, object newValue)
//         {
//             bindableObject.BindingContext = newValue;
//         }
//     }
//
//     public class TabsPage : ReactiveExtendedTabbedPage<ITabItemsModel>, ITabView
//     {
//         private Dictionary<TabItemType, Page> _pages = new Dictionary<TabItemType, Page>();
//
//         public TabsPage()
//         {
//             NavigationPage.SetHasNavigationBar(this, false);
//             this.IsOpaque = true;
//             this.SelectedTabItemTintColor = Colors.Yellow;
//             this.UnselectedTabColor = Colors.GreenyBlue;
//         }
//
//         protected override void OnBindingContextChanged()
//         {
//             base.OnBindingContextChanged();
//
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
//             TabItems = ViewModel.Tabs;
//             this.HideTabItemText = false;
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
//     }
// }