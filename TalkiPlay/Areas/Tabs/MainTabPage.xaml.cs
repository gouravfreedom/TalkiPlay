using System;
using System.Collections.Generic;
using ChilliSource.Mobile.UI.ReactiveUI;
using Humanizer;
using Sharpnado.Tabs;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace TalkiPlay
{
    public partial class MainTabPage : BasePage<MainTabPageViewModel>
    {        
        public MainTabPage()
        {
            InitializeComponent();            

            tabSwitcher.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == ViewSwitcher.SelectedIndexProperty.PropertyName)
                {
                    NotifyViewActivated();
                }
            };

            BuildTabs();

            this.WhenActivated(d => {
                //this.OneWayBind(ViewModel, v => v.IsConnected, view => view.tpDevice.IsVisible).DisposeWith(d);
                //this.OneWayBind(ViewModel, v => v.ShowPairMePopup, view => view.spechBubblePairMe.IsVisible).DisposeWith(d);
                NotifyViewActivated();
            });
        }

        private void BuildTabs()
        {
            var navigator = Locator.Current.GetService<INavigationService>(Constants.MainNavigation);

            var userSettings = Locator.Current.GetService<IUserSettings>();

            if (userSettings.HasTalkiPlayerDevice)
            {
                CreateTab(new CategoryListPage() { BindingContext = new CategoryListPageViewModel(navigator) }, TabItemType.Games, Images.GameTabIcon);
            }
            else
            {
                CreateTab(new GameListPage() { BindingContext = new GameListPageViewModel(navigator, false) }, TabItemType.Games, Images.GameTabIcon);
            }


            CreateTab(new PacksRewardPage() { BindingContext = new PacksRewardPageViewModel() }, TabItemType.Rewards, Images.RewardsTabIcon);


            CreateTab(new SettingsPage() { BindingContext = new SettingsPageViewModel(navigator) }, TabItemType.Settings, Images.SettingsTabIcon);
            tabSwitcher.SelectedIndex = 0;

        }

        private void CreateTab(View view, TabItemType tab, string icon)
        {
            this.tabSwitcher.Children.Add(view);
            this.tabHost.Tabs.Add(new BottomTabItem()
            {
                Padding = new Thickness(8),
                FontFamily = Fonts.FontFamilyRoundedBold,
                //LabelSize = 16,
                Label = tab.Humanize(),
                IconImageSource = ImageSource.FromFile(icon),
                SelectedTabColor = Colors.TabBarSelectedColor,
                UnselectedIconColor = Colors.TabBarUnselectedColor,
                UnselectedLabelColor = Colors.TabBarUnselectedColor
            });
        }

        private void NotifyViewActivated()
        {
            if (tabSwitcher.Children[tabSwitcher.SelectedIndex] is CategoryListPage)
            {
                (tabSwitcher.Children[tabSwitcher.SelectedIndex] as CategoryListPage).AboutToAppear();
            }
            else if (tabSwitcher.Children[tabSwitcher.SelectedIndex] is GameListPage)
            {
                (tabSwitcher.Children[tabSwitcher.SelectedIndex] as GameListPage).AboutToAppear();
            }
            else if (tabSwitcher.Children[tabSwitcher.SelectedIndex] is PacksRewardPage)
            {
                (tabSwitcher.Children[tabSwitcher.SelectedIndex] as PacksRewardPage).AboutToAppear();
            }
            else if (tabSwitcher.Children[tabSwitcher.SelectedIndex] is SettingsPage)
            {
                (tabSwitcher.Children[tabSwitcher.SelectedIndex] as SettingsPage).AboutToAppear();
            }
        }

        private void OnWelcomeAnimClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as DeviceConnectionViewModel;
            vm?.TogglePairMePopup();
        }

        private void OnTappedToPair(object sender, EventArgs e)
        {
            SimpleNavigationService.PushModalAsync(new DeviceSetupPageViewModel(DeviceSetupSource.Connect)).Forget();
        }
    }


    public class MainTabPageViewModel : DeviceConnectionViewModel
    {        
        public override string Title => "Main Page";
        public MainTabPageViewModel()
        {

        }
    }

    public abstract class DeviceConnectionViewModel : BasePageViewModelEx
    {        
        protected ITalkiPlayerManager _talkiPlayerManager;
        public extern bool IsConnected { [ObservableAsProperty] get; }
        [Reactive]
        public bool ShowPairMePopup { get; private set; } = false;

        public DeviceConnectionViewModel()
        {
            _talkiPlayerManager = Locator.Current.GetService<ITalkiPlayerManager>();
        }

        protected override void OnVmAcivated(CompositeDisposable d)
        {
            if (Locator.Current.GetService<IUserSettings>().HasTalkiPlayerDevice)
            {
                this.WhenAnyValue(m => m.IsConnected)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeSafe(isConnected =>
                {
                    if (isConnected)
                    {
                        ShowPairMePopup = false;
                    }
                })
                .DisposeWith(d);


                _talkiPlayerManager.Current?.WhenAnyValue(p => p.IsConnected)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToPropertyEx(this, v => v.IsConnected)
                    .DisposeWith(d);
            }
            else
            {
                var forceTrue = Observable.Return(true)
                    .Select(v => v)
                    .ToPropertyEx(this, v => v.IsConnected)
                    .DisposeWith(d);
            }
        }

        public void TogglePairMePopup()
        {
            ShowPairMePopup = !ShowPairMePopup;
        }
    }
}
