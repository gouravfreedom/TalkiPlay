using System.Diagnostics;
using System.Threading.Tasks;
using ChilliSource.Core.Extensions;
using Plugin.InAppBilling.Abstractions;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class ReceiptVerificationService : IInAppBillingVerifyPurchase
    {
        private readonly IUserRepository _userService;
        private readonly string _bundleId;
        public ReceiptVerificationService(IUserRepository userService, string bundleId)
        {
            _bundleId = bundleId;
            _userService = userService;
        }
        public async Task<bool> VerifyPurchase(string signedData, string signature, string productId = null,
            string transactionId = null)
        {
            Debug.WriteLine("================================================");
            Debug.WriteLine("VerifyPurchase: signedData: " + signedData);
            Debug.WriteLine("VerifyPurchase: productId: " + productId);
            Debug.WriteLine("VerifyPurchase: transactionId: " + transactionId);
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
                    Debug.WriteLine("ReceiptVerificationService.VerifyPurchase: " +
                                    result.ErrorMessage);
                }
                return result.IsSuccessful;
            }
            else
            {
                var receipt = new AppleReceipt(productId, _bundleId, transactionId,
                    signedData);
            
                //this is needed as a temporary workaround for https://github.com/xamarin/xamarin-macios/issues/6443
                await Task.Delay(500);
            
                return await _userService.VerifyAppleReceipt(receipt);
            }
        }
    }
}