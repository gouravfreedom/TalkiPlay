using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    public partial class DevicePage : BasePage<DevicePageViewModel>, 
        IAnimationPage
    {
     
        public DevicePage()
        {
            InitializeComponent();

            DeviceInfoList.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
            DeviceInfoList.ItemTemplate = new DeviceInfoCellTemplateSelector();;
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
            
            
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);

        
            this.WhenActivated(d =>
                {
                    this.WhenAnyValue(m => m.ViewModel.AskStoragePermissionCommand)
                        .Select(_ => Unit.Default)
                        .InvokeCommand(this, v => v.ViewModel.AskStoragePermissionCommand)
                        .DisposeWith(d);
                 
                    this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.ConnectCommand, view => view.NavigationView.RightButton).DisposeWith(d);

                    this.OneWayBind(ViewModel, v => v.DeviceInfoList, view => view.DeviceInfoList.ItemsSource).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.IsConnected, view => view.NavigationView.RightButtonIcon, 
                            vmToViewConverterOverride: new BooleanToObjectBindingConverter<ImageSource>
                        {
                            TrueObject = ImageSource.FromFile(Images.BleDisConnectIcon),
                            FalseObject =  ImageSource.FromFile(Images.BleConnectIcon)
                        } )
                        .DisposeWith(d);

                      
                    this.DeviceInfoList.Events()
                        .ItemSelected
                        .Where(m => m.SelectedItem != null)
                        .Select(m => (DeviceInfoBaseViewModel) m.SelectedItem)
                        .Do(m => this.DeviceInfoList.SelectedItem = null)
                        .Where(m => m is CheckForUpdateViewModel)
                        .InvokeCommand(this, v => v.ViewModel.UpdateCommand)
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

        public IPageAnimation PageAnimation =>   this.ViewModel?.PageAnimation ?? new SlidePageAnimation()
        {
            BounceEffect = false,
            Subtype =  AnimationSubtype.FromBottom,
            Duration = AnimationDuration.Short
        };
    }
}
