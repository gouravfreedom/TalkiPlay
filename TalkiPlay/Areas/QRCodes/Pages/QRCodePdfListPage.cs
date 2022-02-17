using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI;
using FormsControls.Base;
using ReactiveUI;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Acr.UserDialogs;
using Splat;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    public class QRCodePdfListPage : BasePage<QRCodePdfListPageViewModel>
    {
        public QRCodePdfListPage()
        {
            //BackgroundColor = Color.White;


            Device.BeginInvokeOnMainThread(BuildContent);

            // this.WhenActivated(d =>
            // {
            //     this.WhenAnyValue(m => m.ViewModel.LoadDataCommand)
            //         .Select(m => Unit.Default)
            //         .InvokeCommand(this, v => v.ViewModel.LoadDataCommand)
            //         .DisposeWith(d);
            // });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadData().Forget();
        }

        void BuildContent()
        {
            var barHeight = DeviceInfo.StatusbarHeight;
            var navHeight = DeviceInfo.NavBarHeight;
            var totalHeight = barHeight + navHeight;

            var navView = new NavigationView
            {
                BarTintColor = Colors.NavColor1,
                ShowLeftButton = true,
                
                ShowRightButton = false,
                Title = "QR Codes",                
                
                Padding = Dimensions.NavPadding(barHeight)
            };
            navView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(ViewModel.BackCommand));

            var emptyView = new StackLayout
            {
                Margin = 40,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.Center
            };
            emptyView.Children.Add(new ExtendedLabel
            {
                Margin = 40,
                Text = "No QR Code files found.",
                CustomFont = Fonts.LabelBlackFont,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            });

            var collectionView = new CollectionView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                SelectionMode = SelectionMode.None,
                ItemsLayout = new GridItemsLayout(ItemsLayoutOrientation.Vertical)
                {
                    Span = 1,                   
                },
                ItemTemplate = new DataTemplate(typeof(QRCodePdfView)),
                EmptyView = emptyView,
                Footer = new StackLayout(){HeightRequest = 60}
            };
            collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(QRCodePdfListPageViewModel.Items));

            var grid = new Grid()
            {
                RowSpacing = 0,                
            };

            grid.RowDefinitions.Add(new RowDefinition() { Height = totalHeight });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });

            grid.Children.Add(navView);
            grid.Children.Add(collectionView, 0, 1);

            Content = grid;
        }
    }
}
