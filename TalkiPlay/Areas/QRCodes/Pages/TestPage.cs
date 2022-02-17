using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace TalkiPlay
{
    public class TestPage : ContentPage
    {
        Label _label;
        ZXingScannerView _scannerView;

        public TestPage()
        {
            BuildContent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _scannerView.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            _scannerView.IsScanning = false;

            base.OnDisappearing();
        }

        void BuildContent()
        {
            _scannerView = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,

            };
            _scannerView.HeightRequest = Device.Idiom == TargetIdiom.Tablet ? 600 : 300;
            _scannerView.WidthRequest = Device.Idiom == TargetIdiom.Tablet ? 600 : 300;

            _scannerView.Options.UseNativeScanning = true;
            _scannerView.Options.DelayBetweenAnalyzingFrames = 5;
            _scannerView.Options.DelayBetweenContinuousScans = 5;
            _scannerView.OnScanResult += OnScanResult;

            _label = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Test"
            };

            var mainLayout = new StackLayout
            {
                Spacing = 20,
                Margin = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children = { _scannerView, _label }

            };

            Content = mainLayout;
        }

        private void OnScanResult(ZXing.Result result)
        {
            Debug.WriteLine(result.Text);

            Device.BeginInvokeOnMainThread(() =>
            {
                _label.Text = result.Text;
            });
            

            //Task.Run(async () =>
            //{
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        _scannerView.IsAnalyzing = false;
            //    });

            //    await ViewModel.ProcessQRCode(result.Text);

            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        _scannerView.IsAnalyzing = true;
            //    });
            //});

            // Device.BeginInvokeOnMainThread(async () =>
            // {
            //     _scannerView.IsAnalyzing = false;
            //     
            //     await ViewModel.ProcessQRCode(result.Text);         
            //     _scannerView.IsAnalyzing = true;
            // });            
        }

    }
}
