using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using FormsControls.Base;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class AddEditRoomPage :  BasePage<AddEditRoomPageViewModel>, IAnimationPage
    {
     
        public AddEditRoomPage()
        {
            InitializeComponent();
                
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
              
                    this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);

                    
                    this.OneWayBind(ViewModel, v => v.ButtonText, view => view.NextButton.Text).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.NextCommand, view => view.NextButton).DisposeWith(d);
                    //this.OneWayBind(ViewModel, v => v.IsBusy, view => view.NextButton.IsBusy).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.Name.Errors, view => view.NameErrorView.ItemsSource).DisposeWith(d);
                    this.Bind(ViewModel, v => v.Name.Value, view => view.NameEntry.Text).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.Name.IsValid, view => view.NameEntry.IsValid).DisposeWith(d);
                    
                    this.ViewModel.Name.WhenAnyValue(m => m.IsValid)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(isValid =>
                            {
                                NameEntryFrame.BorderColor = !isValid ? Colors.Red : Colors.WhiteColor;
                            })
                        .DisposeWith(d);                                                     

                });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
            safeInsets.Top = 0;
            Padding = safeInsets;
        }


        public void OnAnimationStarted(bool isPopAnimation)
        {
          
        }

        public void OnAnimationFinished(bool isPopAnimation)
        {
            
        }

        public IPageAnimation PageAnimation  => this.ViewModel?.PageAnimation ?? new SlidePageAnimation()
        {
            BounceEffect = false,
            Subtype = AnimationSubtype.FromRight,
            Duration = AnimationDuration.Short
        };
   
      
    }
}
