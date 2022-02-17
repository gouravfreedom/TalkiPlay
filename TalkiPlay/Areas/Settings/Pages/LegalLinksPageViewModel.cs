using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using TalkiPlay.Managers;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    
    public class LegalLinksPageViewModel : BasePageViewModelEx
    {
        public LegalLinksPageViewModel()
        {
            SetupCommands();
            LoadData();
        }


        public ReactiveCommand<SettingsItemViewModel, Unit> SelectCommand { get; private set; }
        public List<SettingsItemViewModel> Items { get; private set; }

        public override string Title => "Legal Info";

        private void LoadData()
        {
            Items = new List<SettingsItemViewModel>();

            if (Device.RuntimePlatform == Device.iOS)
            {
                Items.Add(new SettingsItemViewModel()
                {
                    Label = $"Subscription Details",
                    Type = SettingsType.LegalSubscription
                });
            }

            Items.Add(new SettingsItemViewModel()
            {
                Label = $"Privacy Policy",
                Type = SettingsType.LegalPrivacy
            });

            Items.Add(new SettingsItemViewModel()
            {
                Label = $"Terms & Conditions",
                Type = SettingsType.LegalTerms
            });
        }


        private void ProcessSelection(SettingsItemViewModel vm)
        {
            switch (vm.Type)    
            {
                case SettingsType.LegalSubscription:
                {
                    SimpleNavigationService.PushAsync(new LegalSubscriptionInfoPageViewModel()).Forget();
                    break;
                }
                case SettingsType.LegalPrivacy:
                {
                    WebpageHelper.OpenUrl(Config.PrivacyPolicyUrl, "Privacy Policy");
                    break;
                }
                case SettingsType.LegalTerms:
                {
                    WebpageHelper.OpenUrl(Config.TermsUrl, "Terms of Use");
                    break;
                }
               
            }
        }
        
        private void SetupCommands()
        {
            SelectCommand = ReactiveCommand.CreateFromObservable<SettingsItemViewModel, Unit>(m =>
            {
                ProcessSelection(m);
                return Observable.Return(Unit.Default);
            });
        }

        
    }
}