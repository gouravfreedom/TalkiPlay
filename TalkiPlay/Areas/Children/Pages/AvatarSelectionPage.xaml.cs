using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using EasyLayout.Forms;
using FormsControls.Base;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using BindableObjectExtensions = Xamarin.Forms.Markup.BindableObjectExtensions;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class AvatarSelectionPage  :  BasePage<AvatarSelectionPageViewModel>, IAnimationPage
    {
   
        public AvatarSelectionPage()
        {
            InitializeComponent();
                 
            this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);

            var service = Locator.Current.GetService<IApplicationService>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = Device.RuntimePlatform == Device.iOS ? (int) service.StatusbarHeight : 0;
                var navHeight = (int) service.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                var size = service.ScreenSize;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);
 
                MainLayout.ConstrainLayout(() => 
                    NavigationView.Right() == MainLayout.Right() &&
                    NavigationView.Left() == MainLayout.Left() &&
                    NavigationView.Top() == MainLayout.Top() &&
                    NavigationView.Height() == totalHeight.ToConst()
                );

                var listTop = totalHeight;// + 20;

                var bottomOffset = (int)service.GetSafeAreaInsets().Bottom;
                int buttonMargin = bottomOffset > 0 ? bottomOffset : 20;

                var listBottomMargin = 60 + buttonMargin + listTop + 10;

                MainLayout.ConstrainLayout(() => 
                    AvatarList.Right() == MainLayout.Right() -20 &&
                    AvatarList.Left() == MainLayout.Left() + 20 &&
                    AvatarList.Top() == MainLayout.Top() + listTop.ToConst() &&
                    AvatarList.Bottom() == MainLayout.Bottom() - listBottomMargin.ToConst()
                );
               
                MainLayout.ConstrainLayout(() => 
                    AddChildButton.Right() == MainLayout.Right() -20 &&
                    AddChildButton.Left() == MainLayout.Left() + 20 &&
                    AddChildButton.Bottom() == MainLayout.Bottom() - buttonMargin.ToConst() &&
                    AddChildButton.Height() == 60
                );
            });
                      
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);


            this.WhenActivated(d =>
                {
                    this.WhenAnyValue(m => m.ViewModel.LoadDataCommand)
                        .Select(m => Unit.Default)
                        .InvokeCommand(this, v => v.ViewModel.LoadDataCommand)
                        .DisposeWith(d);

                    
                    this.OneWayBind(ViewModel, v => v.Avatars, view => view.AvatarList.ItemsSource).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.ButtonText, view => view.AddChildButton.Text).DisposeWith(d);
                    //this.OneWayBind(ViewModel, v => v.IsBusy, view => view.AddChildButton.IsBusy).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.SaveCommand, view => view.AddChildButton).DisposeWith(d);

                    this.OneWayBind(ViewModel, v => v.SelectedItem, view => view.AvatarList.SelectedItem)
                        .DisposeWith(d);

                    this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);

                    //BindableObjectExtensions.Bind(CollectionView.SelectionChangedCommandProperty, nameof())
                    
                    this.AvatarList.Events()
                        .SelectionChanged
                        .Where(m => m.CurrentSelection != null && m.CurrentSelection.Count > 0)
                        .Select(m => m.CurrentSelection.FirstOrDefault())
                        .Select(m => (AvatarItemViewModel) m)
                        .Do(m => this.AvatarList.SelectedItems = null)
                        .InvokeCommand(this, v => v.ViewModel.SelectCommand)
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
