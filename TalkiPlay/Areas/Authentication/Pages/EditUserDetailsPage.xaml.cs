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
    public partial class EditUserDetailsPage : BasePage<EditUserDetailsPageViewModel>, IAnimationPage
    {
        public EditUserDetailsPage()
        {
            InitializeComponent();
         
            GradientView.GradientSource = Styles.BuildMainGradientSource();
          
            //this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

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
            
            this.EmailEntry.ReturnCommand = new Command(() => { this.FirstNameEntry.Focus(); });
            this.FirstNameEntry.ReturnCommand = new Command(() => { this.LastNameEntry.Focus(); });

            this.EmailEntry.Effects.Add(new BorderlessEffect());
            this.FirstNameEntry.Effects.Add(new BorderlessEffect());
            this.LastNameEntry.Effects.Add(new BorderlessEffect());
       

            SetupForm();

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, v => v.UpdateDetailsCommand, view => view.UpdateButton.Button).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Email.Errors, view => view.EmailErrorView.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.FirstName.Errors, view => view.FirstNameErrorView.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.LastName.Errors, view => view.LastNameErrorView.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, v => v.Email.Value, view => view.EmailEntry.Text).DisposeWith(d);
                this.Bind(ViewModel, v => v.FirstName.Value, view => view.FirstNameEntry.Text).DisposeWith(d);
                this.Bind(ViewModel, v => v.LastName.Value, view => view.LastNameEntry.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.Email.IsValid, view => view.EmailEntry.IsValid).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.FirstName.IsValid, view => view.FirstNameEntry.IsValid).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.LastName.IsValid, view => view.LastNameEntry.IsValid).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.IsBusy, view => view.UpdateButton.IsBusy).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.UpdateDetailsCommand, v => v.LastNameEntry.ReturnCommand)
                    .DisposeWith(d);
              
                this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);

                
                this.ViewModel.Email.WhenAnyValue(m => m.IsValid)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isValid => { EmailEntryFrame.BorderColor = !isValid ? Colors.Red : Colors.WhiteColor; })
                    .DisposeWith(d);
                  
                this.ViewModel.FirstName.WhenAnyValue(m => m.IsValid)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isValid =>
                    {
                        FirstNameEntryFrame.BorderColor = !isValid ? Colors.Red : Colors.WhiteColor;
                    })
                    .DisposeWith(d); 
                
                this.ViewModel.LastName.WhenAnyValue(m => m.IsValid)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isValid =>
                    {
                        LastNameEntryFrame.BorderColor = !isValid ? Colors.Red : Colors.WhiteColor;
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
            
            EmailEntryFrame.HeightRequest = height;
            EmailEntryFrame.Padding = padding;
            FirstNameEntryFrame.HeightRequest = height;
            FirstNameEntryFrame.Padding = padding;
            LastNameEntryFrame.HeightRequest = height;
            LastNameEntryFrame.Padding = padding;
        }
    }
}
