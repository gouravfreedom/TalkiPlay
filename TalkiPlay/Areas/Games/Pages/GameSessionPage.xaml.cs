using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using EasyLayout.Forms;
using FormsControls.Base;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class GameSessionPage : BasePage<GameSessionPageViewModel>, IAnimationPage
    {
        public GameSessionPage()
        {
            InitializeComponent();
            this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

          
            var service = Locator.Current.GetService<IApplicationService>();
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);
                                  
            this.WhenActivated(d =>
            {
                this.WhenAnyValue(m => m.ViewModel.LoadDataCommand)
                    .Select(m => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.LoadDataCommand)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, v => v.BackImageSource, view => view.BackgroundImageSource).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.GameSessions, view => view.GamesSessionList.ItemsSource).DisposeWith(d);

                this.GamesSessionList.Events()
                    .SelectionChanged
                    .Select(m => m.CurrentSelection.FirstOrDefault())
                    .Where(m => m != null)
                    .Select(m => m as GameSessionViewModel)
                    .InvokeCommand(this, v => v.ViewModel.SelectCommand)
                    .DisposeWith(d);
                    
            });
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            // {
            //     this.ViewModel?.StartScan();
            // }
            //
            var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
            safeInsets.Top = 0;
            Padding = safeInsets;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
          //  this.ViewModel?.StopScan();
        }


        public void OnAnimationStarted(bool isPopAnimation)
        {
           
        }

        public void OnAnimationFinished(bool isPopAnimation)
        {
            
        }

        public IPageAnimation PageAnimation => this.ViewModel?.PageAnimation ??  new SlidePageAnimation()
        {
            BounceEffect = false,
            Subtype = AnimationSubtype.FromRight,
            Duration = AnimationDuration.Short
        };
    }
}
