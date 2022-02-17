using TalkiPlay.Shared;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TalkiPlay.Managers
{
    public static class WebpageHelper
    {
        public static void OpenUrl(string url, string title)
        {
            // if (Device.RuntimePlatform == Device.iOS)
            // {
            //     Xamarin.Essentials.Browser.OpenAsync(url, BrowserLaunchMode.External);
            // }
            // else
            {
                SimpleNavigationService.PushAsync(new WebPageViewModel(
                    url, title)).Forget();
            }
        }
    }
}