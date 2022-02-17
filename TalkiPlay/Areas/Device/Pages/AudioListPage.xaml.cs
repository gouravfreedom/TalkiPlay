using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using FormsControls.Base;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class AudioListPage : BasePage<AudioListPageViewModel>, IAnimationPage
    {
        public AudioListPage()
        {
            InitializeComponent();
      
            AudioList.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
            this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

            var service = Locator.Current.GetService<IApplicationService>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = Device.RuntimePlatform == Device.iOS ? (int) service.StatusbarHeight : 0;
                var navHeight = (int) service.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = totalHeight;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);
         
            });
            
            
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);
        
            this.WhenActivated(d =>
                {
                    this.WhenAnyValue(m => m.ViewModel.LoadCommand)
                        .Select(_ => Unit.Default)
                        .InvokeCommand(this, v => v.ViewModel.LoadCommand)
                        .DisposeWith(d);
                    
                    this.OneWayBind(ViewModel, v => v.AudioItems, view => view.AudioList.ItemsSource).DisposeWith(d);
                   
                    this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.UpdateAllCommand, view => view.UpdateAllButton.Button).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.ShowUpdateAllButton, view => view.UpdateAllButton.IsVisible).DisposeWith(d);

                    this.AudioList.Events()
                        .ItemSelected
                        .Where(m => m.SelectedItem != null)
                        .Select(m => (AudioItemViewModel) m.SelectedItem)
                        .Do(m => this.AudioList.SelectedItem = null)
                        .InvokeCommand(this, v => v.ViewModel.SelectCommand)
                        .DisposeWith(d);
                    
                 
                    AudioList.Events()
                        .Refreshing
                        .Select(m => Unit.Default)
                        .InvokeCommand(this, v => v.ViewModel.RefreshCommand)
                        .DisposeWith(d);

                    this.ViewModel.WhenAnyValue(m => m.IsRefreshing)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(m => { AudioList.IsRefreshing = m; })
                        .DisposeWith(d);

                    var bottomOffset = (int)service.GetSafeAreaInsets().Bottom;

                    this.WhenAnyValue(m => m.ViewModel.ShowUpdateAllButton)
                        .Subscribe(a => { ButtonRow.Height = a ? 60 + bottomOffset : 0; })
                        .DisposeWith(d);

                    
                    UpdateAllButton.ContentPadding = bottomOffset > 0 ? new Thickness(0, 0, 0, 15) : new Thickness();
                    UpdateAllButton.ButtonHeightRequest = 60 + service.GetSafeAreaInsets().Bottom;

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
            Subtype = AnimationSubtype.FromBottom,
            Duration = AnimationDuration.Short
        };
    }
}
