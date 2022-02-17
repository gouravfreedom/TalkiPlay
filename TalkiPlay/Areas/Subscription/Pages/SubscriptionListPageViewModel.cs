using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Splat;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class SubscriptionListPageViewModel : SimpleBasePageModel
    {
        public SubscriptionListPageViewModel()
        {
            SetupCommands();
        }

        public ObservableCollection<ButtonViewModel> Items { get; private set; }

        public bool ShowEmptyState { get; private set; }
        
        public ICommand BackCommand { get; set; }
        
        public ICommand RestoreCommand { get; set; }

        public string HeaderText => "Premium Subscriptions";

        public string EmptyStateText => "No subscriptions found. Please try again later.";

        //public bool HasItems => Items?.Count > 0;
        
        public async Task LoadData()
        {
            Items = new ObservableCollection<ButtonViewModel>();

            // if (Device.RuntimePlatform == Device.Android)
            // {
            //     Items.Add(new ButtonViewModel("1", "$50", null));
            //     return;
            // }

            Dialogs.ShowLoading();

            var productsResult = await SubscriptionService.GetProducts();
            
            if (productsResult.IsSuccessful && productsResult.Result != null)
            {
                foreach (var product in productsResult.Result)
                {
                    string suffix = "";
                    if (product.ProductId == SubscriptionService.GetSubscriptionProductId(SubscriptionType.Monthly))
                    {
                        suffix = " / month";
                    }
                    else if (product.ProductId == SubscriptionService.GetSubscriptionProductId(SubscriptionType.Yearly))
                    {
                        suffix = " / year";
                    }
                    
                    
                    var text = $"{product.LocalizedPrice}{suffix}";
                    Items.Add(new ButtonViewModel(product.ProductId, text, HandleProductSelection));
                }
            }
            else
            {
                Dialogs.Toast(Dialogs.BuildErrorToast(productsResult.Message));
            }

            ShowEmptyState = Items.Count == 0;
            RaisePropertyChanged(nameof(ShowEmptyState));
            RaisePropertyChanged(nameof(Items));

            Dialogs.HideLoading();
        }

        void SetupCommands()
        {
            BackCommand = new Command(() =>
            {
                SimpleNavigationService.PopModalAsync().Forget();
            });
        }
        
        void HandleProductSelection(string productId)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                SubscriptionHelper.ProcessPurchase(productId).Forget();
            }
            else
            {
                SimpleNavigationService.PushAsync(new SubscriptionTermsPageViewModel(productId)).Forget();    
            }
        }
    }
}