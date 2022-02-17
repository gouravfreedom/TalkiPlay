using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using FormsControls.Base;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using static System.Reactive.Unit;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class ChildDetailsPage :  BasePage<ChildDetailsPageViewModel>, IAnimationPage
    {
     
        public ChildDetailsPage()
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
            
            
            // NavigationPage.SetHasNavigationBar(this, false);
            // NavigationPage.SetHasBackButton(this, false);

            
      
            this.WhenActivated(d =>
                { 
                    this.WhenAnyValue(m => m.ViewModel.LoadDataCommand)
                        .Select(m => Default)
                        .InvokeCommand(this, v => v.ViewModel.LoadDataCommand)
                        .DisposeWith(d);
                    
                    this.BindCommand(ViewModel, v => v.ChangeCommand, view => view.ChangeButton.Button).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.Name, view => view.Name.Text);
                    this.OneWayBind(ViewModel, v => v.Age, view => view.Age.Text);
                    
                    this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.BackCommand, view => view.CancelButton.Button).DisposeWith(d);



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
