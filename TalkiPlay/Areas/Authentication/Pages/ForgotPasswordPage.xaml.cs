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
    public partial class ForgotPasswordPage  : BasePage<ForgotPasswordPageViewModel>, IAnimationPage
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();

            GradientView.GradientSource = Styles.BuildMainGradientSource();


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

            
            this.EmailEntry.Effects.Add(new BorderlessEffect());
        

            SetupForm();
            
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, v => v.RequestForgotPasswordTokenCommand, view => view.ResetPasswordButton.Button).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Email.Errors, view => view.EmailErrorView.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, v => v.Email.Value, view => view.EmailEntry.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Email.IsValid, view => view.EmailEntry.IsValid).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.IsBusy, view => view.ResetPasswordButton.IsBusy).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.RequestForgotPasswordTokenCommand, v => v.EmailEntry.ReturnCommand).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.SuccessMessage, view => view.Notice1.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.IsRequestSent, view => view.RequestSuccessNotice.IsVisible).DisposeWith(d);
                //this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
                
                this.ViewModel.Email.WhenAnyValue(m => m.IsValid)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isValid => { EmailEntryFrame.BorderColor = !isValid ? Colors.Red : Colors.WhiteColor; })
                    .DisposeWith(d);
                
                
                this.ViewModel.WhenAnyValue(m => m.IsRequestSent)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(m =>
                    {
                        EmailField.IsVisible = !m;
                        ResetPasswordButton.IsVisible = !m;
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

        public IPageAnimation PageAnimation => new SlidePageAnimation
        {
            BounceEffect = false,
            Subtype = AnimationSubtype.FromRight,
            Duration = AnimationDuration.Short
        };
        
        private void SetupForm()
        {
            var height = Dimensions.GetFormTextFieldHeight();
            var padding = Dimensions.GetFormTextFieldPadding();
            
            EmailEntryFrame.HeightRequest = height;
            EmailEntryFrame.Padding = padding;
        }
    }
}
