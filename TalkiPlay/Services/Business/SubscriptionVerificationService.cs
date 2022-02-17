using System.Diagnostics;
using System.Threading.Tasks;
using ChilliSource.Core.Extensions;
using Plugin.InAppBilling.Abstractions;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class SubscriptionVerificationService : IInAppBillingVerifyPurchase
    {
        private readonly IUserRepository _userService;
        private readonly string _bundleId;
        private readonly string _productId;
        public SubscriptionVerificationService(IUserRepository userService, string bundleId, string productId)
        {
            _userService = userService;
            _bundleId = bundleId;
            _productId = productId;
        }
        public async Task<bool> VerifyPurchase(string signedData, string signature, string productId = null,
            string transactionId = null)
        {
            Debug.WriteLine("================================================");
            Debug.WriteLine("VerifyPurchase: signedData: " + signedData);
            Debug.WriteLine("VerifyPurchase: bundleId: " + _bundleId);
            Debug.WriteLine("================================================");
            
            if (Device.RuntimePlatform == Device.Android)
            {
                var response = signedData.FromJson<AndroidPurchaseVerificationData>();
                var receipt = new GoogleReceipt(response.ProductId, _bundleId, response.DeveloperPayload,
                    response.PurchaseToken);
                
                var result = await _userService.VerifyGoogleSubscription(receipt);
                if (!result.IsSuccessful)
                {
                    Debug.WriteLine("SubscriptionVerificationService.VerifyPurchase: " +
                                    result.ErrorMessage);
                }
                return result.IsSuccessful;
            }
            else
            {
                var result = await _userService.VerifyAppleSubscription(new AppleReceipt(_productId, _bundleId, "",
                    signedData));
                return result;
            }
            
        }
    }
}