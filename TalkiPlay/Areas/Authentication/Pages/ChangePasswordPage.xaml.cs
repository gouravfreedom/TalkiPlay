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
    public partial class ChangePasswordPage : BasePage<ChangePasswordPageViewModel>, IAnimationPage
    {
     
        public ChangePasswordPage()
        {
            InitializeComponent();
            //this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            
            GradientView.GradientSource = Styles.BuildMainGradientSource();

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
            
            this.PasswordEntry.Effects.Add(new BorderlessEffect());
            this.CurrentPasswordEntry.Effects.Add(new BorderlessEffect());
            this.ConfirmPasswordEntry.Effects.Add(new BorderlessEffect());
       

            this.PasswordEntry.ReturnCommand = new Command(() => { this.ConfirmPasswordEntry.Focus(); });
            SetupForm();
            
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, v => v.ChangePasswordCommand, view => view.ResetPasswordButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.CurrentPassword.Errors, view => view.CurrentPasswordErrorView.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Password.Errors, view => view.PasswordErrorView.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.ConfirmPassword.Errors, view => view.ConfirmPasswordErrorView.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, v => v.CurrentPassword.Value, view => view.CurrentPasswordEntry.Text).DisposeWith(d);
                this.Bind(ViewModel, v => v.Password.Value, view => view.PasswordEntry.Text).DisposeWith(d);
                this.Bind(ViewModel, v => v.ConfirmPassword.Value, view => view.ConfirmPasswordEntry.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.CurrentPassword.IsValid, view => view.CurrentPasswordEntry.IsValid).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Password.IsValid, view => view.PasswordEntry.IsValid).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.ConfirmPassword.IsValid, view => view.ConfirmPasswordEntry.IsValid).DisposeWith(d);
                //this.OneWayBind(ViewModel, v => v.IsBusy, view => view.ResetPasswordButton.IsBusy).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.ChangePasswordCommand, v => v.ConfirmPasswordEntry.ReturnCommand)
                    .DisposeWith(d);
                
                this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);

                this.ViewModel.CurrentPassword.WhenAnyValue(m => m.IsValid)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isValid =>
                    {
                        CurrentPasswordEntryFrame.BorderColor = !isValid ? Colors.Red : Colors.WhiteColor;
                    })
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

        public void OnAnimationStarted(bool isPopAnimation)
        {
        }

        public void OnAnimationFinished(bool isPopAnimation)
        {
        }

        public IPageAnimation PageAnimation  => new SlidePageAnimation()
        {
            BounceEffect = false,
            Subtype = AnimationSubtype.FromRight,
            Duration = AnimationDuration.Short
        };
        
        private void SetupForm()
        {
            var height = Dimensions.GetFormTextFieldHeight();
            var padding = Dimensions.GetFormTextFieldPadding();
            
            CurrentPasswordEntryFrame.HeightRequest = height;
            CurrentPasswordEntryFrame.Padding = padding;
            PasswordEntryFrame.HeightRequest = height;
            PasswordEntryFrame.Padding = padding;
            ConfirmPasswordEntryFrame.HeightRequest = height;
            ConfirmPasswordEntryFrame.Padding = padding;
        }
    }
}
