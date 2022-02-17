using System.Windows.Input;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class SubscriptionTermsPageViewModel : SimpleBasePageModel
    {
        private readonly string _productId;
        
        public SubscriptionTermsPageViewModel(string productId)
        {
            SetupCommands();
            _productId = productId;
            ShowContinueButton = _productId != null;
        }
        
        public ICommand ContinueCommand { get; set; }
        
        public bool ShowContinueButton { get; set; }
        
        public ICommand BackCommand { get; set; }

        public string Text => Constants.SubscriptionLegalText;
        
        void SetupCommands()
        {
            ContinueCommand = new Command(() =>
            {
                SubscriptionHelper.ProcessPurchase(_productId).Forget();
                
            });
            
            BackCommand = new Command(() =>
            {
                SimpleNavigationService.PopAsync().Forget();
            });
        }
    }
}