using System.Windows.Input;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class LegalSubscriptionInfoPageViewModel : SimpleBasePageModel
    {
        public LegalSubscriptionInfoPageViewModel()
        {
            BackCommand = new Command(() => SimpleNavigationService.PopAsync().Forget());
        }
        
        public ICommand BackCommand { get; }
        public string Text => Constants.SubscriptionLegalText;
        
    }
}