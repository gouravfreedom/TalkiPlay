using System;
using System.Windows.Input;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class WebPageViewModel : SimpleBasePageModel
    {
        public WebPageViewModel(string url, string title)
        {
            
            Source = url;
            Title = title;
            BackCommand = new Command(() => SimpleNavigationService.PopAsync().Forget());
            
            //ShowBackButton = showBackButton;
        }

        //public bool ShowBackButton { get; }

        public ICommand BackCommand { get; }
        
        public string Title { get; }
        
        public WebViewSource Source { get; }
        
    }
}
