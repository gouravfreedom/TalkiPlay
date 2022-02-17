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
    public partial class AudioPackListPage : BasePage<AudioPackListPageViewModel>, IAnimationPage
    {
        public AudioPackListPage()
        {
            InitializeComponent();
      
            AudioPackList.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
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
                    
                    this.OneWayBind(ViewModel, v => v.AudioPackItems, view => view.AudioPackList.ItemsSource).DisposeWith(d);
                   
                    this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
               
                    this.AudioPackList.Events()
                        .ItemSelected
                        .Where(m => m.SelectedItem != null)
                        .Select(m => (AudioPackItemViewModel) m.SelectedItem)
                        .Do(m => this.AudioPackList.SelectedItem = null)
                        .InvokeCommand(this, v => v.ViewModel.SelectCommand)
                        .DisposeWith(d);
                    
                 
                    AudioPackList.Events()
                        .Refreshing
                        .Select(m => Unit.Default)
                        .InvokeCommand(this, v => v.ViewModel.RefreshCommand)
                        .DisposeWith(d);

                    this.ViewModel.WhenAnyValue(m => m.IsRefreshing)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(m => { AudioPackList.IsRefreshing = m; })
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
            Subtype = AnimationSubtype.FromBottom,
            Duration = AnimationDuration.Short
        };
    }
}
