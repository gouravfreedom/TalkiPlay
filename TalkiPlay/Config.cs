using System;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public static class Config
    {
        public const string AppName = "TalkiPlay";
        public static readonly string DeviceNamePrefix = Constants.DeviceId;
        public const string AssetFolderName = "assets";
        public const string EmailAppLinkPath = "EmailRedirect";
        public const string InviteSource = "InviteUser";
        public const string ResetPasswordSource = "ResetPassword";

        public const string HelpUrl = "https://www.talkiplay.com/faq";
        public const string QRCodeSheetUrl = "https://www.talkiplay.com/qr-codes";
        public const string PrivacyPolicyUrl = "https://www.talkiplay.com/policies";
        public const string TermsUrl = "https://www.talkiplay.com/terms";
        public const string PurchaseLink = "https://store.talkiplay.com/parents/";
        public const string WaitLink = "https://www.talkiplay.com/talkiplayer-go";


#if DEV
        public const string Environment = "Development";
        public const string Domain = "talkiplay-stg.azurewebsites.net";
        //public const string Domain = "app.talkiplay.com";         
        public const string WebUrl = "";
        public const string ApiKey = "14CC9AA4-43A9-4C25-9320-EE5B2A4A59EF";        
        public const bool IsInTestMode = false;
        //public static EnvironmentType EnvironmentType = EnvironmentType.Dev;
        public const string AppCenterSecretiOS = "2b45da53-0199-4f74-bdc4-04f33fa43144";
        public const string AppCenterSecretAndroid = "8d2e592d-2563-4665-93ea-e71f12db979f"; //"39963542-129c-4022-bd11-e5befbbd7cb3";
        public const bool PlayMusic = false;
#endif

#if STAGE
        public const string Environment = "Staging";
        public const string Domain = "talkiplay-stg.azurewebsites.net";          
        public const string WebUrl = "";        
        public const string ApiKey = "14CC9AA4-43A9-4C25-9320-EE5B2A4A59EF";       
        public const bool IsInTestMode = false;
        //public static EnvironmentType EnvironmentType = EnvironmentType.Staging;
        public const string AppCenterSecretiOS = "8f6a888b-415d-4d2f-b755-6e1879c1bb78";// "2b45da53-0199-4f74-bdc4-04f33fa43144";
        public const string AppCenterSecretAndroid = "8d2e592d-2563-4665-93ea-e71f12db979f";// "39963542-129c-4022-bd11-e5befbbd7cb3";
        public const bool PlayMusic = true;
#endif

#if PROD
        public const string Environment = "Production";
        public const string Domain = "app.talkiplay.com";         
        public const string WebUrl = "";
        public const string ApiKey = "14CC9AA4-43A9-4C25-9320-EE5B2A4A59EF";       
        public const bool IsInTestMode = false;
        //public static EnvironmentType EnvironmentType = EnvironmentType.Production;
        public const string AppCenterSecretiOS = "158ae2dd-55f2-44ed-92a9-72ff65c4509c";// "f61f007d-aa88-4c17-aef8-3706c99e309e";
        public const string AppCenterSecretAndroid = "9ff887ec-b4e1-4967-b4e8-ac7b721bb9f1";// "912c9b4d-0e51-4a32-a06b-beb733647ed6";
        public const bool PlayMusic = true;
#endif

        //public static string AppLink = Domain;
        public static readonly string BaseUrl = $"https://{Domain}";
        public static readonly string ApiUrl = $"{BaseUrl}/api";
        public static readonly string FirmwareDownloadUrl = $"{ApiUrl}/v1/firmware/download";

        public static string GetAppCenterSecret()
        {
                switch (Device.RuntimePlatform)
                {
                        case Device.iOS: return AppCenterSecretiOS;
                        case Device.Android: return AppCenterSecretAndroid;
                        default: return null;
                }
        }
    }

    public class Configuration : IConfig
    {
        public string DeviceNamePrefix => Config.DeviceNamePrefix;
        public string GetAssetDownloadUrl(int id) => $"{Config.ApiUrl}/v1/assets/{id}";
        public string ApiKey => Config.ApiKey;
    }
}