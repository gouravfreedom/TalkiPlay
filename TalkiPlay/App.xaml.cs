using System;
using System.Linq;
using System.Threading.Tasks;
using ChilliSource.Mobile.UI.ReactiveUI;
using Microsoft.AspNetCore.WebUtilities;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
[assembly: ExportFont("fontawesome-regular.ttf", Alias = "FontAwesomeRegular")]
[assembly: ExportFont("fontawesome-solid.ttf", Alias = "FontAwesomeSolid")]

namespace TalkiPlay
{
    public partial class App : Application
    {
        

        public App()
        {
            InitializeComponent();
            Sharpnado.Tabs.Initializer.Initialize(false, false);
            Sharpnado.Shades.Initializer.Initialize(loggerEnable: true);

            Bootstrapper.Init();            
            SetMainPage().Forget();
        }

        async Task SetMainPage()
        {
            MainPage = await Bootstrapper.GetMainPage();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            System.Diagnostics.Debug.WriteLine("Sleeping...");
            
           Bootstrapper.StopMusic();
        }

        protected override void OnResume()
        {
            base.OnResume();
           // Bootstrapper.LoadData();
           System.Diagnostics.Debug.WriteLine("Resuming...");

           var settings = Locator.Current.GetService<IUserSettings>();
           if (settings.IsOnboarded)
           {
               Bootstrapper.PlayMusic();
           }

           if (!Bootstrapper.IsProcessingSubscription )
           {
               Bootstrapper.SyncUserDetails();
               SubscriptionHelper.VerifySubscriptionInBackground();                
           }
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);
            var urlPath = uri.Segments.LastOrDefault();

            if (!String.IsNullOrWhiteSpace(urlPath) && urlPath.Equals(Config.EmailAppLinkPath))
            {
                var queryString = QueryHelpers.ParseQuery(uri.Query);

                if (queryString.ContainsKey("Url"))
                {
                    var url = queryString["Url"];
                    var redirectUrl = new Uri(url);

                    var redirectUrlQueryString = QueryHelpers.ParseQuery(redirectUrl.Query);

                    if (redirectUrlQueryString.ContainsKey("Token") && redirectUrlQueryString.ContainsKey("Email"))
                    {
                        var source = queryString["utm_source"];
                        
                        var token = redirectUrlQueryString["Token"];
                        var email = redirectUrlQueryString["Email"];
                        var navigator = Locator.Current.GetService<INavigationService>(Constants.MainNavigation);
                        if (source == Config.InviteSource)
                        {
                              navigator.PushPage(new AcceptInvitePageViewModel(new TokenRequest()
                                {
                                    Email = email,
                                    Token = token
                                }, navigator), resetStack: true, animate: false)
                                .SubscribeSafe();
                        }
                        else if (source == Config.ResetPasswordSource)
                        {
                            navigator.PushPage(new ResetPasswordPageViewModel(new TokenRequest()
                                {
                                    Email = email,
                                    Token = token
                                }, navigator), resetStack: true, animate: false)
                                .SubscribeSafe();
                        }
                        
                      
                    }
                }
            }
        }
    }
}
