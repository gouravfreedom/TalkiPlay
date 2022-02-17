using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using FormsControls.Base;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Splat;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;


namespace TalkiPlay
{
    public partial class LoginPage  : BasePage<LoginPageViewModel>, IAnimationPage
    {        
        public LoginPage()
        {
            InitializeComponent();
            GradientView.GradientSource = Styles.BuildMainGradientSource();


            this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

            var service = Locator.Current.GetService<IApplicationService>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = Device.RuntimePlatform == Device.iOS ? (int)service.StatusbarHeight : 0;
                var navHeight = (int)service.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                NavRow.Height = totalHeight;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);

            });

            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);

            var forgotpasswordGestureRecognizer = new TapGestureRecognizer();            
            ForgotPassword.GestureRecognizers.Add(forgotpasswordGestureRecognizer);
            forgotpasswordGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, nameof(LoginPageViewModel.ForgotPasswordCommand));

            var signupGestureRecognizer = new TapGestureRecognizer();
            SignupLayout.GestureRecognizers.Add(signupGestureRecognizer);
            signupGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, nameof(LoginPageViewModel.SignupCommand));

            this.EmailEntry.ReturnCommand = new Command(() => { this.PasswordEntry.Focus(); });

            
            this.EmailEntry.Effects.Add(new BorderlessEffect());
            this.PasswordEntry.Effects.Add(new BorderlessEffect());
        

            SetupForm();
            
            this.WhenActivated(d =>
            {
                CleaupTabPage();
                this.BindCommand(ViewModel, v => v.LoginCommand, view => view.LoginButton.Button).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Email.Errors, view => view.EmailErrorView.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Password.Errors, view => view.PasswordErrorView.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, v => v.Email.Value, view => view.EmailEntry.Text).DisposeWith(d);
                this.Bind(ViewModel, v => v.Password.Value, view => view.PasswordEntry.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Email.IsValid, view => view.EmailEntry.IsValid).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Password.IsValid, view => view.PasswordEntry.IsValid).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.IsBusy, view => view.LoginButton.IsBusy).DisposeWith(d);
                //this.BindCommand(ViewModel, v => v.ForgotPasswordCommand, view => view._forgotpasswordGestureRecognizer.Command).DisposeWith(d);
                //this.BindCommand(ViewModel, v => v.SignupCommand, view => view._signupGestureRecognizer.Command).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.LoginCommand, v => v.PasswordEntry.ReturnCommand)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
           
                
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
            });
        }

        private void CleaupTabPage()
        {
            // if (Device.RuntimePlatform == Device.Android)
            // {
            //     if (Navigation.NavigationStack.Count > 0)
            //     {
            //         var p = Navigation.NavigationStack[0];
            //
            //         if (p is BottomTabsPage page)
            //         {
            //             page.Children.Clear();
            //             page.CurrentPage = null;
            //
            //             try
            //             {
            //                 Navigation.RemovePage(p);
            //             }
            //             catch (Exception)
            //             {
            //             }
            //         }
            //     }
            // }
        }

        void SetupForm()
        {
            var height = Dimensions.GetFormTextFieldHeight();
            var padding = Dimensions.GetFormTextFieldPadding();

            EmailEntryFrame.HeightRequest = height;
            EmailEntryFrame.Padding = padding;
            PasswordEntryFrame.HeightRequest = height;
            PasswordEntryFrame.Padding = padding;
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
