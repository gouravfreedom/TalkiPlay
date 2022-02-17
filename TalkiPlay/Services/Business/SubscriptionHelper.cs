using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ChilliSource.Mobile.Api;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TalkiPlay
{
    public static class SubscriptionHelper
    {
        static DateTime _lastSubscriptionVerificationTime;
        
        public static async Task ProcessPurchase(string productId)
        {
           
            Dialogs.ShowLoading();
            Bootstrapper.IsProcessingSubscription = true;
            
            var userService = Locator.Current.GetService<IUserRepository>();
            
            var purchaseResult = await SubscriptionService.PurchaseItem(userService, AppInfo.PackageName, productId);
            if (purchaseResult.IsSuccessful)
            {
                try
                {
                    //this is needed as a temporary workaround for https://github.com/xamarin/xamarin-macios/issues/6443
                    await Task.Delay(500);
                    
                    await SecureSettingsService.UpdateUserSubscriptionStatus(UserSubscriptionStatus.AppStore);
                    await userService.UpdateCompany(new CompanyPatchRequest(true, productId));
                    
                    MessageBus.Current.SendMessage(new SubscriptionChangedMessage());
                    Dialogs.HideLoading();
                    
                    Dialogs.Toast(Dialogs.BuildSuccessToast("Purchase completed successfully!"));
                    SimpleNavigationService.PopModalAsync().Forget();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    
                    Dialogs.HideLoading();
                    e.ShowExceptionDialog();
                }
            }
            else
            {
                Dialogs.HideLoading();
                purchaseResult.Exception.ShowExceptionDialog();
            }
            
            Bootstrapper.IsProcessingSubscription = false;
        }
        
        public static void VerifySubscriptionInBackground()
        {
            Task.Run(async () =>
            {
                await VerifySubscription();
            });
        }

        public static async Task VerifySubscription()
        {
            if (Xamarin.Essentials.DeviceInfo.DeviceType == DeviceType.Virtual)
            {
                return;
            }
            
            if ((DateTime.Now - _lastSubscriptionVerificationTime).TotalSeconds <= 5)
            {
                return;
            }

            if (Xamarin.Essentials.Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                return;
            }
            
            try
            {
                var user = await SecureSettingsService.GetUser();
                if (user == null)
                {
                    return;
                }
                
                var api = Locator.Current.GetService<IApi<ITalkiPlayApi>>();
                var userService = Locator.Current.GetService<IUserRepository>();
                    
                var company = await api.Client.GetCompany();
                if (company.HasAppStoreSubscription || !string.IsNullOrEmpty(company.AppStoreSubscriptionProductId))
                {
                    var verification = await SubscriptionService.VerifySubscription(userService, 
                        AppInfo.PackageName, company.AppStoreSubscriptionProductId);
                    if (verification.IsSuccessful)
                    {
                        Debug.WriteLine("Bootstrapper.VerifySubscription: subscription verification succeeded");
                        await SecureSettingsService.UpdateUserSubscriptionStatus(UserSubscriptionStatus.AppStore);
                        await userService.UpdateCompany(new CompanyPatchRequest(true, company.AppStoreSubscriptionProductId));
                    }
                    else
                    {
                        Debug.WriteLine("Bootstrapper.VerifySubscription: subscription verification failed");
                        await SecureSettingsService.UpdateUserSubscriptionStatus(UserSubscriptionStatus.None);
                        await userService.UpdateCompany(new CompanyPatchRequest(false, company.AppStoreSubscriptionProductId));
                    }
                    MessageBus.Current.SendMessage(new SubscriptionChangedMessage());
                    Debug.WriteLine("Bootstrapper.VerifySubscription: completed");
                }
                    
            }
            catch (Exception e)
            {
                Serilog.Log.Error(e, e.Message);
            }

            _lastSubscriptionVerificationTime = DateTime.Now;
        }
    }
}