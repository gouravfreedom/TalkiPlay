using System;
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
    public partial class SignupPage : BasePage<SignupPageViewModel>, IAnimationPage
    {        
        public SignupPage()
        {
            InitializeComponent();
            bool isDeviceMode = Locator.Current.GetService<IUserSettings>().HasTalkiPlayerDevice;
            lblCompanyType.Text = isDeviceMode ? "Company name" : "Household name";
            HouseholdEntry.Placeholder = isDeviceMode ? "Company name..." : "Household name...";

            GradientView.GradientSource = Styles.BuildMainGradientSource();

            //this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

            var service = Locator.Current.GetService<IApplicationService>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = Device.RuntimePlatform == Device.iOS ? (int) service.StatusbarHeight : 0;
                var navHeight = (int) service.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = totalHeight;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);
            });
            
            
            var loginGestureRecognizer = new TapGestureRecognizer();
            LoginLayout.GestureRecognizers.Add(loginGestureRecognizer);
            loginGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, nameof(SignupPageViewModel.LoginCommand));
            
            this.HouseholdEntry.ReturnCommand = new Command(() => { this.EmailEntry.Focus(); });
            this.EmailEntry.ReturnCommand = new Command(() => { this.PasswordEntry.Focus(); });            
            this.PasswordEntry.ReturnCommand = new Command(() => { this.ConfirmPasswordEntry.Focus(); });

            
            this.HouseholdEntry.Effects.Add(new BorderlessEffect());
            this.EmailEntry.Effects.Add(new BorderlessEffect());
            this.PasswordEntry.Effects.Add(new BorderlessEffect());
            this.ConfirmPasswordEntry.Effects.Add(new BorderlessEffect());
            
            SetupForm();
            
            
            this.WhenActivated(d =>
            {
                Dialogs.HideLoading();
                
                this.BindCommand(ViewModel, v => v.SignupCommand, view => view.SignupButton).DisposeWith(d);
              
            
                this.OneWayBind(ViewModel, v => v.Household.Errors, view => view.HouseholdErrorView.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Email.Errors, view => view.EmailErrorView.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Password.Errors, view => view.PasswordErrorView.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.HasAgreedToTnc.Errors, view => view.TncErrorView.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.ConfirmPassword.Errors, view => view.ConfirmPasswordErrorView.ItemsSource).DisposeWith(d);
                
                this.Bind(ViewModel, v => v.Household.Value, view => view.HouseholdEntry.Text).DisposeWith(d);
                this.Bind(ViewModel, v => v.Email.Value, view => view.EmailEntry.Text).DisposeWith(d);
                this.Bind(ViewModel, v => v.Password.Value, view => view.PasswordEntry.Text).DisposeWith(d);                
                this.Bind(ViewModel, v => v.ConfirmPassword.Value, view => view.ConfirmPasswordEntry.Text).DisposeWith(d);
                this.Bind(ViewModel, v => v.HasAgreedToTnc.Value, view => view.TncView.HasAgreed).DisposeWith(d);
            
                this.OneWayBind(ViewModel, v => v.Household.IsValid, view => view.HouseholdEntry.IsValid).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Email.IsValid, view => view.EmailEntry.IsValid).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Password.IsValid, view => view.PasswordEntry.IsValid).DisposeWith(d);                
                this.OneWayBind(ViewModel, v => v.ConfirmPassword.IsValid, view => view.ConfirmPasswordEntry.IsValid).DisposeWith(d);                
            
                //this.OneWayBind(ViewModel, v => v.IsBusy, view => view.SignupButton.IsBusy).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.SignupCommand, v => v.ConfirmPasswordEntry.ReturnCommand)
                    .DisposeWith(d);
                      
                this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
            
                this.ViewModel.Household.WhenAnyValue(m => m.IsValid)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isValid => { HouseholdEntryFrame.BorderColor = !isValid ? Colors.Red : Colors.WhiteColor; })
                    .DisposeWith(d);
            
                this.ViewModel.Email.WhenAnyValue(m => m.IsValid)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isValid => { EmailEntryFrame.BorderColor = !isValid ? Colors.Red : Colors.WhiteColor; })
                    .DisposeWith(d);
                
                
                this.ViewModel.Password.WhenAnyValue(m => m.IsValid)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isValid =>
                    {
                        PasswordEntryFrame.BorderColor = !isValid ? Colors.Red : Colors.WhiteColor;
                    })
                    .DisposeWith(d); 
                                                
                this.ViewModel.ConfirmPassword.WhenAnyValue(m => m.IsValid)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isValid =>
                    {
                        ConfirmPasswordEntryFrame.BorderColor = !isValid ? Colors.Red : Colors.WhiteColor;
                    })
                    .DisposeWith(d); 
                              
            });
        }

        void SetupForm()
        {
            var height = Dimensions.GetFormTextFieldHeight();
            var padding = Dimensions.GetFormTextFieldPadding();

            HouseholdEntryFrame.HeightRequest = height;
            HouseholdEntryFrame.Padding = padding;

            EmailEntryFrame.HeightRequest = height;
            EmailEntryFrame.Padding = padding;

            PasswordEntryFrame.HeightRequest = height;
            PasswordEntryFrame.Padding = padding;
            ConfirmPasswordEntryFrame.HeightRequest = height;
            ConfirmPasswordEntryFrame.Padding = padding;
        }

        public void OnAnimationStarted(bool isPopAnimation)
        {

        }

        public void OnAnimationFinished(bool isPopAnimation)
        {

        }

        public IPageAnimation PageAnimation => new SlidePageAnimation()
        {
            BounceEffect = false,
            Subtype = AnimationSubtype.FromRight,
            Duration = AnimationDuration.Short
        };

       
    }
}
