using System;
using System.Collections.Generic;
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
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class RoomImageSelectionPage  :  BasePage<RoomImageSelectionPageViewModel>, IAnimationPage
    {
   
        public RoomImageSelectionPage()
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
                    ImageList.Right() == MainLayout.Right() -10  &&
                    ImageList.Left() == MainLayout.Left() + 10 &&
                    ImageList.Top() == MainLayout.Top() + listTop.ToConst() &&
                    ImageList.Bottom() == MainLayout.Bottom() - listBottomMargin.ToConst()
                );

               

                MainLayout.ConstrainLayout(() => 
                    AddRoomButton.Right() == MainLayout.Right() -20 &&
                    AddRoomButton.Left() == MainLayout.Left() + 20 &&
                    AddRoomButton.Bottom() == MainLayout.Bottom() - buttonMargin.ToConst() &&
                    AddRoomButton.Height() == 60
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

                    
                    this.OneWayBind(ViewModel, v => v.Images, view => view.ImageList.ItemsSource).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.ButtonText, view => view.AddRoomButton.Text).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.IsBusy, view => view.AddRoomButton.IsBusy).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.SaveCommand, view => view.AddRoomButton.Button).DisposeWith(d);

                    this.OneWayBind(ViewModel, v => v.SelectedItem, view => view.ImageList.SelectedItem)
                        .DisposeWith(d);

                    this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);

                    
                    this.ImageList.Events()
                        .SelectionChanged
                        .Where(m => m.CurrentSelection != null && m.CurrentSelection.Count > 0)
                        .Select(m => m.CurrentSelection.FirstOrDefault())
                        .Select(m => (RoomImageItemViewModel) m)
                        .Do(m => this.ImageList.SelectedItems = null)
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
