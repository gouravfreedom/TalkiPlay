using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.Core;
using ChilliSource.Mobile.UI;
using ChilliSource.Mobile.UI.ReactiveUI;
using Humanizer;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;
using FormsControls.Base;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;
using Page = Xamarin.Forms.Page;
using Unit = System.Reactive.Unit;

namespace TalkiPlay
{
    public partial class TagItemsCollectionHelpPage : BasePage<TagItemHelpPageViewModel>,  IAnimationPage
    {
        public TagItemsCollectionHelpPage()
        {
            InitializeComponent();

            this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

            var service = Locator.Current.GetService<IApplicationService>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS ? (int) service.StatusbarHeight : 0;
                var navHeight = (int) service.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = totalHeight;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);

            });
            
            
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);

       
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, v => v.NextCommand, view => view.ContinueButton.Button).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
                this.WhenAnyValue(m => m.ViewModel.CanClose)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Where(a => a)
                    .SubscribeSafe(async a => { await Navigation.PopModalAsync(); })
                    .DisposeWith(d);
            });
            
        }

        public void OnAnimationStarted(bool isPopAnimation)
        {
           
        }

        public void OnAnimationFinished(bool isPopAnimation)
        {
            
        }

        public IPageAnimation PageAnimation => this.ViewModel?.PageAnimation ?? new SlidePageAnimation()
        {
            BounceEffect = false,
            Subtype = AnimationSubtype.FromRight,
            Duration = AnimationDuration.Short
        };
    }
    
    public class TagItemHelpPageViewModel : BasePageViewModel, IActivatableViewModel
    {
        private readonly IPageViewModel _viewModel;

        public TagItemHelpPageViewModel(IPageViewModel viewModel)
        {
            Activator = new ViewModelActivator();
            _viewModel = viewModel;

            NextCommand = ReactiveCommand.Create( () =>
            {
                CanClose = true;

                if (_viewModel is ITagItemSetupPageViewModel vm)
                {
                   vm.CloseCommand.Execute().SubscribeSafe();
                }
            });
        }
        
        public override string Title => "Collection";
        public ViewModelActivator Activator { get; }
        
        public ReactiveCommand<Unit,Unit> NextCommand { get; set; }
        
        [Reactive] public bool CanClose { get; set; } 
        
    }
    
    // public static class TagItemCollectionPageHelper
    // {
    //     public static Page GetTabbedPage(IPageViewModel parentViewModel)
    //     {
    //         Page startPage;
    //         if (Device.RuntimePlatform == Device.Android)
    //         {
    //             startPage = new TagItemCollectionHelpBottomTabbedPage();
    //         }
    //         else
    //         {
    //             startPage = new TagItemCollectionHelpTabbedPage();
    //         }
    //         
    //         var mainPage = new TransitionReactiveNavigationViewHost(startPage);
    //         var tabs = new TagItemsHelpTabItemsModel((ITabView) startPage, parentViewModel);
    //         var viewModel = tabs;
    //         ((IViewFor) startPage).ViewModel = viewModel;
    //         startPage.BindingContext = viewModel;
    //         startPage.Title = viewModel.Title;
    //         return mainPage;
    //     }
    // }
    //
    // public class TagItemCollectionHelpTabbedPage : TabsPage 
    // {
    //     
    //     public TagItemCollectionHelpTabbedPage()
    //     {
    //
    //         this.WhenActivated(d =>
    //         {
    //             ChangeTab(TabItemType.Items);
    //
    //             Observable.FromEventPattern<EventHandler, EventArgs>(h => this.CurrentPageChanged += h,
    //                     h => this.CurrentPageChanged -= h)
    //                 .ObserveOn(RxApp.MainThreadScheduler)
    //                 .Where(m => this.CurrentPage is TransitionReactiveNavigationViewHost page && page.CurrentPage is EmptyPage)
    //                 .Do(m =>
    //                 {
    //                     ChangeTab(TabItemType.Items);
    //
    //                 })
    //                 .SubscribeSafe(_ => { })
    //                 .DisposeWith(d);
    //         });
    //     }
    //     
    // }
    //
    // public class TagItemCollectionHelpBottomTabbedPage : BottomTabsPage
    // {
    //     public TagItemCollectionHelpBottomTabbedPage() 
    //     {
    //         this.WhenActivated(d =>
    //         {
    //             ChangeTab(TabItemType.Items);
    //
    //             Observable.FromEventPattern<EventHandler, EventArgs>(h => this.CurrentPageChanged += h,
    //                     h => this.CurrentPageChanged -= h)
    //                 .ObserveOn(RxApp.MainThreadScheduler)
    //                 .Where(m => this.CurrentPage is TransitionReactiveNavigationViewHost page && page.CurrentPage is EmptyPage)
    //                 .Do(m =>
    //                 {
    //                     ChangeTab(TabItemType.Items);
    //
    //                 })
    //                 .SubscribeSafe(_ => { })
    //                 .DisposeWith(d);
    //         });
    //     }
    //     
    // }
    //
    // public class EmptyPageViewModel : BasePageViewModel
    // {
    //     public override string Title => "";
    //     
    // }
    //
    // public class EmptyPage : BasePage<EmptyPageViewModel>
    // {
    //     
    //     
    // }
    //
    //
    // public class TagItemsHelpTabItemsModel : ITabItemsModel
    // {
    //     private readonly ITabView _view;
    //
    //     public TagItemsHelpTabItemsModel(ITabView view, IPageViewModel parentModel)
    //     {
    //         _view = view;
    //         Tabs = new ObservableRangeCollection<ITabItem>();
    //         SetupTabs(parentModel);
    //     }
    //
    //     private void SetupTabs(IPageViewModel parentModel)
    //     {
    //       Tabs.Add(new TabItem
    //       {
    //           Icon = Images.GameTabIcon,
    //           IsEnabled = true,
    //             PageFactory = () => Bootstrapper.GetTabItemPage(() => new EmptyPage(), 
    //                 navigator => new EmptyPageViewModel(), 
    //                 TabItemType.Games.Humanize(), 
    //                 Images.GameTabIcon,
    //                 TabItemType.Games.ToString()),
    //           SelectedIcon = Images.GameTabSelectedIcon,
    //           Type = TabItemType.Games,
    //           Title = TabItemType.Games.Humanize()
    //       });
    //       Tabs.Add(new TabItem
    //       {
    //           Icon = Images.KidsTabIcon,
    //           IsEnabled = true,
    //           PageFactory = () => Bootstrapper.GetTabItemPage(() => new EmptyPage(), navigator => new EmptyPageViewModel(),
    //               TabItemType.Children.Humanize(), 
    //               Images.KidsTabIcon,
    //               TabItemType.Children.ToString()),
    //           SelectedIcon = Images.KidsTabSelectedIcon,
    //           Title = TabItemType.Children.Humanize(),
    //           Type = TabItemType.Children
    //       });
    //         Tabs.Add(new TabItem
    //         {
    //             Icon = Images.ItemsTabIcon,
    //             IsEnabled = true,
    //             PageFactory = () => Bootstrapper.GetTabItemPage(() => new TagItemsCollectionHelpPage(), navigator => new TagItemHelpPageViewModel(parentModel),
    //                 TabItemType.Items.Humanize(), 
    //                 Images.ItemsTabIcon,
    //                 TabItemType.Items.ToString()),
    //             SelectedIcon = Images.ItemsTabSelectedIcon,
    //             Title = TabItemType.Items.Humanize(),
    //             Type = TabItemType.Items
    //         });
    //         Tabs.Add(new TabItem
    //         {
    //             Icon = Images.SettingsTabIcon,
    //             IsEnabled = true,
    //             PageFactory = () => Bootstrapper.GetTabItemPage(() => new EmptyPage(), navigator => new EmptyPageViewModel(), 
    //                 TabItemType.Settings.Humanize(),
    //                 Images.SettingsTabIcon,
    //                 TabItemType.Settings.ToString()),
    //             SelectedIcon = Images.SettingsTabSelectedIcon,
    //             Title = TabItemType.Settings.Humanize(),
    //             Type = TabItemType.Settings
    //         });
    //        
    //     }
    //
    //     public ObservableRangeCollection<ITabItem> Tabs { get; }
    //     public string Title => "";
    //     
    // }

}
