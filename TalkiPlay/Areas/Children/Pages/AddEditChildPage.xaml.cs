using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using FormsControls.Base;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace TalkiPlay
{
    public partial class AddEditChildPage :  BasePage<AddEditChildPageViewModel>, IAnimationPage
    {
     
        public AddEditChildPage()
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
            
            // NavigationPage.SetHasNavigationBar(this, false);
            // NavigationPage.SetHasBackButton(this, false);

            this.DateOfBirth.MaximumDate = DateTime.Today;
            this.DateOfBirth.MinimumDate = DateTime.MinValue;
            this.DateOfBirth.Date = DateTime.Today;
            
            //this.FirstNameEntry.Effects.Add(new BorderlessEffect());
            //this.DateOfBirth.Effects.Add(new BorderlessEffect());
            
            this.FirstNameEntry.ReturnCommand = new Command(() => { this.DateOfBirth.Focus(); });
            //this.LastNameEntry.ReturnCommand = new Command(() => { this.DateOfBirth.Focus(); });

            SetupForm();
            
            this.WhenActivated(d =>
                { 
              
                    this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);

                    
                    this.OneWayBind(ViewModel, v => v.ButtonText, view => view.NextButton.Text).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.NextCommand, view => view.NextButton.Button).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.IsBusy, view => view.NextButton.IsBusy).DisposeWith(d);

                    this.OneWayBind(ViewModel, v => v.FirstName.Errors, view => view.FirstNameErrorView.ItemsSource).DisposeWith(d);
                    this.Bind(ViewModel, v => v.FirstName.Value, view => view.FirstNameEntry.Text).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.FirstName.IsValid, view => view.FirstNameEntry.IsValid).DisposeWith(d);

                    //this.OneWayBind(ViewModel, v => v.LastName.Errors, view => view.LastNameErrorView.ItemsSource).DisposeWith(d);
                    //this.Bind(ViewModel, v => v.LastName.Value, view => view.LastNameEntry.Text).DisposeWith(d);
                    //this.OneWayBind(ViewModel, v => v.LastName.IsValid, view => view.LastNameEntry.IsValid).DisposeWith(d);

                    this.OneWayBind(ViewModel, v => v.DateOfBirth.Errors, view => view.DateOfBirthErrorView.ItemsSource).DisposeWith(d);
                    this.Bind(ViewModel, v => v.DateOfBirth.Value, view => view.DateOfBirth.Date).DisposeWith(d);

                    this.ViewModel.FirstName.WhenAnyValue(m => m.IsValid)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(isValid => { FirstNameEntryFrame.BorderColor = !isValid ? Colors.Red : GetFrameBorderColor(); })
                        .DisposeWith(d);
                
                    //this.ViewModel.LastName.WhenAnyValue(m => m.IsValid)
                    //    .ObserveOn(RxApp.MainThreadScheduler)
                    //    .Subscribe(isValid => { LastNameEntryFrame.BorderColor = !isValid ? Colors.Red : Colors.WhiteColor; })
                    //    .DisposeWith(d);
                
                    this.ViewModel.DateOfBirth.WhenAnyValue(m => m.IsValid)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(isValid =>
                        {
                           DateOfBirthEntryFrame.BorderColor = !isValid ? Colors.Red : GetFrameBorderColor();
                        })
                        .DisposeWith(d);
                });
        }

        Color GetFrameBorderColor()
        {
            return Colors.EntryBorderColor;
            // return Device.RuntimePlatform == Device.Android
            //     ? Colors.EntryBorderColor
            //     : Colors.EntryBorderColor;
        }
        
        private void SetupForm()
        {
            var height = Dimensions.GetFormTextFieldHeight();
            var padding = Dimensions.GetFormTextFieldPadding();
            FirstNameEntryFrame.HeightRequest = height;
            FirstNameEntryFrame.Padding = padding;

            if (Device.RuntimePlatform == Device.Android)
            {
                //DateOfBirth.Margin = new Thickness(2,2,2,0);
                //DateOfBirthEntryFrame.HeightRequest = 40;
                DateOfBirthEntryFrame.Padding = 0;
                DateOfBirth.Margin = new Thickness(10,7,10,7);
            }
            else
            {
                DateOfBirthEntryFrame.HeightRequest = height;
                DateOfBirthEntryFrame.Padding = padding;
                //DateOfBirth.BackgroundColor = Color.White;
                DateOfBirth.Margin = new Thickness(10,0,0,0);;
            }
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
