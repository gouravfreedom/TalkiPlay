using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChilliSource.Mobile.Core;
using Humanizer;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Abstractions = Plugin.InAppBilling.Abstractions;

namespace TalkiPlay.Shared
{
    //https://devblogs.microsoft.com/xamarin/integrating-in-app-purchases-in-mobile-apps/
    //https://github.com/jamesmontemagno/InAppBillingPlugin
    //https://jamesmontemagno.github.io/InAppBillingPlugin/PurchaseSubscription.html

    public enum SubscriptionType
    {
        Monthly,
        Yearly
    }
    
    public static class SubscriptionService
    {
        const string PremiumYearlyProductId = "io.talkiplay.TalkiPlay.PremiumYearly";
        const string PremiumMonthlyProductId = "io.talkiplay.TalkiPlay.PremiumMonthly";

        static readonly Abstractions.ItemType SubscriptionItemType = Abstractions.ItemType.Subscription;

        public static string GetSubscriptionProductId(SubscriptionType type)
        {
            switch (type)
            {
                case SubscriptionType.Monthly:
                {
                    return Device.RuntimePlatform == Device.Android ? PremiumMonthlyProductId.ToLower() : PremiumMonthlyProductId;
                }
                case SubscriptionType.Yearly:
                {
                    return Device.RuntimePlatform == Device.Android ? PremiumYearlyProductId.ToLower() : PremiumYearlyProductId;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static async Task<bool> GetUserHasSubscription()
        {
            #if DEV
            return true;
            #endif
            
            if (Xamarin.Essentials.DeviceInfo.DeviceType == DeviceType.Virtual)
            {
                return true;
            }
            
            var user = await SecureSettingsService.GetUser();
            return user.SubscriptionStatus != UserSubscriptionStatus.None;
        }
        
        public static async Task<OperationResult<IEnumerable<InAppBillingProduct>>> GetProducts()
        {
            var billing = CrossInAppBilling.Current;
            var productIds = new string []
            {
                GetSubscriptionProductId(SubscriptionType.Yearly),
                GetSubscriptionProductId(SubscriptionType.Monthly)
            };
            
            try
            {
                var connectionResult = await Connect();
                if (!connectionResult.IsSuccessful)
                {
                    return OperationResult<IEnumerable<InAppBillingProduct>>.AsFailure(connectionResult.Message);
                }
                
                var items = await billing.GetProductInfoAsync(SubscriptionItemType, productIds);

                return OperationResult<IEnumerable<InAppBillingProduct>>.AsSuccess(items);
                
            }
            catch (InAppBillingPurchaseException pEx)
            {
                return OperationResult<IEnumerable<InAppBillingProduct>>.AsFailure(pEx);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<InAppBillingProduct>>.AsFailure(ex);
            }
            finally
            {    
                await billing.DisconnectAsync();
            }
        }
        
        public static async Task<OperationResult> PurchaseItem(IUserRepository repository, string bundleId, string productId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(bundleId) || repository == null)
            {
                return OperationResult.AsFailure("Invalid parameters");
            }
            
            var payload = Guid.NewGuid().ToString();
            
            var billing = CrossInAppBilling.Current;
            try
            {
                var connectionResult = await Connect();
                if (!connectionResult.IsSuccessful)
                {
                    return OperationResult.AsFailure(connectionResult.Message);
                }
                
                //check purchases
                var purchase = await billing.PurchaseAsync(productId,
                    Plugin.InAppBilling.Abstractions.ItemType.Subscription, payload, 
                    new ReceiptVerificationService(repository,bundleId));

                //possibility that a null came through.
                if (purchase == null)
                {
                    Debug.WriteLine("SubscriptionService.PurchaseItem failure");
                    return OperationResult.AsFailure("Purchase could not be processed. Please try again later.");
                    //did not purchase
                }
                else
                {
                    Debug.WriteLine("SubscriptionService.PurchaseItem success");
                    return OperationResult.AsSuccess();
                }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                
                //Billing Exception handle this based on the type
                Debug.WriteLine("PurchaseItem: InAppBillingPurchaseException: " + purchaseEx);
                Debug.WriteLine("PurchaseItem: " + purchaseEx.PurchaseError);
                return OperationResult.AsFailure(purchaseEx.Message + " - " + purchaseEx.PurchaseError.Humanize());

            }
            catch (Exception ex)
            {
                //Something else has gone wrong, log it
                Serilog.Log.Error(ex, ex.Message);
                Debug.WriteLine("PurchaseItem: Exception: " + ex);
                return OperationResult.AsFailure(ex);
            }
            finally
            {
                await billing.DisconnectAsync();
            }
        }

        public static async Task<OperationResult<string>> RestoreSubscriptions(IUserRepository userService, string bundleId)
        {
            var subscriptions = new List<string>()
            {
                GetSubscriptionProductId(SubscriptionType.Yearly), 
                GetSubscriptionProductId(SubscriptionType.Monthly)
            };

            foreach (var subscription in subscriptions)
            {
                var subscriptionVerification = await VerifySubscription(userService, 
                    bundleId, subscription);
                if (subscriptionVerification.IsSuccessful)
                {
                    return OperationResult<string>.AsSuccess(subscription);
                }
            }
            
            return OperationResult<string>.AsFailure("No active subscriptions found.");
        }
        
        public static async Task<OperationResult> VerifySubscription(IUserRepository userService, string bundleId, string productId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(bundleId) || userService == null)
            {
                return OperationResult.AsFailure("Invalid parameters");
            }
            
            var billing = CrossInAppBilling.Current;
            try
            { 
                var connectionResult = await Connect();
                if (!connectionResult.IsSuccessful)
                {
                    return OperationResult.AsFailure(connectionResult.Message);
                }

                //check purchases
                var purchases = await billing.GetPurchasesAsync(SubscriptionItemType, 
                    new SubscriptionVerificationService(userService, bundleId, productId));

                //check for null just incase
                if(purchases?.Any(p => p.ProductId == productId) ?? false)
                {
                    //Purchase restored
                    Debug.WriteLine("SubscriptionService.VerifySubscription success");
                    return OperationResult.AsSuccess();
                }
                else
                {
                    //no purchases found
                    Debug.WriteLine("SubscriptionService.VerifySubscription failure");
                    return OperationResult.AsFailure("No valid purchases found");
                }
            }    
            catch (InAppBillingPurchaseException purchaseEx)
            {
                //Billing Exception handle this based on the type
                Debug.WriteLine("VerifyPurchase: InAppBillingPurchaseException: " + purchaseEx);
                Debug.WriteLine("VerifyPurchase: " + purchaseEx.PurchaseError);
                return OperationResult.AsFailure(purchaseEx.Message + " - " + purchaseEx.PurchaseError.Humanize());
            }
            catch (Exception ex)
            {
                //Something has gone wrong
                Serilog.Log.Error(ex, ex.Message);
                Debug.WriteLine("VerifyPurchase: Exception: " + ex);
                return OperationResult.AsFailure(ex);
                
            }
            finally
            {    
                await billing.DisconnectAsync();
            }
        }
        
        static async Task<OperationResult> Connect()
        {
            var connected = await CrossInAppBilling.Current
                .ConnectAsync(SubscriptionItemType);

            if (!connected)
            {
                return OperationResult
                    .AsFailure("Could not connect to billing service. Please try again later.");
            }

            return OperationResult.AsSuccess();
        }
    }
}