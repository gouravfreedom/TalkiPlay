using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using EasyLayout.Forms;
using FormsControls.Base;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public partial class TestBleRequestSelectionPage :  BasePage<TestBleRequestPageViewModel>, IAnimationPage
    {
   
        public TestBleRequestSelectionPage()
        {
            InitializeComponent();
                 
            this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            UploadTypeList.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
            UploadTypeList.On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsFastScrollEnabled(false);
            UploadTypeList.ItemTemplate = new DataTemplate(typeof(TestBleRequstItemView));// new TestBleRequestItemTemplateSelector();
            var service = Locator.Current.GetService<IApplicationService>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var barHeight = Device.RuntimePlatform == Device.iOS ? (int) service.StatusbarHeight : 0;
                var navHeight = (int) service.NavBarHeight;
                var totalHeight = barHeight + navHeight;
                //var size = service.ScreenSize;
                NavRow.Height = totalHeight;
                NavigationView.Padding = Dimensions.NavPadding(barHeight);
 
                //MainLayout.ConstrainLayout(() => 
                //    NavigationView.Right() == MainLayout.Right() &&
                //    NavigationView.Left() == MainLayout.Left() &&
                //    NavigationView.Top() == MainLayout.Top() &&
                //    NavigationView.Height() == totalHeight.ToConst()
                //);

                //var listTop = totalHeight;

                //MainLayout.ConstrainLayout(() => 
                //    UploadTypeList.Right() == MainLayout.Right() &&
                //    UploadTypeList.Left() == MainLayout.Left()&&
                //    UploadTypeList.Top() == MainLayout.Top() + listTop.ToConst() &&
                //    UploadTypeList.Bottom() == MainLayout.Bottom()
                
                //);

                //int gridBottomPadding = 10 + (int)service.GetSafeAreaInsets(false).Bottom;

                //MainLayout.ConstrainLayout(() => 
                //    Grid.Right() == MainLayout.Right() &&
                //    Grid.Left() == MainLayout.Left()  &&
                //    Grid.Bottom() == MainLayout.Bottom() - gridBottomPadding.ToConst() &&
                //    Grid.Height() == 130
                //);

            });
          

            
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);


            this.WhenActivated(d =>
                {
                
                    this.OneWayBind(ViewModel, v => v.Data, view => view.UploadTypeList.ItemsSource).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.SelectJsonFileCommand, view => view.SelectJsonFileButton.Button).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.SelectFileCommand, view => view.SelectFileButton.Button).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.UploadCommand, view => view.SendButton.Button).DisposeWith(d);

                    this.OneWayBind(ViewModel, v => v.Title, view => view.NavigationView.Title).DisposeWith(d);
                    this.BindCommand(ViewModel, v => v.BackCommand, view => view.NavigationView.LeftButton).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.BackCommand, view => view.BackButtonPressed).DisposeWith(d);
                    this.OneWayBind(ViewModel, v => v.IsConnected, view => view.NavigationView.RightButtonIcon, 
                            vmToViewConverterOverride: new BooleanToObjectBindingConverter<ImageSource>
                            {
                                TrueObject = ImageSource.FromFile(Images.BleDisConnectIcon),
                                FalseObject =  ImageSource.FromFile(Images.BleConnectIcon)
                            } )
                        .DisposeWith(d);
                    
                    this.BindCommand(ViewModel, v => v.ConnectCommand, view => view.NavigationView.RightButton).DisposeWith(d);

                    this.UploadTypeList.Events()
                        .ItemSelected
                        .Select(m => m.SelectedItem)
                        .Where(m => m != null)
                        .Where(m => ! (m is IEmptyItemViewModel))
                        .Select(m => (ItemSelectionViewModel) m)
                        .Do(m => this.UploadTypeList.SelectedItem = null)
                        .InvokeCommand(this, v => v.ViewModel.SelectCommand)
                        .DisposeWith(d);
                    
                    this.WhenAnyObservable(v => v.ViewModel.UploadCommand.CanExecute)
                        .SubscribeSafe(mn =>
                        {
                            this.SendButton.ButtonColor = mn ? Colors.TealColor : Colors.WarmGrey;
                        })
                        .DisposeWith(d);

                    var bottomOffset = (int)service.GetSafeAreaInsets().Bottom;

                    Grid.Margin = new Thickness(0, 10, 0, 10 + bottomOffset);
                    ButtonRow.Height = 150 + bottomOffset;
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
